import _, { forEach } from "lodash";
import moment from "moment";
import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { CurrencyApi, OtherInvestmentApi, OtherInvestmentBalaceHistoryModel, OtherInvestmentBalanceSummaryModel, OtherInvestmentModel, PaymentModel } from "../../ApiClient/Main";
import { LineChartData } from "../../Model/LineChartData";
import { LineChartDataSets } from "../../Model/LineChartDataSets";
import { ProgressCalculatorService } from "../../Services/ProgressCalculatorService";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { ComponentPanel } from "../../Utils/ComponentPanel";
import Dictionary from "../../Utils/Dictionary";
import { Investments, Ranking } from "../../Utils/Ranking";
import { LineChart } from "../Charts/LineChart";
import { LineChartSettingManager } from "../Charts/LineChartSettingManager";
import { PieChart, PieChartData } from "../Charts/PieChart";
import CurrencyTickerSelectModel from "../Crypto/CurrencyTickerSelectModel";

class OtherInvestmentSummaryState {
    balanceSum: number;
    investedSum: number;
    chartData: LineChartDataSets[];
    rankingData: Investments[];
    pieData: PieChartData[]
}

export default class OtherInvestmentSummary extends React.Component<RouteComponentProps, OtherInvestmentSummaryState>{
    private otherInvestmentApi: OtherInvestmentApi;
    private currencies: CurrencyTickerSelectModel[];
    private progressCalculator: ProgressCalculatorService;

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { balanceSum: 0, investedSum: 0, chartData: [], rankingData: [], pieData: [] };
    }

    componentDidMount(): void {
        this.initData();
    }

    private initData = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.otherInvestmentApi = await apiFactory.getClient(OtherInvestmentApi);
        const currencyApi = await apiFactory.getClient(CurrencyApi);
        this.currencies = (await currencyApi.currencyAllGet()).map(c => ({ id: c.id, ticker: c.symbol }));
        this.progressCalculator = new ProgressCalculatorService();
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
            }
        }

        balanceSum = _.last(sortedBalance)?.balance ?? 0;
        investedSum = _.last(sortedInvested)?.amount ?? 0;
        investedChartData = sortedInvested.map(b => ({ x: moment(b.date).format('YYYY-MM-DD hh:ss'), y: b.amount }));
        balanceChartData = sortedBalance.map(b => ({ x: moment(b.date).format('YYYY-MM-DD hh:ss'), y: b.balance }));

        let chartData = [{ id: 'Invested', data: investedChartData }, { id: 'Balance', data: balanceChartData }];

        let rankingData: Investments[] = [];
        let pieData: PieChartData[] = [];
        summary.actualBalanceData.forEach(a => {
            let totalInvested = a?.invested;
            const investmentData = _.first(data.filter(o => o.id == a.otherInvestmentId));

            if (investmentData != null && investmentData != undefined)
                totalInvested += investmentData.openingBalance ?? 0;

            const progress = this.progressCalculator.calculareProgress(totalInvested, a.balance);
            rankingData.push({ name: investmentData.name, investmentProgress: progress });
            pieData.push({ id: investmentData.code, label: "", value: a?.balance });
        });
        rankingData = _.take(_.orderBy(rankingData, o => o.investmentProgress, 'desc'), 3);
        this.setState({ investedSum: investedSum, balanceSum: balanceSum, chartData, rankingData, pieData });
    }

    private getMinLineChartData = () => {
        let minVal: number = Number.MAX_VALUE;
        let maxVal: number = Number.MAX_VALUE;

        this.state.chartData.forEach(e => {
            minVal = Math.min(_.minBy(e.data, m => m.y).y, minVal);
            maxVal = Math.min(_.maxBy(e.data, m => m.y).y, maxVal);
        })

        return { min: minVal, max: maxVal };
    }

    public render() {
        const bounds = this.getMinLineChartData();
        const profit = (this.state.balanceSum - this.state.investedSum) ?? 0;
        let profitPct = 0;

        if (this.state.balanceSum != 0 && profit != 0)
            profitPct = (profit / this.state.investedSum) * 100;

        const profitColor = profit < 0 ? "text-red-800" : "text-green-800";

        return (
            <div>
                <h3 className="text-2xl p-4 text-center">Other investment summary</h3>
                <ComponentPanel classStyle="mx-auto w-1/3">
                    <React.Fragment>
                        <div className="flex flex-row justify-around">
                            <p className="text-2xl text-white font-black">Balance: {this.state.balanceSum}</p>
                            <p className="text-2xl text-white font-black">Invested: {this.state.investedSum}</p>
                        </div>
                        <div className="mt-4">
                            <p className={"text-2xl font-black " + profitColor}>Profit: {profit} ({profitPct.toFixed(1)})%</p>
                        </div>
                    </React.Fragment>
                </ComponentPanel>

                <div className="flex flex-row">
                    <ComponentPanel classStyle="w-1/3">
                        <React.Fragment>
                            <h4 className="text-left text-2xl text-white font-black">Investment balance progress</h4>
                            <div className="h-80">
                                <LineChart dataSets={this.state.chartData} chartProps={LineChartSettingManager.getOtherInvestmentSummarySetting(bounds.min, bounds.max)}></LineChart>
                            </div>
                        </React.Fragment>
                    </ComponentPanel>
                    <ComponentPanel classStyle="w-1/3">
                        <React.Fragment>
                            <h4 className="text-left text-2xl text-white font-black">Top 3</h4>
                            <div className="h-80">
                                <Ranking data={this.state.rankingData}></Ranking>
                            </div>
                        </React.Fragment>
                    </ComponentPanel>
                    <ComponentPanel classStyle="w-1/3">
                        <React.Fragment>
                            <h4 className="text-left text-2xl text-white font-black">Investments diversification</h4>
                            <div className="h-80">
                                <PieChart data={this.state.pieData}></PieChart>
                            </div>
                        </React.Fragment>
                    </ComponentPanel>
                </div>
            </div>
        );
    }
}