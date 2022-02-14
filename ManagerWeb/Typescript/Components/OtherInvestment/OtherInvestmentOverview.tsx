import React from "react";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { BaseList } from "../BaseList";
import moment from "moment";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { RouteComponentProps } from 'react-router-dom';
import { CurrencyApi, OtherInvestmentApi, OtherInvestmentModel } from "../../ApiClient/Main";
import CurrencyTickerSelectModel from "../Crypto/CurrencyTickerSelectModel";
import OtherInvestmentViewModel from "../../Model/OtherInvestmentViewModel";
import { Dialog, DialogContent, DialogTitle } from "@material-ui/core";
import { OtherInvestmentForm } from "./OtherInvestmentForm";

const theme = createMuiTheme({
    palette: {
        type: 'dark',
        primary: {
            main: "#e03d15ff",
        }
    }
});

class OtherInvestmentOverviewState {
    otherInvesmnets: OtherInvestmentViewModel[];
    selectedModel: OtherInvestmentViewModel;
    formKey: number;
    openedForm: boolean;
}

export default class OtherInvestmentOverview extends React.Component<RouteComponentProps, OtherInvestmentOverviewState>{
    private otherInvestmentApi: OtherInvestmentApi;
    private currencies: CurrencyTickerSelectModel[];

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { otherInvesmnets: [], formKey: Date.now(), selectedModel: undefined, openedForm: false };
    }

    public componentDidMount = () => this.init();

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.otherInvestmentApi = await apiFactory.getClient(OtherInvestmentApi);
        const currencyApi = await apiFactory.getClient(CurrencyApi);
        this.currencies = (await currencyApi.currencyAllGet()).map(c => ({ id: c.id, ticker: c.symbol }));
        const data: OtherInvestmentModel[] = await this.otherInvestmentApi.otherInvestmentAllGet();
        const viewModels: OtherInvestmentViewModel[] = data.map(d => this.mapDataModelToViewModel(d));
        this.setState({ otherInvesmnets: viewModels });
    }

    private renderTemplate = (p: OtherInvestmentViewModel): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/2">{p.name},-</p>
                <p className="mx-6 my-1 w-1/2">{p.openingBalance}</p>
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
        // show dialog
    }

    private editInvesment = () => {
        // show dialog with data
    }

    private mapDataModelToViewModel = (otherInvestment: OtherInvestmentModel): OtherInvestmentViewModel => {
        let model: OtherInvestmentViewModel = new OtherInvestmentViewModel();
        model.currencySymbol = otherInvestment.currencySymbol;
        model.currencySymbolId = otherInvestment.currencySymbolId;
        model.currencies = this.currencies;
        model.id = otherInvestment.id;
        model.created = moment(otherInvestment.created).format("YYYY-MM-DD");
        model.name = otherInvestment.name;
        model.code = otherInvestment.code;
        model.onSave = this.saveTrade;
        return model;
    }

    private saveTrade = async (otherInvestmentData: OtherInvestmentViewModel): Promise<void> => {
        const otherInvestment: OtherInvestmentModel = {
        };

        if (otherInvestmentData.id)
            await this.otherInvestmentApi.otherInvestmentPut({ otherInvestmentModel: otherInvestment });
        else
            await this.otherInvestmentApi.otherInvestmentPost({ otherInvestmentModel: otherInvestment });

        // this.setState({ openedForm: false, selectedModel: undefined });
        // this.loadGoldData();
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
                    <div className="grid grid-cols-2">
                        <div>
                            <div className="pb-10 h-64 overflow-y-scroll pr-4">
                                <BaseList<OtherInvestmentViewModel> data={this.state.otherInvesmnets} template={this.renderTemplate} header={this.renderHeader()}
                                    addItemHandler={this.addInvesment} itemClickHandler={this.editInvesment}></BaseList>
                            </div>
                        </div>
                        <div>Detail</div>
                        <div className="col-span-2">Overview</div>
                    </div>
                </div>
                <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Detail transakce"
                    maxWidth="md" fullWidth={true}>
                    <DialogTitle id="form-dialog-title">Zlatý slitek</DialogTitle>
                    <DialogContent>
                        <OtherInvestmentForm {...this.state.selectedModel} />
                    </DialogContent>
                </Dialog>
            </ThemeProvider>
        );
    }
}