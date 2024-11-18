import React from "react";
import { CryptoApi, CryptoApiInterface, TradeHistory } from "../../ApiClient/Main";
import _ from "lodash";
import { PieChart, PieChartData } from "../Charts/PieChart";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { RouteComponentProps } from "react-router-dom";
import { ComponentPanel } from "../../Utils/ComponentPanel";
import { CryptoEndpointsApi, ForexEndpointsApi } from "../../ApiClient/Fin";
import { CurrencySymbol as ForexSymbol } from "../../ApiClient/Fin";
import CryptoService from "../../Services/CryptoService";


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
        this.cryptoService = new CryptoService(this.cryptoApi, this.forexFinApi, this.cryptoFinApi, this.forexFinApi);

        let trades: TradeHistory[] = await this.cryptoApi.cryptosAllGet();
        let groupedTrades = _.groupBy(trades, t => t.cryptoTicker);
        let cryptoSums: CryptoSum[] = [];

        for (let ticker in groupedTrades) {
            let value = groupedTrades[ticker];
            const cryptoCalculationModel = await this.cryptoService.calculateCryptoTotalUsdValue(value, ticker, ForexSymbol.Czk);

            cryptoSums.push({
                tradeSizeSum: cryptoCalculationModel.totalyStacked, ticker: ticker, tradeValueSum: cryptoCalculationModel.usdSum, valueTicker: value[0].currencySymbol,
                finalCurrencyPrice: cryptoCalculationModel.finalCurrencyPrice, finalCurrencyPriceTrade: cryptoCalculationModel.finalCurrencyPriceTrade
            });
        }

        cryptoSums = _.orderBy(cryptoSums, a => a.finalCurrencyPriceTrade, 'desc');
        this.setState({ allCryptoSum: cryptoSums });
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
            <ComponentPanel classStyle="p-5">
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