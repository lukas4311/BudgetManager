import { TradeHistory } from "../ApiClient/Main/models";
import CryptoTickerSelectModel from "../Components/Crypto/CryptoTickerSelectModel";
import { CryptoTradeViewModel } from "../Components/Crypto/CryptoTradeForm";
import { NetWorthMonthGroupModel } from "./NetWorthService";

export interface ICryptoService {
    getTradeData(): Promise<CryptoTradeViewModel[]>;
    getCryptoTickers(): Promise<CryptoTickerSelectModel[]>;
    createCryptoTrade(tradeModel: CryptoTradeViewModel): Promise<void>;
    updateCryptoTrade(tradeModel: CryptoTradeViewModel): Promise<void>;
    deleteCryptoTrade(tradeId: number): Promise<void>;
    getRawTradeData(): Promise<TradeHistory[]>;
    getExchangeRate(from: string, to: string): Promise<number>;
    getCryptoCurrentNetWorth(currency: string): Promise<number>;
    getMonthlyGroupedAccumulatedCrypto(trades: TradeHistory[], currency: string): Promise<NetWorthMonthGroupModel[]>;
}
