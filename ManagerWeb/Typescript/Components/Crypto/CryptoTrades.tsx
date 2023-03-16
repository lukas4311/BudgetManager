import { Button, Dialog, DialogContent, DialogTitle } from "@material-ui/core";
import moment from "moment";
import React from "react";
import { CryptoApi, CryptoApiInterface, CurrencyApi } from "../../ApiClient/Main/apis";
import { CryptoTradeForm, CryptoTradeViewModel } from "./CryptoTradeForm";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { BaseList, IBaseModel } from "../BaseList";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { RouteComponentProps } from "react-router-dom";
import { CryptoTicker, TradeHistory } from "../../ApiClient/Main/models";
import CryptoTickerSelectModel from "./CryptoTickerSelectModel";
import { ComponentPanel } from "../../Utils/ComponentPanel";
import { CurrencyService } from "../../Services/CurrencyService";

class CryptoTradesState {
    trades: CryptoTradeViewModel[];
    openedForm: boolean;
    selectedTrade: CryptoTradeViewModel;
    cryptoFormKey: number;
}

const theme = createMuiTheme({
    palette: {
        type: 'dark',
    }
});

export default class CryptoTrades extends React.Component<RouteComponentProps, CryptoTradesState> {
    private cryptoApi: CryptoApiInterface;
    private currencyApi: CurrencyApi;
    private cryptoTickers: CryptoTickerSelectModel[];
    private currencies: any[];
    currencyService: any;


    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { trades: [], openedForm: false, selectedTrade: undefined, cryptoFormKey: Date.now() };
    }

    public componentDidMount() {
        this.init();
    }

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        const currencyApi = await apiFactory.getClient(CurrencyApi);
        this.currencyService = new CurrencyService(currencyApi);
        this.loadCryptoTradesData();
    }

    // private async load(): Promise<void> {
    //     const apiFactory = new ApiClientFactory(this.props.history);
    //     this.cryptoApi = await apiFactory.getClient(CryptoApi);
    //     this.currencyApi = await apiFactory.getClient(CurrencyApi);

    //     this.loadCryptoTradesData();
    // }

    private loadCryptoTradesData = async () => {
        this.cryptoTickers = (await this.cryptoApi.cryptosTickersGet()).map(c => ({ id: c.id, ticker: c.ticker }));
        this.currencies = (await this.currencyApi.currencyAllGet()).map(c => ({ id: c.id, ticker: c.symbol }));
        let tradesData: TradeHistory[] = await this.cryptoApi.cryptosAllGet();
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
        model.currencies = this.currencies;
        model.cryptoTickers = this.cryptoTickers;
        return model;
    }

    private saveTrade = (data: CryptoTradeViewModel): void => {
        const tradeHistory: TradeHistory = {
            cryptoTickerId: data.cryptoTickerId,
            currencySymbolId: data.currencySymbolId,
            id: data.id,
            tradeSize: data.tradeSize,
            tradeTimeStamp: moment(data.tradeTimeStamp).toDate(),
            tradeValue: data.tradeValue
        };

        if (data.id)
            this.cryptoApi.cryptosPut({ tradeHistory });
        else
            this.cryptoApi.cryptosPost({ tradeHistory });

        this.setState({ openedForm: false, selectedTrade: undefined });
        this.loadCryptoTradesData();
    }

    private addNewItem = (): void => {
        let model: CryptoTradeViewModel = new CryptoTradeViewModel();
        model.onSave = this.saveTrade;
        model.currencies = this.currencies;
        model.cryptoTickers = this.cryptoTickers;
        model.cryptoTickerId = this.cryptoTickers[0].id;
        model.currencySymbolId = this.currencies[0].id;
        model.tradeTimeStamp = moment().format("YYYY-MM-DD");
        model.tradeSize = 0;
        model.tradeValue = 0;
        this.setState({ openedForm: true, cryptoFormKey: Date.now(), selectedTrade: model });
    }

    private budgetEdit = async (id: number): Promise<void> => {
        let tradeHistory = this.state.trades.filter(t => t.id == id)[0];
        this.setState({ selectedTrade: tradeHistory, openedForm: true });
    }

    private deleteTrade = async (id: number) => {
        await this.cryptoApi.cryptosDelete({ body: id });
        this.loadCryptoTradesData();
    }

    private handleClose = () => this.setState({ openedForm: false });

    private renderHeader = (): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/10">Ticker</p>
                <p className="mx-6 my-1 w-3/10">Trade size</p>
                <p className="mx-6 my-1 w-2/10">Date</p>
                <p className="mx-6 my-1 w-3/10">Value</p>
                <p className="mx-6 my-1 w-1/10">Currency</p>
                <p className="mx-6 my-1 w-1/10"></p>
            </>
        );
    }

    private renderTemplate = (p: CryptoTradeViewModel): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/10">{p.cryptoTicker.toUpperCase()}</p>
                <p className="mx-6 my-1 w-3/10">{p.tradeSize}</p>
                <p className="mx-6 my-1 w-2/10">{moment(p.tradeTimeStamp).format('DD.MM.YYYY')}</p>
                <p className="mx-6 my-1 w-3/10">{Math.abs(p.tradeValue).toFixed(2)}</p>
                <p className="mx-6 my-1 w-1/10">{p.currencySymbol}</p>
                <p className="mx-6 my-1 w-1/10"><BuySellBadge tradeValue={p.tradeValue} /></p>
            </>
        );
    }

    render() {
        return (
            <ComponentPanel>
                <div className="pr-5 h-full">
                    <ThemeProvider theme={theme}>
                        <BaseList<CryptoTradeViewModel> title="Trade list" data={this.state.trades} template={this.renderTemplate} deleteItemHandler={this.deleteTrade}
                            header={this.renderHeader()} addItemHandler={this.addNewItem} itemClickHandler={this.budgetEdit} dataAreaClass="h-70vh overflow-y-auto">
                        </BaseList>
                        <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Detail transakce"
                            maxWidth="md" fullWidth={true}>
                            <DialogTitle id="form-dialog-title" className="bg-prussianBlue">Detail transakce</DialogTitle>
                            <DialogContent className="bg-prussianBlue">
                                <div className="p-2 overflow-y-auto">
                                    <CryptoTradeForm
                                        {...this.state.selectedTrade}
                                    ></CryptoTradeForm>
                                </div>
                            </DialogContent>
                        </Dialog>
                    </ThemeProvider>
                </div>
            </ComponentPanel>
        );
    }
}

export const BuySellBadge = (props: { tradeValue: number }) =>
    (<span className={(props.tradeValue > 0 ? "bg-red-700" : "bg-green-700") + " px-2 py-1 text-xs font-meduim"}>{props.tradeValue > 0 ? "SELL" : "BUY"}</span>);