import _, { forEach } from "lodash";
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
        const sortedInvested = _.orderBy(allPayments, [(obj) => new Date(obj.date)], ['asc']);
        let prevInvested = 0;
        balanceSum = _.last(sortedBalance)?.balance ?? 0;
        investedSum = _.last(sortedInvested)?.amount ?? 0;

        for (const s of sortedInvested) {
            s.amount += prevInvested;
            prevInvested = s.amount;
        }

        if (sortedBalance.length != 0) {
            let lastBalaceOfType: Dictionary<number> = {};
            lastBalaceOfType[sortedBalance[0].otherInvestmentId] = sortedBalance[0].balance;

            for (let i = 1; i < sortedBalance.length; i++) {
                let balanceItem = sortedBalance[i];
                const lastBalance = lastBalaceOfType[balanceItem.otherInvestmentId] ?? 0;
                lastBalaceOfType[balanceItem.otherInvestmentId] = balanceItem.balance;
                const prevBalance = sortedBalance[i - 1];
                balanceItem.balance = prevBalance.balance + (balanceItem.balance - lastBalance);
                console.log(balanceItem);
            }
        }

        investedChartData = sortedInvested.map(b => ({ x: moment(b.date).format('YYYY-MM-DD hh:ss'), y: b.amount }));
        balanceChartData = sortedBalance.map(b => ({ x: moment(b.date).format('YYYY-MM-DD hh:ss'), y: b.balance }));

        let chartData = [{ id: 'Invested', data: investedChartData }, { id: 'Balance', data: balanceChartData }];
        this.setState({ investedSum: investedSum, balanceSum: balanceSum, chartData });
    }

    private getMinLineChartData = () => {
        let minVal: number = Number.MAX_VALUE;
        let maxVal: number = Number.MAX_VALUE;

        const data = this.state.chartData.forEach(e => {
            minVal = Math.min(_.minBy(e.data, m => m.y).y, minVal);
            maxVal = Math.min(_.maxBy(e.data, m => m.y).y, maxVal);
        })

        return { min: minVal, max: maxVal };
    }

    public render() {
        const bounds = this.getMinLineChartData();

        return (
            <div>
                <h3 className="text-xl p-4 text-center">Other investment summary</h3>
                <p>Balance: {this.state.balanceSum}</p>
                <p>Invested: {this.state.investedSum}</p>

                <div className="flex flex-row">
                    <div className="w-1/2 h-64">
                        <LineChart dataSets={this.state.chartData} chartProps={LineChartSettingManager.getOtherInvestmentSummarySetting(bounds.min, bounds.max)}></LineChart>
                    </div>
                </div>
            </div>
        );
    }
}

interface Dictionary<T> {
    [key: string]: T;
}


