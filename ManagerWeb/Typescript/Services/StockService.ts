import _, { forEach } from 'lodash';
import moment from 'moment';
import { StockApi } from '../ApiClient/Main/apis';
import { StockPrice, StockTickerModel, StockTradeHistoryModel, TradeHistory } from '../ApiClient/Main/models';
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
    
    public async getStockTradeHistoryByTicker(ticker: string) {
        return await this.stockApi.stockStockTradeHistoryTickerGet({ ticker: ticker });
    }

    public getStockTradesHistoryInSelectedCurrency = async (): Promise<StockViewModel[]> => {
        let trades = await this.getStockTradeHistory();

        for (let trade of trades) {
            const exchangeRate = await this.calculateForexExchangeRateToUsd(trade);
            const newTradeValue = trade.tradeValue * exchangeRate;
            trade.tradeValue = newTradeValue;
        }

        return trades;
    }

    public getStocksAccumulatedSize = async () => {
        let data = await this.getStockTradesHistoryInSelectedCurrency();
        // const filteredData = data.filter(d => moment(d.tradeTimeStamp).toDate() > fromDate && moment(d.tradeTimeStamp).toDate() < toDate);
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

            let stockGroupModel: StockGroupModel = { tickerName: firstStockRecord.stockTicker, tickerId: firstStockRecord.stockTickerId, size: sizeSum, 
                stockSpentPrice: calculatedTradesSpent, stockCurrentWealth: tickerCurrentPrice * sizeSum, stockSellPrice: calculatedTradesSell };
            groupedModels.push(stockGroupModel);
        }

        return groupedModels;
    }

    public async getStocksNetWorthSum(finalCurrency: string): Promise<number> {
        let groupedTickers = await this.getStocksTickerGroupedTradeHistory();
        let netWorth = _.sumBy(groupedTickers, t => t.stockCurrentWealth);
        return netWorth;
    }

    public async getMonthlyGroupedAccumulated(fromDate: Date, toDate: Date, trades: StockViewModel[], currency: string): Promise<NetWorthMonthGroupModel[]> {
        // const months = this.getMonthsBetween(fromDate, toDate);
        // const tradesWithPlusMinusSign = trades.map(t => ({ ...t, tradeSize: t.tradeValue > 0 ? t.tradeSizeAfterSplit * -1 : t.tradeSizeAfterSplit } as StockViewModel));
        // const stockGroupData: NetWorthMonthGroupModel[] = [];
        // let prevMonthSum = 0;

        // for (const month of months) {
        //     const monthTrades = tradesWithPlusMinusSign.filter(t => moment(t.tradeTimeStamp).format('YYYY-MM') === month.date);
        //     const monthGroupedTrades = _.chain(monthTrades).groupBy(t => t.stockTicker)
        //         .map((value, key) => ({ ticker: key, trades: value }))
        //         .value();

        //     for (const monthTickerGroup of monthGroupedTrades) {
        //         let monthTradeFirst = _.first(monthTickerGroup.trades);

        //         if (monthTradeFirst) {
        //             const dateForForexExchangeGetString = moment(monthTradeFirst.tradeTimeStamp).format('YYYY-MM');
        //             let dateForForexExchangeGet = moment(`${dateForForexExchangeGetString}-01`);
        //             dateForForexExchangeGet.add(1, 'month');
        //             let finalCalculation = await this.calculateStockTotalUsdValueForDate(monthTickerGroup.trades, monthTickerGroup.ticker, ForexSymbol.Czk, dateForForexExchangeGet.toDate())
        //             prevMonthSum += finalCalculation.finalCurrencyPriceTrade;
        //         }
        //     }

        //     stockGroupData.push({ date: month, amount: prevMonthSum });
        // }

        // return stockGroupData;
        throw new Error("Not implemented");
    }

    // public async calculateStockTotalUsdValueForDate(tradeHistory: StockViewModel[], ticker: string, finalCurrency: ForexSymbol, finalCurrencyDate: Date): Promise<StockCalculationModel> {
    //     let totalyStackedAmountOfStocks = tradeHistory.reduce((partial_sum, v) => partial_sum + v.tradeSizeAfterSplit, 0);
    //     let exhangeRateTrade: number = 0;

    //     try {
    //         exhangeRateTrade = await this.getStockCurrentPrice(ticker);
    //     } catch (error) {
    //         console.log(`Error while downloading of fin data for ticker: ${ticker}`);
    //         return { finalCurrencyPrice: 0, finalCurrencyPriceTrade: 0, usdSum: 0, totalyStacked: 0 };
    //     }

    //     const usdSum = await this.calculateStockTradesSpentUsdSum(tradeHistory);
    //     const finalExhangeRate = await this.forexFinApi.getForexPairPriceAtDate({ date: finalCurrencyDate, from: ForexSymbol.Usd, to: finalCurrency });
    //     let finalCurrencyPrice = usdSum * finalExhangeRate;
    //     let finalCurrencyPriceTrade = totalyStackedAmountOfStocks * exhangeRateTrade * finalExhangeRate;

    //     return { finalCurrencyPrice, finalCurrencyPriceTrade, usdSum, totalyStacked: totalyStackedAmountOfStocks };
    // }

    public async getStockCurrentPrice(ticker: string): Promise<number> {
        let date = moment(Date.now()).subtract(1, 'd').toDate();
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

    private calculateStockTradesSpentUsdSum = async (tradeHistory: StockViewModel[]): Promise<number> => {
        let sum = 0;

        for (const trade of tradeHistory.filter(s => s.tradeValue < 0)) {
            let exhangeRate: number = await this.calculateForexExchangeRateToUsd(trade);
            sum += Math.abs(trade.tradeValue) * exhangeRate;
        }

        return sum;
    }

    private calculateStockTradesSellUsdSum = async (tradeHistory: StockViewModel[]): Promise<number> => {
        let sum = 0;

        for (const trade of tradeHistory.filter(s => s.tradeValue > 0)) {
            let exhangeRate: number = await this.calculateForexExchangeRateToUsd(trade);
            sum += Math.abs(trade.tradeValue) * exhangeRate;
        }

        return sum;
    }

    private async calculateForexExchangeRateToUsd(trade: StockViewModel) {
        let exhangeRate: number = 1;
        let forexSymbol = this.convertStringToForexEnum(trade.currencySymbol);

        if (forexSymbol)
            exhangeRate = await this.forexFinApi.getForexPairPriceAtDate({ date: moment(trade.tradeTimeStamp).toDate(), from: forexSymbol, to: ForexSymbol.Usd });
        else {
            console.log(`Currency (${trade.currencySymbol}) is not compatible!`);
            exhangeRate = 1;
        }
        return exhangeRate;
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