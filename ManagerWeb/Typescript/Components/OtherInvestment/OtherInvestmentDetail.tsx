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
import { PaymentModel, TagApi, TagModel } from "../../ApiClient/Main";
import { OtherInvestmentTagForm } from "./OtherInvestmentTagForm";
import { TagFormViewModel } from "../../Model/TagFormViewModel";
import { LineChart } from "../Charts/LineChart";
import { LineChartData } from "../../Model/LineChartData";
import { LineChartDataSets } from "../../Model/LineChartDataSets";
import { LineSvgProps } from "@nivo/line";
import { LineChartSettingManager } from "../Charts/LineChartSettingManager";

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
    openedFormBalance: boolean;
    selectedModel: OtherInvestmentBalaceHistoryViewModel;
    openedFormTags: boolean;
    tagViewModel: TagFormViewModel;
    linkedTagCode: string;
    linkedPayments: PaymentModel[];
    totalInvested: number;
}

export default class OtherInvestmentDetail extends React.Component<OtherInvestmentDetailProps, OtherInvestmentDetailState>{
    private otherInvestmentApi: OtherInvestmentApi;
    private icons: IconsData = new IconsData();
    private tagApi: TagApi;
    private tags: TagModel[];

    constructor(props: OtherInvestmentDetailProps) {
        super(props);
        this.state = {
            balances: [], progressOverall: 0, progressYY: 0, openedFormBalance: false, selectedModel: undefined,
            openedFormTags: false, tagViewModel: undefined, linkedTagCode: "", linkedPayments: [], totalInvested: 0
        };
    }

    public componentDidMount = () => this.init();

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.route.history);
        this.otherInvestmentApi = await apiFactory.getClient(OtherInvestmentApi);
        this.tagApi = await apiFactory.getClient(TagApi);
        await this.loadData();
    }

    private async loadData() {
        const otherinvestmentid = this.props.selectedInvestment.id;
        const data: OtherInvestmentBalaceHistoryModel[] = await this.otherInvestmentApi.otherInvestmentOtherInvestmentIdBalanceHistoryGet({ otherInvestmentId: otherinvestmentid });
        this.tags = await this.tagApi.tagsAllUsedGet();
        const linkedTag = await this.otherInvestmentApi.otherInvestmentIdLinkedTagGet({ id: otherinvestmentid });
        let linkedTagCode = "";
        let linkedPayments: PaymentModel[] = [];
        let totalInvested: number;

        if (linkedTag != undefined) {
            linkedTagCode = _.first(_.filter(this.tags, t => t.id == linkedTag.tagId))?.code ?? "";
            linkedPayments = await this.otherInvestmentApi.otherInvestmentIdTagedPaymentsTagIdGet({ id: otherinvestmentid, tagId: linkedTag.tagId });
            totalInvested = _.sumBy(linkedPayments, p => p.amount) + this.props.selectedInvestment.openingBalance;
        }

        const viewModels: OtherInvestmentBalaceHistoryViewModel[] = data.map(d => this.mapDataModelToViewModel(d));
        const progressYY = await this.otherInvestmentApi.otherInvestmentIdProfitOverYearsYearsGet({ id: otherinvestmentid, years: 1 });
        const progressOverall = await this.otherInvestmentApi.otherInvestmentIdProfitOverallGet({ id: otherinvestmentid });
        this.setState({ balances: viewModels, progressOverall, progressYY, linkedTagCode, linkedPayments, totalInvested });
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

    private getActualBalance = (): string => {
        let balances = this.state.balances;

        if (balances?.length != 0) {
            const sortedArray = _.orderBy(balances, [(obj) => new Date(obj.date)], ['desc'])
            return sortedArray[0].balance.toFixed(0);
        }

        return "Any balance";
    }

    private getChartData = (): LineChartDataSets[] => {
        let balanceChartData: LineChartDataSets[] = [{ id: 'Balance', data: [] }];
        let balances = this.state.balances;

        if (balances?.length != 0) {
            let balanceData: LineChartData[] = balances.map(b => ({ x: moment(b.date).format('YYYY-MM-DD'), y: b.balance }))
            balanceChartData = [{ id: 'Balance', data: balanceData }];
        }

        return balanceChartData;
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

        this.setState({ openedFormBalance: false, selectedModel: undefined });
        this.loadData();
    }

    private addBalance = () => {
        let viewModel: OtherInvestmentBalaceHistoryViewModel = {
            onSave: this.saveBalance,
            balance: 0,
            date: moment().format("YYYY-MM-DD"),
            otherInvestmentId: this.props.selectedInvestment.id
        };

        this.setState({ openedFormBalance: true, selectedModel: viewModel });
    }

    private editInvesment = (id: number) => {
        let selectedModel = _.first(this.state.balances.filter(t => t.id == id))
        this.setState({ openedFormBalance: true, selectedModel });
    }

    private onCreateConnectionWithPaymentTag = () => {
        let firstTag = _.first(this.tags);

        if (firstTag != undefined) {
            let tagModel: TagFormViewModel = {
                onSave: this.createConnectionWithPaymentTag,
                tagId: firstTag.id,
                tags: this.tags
            };

            this.setState({ openedFormTags: true, tagViewModel: tagModel });
        }
    }

    private createConnectionWithPaymentTag = async (tagId: number) => {
        if (tagId != undefined && tagId != 0)
            this.otherInvestmentApi.otherInvestmentIdTagedPaymentsTagIdPost({ tagId, id: this.props.selectedInvestment.id });

        this.setState({ openedFormTags: false, tagViewModel: undefined });
        await this.loadData();
    }

    private handleCloseBalance = () => {
        this.setState({ openedFormBalance: false, selectedModel: undefined });
    }

    private handleCloseTag = () => {
        this.setState({ openedFormTags: false, tagViewModel: undefined });
    }

    private renderPaymentTemplate = (p: PaymentModel): JSX.Element => {
        let iconsData: IconsData = new IconsData();

        return (
            <>
                <p className="mx-6 my-1 w-2/12">{p.amount},-</p>
                <p className="mx-6 my-1 w-2/12">{p.name}</p>
                <p className="mx-6 my-1 w-3/12">{moment(p.date).format('DD.MM.YYYY')}</p>
            </>
        );
    }

    render = () => {
        return (
            <ThemeProvider theme={theme}>
                <div className="bg-lightGray rounded-xl m-6 p-4">
                    <div className="flex flex-row justify-center">
                        <h2 className="text-vermilion text-3xl font-bold">{this.props.selectedInvestment?.code} detail</h2>
                        <p className="self-end ml-4 mr-2">currently invested</p>
                        <h2 className="text-vermilion text-2xl font-bold self-center">{this.state.totalInvested}</h2>
                    </div>
                    <div className="flex flex-col pt-4">
                        <div className="h-64 w-full">
                            <LineChart dataSets={this.getChartData()} chartProps={LineChartSettingManager.getOtherInvestmentChartSetting()}></LineChart>
                        </div>
                        <div className="flex flex-row justify-around">
                            <p className="text-xl">Curent value {this.getActualBalance()}</p>
                            <p>Overall progress {_.round(this.state.progressOverall, 2)}%</p>
                            <p>Y/Y progress {_.round(this.state.progressYY, 2)}%</p>
                        </div>
                    </div>
                    <div className="grid grid-cols-2 gap-4 mt-6">
                        <BaseList<OtherInvestmentBalaceHistoryViewModel> data={this.state.balances} template={this.renderTemplate} header={this.renderHeader()}
                            addItemHandler={this.addBalance} useRowBorderColor={true} itemClickHandler={this.editInvesment}></BaseList>
                        <div className="flex flex-col p-4">
                            <p className="text-xl mb-2 text-left">Payments to investment</p>
                            <BaseList<PaymentModel> data={this.state.linkedPayments} template={this.renderPaymentTemplate}></BaseList>
                            <Button className='bg-vermilion w-full' onClick={this.onCreateConnectionWithPaymentTag}>
                                <span className="w-6">{this.icons.link}</span>
                                <span className="ml-6 text-xs font-semibold">{this.state.linkedTagCode}</span>
                            </Button>
                        </div>
                    </div>
                </div>
                <Dialog open={this.state.openedFormBalance} onClose={this.handleCloseBalance} aria-labelledby="Balance at date"
                    maxWidth="md" fullWidth={true}>
                    <DialogTitle id="form-dialog-title">Balance form</DialogTitle>
                    <DialogContent>
                        <OtherInvestmentBalanceForm {...this.state.selectedModel} />
                    </DialogContent>
                </Dialog>
                <Dialog open={this.state.openedFormTags} onClose={this.handleCloseTag} aria-labelledby="Balance at date"
                    maxWidth="md" fullWidth={true}>
                    <DialogTitle id="form-dialog-title">Tag form</DialogTitle>
                    <DialogContent>
                        <OtherInvestmentTagForm {...this.state.tagViewModel} />
                    </DialogContent>
                </Dialog>
            </ThemeProvider>
        );
    }
}