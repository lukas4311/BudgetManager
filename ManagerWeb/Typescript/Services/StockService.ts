import * as H from 'history';
import _ from 'lodash';
import { StockApi } from '../ApiClient/Main/apis';
import { StockTickerModel } from '../ApiClient/Main/models';
import ApiUrls from '../Model/Setting/ApiUrl';
import { StockViewModel, TradeAction } from '../Model/StockViewModel';
import ApiClientFactory from "../Utils/ApiClientFactory";

export class StockGroupModel {
    tickerId: number;
    tickerName: string;
    size: number;
    stockValues: number;
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

    public getStockTradeHistory = async (): Promise<StockViewModel[]> => {
        const tickers = await this.getStockTickers();
        const stockTrades = await this.stockApi.stockStockTradeHistoryGet();
        return stockTrades.map(s => {
            let viewModel = StockViewModel.mapFromDataModel(s);
            viewModel.stockTicker = _.first(tickers.filter(f => f.id == viewModel.stockTickerId))?.ticker ?? "undefined"
            return viewModel;
        });
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
        return values;
    }
}