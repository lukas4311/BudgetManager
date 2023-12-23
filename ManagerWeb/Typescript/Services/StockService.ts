import _, { forEach } from 'lodash';
import moment from 'moment';
import { StockApi } from '../ApiClient/Main/apis';
import { ECurrencySymbol, StockPrice, StockTickerModel, StockTradeHistoryModel, TradeHistory } from '../ApiClient/Main/models';
import { StockViewModel, TradeAction } from '../Model/StockViewModel';
import { IStockService } from './IStockService';
import { ICryptoService } from './ICryptoService';
import { NetWorthMonthGroupModel } from './NetWorthService';
import { ForexEndpointsApi, CurrencySymbol as ForexSymbol, StockEndpointsApi } from "../ApiClient/Fin"

const usdSymbol = "USD";

export class StockGroupModel {
    tickerId: number;
    tickerName: string;
    size: number;
    stockSpentPrice: number;
    stockCurrentWealth: number;
    stockSellPrice: number;
}

export default class StockService implements IStockService {
    private stockApi: StockApi;
    private cryptoService: ICryptoService;
    private forexFinApi: ForexEndpointsApi;
    private stockFinApi: StockEndpointsApi;

    constructor(stockApi: StockApi, cryptoService: ICryptoService, forexFinApi: ForexEndpointsApi, stockFinApi: StockEndpointsApi) {
        this.stockApi = stockApi;
        this.cryptoService = cryptoService;
        this.forexFinApi = forexFinApi;
        this.stockFinApi = stockFinApi;
    }

    public getStockTickers = async (): Promise<StockTickerModel[]> => {
        return await this.stockApi.stockStockTickerGet();
    }

    public async getStockTradeHistory(): Promise<StockViewModel[]> {
        const tickers = await this.getStockTickers();
        const stockTrades = await this.stockApi.stockStockTradeHistoryGet();
        return stockTrades.map(s => {
            let viewModel = StockViewModel.mapFromDataModel(s);
            viewModel.stockTicker = _.first(tickers.filter(f => f.id == viewModel.stockTickerId))?.ticker ?? "undefined"
            return viewModel;
        });
    }

    public async getStockTradeHistoryInSelectedCurrency(currencySymbol: ECurrencySymbol): Promise<StockViewModel[]> {
        const tickers = await this.getStockTickers();
        const stockTrades = await this.stockApi.stockStockTradeHistoryExhangedToForexSymbolGet({ forexSymbol: currencySymbol });
        return stockTrades.map(s => {
            let viewModel = StockViewModel.mapFromDataModel(s);
            viewModel.stockTicker = _.first(tickers.filter(f => f.id == viewModel.stockTickerId))?.ticker ?? "undefined"
            return viewModel;
        });
    }

    public async getStockTradeHistoryByTicker(ticker: string) {
        return await this.stockApi.stockStockTradeHistoryTickerGet({ ticker: ticker });
    }

    public getStockTradesHistoryInSelectedCurrency = async (): Promise<StockViewModel[]> => {
        let trades = await this.getStockTradeHistoryInSelectedCurrency(ECurrencySymbol.Usd);
        return trades;
    }

    public getStocksAccumulatedSize = async (): Promise<Map<string, Map<string, number>>> => {
        let data = await this.getStockTradesHistoryInSelectedCurrency();
        const orderData = _.sortBy(data, d => new Date(d.tradeTimeStamp), ['asc']);

        let accumulatedSizeInDays: Map<string, Map<string, number>> = new Map<string, Map<string, number>>();
        let stockAccumulatedSize = new Map<string, number>();

        for (let trade of orderData) {
            if (stockAccumulatedSize.has(trade.stockTicker)) {
                let currentTickerSize = stockAccumulatedSize.get(trade.stockTicker);
                stockAccumulatedSize.set(trade.stockTicker, currentTickerSize + (trade.tradeSizeAfterSplit * (trade.tradeValue <= 0 ? 1 : -1)));
            } else {
                stockAccumulatedSize.set(trade.stockTicker, trade.tradeSizeAfterSplit * (trade.tradeValue <= 0 ? 1 : -1));
            }

            accumulatedSizeInDays.set(trade.tradeTimeStamp, _.clone(stockAccumulatedSize));
        }

        return accumulatedSizeInDays;
    }

    public getStocksAccumulatedValue = async (): Promise<Map<string, Map<string, number>>> => {
        let stockAccumulatedSizes = await this.getStocksAccumulatedSize();

        let accumulatedValueInDays: Map<string, Map<string, number>> = new Map<string, Map<string, number>>();
        let stockAccumulatedValue = new Map<string, number>();

        for (const [dateKey, tickersSizeAccumulated] of stockAccumulatedSizes) {
            const date = moment(dateKey).toDate();
            const tickers = Array.from(tickersSizeAccumulated.keys());
            const tickersPrice = await this.stockFinApi.getStocksPriceDataAtDate({ date: date, tickers: tickers })

            for (const stockPriceInfo of tickersPrice) {
                const accumulatedSize = tickersSizeAccumulated.get(stockPriceInfo.ticker);
                const tickerValue = (stockPriceInfo?.price ?? 1) * accumulatedSize;
                stockAccumulatedValue.set(stockPriceInfo.ticker, tickerValue);
            }

            accumulatedValueInDays.set(dateKey, _.clone(stockAccumulatedValue));
        }

        return accumulatedValueInDays;
    }

    public getStocksTickerGroupedTradeHistory = async (): Promise<StockGroupModel[]> => {
        const stocks = await this.getStockTradesHistoryInSelectedCurrency();
        let groupedTradesByTicker = _.chain(stocks)
            .groupBy(g => g.stockTickerId)
            .value();
        let groupedModels = []

        for (const tickerKey in groupedTradesByTicker) {
            const trades = groupedTradesByTicker[tickerKey];
            const firstStockRecord = _.first(trades);

            const calculatedTradesSpent = _.sumBy(trades.filter(t => t.action == TradeAction.Buy), t => t.tradeValue) * -1;
            const calculatedTradesSell = _.sumBy(trades.filter(t => t.action == TradeAction.Sell), t => t.tradeValue);
            const sizeSum = _.sumBy(trades, s => s.action == TradeAction.Buy ? s.tradeSizeAfterSplit : s.tradeSizeAfterSplit * -1);
            const tickerCurrentPrice = await this.getStockCurrentPrice(firstStockRecord.stockTicker);

            let stockGroupModel: StockGroupModel = {
                tickerName: firstStockRecord.stockTicker, tickerId: firstStockRecord.stockTickerId, size: sizeSum,
                stockSpentPrice: calculatedTradesSpent, stockCurrentWealth: tickerCurrentPrice * sizeSum, stockSellPrice: calculatedTradesSell
            };
            groupedModels.push(stockGroupModel);
        }

        return groupedModels;
    }

    public async getStocksNetWorthSum(finalCurrency: string): Promise<number> {
        let groupedTickers = await this.getStocksTickerGroupedTradeHistory();
        const finalCurrencyExcahngeRate = await this.getExchangeRate(usdSymbol, finalCurrency);

        let netWorth = _.sumBy(groupedTickers, t => t.stockCurrentWealth);
        return netWorth * (finalCurrencyExcahngeRate && finalCurrencyExcahngeRate != 0 ? finalCurrencyExcahngeRate : 1);
    }

    public async getMonthlyGroupedAccumulated(fromDate: Date, toDate: Date, trades: StockViewModel[], currency: string): Promise<NetWorthMonthGroupModel[]> {
        const stockGroupData: NetWorthMonthGroupModel[] = [];
        const months = this.getMonthsBetween(fromDate, toDate);
        const accumulatedTrades = await this.getStocksAccumulatedValue();
        let accumulatedValueInDay: GroupedStockValues[] = []
        accumulatedTrades.forEach((value, key) => {
            let date = moment(key).toDate();
            const stockValues = Array.from(value.values());
            let groupedStockValues: GroupedStockValues = { date: date, accumulatedValue: _.sum(stockValues) };
            accumulatedValueInDay.push(groupedStockValues);
        });
        accumulatedValueInDay = _.orderBy(accumulatedValueInDay, t => t.date, "asc");
        const lastDate = _.last(accumulatedValueInDay);

        for (const month of months) {
            const monthValueSum = _.last(accumulatedValueInDay.filter(t => moment(t.date).format('YYYY-MM') <= month.date))?.accumulatedValue ?? lastDate.accumulatedValue;
            stockGroupData.push({ date: month, amount: monthValueSum });
        }

        return stockGroupData;
    }

    public async getStockCurrentPrice(ticker: string): Promise<number> {
        let date = moment(Date.now()).subtract(1, 'd').toDate();
        const tickerUpper = ticker.toUpperCase();

        try {
            return (await this.stockFinApi.getStockPriceDataAtDate({ ticker: tickerUpper, date: date }))?.price ?? 0
        } catch (error) {
            return 0;
        }
    }

    public async getStockPrice(ticker: string, date: Date): Promise<number> {
        const tickerUpper = ticker.toUpperCase();

        try {
            return (await this.stockFinApi.getStockPriceDataAtDate({ ticker: tickerUpper, date: date }))?.price ?? 0
        } catch (error) {
            return 0;
        }
    }

    public async getLastMonthTickersPrice(tickers: string[]): Promise<TickersWithPriceHistory[]> {
        let tickersWithPrice: TickersWithPriceHistory[] = [];

        for (const ticker of tickers) {
            const priceHistory = await this.getStockPriceHistory(ticker);
            tickersWithPrice.push({ ticker: ticker, price: priceHistory });
        }

        return tickersWithPrice;
    }

    public async getStockPriceHistory(ticker: string, from?: Date): Promise<StockPrice[]> {
        const fromDate = from ?? moment(new Date()).subtract(30, "d").toDate();
        const priceHistory = await this.stockFinApi.getStockPriceDataFromDate({ from: fromDate, ticker: ticker });
        return priceHistory;
    }

    public async updateStockTradeHistory(data: StockViewModel) {
        const stockHistoryTrade: StockTradeHistoryModel = {
            id: data.id,
            currencySymbolId: data.currencySymbolId,
            stockTickerId: data.stockTickerId,
            tradeSize: data.tradeSize,
            tradeTimeStamp: new Date(data.tradeTimeStamp),
            tradeValue: data.tradeValue
        };

        await this.stockApi.stockStockTradeHistoryPut({ stockTradeHistoryModel: stockHistoryTrade });
    }

    public async createStockTradeHistory(data: StockViewModel) {
        const stockHistoryTrade: StockTradeHistoryModel = {
            id: data.id,
            currencySymbolId: data.currencySymbolId,
            stockTickerId: data.stockTickerId,
            tradeSize: data.tradeSize,
            tradeTimeStamp: new Date(data.tradeTimeStamp),
            tradeValue: data.tradeValue
        };

        await this.stockApi.stockStockTradeHistoryPost({ stockTradeHistoryModel: stockHistoryTrade });
    }

    public async deleteStockTradeHistory(id: number) {
        await this.stockApi.stockStockTradeHistoryDelete({ body: id });
    }

    public async getCompanyProfile(ticker: string) {
        return await this.stockApi.stockStockTickerCompanyProfileGet({ ticker: ticker })
    }

    // private async calculateForexExchangeRateToUsd(trade: StockViewModel) {
    //     let exhangeRate: number = 1;
    //     let forexSymbol = this.convertStringToForexEnum(trade.currencySymbol);

    //     if (forexSymbol)
    //         exhangeRate = await this.forexFinApi.getForexPairPriceAtDate({ date: moment(trade.tradeTimeStamp).toDate(), from: forexSymbol, to: ForexSymbol.Usd });
    //     else {
    //         console.log(`Currency (${trade.currencySymbol}) is not compatible!`);
    //         exhangeRate = 1;
    //     }
    //     return exhangeRate;
    // }

    public async getExchangeRate(from: string, to: string): Promise<number> {
        let date = moment(Date.now()).subtract(1, 'd').toDate();
        const fromEnum = this.convertStringToForexEnum(from);
        const toEnum = this.convertStringToForexEnum(to);
        return await this.forexFinApi.getForexPairPriceAtDate({ date: date, from: fromEnum, to: toEnum });
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

    private convertStringToForexEnum(value: string): ForexSymbol | undefined {
        if (Object.values(ForexSymbol).includes(value as ForexSymbol))
            return value as ForexSymbol;

        return undefined;
    }
}

export interface TickersWithPriceHistory {
    ticker: string;
    price: StockPrice[];
}

export class StockCalculationModel {
    totalyStacked: number;
    usdSum: number;
    finalCurrencyPrice: number;
    finalCurrencyPriceTrade: number;
}

class GroupedStockValues {
    date: Date;
    accumulatedValue: number;
}