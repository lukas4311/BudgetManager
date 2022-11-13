import * as H from 'history';
import _ from 'lodash';
import { StockApi } from '../ApiClient/Main/apis';
import { StockTickerModel } from '../ApiClient/Main/models';
import ApiUrls from '../Model/Setting/ApiUrl';
import { StockViewModel } from '../Model/StockViewModel';
import ApiClientFactory from "../Utils/ApiClientFactory";

export default class StockService {
    private stockApi: StockApi;

    constructor(history: H.History<any>, setting: ApiUrls) {
        const apiFactory = new ApiClientFactory(history);
        this.stockApi = apiFactory.getClientWithSetting(StockApi, setting);
    }

    public getStockTickers = async (): Promise<StockTickerModel[]> => {
        return await this.stockApi.stockStockTickerGet();
    }

    public getStockTradeHistory = async () => {
        const tickers = await this.getStockTickers();
        const stockTrades = await this.stockApi.stockStockTradeHistoryGet();
        return stockTrades.map(s => {
            let viewModel = StockViewModel.mapFromDataModel(s);
            viewModel.stockTicker = _.first(tickers.filter(f => f.id == viewModel.stockTickerId))?.ticker ?? "undefined"
            return viewModel;
        });
    }
}