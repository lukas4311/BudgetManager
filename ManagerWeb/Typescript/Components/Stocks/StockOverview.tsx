import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { MainFrame } from "../MainFrame";
import { BaseList } from "../BaseList";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { CryptoApi, CurrencyApi, EnumApi, StockApi } from "../../ApiClient/Main/apis";
import { CompanyProfileModel, CurrencySymbol, StockPrice, StockTickerModel } from "../../ApiClient/Main/models";
import moment from "moment";
import _ from "lodash";
import { StockViewModel } from "../../Model/StockViewModel";
import { StockTradeForm } from "./StockTradeForm";
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
import { PieChart, PieChartData } from "../Charts/PieChart";
import { IStockService } from "../../Services/IStockService";
import { LineChartProps } from "../../Model/LineChartProps";
import { ToggleButtonGroup, ToggleButton, Button, Dialog, DialogTitle, DialogContent, TextField, Select, MenuItem } from "@mui/material";
import { SnackbarSeverity } from "../../App";
import { NewTickerForm } from "./NewTickerForm";
import { FixTickerForm } from "./FixTickerForm";
import { BrokerUpload } from "./BrokerUpload";
import { TickerCard } from "./TickerCard";
import StyleConstants from "../../Utils/StyleConstants";

const tickerMetadataAttribute = "price_ticker";

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
    isOpenedTickerFix: boolean;
    selectedFixTicker: FixTickerParams;
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
            selectedDisplayChoice: DisplayChioce.Portfolio, isOpenedTickerRequest: false, stockBrokerParsers: new Map<number, string>(), selectedBroker: -1, isFileUploadOpened: false, isOpenedTickerFix: false, selectedFixTicker: undefined
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
        const stockSummaryBuy = _.sumBy(stockGrouped, s => s.stockSpentPrice);
        const stockSummarySell = _.sumBy(stockGrouped, s => s.stockSellPrice);
        const stockSummaryWealth = _.sumBy(stockGrouped, s => s.stockCurrentWealth);
        const tickersAdjustedWithPriceTicker = stockGrouped.map(a => this.tryToGetPriceTicker(a.tickerName));

        const tickerPrices = await this.stockService.getLastMonthTickersPrice(tickersAdjustedWithPriceTicker);
        const lineChartData = await this.prepareStockDataToLineChart();
        stockGrouped = _.orderBy(stockGrouped, a => a.stockCurrentWealth, 'desc');

        this.setState({ stocks, stockGrouped, stockSummary: { totalyBought: stockSummaryBuy, totalySold: stockSummarySell, totalWealth: stockSummaryWealth }, stockPrice: tickerPrices, lineChartData: { dataSets: lineChartData } });
    }

    private tryToGetPriceTicker(ticker: string) {
        const metadata = _.first(this.tickers.filter(t => t.ticker == ticker))?.metadata;

        if (!metadata)
            return ticker;

        const metadataObject = JSON.parse(metadata);
        const priceTicker = metadataObject[tickerMetadataAttribute];
        return priceTicker ?? ticker;
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

    private showCompanyProfile = async (companyTicker: string) => {
        const tickerModel = _.first(this.tickers.filter(t => t.ticker == companyTicker));
        let loaded: boolean = true;

        if (tickerModel.metadata == null)
            loaded = false;

        const companyProfile = await this.stockService.getCompanyProfile(companyTicker);
        const last5YearDate = moment(new Date()).subtract(5, "y").toDate();
        const companyPrice = await this.stockService.getStockPriceHistory(companyTicker, last5YearDate);
        const companyTrades = await this.stockService.getStockTradeHistoryByTicker(companyTicker);
        const tradesViewModel = companyTrades.map(c => StockViewModel.mapFromDataModel(c));
        let complexModel: StockComplexModel = { ticker: companyTicker, companyInfo: companyProfile, company5YPrice: companyPrice, trades: tradesViewModel };

        if (companyProfile != undefined)
            this.setState({ selectedCompany: complexModel });
    }

    private getTickerWarnings = (ticker: StockGroupModel): [boolean, boolean] => {
        const hasMetadata = _.first(this.tickers.filter(t => t.id == ticker.tickerId))?.metadata != undefined;
        const hasPrice = _.first(this.state.stockPrice.filter(t => t.ticker == ticker.tickerName))?.price.length != 0;
        return [hasMetadata, hasPrice];
    }

    private onWarningClick = (e: React.MouseEvent<SVGSVGElement, MouseEvent>, ticker: StockGroupModel): void => {
        e.preventDefault();
        e.stopPropagation();
        const [hasMetadata, hasPrice] = this.getTickerWarnings(ticker);
        this.setState({ selectedFixTicker: { hasMetadata, hasPrice, tickerId: ticker.tickerId }, isOpenedTickerFix: true });
    }

    private onFixTickerSave = async (priceTicker: string, metadataTicker: string) => {
        const fixTicker = this.state.selectedFixTicker;
        const ticker = _.first(this.tickers.filter(t => t.id == fixTicker.tickerId));

        if (!fixTicker.hasMetadata)
            await this.stockApi.stockStockTickerTickerIdPut({ tickerId: fixTicker.tickerId, stockTickerModel: { id: ticker.id, name: ticker.name, metadata: ticker.metadata, ticker: metadataTicker } });

        if (!fixTicker.hasPrice) {
            const metadata = ticker?.metadata;
            const metadataObj = JSON.parse(metadata);
            metadataObj[tickerMetadataAttribute] = priceTicker;
            await this.stockApi.stockStockTickerTickerIdMetadataPut({ tickerId: fixTicker.tickerId, body: JSON.stringify(metadataObj) });
        }

        this.setState({ isOpenedTickerFix: false, selectedFixTicker: undefined });
    }

    private handleCloseCompanyProfile = () =>
        this.setState({ selectedCompany: undefined });

    private handleClosetickerRequest = () =>
        this.setState({ isOpenedTickerRequest: false });

    private handleCloseTickerFix = () => {
        this.setState({ isOpenedTickerFix: false, selectedFixTicker: undefined });
    }

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
                        <ComponentPanel classStyle={"w-7/12 mr-4" + StyleConstants.componentPanelStyles}>
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
                                            Upload stock report
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
                                            {this.state.stockGrouped.map(g => <TickerCard key={g.tickerId} ticker={g} tickers={this.tickers} tickersPrice={this.state.stockPrice}
                                                onTickerCardClick={this.showCompanyProfile} onWarningClick={this.onWarningClick}></TickerCard>)}
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
                            <ComponentPanel classStyle={StyleConstants.componentPanelStyles}>
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
                            <ComponentPanel classStyle={"mt-4" + StyleConstants.componentPanelStyles}>
                                <>
                                    <h2 className="text-xl font-semibold mb-6">Stock portfolio</h2>
                                    <div>
                                        {this.renderChartPortfolio()}
                                    </div>
                                </>
                            </ComponentPanel>
                            <ComponentPanel classStyle={"mt-4" + StyleConstants.componentPanelStyles}>
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
                    <Dialog open={this.state.isOpenedTickerFix && this.state.selectedFixTicker != undefined} onClose={this.handleCloseTickerFix} aria-labelledby="" maxWidth="lg" fullWidth={true}>
                        <DialogContent className="bg-prussianBlue">
                            <FixTickerForm onSave={this.onFixTickerSave} {...this.state.selectedFixTicker} />
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

class FixTickerParams {
    hasMetadata: boolean;
    hasPrice: boolean;
    tickerId: number;
}

StockOverview.contextType = AppCtx;

export default StockOverview;