import { IBaseModel } from "../Components/BaseList";
import { StockTradeHistoryGetModel } from "../ApiClient/Main/models";
import moment from "moment";

export class StockViewModel implements IBaseModel {
    id: number;
    tradeTimeStamp: string;
    stockTickerId: number;
    stockTicker: string;
    tradeSize: number;
    tradeValue: number;
    currencySymbolId: number;
    currencySymbol: string;
    // onSave: (data: StockViewModel) => void;

    static mapFromDataModel(s: StockTradeHistoryGetModel): StockViewModel {
        return {
            currencySymbol: s.currencySymbol,
            currencySymbolId: s.currencySymbolId,
            id: s.id,
            stockTickerId: s.stockTickerId,
            tradeSize: s.tradeSize,
            tradeTimeStamp: moment(s.tradeTimeStamp).format("YYYY-MM-DD"),
            tradeValue: s.tradeValue,
            stockTicker: undefined,
            // onSave: undefined
        };
    }
}