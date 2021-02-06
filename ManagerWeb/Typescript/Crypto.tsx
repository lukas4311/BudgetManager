import React from "react";
import ReactDOM from "react-dom";
import CryptoPayments from "./Components/Crypto/CryptoTrades";
import ErrorBoundary from "./Utils/ErrorBoundry";

class CryptoComponent extends React.Component<{},{}>{
    render(){
        return(
            <div className="">
                <p className="text-3xl text-center mt-6">Crypto přehled</p>
                <div className="w-full p-4"><CryptoPayments></CryptoPayments></div>
                
            </div>
        );
    }
}

ReactDOM.render(<ErrorBoundary><CryptoComponent /></ErrorBoundary>, document.getElementById('overview'));