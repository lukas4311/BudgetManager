import { CurrencyApiInterface } from "../ApiClient/Main";
import CurrencyTickerSelectModel from "../Components/Crypto/CurrencyTickerSelectModel";
import { ICurrencyService } from "./ICurrencyService";

export class CurrencyService implements ICurrencyService {
    private currencyApi: CurrencyApiInterface;

    constructor(currencyApi: CurrencyApiInterface) {
        this.currencyApi = currencyApi;
    }

    public async getAllCurrencies(): Promise<CurrencyTickerSelectModel[]> {
        return (await this.currencyApi.currencyAllGet()).map(c => ({ id: c.id, ticker: c.symbol }));
    }
}