import React, { Component } from 'react'
import NetWorthService from '../../Services/NetWorthService';
import PaymentService from '../../Services/PaymentService';
import StockService from '../../Services/StockService';
import CryptoService from '../../Services/CryptoService';
import { RouteComponentProps } from 'react-router-dom';
import { BankAccountApi, CryptoApi, OtherInvestmentApi, PaymentApi, StockApi } from '../../ApiClient/Main';
import BankAccountService from '../../Services/BankAccountService';
import OtherInvestmentService from '../../Services/OtherInvestmentService';
import ApiClientFactory from '../../Utils/ApiClientFactory';
import { SpinnerCircularSplit } from 'spinners-react';
import { ThemeProvider, createMuiTheme } from '@material-ui/core';
import { MainFrame } from '../MainFrame';

const theme = createMuiTheme({
    palette: {
        type: 'dark',
        primary: {
            main: "#e03d15ff",
        }
    }
});

class NetWorthOverviewState {
    loading: boolean;
}

export default class NetWorthOverview extends Component<RouteComponentProps, NetWorthOverviewState> {
    netWorthService: NetWorthService;
    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { loading: true };
    }

    public componentDidMount() {
        this.init();
    }

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        const bankAccountApi = await apiFactory.getClient(BankAccountApi);
        const paymentApi = await apiFactory.getClient(PaymentApi);
        const stockApi = await apiFactory.getClient(StockApi);
        const cryptoApi = await apiFactory.getClient(CryptoApi);
        const otherInvestmentApi = await apiFactory.getClient(OtherInvestmentApi);
        this.netWorthService = new NetWorthService(new PaymentService(paymentApi), new StockService(stockApi), new CryptoService(cryptoApi), new OtherInvestmentService(otherInvestmentApi), new BankAccountService(bankAccountApi));
        const data = this.netWorthService.getNetWorthHistory();
        this.setState({ loading: false });
    }

    render() {
        return (
            <ThemeProvider theme={theme}>
                <div className="">
                    <MainFrame header='Net worth overview'>
                        <React.Fragment>
                            {
                                this.state.loading ? (
                                    <div className="flex text-center justify-center h-full">
                                        <SpinnerCircularSplit size={150} thickness={110} speed={70} color="rgba(27, 39, 55, 1)" secondaryColor="rgba(224, 61, 21, 1)" />
                                    </div>
                                ) :
                                    <div>
                                        
                                    </div>
                            }
                        </React.Fragment>
                    </MainFrame>
                </div >
            </ ThemeProvider >
        )
    }
}
