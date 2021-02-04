import React from "react";

class CryptoTrade {
    id:number;
    tradeTimeStamp: Date;
    cryptoTicker: string;
    tradeValue: number;
}



export default class CryptoPayments extends React.Component<{}, {}> {

    render() {
        return (
            <div>Seznam plateb</div>
        );
    }
}