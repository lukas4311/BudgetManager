import { TradeHistory } from "../ApiClient/Main/models";
import CryptoTickerSelectModel from "../Components/Crypto/CryptoTickerSelectModel";
import { CryptoTradeViewModel } from "../Components/Crypto/CryptoTradeForm";

export interface ICryptoService {
    getTradeData(): Promise<CryptoTradeViewModel[]>;
    getCryptoTickers(): Promise<CryptoTickerSelectModel[]>;
    createCryptoTrade(tradeModel: CryptoTradeViewModel): Promise<void>;
    updateCryptoTrade(tradeModel: CryptoTradeViewModel): Promise<void>;
    deleteCryptoTrade(tradeId: number): Promise<void>;
    getRawTradeData(): Promise<TradeHistory[]>;
    getExchangeRate(from: string, to: string): Promise<number>;
}
