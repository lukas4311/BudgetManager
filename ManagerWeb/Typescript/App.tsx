import React, { useEffect, useState } from "react";
import ReactDOM from "react-dom";
import { AppContext, AppCtx } from "./Context/AppCtx";
import { BrowserRouter as Router, Switch, Route } from "react-router-dom";
import Menu from "./Components/Menu";
import Overview from './Overview';
import Crypto from './Crypto';
import ErrorBoundary from "./Utils/ErrorBoundry";
import PaymentsOverview from "./Components/Payments/PaymentsOverview";
import BudgetComponent from "./Components/Budget/BudgetComponent";
import moment from "moment";
import Auth from "./Components/Auth/Auth";
import PrivateRoute from "./Utils/PrivateRoute";
import BankAccountOverview from "./Components/BankAccount/BankAccountOverview";
import Comodities from "./Components/Comodities/Comodities";
import OtherInvestmentOverview from "./Components/OtherInvestment/OtherInvestmentOverview";
import StockOverview from "./Components/Stocks/StockOverview";
import { SpinnerCircularSplit } from 'spinners-react';
import DataLoader from "./Services/DataLoader";
import ApiUrls from "./Model/Setting/ApiUrl";
import NetWorthOverview from "./Components/NetWorth/NetWorthOverview";
import { createTheme, ThemeProvider } from '@mui/material/styles';

const theme = createTheme({
    palette: {
      mode: 'dark',
    },
    components: {
      MuiTextField: {
        styleOverrides: {
          root: {
            '& .MuiInputBase-input': {
              color: 'white',
            },
            '& .MuiOutlinedInput-root': {
              '& fieldset': {
                borderColor: 'white',
              },
              '&:hover fieldset': {
                borderColor: 'white',
              },
              '&.Mui-focused fieldset': {
                borderColor: 'white',
              },
            },
          },
        },
      },
    },
  });

// const useStyles = makeStyles((theme) => {
//     root: {

//     }
// });

export default function App() {
    const [isLoaded, setIsLoaded] = useState<boolean>(false);
    const [context, setContext] = useState<AppContext>({ apiUrls: { authApi: "aaa", mainApi: "bbb", finApi: "fff" } });

    useEffect(() => {
        async function getSetting() {
            let settingLoader = new DataLoader();
            let setting: ApiUrls = await settingLoader.getSetting();
            setContext({ apiUrls: setting });
            setIsLoaded(true);
        };

        getSetting();
    }, []);

    return (
        <React.Fragment>
            <AppCtx.Provider value={context}>
                {isLoaded
                    ? (
                        <Router>
                            <ThemeProvider theme={theme}>
                                <div className="bg-mainDarkBlue h-full flex flex-col overflow-x-hidden">
                                    <header className="bg-mainDarkBlue flex flex-row text-white pt-4 pb-2 px-12">
                                        <div>
                                            <nav id="navMenu">
                                                <Menu></Menu>
                                            </nav>
                                        </div>
                                        <nav id="navMenu"></nav>
                                    </header>
                                    <div className="baseContainer mx-auto lg:w-11/12 w-full flex-grow">
                                        <main role="main" className="pb-3 text-white">
                                            <Switch>
                                                <Route path="/login" component={Auth} />
                                                <PrivateRoute path="/payments" component={PaymentsOverview} />
                                                <PrivateRoute path="/crypto-overview" component={Crypto} />
                                                <PrivateRoute path="/budget" component={BudgetComponent} />
                                                <PrivateRoute path="/bankaccount-overview" component={BankAccountOverview} />
                                                <PrivateRoute path="/comodity" component={Comodities} />
                                                <PrivateRoute path="/other-investment" component={OtherInvestmentOverview} />
                                                <PrivateRoute path="/stock" component={StockOverview} />
                                                <PrivateRoute path="/" component={NetWorthOverview} />
                                            </Switch>
                                        </main>
                                    </div>

                                    <footer className="text-center  m-4 text-white">
                                        <span className="m-auto">{moment().format('YYYY-MM-DD') + " - Budget&Investment"}</span>
                                    </footer>
                                    <script src="~/js/site.js" asp-append-version="true"></script>
                                    <script src="~/js/menu.js"></script>
                                </div>
                            </ThemeProvider>
                        </Router>
                    )
                    : (
                        <div className="flex text-center justify-center h-full">
                            {/* <div id="loading"></div> */}
                            <SpinnerCircularSplit size={150} thickness={110} speed={70} color="rgba(27, 39, 55, 1)" secondaryColor="rgba(224, 61, 21, 1)" />
                        </div>
                    )
                }
            </AppCtx.Provider>
        </React.Fragment>
    );
}

ReactDOM.render(<ErrorBoundary><App /></ErrorBoundary>, document.getElementById('app'));