import _, { forEach } from "lodash";
import { IBankAccountService } from "./IBankAccountService";
import { ICryptoService } from "./ICryptoService";
import { IOtherInvestmentService } from "./IOtherInvestmentService";
import { IPaymentService } from "./IPaymentService";
import { IStockService } from "./IStockService";
import { TradeHistory } from "../ApiClient/Main";
import { IComodityService } from "./IComodityService";
import moment from "moment";

export enum PaymentType {
    Revenue = 'Revenue',
    Expense = 'Expense'
}

const czkSymbol = "CZK";

export default class NetWorthService {
    private paymentService: IPaymentService;
    private stockService: IStockService;
    private cryptoService: ICryptoService;
    private otherInvestment: IOtherInvestmentService;
    private bankAccount: IBankAccountService;
    private comodityService: IComodityService;

    constructor(paymentService: IPaymentService, stockService: IStockService, cryptoService: ICryptoService, otherInvestment: IOtherInvestmentService, bankAccount: IBankAccountService, comodityService: IComodityService) {
        this.paymentService = paymentService;
        this.stockService = stockService;
        this.cryptoService = cryptoService;
        this.otherInvestment = otherInvestment;
        this.bankAccount = bankAccount;
        this.comodityService = comodityService;
    }

    async getCurrentNetWorth(): Promise<number> {
        const bankAccounts = await this.bankAccount.getAllBankAccounts();
        const bankAccountBaseLine = _.sumBy(bankAccounts, s => s.openingBalance);

        const limitDate = new Date(1970, 1, 1);
        const paymentHistory = await this.paymentService.getExactDateRangeDaysPaymentData(limitDate, undefined, undefined);
        const income = _.sumBy(paymentHistory.filter(p => p.paymentTypeCode == PaymentType.Revenue), s => s.amount);
        const expense = _.sumBy(paymentHistory.filter(p => p.paymentTypeCode == PaymentType.Expense), s => s.amount);

        let currentBalance = bankAccountBaseLine + income - expense;

        const otherInvestments = await this.otherInvestment.getSummary();
        const otherInvestmentsbalance = _.sumBy(otherInvestments.actualBalanceData, s => s.balance);

        currentBalance += otherInvestmentsbalance;

        const cryptoSum = await this.cryptoService.getCryptoCurrentNetWorth(czkSymbol);
        currentBalance += cryptoSum;

        const stockSum = await this.stockService.getStockNetWorth(czkSymbol);
        currentBalance += stockSum;

        const comoditySum = await this.comodityService.getComodityNetWorth();
        currentBalance += comoditySum;

        console.log("🚀 ~ file: NetWorthService.ts:52 ~ NetWorthService ~ getCurrentNetWorth ~ currentBalance:", currentBalance)
        return currentBalance;
    }

    async getNetWorthGroupedByMonth() {
        const bankAccounts = await this.bankAccount.getAllBankAccounts();
        const bankAccountBaseLine = _.sumBy(bankAccounts, s => s.openingBalance);

        const limitDate = new Date(1970, 1, 1);
        const paymentHistory = await this.paymentService.getExactDateRangeDaysPaymentData(limitDate, undefined, undefined);
        const paymentGroupedData = [];
        
        const paymentHistoryGroupedByMonth = _.chain(paymentHistory)
        .groupBy(s => moment(s.date).format('YYYY-MM'))
        .map((value, key) => ({ date: moment(key + "-1"), amount: _.sumBy(value, s => s.amount) }))
        .orderBy(f => f.date, ['asc'])
        .reduce((acc, model) => {
            acc[model.date.format("YYYY-MM")] = acc.prev + model.amount + bankAccountBaseLine;
            acc.prev = acc[model.date.format("YYYY-MM")];
            return acc;
        }, { prev: 0 })
        .value();
        
        _.chain(paymentHistory)
        .groupBy(s => moment(s.date).format('YYYY-MM'))
        .map((value, key) => ({ date: moment(key + "-1"), amount: _.sumBy(value, s => s.amount) }))
        .orderBy(f => f.date, ['asc'])
        .reduce((acc, model) => {
            const amount = acc.prev + model.amount + bankAccountBaseLine;
            paymentGroupedData.push({ date: model.date, amount: amount });
            acc.prev = amount;
            return acc;
        }, { prev: 0 });
        
        console.log("🚀 ~ file: NetWorthService.ts:73 ~ NetWorthService ~ getNetWorthGroupedByMonth ~ paymentHistoryGroupedByMonth:", paymentHistoryGroupedByMonth)
        console.log("🚀 ~ file: NetWorthService.ts:71 ~ NetWorthService ~ getNetWorthGroupedByMonth ~ paymentGroupedData:", paymentGroupedData)
        return undefined;
    }
}