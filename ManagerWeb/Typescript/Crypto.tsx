import React from "react";
import { RouteComponentProps } from "react-router";
import CryptoPortfolio from "./Components/Crypto/CryptoPortfolio";
import CryptoPayments from "./Components/Crypto/CryptoTrades";

export default class Crypto extends React.Component<RouteComponentProps, {}>{
    render() {
        return (
            <div className="">
                <p className="text-3xl text-center mt-6">Crypto overview</p>
                <div className="flex w-full text-center mt-6 p-4 bg-prussianBlue rounded-lg">
                    <div className="w-7/12 p-4 overflow-y-auto"><CryptoPayments {...this.props}></CryptoPayments></div>
                    <div className="w-5/12 p-4 overflow-y-auto"><CryptoPortfolio {...this.props}></CryptoPortfolio></div>
                </div>
            </div>
        );
    }
}