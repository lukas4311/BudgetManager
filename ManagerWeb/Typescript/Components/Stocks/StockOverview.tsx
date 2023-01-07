import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { MainFrame } from "../MainFrame";
import { BaseList } from "../BaseList";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { CurrencyApi, StockApi } from "../../ApiClient/Main/apis";
import { CurrencySymbol, StockTickerModel, StockTradeHistoryModel } from "../../ApiClient/Main/models";
import moment from "moment";
import _ from "lodash";
import { Dialog, DialogContent, DialogTitle } from "@material-ui/core";
import { StockViewModel, TradeAction } from "../../Model/StockViewModel";
import { StockTradeForm } from "./StockTradeForm";
import { createMuiTheme, ThemeProvider, useTheme } from "@material-ui/core/styles";
import { AppContext, AppCtx } from "../../Context/AppCtx";
import StockService, { StockGroupModel } from "../../Services/StockService";
import { BuySellBadge } from "../Crypto/CryptoTrades";
import { Loading } from "../../Utils/Loading";
import { ComponentPanel } from "../../Utils/ComponentPanel";

const theme = createMuiTheme({
    palette: {
        type: 'dark',
        primary: {
            main: "#e03d15ff",
        }
    }
});

class StockSummary {
    totalyBought: number;
    totalySold: number;
}

interface StockOverviewState {
    stocks: StockViewModel[];
    stockGrouped: StockGroupModel[];
    formKey: number;
    openedForm: boolean;
    selectedModel: StockViewModel;
    stockSummary: StockSummary;
}

class StockOverview extends React.Component<RouteComponentProps, StockOverviewState> {
    private tickers: StockTickerModel[] = [];
    private currencies: CurrencySymbol[] = [];
    private stockApi: StockApi = undefined;
    private stockService: StockService = undefined;

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { stocks: [], stockGrouped: [], formKey: Date.now(), openedForm: false, selectedModel: undefined, stockSummary: undefined };
    }

    public componentDidMount = () => this.init();

    private async init() {
        const appContext: AppContext = this.context;
        const apiFactory = new ApiClientFactory(this.props.history);
        this.stockApi = await apiFactory.getClient(StockApi);
        const currencyApi = await apiFactory.getClient(CurrencyApi);
        this.stockService = new StockService(this.props.history, appContext.apiUrls);

        this.tickers = await this.stockService.getStockTickers();
        this.currencies = (await currencyApi.currencyAllGet()).map(c => ({ id: c.id, symbol: c.symbol }));
        this.loadStockData();
    }

    private loadStockData = async () => {
        const stocks = await this.stockService.getStockTradeHistory();
        let stockGrouped = await this.stockService.getGroupedTradeHistory();
        stockGrouped = _.orderBy(stockGrouped, a => a.stockValues, 'asc');
        const stockSummaryBuy = Math.abs(_.sumBy(stocks.filter(s => s.action == TradeAction.Buy), a => a.tradeValue));
        const stockSummarySell = Math.abs(_.sumBy(stocks.filter(s => s.action == TradeAction.Sell), a => a.tradeValue));
        this.setState({ stocks, stockGrouped, stockSummary: { totalyBought: stockSummaryBuy, totalySold: stockSummarySell } });
    }

    private saveStockTrade = async (data: StockViewModel) => {
        const stockHistoryTrade: StockTradeHistoryModel = {
            id: data.id,
            currencySymbolId: data.currencySymbolId,
            stockTickerId: data.stockTickerId,
            tradeSize: data.tradeSize,
            tradeTimeStamp: new Date(data.tradeTimeStamp),
            tradeValue: data.tradeValue
        };

        if (data.id)
            await this.stockApi.stockStockTradeHistoryPut({ stockTradeHistoryModel: stockHistoryTrade });
        else
            await this.stockApi.stockStockTradeHistoryPost({ stockTradeHistoryModel: stockHistoryTrade });

        this.setState({ openedForm: false, selectedModel: undefined });
        this.loadStockData();
    }

    private renderTemplate = (s: StockViewModel): JSX.Element => {
        return (
            <>
                <p className="w-1/5 h-full border border-vermilion flex items-center justify-center">{s.stockTicker}</p>
                <p className="w-1/5 h-full border border-vermilion flex items-center justify-center">{s.tradeSize}</p>
                <p className="w-1/5 h-full border border-vermilion flex items-center justify-center">{Math.abs(s.tradeValue).toFixed(2)} ({s.currencySymbol})</p>
                <p className="w-1/5 h-full border border-vermilion flex items-center justify-center">{s.tradeTimeStamp}</p>
                <p className="w-1/5 h-full border border-vermilion flex items-center justify-center py-1"><BuySellBadge tradeValue={s.tradeValue} /></p>
            </>
        );
    }

    private renderHeader = (): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/2">Stock ticker</p>
                <p className="mx-6 my-1 w-1/2">Size</p>
                <p className="mx-6 my-1 w-1/2">Value</p>
                <p className="mx-6 my-1 w-1/2">Time</p>
                <p className="mx-6 my-1 w-1/2"></p>
            </>
        );
    }

    private addStockTrade = (): void => {
        let model: StockViewModel = new StockViewModel();
        model.tradeTimeStamp = moment().format("YYYY-MM-DD");
        model.tradeSize = 0;
        model.tradeValue = 0;
        model.currencySymbolId = this.currencies[0].id;
        model.stockTickerId = this.tickers[0].id;
        this.setState({ openedForm: true, formKey: Date.now(), selectedModel: model });
    }

    private editStock = (id: number): void => {
        let selectedModel = this.state.stocks.filter(t => t.id == id)[0];
        this.showDetail(selectedModel);
    }

    private showDetail = (selectedModel: StockViewModel) =>
        this.setState({ openedForm: true, selectedModel: selectedModel, formKey: Date.now() });

    private handleClose = () =>
        this.setState({ openedForm: false, formKey: Date.now(), selectedModel: undefined });

    private deleteTrade = async (id: number): Promise<void> => {
        await this.stockApi.stockStockTradeHistoryDelete({ body: id });
        await this.loadStockData();
    }

    render() {
        return (
            <ThemeProvider theme={theme}>
                <MainFrame header='Stocks'>
                    <>
                        <div className="flex flex-row pt-5">
                            <ComponentPanel classStyle="w-7/12">
                                <div className="flex flex-col h-full">
                                    <div className="flex flex-col">
                                        <h2 className="text-xl font-semibold mb-6">Current portfolio</h2>
                                        <div className="flex flex-wrap justify-around ">
                                            {this.state.stockGrouped.map(g =>
                                                <div key={g.tickerId} className="w-3/12 bg-battleshipGrey border-2 border-vermilion p-4 mx-2 mb-6 rounded-xl">
                                                    <div className="grid grid-cols-2">
                                                        <p className="text-xl font-bold text-left">{g.tickerName}</p>
                                                        <div>
                                                            <p className="text-lg text-left">{g.size.toFixed(3)}</p>
                                                            <p className="text-lg text-left">{Math.abs(g.stockValues).toFixed(2)} Kč</p>
                                                        </div>
                                                    </div>
                                                </div>)}
                                        </div>
                                    </div>
                                    <div className="m-5 flex flex-col">
                                        <h2 className="text-xl font-semibold mb-6">All trades</h2>
                                        <div className="overflow-y-scroll">
                                            <BaseList<StockViewModel> data={this.state.stocks} template={this.renderTemplate} header={this.renderHeader()} dataAreaClass="h-70vh overflow-y-auto"
                                                addItemHandler={this.addStockTrade} itemClickHandler={this.editStock} useRowBorderColor={true} deleteItemHandler={this.deleteTrade} ></BaseList>
                                        </div>
                                    </div>
                                </div>
                            </ComponentPanel>
                            <ComponentPanel classStyle="w-5/12">
                                <>
                                    <h2 className="text-xl font-semibold mb-6">Stock summary</h2>
                                    {this.state.stockSummary == undefined ? <Loading className="m-auto mt-4" /> : (
                                        <div className='flex flex-col text-white font-semibold text-left px-4 justify-evenly'>
                                            <p className="">Totaly bought: {this.state.stockSummary.totalyBought}</p>
                                            <p className="mt-2">Totaly sold: {this.state.stockSummary.totalySold}</p>
                                        </div>
                                    )}
                                </>
                            </ComponentPanel>
                        </div>
                        <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Stock form"
                            maxWidth="md" fullWidth={true}>
                            <DialogTitle id="form-dialog-title" className="bg-prussianBlue">Investment form</DialogTitle>
                            <DialogContent className="bg-prussianBlue">
                                <StockTradeForm stockTradeViewModel={this.state.selectedModel} currencies={this.currencies} stockTickers={this.tickers} onSave={this.saveStockTrade} />
                            </DialogContent>
                        </Dialog>
                    </>
                </MainFrame>
            </ThemeProvider>
        );
    }
}

StockOverview.contextType = AppCtx;

export default StockOverview;