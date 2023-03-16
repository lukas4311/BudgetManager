import _ from "lodash";
import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { ComodityApi, ComodityApiInterface, CurrencyApi } from "../../ApiClient/Main/apis";
import { ComodityTradeHistoryModel, ComodityTypeModel } from "../../ApiClient/Main/models";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import Gold from "./Gold";
import { Dialog, DialogContent, DialogTitle } from "@material-ui/core";
import { ComoditiesForm, ComoditiesFormViewModel } from "./ComoditiesForm";
import moment from "moment";
import CurrencyTickerSelectModel from "../Crypto/CurrencyTickerSelectModel";
import { ConfirmationForm, ConfirmationResult } from "../ConfirmationForm";
import { MainFrame } from "../MainFrame";
import { ComponentPanel } from "../../Utils/ComponentPanel";
import ComodityService from "../../Services/ComodityService";
import { CurrencyService } from "../../Services/CurrencyService";

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
    comodityMenu: Array<ComodityMenuItem>;
    confirmDialogKey: number;
    confirmDialogIsOpen: boolean;
}

class ComodityMenuItem {
    title: string;
    id: number;
    selected: boolean;
}

export default class Comodities extends React.Component<RouteComponentProps, ComoditiesState>{
    private goldCode: string = 'AU';
    private goldType: ComodityTypeModel;
    private currencies: CurrencyTickerSelectModel[];
    private confirmationDeleteId: number;
    private comodityService: ComodityService;
    private currencyService: CurrencyService;

    constructor(props: RouteComponentProps) {
        super(props);
        const menu: Array<ComodityMenuItem> = [
            { id: 1, title: "Gold", selected: true },
            { id: 2, title: "Silver", selected: false },
            { id: 3, title: "Others", selected: false }
        ];
        this.state = {
            goldIngots: [], openedForm: false, dialogTitle: "", selectedModel: undefined, formKey: Date.now(),
            comodityMenu: menu, confirmDialogIsOpen: false, confirmDialogKey: Date.now()
        };
    }

    componentDidMount(): void {
        this.init();
    }

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        const comodityApi = await apiFactory.getClient(ComodityApi);
        const currencyApi = await apiFactory.getClient(CurrencyApi);
        this.comodityService = new ComodityService(comodityApi);
        this.currencyService = new CurrencyService(currencyApi);
        this.loadData();
    }

    private loadData = async () => {
        const comodityTypes = await this.comodityService.getComodityTypes();
        this.goldType = _.first(comodityTypes.filter(c => c.code == this.goldCode));
        this.currencies = await this.currencyService.getAllCurrencies();
        await this.loadGoldData();
    }

    private loadGoldData = async () => {
        const goldIngots = await this.comodityService.getAllComodityTrades(this.goldType.id);
        this.setState({ goldIngots: goldIngots });
    }

    private addNewGold = () => {
        let model: ComoditiesFormViewModel = new ComoditiesFormViewModel();
        model.buyTimeStamp = moment().format("YYYY-MM-DD");
        model.comodityTypeName = "Gold";
        model.comodityUnit = this.goldType.comodityUnit;
        model.price = 0;
        model.company = "";
        model.comodityAmount = 0;
        model.currencySymbolId = this.currencies[0].id;
        this.setState({ openedForm: true, formKey: Date.now(), selectedModel: model });
    }


    private editGold = (id: number) => {
        let tradeHistory = this.state.goldIngots.filter(t => t.id == id)[0];
        this.setState({ selectedModel: tradeHistory, openedForm: true });
        this.setState({ openedForm: true, dialogTitle: "Gold" });
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
            await this.comodityService.updateComodityTrade(tradeHistory);
        else
            await this.comodityService.createComodityTrade(tradeHistory);

        this.setState({ openedForm: false, selectedModel: undefined });
        this.loadGoldData();
    }

    private deleteTradeConfirm = async (id: number): Promise<void> => {
        this.confirmationDeleteId = id;
        this.setState({ confirmDialogIsOpen: true, confirmDialogKey: Date.now() });
    }

    private deleteTrade = async (res: ConfirmationResult) => {
        if (res == ConfirmationResult.Ok)
            await this.comodityService.deleteComodityTrade(this.confirmationDeleteId);

        this.confirmationDeleteId = undefined;
        this.setState({ confirmDialogIsOpen: false, openedForm: false, selectedModel: undefined });
        await this.loadGoldData();
    }

    private handleClose = () => this.setState({ openedForm: false });

    private comodityMenuClick(id: number) {
        let menu = this.state.comodityMenu;
        menu.forEach(a => a.selected = false);
        let selectedMenu = menu.filter(m => m.id == id)[0];
        selectedMenu.selected = true;
        this.setState({ comodityMenu: menu });
    }

    private showSelectedComponent = (): JSX.Element => {
        let component: JSX.Element =
            (<div>
                <h3 className="text-xl text-center">You don't have any comodity of this type</h3>
            </div>)
        const selectedMenu = _.first(this.state.comodityMenu.filter(a => a.selected == true));

        if (selectedMenu.id == 1)
            component = <Gold comoditiesViewModels={this.state.goldIngots} routeComponent={this.props.history}
                addNewIngot={() => this.addNewGold()} editIngot={this.editGold} />

        return component;
    }

    public render() {
        return (
            <div className="">
                <ThemeProvider theme={theme}>
                    <MainFrame header='Comodities overview'>
                        <ComponentPanel classStyle="w-2/3 mx-auto">
                            <>
                                <div className="flex">
                                    <div className="w-7/12 overflow-y-auto flex p-4">
                                        <div className="w-4/5 mx-auto px-10">
                                            {this.showSelectedComponent()}
                                        </div>
                                    </div>
                                    <div className="w-5/12 overflow-y-auto flex flex-col items-stretch content-between">
                                        {this.state.comodityMenu.map(c =>
                                        (
                                            <div className={"p-3 ml-auto flex flex-col flex-grow justify-center w-1/3 bg-gray-700 text-2xl text-center hover:bg-gray-600 duration-500 cursor-default " + (c.selected ? "bg-vermilion" : "")} onClick={_ => this.comodityMenuClick(c.id)}>
                                                <span className="">{c.title}</span>
                                            </div>
                                        )
                                        )}
                                    </div>
                                </div>
                                <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Detail transakce"
                                    maxWidth="md" fullWidth={true}>
                                    <DialogTitle id="form-dialog-title" className="bg-prussianBlue">Golden ingots</DialogTitle>
                                    <DialogContent className="bg-prussianBlue">
                                        {this.state.selectedModel != undefined ?
                                            <ComoditiesForm viewModel={this.state.selectedModel} onSave={this.saveTrade} onDelete={this.deleteTradeConfirm} currencies={this.currencies} /> :
                                            <></>
                                        }
                                    </DialogContent>
                                </Dialog>
                                <ConfirmationForm key={this.state.confirmDialogKey} onClose={() => this.deleteTrade(ConfirmationResult.Cancel)} onConfirm={this.deleteTrade} isOpen={this.state.confirmDialogIsOpen} />
                            </>
                        </ComponentPanel>
                    </MainFrame>
                </ThemeProvider>
            </div>
        );
    }
}