import _, { forEach } from "lodash";
import { IBankAccountService } from "./IBankAccountService";
import { ICryptoService } from "./ICryptoService";
import { IOtherInvestmentService } from "./IOtherInvestmentService";
import { IPaymentService } from "./IPaymentService";
import { IStockService } from "./IStockService";
import { IComodityService } from "./IComodityService";
import moment, { Moment } from "moment";

export enum PaymentType {
    Revenue = 'Revenue',
    Expense = 'Expense'
}

const czkSymbol = "CZK";

export default class NetWorthService {
    private paymentService: IPaymentService;
    private stockService: IStockService;
    private cryptoService: ICryptoService;
    private otherInvestmentService: IOtherInvestmentService;
    private bankAccountService: IBankAccountService;
    private comodityService: IComodityService;

    constructor(paymentService: IPaymentService, stockService: IStockService, cryptoService: ICryptoService, otherInvestment: IOtherInvestmentService, bankAccount: IBankAccountService, comodityService: IComodityService) {
        this.paymentService = paymentService;
        this.stockService = stockService;
        this.cryptoService = cryptoService;
        this.otherInvestmentService = otherInvestment;
        this.bankAccountService = bankAccount;
        this.comodityService = comodityService;
    }

    async getCurrentNetWorth(): Promise<TotalNetWorthDetail> {
        const bankAccounts = await this.bankAccountService.getAllBankAccounts();
        const bankAccountBaseLine = _.sumBy(bankAccounts, s => s.openingBalance);
        let totalNetWorthDetail = new TotalNetWorthDetail();

        const limitDate = new Date(1970, 1, 1);
        const paymentHistory = await this.paymentService.getExactDateRangeDaysPaymentData(limitDate, undefined, undefined);
        const income = _.sumBy(paymentHistory.filter(p => p.paymentTypeCode == PaymentType.Revenue), s => s.amount);
        const expense = _.sumBy(paymentHistory.filter(p => p.paymentTypeCode == PaymentType.Expense), s => s.amount);

        totalNetWorthDetail.money = bankAccountBaseLine + income - expense;

        const otherInvestments = await this.otherInvestmentService.getSummary();
        const otherInvestmentsbalance = _.sumBy(otherInvestments.actualBalanceData, s => s.balance);

        totalNetWorthDetail.other = otherInvestmentsbalance;

        const cryptoSum = await this.cryptoService.getCryptoCurrentNetWorth(czkSymbol);
        totalNetWorthDetail.crypto = cryptoSum;

        const stockSum = await this.stockService.getStocksNetWorthSum(czkSymbol);
        totalNetWorthDetail.stock = stockSum;

        const comoditySum = await this.comodityService.getComodityNetWorth();
        totalNetWorthDetail.comodity = comoditySum;

        return totalNetWorthDetail;
    }

    async getNetWorthGroupedByMonth() {
        const fromDate = new Date(2020, 0, 1);
        const toDate = moment().toDate();
        const bankAccounts = await this.bankAccountService.getAllBankAccounts();
        const bankAccountBaseLine = _.sumBy(bankAccounts, s => s.openingBalance);

        const limitDate = new Date(1970, 1, 1);
        let paymentHistory = await this.paymentService.getExactDateRangeDaysPaymentData(limitDate, undefined, undefined);
        paymentHistory = paymentHistory.map(p => ({ ...p, amount: p.paymentTypeCode == PaymentType.Revenue ? p.amount : -p.amount }));
        const paymentGroupedData: NetWorthMonthGroupModel[] = [];

        _.chain(paymentHistory)
            .groupBy(s => moment(s.date).format('YYYY-MM'))
            .map((value, key) => ({ date: moment(key + "-1"), amount: _.sumBy(value, s => s.amount) }))
            .orderBy(f => f.date, ['asc'])
            .reduce((acc, model) => {
                const amount = acc.prev + model.amount + bankAccountBaseLine;
                paymentGroupedData.push({ date: model.date, amount: amount });
                acc.prev = amount - bankAccountBaseLine;
                return acc;
            }, { prev: 0 }).value();

        let tractRecordCollection = new NetWorthCollection(paymentGroupedData);

        const otherInvestments = await this.otherInvestmentService.getAll();
        const otherInvetmentsMonthlyGrouped = await this.otherInvestmentService.getMonthlyGroupedAccumulatedPayments(fromDate, toDate, otherInvestments);

        for (let monthTractRecord of otherInvetmentsMonthlyGrouped)
            tractRecordCollection.addTrackRecordSpotWithRecalculation(monthTractRecord);

        const tradeData = await this.cryptoService.getRawTradeData();
        const cryptoNetWorth = await this.cryptoService.getMonthlyGroupedAccumulatedCrypto(fromDate, toDate, tradeData, czkSymbol);
        for (let monthTractRecord of cryptoNetWorth)
            tractRecordCollection.addTrackRecordSpotWithRecalculation(monthTractRecord);
        console.log("🚀 ~ NetWorthService ~ getNetWorthGroupedByMonth ~ cryptoNetWorth:", cryptoNetWorth)

        const stockTradeData = await this.stockService.getStockTradeHistory();
        const acumulatedData = await this.stockService.getMonthlyGroupedAccumulated(fromDate, toDate, stockTradeData, czkSymbol);
        for (let monthTractRecord of acumulatedData)
            tractRecordCollection.addTrackRecordSpotWithRecalculation(monthTractRecord);
        console.log("🚀 ~ NetWorthService ~ getNetWorthGroupedByMonth ~ acumulatedData:", acumulatedData)

        const finalTractRecord = tractRecordCollection.getTractRecord();

        return finalTractRecord;
    }
}

class NetWorthCollection {
    private netWorthTrackRecord: NetWorthMonthGroupModel[];

    constructor(initNetWorthTrackRecord: NetWorthMonthGroupModel[]) {
        this.netWorthTrackRecord = initNetWorthTrackRecord;
    }

    public addTrackRecordSpot(trackSpot: NetWorthMonthGroupModel) {
        this.netWorthTrackRecord.push(trackSpot);
    }

    public addTrackRecordSpotWithRecalculation(trackRecord: NetWorthMonthGroupModel) {
        let tractRecordsBefore = _.filter(this.netWorthTrackRecord, r => r.date < trackRecord.date) ?? [];
        let tractRecordsAfter = _.filter(this.netWorthTrackRecord, r => r.date > trackRecord.date) ?? [];
        let existingMonthRecord = _.first(_.filter(this.netWorthTrackRecord, r => r.date == trackRecord.date));
        let currentMonthNethWorthModel: NetWorthMonthGroupModel = undefined;

        if (existingMonthRecord)
            currentMonthNethWorthModel = {
                date: moment(trackRecord.date.format('YYYY-MM') + '-1'),
                amount: currentMonthNethWorthModel.amount + trackRecord.amount
            };
        else {
            const lastAccumulatedNetWorthBeforeSpotToAdd = _.first(_.orderBy(tractRecordsBefore, r => r.date, 'desc'))?.amount ?? 0;
            currentMonthNethWorthModel = {
                date: moment(trackRecord.date.format('YYYY-MM') + '-1'),
                amount: lastAccumulatedNetWorthBeforeSpotToAdd + trackRecord.amount
            };
        }

        for (let trackSpotFuture of tractRecordsAfter)
            trackSpotFuture.amount += trackRecord.amount;

        this.netWorthTrackRecord = [...tractRecordsBefore, currentMonthNethWorthModel, ...tractRecordsAfter];
    }

    public addTrackRecordsFromExistingNetWorthTrackRecord(trackRecords: NetWorthMonthGroupModel[]) {
        if (!trackRecords && trackRecords?.length == 0)
            return;

        const orderedTractRecords = _.orderBy(trackRecords, r => r.date, 'asc')
        let lastAddedNetWorth = 0;

        for (let trackRecord of orderedTractRecords) {
            const recordAmount = trackRecord.amount;
            trackRecord.amount -= lastAddedNetWorth;
            this.addTrackRecordSpotWithRecalculation({ ...trackRecord });
            lastAddedNetWorth = recordAmount;
        }
    }

    public getTractRecord = () => this.netWorthTrackRecord;
}

export class NetWorthMonthGroupModel {
    date: Moment;
    amount: number;
}

export class TotalNetWorthDetail {
    public money: number = 0;
    public stock: number = 0;
    public other: number = 0;
    public crypto: number = 0;
    public comodity: number = 0;
    public total = () => this.money + this.stock + this.other + this.crypto + this.comodity;
}