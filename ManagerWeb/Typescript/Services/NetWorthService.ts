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

    async getCurrentNetWorth(): Promise<number> {
        const bankAccounts = await this.bankAccountService.getAllBankAccounts();
        const bankAccountBaseLine = _.sumBy(bankAccounts, s => s.openingBalance);

        const limitDate = new Date(1970, 1, 1);
        const paymentHistory = await this.paymentService.getExactDateRangeDaysPaymentData(limitDate, undefined, undefined);
        const income = _.sumBy(paymentHistory.filter(p => p.paymentTypeCode == PaymentType.Revenue), s => s.amount);
        const expense = _.sumBy(paymentHistory.filter(p => p.paymentTypeCode == PaymentType.Expense), s => s.amount);

        let currentBalance = bankAccountBaseLine + income - expense;

        const otherInvestments = await this.otherInvestmentService.getSummary();
        const otherInvestmentsbalance = _.sumBy(otherInvestments.actualBalanceData, s => s.balance);

        currentBalance += otherInvestmentsbalance;

        const cryptoSum = await this.cryptoService.getCryptoCurrentNetWorth(czkSymbol);
        currentBalance += cryptoSum;

        const stockSum = await this.stockService.getStocksNetWorthSum(czkSymbol);
        currentBalance += stockSum;

        const comoditySum = await this.comodityService.getComodityNetWorth();
        currentBalance += comoditySum;

        return currentBalance;
    }

    async getNetWorthGroupedByMonth() {
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

        const otherInvestments = await this.otherInvestmentService.getAll();
        const otherInvetmentsMonthlyGrouped = await this.otherInvestmentService.getMonthlyGroupedAccumulatedPayments(new Date(2023, 0, 1), new Date(2023, 6, 1), otherInvestments);

        const tradeData = await this.cryptoService.getRawTradeData();
        const cryptoNetWorth = this.cryptoService.getMonthlyGroupedAccumulatedCrypto(new Date(2022, 0, 1), new Date(2023, 0, 1), tradeData, czkSymbol);

        const stockTradeData = await this.stockService.getStockTradeHistory();
        const acumulatedData = await this.stockService.getMonthlyGroupedAccumulated(new Date(2022, 0, 1), new Date(2023, 0, 1), stockTradeData, czkSymbol);

        return paymentGroupedData;
    }
}

export class NetWorthMonthGroupModel {
    date: Moment;
    amount: number;
}