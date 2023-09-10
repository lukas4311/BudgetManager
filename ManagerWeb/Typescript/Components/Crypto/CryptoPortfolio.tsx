import React from "react";
import { Configuration, CryptoApi, CryptoApiInterface, CurrencySymbol, TradeHistory } from "../../ApiClient/Main";
import _ from "lodash";
import { PieChart, PieChartData } from "../Charts/PieChart";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { RouteComponentProps } from "react-router-dom";
import { ComponentPanel } from "../../Utils/ComponentPanel";
import { CryptoEndpointsApi, ForexEndpointsApi } from "../../ApiClient/Fin";
import moment from "moment";
import {CurrencySymbol as ForexSymbol}  from "../../ApiClient/Fin";

const usdSymbol = "USD";

class CryptoSum {
    ticker: string;
    tradeSizeSum: number;
    tradeValueSum: number;
    valueTicker: string;
    usdPrice: number;
    usdPriceTrade: number;
}

class CryptoPortfolioState {
    allCryptoSum: CryptoSum[];
}

interface ExchangeRateStrategy{
    getExhangeRate();
}

export default class CryptoPortfolio extends React.Component<RouteComponentProps, CryptoPortfolioState> {
    cryptoApi: CryptoApiInterface;
    cryptoFinApi: CryptoEndpointsApi;
    forexFinApi: ForexEndpointsApi;

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

        let trades: TradeHistory[] = await this.cryptoApi.cryptosAllGet();
        let groupedTrades = _.groupBy(trades, t => t.cryptoTicker);
        let cryptoSums: CryptoSum[] = [];
        let that = this;

        _.forOwn(groupedTrades, async function (value: TradeHistory[], key) {
            let date = moment(Date.now()).subtract(1, 'd').toDate();
            let exhangeRateTrade: number = (await that.cryptoFinApi.getCryptoPriceDataAtDate({ ticker: _.upperCase(key), date: date }))?.price ?? 0;
            let sumTradeSize = value.reduce((partial_sum, v) => partial_sum + v.tradeSize, 0);
            
            // TODO: add different exchange rate for different currency
            let uniqueSourceCurrency = _.uniqBy(value, d => d.currencySymbol).map(t => t.currencySymbol);
            console.log("🚀 ~ file: CryptoPortfolio.tsx:58 ~ CryptoPortfolio ~ uniqueSourceCurrency:", uniqueSourceCurrency)
            let sumValue = value.reduce((partial_sum, v) => partial_sum + v.tradeValue, 0);
            let testForexSym = that.convertStringToForexEnum("EUR");
            let exhangeRate: number = await that.forexFinApi.getForexPairPriceAtDate({date: date, from: testForexSym, to: ForexSymbol.Usd});

            cryptoSums.push({ tradeSizeSum: sumTradeSize, ticker: key, tradeValueSum: sumValue, valueTicker: value[0].currencySymbol, usdPrice: sumValue * exhangeRate, usdPriceTrade: sumTradeSize * exhangeRateTrade });
            cryptoSums = _.orderBy(cryptoSums, a => a.tradeValueSum, 'asc');
            that.setState({ allCryptoSum: cryptoSums });
        });
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
            let chartData: PieChartData[] = this.state.allCryptoSum.map(a => ({ id: a.ticker, label: a.ticker, value: Math.floor(a.usdPriceTrade) }));
            element = (
                <div className="h-96">
                    <PieChart data={chartData} labelPostfix="USD"></PieChart>
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
                                <p className="mx-6 my-1 w-1/3">Sum velikosti</p>
                                <p className="mx-6 my-1 w-1/3">Sum hodnoty</p>
                            </div>
                            {this.state.allCryptoSum.map(p =>
                                <div key={p.ticker} className="paymentRecord bg-battleshipGrey rounded-r-full flex mr-6 mt-1 hover:bg-vermilion cursor-pointer">
                                    <p className="mx-6 my-1 w-1/3">{p.ticker.toUpperCase()}</p>
                                    <p className="mx-6 my-1 w-1/3">{p.tradeSizeSum.toFixed(3)}({p.usdPriceTrade.toFixed(2)} USD)</p>
                                    <p className="mx-6 my-1 w-1/3">{p.tradeValueSum.toFixed(2)} USD</p>
                                </div>
                            )}
                        </div>
                        : <div>
                            <p>Probíhá načátíní</p>
                        </div>
                    }
                    {this.renderChart()}
                </div>
            </ComponentPanel>
        );
    }
}