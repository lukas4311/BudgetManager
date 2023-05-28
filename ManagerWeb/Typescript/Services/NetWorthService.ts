import { IBankAccountService } from "./IBankAccountService";
import { ICryptoService } from "./ICryptoService";
import { IOtherInvestmentService } from "./IOtherInvestmentService";
import { IPaymentService } from "./IPaymentService";
import { IStockService } from "./IStockService";

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

    async getNetWorthHistory(from: Date) {
        // get all bank accounts
        const bankAccounts = await this.bankAccount.getAllBankAccounts();
        console.log("🚀 ~ file: NetWorthService.ts:25 ~ NetWorthService ~ getNetWorthHistory ~ bankAccounts:", bankAccounts)

        // get payment history from payment service
        const paymentHistory = await this.paymentService.getExactDateRangeDaysPaymentData(from, undefined, undefined);
        console.log("🚀 ~ file: NetWorthService.ts:29 ~ NetWorthService ~ getNetWorthHistory ~ paymentHistory:", paymentHistory)
    }
}