import { IBaseModel } from "../Components/BaseList";
import { StockTradeHistoryGetModel } from "../ApiClient/Main/models";
import moment from "moment";

export enum TradeAction {
    Buy,
    Sell
}

export class StockViewModel implements IBaseModel {
    public id: number;
    public tradeTimeStamp: string;
    public stockTickerId: number;
    public stockTicker: string;
    public tradeSize: number;
    public tradeValue: number;
    public currencySymbolId: number;
    public currencySymbol: string;
    public tradeSizeAfterSplit: number;
    get action(): TradeAction {
        return this.tradeValue >= 0 ? TradeAction.Sell : TradeAction.Buy;
    }

    static mapFromDataModel(s: StockTradeHistoryGetModel): StockViewModel {
        let viewModel = new StockViewModel();
        viewModel.currencySymbol = s.currencySymbol;
        viewModel.currencySymbolId = s.currencySymbolId;
        viewModel.id = s.id;
        viewModel.stockTickerId = s.stockTickerId;
        viewModel.tradeSize = s.tradeSize;
        viewModel.tradeTimeStamp = moment(s.tradeTimeStamp).format("YYYY-MM-DD");
        viewModel.tradeValue = s.tradeValue;
        viewModel.stockTicker = undefined;
        viewModel.tradeSizeAfterSplit = s.tradeSizeAfterSplit;

        return viewModel;
    }
}