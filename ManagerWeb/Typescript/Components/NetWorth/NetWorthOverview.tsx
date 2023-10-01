import React, { Component } from 'react'
import NetWorthService from '../../Services/NetWorthService';
import PaymentService from '../../Services/PaymentService';
import StockService from '../../Services/StockService';
import CryptoService from '../../Services/CryptoService';
import { RouteComponentProps } from 'react-router-dom';
import { BankAccountApi, ComodityApi, CryptoApi, OtherInvestmentApi, PaymentApi, StockApi } from '../../ApiClient/Main';
import BankAccountService from '../../Services/BankAccountService';
import OtherInvestmentService from '../../Services/OtherInvestmentService';
import ApiClientFactory from '../../Utils/ApiClientFactory';
import { SpinnerCircularSplit } from 'spinners-react';
import { ThemeProvider, createMuiTheme } from '@material-ui/core';
import { MainFrame } from '../MainFrame';
import { ICryptoService } from '../../Services/ICryptoService';
import ComodityService from '../../Services/ComodityService';
import { ComponentPanel } from '../../Utils/ComponentPanel';
import { ComodityEndpointsApi, CryptoEndpointsApi, ForexEndpointsApi, StockEndpointsApi } from '../../ApiClient/Fin';

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
    netWorth: number;
}

export default class NetWorthOverview extends Component<RouteComponentProps, NetWorthOverviewState> {
    netWorthService: NetWorthService;
    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { loading: true, netWorth: 0 };
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
        const comodityApi = await apiFactory.getClient(ComodityApi);
        const comodityFinApi = await apiFactory.getFinClient(ComodityEndpointsApi);
        const otherInvestmentApi = await apiFactory.getClient(OtherInvestmentApi);
        const forexApi = await apiFactory.getFinClient(ForexEndpointsApi);
        const cryptoFinApi = await apiFactory.getFinClient(CryptoEndpointsApi);
        const stockFinApi = await apiFactory.getFinClient(StockEndpointsApi);
        const cryptoService: ICryptoService = new CryptoService(cryptoApi, forexApi, cryptoFinApi, forexApi);
        this.netWorthService = new NetWorthService(new PaymentService(paymentApi), new StockService(stockApi, cryptoService, forexApi, stockFinApi), cryptoService, new OtherInvestmentService(otherInvestmentApi), new BankAccountService(bankAccountApi), new ComodityService(comodityApi, comodityFinApi));
        // const data = await this.netWorthService.getCurrentNetWorth();
        await this.netWorthService.getNetWorthGroupedByMonth();
        this.setState({ loading: false, netWorth: 0 });
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
                                    <div className='flex flex-row'>
                                        <ComponentPanel classStyle="w-1/2">
                                            <h2>Your net worth is {this.state.netWorth.toFixed(0)}</h2>
                                        </ComponentPanel>
                                    </div>
                            }
                        </React.Fragment>
                    </MainFrame>
                </div >
            </ ThemeProvider >
        )
    }
}
