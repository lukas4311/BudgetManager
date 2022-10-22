import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { MainFrame } from "../MainFrame";
import { BaseList, IBaseModel } from "../BaseList";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { StockApi } from "../../ApiClient/Main/apis";
import { StockTickerModel, StockTradeHistoryGetModel } from "../../ApiClient/Main/models";
import moment from "moment";
import _ from "lodash";

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
}

class StockViewModel implements IBaseModel {
    id: number;
    tradeTimeStamp: string;
    stockTickerId: number;
    stockTicker: string;
    tradeSize: number;
    tradeValue: number;
    currencySymbolId: number;
    currencySymbol: string;
    onSave: (data: StockViewModel) => void;

    static mapFromDataModel(s: StockTradeHistoryGetModel): StockViewModel {
        return {
            currencySymbol: s.currencySymbol,
            currencySymbolId: s.currencySymbolId,
            id: s.id,
            stockTickerId: s.stockTickerId,
            tradeSize: s.tradeSize,
            tradeTimeStamp: moment(s.tradeTimeStamp).format("YYYY-MM-DD"),
            tradeValue: s.tradeValue,
            stockTicker: undefined,
            onSave: undefined
        };
    }
}

class StockOverview extends React.Component<RouteComponentProps, StockOverviewState> {
    private tickers: StockTickerModel[] = [];

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = {stocks: []};
    }

    public componentDidMount = () => this.init();

    private async init() {
        const apiFactory = new ApiClientFactory(this.props.history);
        const stockApi = await apiFactory.getClient(StockApi);
        this.tickers = await stockApi.stockStockTickerGet();
        const stockTrades = await stockApi.stockStockTradeHistoryGet();
        const stocks = stockTrades.map(s => {
            let viewModel = StockViewModel.mapFromDataModel(s);
            viewModel.onSave = this.saveStockTrade;
            viewModel.stockTicker = _.first(this.tickers.filter(f => f.id == viewModel.stockTickerId))?.ticker ?? "undefined" 
            return viewModel;
        });
        this.setState({ stocks });
    }

    private saveStockTrade = async (data: StockViewModel) => {
        console.log("Save stock");
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
        console.log("Show add stock trade")
    }

    private editStock = (id: number): void => {
        console.log("Edit stock trade")
    }

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
                    </>
                </MainFrame>
            </ThemeProvider>
        );
    }
}

export default StockOverview;