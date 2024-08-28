import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { MainFrame } from "../MainFrame";
import { BaseList } from "../BaseList";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { CryptoApi, CurrencyApi, EnumApi, StockApi } from "../../ApiClient/Main/apis";
import { CompanyProfileModel, CurrencySymbol, StockPrice, StockTickerModel, StockTradeHistoryModel } from "../../ApiClient/Main/models";
import moment from "moment";
import _, { max } from "lodash";
// import { Button, ButtonGroup, Dialog, DialogContent, DialogTitle } from "@mui/material";
import { StockViewModel, TradeAction } from "../../Model/StockViewModel";
import { StockTradeForm } from "./StockTradeForm";
import { createMuiTheme, ThemeProvider, useTheme } from "@mui/material/styles";
import { AppContext, AppCtx } from "../../Context/AppCtx";
import StockService, { StockGroupModel, TickersWithPriceHistory } from "../../Services/StockService";
import { BuySellBadge } from "../Crypto/CryptoTrades";
import { Loading } from "../../Utils/Loading";
import { ComponentPanel } from "../../Utils/ComponentPanel";
import { LineChartDataSets } from "../../Model/LineChartDataSets";
import { LineChartData } from "../../Model/LineChartData";
import { LineChart } from "../Charts/LineChart";
import { LineChartSettingManager } from "../Charts/LineChartSettingManager";
import { CompanyProfile } from "./CompanyProfile";
import CryptoService from "../../Services/CryptoService";
import { CryptoEndpointsApi, ForexEndpointsApi, StockEndpointsApi } from "../../ApiClient/Fin";
import ArrowDropUpIcon from '@mui/icons-material/ArrowDropUp';
import ArrowDropDownIcon from '@mui/icons-material/ArrowDropDown';
import { PieChart, PieChartData } from "../Charts/PieChart";
import { IStockService } from "../../Services/IStockService";
import { LineChartProps } from "../../Model/LineChartProps";
import { ToggleButtonGroup, ToggleButton, Button, Dialog, DialogTitle, DialogContent, TextField, Select, MenuItem } from "@mui/material";
import PublishIcon from '@mui/icons-material/Publish';
import { SnackbarSeverity } from "../../App";
import { NewTickerForm } from "./NewTickerForm";
import NewTickerModel from "../../Model/NewTickerModel";


enum DisplayChioce {
    Portfolio = "Portfolio",
    Trades = "Trades"
}

class StockSummary {
    totalWealth: number;
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
    stockPrice: TickersWithPriceHistory[];
    selectedCompany: StockComplexModel;
    lineChartData: LineChartProps;
    selectedDisplayChoice: string;
    isOpenedTickerRequest: boolean;
    stockBrokerParsers: Map<number, string>;
    selectedBroker: number;
    isFileUploadOpened: boolean;
}

export class StockComplexModel {
    ticker: string;
    companyInfo: CompanyProfileModel;
    company5YPrice: StockPrice[];
    trades: StockViewModel[];
}

class StockOverview extends React.Component<RouteComponentProps, StockOverviewState> {
    private stockParsersTypeCode = 'AvailableStockBrokerParsers';
    private tickers: StockTickerModel[] = [];
    private currencies: CurrencySymbol[] = [];
    private stockApi: StockApi = undefined;
    private stockService: IStockService = undefined;
    private cryptoApi: CryptoApi;
    private cryptoFinApi: CryptoEndpointsApi;
    private forexFinApi: ForexEndpointsApi;

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = {
            stocks: [], stockGrouped: [], formKey: Date.now(), openedForm: false, selectedModel: undefined, stockSummary: undefined, stockPrice: [], selectedCompany: undefined, lineChartData: { dataSets: [] },
            selectedDisplayChoice: DisplayChioce.Portfolio, isOpenedTickerRequest: false, stockBrokerParsers: new Map<number, string>(), selectedBroker: -1, isFileUploadOpened: false
        };
    }

    public componentDidMount = () => this.init();

    private async init() {
        const appContext: AppContext = this.context as AppContext;
        const apiFactory = new ApiClientFactory(this.props.history);
        this.stockApi = await apiFactory.getClient(StockApi);
        const currencyApi = await apiFactory.getClient(CurrencyApi);
        const enumApi = await apiFactory.getClient(EnumApi);
        this.cryptoApi = await apiFactory.getClient(CryptoApi);
        this.cryptoFinApi = await apiFactory.getFinClient(CryptoEndpointsApi);
        this.forexFinApi = await apiFactory.getFinClient(ForexEndpointsApi);
        const stockFinApi = await apiFactory.getFinClient(StockEndpointsApi);
        this.stockService = new StockService(this.stockApi, new CryptoService(this.cryptoApi, this.forexFinApi, this.cryptoFinApi, this.forexFinApi), this.forexFinApi, stockFinApi);

        const parsers = await enumApi.typeEnumItemTypeCodeGet({ enumItemTypeCode: this.stockParsersTypeCode });
        this.tickers = await this.stockService.getStockTickers();
        this.currencies = (await currencyApi.currencyAllGet()).map(c => ({ id: c.id, symbol: c.symbol }));
        this.setState({ stockBrokerParsers: new Map(parsers.map(p => [p.id, p.name])) });
        this.loadStockData();
    }

    private loadStockData = async () => {
        const stocks = await this.stockService.getStockTradeHistory();
        let stockGrouped = await this.stockService.getStocksTickerGroupedTradeHistory();

        // console.log(getAccumulatedNetWorh);
        const stockSummaryBuy = _.sumBy(stockGrouped, s => s.stockSpentPrice);
        const stockSummarySell = _.sumBy(stockGrouped, s => s.stockSellPrice);
        const stockSummaryWealth = _.sumBy(stockGrouped, s => s.stockCurrentWealth);
        const tickers = stockGrouped.map(a => a.tickerName);
        const tickerPrices = await this.stockService.getLastMonthTickersPrice(tickers);
        const lineChartData = await this.prepareStockDataToLineChart();

        stockGrouped = _.orderBy(stockGrouped, a => a.stockCurrentWealth, 'desc');
        this.setState({ stocks, stockGrouped, stockSummary: { totalyBought: stockSummaryBuy, totalySold: stockSummarySell, totalWealth: stockSummaryWealth }, stockPrice: tickerPrices, lineChartData: { dataSets: lineChartData } });
    }

    private prepareStockDataToLineChart = async () => {
        let getAccumulatedValue = await this.stockService.getStocksAccumulatedValue();
        let priceData: LineChartData[] = [];
        let lineChartData: LineChartDataSets[] = [{ id: 'Price', data: [] }];

        if (getAccumulatedValue.size > 2) {
            for (const [dateKey, tickerValues] of getAccumulatedValue) {
                let totalDayValue = 0;
                tickerValues.forEach(d => totalDayValue += d);
                priceData.push({ x: dateKey, y: totalDayValue });
            }
        }

        lineChartData = [{ id: 'Price', data: priceData }];
        return lineChartData;
    }

    private saveStockTrade = async (stockViewModel: StockViewModel) => {
        if (stockViewModel.id)
            await this.stockService.updateStockTradeHistory(stockViewModel);
        else
            await this.stockService.createStockTradeHistory(stockViewModel);

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
        await this.stockService.deleteStockTradeHistory(id);
        await this.loadStockData();
    }

    private renderChart = (ticker: string) => {
        let lineChartData: LineChartDataSets[] = [{ id: 'Price', data: [] }];
        let tradeHistory = _.first(this.state.stockPrice.filter(f => f.ticker == ticker));

        if (tradeHistory != undefined && tradeHistory.price.length > 5) {
            let prices = tradeHistory.price;
            const sortedArray = _.orderBy(prices, [(obj) => new Date(obj.time)], ['asc']);
            let priceData: LineChartData[] = sortedArray.map(b => ({ x: moment(b.time).format('YYYY-MM-DD'), y: b.price }));
            lineChartData = [{ id: 'Price', data: priceData }];
        }

        return (
            <div className="h-8">
                <LineChart dataSets={lineChartData} chartProps={LineChartSettingManager.getStockChartSetting()}></LineChart>
            </div>
        );
    }

    private renderChartPortfolio = () => {
        let element: JSX.Element;

        if (this.state.stockGrouped != undefined && this.state.stockGrouped.length != 0) {
            let chartData: PieChartData[] = this.state.stockGrouped.map(a => ({ id: a.tickerName, label: a.tickerName, value: Math.floor(a.stockCurrentWealth) }));
            element = (
                <div className="h-96">
                    <PieChart data={chartData} labelPostfix="USD"></PieChart>
                </div>
            )
        }

        return element;
    }

    private calculareProfit = (actualPrice: number, buyPrice: number) => {
        if (buyPrice <= 0 || actualPrice <= 0)
            return 0;

        let profitOrLoss = ((actualPrice - buyPrice) / buyPrice) * 100;

        return profitOrLoss;
    }

    private showCompanyProfile = async (companyTicker: string) => {
        const tickerModel = _.first(this.tickers.filter(t => t.ticker == companyTicker));
        let loaded: boolean = true;
        
        if (tickerModel.metadata == null)
            loaded = false;

        const metadata = tickerModel.metadata
        const companyProfile = await this.stockService.getCompanyProfile(companyTicker);
        const last5YearDate = moment(new Date()).subtract(5, "y").toDate();
        const companyPrice = await this.stockService.getStockPriceHistory(companyTicker, last5YearDate);
        const companyTrades = await this.stockService.getStockTradeHistoryByTicker(companyTicker);
        const tradesViewModel = companyTrades.map(c => StockViewModel.mapFromDataModel(c));
        let complexModel: StockComplexModel = { ticker: companyTicker, companyInfo: companyProfile, company5YPrice: companyPrice, trades: tradesViewModel };

        if (companyProfile != undefined)
            this.setState({ selectedCompany: complexModel });
    }

    private renderTickerFinInfo = (ticker: StockGroupModel) => {
        const profitOrLoss = this.calculareProfit(ticker.stockCurrentWealth, ticker.stockSpentPrice);

        return (
            <div key={ticker.tickerId} className="w-3/12 bg-battleshipGrey border-2 border-vermilion p-4 mx-2 mb-6 rounded-xl" onClick={_ => this.showCompanyProfile(ticker.tickerName)}>
                <div className="grid grid-cols-3 mb-2">
                    <div className="flex flex-row col-span-2">
                        {(profitOrLoss >= 0 ? <ArrowDropUpIcon className="fill-green-700 h-10 w-10" /> : <ArrowDropDownIcon className="fill-red-700 h-10 w-10" />)}
                        <div className="flex flex-col text-left">
                            <p className={"text-xl font-bold text-left mt-1"}>{ticker.tickerName}</p>
                            {ticker.stockCurrentWealth != 0 ? (<p className="text-2xl font-extrabold">{profitOrLoss.toFixed(2)} %</p>) : <></>}
                        </div>
                    </div>
                    <div className="text-right">
                        <p className="text-lg">{ticker.size.toFixed(3)}</p>
                        <p className="text-lg">{Math.abs(ticker.stockCurrentWealth).toFixed(2)} $</p>
                    </div>
                </div>
                {this.renderChart(ticker.tickerName)}
            </div>
        );
    }

    private handleCloseCompanyProfile = () =>
        this.setState({ selectedCompany: undefined });

    private handleClosetickerRequest = () =>
        this.setState({ isOpenedTickerRequest: false });

    private handleCloseFileUpload = () =>
        this.setState({ isFileUploadOpened: false });

    private handleDisplayChoice = (_: any, displayChoice: DisplayChioce) => {
        if (displayChoice)
            this.setState({ selectedDisplayChoice: displayChoice });
    }

    private uploadBrokerReport = async (e: React.ChangeEvent<HTMLInputElement>) => {
        if (!e.target.files)
            return;

        const appContext: AppContext = this.context as AppContext;
        try {
            const files: Blob = (e.target as HTMLInputElement).files?.[0];
            await this.stockApi.stockBrokerReportBrokerIdPost({ brokerId: this.state.selectedBroker, file: files });
            appContext.setSnackbarMessage({ message: "Broker report was uploaded to be processed", severity: SnackbarSeverity.success });
        } catch (error) {
            appContext.setSnackbarMessage({ message: "Error while uploading", severity: SnackbarSeverity.error });
        }
    }

    private newTickerRequest = () => {
        this.setState({ isOpenedTickerRequest: true });
    }

    private sendTickerRequest = async (ticker: string) => {
        await this.stockApi.stockTickerRequestPost({ tickerRequest: { ticker: ticker } });
        this.setState({ isOpenedTickerRequest: false });
        const appContext: AppContext = this.context as AppContext;
        appContext.setSnackbarMessage({ message: "Ticker request has been queued.", severity: SnackbarSeverity.success })
    }

    render() {
        const yValues: number[] = this.state?.lineChartData?.dataSets[0]?.data?.map(a => a.y) ?? [];
        const minStockValue = Math.min(...yValues);
        const maxStockValue = Math.max(...yValues);

        return (
            <MainFrame header='Stocks'>
                <>
                    <div className="flex flex-row pt-5">
                        <ComponentPanel classStyle="w-7/12">
                            <div className="flex flex-col h-full">
                                <div className="flex flex-row">
                                    <div className="w-1/2 text-left">
                                        <ToggleButtonGroup className="ml-auto" value={this.state.selectedDisplayChoice} onChange={this.handleDisplayChoice} aria-label="text formatting" size="small" exclusive>
                                            <ToggleButton value={DisplayChioce.Portfolio} aria-label="Portfolio">
                                                <span className={this.state.selectedDisplayChoice == DisplayChioce.Portfolio ? "text-vermilion" : "text-white"}>Portfolio</span>
                                            </ToggleButton>
                                            <ToggleButton value={DisplayChioce.Trades} aria-label="Trades">
                                                <span className={this.state.selectedDisplayChoice == DisplayChioce.Trades ? "text-vermilion" : "text-white"}>Trades</span>
                                            </ToggleButton>
                                        </ToggleButtonGroup>
                                    </div>
                                    <div className="w-1/2">
                                        <Button component="label" variant="outlined" color="primary" className="block ml-auto bg-vermilion text-white mb-3 w-2/3"
                                            onClick={() => this.setState({ isFileUploadOpened: true })}>
                                            Upload crypto report
                                        </Button>
                                    </div>
                                </div>
                                {this.state.selectedDisplayChoice == DisplayChioce.Portfolio ? (
                                    <div className="flex flex-col">
                                        <h2 className="text-xl font-semibold">Current portfolio</h2>
                                        <div className="text-right mb-6">
                                            <Button className='bg-vermilion text-mainDarkBlue text-xs' onClick={this.newTickerRequest}>
                                                <span className="w-4">
                                                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" className="fill-current cursor-pointer">
                                                        <path d="M0 0h24v24H0z" fill="none" />
                                                        <path d="M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-2 10h-4v4h-2v-4H7v-2h4V7h2v4h4v2z" />
                                                    </svg>
                                                </span>
                                                <span>New ticker request</span>
                                            </Button>
                                        </div>
                                        <div className="flex flex-wrap justify-around ">
                                            {this.state.stockGrouped.map(g => this.renderTickerFinInfo(g))}
                                        </div>
                                    </div>
                                ) : (
                                    <div className="flex flex-col">
                                        <h2 className="text-xl font-semibold mb-6">All trades</h2>
                                        <div className="overflow-y-scroll">
                                            <BaseList<StockViewModel> data={this.state.stocks} template={this.renderTemplate} header={this.renderHeader()} dataAreaClass="h-70vh overflow-y-auto"
                                                addItemHandler={this.addStockTrade} itemClickHandler={this.editStock} useRowBorderColor={true} deleteItemHandler={this.deleteTrade} ></BaseList>
                                        </div>
                                    </div>
                                )}
                            </div>
                        </ComponentPanel>
                        <div className="flex flex-col w-5/12">
                            <ComponentPanel>
                                <>
                                    <h2 className="text-xl font-semibold mb-6">Stock summary</h2>
                                    {this.state.stockSummary == undefined ? <Loading className="m-auto mt-4" /> : (
                                        <div className='flex flex-col text-2xl text-white font-semibold text-left px-4 justify-evenly'>
                                            <p className="">Total value: {this.state.stockSummary.totalWealth.toFixed(0)}$</p>
                                            <p className="mt-2">Total bought: {this.state.stockSummary.totalyBought.toFixed(0)}$</p>
                                            <p className="mt-2">Total sold: {this.state.stockSummary.totalySold.toFixed(0)}$</p>
                                        </div>
                                    )}
                                </>
                            </ComponentPanel>
                            <ComponentPanel>
                                <>
                                    <h2 className="text-xl font-semibold mb-6">Stock portfolio</h2>
                                    <div>
                                        {this.renderChartPortfolio()}
                                    </div>
                                </>
                            </ComponentPanel>
                            <ComponentPanel>
                                <>
                                    <h2 className="text-xl font-semibold mb-6">Stock value history</h2>
                                    <div className="h-64">
                                        <LineChart dataSets={this.state.lineChartData.dataSets} chartProps={LineChartSettingManager.getStockChartSettingForStockValueHistory(minStockValue, maxStockValue)}></LineChart>
                                    </div>
                                </>
                            </ComponentPanel>
                        </div>
                    </div>
                    <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Stock form"
                        maxWidth="md" fullWidth={true}>
                        <DialogTitle id="form-dialog-title" className="bg-prussianBlue">Investment form</DialogTitle>
                        <DialogContent className="bg-prussianBlue">
                            <StockTradeForm stockTradeViewModel={this.state.selectedModel} currencies={this.currencies} stockTickers={this.tickers} onSave={this.saveStockTrade} />
                        </DialogContent>
                    </Dialog>
                    <Dialog open={this.state.selectedCompany != undefined} onClose={this.handleCloseCompanyProfile} aria-labelledby="" maxWidth="lg" fullWidth={true}>
                        <DialogContent className="bg-prussianBlue">
                            <CompanyProfile companyProfile={this.state.selectedCompany}></CompanyProfile>
                        </DialogContent>
                    </Dialog>
                    <Dialog open={this.state.isOpenedTickerRequest} onClose={this.handleClosetickerRequest} aria-labelledby="" maxWidth="lg" fullWidth={true}>
                        <DialogContent className="bg-prussianBlue">
                            <NewTickerForm onSave={this.sendTickerRequest} />
                        </DialogContent>
                    </Dialog>
                    <Dialog open={this.state.isFileUploadOpened} onClose={this.handleCloseFileUpload} aria-labelledby="" maxWidth="sm" fullWidth={true}>
                        <DialogContent className="bg-prussianBlue">
                            <BrokerUpload onUploadBrokerReport={this.uploadBrokerReport} stockBrokerParsers={this.state.stockBrokerParsers}
                                onBrokerSelect={(e) => this.setState({ selectedBroker: e })} selectedBroker={this.state.selectedBroker} />
                        </DialogContent>
                    </Dialog>
                </>
            </MainFrame>
        );
    }
}

StockOverview.contextType = AppCtx;

export default StockOverview;

export class BrokerUploadProps {
    onUploadBrokerReport: (e: React.ChangeEvent<HTMLInputElement>) => Promise<void>;
    stockBrokerParsers: Map<number, string>;
    onBrokerSelect: (e: any) => void;
    selectedBroker: number;
}

export const BrokerUpload = (props: BrokerUploadProps) => {
    return (
        <div className="flex flex-col">
            <Select
                labelId="demo-simple-select-label"
                id="demo-simple-select"
                size="small"
                value={props.selectedBroker}
                onChange={e => props.onBrokerSelect(e.target.value)}
                className="w-full lg:w-2/3 mx-auto">
                {Array.from(props.stockBrokerParsers.entries()).map(([key, value]) => (
                    <MenuItem key={key} value={key}>{value}</MenuItem>
                ))}
            </Select >
            <Button
                component="label"
                variant="outlined"
                color="primary"
                className="block ml-auto bg-vermilion text-white mb-3 mt-4 w-full lg:w-2/3 mx-auto">
                <div className="flex flex-row justify-center">
                    <PublishIcon />
                    <span className="ml-4">Upload crypto report</span>
                </div>
                <input type="file" accept=".csv" hidden onChange={props.onUploadBrokerReport} />
            </Button>
        </div>
    );
}