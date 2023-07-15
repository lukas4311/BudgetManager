import _ from 'lodash';
import moment from 'moment';
import { StockApi } from '../ApiClient/Main/apis';
import { StockPrice, StockTickerModel, StockTradeHistoryModel } from '../ApiClient/Main/models';
import { StockViewModel, TradeAction } from '../Model/StockViewModel';
import { IStockService } from './IStockService';
import { ICryptoService } from './ICryptoService';

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
    cryptoService: ICryptoService;

    constructor(stockApi: StockApi, cryptoService: ICryptoService) {
        this.stockApi = stockApi;
        this.cryptoService = cryptoService;
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

    public async getStockNetWorth(czkSymbol: string): Promise<number> {
        let netWorth = 0;
        let stockGrouped = await this.getGroupedTradeHistory();
        const tickers = stockGrouped.map(a => a.tickerName);
        const tickersPrice = await this.getLastMonthTickersPrice(tickers);
        const actualPriceCzk = await this.cryptoService.getExchangeRate("USD", czkSymbol);

        for (const stock of stockGrouped) {
            const tickerPrices = _.first(tickersPrice.filter(f => f.ticker == stock.tickerName));

            if (tickerPrices != undefined) {
                const actualPrice = _.first(_.orderBy(tickerPrices.price, [(obj) => new Date(obj.time)], ['desc']));
                netWorth += stock.size * (actualPrice?.price ?? 0) * actualPriceCzk;
            }
        }

        return netWorth;
    }

    public async getMonthlyGroupedAccumulated(trades: StockViewModel[], currency: string): Promise<number> {
        const stockGroupedData = [];
        const stockGroupDataWithCurrencyAmount = [];
        const stockExchangeRate = new Map<string, number>();
        const cryptos = _.chain(trades).groupBy(a => a.stockTicker).map((value, key) => ({ ticker: key, trades: value })).value();

        for (const crypto of cryptos.filter(f => f.trades.length > 0)) {
            _.chain(crypto.trades)
                .groupBy(s => moment(s.tradeTimeStamp).format('YYYY-MM'))
                .map((value, key) => ({ date: key, tradeSize: _.sumBy(value, s => s.tradeSize), stockTickerId: _.first(value)?.stockTickerId, stockTicker: _.first(value)?.stockTicker }))
                .orderBy(f => f.date, ['asc'])
                .reduce((acc, model) => {
                    const tradeSize = acc.prev + model.tradeSize;
                    stockGroupedData.push({ date: model.date, size: tradeSize, tickerId: model.stockTickerId, ticker: model.stockTicker });
                    acc.prev = tradeSize;
                    return acc;
                }, { prev: 0 })
                .value();
        }

        const finalCurrencyExcahngeRate = await this.cryptoService.getExchangeRate(usdSymbol, currency);

        console.log("Start", moment(Date.now()).format("HH:mm:ss"));
        for (const monthGroups of stockGroupedData) {
            let exchangeRate = stockExchangeRate.get(monthGroups.ticker);

            if (!exchangeRate) {
                exchangeRate = await this.cryptoService.getExchangeRate(monthGroups.ticker, usdSymbol);
                stockExchangeRate.set(monthGroups.ticker, exchangeRate);
            }

            const finalMultiplier = exchangeRate * finalCurrencyExcahngeRate;
            stockGroupDataWithCurrencyAmount.push({ date: monthGroups.date, amount: monthGroups.size * finalMultiplier, ticker: monthGroups.ticker });
        }
        console.log("End", moment(Date.now()).format("HH:mm:ss"));

        return 0;
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
}

export interface TickersWithPriceHistory {
    ticker: string;
    price: StockPrice[];
}