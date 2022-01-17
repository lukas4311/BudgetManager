import _ from "lodash";
import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { ComodityApi, CurrencyApi } from "../../ApiClient/Main/apis";
import { ComodityTradeHistoryModel, ComodityTypeModel, CurrencySymbol } from "../../ApiClient/Main/models";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import Gold from "./Gold";
import { GoldIngot } from "./GoldIngot";
import { Dialog, DialogContent, DialogTitle } from "@material-ui/core";
import { ComoditiesForm, ComoditiesFormViewModel } from "./ComoditiesForm";
import moment from "moment";
import CurrencyTickerSelectModel from "../Crypto/CurrencyTickerSelectModel";

const theme = createMuiTheme({
    palette: {
        type: 'dark',
    }
});

class ComoditiesState {
    goldIngots: ComoditiesFormViewModel[];
    openedForm: boolean;
    dialogTitle: string;
    selectedModel: ComoditiesFormViewModel;
    formKey: number;
    comodityMenu: Array<ComodityMenuItem>
}

class ComodityMenuItem {
    title: string;
    id: number;
    selected: boolean;
}

export default class Comodities extends React.Component<RouteComponentProps, ComoditiesState>{
    private comodityApi: ComodityApi;
    private goldCode: string = 'AU';
    private goldType: ComodityTypeModel;
    private currencies: CurrencyTickerSelectModel[];

    constructor(props: RouteComponentProps) {
        super(props);
        const menu: Array<ComodityMenuItem> = [
            { id: 1, title: "Gold", selected: true },
            { id: 2, title: "Silver", selected: false },
            { id: 3, title: "Others", selected: false }
        ];
        this.state = { goldIngots: [], openedForm: false, dialogTitle: "", selectedModel: undefined, formKey: Date.now(), comodityMenu: menu };
    }

    componentDidMount(): void {
        this.init();
    }

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.comodityApi = await apiFactory.getClient(ComodityApi);
        this.loadData();
    }

    private loadData = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        const currencyApi = await apiFactory.getClient(CurrencyApi);
        const comodityTypes = await this.comodityApi.comoditiesComodityTypeAllGet();
        this.goldType = comodityTypes.filter(c => c.code == this.goldCode)[0];
        this.currencies = (await currencyApi.currencyAllGet()).map(c => ({ id: c.id, ticker: c.symbol }));
        await this.loadGoldData();
    }

    private loadGoldData = async () => {
        let data: ComodityTradeHistoryModel[] = await this.comodityApi.comoditiesAllGet();
        const goldIngots = data.filter(a => a.comodityTypeId == this.goldType.id).map(g => this.mapDataModelToViewModel(g));
        this.setState({ goldIngots: goldIngots });
    }

    private addNewGold = () => {
        let model: ComoditiesFormViewModel = new ComoditiesFormViewModel();
        model.onSave = this.saveTrade;
        model.onDelete = this.deleteTrade;
        model.buyTimeStamp = moment().format("YYYY-MM-DD");
        model.comodityTypeName = "Gold";
        model.comodityUnit = this.goldType.comodityUnit;
        model.price = 0;
        model.company = "";
        model.comodityAmount = 0;
        model.currencies = this.currencies;
        model.currencySymbolId = this.currencies[0].id;
        this.setState({ openedForm: true, formKey: Date.now(), selectedModel: model });
    }


    private editGold = (id: number) => {
        let tradeHistory = this.state.goldIngots.filter(t => t.id == id)[0];
        this.setState({ selectedModel: tradeHistory, openedForm: true });
        this.setState({ openedForm: true, dialogTitle: "Gold" });
    }

    private mapDataModelToViewModel = (tradeHistory: ComodityTradeHistoryModel): ComoditiesFormViewModel => {
        let model: ComoditiesFormViewModel = new ComoditiesFormViewModel();
        model.currencySymbol = tradeHistory.currencySymbol;
        model.currencySymbolId = tradeHistory.currencySymbolId;
        model.id = tradeHistory.id;
        model.price = tradeHistory.tradeValue;
        model.buyTimeStamp = moment(tradeHistory.tradeTimeStamp).format("YYYY-MM-DD");
        model.comodityAmount = tradeHistory.tradeSize;
        model.onSave = this.saveTrade;
        model.onDelete = this.deleteTrade;
        model.currencies = this.currencies;
        model.company = tradeHistory.company;
        return model;
    }

    private saveTrade = async (data: ComoditiesFormViewModel): Promise<void> => {
        const tradeHistory: ComodityTradeHistoryModel = {
            comodityTypeId: this.goldType.id,
            comodityUnitId: this.goldType.comodityUnitId,
            company: data.company,
            id: data.id,
            tradeSize: data.comodityAmount,
            currencySymbolId: data.currencySymbolId,
            tradeTimeStamp: moment(data.buyTimeStamp).toDate(),
            tradeValue: data.price
        };

        if (data.id)
            await this.comodityApi.comoditiesPut({ comodityTradeHistoryModel: tradeHistory });
        else
            await this.comodityApi.comoditiesPost({ comodityTradeHistoryModel: tradeHistory });

        this.setState({ openedForm: false, selectedModel: undefined });
        this.loadGoldData();
    }

    private deleteTrade = async (id: number): Promise<void> => {
        console.log("delete");
        // TODO: ask for confirmation (do universal confirmation modal window)
        // await this.comodityApi.comoditiesDelete({body: id});
    }

    private handleClose = () => this.setState({ openedForm: false });

    private comodityMenuClick(id: number) {
        let menu = this.state.comodityMenu;
        menu.forEach(a => a.selected = false);
        let selectedMenu = menu.filter(m => m.id == id)[0];
        selectedMenu.selected = true;
        this.setState({ comodityMenu: menu });
    }

    public render() {
        return (
            <div className="">
                <ThemeProvider theme={theme}>
                    <p className="text-3xl text-center mt-6">Comodities overview</p>
                    <div className="flex">
                        <div className="w-7/12 p-4 overflow-y-auto flex">
                            <div className="w-4/5 mx-auto px-10">
                                <Gold comoditiesViewModels={this.state.goldIngots} routeComponent={this.props.history}
                                    addNewIngot={() => this.addNewGold()} editIngot={this.editGold} />
                            </div>
                        </div>
                        <div className="w-5/12 p-4 overflow-y-auto flex flex-col">
                            {this.state.comodityMenu.map(c =>
                                (<div className={"mx-auto p-3 w-1/3 bg-gray-700 text-2xl text-center hover:bg-gray-600 duration-500 cursor-default " + (c.selected ? "bg-vermilion" : "")} onClick={_ => this.comodityMenuClick(c.id)}>{c.title}</div>)
                            )}
                        </div>
                    </div>
                    <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Detail transakce"
                        maxWidth="md" fullWidth={true}>
                        <DialogTitle id="form-dialog-title">Zlatý slitek</DialogTitle>
                        <DialogContent>
                            <ComoditiesForm {...this.state.selectedModel} />
                        </DialogContent>
                    </Dialog>
                </ThemeProvider>
            </div>
        );
    }
}