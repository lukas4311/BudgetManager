import React from "react";
import { CryptoApi, CryptoApiInterface } from "../../ApiClient";

class CryptoTradesState {
    trades: any;
}

export default class CryptoTrades extends React.Component<{}, CryptoTradesState> {
    cryptoInterface: CryptoApiInterface;

    constructor(props: {}) {
        super(props);
        this.cryptoInterface = new CryptoApi();
        this.state = { trades: undefined };
    }

    public componentDidMount() {
        this.load();
    }

    private async load(): Promise<void> {
        let trades = await this.cryptoInterface.cryptoGetAllGet();
        this.setState({ trades });
    }

    render() {
        return (
            <div>Seznam plateb</div>
        );
    }
}