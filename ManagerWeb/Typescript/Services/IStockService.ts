import { StockPrice, StockTickerModel } from '../ApiClient/Main/models';
import { StockViewModel } from '../Model/StockViewModel';
import { StockGroupModel, TickersWithPriceHistory } from './StockService';


export interface IStockService {
    getStockTickers(): Promise<StockTickerModel[]>;
    getStockTradeHistory(): Promise<StockViewModel[]>;
    getStockTradeHistoryByTicker(ticker: string): Promise<any>;
    getGroupedTradeHistory(): Promise<StockGroupModel[]>;
    getStockPriceHistory(ticker: string, from?: Date): Promise<StockPrice[]>;
    getLastMonthTickersPrice(tickers: string[]): Promise<TickersWithPriceHistory[]>;
    updateStockTradeHistory(data: StockViewModel): Promise<any>;
    createStockTradeHistory(data: StockViewModel): Promise<any>;
    deleteStockTradeHistory(id: number): Promise<any>;
    getCompanyProfile(ticker: string): Promise<any>;
    getStockNetWorth(czkSymbol: string): Promise<number>;
}