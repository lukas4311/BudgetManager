import React from "react";
import ReactDOM from "react-dom";
import BankAccountOverview from "./Components/BankAccount/BankAccountOverview";
import ErrorBoundary from "./Utils/ErrorBoundry";

class BankAccounts extends React.Component<{}, {}>{
    render() {
        return (
            <div className="">
                <p className="text-3xl text-center mt-6">Přehled bankovních účtů</p>
                <div className="flex">
                    <div className="w-full p-4 overflow-y-auto"><BankAccountOverview key="overviewBanks"/></div>
                </div>
            </div>
        );
    }
}

ReactDOM.render(<ErrorBoundary><BankAccounts /></ErrorBoundary>, document.getElementById('overview'));