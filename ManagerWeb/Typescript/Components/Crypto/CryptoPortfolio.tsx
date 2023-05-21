import React from "react";
import { Configuration, CryptoApi, CryptoApiInterface, TradeHistory } from "../../ApiClient/Main";
import _ from "lodash";
import { PieChart, PieChartData } from "../Charts/PieChart";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { RouteComponentProps } from "react-router-dom";
import { ComponentPanel } from "../../Utils/ComponentPanel";

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

export default class CryptoPortfolio extends React.Component<RouteComponentProps, CryptoPortfolioState> {
    cryptoApi: CryptoApiInterface;

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

        let trades: TradeHistory[] = await this.cryptoApi.cryptosAllGet();
        let groupedTrades = _.groupBy(trades, t => t.cryptoTicker);
        let cryptoSums: CryptoSum[] = [];
        let that = this;

        _.forOwn(groupedTrades, async function (value: TradeHistory[], key) {
            let sumTradeSize = value.reduce((partial_sum, v) => partial_sum + v.tradeSize, 0);
            let exhangeRateTrade: number = await that.cryptoApi.cryptosActualExchangeRateFromCurrencyToCurrencyGet({ fromCurrency: key, toCurrency: usdSymbol });

            let sumValue = value.reduce((partial_sum, v) => partial_sum + v.tradeValue, 0);
            let exhangeRate: number = await that.cryptoApi.cryptosActualExchangeRateFromCurrencyToCurrencyGet({ fromCurrency: value[0].currencySymbol, toCurrency: usdSymbol });

            cryptoSums.push({ tradeSizeSum: sumTradeSize, ticker: key, tradeValueSum: sumValue, valueTicker: value[0].currencySymbol, usdPrice: sumValue * exhangeRate, usdPriceTrade: sumTradeSize * exhangeRateTrade });
            cryptoSums = _.orderBy(cryptoSums, a => a.tradeValueSum, 'asc');
            that.setState({ allCryptoSum: cryptoSums });
        });
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