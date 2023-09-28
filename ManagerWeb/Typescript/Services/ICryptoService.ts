import { TradeHistory } from "../ApiClient/Main/models";
import CryptoTickerSelectModel from "../Components/Crypto/CryptoTickerSelectModel";
import { CryptoTradeViewModel } from "../Components/Crypto/CryptoTradeForm";
import { NetWorthMonthGroupModel } from "./NetWorthService";
import { CurrencySymbol as ForexSymbol } from "../ApiClient/Fin";
import { CryptoCalculationModel } from "./CryptoService";

export interface ICryptoService {
    getTradeData(): Promise<CryptoTradeViewModel[]>;
    getCryptoTickers(): Promise<CryptoTickerSelectModel[]>;
    createCryptoTrade(tradeModel: CryptoTradeViewModel): Promise<void>;
    updateCryptoTrade(tradeModel: CryptoTradeViewModel): Promise<void>;
    deleteCryptoTrade(tradeId: number): Promise<void>;
    getRawTradeData(): Promise<TradeHistory[]>;
    getExchangeRate(from: string, to: string): Promise<number>;
    getCryptoCurrentNetWorth(currency: string): Promise<number>;
    getMonthlyGroupedAccumulatedCrypto(fromDate: Date, toDate: Date, trades: TradeHistory[], currency: string): Promise<NetWorthMonthGroupModel[]>;
    getCryptoCurrentPrice(ticker: string): Promise<number>;
    calculateCryptoTotalUsdValue(tradeHistory: TradeHistory[], ticker: string, finalCurrency: ForexSymbol): Promise<CryptoCalculationModel>;
}
