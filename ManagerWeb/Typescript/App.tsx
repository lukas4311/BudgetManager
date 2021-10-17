import React from "react";
import ReactDOM from "react-dom";
import {
    BrowserRouter as Router,
    Switch,
    Route,
    Link,
} from "react-router-dom";
import Menu from "./Components/Menu";
import Overview from './Overview';
import Crypto from './Crypto';
import ErrorBoundary from "./Utils/ErrorBoundry";
import PaymentsOverview from "./Components/Payments/PaymentsOverview";
import BudgetComponent from "./Components/Budget/BudgetComponent";
import { IconsData } from "./Enums/IconsEnum";


export default function App() {
    return (
        <Router>
            <div className="bg-mainDarkBlue bg-black h-full flex flex-col overflow-x-hidden">
                <header className="bg-mainDarkBlue flex flex-row text-white p-4 px-12">
                    <div>
                        {new IconsData().logo}
                        <nav id="navMenu">
                            <Menu></Menu>
                        </nav>
                    </div>
                    <nav id="navMenu"></nav>
                </header>
                <div className="baseContainer mx-auto lg:w-11/12 w-full">
                    <main role="main" className="pb-3 text-white">
                        <Switch>
                            <Route path="/payments">
                                <PaymentsOverview />
                            </Route>
                            <Route path="/crypto-overview">
                                <Crypto />
                            </Route>
                            <Route path="/budget">
                                <BudgetComponent />
                            </Route>
                            <Route path="/">
                                <Overview />
                            </Route>
                        </Switch>
                    </main>
                </div>

                <footer className="text-center  m-4 text-white">
                    {/* <span className="m-auto">@DateTime.Now.Year - Budget&Investment</span> */}FOOTER
                </footer>
                <script src="~/js/site.js" asp-append-version="true"></script>
                <script src="~/js/menu.js"></script>
                @RenderSection("Scripts", required: false)
            </div>




            {/* <div>
                <ul>
                    <li>
                        <Link to="/">Home</Link>
                    </li>
                    <li>
                        <Link to="/about">About</Link>
                    </li>
                    <li>
                        <Link to="/topics">Topics</Link>
                    </li>
                </ul>


            </div> */}
        </Router>
    );
}

ReactDOM.render(<ErrorBoundary><App /></ErrorBoundary>, document.getElementById('app'));