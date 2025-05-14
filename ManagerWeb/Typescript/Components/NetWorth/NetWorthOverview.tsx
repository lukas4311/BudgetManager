import React, { Component } from 'react'
import NetWorthService, { NetWorthMonthGroupModel, TotalNetWorthDetail } from '../../Services/NetWorthService';
import PaymentService from '../../Services/PaymentService';
import StockService from '../../Services/StockService';
import CryptoService from '../../Services/CryptoService';
import { RouteComponentProps } from 'react-router-dom';
import { BankAccountApi, ComodityApi, CryptoApi, OtherInvestmentApi, PaymentApi, StockApi } from '../../ApiClient/Main';
import BankAccountService from '../../Services/BankAccountService';
import OtherInvestmentService from '../../Services/OtherInvestmentService';
import ApiClientFactory from '../../Utils/ApiClientFactory';
import { SpinnerCircularSplit } from 'spinners-react';
import { MainFrame } from '../MainFrame';
import { ICryptoService } from '../../Services/ICryptoService';
import ComodityService from '../../Services/ComodityService';
import { ComponentPanel } from '../../Utils/ComponentPanel';
import { ComodityEndpointsApi, CryptoEndpointsApi, ForexEndpointsApi, StockEndpointsApi } from '../../ApiClient/Fin';
import { PieChart, PieChartData } from '../Charts/PieChart';
import { LineChart } from '../Charts/LineChart';
import { LineChartSettingManager } from '../Charts/LineChartSettingManager';
import { LineChartData } from '../../Model/LineChartData';
import moment from 'moment';
import { LineChartDataSets } from '../../Model/LineChartDataSets';
import _ from 'lodash';
import { ProgressCalculatorService } from '../../Services/ProgressCalculatorService';

class NetWorthOverviewState {
    loading: boolean;
    netWorth: number;
    netWorthDetail: TotalNetWorthDetail;
    pieDiversityData: PieChartData[];
    netWorthLineChartData: LineChartDataSets[];
}

export default class NetWorthOverview extends Component<RouteComponentProps, NetWorthOverviewState> {
    netWorthService: NetWorthService;
    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { loading: true, netWorth: 0, netWorthDetail: new TotalNetWorthDetail(), pieDiversityData: [], netWorthLineChartData: [] };
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
        const netWorthDetail = await this.netWorthService.getCurrentNetWorth();
        const monthData = await this.netWorthService.getNetWorthGroupedByMonth();

        await this.preparePieChartData(netWorthDetail);
        await this.prepareLineChartData(monthData);
        const progres = this.calculateYoy(monthData);

        this.setState({ loading: false, netWorth: netWorthDetail.total(), netWorthDetail: netWorthDetail });
    }

    private preparePieChartData(netWorthDetail: TotalNetWorthDetail) {
        let pieData: PieChartData[] = [];

        pieData.push({ id: "money", label: "money", value: Math.round(netWorthDetail?.money ?? 0) });
        pieData.push({ id: "stock", label: "stock", value: Math.round(netWorthDetail?.stock ?? 0) });
        pieData.push({ id: "crypto", label: "crypto", value: Math.round(netWorthDetail?.crypto ?? 0) });
        pieData.push({ id: "comodity", label: "comodity", value: Math.round(netWorthDetail?.comodity ?? 0) });
        pieData.push({ id: "other", label: "other", value: Math.round(netWorthDetail?.other ?? 0) });

        this.setState({ pieDiversityData: pieData });
    }

    private prepareLineChartData(monthData: NetWorthMonthGroupModel[]) {
        let netWorthChartData: LineChartData[] = []
        netWorthChartData = monthData.map(b => ({ x: moment(b.date).format('YYYY-MM-DD'), y: b.amount }));
        let chartData = [{ id: 'Net worth', data: netWorthChartData }];
        this.setState({ netWorthLineChartData: chartData });
    }

    private calculateYoy(monthData: NetWorthMonthGroupModel[]) {
        const progressCalculator = new ProgressCalculatorService();
        const last = _.last(monthData);
        const indexOfYearBefore = monthData.length - 12;

        if(indexOfYearBefore > 0){
            const beforeYear = monthData[indexOfYearBefore];
            return progressCalculator.calculareProgress(beforeYear.amount, last.amount);
        }

        return 0;
    }

    render() {
        return (
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
                                    <ComponentPanel classStyle="w-1/2 text-center mr-4">
                                        <React.Fragment>
                                            <h2 className='text-2xl'>Your net worth is</h2>
                                            <h2 className='text-3xl'>{this.state.netWorth.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 })}</h2>
                                            <h3 className='text-2xl'>YoY { }</h3>

                                            <div className="h-96 mt-16">
                                                <LineChart dataSets={this.state.netWorthLineChartData} chartProps={LineChartSettingManager.getNetWorthChartSettingForCompanyInfo()}></LineChart>
                                            </div>
                                        </React.Fragment>
                                    </ComponentPanel>
                                    <ComponentPanel classStyle="w-1/2 text-center">
                                        <React.Fragment>
                                            <div className='px-12'>
                                                <h2 className='text-2xl'>Net worth detail</h2>
                                                <p className='text-xl text-left'>Money net worth: {this.state.netWorthDetail?.money.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 }) ?? 0}</p>
                                                <p className='text-xl text-left'>Stock net worth: {this.state.netWorthDetail?.stock.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 }) ?? 0}</p>
                                                <p className='text-xl text-left'>Crypto net worth: {this.state.netWorthDetail?.crypto.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 }) ?? 0}</p>
                                                <p className='text-xl text-left'>Comodity net worth: {this.state.netWorthDetail?.comodity.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 }) ?? 0}</p>
                                                <p className='text-xl text-left'>Other net worth: {this.state.netWorthDetail?.other.toLocaleString('en-US', { minimumFractionDigits: 0, maximumFractionDigits: 0 }) ?? 0}</p>
                                            </div>
                                            <div className='mt-12'>
                                                <h4 className="text-2xl text-white">Net worth diversification</h4>
                                                <div className="h-96">
                                                    <PieChart data={this.state.pieDiversityData}></PieChart>
                                                </div>
                                            </div>
                                        </React.Fragment>
                                    </ComponentPanel>
                                </div>
                        }
                    </React.Fragment>
                </MainFrame>
            </div >
        )
    }
}
