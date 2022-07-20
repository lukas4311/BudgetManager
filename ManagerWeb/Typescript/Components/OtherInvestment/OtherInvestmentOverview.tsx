import React from "react";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { BaseList } from "../BaseList";
import moment from "moment";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { RouteComponentProps } from 'react-router-dom';
import { CurrencyApi, OtherInvestmentApi, OtherInvestmentBalaceHistoryModel, OtherInvestmentBalanceSummaryModel } from "../../ApiClient/Main";
import CurrencyTickerSelectModel from "../Crypto/CurrencyTickerSelectModel";
import OtherInvestmentViewModel from "../../Model/OtherInvestmentViewModel";
import { Dialog, DialogContent, DialogTitle } from "@material-ui/core";
import { OtherInvestmentForm } from "./OtherInvestmentForm";
import OtherInvestmentDetail from "./OtherInvestmentDetail";
import { OtherInvestmentModel } from "../../ApiClient/Main/models/OtherInvestmentModel";
import OtherInvestmentSummary from "./OtherInvestmentSummary";
import _ from "lodash";
import { ProgressCalculatorService } from "../../Services/ProgressCalculatorService";
import { MainFrame } from "../MainFrame";

const theme = createMuiTheme({
    palette: {
        type: 'dark',
        primary: {
            main: "#e03d15ff",
        }
    }
});

class OtherInvestmentOverviewState {
    otherInvestments: OtherInvestmentViewModel[];
    selectedModel: OtherInvestmentViewModel;
    actualSummary: Array<OtherInvestmentBalaceHistoryModel>;
    formKey: number;
    openedForm: boolean;
    showDetail: boolean;
}

export default class OtherInvestmentOverview extends React.Component<RouteComponentProps, OtherInvestmentOverviewState>{
    private otherInvestmentApi: OtherInvestmentApi;
    private currencies: CurrencyTickerSelectModel[];
    private progressCalculator: ProgressCalculatorService;

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { otherInvestments: [], formKey: Date.now(), selectedModel: undefined, openedForm: false, showDetail: false, actualSummary: [] };
    }

    public componentDidMount = () => this.init();

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.progressCalculator = new ProgressCalculatorService();
        this.otherInvestmentApi = await apiFactory.getClient(OtherInvestmentApi);
        const currencyApi = await apiFactory.getClient(CurrencyApi);
        this.currencies = (await currencyApi.currencyAllGet()).map(c => ({ id: c.id, ticker: c.symbol }));
        await this.loadData();
    }

    private async loadData() {
        const summary: OtherInvestmentBalanceSummaryModel = await this.otherInvestmentApi.otherInvestmentSummaryGet();
        const actualSummary: Array<OtherInvestmentBalaceHistoryModel> = summary.actualBalanceData;
        const data: OtherInvestmentModel[] = await this.otherInvestmentApi.otherInvestmentAllGet();
        const viewModels: OtherInvestmentViewModel[] = data.map(d => this.mapDataModelToViewModel(d));

        this.setState({ otherInvestments: viewModels, actualSummary });
    }

    private renderTemplate = (p: OtherInvestmentViewModel): JSX.Element => {
        const actualBalanceSummary = _.first(this.state.actualSummary.filter(f => f.otherInvestmentId == p.id));
        const totalInvested = p.openingBalance + actualBalanceSummary?.invested;
        const totalProgress = this.progressCalculator.calculareProgress(totalInvested, actualBalanceSummary.balance)
        const bgColor: string = totalProgress >= 0 ? "bg-green-700" : "bg-red-700";

        return (
            <>
                <p className="w-1/3 h-full border border-vermilion flex items-center justify-center">{p.name}</p>
                <p className="w-1/3 h-full border border-vermilion flex items-center justify-center">{p.openingBalance},-</p>
                <p className="w-1/3 h-full border border-vermilion flex items-center justify-center">{totalInvested},-</p>
                <div className="w-1/3 h-full border border-vermilion flex items-center justify-center rounded-r-full">
                    <div className={bgColor + " my-1 px-1 mx-auto rounded-md flex flex-row content-start items-center"}>
                        <p className="w-1/2 text-white text-right">{actualBalanceSummary.balance} </p>
                        <p className="w-1/2 font-semibold ml-1 text-white text-left">{p.currencySymbol}</p>
                        <p className="w-1/2 font-extralight text-xs ml-1 text-white text-left">{totalProgress.toFixed(2)}%</p>
                    </div>
                </div>
            </>
        );
    }

    private renderHeader = (): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/2">Investment name</p>
                <p className="mx-6 my-1 w-1/2">Opening balance</p>
                <p className="mx-6 my-1 w-1/2">Total invested</p>
                <p className="mx-6 my-1 w-1/2">Actual balance</p>
            </>
        );
    }

    private addInvesment = () => {
        let model: OtherInvestmentViewModel = new OtherInvestmentViewModel();
        model.onSave = this.saveTrade;
        model.created = moment().format("YYYY-MM-DD");
        model.name = "";
        model.code = "";
        model.openingBalance = 0;
        model.currencies = this.currencies;
        model.currencySymbolId = this.currencies[0].id;
        this.setState({ openedForm: true, formKey: Date.now(), selectedModel: model });
    }

    private editInvesment = (id: number) => {
        let selectedModel = this.state.otherInvestments.filter(t => t.id == id)[0];
        this.showDetail(selectedModel);
    }

    private showDetail = (selectedModel: OtherInvestmentViewModel) => {
        this.setState({ showDetail: true, selectedModel: selectedModel, formKey: Date.now() });
    }

    private mapDataModelToViewModel = (otherInvestment: OtherInvestmentModel): OtherInvestmentViewModel => {
        let model: OtherInvestmentViewModel = new OtherInvestmentViewModel();
        model.currencySymbol = this.currencies.find(f => f.id == otherInvestment.currencySymbolId).ticker;
        model.currencySymbolId = otherInvestment.currencySymbolId;
        model.currencies = this.currencies;
        model.id = otherInvestment.id;
        model.created = moment(otherInvestment.created).format("YYYY-MM-DD");
        model.name = otherInvestment.name;
        model.code = otherInvestment.code;
        model.openingBalance = otherInvestment.openingBalance;
        model.onSave = this.saveTrade;

        return model;
    }

    private saveTrade = async (otherInvestmentData: OtherInvestmentViewModel): Promise<void> => {
        const otherInvestment: OtherInvestmentModel = {
            code: otherInvestmentData.code,
            created: new Date(otherInvestmentData.created),
            currencySymbolId: otherInvestmentData.currencySymbolId,
            id: otherInvestmentData.id,
            name: otherInvestmentData.name,
            openingBalance: otherInvestmentData.openingBalance
        };

        if (otherInvestmentData.id)
            await this.otherInvestmentApi.otherInvestmentPut({ otherInvestmentModel: otherInvestment });
        else
            await this.otherInvestmentApi.otherInvestmentPost({ otherInvestmentModel: otherInvestment });

        this.setState({ openedForm: false, selectedModel: undefined });
        this.loadData();
    }

    private handleClose = () => this.setState({ openedForm: false, selectedModel: undefined });

    private refresh = () => {
        this.setState({ openedForm: false, selectedModel: undefined, showDetail: false });
        this.loadData();
    }

    public render() {
        return (
            <ThemeProvider theme={theme}>
                <MainFrame header='Other investments'>
                    <>
                        <h2 className="text-2xl"></h2>
                        <div className="flex flex-row">
                            <div className="w-2/5">
                                <div className="m-5 overflow-y-scroll">
                                    <BaseList<OtherInvestmentViewModel> data={this.state.otherInvestments} template={this.renderTemplate} header={this.renderHeader()}
                                        addItemHandler={this.addInvesment} itemClickHandler={this.editInvesment} useRowBorderColor={true} hideIconRowPart={true}></BaseList>
                                </div>
                            </div>
                            <div className="w-3/5">{this.state.showDetail ? <OtherInvestmentDetail key={this.state.formKey} selectedInvestment={this.state.selectedModel} route={this.props} refreshRecords={this.refresh} /> : <div />}</div>
                        </div>
                        <div>
                            <OtherInvestmentSummary {...this.props}></OtherInvestmentSummary>
                        </div>
                        <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Investment form"
                            maxWidth="md" fullWidth={true}>
                            <DialogTitle id="form-dialog-title">Investment form</DialogTitle>
                            <DialogContent>
                                <OtherInvestmentForm {...this.state.selectedModel} />
                            </DialogContent>
                        </Dialog>
                    </>
                </MainFrame>
            </ThemeProvider>
        );
    }
}