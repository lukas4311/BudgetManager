import React from "react";
import { Configuration, CryptoApi, CryptoApiInterface, CurrencySymbol, TradeHistory } from "../../ApiClient/Main";
import _ from "lodash";
import { PieChart, PieChartData } from "../Charts/PieChart";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { RouteComponentProps } from "react-router-dom";
import { ComponentPanel } from "../../Utils/ComponentPanel";
import { CryptoEndpointsApi, ForexEndpointsApi } from "../../ApiClient/Fin";
import moment from "moment";
import { CurrencySymbol as ForexSymbol } from "../../ApiClient/Fin";
import CryptoService from "../../Services/CryptoService";

const usdSymbol = "USD";
const stableCoins = ["USDC", "USDT", "BUSD"]

class CryptoSum {
    ticker: string;
    tradeSizeSum: number;
    tradeValueSum: number;
    valueTicker: string;
    finalCurrencyPrice: number;
    finalCurrencyPriceTrade: number;
}

class CryptoPortfolioState {
    allCryptoSum: CryptoSum[];
}

export default class CryptoPortfolio extends React.Component<RouteComponentProps, CryptoPortfolioState> {
    private cryptoApi: CryptoApiInterface;
    private cryptoFinApi: CryptoEndpointsApi;
    private forexFinApi: ForexEndpointsApi;
    private cryptoService: CryptoService;

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { allCryptoSum: undefined };
    }

    public componentDidMount() {
        this.load();
    }

    private load = async (): Promise<void> => {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.cryptoApi = await apiFactory.getClient(CryptoApi);
        this.cryptoFinApi = await apiFactory.getFinClient(CryptoEndpointsApi);
        this.forexFinApi = await apiFactory.getFinClient(ForexEndpointsApi);
        this.cryptoService = new CryptoService(this.cryptoApi, this.forexFinApi, this.cryptoFinApi);

        let trades: TradeHistory[] = await this.cryptoApi.cryptosAllGet();
        let groupedTrades = _.groupBy(trades, t => t.cryptoTicker);
        let cryptoSums: CryptoSum[] = [];
        let that = this;

        for (let ticker in groupedTrades) {
            let value = groupedTrades[ticker];

            let totalyStacked = value.reduce((partial_sum, v) => partial_sum + v.tradeSize, 0);

            let date = moment(Date.now()).subtract(1, 'd').toDate();
            let exhangeRateTrade: number = await this.cryptoService.getCryptoCurrentPrice(ticker);

            const usdSum = await this.calculateCryptoTradesUsdSum(value);
            const finalExhangeRate = await that.forexFinApi.getForexPairPriceAtDate({ date: date, from: ForexSymbol.Usd, to: ForexSymbol.Czk });

            cryptoSums.push({
                tradeSizeSum: totalyStacked, ticker: ticker, tradeValueSum: usdSum, valueTicker: value[0].currencySymbol,
                finalCurrencyPrice: usdSum * finalExhangeRate, finalCurrencyPriceTrade: totalyStacked * exhangeRateTrade * finalExhangeRate
            });

        }

        cryptoSums = _.orderBy(cryptoSums, a => a.finalCurrencyPriceTrade, 'desc');
        that.setState({ allCryptoSum: cryptoSums });
    }

    private calculateCryptoTradesUsdSum = async (tradeHistory: TradeHistory[]): Promise<number> => {
        let date = moment(Date.now()).subtract(1, 'd').toDate();
        let sum = 0;

        for (const trade of tradeHistory) {
            let exhangeRate: number = 1
            let forexSymbol = this.convertStringToForexEnum(trade.currencySymbol);

            if (forexSymbol)
                exhangeRate = await this.forexFinApi.getForexPairPriceAtDate({ date: date, from: forexSymbol, to: ForexSymbol.Usd });
            else if (_.some(stableCoins, c => c == trade.currencySymbol))
                exhangeRate = 1;
            else {
                const cryptoPrice = await this.cryptoFinApi.getCryptoPriceDataAtDate({ date: trade.tradeTimeStamp, ticker: _.upperCase(trade.currencySymbol) });

                if (cryptoPrice)
                    exhangeRate = cryptoPrice.price;
            }

            sum += Math.abs(trade.tradeValue) * exhangeRate;
        }

        return sum;
    }

    private convertStringToForexEnum(value: string): ForexSymbol | undefined {
        if (Object.values(ForexSymbol).includes(value as ForexSymbol)) {
            return value as ForexSymbol;
        }

        return undefined;
    }

    private renderChart = () => {
        let element: JSX.Element;

        if (this.state.allCryptoSum != undefined && this.state.allCryptoSum.length != 0) {
            let chartData: PieChartData[] = this.state.allCryptoSum.map(a => ({ id: a.ticker, label: a.ticker, value: Math.floor(a.finalCurrencyPriceTrade) }));
            element = (
                <div className="h-96">
                    <PieChart data={chartData} labelPostfix="CZK"></PieChart>
                </div>
            )
        }

        return element;
    }

    render() {
        return (
            <ComponentPanel>
                <div>
                    <h2 className="text-xl ml-12">Crypto portfolio</h2>
                    {this.state.allCryptoSum != undefined ?
                        <div className="pb-10 overflow-y-scroll">
                            <div className="font-bold bg-battleshipGrey rounded-r-full flex mr-6 mt-1">
                                <p className="mx-6 my-1 w-1/3">Ticker</p>
                                <p className="mx-6 my-1 w-1/3">Stacked amount</p>
                                <p className="mx-6 my-1 w-1/3">Current value</p>
                                <p className="mx-6 my-1 w-1/3">Bought for</p>
                            </div>
                            {this.state.allCryptoSum.map(p =>
                                <div key={p.ticker} className="paymentRecord bg-battleshipGrey rounded-r-full flex mr-6 mt-1 hover:bg-vermilion cursor-pointer">
                                    <p className="mx-6 my-1 w-1/3">{p.ticker.toUpperCase()}</p>
                                    <p className="mx-6 my-1 w-1/3">{p.tradeSizeSum.toFixed(3)}</p>
                                    <p className="mx-6 my-1 w-1/3">{p.finalCurrencyPriceTrade.toFixed(2)} CZK</p>
                                    <p className="mx-6 my-1 w-1/3">{p.finalCurrencyPrice.toFixed(2)} CZK</p>
                                </div>
                            )}
                        </div>
                        : <div>
                            <p>Loading ...</p>
                        </div>
                    }
                    {this.renderChart()}
                </div>
            </ComponentPanel>
        );
    }
}