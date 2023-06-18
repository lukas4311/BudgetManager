import _, { forEach } from "lodash";
import { IBankAccountService } from "./IBankAccountService";
import { ICryptoService } from "./ICryptoService";
import { IOtherInvestmentService } from "./IOtherInvestmentService";
import { IPaymentService } from "./IPaymentService";
import { IStockService } from "./IStockService";
import { TradeHistory } from "../ApiClient/Main";

export enum PaymentType {
    Revenue = 'Revenue',
    Expense = 'Expense'
}

const usdSymbol = "USD";
const czkSymbol = "CZK";

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

    async getNetWorthHistory(): Promise<number> {
        // get all bank accounts
        const bankAccounts = await this.bankAccount.getAllBankAccounts();
        const bankAccountBaseLine = _.sumBy(bankAccounts, s => s.openingBalance);

        const limitDate = new Date(1970, 1, 1);
        // get payment history from payment service
        const paymentHistory = await this.paymentService.getExactDateRangeDaysPaymentData(limitDate, undefined, undefined);
        const income = _.sumBy(paymentHistory.filter(p => p.paymentTypeCode == PaymentType.Revenue), s => s.amount);
        const expense = _.sumBy(paymentHistory.filter(p => p.paymentTypeCode == PaymentType.Expense), s => s.amount);

        let currentBalance = bankAccountBaseLine + income - expense;

        const otherInvestments = await this.otherInvestment.getSummary();
        const otherInvestmentsbalance = _.sumBy(otherInvestments.actualBalanceData, s => s.balance);

        currentBalance += otherInvestmentsbalance;

        // const cryptoBalance = this.cryptoService.
        this.getCryptoUsdSum();

        return currentBalance;
    }

    private async getCryptoUsdSum() {
        let cryptoSum = 0;
        let trades: TradeHistory[] = await this.cryptoService.getRawTradeData();
        let groupedTrades = _.chain(trades).groupBy(t => t.cryptoTicker)
            .map((value, key) => ({ ticker: key, sum: _.sumBy(value, s => s.tradeSize) }))
            .value();

        console.log("🚀 ~ file: NetWorthService.ts:58 ~ NetWorthService ~ getCryptoUsdSum ~ groupedTrades:", groupedTrades);

        for (const ticker of groupedTrades) {
            const exchangeRate = await this.cryptoService.getExchangeRate(ticker.ticker, czkSymbol);

            if (exchangeRate) {
                const sumedInFinalCurrency = exchangeRate * ticker.sum;
                cryptoSum += sumedInFinalCurrency;
            }
        }
        
        console.log("🚀 ~ file: NetWorthService.ts:71 ~ NetWorthService ~ getCryptoUsdSum ~ cryptoSum:", cryptoSum)
    }
}

class CryptoSum {
    usdPrice: number;
}