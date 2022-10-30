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
import { TextField } from "@material-ui/core";

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
    formKey: number;
    openedForm: boolean;
    selectedModel: StockViewModel;
}

class StockOverview extends React.Component<RouteComponentProps, StockOverviewState> {
    private tickers: StockTickerModel[] = [];
    private currencies: CurrencySymbol[] = [];
    private stockApi: StockApi = undefined;

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { stocks: [], formKey: Date.now(), openedForm: false, selectedModel: undefined };
    }

    public componentDidMount = () => this.init();

    private async init() {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.stockApi = await apiFactory.getClient(StockApi);
        const currencyApi = await apiFactory.getClient(CurrencyApi);
        this.tickers = await this.stockApi.stockStockTickerGet();
        this.currencies = (await currencyApi.currencyAllGet()).map(c => ({ id: c.id, symbol: c.symbol }));
        const stockTrades = await this.stockApi.stockStockTradeHistoryGet();
        const stocks = stockTrades.map(s => {
            let viewModel = StockViewModel.mapFromDataModel(s);
            viewModel.onSave = this.saveStockTrade;
            viewModel.stockTicker = _.first(this.tickers.filter(f => f.id == viewModel.stockTickerId))?.ticker ?? "undefined"
            return viewModel;
        });
        this.setState({ stocks });
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
        // this.loadData();
    }

    private renderTemplate = (s: StockViewModel): JSX.Element => {
        return (
            <>
                <p className="w-1/3 h-full border border-vermilion flex items-center justify-center">{s.stockTicker}</p>
                <p className="w-1/3 h-full border border-vermilion flex items-center justify-center">{s.tradeSize}</p>
                <p className="w-1/3 h-full border border-vermilion flex items-center justify-center">{s.tradeValue} ({s.currencySymbol})</p>
                <p className="w-1/3 h-full border border-vermilion flex items-center justify-center">{s.tradeTimeStamp}</p>
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
            </>
        );
    }

    private addStockTrade = (): void => {
        let model: StockViewModel = new StockViewModel();
        model.onSave = this.saveStockTrade;
        model.tradeTimeStamp = moment().format("YYYY-MM-DD");
        model.tradeSize = 0;
        model.tradeValue = 0;
        model.currencySymbolId = this.currencies[0].id;
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

    render() {
        return (
            <ThemeProvider theme={theme}>
                <MainFrame header='Stocks'>
                    <>
                        <div className="flex flex-row">
                            <div className="w-2/5">
                                <div className="m-5 overflow-y-scroll">
                                    <BaseList<StockViewModel> data={this.state.stocks} template={this.renderTemplate} header={this.renderHeader()}
                                        addItemHandler={this.addStockTrade} itemClickHandler={this.editStock} useRowBorderColor={true} hideIconRowPart={true}></BaseList>
                                </div>
                            </div>
                        </div>
                        <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Stock form"
                            maxWidth="md" fullWidth={true}>
                            <DialogTitle id="form-dialog-title">Investment form</DialogTitle>
                            <DialogContent>
                                <StockTradeForm stockTradeViewModel={this.state.selectedModel} currencies={this.currencies} stockTickers={this.tickers}/>
                            </DialogContent>
                        </Dialog>
                    </>
                </MainFrame>
            </ThemeProvider>
        );
    }
}

export default StockOverview;