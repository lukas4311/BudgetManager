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
                    <React.Fragment>
                        <div className="w-7/12 p-4 overflow-y-auto"><CryptoPayments {...this.props}></CryptoPayments></div>
                        <div className="w-5/12 p-4 overflow-y-auto"><CryptoPortfolio {...this.props}></CryptoPortfolio></div>
                    </React.Fragment>
                </MainFrame>
            </div>
        );
    }
}