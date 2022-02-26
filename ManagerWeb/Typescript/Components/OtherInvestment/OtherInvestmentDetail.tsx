import moment from "moment";
import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { OtherInvestmentApi } from "../../ApiClient/Main/apis/OtherInvestmentApi";
import { OtherInvestmentBalaceHistoryModel } from "../../ApiClient/Main/models/OtherInvestmentBalaceHistoryModel";
import OtherInvestmentViewModel from "../../Model/OtherInvestmentViewModel";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { BaseList, IBaseModel } from "../BaseList";

class OtherInvestmentDetailProps {
    selectedInvestment: OtherInvestmentViewModel;
    route: RouteComponentProps;
}

class OtherInvestmentBalaceHistoryViewModel implements IBaseModel {
    id?: number | null;
    date?: string;
    balance?: number;
    otherInvestmentId?: number;
    onSave: (data: OtherInvestmentViewModel) => void;
}

class OtherInvestmentDetailState {
    balances: OtherInvestmentBalaceHistoryViewModel[];
    progressYY: number;
    progressOverall: number;
    openedForm: boolean;
    selectedModel: OtherInvestmentBalaceHistoryViewModel;
}

export default class OtherInvestmentDetail extends React.Component<OtherInvestmentDetailProps, OtherInvestmentDetailState>{
    private otherInvestmentApi: OtherInvestmentApi;

    constructor(props: OtherInvestmentDetailProps) {
        super(props);
        this.state = { balances: [], progressOverall: 0, progressYY: 0, openedForm: false, selectedModel: undefined };
    }

    public componentDidMount = () => this.init();

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.route.history);
        this.otherInvestmentApi = await apiFactory.getClient(OtherInvestmentApi);
        await this.loadData();
    }

    private async loadData() {
        const data: OtherInvestmentBalaceHistoryModel[] = await this.otherInvestmentApi.otherInvestmentOtherInvestmentIdBalanceHistoryGet({ otherInvestmentId: this.props.selectedInvestment.id });
        const viewModels: OtherInvestmentBalaceHistoryViewModel[] = data.map(d => this.mapDataModelToViewModel(d));
        const progressYY = await this.otherInvestmentApi.otherInvestmentIdProfitOverYearsYearsGet({ id: this.props.selectedInvestment.id, years: 1 });
        const progressOverall = await this.otherInvestmentApi.otherInvestmentIdProfitOverallGet({ id: this.props.selectedInvestment.id });
        this.setState({ balances: viewModels, progressOverall, progressYY });
    }

    private renderTemplate = (p: OtherInvestmentBalaceHistoryViewModel): JSX.Element => {
        return (
            <>
                <p className="w-1/2 border border-vermilion">{p.date},-</p>
                <p className="w-1/2 border border-vermilion">{p.balance}</p>
            </>
        );
    }

    private renderHeader = (): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/2">Check date</p>
                <p className="mx-6 my-1 w-1/2">Balance</p>
            </>
        );
    }

    private mapDataModelToViewModel = (otherInvestmentBalance: OtherInvestmentBalaceHistoryModel): OtherInvestmentBalaceHistoryViewModel => {
        let model: OtherInvestmentBalaceHistoryViewModel = new OtherInvestmentBalaceHistoryViewModel();
        model.id = otherInvestmentBalance.id;
        model.date = moment(otherInvestmentBalance.date).format("YYYY-MM-DD");
        model.balance = otherInvestmentBalance.balance;
        model.otherInvestmentId = otherInvestmentBalance.otherInvestmentId;
        model.onSave = this.saveBalance;
        return model;
    }

    private saveBalance = () => async (otherInvestmentData: OtherInvestmentBalaceHistoryViewModel): Promise<void> => {
        const otherInvestmentBalance: OtherInvestmentBalaceHistoryModel = {
            id: otherInvestmentData.id,
            balance: otherInvestmentData.balance,
            date: new Date(otherInvestmentData.date),
            otherInvestmentId: otherInvestmentData.otherInvestmentId
        };

        console.log(otherInvestmentData);

        if (otherInvestmentData.id)
            await this.otherInvestmentApi.balanceHistoryPut({ otherInvestmentBalaceHistoryModel: otherInvestmentBalance });
        else
            await this.otherInvestmentApi.otherInvestmentOtherInvestmentIdBalanceHistoryPost({ otherInvestmentId: otherInvestmentBalance.otherInvestmentId, otherInvestmentBalaceHistoryModel: otherInvestmentBalance });

        this.setState({ openedForm: false, selectedModel: undefined });
        this.loadData();
    }

    private addBalance = () => {

    }

    render = () => {
        return (
            <div className="bg-lightGray rounded-xl m-6 p-4">
                <div className="flex flex-row justify-center">
                    <h2 className="text-vermilion text-3xl font-bold">{this.props.selectedInvestment?.code} detail</h2>
                    <p className="self-end ml-4 mr-2">Initial invest</p>
                    <h2 className="text-vermilion text-2xl font-bold self-center">{this.props.selectedInvestment?.openingBalance}</h2>
                </div>
                <div className="grid grid-cols-2 gap-4">
                    <div>
                        <p>Curent value</p>
                        <p>Overall progress {this.state.progressOverall}</p>
                        <p>Y/Y progress {this.state.progressYY}</p>
                    </div>
                    <div>GRAF</div>
                </div>
                <div className="grid grid-cols-2 gap-4">
                    <BaseList<OtherInvestmentBalaceHistoryViewModel> data={this.state.balances} template={this.renderTemplate} header={this.renderHeader()}
                        addItemHandler={this.addBalance}></BaseList>
                </div>
            </div>
        );
    }
}