import moment from "moment";
import { CryptoApiInterface, TradeHistory } from "../ApiClient/Main";
import CryptoTickerSelectModel from "../Components/Crypto/CryptoTickerSelectModel";
import { CryptoTradeViewModel } from "../Components/Crypto/CryptoTradeForm";
import { ICryptoService } from "./ICryptoService";
import _, { forEach } from "lodash";
import { NetWorthMonthGroupModel } from "./NetWorthService";
import { CryptoEndpointsApi, ForexEndpointsApi } from "../ApiClient/Fin";
import { CurrencySymbol as ForexSymbol } from "../ApiClient/Fin"

const usdSymbol = "USD";
const czkSymbol = "CZK";
const stableCoins = ["USDC", "USDT", "BUSD"]

export default class CryptoService implements ICryptoService {
    private cryptoApi: CryptoApiInterface;
    private forexApi: ForexEndpointsApi;
    private cryptoFinApi: CryptoEndpointsApi;
    private forexFinApi: ForexEndpointsApi;

    constructor(cryptoApi: CryptoApiInterface, forexApi: ForexEndpointsApi, cryptoFinApi: CryptoEndpointsApi, forexFinApi: ForexEndpointsApi) {
        this.cryptoApi = cryptoApi;
        this.forexApi = forexApi;
        this.cryptoFinApi = cryptoFinApi;
        this.forexFinApi = forexFinApi;
    }

    public async getTradeData(): Promise<CryptoTradeViewModel[]> {
        const data = await this.cryptoApi.cryptosAllGet();
        let trades: CryptoTradeViewModel[] = data.map(t => this.mapDataModelToViewModel(t));
        return trades;
    }

    public async getRawTradeData(): Promise<TradeHistory[]> {
        return await this.cryptoApi.cryptosAllGet();
    }

    public async getCryptoTickers(): Promise<CryptoTickerSelectModel[]> {
        return (await this.cryptoApi.cryptosTickersGet()).map(c => ({ id: c.id, ticker: c.ticker }))
    }

    public async createCryptoTrade(tradeModel: CryptoTradeViewModel) {
        const tradeHistory = this.mapViewModelToDataModel(tradeModel);
        await this.cryptoApi.cryptosPost({ tradeHistory: tradeHistory });
    }

    public async updateCryptoTrade(tradeModel: CryptoTradeViewModel) {
        const tradeHistory = this.mapViewModelToDataModel(tradeModel);
        await this.cryptoApi.cryptosPut({ tradeHistory: tradeHistory });
    }

    public async deleteCryptoTrade(tradeId: number) {
        await this.cryptoApi.cryptosDelete({ body: tradeId });
    }

    public async getExchangeRate(from: string, to: string): Promise<number> {
        let date = moment(Date.now()).subtract(1, 'd').toDate();
        return await this.forexApi.getForexPairPriceAtDate({ date: date, from: ForexSymbol.Czk, to: ForexSymbol.Usd });
        // return await this.cryptoApi.cryptosActualExchangeRateFromCurrencyToCurrencyGet({ fromCurrency: from, toCurrency: to });
    }

    public async getCryptoCurrentNetWorth(currency: string): Promise<number> {
        let cryptoSum = 0;
        let trades: TradeHistory[] = await this.getRawTradeData();
        let groupedTrades = _.chain(trades).groupBy(t => t.cryptoTicker)
            .map((value, key) => ({ ticker: key, sum: _.sumBy(value, s => s.tradeSize) }))
            .value();

        const finalCurrencyExcahngeRate = await this.getExchangeRate(usdSymbol, currency);

        for (const ticker of groupedTrades) {
            const dollarExcahngeRate = await this.getExchangeRate(ticker.ticker, usdSymbol);
            const finalMultiplier = dollarExcahngeRate * finalCurrencyExcahngeRate;

            if (finalMultiplier != 0) {
                const sumedInFinalCurrency = finalMultiplier * ticker.sum;
                cryptoSum += sumedInFinalCurrency;
            }
        }

        return cryptoSum;
    }

    public async getMonthlyGroupedAccumulatedCrypto(fromDate: Date, toDate: Date, trades: TradeHistory[], currency: string): Promise<NetWorthMonthGroupModel[]> {
        const months = this.getMonthsBetween(fromDate, toDate);
        const tradesWithPlusMinusSign = trades.map(t => ({ ...t, tradeSize: t.tradeValue > 0 ? t.tradeSize * -1 : t.tradeSize }));
        const cryptoGroupData: NetWorthMonthGroupModel[] = [];
        let prevMonthSum = 0;

        for (const month of months) {
            const monthTrades = tradesWithPlusMinusSign.filter(t => moment(t.tradeTimeStamp).format('YYYY-MM') === month.date);
            const monthGroupedTrades = _.chain(monthTrades).groupBy(t => t.cryptoTicker)
                .map((value, key) => ({ ticker: key, trades: value }))
                .value();

            for (const monthTickerGroup of monthGroupedTrades) {
                let monthTradeFirst = _.first(monthTickerGroup.trades);

                if (monthTradeFirst) {
                    const dateForForexExchangeGetString = moment(monthTradeFirst.tradeTimeStamp).format('YYYY-MM');
                    let dateForForexExchangeGet = moment(`${dateForForexExchangeGetString}-01`);
                    dateForForexExchangeGet.add(1, 'month');
                    let finalCalculation = await this.calculateCryptoTotalUsdValueForDate(monthTickerGroup.trades, monthTickerGroup.ticker, ForexSymbol.Czk, dateForForexExchangeGet.toDate())
                    prevMonthSum += finalCalculation.finalCurrencyPrice;
                }
            }

            cryptoGroupData.push({ date: month, amount: prevMonthSum });
        }
        
        console.log("🚀 ~ file: CryptoService.ts:109 ~ CryptoService ~ getMonthlyGroupedAccumulatedCrypto ~ cryptoGroupData:", cryptoGroupData)
        return [];
    }

    public async getCryptoCurrentPrice(ticker: string): Promise<number> {
        let date = moment(Date.now()).subtract(1, 'd').toDate();
        return (await this.cryptoFinApi.getCryptoPriceDataAtDate({ ticker: _.upperCase(ticker), date: date }))?.price ?? 0;
    }

    public async calculateCryptoTotalUsdValue(tradeHistory: TradeHistory[], ticker: string, finalCurrency: ForexSymbol): Promise<CryptoCalculationModel> {
        let date = moment(Date.now()).subtract(1, 'd').toDate();
        return this.calculateCryptoTotalUsdValueForDate(tradeHistory, ticker, finalCurrency, date)
    }

    public async calculateCryptoTotalUsdValueForDate(tradeHistory: TradeHistory[], ticker: string, finalCurrency: ForexSymbol, finalCurrencyDate: Date): Promise<CryptoCalculationModel> {
        let totalyStacked = tradeHistory.reduce((partial_sum, v) => partial_sum + v.tradeSize, 0);
        let exhangeRateTrade: number = await this.getCryptoCurrentPrice(ticker);
        const usdSum = await this.calculateCryptoTradesUsdSum(tradeHistory);
        const finalExhangeRate = await this.forexFinApi.getForexPairPriceAtDate({ date: finalCurrencyDate, from: ForexSymbol.Usd, to: finalCurrency });
        let finalCurrencyPrice = usdSum * finalExhangeRate;
        let finalCurrencyPriceTrade = totalyStacked * exhangeRateTrade * finalExhangeRate;

        return { finalCurrencyPrice, finalCurrencyPriceTrade, usdSum, totalyStacked };
    }

    public calculateCryptoTradesUsdSum = async (tradeHistory: TradeHistory[]): Promise<number> => {
        let date = moment(Date.now()).subtract(1, 'd').toDate();
        let sum = 0;

        for (const trade of tradeHistory) {
            let exhangeRate: number = 1
            let forexSymbol = this.convertStringToForexEnum(trade.currencySymbol);

            if (forexSymbol)
                exhangeRate = await this.forexFinApi.getForexPairPriceAtDate({ date: date, from: forexSymbol, to: ForexSymbol.Usd });
            else if (_.some(stableCoins, c => c == trade.currencySymbol))
                exhangeRate = 1;
            else {
                const cryptoPrice = await this.cryptoFinApi.getCryptoPriceDataAtDate({ date: trade.tradeTimeStamp, ticker: _.upperCase(trade.currencySymbol) });

                if (cryptoPrice)
                    exhangeRate = cryptoPrice.price;
            }

            sum += Math.abs(trade.tradeValue) * exhangeRate;
        }

        return sum;
    }

    private convertStringToForexEnum(value: string): ForexSymbol | undefined {
        if (Object.values(ForexSymbol).includes(value as ForexSymbol))
            return value as ForexSymbol;

        return undefined;
    }

    private mapViewModelToDataModel = (tradeModel: CryptoTradeViewModel) => {
        const tradeHistory: TradeHistory = {
            cryptoTickerId: tradeModel.cryptoTickerId,
            currencySymbolId: tradeModel.currencySymbolId,
            id: tradeModel.id,
            tradeSize: tradeModel.tradeSize,
            tradeTimeStamp: moment(tradeModel.tradeTimeStamp).toDate(),
            tradeValue: tradeModel.tradeValue
        };

        return tradeHistory;
    }

    private mapDataModelToViewModel = (tradeHistory: TradeHistory): CryptoTradeViewModel => {
        let model: CryptoTradeViewModel = new CryptoTradeViewModel();
        model.cryptoTicker = tradeHistory.cryptoTicker;
        model.cryptoTickerId = tradeHistory.cryptoTickerId;
        model.currencySymbol = tradeHistory.currencySymbol;
        model.currencySymbolId = tradeHistory.currencySymbolId;
        model.id = tradeHistory.id;
        model.tradeSize = tradeHistory.tradeSize;
        model.tradeTimeStamp = moment(tradeHistory.tradeTimeStamp).format("YYYY-MM-DD");
        model.tradeValue = tradeHistory.tradeValue;
        return model;
    }

    private getMonthsBetween(fromDate: Date, toDate: Date) {
        const start = moment(fromDate);
        const end = moment(toDate);
        const months = [];

        while (start.isBefore(end)) {
            months.push({ date: start.format('YYYY-MM') });
            start.add(1, 'month');
        }

        return months;
    }
}

export class CryptoCalculationModel {
    totalyStacked: number;
    usdSum: number;
    finalCurrencyPrice: number;
    finalCurrencyPriceTrade: number;
}