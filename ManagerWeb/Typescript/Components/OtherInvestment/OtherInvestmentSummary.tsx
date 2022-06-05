import _ from "lodash";
import moment from "moment";
import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { CurrencyApi, OtherInvestmentApi, OtherInvestmentBalaceHistoryModel, OtherInvestmentBalanceSummaryModel, OtherInvestmentModel, PaymentModel } from "../../ApiClient/Main";
import { LineChartData } from "../../Model/LineChartData";
import { LineChartDataSets } from "../../Model/LineChartDataSets";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { LineChart } from "../Charts/LineChart";
import { LineChartSettingManager } from "../Charts/LineChartSettingManager";
import CurrencyTickerSelectModel from "../Crypto/CurrencyTickerSelectModel";

class OtherInvestmentSummaryState {
    balanceSum: number;
    investedSum: number;
    chartData: LineChartDataSets[];
}

export default class OtherInvestmentSummary extends React.Component<RouteComponentProps, OtherInvestmentSummaryState>{
    private otherInvestmentApi: OtherInvestmentApi;
    private currencies: CurrencyTickerSelectModel[];

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { balanceSum: 0, investedSum: 0, chartData: [] };
    }

    componentDidMount(): void {
        this.initData();
    }

    private initData = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.otherInvestmentApi = await apiFactory.getClient(OtherInvestmentApi);
        const currencyApi = await apiFactory.getClient(CurrencyApi);
        this.currencies = (await currencyApi.currencyAllGet()).map(c => ({ id: c.id, ticker: c.symbol }));
        await this.loadData();
    }

    private async loadData() {
        const summary: OtherInvestmentBalanceSummaryModel = await this.otherInvestmentApi.otherInvestmentSummaryGet();
        const actualSummary: Array<OtherInvestmentBalaceHistoryModel> = summary.actualBalanceData;
        const data: OtherInvestmentModel[] = await this.otherInvestmentApi.otherInvestmentAllGet();

        let investedChartData: LineChartData[] = []
        let balanceChartData: LineChartData[] = []
        let allPayments: PaymentModel[] = [];
        let allBalances: OtherInvestmentBalaceHistoryModel[] = [];

        for (const o of data) {
            const linkedTag = await this.otherInvestmentApi.otherInvestmentIdLinkedTagGet({ id: o.id });
            let linkedPayments: PaymentModel[] = [];

            if (linkedTag != undefined) {
                linkedPayments = await this.otherInvestmentApi.otherInvestmentIdTagedPaymentsTagIdGet({ id: o.id, tagId: linkedTag.tagId });
                allPayments.push(...linkedPayments);
            }

            const otherInvestmentBalance: OtherInvestmentBalaceHistoryModel[] = await this.otherInvestmentApi.otherInvestmentOtherInvestmentIdBalanceHistoryGet({ otherInvestmentId: o.id });
            allBalances.push(...otherInvestmentBalance);
        };

        let investedSum = 0;
        let balanceSum = 0;

        const sortedBalance = _.orderBy(allBalances, [(obj) => new Date(obj.date)], ['asc']);
        balanceChartData = sortedBalance.map(b => ({ x: moment(b.date).format('YYYY-MM-DD'), y: b.balance }));

        const sortedInvested = _.orderBy(allPayments, [(obj) => new Date(obj.date)], ['asc']);
        let prevInvested = 0;
        for (const s of sortedInvested) {
            s.amount += prevInvested;
            prevInvested = s.amount;
        }
        investedChartData = sortedInvested.map(b => ({ x: moment(b.date).format('YYYY-MM-DD'), y: b.amount }));

        let chartData = [{ id: 'Invested', data: investedChartData }, { id: 'Balance', data: balanceChartData }];
        this.setState({ investedSum: investedSum, balanceSum: balanceSum, chartData });
    }

    public render() {
        return (
            <div>
                <h3 className="text-xl p-4 text-center">Other investment summary</h3>
                <p>Balance: {this.state.balanceSum}</p>
                <p>Invested: {this.state.investedSum}</p>

                <div className="h-64">
                    <LineChart dataSets={this.state.chartData} chartProps={LineChartSettingManager.getOtherInvestmentChartSetting()}></LineChart>
                </div>
            </div>
        );
    }
}

