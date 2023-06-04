import _ from "lodash";
import { IBankAccountService } from "./IBankAccountService";
import { ICryptoService } from "./ICryptoService";
import { IOtherInvestmentService } from "./IOtherInvestmentService";
import { IPaymentService } from "./IPaymentService";
import { IStockService } from "./IStockService";

export enum PaymentType {
    Revenue = 'Revenue',
    Expense = 'Expense'
}

export default class NetWorthService {
    private paymentService: IPaymentService;
    private stockService: IStockService;
    private cryptoService: ICryptoService;
    private otherInvestment: IOtherInvestmentService;
    private bankAccount: IBankAccountService;

    constructor(paymentService: IPaymentService, stockService: IStockService, cryptoService: ICryptoService, otherInvestment: IOtherInvestmentService, bankAccount: IBankAccountService) {
        this.paymentService = paymentService;
        this.stockService = stockService;
        this.cryptoService = cryptoService;
        this.otherInvestment = otherInvestment;
        this.bankAccount = bankAccount;
    }

    async getNetWorthHistory() {
        // get all bank accounts
        const bankAccounts = await this.bankAccount.getAllBankAccounts();
        const bankAccountBaseLine = _.sumBy(bankAccounts, s => s.openingBalance);
        console.log("🚀 ~ file: NetWorthService.ts:27 ~ NetWorthService ~ getNetWorthHistory ~ bankAccountBaseLine:", bankAccountBaseLine)


        const limitDate = new Date(1970, 1, 1);
        // get payment history from payment service
        const paymentHistory = await this.paymentService.getExactDateRangeDaysPaymentData(limitDate, undefined, undefined);
        console.log("🚀 ~ file: NetWorthService.ts:29 ~ NetWorthService ~ getNetWorthHistory ~ paymentHistory:", paymentHistory);
        const income = _.sumBy(paymentHistory.filter(p => p.paymentTypeCode == PaymentType.Revenue), s => s.amount);
        const expense = _.sumBy(paymentHistory.filter(p => p.paymentTypeCode == PaymentType.Expense), s => s.amount);

        let currentBalance = bankAccountBaseLine + income - expense;
        console.log("🚀 ~ file: NetWorthService.ts:43 ~ NetWorthService ~ getNetWorthHistory ~ currentBalance:", currentBalance);

        const otherInvestments = await this.otherInvestment.getSummary();
        const otherInvestmentsbalance = _.sumBy(otherInvestments.actualBalanceData, s => s.balance);
        console.log("🚀 ~ file: NetWorthService.ts:47 ~ NetWorthService ~ getNetWorthHistory ~ otherInvestmentsbalance:", otherInvestmentsbalance)

         currentBalance += otherInvestmentsbalance;
         console.log("🚀 ~ file: NetWorthService.ts:50 ~ NetWorthService ~ getNetWorthHistory ~ currentBalance:", currentBalance)
    }
}