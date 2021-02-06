import moment from "moment";
import React from "react";
import { Configuration, CryptoApi, CryptoApiInterface, TradeHistory } from "../../ApiClient";
import * as _ from "lodash"

class CryptoSum {
    ticker: string;
    sum: number;
}

class CryptoPortfolioState {
    allCryptoSum: CryptoSum[];
}

export default class CryptoPortfolio extends React.Component<{}, CryptoPortfolioState> {
    cryptoInterface: CryptoApiInterface;

    constructor(props: {}) {
        super(props);
        this.cryptoInterface = new CryptoApi(new Configuration({ basePath: "https://localhost:5001" }));
        this.state = { allCryptoSum: undefined };
    }

    public componentDidMount() {
        this.load();
    }

    private async load(): Promise<void> {
        let trades: TradeHistory[] = await this.cryptoInterface.cryptoGetAllGet();
        let groupedTrades = _.groupBy(trades, t => t.cryptoTicker);
        let cryptoSums: CryptoSum[] = [];

        _.forOwn(groupedTrades, function (value: TradeHistory[], key) {
            console.log(key);
            let sum = value.reduce((partial_sum, v) => partial_sum + v.tradeSize, 0)
            cryptoSums.push({ sum: sum, ticker: key });
        });

        this.setState({ allCryptoSum: cryptoSums });
    }

    render() {
        return (
            <div>
                <h2 className="text-xl ml-12 mb-10">Portfolio</h2>
                {this.state.allCryptoSum != undefined ?
                    <div className="pb-10 h-64 overflow-y-scroll">
                        {this.state.allCryptoSum.map(p =>
                            <div key={p.ticker} className="paymentRecord bg-battleshipGrey rounded-r-full flex mr-6 mt-1 hover:bg-vermilion cursor-pointer">
                                <p className="mx-6 my-1 w-1/3">{p.ticker}</p>
                                <p className="mx-6 my-1 w-2/3">{p.sum}</p>
                            </div>
                        )}
                    </div>
                    : <div>
                        <p>Probíhá načátíní</p>
                    </div>
                }
            </div>
        );
    }
}