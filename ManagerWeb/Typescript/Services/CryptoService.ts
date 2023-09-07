import moment from "moment";
import { CryptoApiInterface, TradeHistory } from "../ApiClient/Main";
import CryptoTickerSelectModel from "../Components/Crypto/CryptoTickerSelectModel";
import { CryptoTradeViewModel } from "../Components/Crypto/CryptoTradeForm";
import { ICryptoService } from "./ICryptoService";
import _, { forEach } from "lodash";
import { NetWorthMonthGroupModel } from "./NetWorthService";
import { ForexEndpointsApi } from "../ApiClient/Fin";
import {CurrencySymbol as ForexSymbol}  from "../ApiClient/Fin"

const usdSymbol = "USD";
const czkSymbol = "CZK";

export default class CryptoService implements ICryptoService {
    private cryptoApi: CryptoApiInterface;
    private forexApi: ForexEndpointsApi;

    constructor(cryptoApi: CryptoApiInterface, forexApi: ForexEndpointsApi) {
        this.cryptoApi = cryptoApi;
        this.forexApi = forexApi;
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
        return await this.forexApi.getForexPairPriceAtDate({date: date, from: ForexSymbol.Czk, to: ForexSymbol.Usd});
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
        const cryptoExchangeRate = new Map<string, number>();
        const tradesWithPlusMinusSign = trades.map(t => ({ ...t, tradeSize: t.tradeValue > 0 ? t.tradeSize * -1 : t.tradeSize }));
        console.log("🚀 ~ file: CryptoService.ts:77 ~ CryptoService ~ getMonthlyGroupedAccumulatedCrypto ~ tradesWithPlusMinusSign:", tradesWithPlusMinusSign)
        const cryptoGroupData: NetWorthMonthGroupModel[] = [];
        let prevMonthSum = 0;

        for (const month of months) {
            const monthTrades = tradesWithPlusMinusSign.filter(t => moment(t.tradeTimeStamp).format('YYYY-MM') === month.date);
            const monthGroupedTrades = _.chain(monthTrades).groupBy(t => t.cryptoTicker)
            .map((value, key) => ({ ticker: key, sum: _.sumBy(value, s => s.tradeSize) }))
            .value();
            
            let aggregatedSum = prevMonthSum;
            // FIXME: there i need to get echange rate for this month date not for current date
            const exchangeRateCurrency = await this.getExchangeRate(usdSymbol, currency);

            for (const monthTickerGroup of monthGroupedTrades) {
                let exchangeRate = cryptoExchangeRate.get(monthTickerGroup.ticker);

                if (!exchangeRate) {
                    // FIXME: there i need to get echange rate for this month date not for current date, cache is not working for this use case
                    exchangeRate = await this.getExchangeRate(monthTickerGroup.ticker, usdSymbol);
                    cryptoExchangeRate.set(monthTickerGroup.ticker, exchangeRate);
                }

                const finalMultiplier = exchangeRate * exchangeRateCurrency;
                console.log("🚀 ~ file: CryptoService.ts:99 ~ CryptoService ~ getMonthlyGroupedAccumulatedCrypto ~ finalMultiplier:", finalMultiplier)

                if (finalMultiplier != 0)
                    aggregatedSum += finalMultiplier * monthTickerGroup.sum;
            }

            prevMonthSum = aggregatedSum;
            cryptoGroupData.push({ date: month, amount: prevMonthSum });
        }
        
        console.log("🚀 ~ file: CryptoService.ts:104 ~ CryptoService ~ getMonthlyGroupedAccumulatedCrypto ~ cryptoGroupData:", cryptoGroupData)

        return [];
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
        // model.onSave = this.saveTrade;
        // model.currencies = this.currencies;
        // model.cryptoTickers = this.cryptoTickers;
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