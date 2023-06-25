import moment from "moment";
import { CryptoApiInterface, TradeHistory } from "../ApiClient/Main";
import CryptoTickerSelectModel from "../Components/Crypto/CryptoTickerSelectModel";
import { CryptoTradeViewModel } from "../Components/Crypto/CryptoTradeForm";
import { ICryptoService } from "./ICryptoService";
import _ from "lodash";

const usdSymbol = "USD";
const czkSymbol = "CZK";

export default class CryptoService implements ICryptoService {
    private cryptoApi: CryptoApiInterface;

    constructor(cryptoApi: CryptoApiInterface) {
        this.cryptoApi = cryptoApi;
    }

    public async getTradeData(): Promise<CryptoTradeViewModel[]> {
        const data = await this.cryptoApi.cryptosAllGet();
        let trades: CryptoTradeViewModel[] = data.map(t => this.mapDataModelToViewModel(t));
        return trades;
    }

    public async getRawTradeData(): Promise<TradeHistory[]> {
        return await this.cryptoApi.cryptosAllGet();
    }

    public async getCryptoTickers(): Promise<CryptoTickerSelectModel[]> {
        return (await this.cryptoApi.cryptosTickersGet()).map(c => ({ id: c.id, ticker: c.ticker }))
    }

    public async createCryptoTrade(tradeModel: CryptoTradeViewModel) {
        const tradeHistory = this.mapViewModelToDataModel(tradeModel);
        await this.cryptoApi.cryptosPost({ tradeHistory: tradeHistory });
    }

    public async updateCryptoTrade(tradeModel: CryptoTradeViewModel) {
        const tradeHistory = this.mapViewModelToDataModel(tradeModel);
        await this.cryptoApi.cryptosPut({ tradeHistory: tradeHistory });
    }

    public async deleteCryptoTrade(tradeId: number) {
        await this.cryptoApi.cryptosDelete({ body: tradeId });
    }

    public async getExchangeRate(from: string, to: string): Promise<number> {
        return await this.cryptoApi.cryptosActualExchangeRateFromCurrencyToCurrencyGet({ fromCurrency: from, toCurrency: to });
    }

    public async getCryptoCurrentNetWorth(currency: string): Promise<number> {
        let cryptoSum = 0;
        let trades: TradeHistory[] = await this.getRawTradeData();
        let groupedTrades = _.chain(trades).groupBy(t => t.cryptoTicker)
            .map((value, key) => ({ ticker: key, sum: _.sumBy(value, s => s.tradeSize) }))
            .value();

        const finalCurrencyExcahngeRate = await this.getExchangeRate(usdSymbol, currency);

        for (const ticker of groupedTrades) {
            const dollarExcahngeRate = await this.getExchangeRate(ticker.ticker, usdSymbol);
            const finalMultiplier = dollarExcahngeRate * finalCurrencyExcahngeRate;

            if (finalMultiplier != 0) {
                const sumedInFinalCurrency = finalMultiplier * ticker.sum;
                cryptoSum += sumedInFinalCurrency;
            }
        }

        return cryptoSum;
    }

    private mapViewModelToDataModel = (tradeModel: CryptoTradeViewModel) => {
        const tradeHistory: TradeHistory = {
            cryptoTickerId: tradeModel.cryptoTickerId,
            currencySymbolId: tradeModel.currencySymbolId,
            id: tradeModel.id,
            tradeSize: tradeModel.tradeSize,
            tradeTimeStamp: moment(tradeModel.tradeTimeStamp).toDate(),
            tradeValue: tradeModel.tradeValue
        };

        return tradeHistory;
    }

    private mapDataModelToViewModel = (tradeHistory: TradeHistory): CryptoTradeViewModel => {
        let model: CryptoTradeViewModel = new CryptoTradeViewModel();
        model.cryptoTicker = tradeHistory.cryptoTicker;
        model.cryptoTickerId = tradeHistory.cryptoTickerId;
        model.currencySymbol = tradeHistory.currencySymbol;
        model.currencySymbolId = tradeHistory.currencySymbolId;
        model.id = tradeHistory.id;
        model.tradeSize = tradeHistory.tradeSize;
        model.tradeTimeStamp = moment(tradeHistory.tradeTimeStamp).format("YYYY-MM-DD");
        model.tradeValue = tradeHistory.tradeValue;
        // model.onSave = this.saveTrade;
        // model.currencies = this.currencies;
        // model.cryptoTickers = this.cryptoTickers;
        return model;
    }
}