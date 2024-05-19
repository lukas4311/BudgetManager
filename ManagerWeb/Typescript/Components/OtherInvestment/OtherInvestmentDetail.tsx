import moment from "moment";
import React from "react";
import { createMuiTheme, ThemeProvider } from "@mui/material/styles";
import { RouteComponentProps } from "react-router-dom";
import { OtherInvestmentApi } from "../../ApiClient/Main/apis/OtherInvestmentApi";
import { OtherInvestmentBalaceHistoryModel } from "../../ApiClient/Main/models/OtherInvestmentBalaceHistoryModel";
import OtherInvestmentViewModel from "../../Model/OtherInvestmentViewModel";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { BaseList, IBaseModel } from "../BaseList";
import { Button, Dialog, DialogContent, DialogTitle } from "@mui/material";
import { OtherInvestmentBalanceForm } from "./OtherInvestmentBalanceForm";
import _ from "lodash";
import { IconsData } from "../../Enums/IconsEnum";
import { PaymentModel, TagApi, TagModel } from "../../ApiClient/Main";
import { OtherInvestmentTagForm } from "./OtherInvestmentTagForm";
import { TagFormViewModel } from "../../Model/TagFormViewModel";
import { LineChart } from "../Charts/LineChart";
import { LineChartData } from "../../Model/LineChartData";
import { LineChartDataSets } from "../../Model/LineChartDataSets";
import { LineChartSettingManager } from "../Charts/LineChartSettingManager";
import { ConfirmationForm, ConfirmationResult } from "../ConfirmationForm";
import OtherInvestmentService from "../../Services/OtherInvestmentService";
import TagService from "../../Services/TagService";
import { SpinnerCircularSplit } from "spinners-react";

class OtherInvestmentDetailProps {
    selectedInvestment: OtherInvestmentViewModel;
    route: RouteComponentProps;
    refreshRecords: () => void;
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
    confirmDialogKey: number;
    confirmDialogIsOpen: boolean;
}

export default class OtherInvestmentDetail extends React.Component<OtherInvestmentDetailProps, OtherInvestmentDetailState> {
    private otherInvesmentService: OtherInvestmentService;
    private tagSevice: TagService;
    private icons: IconsData = new IconsData();
    private tags: TagModel[];

    constructor(props: OtherInvestmentDetailProps) {
        super(props);
        this.state = {
            balances: [], progressOverall: 0, progressYY: 0, openedFormBalance: false, selectedModel: undefined, confirmDialogIsOpen: false,
            openedFormTags: false, tagViewModel: undefined, linkedTagCode: "", linkedPayments: [], totalInvested: 0, confirmDialogKey: Date.now()
        };
    }

    public componentDidMount = () => this.init();

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.route.history);
        const otherInvestmentApi = await apiFactory.getClient(OtherInvestmentApi);
        this.otherInvesmentService = new OtherInvestmentService(otherInvestmentApi);
        const tagApi = await apiFactory.getClient(TagApi);
        this.tagSevice = new TagService(tagApi);
        await this.loadData();
    }

    private async loadData() {
        const otherinvestmentid = this.props.selectedInvestment.id;
        let viewModels = await this.otherInvesmentService.getBalanceHistory(otherinvestmentid);
        viewModels = _.orderBy(viewModels, [(obj) => new Date(obj.date)], ['asc']);

        this.tags = await this.tagSevice.getAllUsedTags();
        const linkedTag = await this.otherInvesmentService.getTagConnectedWithInvetment(otherinvestmentid);
        let linkedTagCode = "";
        let linkedPayments: PaymentModel[] = [];
        let totalInvested: number;

        if (linkedTag != undefined) {
            linkedTagCode = _.first(_.filter(this.tags, t => t.id == linkedTag.tagId))?.code ?? "";
            linkedPayments = await this.otherInvesmentService.getPaymentLinkedToTagOfOtherInvestment(otherinvestmentid, linkedTag.tagId);
            totalInvested = _.sumBy(linkedPayments, p => p.amount) + this.props.selectedInvestment.openingBalance;
        }

        const progressYY = await this.otherInvesmentService.getYearToYearProfit(otherinvestmentid, 1);
        const progressOverall = await this.otherInvesmentService.getOverallProfit(otherinvestmentid);
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
            const sortedArray = _.orderBy(balances, [(obj) => new Date(obj.date)], ['asc'])
            let balanceData: LineChartData[] = sortedArray.map(b => ({ x: moment(b.date).format('YYYY-MM-DD'), y: b.balance }))
            balanceChartData = [{ id: 'Balance', data: balanceData }];
        }

        return balanceChartData;
    }

    private saveBalance = async (otherInvestmentData: OtherInvestmentBalaceHistoryViewModel) => {
        const otherInvestmentBalance: OtherInvestmentBalaceHistoryModel = {
            id: otherInvestmentData.id,
            balance: otherInvestmentData.balance,
            date: new Date(otherInvestmentData.date),
            otherInvestmentId: otherInvestmentData.otherInvestmentId
        };

        if (otherInvestmentData.id)
            await this.otherInvesmentService.updateOtherInvestmentBalanceHistory(otherInvestmentBalance);
        else
            await this.otherInvesmentService.createOtherInvestmentBalanceHistory(otherInvestmentBalance.otherInvestmentId, otherInvestmentBalance);

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

    private editInvestment = (id: number) => {
        let selectedModel = _.first(this.state.balances.filter(t => t.id == id))
        this.setState({ openedFormBalance: true, selectedModel });
    }

    private onCreateConnectionWithPaymentTag = () => {
        let firstTag = _.first(this.tags);

        if (firstTag != undefined) {
            let tagModel: TagFormViewModel = {
                tagId: firstTag.id,
                tags: this.tags
            };

            this.setState({ openedFormTags: true, tagViewModel: tagModel });
        }
    }

    private createConnectionWithPaymentTag = async (tagId: number) => {
        if (tagId != undefined && tagId != 0)
            this.otherInvesmentService.createConnectionWithPaymentTag(tagId, this.props.selectedInvestment.id);

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
        return (
            <>
                <p className="mx-6 my-1 w-2/12">{p.amount},-</p>
                <p className="mx-6 my-1 w-2/12">{p.name}</p>
                <p className="mx-6 my-1 w-3/12">{moment(p.date).format('DD.MM.YYYY')}</p>
            </>
        );
    }

    private deleteOtherInvestment = async (res: ConfirmationResult) => {
        if (res == ConfirmationResult.Ok)
            await this.otherInvesmentService.deleteOtherInvestment(this.props.selectedInvestment.id);

        this.setState({ confirmDialogIsOpen: false });
        this.props.refreshRecords();
    }

    private showDialog = () => {
        this.setState({ confirmDialogIsOpen: true, confirmDialogKey: Date.now() })
    }

    render = () => {
        return (
            <React.Fragment>
                {this.props ? (
                    <>
                        <div className="bg-lightGray rounded-xl m-6 p-4">
                            <div className="w-8 binWithAnimation" onClick={this.showDialog}>{new IconsData().bin}</div>
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
                                    <p className="text-xl">Curent value <span className="text-vermilion text-2xl font-medium">{this.getActualBalance()}</span></p>
                                    <p>Overall progress <span className={"font-medium " + (this.state.progressOverall < 0 ? "text-red-700 " : "text-green-700")}>{_.round(this.state.progressOverall, 2)}</span>%</p>
                                    <p>YOY progress <span className={"font-medium " + (this.state.progressYY < 0 ? "text-red-700" : "text-green-700")}>{_.round(this.state.progressYY, 2)}</span>%</p>
                                </div>
                            </div>
                            <div className="grid grid-cols-2 gap-4 mt-6">
                                <BaseList<OtherInvestmentBalaceHistoryViewModel> data={this.state.balances} template={this.renderTemplate} header={this.renderHeader()}
                                    addItemHandler={this.addBalance} useRowBorderColor={true} itemClickHandler={this.editInvestment}></BaseList>
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
                            <DialogContent className="bg-prussianBlue">
                                {this.state.selectedModel ? <OtherInvestmentBalanceForm viewModel={this.state.selectedModel} onSave={this.saveBalance} /> : <></>}
                            </DialogContent>
                        </Dialog>
                        <Dialog open={this.state.openedFormTags && (this.state.tagViewModel ? true : false)} onClose={this.handleCloseTag} aria-labelledby="Balance at date"
                            maxWidth="md" fullWidth={true}>
                            <DialogTitle id="form-dialog-title" className="bg-prussianBlue">Tag form</DialogTitle>
                            <DialogContent className="bg-prussianBlue">
                                <OtherInvestmentTagForm viewModel={this.state.tagViewModel} onSave={this.createConnectionWithPaymentTag} />
                            </DialogContent>
                        </Dialog>
                        <ConfirmationForm key={this.state.confirmDialogKey} onClose={() => this.deleteOtherInvestment(ConfirmationResult.Cancel)} onConfirm={this.deleteOtherInvestment} isOpen={this.state.confirmDialogIsOpen} />
                    </>
                )
                    : (
                        <div className="flex text-center justify-center h-full">
                            {/* <div id="loading"></div> */}
                            <SpinnerCircularSplit size={150} thickness={110} speed={70} color="rgba(27, 39, 55, 1)" secondaryColor="rgba(224, 61, 21, 1)" />
                        </div>
                    )}
            </React.Fragment>
        );
    }
}