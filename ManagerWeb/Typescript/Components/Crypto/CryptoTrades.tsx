import { Button, Dialog, DialogContent, DialogTitle } from "@material-ui/core";
import moment from "moment";
import React from "react";
import { Configuration, CryptoApi, CryptoApiInterface, TradeHistory } from "../../ApiClient";
import { CryptoTradeForm, CryptoTradeViewModel } from "./CryptoTradeForm";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { BaseList, IBaseModel } from "../BaseList";

class CryptoTradesState {
    trades: CryptoTradeViewModel[];
    openedForm: boolean;
    selectedTrade: CryptoTradeViewModel;
}

const theme = createMuiTheme({
    palette: {
        type: 'dark',
    }
});

export default class CryptoTrades extends React.Component<{}, CryptoTradesState> {
    cryptoInterface: CryptoApiInterface;

    constructor(props: {}) {
        super(props);
        this.cryptoInterface = new CryptoApi(new Configuration({ basePath: "https://localhost:5001" }));
        this.state = { trades: [], openedForm: false, selectedTrade: undefined };
    }

    public componentDidMount() {
        this.load();
    }

    private async load(): Promise<void> {
        let tradesData: TradeHistory[] = await this.cryptoInterface.cryptoGetAllGet();
        let trades: CryptoTradeViewModel[] = tradesData.map(t => this.mapDataModelToViewModel(t));
        trades.sort((a, b) => moment(a.tradeTimeStamp).format("YYYY-MM-DD") > moment(b.tradeTimeStamp).format("YYYY-MM-DD") ? 1 : -1);
        this.setState({ trades });
    }

    private mapDataModelToViewModel = (tradeHistory: TradeHistory): CryptoTradeViewModel => {
        let model: CryptoTradeViewModel = new CryptoTradeViewModel();
        model.cryptoTicker = tradeHistory.cryptoTicker;
        model.cryptoTickerId = tradeHistory.cryptoTickerId;
        model.currencySymbol = tradeHistory.currencySymbol;
        model.currencySymbolId = tradeHistory.currencySymbolId;
        model.id = tradeHistory.id;
        model.tradeSize = tradeHistory.tradeSize;
        model.tradeTimeStamp = moment(tradeHistory.tradeTimeStamp).format("YYYY-MM-DD");
        model.tradeValue = tradeHistory.tradeValue;
        model.onSave = this.saveTrade;
        return model;
    }

    private saveTrade = (data: CryptoTradeViewModel): void => {
        console.log(`Saved ${data}`)
    }

    private handleClickOpen = (tradeHistory: CryptoTradeViewModel) => {
        this.setState({ selectedTrade: tradeHistory, openedForm: true });
    };

    private handleClose = () => {
        this.setState({ openedForm: false });
    };

    private renderHeader = (): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/10">Ticker</p>
                <p className="mx-6 my-1 w-3/10">Velikost tradu</p>
                <p className="mx-6 my-1 w-2/10">Datum tradu</p>
                <p className="mx-6 my-1 w-3/10">Celkova hodnota</p>
                <p className="mx-6 my-1 w-1/10">Měna</p>
            </>
        );
    }

    private renderTemplate = (p: CryptoTradeViewModel): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/10">{p.cryptoTicker.toUpperCase()}</p>
                <p className="mx-6 my-1 w-3/10">{p.tradeSize}</p>
                <p className="mx-6 my-1 w-2/10">{moment(p.tradeTimeStamp).format('DD.MM.YYYY')}</p>
                <p className="mx-6 my-1 w-3/10">{p.tradeValue.toFixed(2)}</p>
                <p className="mx-6 my-1 w-1/10">{p.currencySymbol}</p>
            </>
        );
    }

    private addNewItem = (): void => {
        // this.setState({ showBudgetFormModal: true, budgetFormKey: Date.now(), selectedBudget: undefined });
    }

    private budgetEdit = async (id: number): Promise<void> => {
        let tradeHistory = this.state.trades.filter(t => t.id == id)[0];
        this.setState({ selectedTrade: tradeHistory, openedForm: true });
        // const budgetModel: BudgetModel = await this.budgetApi.budgetGetGet({ id: id });
        // let budgetFormModel: BudgetFormModel = {
        //     amount: budgetModel.amount, from: moment(budgetModel.dateFrom).format("YYYY-MM-DD"),
        //     to: moment(budgetModel.dateTo).format("YYYY-MM-DD"), id: budgetModel.id, name: budgetModel.name, onSave: this.saveFormData
        // };
        // this.setState({ selectedBudgetId: id, showBudgetFormModal: true, budgetFormKey: Date.now(), selectedBudget: budgetFormModel });
    }

    render() {
        return (
            <div className="pr-5 h-full">
                <ThemeProvider theme={theme}>
                    <BaseList<CryptoTradeViewModel> title="Trade list" data={this.state.trades} template={this.renderTemplate}
                        header={this.renderHeader()} addItemHandler={this.addNewItem} itemClickHandler={this.budgetEdit}>
                    </BaseList>
                    <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Detail transakce"
                        maxWidth="md" fullWidth={true}>
                        <DialogTitle id="form-dialog-title">Detail transakce</DialogTitle>
                        <DialogContent>
                            <div className="p-2 overflow-y-auto">
                                <CryptoTradeForm
                                    {...this.state.selectedTrade}
                                ></CryptoTradeForm>
                            </div>
                        </DialogContent>
                    </Dialog>
                </ThemeProvider>
            </div>
        );
    }
}