import _ from "lodash";
import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { CurrencyApi, OtherInvestmentApi, OtherInvestmentBalaceHistoryModel, OtherInvestmentBalanceSummaryModel, OtherInvestmentModel } from "../../ApiClient/Main";
import OtherInvestmentViewModel from "../../Model/OtherInvestmentViewModel";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import CurrencyTickerSelectModel from "../Crypto/CurrencyTickerSelectModel";

class OtherInvestmentSummaryState {
    balanceSum: number;
    investedSum: number;
}

export default class OtherInvestmentSummary extends React.Component<RouteComponentProps, OtherInvestmentSummaryState>{
    private otherInvestmentApi: OtherInvestmentApi;
    private currencies: CurrencyTickerSelectModel[];

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { balanceSum: 0, investedSum: 0 };
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
        console.log(data);
        console.log(actualSummary);
        let investedSum = 0;
        let balanceSum = 0;
        data.forEach(o => {
            const summary = _.first(actualSummary.filter(s => s.otherInvestmentId == o.id));
            const actualBalance = summary?.balance ?? 0;
            const invested = summary?.invested ?? 0;

            investedSum += o.openingBalance + invested;
            balanceSum += actualBalance;
        });

        this.setState({ investedSum: investedSum, balanceSum: balanceSum });
    }

    public render() {
        return (
            <div>
                <h3 className="text-xl p-4 text-center">Other investment summary</h3>
                <p>Balance: {this.state.balanceSum}</p>
                <p>Invested: {this.state.investedSum}</p>
            </div>
        );
    }
}

