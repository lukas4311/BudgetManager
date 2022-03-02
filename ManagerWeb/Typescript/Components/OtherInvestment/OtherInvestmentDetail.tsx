import moment from "moment";
import React from "react";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { RouteComponentProps } from "react-router-dom";
import { OtherInvestmentApi } from "../../ApiClient/Main/apis/OtherInvestmentApi";
import { OtherInvestmentBalaceHistoryModel } from "../../ApiClient/Main/models/OtherInvestmentBalaceHistoryModel";
import OtherInvestmentViewModel from "../../Model/OtherInvestmentViewModel";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { BaseList, IBaseModel } from "../BaseList";
import { Button, Dialog, DialogContent, DialogTitle } from "@material-ui/core";
import { OtherInvestmentBalanceForm } from "./OtherInvestmentBalanceForm";
import _ from "lodash";
import { IconsData } from "../../Enums/IconsEnum";

const theme = createMuiTheme({
    palette: {
        type: 'dark',
        primary: {
            main: "#e03d15ff",
        }
    }
});

class OtherInvestmentDetailProps {
    selectedInvestment: OtherInvestmentViewModel;
    route: RouteComponentProps;
}

export class OtherInvestmentBalaceHistoryViewModel implements IBaseModel {
    id?: number | null;
    date?: string;
    balance?: number;
    otherInvestmentId?: number;
    onSave: (data: OtherInvestmentBalaceHistoryViewModel) => void;
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
    private icons: IconsData = new IconsData();

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
                <p className="w-1/2 border border-vermilion">{p.date}</p>
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

    private saveBalance = async (otherInvestmentData: OtherInvestmentBalaceHistoryViewModel) => {
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
        let viewModel: OtherInvestmentBalaceHistoryViewModel = {
            onSave: this.saveBalance,
            balance: 0,
            date: moment().format("YYYY-MM-DD"),
            otherInvestmentId: this.props.selectedInvestment.id
        };

        this.setState({ openedForm: true, selectedModel: viewModel });
    }

    private editInvesment = (id: number) => {
        let selectedModel = _.first(this.state.balances.filter(t => t.id == id))
        this.setState({ openedForm: true, selectedModel });
    }

    private handleClose = () => {
        this.setState({ openedForm: false, selectedModel: undefined });
    }

    render = () => {
        return (
            <ThemeProvider theme={theme}>
                <div className="bg-lightGray rounded-xl m-6 p-4">
                    <div className="flex flex-row justify-center">
                        <h2 className="text-vermilion text-3xl font-bold">{this.props.selectedInvestment?.code} detail</h2>
                        <p className="self-end ml-4 mr-2">currently invested</p>
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
                    <div className="grid grid-cols-2 gap-4 mt-6">
                        <BaseList<OtherInvestmentBalaceHistoryViewModel> data={this.state.balances} template={this.renderTemplate} header={this.renderHeader()}
                            addItemHandler={this.addBalance} useRowBorderColor={true} itemClickHandler={this.editInvesment}></BaseList>
                        <div className="flex flex-col p-4">
                            <p>Base list with payments with specific tags</p>
                            <Button className='bg-vermilion w-full' onClick={e => console.log("add tag")}>
                                <span className="w-6">{this.icons.link}</span>
                            </Button>
                        </div>
                    </div>
                </div>
                <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Balance at date"
                    maxWidth="md" fullWidth={true}>
                    <DialogTitle id="form-dialog-title">Balance form</DialogTitle>
                    <DialogContent>
                        <OtherInvestmentBalanceForm {...this.state.selectedModel} />
                    </DialogContent>
                </Dialog>
            </ThemeProvider>
        );
    }
}