import * as H from 'history';
import _ from 'lodash';
import moment from 'moment';
import { StockApi } from '../ApiClient/Main/apis';
import { InterestRate, StockPrice, StockTickerModel, StockTradeHistoryModel } from '../ApiClient/Main/models';
import ApiUrls from '../Model/Setting/ApiUrl';
import { StockViewModel, TradeAction } from '../Model/StockViewModel';
import ApiClientFactory from "../Utils/ApiClientFactory";

export class StockGroupModel {
    tickerId: number;
    tickerName: string;
    size: number;
    stockValues: number;
    stocksActualPrice: number;
}

export default class StockService {
    private stockApi: StockApi;

    constructor(history: H.History<any>, setting: ApiUrls) {
        const apiFactory = new ApiClientFactory(history);
        this.stockApi = apiFactory.getClientWithSetting(StockApi, setting);
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