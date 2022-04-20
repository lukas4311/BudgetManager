import React from "react";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { BaseList } from "../BaseList";
import moment from "moment";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { RouteComponentProps } from 'react-router-dom';
import { CurrencyApi, OtherInvestmentApi } from "../../ApiClient/Main";
import CurrencyTickerSelectModel from "../Crypto/CurrencyTickerSelectModel";
import OtherInvestmentViewModel from "../../Model/OtherInvestmentViewModel";
import { Dialog, DialogContent, DialogTitle } from "@material-ui/core";
import { OtherInvestmentForm } from "./OtherInvestmentForm";
import OtherInvestmentDetail from "./OtherInvestmentDetail";
import { OtherInvestmentModel } from "../../ApiClient/Main/models/OtherInvestmentModel";

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
    formKey: number;
    openedForm: boolean;
    showDetail: boolean;
}

export default class OtherInvestmentOverview extends React.Component<RouteComponentProps, OtherInvestmentOverviewState>{
    private otherInvestmentApi: OtherInvestmentApi;
    private currencies: CurrencyTickerSelectModel[];

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { otherInvestments: [], formKey: Date.now(), selectedModel: undefined, openedForm: false, showDetail: false };
    }

    public componentDidMount = () => this.init();

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.otherInvestmentApi = await apiFactory.getClient(OtherInvestmentApi);
        const currencyApi = await apiFactory.getClient(CurrencyApi);
        this.currencies = (await currencyApi.currencyAllGet()).map(c => ({ id: c.id, ticker: c.symbol }));
        await this.loadData();

    }

    private async loadData() {
        const data: OtherInvestmentModel[] = await this.otherInvestmentApi.otherInvestmentAllGet();
        const viewModels: OtherInvestmentViewModel[] = data.map(d => this.mapDataModelToViewModel(d));
        this.setState({ otherInvestments: viewModels });
    }

    private renderTemplate = (p: OtherInvestmentViewModel): JSX.Element => {
        return (
            <>
                <p className="w-1/2 border border-vermilion">{p.name},-</p>
                <p className="w-1/2 border border-vermilion">{p.openingBalance}</p>
            </>
        );
    }

    private renderHeader = (): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/2">Investment name</p>
                <p className="mx-6 my-1 w-1/2">Opening balance</p>
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
        this.setState({ showDetail: true, selectedModel: selectedModel });
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

        console.log(otherInvestmentData);

        if (otherInvestmentData.id)
            await this.otherInvestmentApi.otherInvestmentPut({ otherInvestmentModel: otherInvestment });
        else
            await this.otherInvestmentApi.otherInvestmentPost({ otherInvestmentModel: otherInvestment });

        this.setState({ openedForm: false, selectedModel: undefined });
        this.loadData();
    }

    private handleClose = () => {
        this.setState({ openedForm: false, selectedModel: undefined });
    }

    public render() {
        return (
            <ThemeProvider theme={theme}>
                <h2 className="text-xl p-4 text-center">Other investments</h2>
                <div className="text-center mt-4 bg-prussianBlue rounded-lg">
                    <h2 className="text-2xl"></h2>
                    <div className="flex flex-row">
                        <div className="w-2/5">
                            <div className="m-5 h-64 overflow-y-scroll">
                                <BaseList<OtherInvestmentViewModel> data={this.state.otherInvestments} template={this.renderTemplate} header={this.renderHeader()}
                                    addItemHandler={this.addInvesment} itemClickHandler={this.editInvesment} useRowBorderColor={true}></BaseList>
                            </div>
                        </div>
                        <div className="w-3/5">{this.state.showDetail ? <OtherInvestmentDetail selectedInvestment={this.state.selectedModel} route={this.props} /> : <div />}</div>
                    </div>
                </div>
                <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Investment form"
                    maxWidth="md" fullWidth={true}>
                    <DialogTitle id="form-dialog-title">Investment form</DialogTitle>
                    <DialogContent>
                        <OtherInvestmentForm {...this.state.selectedModel} />
                    </DialogContent>
                </Dialog>
            </ThemeProvider>
        );
    }
}