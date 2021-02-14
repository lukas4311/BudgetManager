import moment from "moment";
import React from "react";
import { Configuration, CryptoApi, CryptoApiInterface, TradeHistory } from "../../ApiClient";

class CryptoTradesState {
    trades: TradeHistory[];
}

export default class CryptoTrades extends React.Component<{}, CryptoTradesState> {
    cryptoInterface: CryptoApiInterface;

    constructor(props: {}) {
        super(props);
        this.cryptoInterface = new CryptoApi(new Configuration({ basePath: "https://localhost:5001" }));
        this.state = { trades: undefined };
    }

    public componentDidMount() {
        this.load();
    }

    private async load(): Promise<void> {
        let trades: TradeHistory[] = await this.cryptoInterface.cryptoGetAllGet();
        trades.sort((a, b) => moment(a.tradeTimeStamp).format("YYYY-MM-DD") > moment(b.tradeTimeStamp).format("YYYY-MM-DD") ? 1 : -1);
        this.setState({ trades });
    }

    render() {
        return (
            <div className="">
                <h2 className="text-xl ml-12 mb-10">Seznam plateb</h2>
                {this.state.trades != undefined ?
                    <div className="pb-10 max-h-96 overflow-x-auto">
                        <div className="font-bold bg-battleshipGrey rounded-r-full flex mr-6 mt-1 hover:bg-vermilion cursor-pointer">
                                <p className="mx-6 my-1 w-1/10">Ticker</p>
                                <p className="mx-6 my-1 w-3/10">Velikost tradu</p>
                                <p className="mx-6 my-1 w-2/10">Datum tradu</p>
                                <p className="mx-6 my-1 w-3/10">Celkova hodnota</p>
                                <p className="mx-6 my-1 w-1/10">Měna</p>
                            </div>
                        {this.state.trades.map(p =>
                            <div key={p.id} className="paymentRecord bg-battleshipGrey rounded-r-full flex mr-6 mt-1 hover:bg-vermilion cursor-pointer">
                                <p className="mx-6 my-1 w-1/10">{p.cryptoTicker.toUpperCase()}</p>
                                <p className="mx-6 my-1 w-3/10">{p.tradeSize}</p>
                                <p className="mx-6 my-1 w-2/10">{moment(p.tradeTimeStamp).format('DD.MM.YYYY')}</p>
                                <p className="mx-6 my-1 w-3/10">{p.tradeValue.toFixed(2)}</p>
                                <p className="mx-6 my-1 w-1/10">{p.currencySymbol}</p>
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