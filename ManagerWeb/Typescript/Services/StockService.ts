import _ from 'lodash';
import moment from 'moment';
import { StockApi } from '../ApiClient/Main/apis';
import { StockPrice, StockTickerModel, StockTradeHistoryModel } from '../ApiClient/Main/models';
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
    stockValues: number;
    stocksActualPrice: number;
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

    public getGroupedTradeHistory = async (): Promise<StockGroupModel[]> => {
        const stocks = await this.getStockTradeHistory();
        const tickers = await this.getStockTickers();
        let values: StockGroupModel[] = _.chain(stocks)
            .groupBy(g => g.stockTickerId)
            .map((group) => {
                let groupModel: StockGroupModel = new StockGroupModel();
                groupModel.tickerId = group[0].stockTickerId;
                groupModel.tickerName = _.first(tickers.filter(t => t.id == group[0].stockTickerId)).ticker;
                groupModel.size = _.sumBy(group, s => {
                    if (s.action == TradeAction.Buy)
                        return s.tradeSize;
                    else
                        return s.tradeSize * -1;
                });
                groupModel.stockValues = _.sumBy(group, s => s.tradeValue);
                return groupModel;
            })
            .value();

        return values.filter(s => s.size > 0.00001);
    }

    public async getStockNetWorth(finalCurrency: string): Promise<number> {
        let netWorth = 0;
        let stockGrouped = await this.getGroupedTradeHistory();
        const tickers = stockGrouped.map(a => a.tickerName);
        const tickersPrice = await this.getLastMonthTickersPrice(tickers);
        const actualPriceToFinal = await this.cryptoService.getExchangeRate("USD", finalCurrency);

        for (const stock of stockGrouped) {
            const tickerPrices = _.first(tickersPrice.filter(f => f.ticker == stock.tickerName));

            if (tickerPrices != undefined) {
                const actualPrice = _.first(_.orderBy(tickerPrices.price, [(obj) => new Date(obj.time)], ['desc']));
                netWorth += stock.size * (actualPrice?.price ?? 0) * actualPriceToFinal;
            }
        }

        return netWorth;
    }

    public async getMonthlyGroupedAccumulated(fromDate: Date, toDate: Date, trades: StockViewModel[], currency: string): Promise<NetWorthMonthGroupModel[]> {
        const months = this.getMonthsBetween(fromDate, toDate);
        const tradesWithPlusMinusSign = trades.map(t => ({ ...t, tradeSize: t.tradeValue > 0 ? t.tradeSize * -1 : t.tradeSize } as StockViewModel));
        const stockGroupData: NetWorthMonthGroupModel[] = [];
        let prevMonthSum = 0;

        for (const month of months) {
            const monthTrades = tradesWithPlusMinusSign.filter(t => moment(t.tradeTimeStamp).format('YYYY-MM') === month.date);
            const monthGroupedTrades = _.chain(monthTrades).groupBy(t => t.stockTicker)
                .map((value, key) => ({ ticker: key, trades: value }))
                .value();

            for (const monthTickerGroup of monthGroupedTrades) {
                let monthTradeFirst = _.first(monthTickerGroup.trades);

                if (monthTradeFirst) {
                    const dateForForexExchangeGetString = moment(monthTradeFirst.tradeTimeStamp).format('YYYY-MM');
                    let dateForForexExchangeGet = moment(`${dateForForexExchangeGetString}-01`);
                    dateForForexExchangeGet.add(1, 'month');
                    let finalCalculation = await this.calculateCryptoTotalUsdValueForDate(monthTickerGroup.trades, monthTickerGroup.ticker, ForexSymbol.Czk, dateForForexExchangeGet.toDate())
                    prevMonthSum += finalCalculation.finalCurrencyPriceTrade;
                }
            }

            stockGroupData.push({ date: month, amount: prevMonthSum });
        }

        return stockGroupData;
    }

    public async calculateStockTotalUsdValue(tradeHistory: StockViewModel[], ticker: string, finalCurrency: ForexSymbol): Promise<StockCalculationModel> {
        let date = moment(Date.now()).subtract(1, 'd').toDate();
        return this.calculateCryptoTotalUsdValueForDate(tradeHistory, ticker, finalCurrency, date)
    }

    public async calculateCryptoTotalUsdValueForDate(tradeHistory: StockViewModel[], ticker: string, finalCurrency: ForexSymbol, finalCurrencyDate: Date): Promise<StockCalculationModel> {
        let totalyStackedAmountOfStocks = tradeHistory.reduce((partial_sum, v) => partial_sum + v.tradeSize, 0);
        let exhangeRateTrade: number = 0;

        try {
            exhangeRateTrade = await this.getStockCurrentPrice(ticker);
        } catch (error) {
            console.log(`Error while downloading of fin data for ticker: ${ticker}`);
            return { finalCurrencyPrice: 0, finalCurrencyPriceTrade: 0, usdSum: 0, totalyStacked: 0 };
        }

        const usdSum = await this.calculateStockTradesUsdSum(tradeHistory);
        const finalExhangeRate = await this.forexFinApi.getForexPairPriceAtDate({ date: finalCurrencyDate, from: ForexSymbol.Usd, to: finalCurrency });
        let finalCurrencyPrice = usdSum * finalExhangeRate;
        let finalCurrencyPriceTrade = totalyStackedAmountOfStocks * exhangeRateTrade * finalExhangeRate;

        return { finalCurrencyPrice, finalCurrencyPriceTrade, usdSum, totalyStacked: totalyStackedAmountOfStocks };
    }

    public calculateStockTradesUsdSum = async (tradeHistory: StockViewModel[]): Promise<number> => {
        let date = moment(Date.now()).subtract(1, 'd').toDate();
        let sum = 0;

        for (const trade of tradeHistory) {
            let exhangeRate: number = 1;
            let forexSymbol = this.convertStringToForexEnum(trade.currencySymbol);

            if (forexSymbol)
                exhangeRate = await this.forexFinApi.getForexPairPriceAtDate({ date: date, from: forexSymbol, to: ForexSymbol.Usd });
            else {
                console.log(`Currency (${trade.currencySymbol}) is not compatible!`);
                exhangeRate = 1;
            }

            sum += Math.abs(trade.tradeValue) * exhangeRate;
        }

        return sum;
    }

    public async getStockCurrentPrice(ticker: string): Promise<number> {
        let date = moment(Date.now()).subtract(1, 'd').toDate();
        const tickerUpper =  ticker.toUpperCase();
        return (await this.stockFinApi.getStockPriceDataAtDate({ ticker: tickerUpper, date: date }))?.price ?? 0;
    }

    public async getStockPriceHistory(ticker: string, from?: Date): Promise<StockPrice[]> {
        const fromDate = from ?? moment(new Date()).subtract(30, "d").toDate();
        const priceHistory = await this.stockApi.stockStockTickerPriceFromGet({ from: fromDate, ticker: ticker });
        return priceHistory;
    }

    public async getLastMonthTickersPrice(tickers: string[]): Promise<TickersWithPriceHistory[]> {
        let tickersWithPrice: TickersWithPriceHistory[] = [];

        for (const ticker of tickers) {
            const priceHistory = await this.getStockPriceHistory(ticker);
            tickersWithPrice.push({ ticker: ticker, price: priceHistory });
        }

        return tickersWithPrice;
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