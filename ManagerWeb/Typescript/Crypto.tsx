import React from "react";
import ReactDOM from "react-dom";
import ErrorBoundary from "./Utils/ErrorBoundry";

class CryptoComponent extends React.Component<{},{}>{
    render(){
        return(
            <div className="">
                <p className="text-3xl text-center mt-6">Crypto přehled</p>
                <div className="w-full p-4">NECO CHYTREHO</div>
            </div>
        );
    }
}

ReactDOM.render(<ErrorBoundary><CryptoComponent /></ErrorBoundary>, document.getElementById('overview'));