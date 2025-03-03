import React from "react";
import { RouteComponentProps } from "react-router";
import CryptoPortfolio from "./Components/Crypto/CryptoPortfolio";
import CryptoPayments from "./Components/Crypto/CryptoTrades";
import { MainFrame } from "./Components/MainFrame";

export default class Crypto extends React.Component<RouteComponentProps, {}>{
    render() {
        return (
            <div className="">
                <MainFrame header='Crypto'>
                        <div className="flex flex-row">
                            <div className="w-6/12"><CryptoPayments {...this.props}></CryptoPayments></div>
                            <div className="w-6/12 ml-4"><CryptoPortfolio {...this.props}></CryptoPortfolio></div>
                        </div>
                </MainFrame>
            </div>
        );
    }
}