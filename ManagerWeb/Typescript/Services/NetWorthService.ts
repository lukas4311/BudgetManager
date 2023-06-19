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

        const stockSum = await this.getStockNetWorth(czkSymbol);
        currentBalance += stockSum;

        console.log("🚀 ~ file: NetWorthService.ts:52 ~ NetWorthService ~ getCurrentNetWorth ~ currentBalance:", currentBalance)
        return currentBalance;
    }

    async getStockNetWorth(czkSymbol: string): Promise<number> {
        let netWorth = 0;
        let stockGrouped = await this.stockService.getGroupedTradeHistory();
        const tickers = stockGrouped.map(a => a.tickerName);
        const tickersPrice = await this.stockService.getLastMonthTickersPrice(tickers);

        for (const stock of stockGrouped) {
            const tickerPrices = _.first(tickersPrice.filter(f => f.ticker == stock.tickerName));

            if (tickerPrices != undefined) {
                const actualPrice = _.first(_.orderBy(tickerPrices.price, [(obj) => new Date(obj.time)], ['desc']));
                const actualPriceCzk = await this.cryptoService.getExchangeRate("USD", czkSymbol);
                netWorth += stock.size * (actualPrice?.price ?? 0) * actualPriceCzk;
            }
        }

        return netWorth;
    }
}