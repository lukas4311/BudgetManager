import React from "react";
import ReactDOM from "react-dom";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";
import Menu from "./Components/Menu";
import Overview from './Overview';
import Crypto from './Crypto';
import ErrorBoundary from "./Utils/ErrorBoundry";
import PaymentsOverview from "./Components/Payments/PaymentsOverview";
import BudgetComponent from "./Components/Budget/BudgetComponent";
import { IconsData } from "./Enums/IconsEnum";
import moment from "moment";
import Auth from "./Components/Auth/Auth";
import PrivateRoute from "./Utils/PrivateRoute";
import BankAccountOverview from "./Components/BankAccount/BankAccountOverview";


export default function App() {
    return (
        <Router>
            <div className="bg-mainDarkBlue bg-black h-full flex flex-col overflow-x-hidden">
                <header className="bg-mainDarkBlue flex flex-row text-white pt-4 pb-2 px-12">
                    <div>
                        {/* {new IconsData().logo} */}
                        <img src="./images/logo.png" alt="logo" className="w-2/12 inline-block"/>
                        <nav id="navMenu">
                            <Menu></Menu>
                        </nav>
                    </div>
                    <nav id="navMenu"></nav>
                </header>
                <div className="baseContainer mx-auto lg:w-11/12 w-full">
                    <main role="main" className="pb-3 text-white">
                        <Switch>
                            <Route path="/login" component={Auth} />
                            <PrivateRoute path="/payments" component={PaymentsOverview} />
                            <PrivateRoute path="/crypto-overview" component={Crypto} />
                            <PrivateRoute path="/budget" component={BudgetComponent} />
                            <PrivateRoute path="/bankaccount-overview" component={BankAccountOverview} />
                            <PrivateRoute path="/" component={Overview} />
                        </Switch>
                    </main>
                </div>

                <footer className="text-center  m-4 text-white">
                    <span className="m-auto">{moment().format('YYYY-MM-DD') + " - Budget&Investment"}</span>
                </footer>
                <script src="~/js/site.js" asp-append-version="true"></script>
                <script src="~/js/menu.js"></script>
            </div>
        </Router>
    );
}

ReactDOM.render(<ErrorBoundary><App /></ErrorBoundary>, document.getElementById('app'));