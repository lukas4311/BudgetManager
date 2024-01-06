import { Button, Dialog, DialogContent, DialogTitle } from "@material-ui/core";
import moment from "moment";
import React, { MouseEventHandler } from "react";
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
import CryptoService from "../../Services/CryptoService";
import { CryptoEndpointsApi, ForexEndpointsApi } from "../../ApiClient/Fin";
import PublishIcon from '@material-ui/icons/Publish';

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
    private cryptoTickers: CryptoTickerSelectModel[];
    private currencies: any[];
    private currencyService: any;
    private cryptoService: CryptoService;

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
        this.cryptoApi = await apiFactory.getClient(CryptoApi);
        const forexApi = await apiFactory.getFinClient(ForexEndpointsApi);
        const cryptoFin = await apiFactory.getFinClient(CryptoEndpointsApi);
        this.currencyService = new CurrencyService(currencyApi);
        this.cryptoService = new CryptoService(this.cryptoApi, forexApi, cryptoFin, forexApi);
        this.loadCryptoTradesData();
    }

    private loadCryptoTradesData = async () => {
        this.cryptoTickers = await this.cryptoService.getCryptoTickers();
        this.currencies = await this.currencyService.getAllCurrencies();
        let trades: CryptoTradeViewModel[] = await this.cryptoService.getTradeData();
        trades.sort((a, b) => moment(a.tradeTimeStamp).format("YYYY-MM-DD") > moment(b.tradeTimeStamp).format("YYYY-MM-DD") ? 1 : -1);
        this.setState({ trades });
    }

    private saveTrade = async (data: CryptoTradeViewModel): Promise<void> => {
        if (data.id)
            await this.cryptoService.updateCryptoTrade(data);
        else
            await this.cryptoService.createCryptoTrade(data);

        this.setState({ openedForm: false, selectedTrade: undefined });
        this.loadCryptoTradesData();
    }

    private addNewItem = (): void => {
        let model: CryptoTradeViewModel = new CryptoTradeViewModel();
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
        await this.cryptoService.deleteCryptoTrade(id);
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

    private uploadBrokerReport = async (e: React.ChangeEvent<HTMLInputElement>) => {
        if (!e.target.files)
            return;

        const files: Blob = (e.target as HTMLInputElement).files?.[0];
        await this.cryptoApi.cryptosBrokerReportPost({ file: files });
    }

    render() {
        return (
            <ComponentPanel>
                <React.Fragment>
                    <Button
                        component="label"
                        variant="outlined"
                        color="primary"
                        className="block mr-auto bg-vermilion text-white mb-3 w-1/3"

                    >
                        <div className="flex flex-row justify-center">
                            <PublishIcon />
                            <span className="ml-4">Upload crypto report</span>
                        </div>
                        <input type="file" accept=".csv" hidden onChange={this.uploadBrokerReport} />
                    </Button>
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
                                        <CryptoTradeForm onSave={this.saveTrade} currencies={this.currencies} cryptoTickers={this.cryptoTickers}
                                            viewModel={this.state.selectedTrade} />
                                    </div>
                                </DialogContent>
                            </Dialog>
                        </ThemeProvider>
                    </div>
                </React.Fragment>
            </ComponentPanel>
        );
    }
}

export const BuySellBadge = (props: { tradeValue: number }) =>
    (<span className={(props.tradeValue > 0 ? "bg-red-700" : "bg-green-700") + " px-2 py-1 text-xs font-meduim"}>{props.tradeValue > 0 ? "SELL" : "BUY"}</span>);