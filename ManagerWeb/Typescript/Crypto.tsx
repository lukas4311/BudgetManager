import React from "react";
import ReactDOM from "react-dom";
import CryptoPortfolio from "./Components/Crypto/CryptoPortfolio";
import { CryptoTradeForm } from "./Components/Crypto/CryptoTradeForm";
import CryptoPayments from "./Components/Crypto/CryptoTrades";
import ErrorBoundary from "./Utils/ErrorBoundry";

class CryptoComponent extends React.Component<{}, {}>{
    render() {
        return (
            <div className="">
                <p className="text-3xl text-center mt-6">Crypto přehled</p>
                <div className="flex">
                    <div className="w-1/2 p-4 overflow-y-auto"><CryptoPayments></CryptoPayments></div>
                    <div className="w-1/2 p-4 overflow-y-auto"><CryptoPortfolio></CryptoPortfolio></div>
                    <div className="w-1/2 p-4 overflow-y-auto"><CryptoTradeForm></CryptoTradeForm></div>
                </div>
            </div>
        );
    }
}

ReactDOM.render(<ErrorBoundary><CryptoComponent /></ErrorBoundary>, document.getElementById('overview'));