import CurrencyTickerSelectModel from "../Components/Crypto/CurrencyTickerSelectModel";


export interface ICurrencyService {
    getAllCurrencies(): Promise<CurrencyTickerSelectModel[]>;
}
