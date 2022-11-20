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
import { StockViewModel } from "../../Model/StockViewModel";
import { StockTradeForm } from "./StockTradeForm";
import { createMuiTheme, ThemeProvider, useTheme } from "@material-ui/core/styles";
import { AppContext, AppCtx } from "../../Context/AppCtx";
import StockService from "../../Services/StockService";
import { BuySellBadge } from "../Crypto/CryptoTrades";

const theme = createMuiTheme({
    palette: {
        type: 'dark',
        primary: {
            main: "#e03d15ff",
        }
    }
});

interface StockOverviewState {
    stocks: StockViewModel[];
    stockGrouped: StockGroupModel[];
    formKey: number;
    openedForm: boolean;
    selectedModel: StockViewModel;
}

class StockGroupModel {
    tickerId: number;
    tickerName: string;
    size: number;
    stockValues: number;
}

class StockOverview extends React.Component<RouteComponentProps, StockOverviewState> {
    private tickers: StockTickerModel[] = [];
    private currencies: CurrencySymbol[] = [];
    private stockApi: StockApi = undefined;
    private stockService: StockService = undefined;

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { stocks: [], stockGrouped: [], formKey: Date.now(), openedForm: false, selectedModel: undefined };
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
        let stockGrouped = this.groupeStocks(stocks);
        stockGrouped = _.orderBy(stockGrouped, a => a.stockValues, 'desc');
        this.setState({ stocks, stockGrouped });
    }

    private groupeStocks = (stocks: StockViewModel[]): StockGroupModel[] => {
        let values: StockGroupModel[] = _.chain(stocks)
            .groupBy(g => g.stockTickerId)
            .map((group) => {
                let groupModel: StockGroupModel = new StockGroupModel();
                groupModel.tickerId = group[0].stockTickerId;
                groupModel.tickerName = _.first(this.tickers.filter(t => t.id == group[0].stockTickerId)).ticker;
                groupModel.size = _.sumBy(group, s => s.tradeSize);
                groupModel.stockValues = _.sumBy(group, s => s.tradeValue);
                return groupModel;
            })
            .value();
        return values;
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
                        <div className="flex flex-row">
                            <div className="w-7/12">
                                <div className="m-5 flex flex-col">
                                    <h2 className="text-xl font-semibold mb-6">All trades</h2>
                                    <div className="overflow-y-scroll">
                                        <BaseList<StockViewModel> data={this.state.stocks} template={this.renderTemplate} header={this.renderHeader()}
                                            addItemHandler={this.addStockTrade} itemClickHandler={this.editStock} useRowBorderColor={true} deleteItemHandler={this.deleteTrade}></BaseList>
                                    </div>
                                </div>
                            </div>
                            <div className="w-5/12">
                                <div className="m-5 flex flex-col">
                                    <h2 className="text-xl font-semibold mb-6">Current portfolio</h2>
                                    <div className="flex flex-wrap justify-around ">
                                        {this.state.stockGrouped.map(g =>
                                            <div key={g.tickerId} className="w-3/12 bg-battleshipGrey border-2 border-vermilion p-4 mx-2 mb-6 rounded-xl">
                                                <div className="grid grid-cols-2">
                                                    <p className="text-xl font-bold text-left">{g.tickerName}</p>
                                                    <div>
                                                        <p className="text-lg text-left">{g.size.toFixed(2)}</p>
                                                        <p className="text-lg text-left">{Math.abs(g.stockValues).toFixed(2)} Kč</p>
                                                    </div>
                                                </div>
                                            </div>)}
                                    </div>
                                </div>
                            </div>
                        </div>
                        <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Stock form"
                            maxWidth="md" fullWidth={true}>
                            <DialogTitle id="form-dialog-title">Investment form</DialogTitle>
                            <DialogContent>
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