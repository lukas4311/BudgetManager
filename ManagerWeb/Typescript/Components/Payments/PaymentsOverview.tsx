﻿import * as React from 'react'
import moment from 'moment';
import PaymentForm from './PaymentForm'
import { IconsData } from '../../Enums/IconsEnum';
import { LineChart } from '../Charts/LineChart';
import { LineChartProps } from '../../Model/LineChartProps';
import { CalendarChartProps } from '../../Model/CalendarChartProps';
import { CalendarChart } from '../Charts/CalendarChart';
import { RadarChartProps } from '../../Model/RadarChartProps';
import { RadarChart } from '../Charts/RadarChart';
import { ChartDataProcessor } from '../../Services/ChartDataProcessor';
import DateRangeComponent from '../../Utils/DateRangeComponent';
import BudgetComponent from '../Budget/BudgetComponent';
import { Select, MenuItem, Dialog, DialogTitle, DialogContent } from '@mui/material';
import { createMuiTheme, ThemeProvider } from "@mui/material/styles";
import { BaseList } from '../BaseList';
import ApiClientFactory from '../../Utils/ApiClientFactory'
import { BankAccountApi, BankAccountApiInterface, BankAccountModel, BankBalanceModel, PaymentApi, PaymentCategoryModel, PaymentModel } from '../../ApiClient/Main';
import { RouteComponentProps } from 'react-router-dom';
import { LineChartSettingManager } from '../Charts/LineChartSettingManager';
import _ from 'lodash';
import { BarChart, BarData } from '../Charts/BarChart';
import { BarChartSettingManager } from '../Charts/BarChartSettingManager';
import { ComponentPanel } from '../../Utils/ComponentPanel';
import { MainFrame } from '../MainFrame';
import PaymentService, { MonthlyGroupedPayments } from '../../Services/PaymentService';
import ScoreList from '../../Utils/ScoreList';
import { LineChartDataSets } from '../../Model/LineChartDataSets';
import { LineChartData } from '../../Model/LineChartData';
import { PieChart, PieChartData, PieChartProps } from '../Charts/PieChart';

interface PaymentsOverviewState {
    payments: PaymentModel[];
    selectedFilter: DateFilter;
    filterDateFrom: string;
    filterDateTo: string;
    showPaymentFormModal: boolean;
    bankAccounts: Array<BankAccountModel>;
    selectedBankAccount?: number;
    showBankAccountError: boolean;
    paymentId: number;
    formKey: number;
    apiError: string;
    expenseChartData: LineChartProps;
    balanceChartData: LineChartProps;
    calendarChartData: CalendarChartProps;
    barChartData: BarData[];
    monthlyGrouped: any[];
    averageMonthExpense: number;
    averageMonthRevenue: number;
    averageMonthInvestments: number;
    topPayments: PaymentModel[];
}

interface DateFilter {
    caption: string;
    days: number;
    key: number;
}

const defaultSelectedBankAccount = -1;

export default class PaymentsOverview extends React.Component<RouteComponentProps, PaymentsOverviewState> {
    private defaultBankOption: string = "All";
    private filters: DateFilter[];
    private apiErrorMessage: string = "An error occurred while retrieving the data.";
    private chartDataProcessor: ChartDataProcessor;
    private bankAccountApi: BankAccountApiInterface;
    private paymentService: PaymentService;
    private categories: PaymentCategoryModel[];

    constructor(props: RouteComponentProps) {
        super(props);
        moment.locale('cs');
        this.filters = [{ caption: "7d", days: 7, key: 1 }, { caption: "1m", days: 30, key: 2 }, { caption: "3m", days: 90, key: 3 }];
        const bankAccounts: BankAccountModel[] = [];
        bankAccounts.unshift({ code: this.defaultBankOption, id: -1, openingBalance: 0 });

        this.state = {
            payments: [], selectedFilter: undefined, showPaymentFormModal: false, bankAccounts: bankAccounts, selectedBankAccount: -1,
            showBankAccountError: false, paymentId: null, formKey: Date.now(), apiError: undefined,
            expenseChartData: { dataSets: [] }, balanceChartData: { dataSets: [] }, calendarChartData: { dataSets: [], fromYear: new Date().getFullYear() - 1, toYear: new Date().getFullYear() }
            , filterDateTo: '', filterDateFrom: '', barChartData: [], averageMonthExpense: 0, averageMonthRevenue: 0, averageMonthInvestments: 0, topPayments: [], monthlyGrouped: []
        };

        this.chartDataProcessor = new ChartDataProcessor();
    }

    public async componentDidMount() {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.bankAccountApi = await apiFactory.getClient(BankAccountApi);
        const paymentApi = await apiFactory.getClient(PaymentApi);
        this.paymentService = new PaymentService(paymentApi);
        this.setState({ selectedFilter: this.filters[0] });
        let bankAccounts: BankAccountModel[] = [];
        bankAccounts = await this.bankAccountApi.v1BankAccountsAllGet();

        bankAccounts.unshift({ code: this.defaultBankOption, id: -1, openingBalance: 0 });
        this.categories = await this.paymentService.getPaymentCategories();
        this.setState({ bankAccounts: bankAccounts, selectedBankAccount: defaultSelectedBankAccount });
        const from = moment(Date.now()).subtract(this.state.selectedFilter.days, 'days').toDate();
        const to = moment(Date.now()).toDate();
        await this.getPaymentData(moment(Date.now()).subtract(this.state.selectedFilter.days, 'days').toDate(), moment(Date.now()).toDate(), null);
        const groupedPayments = await this.paymentService.getPaymentsSumGroupedByMonth(from, to, null);
        this.setBarchChartData(groupedPayments);
    }

    private async getPaymentData(dateFrom: Date, dateTo: Date, bankAccountId: number) {
        const payments = await this.getExactDateRangeDaysPaymentData(dateFrom, dateTo, bankAccountId);
        const groupedPayments = await this.paymentService.getPaymentsSumGroupedByMonth(dateFrom, dateTo, null);
        this.setBarchChartData(groupedPayments);
        this.setPayments(payments);
    }

    private getExactDateRangeDaysPaymentData = async (dateFrom: Date, dateTo: Date, bankAccountId: number): Promise<PaymentModel[]> =>
        await this.paymentService.getExactDateRangeDaysPaymentData(dateFrom, dateTo, bankAccountId);


    private setPayments = async (payments: Array<PaymentModel>) => {
        if (payments != undefined) {
            const expenses = this.chartDataProcessor.prepareExpenseChartData(payments);
            const expensesWithoutInvestments = this.chartDataProcessor.prepareExpenseWithoutInvestmentsChartData(payments);
            const revenueChartData = this.chartDataProcessor.prepareRevenuesChartData(payments);
            const chartData = this.chartDataProcessor.prepareCalendarCharData(payments);
            const pieData: PieChartData[] = this.chartDataProcessor.prepareDataForPieChart(payments);
            let dateTo: string;

            if (this.state.selectedFilter != undefined)
                dateTo = (moment(Date.now()).subtract(this.state.selectedFilter.days, 'days').format("YYYY-MM-DD"));
            else
                dateTo = this.state.filterDateTo;

            const fromLastOrderder = _.orderBy(payments, a => a.date, "desc");
            let bankAccountBalanceResponse: BankBalanceModel[] = await this.bankAccountApi.v1BankAccountsAllBalanceToDateGet({ toDate: moment((dateTo)).toDate() });
            const balance = await this.chartDataProcessor.prepareBalanceChartData(payments, bankAccountBalanceResponse, this.state.selectedBankAccount);
            const barChartData = pieData.map(d => ({ key: d.id, value: d.value }));
            const averageMonthExpense = this.paymentService.getAverageMonthExpense(payments);
            const averageMonthRevenue = this.paymentService.getAverageMonthRevenues(payments);
            const averageMonthInvestments = this.paymentService.getAverageMonthInvestment(payments);
            const topPayments = this.paymentService.getTopPaymentsByAmount(payments, 5, "Expense");

            this.setState({
                payments: fromLastOrderder, expenseChartData: {
                    dataSets: [
                        { id: 'Expense', data: expenses },
                        { id: "Expense wihtou investment", data: expensesWithoutInvestments },
                        { id: "Revenue", data: revenueChartData }
                    ]
                }, topPayments, balanceChartData: { dataSets: [{ id: 'Balance', data: balance }] },
                calendarChartData: { dataSets: chartData, fromYear: new Date().getFullYear() - 1, toYear: new Date().getFullYear() },
                barChartData, averageMonthExpense: averageMonthExpense, averageMonthRevenue: averageMonthRevenue, averageMonthInvestments: averageMonthInvestments,
            });
        } else {
            this.setState({ apiError: this.apiErrorMessage });
        }
    }

    private setBarchChartData(monthlyGroupedPayments: MonthlyGroupedPayments[]) {
        const monthlyGrouped = monthlyGroupedPayments.map(d => ({ key: d.dateGroup, positive: d.amountSum >= 0 ? d.amountSum : 0, negative: d.amountSum < 0 ? d.amountSum : 0 }));
        this.setState({ monthlyGrouped });
    }

    private filterClick = (filterKey: number) => {
        let selectedFilter = this.filters.find(f => f.key == filterKey);

        if (this.state.selectedFilter != selectedFilter) {
            this.setState({ selectedFilter: selectedFilter }, () => this.getFilteredPaymentData(this.state.selectedBankAccount));
        }
    }

    private addNewPayment = () => {
        if (this.state.selectedBankAccount != defaultSelectedBankAccount) {
            this.setState({ showPaymentFormModal: true, showBankAccountError: false, paymentId: null, formKey: Date.now() });
        }
        else {
            this.setState({ showBankAccountError: true });
        }
    }

    private paymentEdit = (id: number): void => {
        this.setState({ paymentId: id, showPaymentFormModal: true, formKey: Date.now() });
    }

    private hideModal = () => this.setState({ showPaymentFormModal: false, paymentId: null, formKey: Date.now() });

    private handleConfirmationClose = () => {
        this.hideModal();
        this.getFilteredPaymentData(this.state.selectedBankAccount);
    }

    private bankAccountChange = (e: any) => {
        let selectedbankId: number = parseInt(e.target.value.toString());
        this.setState({ selectedBankAccount: (isNaN(selectedbankId) ? 0 : selectedbankId) });
        this.getFilteredPaymentData(selectedbankId);
    }

    private getFilteredPaymentData(bankId: number) {
        if (bankId == -1)
            bankId = null;

        if (this.state.selectedFilter != undefined)
            this.getPaymentData(moment(Date.now()).subtract(this.state.selectedFilter.days, 'days').toDate(), moment(Date.now()).toDate(), bankId);
        else
            this.getPaymentData(moment(this.state.filterDateFrom).toDate(), moment(this.state.filterDateTo).toDate(), bankId);
    }

    private showErrorMessage() {
        let tag: JSX.Element = <React.Fragment></React.Fragment>;
        if (this.state.apiError != undefined)
            tag = <span className="errorMessage inline-block px-6 py-2 mt-2 bg-red-700 rounded-full w-2/3">{this.state.apiError}</span>

        return tag;
    }

    private getPaymentColor(paymentTypeCode: string): string {
        switch (paymentTypeCode) {
            case "Revenue":
                return "bg-green-800";
            case "Expense":
                return "bg-red-600";
            case "Transfer":
                return "bg-blue-500";
        }
    }

    private rangeDatesHandler = (dateFrom: string, dateTo: string): void => {
        this.setState({ selectedFilter: undefined, filterDateTo: dateTo, filterDateFrom: dateFrom }, () => this.getFilteredPaymentData(this.state.selectedBankAccount));
    }

    private clonePayment = (e: React.MouseEvent<HTMLSpanElement, MouseEvent>, id: number) => {
        console.log("clone: " + id);
        e.preventDefault();
        e.stopPropagation();
        this.paymentService.clonePayment(id);
    }

    private renderTemplate = (p: PaymentModel): JSX.Element => {
        let iconsData: IconsData = new IconsData();

        return (
            <>
                <span className={"min-h-full w-4 inline-block " + this.getPaymentColor(p.paymentTypeCode)}></span>
                <p className="mx-6 my-1 w-2/12">{p.amount},-</p>
                <p className="mx-6 my-1 w-2/12 truncate">{p.name}</p>
                <p className="mx-6 my-1 w-3/12">{moment(p.date).format('DD.MM.YYYY')}</p>
                <span className="mx-6 my-1 w-1/12 categoryIcon fill-white">{iconsData[p.paymentCategoryIcon]}</span>
                <span className="ml-auto my-1 w-2/12 categoryIcon fill-white" onClick={e => this.clonePayment(e, p.id)}>{iconsData.copy}</span>
            </>
        );
    }

    private getExpensesMaxValue = () => {
        let maxValue = 0;
        let sourceData: LineChartData[] = [];
        const data = this.state.expenseChartData?.dataSets;

        for (let dataSet of data)
            sourceData = sourceData.concat(...dataSet.data);

        if (!sourceData || sourceData.length == 0)
            return maxValue;

        maxValue = _.maxBy(sourceData, o => o.y).y;
        return maxValue;
    }

    private deletePayment = (id: number) => {
        this.paymentService.deletePayment(id);
    }

    public render() {
        let expenses = _.sumBy(this.state.payments.filter(a => a.paymentTypeCode == 'Expense'), e => e.amount);
        let income = _.sumBy(this.state.payments.filter(a => a.paymentTypeCode == 'Revenue'), e => e.amount);
        let saved = income - expenses;
        let savedPct = income == 0 ? 0 : (saved / income) * 100;
        // TODO: fix problem with name instead of code
        let categoryInvested = this.categories?.filter(a => a.name == "Invetsment")[0];
        let invested = 0;
        let investedPct = 0;

        if (categoryInvested) {
            invested = _.sumBy(this.state.payments.filter(p => p.paymentCategoryId == categoryInvested.id), s => s.amount);
            investedPct = income == 0 ? 0 : (invested / income) * 100;
        }

        let minGroupedPayment = 0;
        let maxGroupedPayment = 0;

        if (this.state.monthlyGrouped.length > 0) {
            minGroupedPayment = _.minBy(this.state.monthlyGrouped, o => o.negative)?.negative ?? 0;
            maxGroupedPayment = _.maxBy(this.state.monthlyGrouped, o => o.positive)?.positive ?? 0;
        }

        return (
            <React.Fragment>
                <div className="">
                    <MainFrame header='Payments overview'>
                        <React.Fragment>
                            {this.showErrorMessage()}
                            <div className="flex flex-col lg:flex-row lg:flex-wrap 2xl:flex-nowrap w-full">
                                <div className="xl:w-full 3xl:w-1/2">
                                    <ComponentPanel classStyle="">
                                        <>
                                            <div className="py-4 flex">
                                                <h2 className="text-xl ml-12">Income/expense</h2>
                                                <span className="inline-block ml-auto mr-5" onClick={this.addNewPayment}>
                                                    <svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 0 24 24" width="24" className="fill-current text-white hover:text-vermilion transition ease-out duration-700 cursor-pointer">
                                                        <path d="M0 0h24v24H0z" fill="none" />
                                                        <path d="M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-2 10h-4v4h-2v-4H7v-2h4V7h2v4h4v2z" />
                                                    </svg>
                                                </span>
                                            </div>
                                            <div className='flex flex-col lg:flex-row 3xl:flex-col'>
                                                <div className='w-full lg:w-4/10 3xl:w-full'>
                                                    <div className="flex flex-col lg:flex-row items-center mb-3 ml-6">
                                                        <Select
                                                            labelId="demo-simple-select-label"
                                                            id="demo-simple-select"
                                                            size="small"
                                                            value={this.state.selectedBankAccount}
                                                            onChange={this.bankAccountChange}
                                                            className="w-full lg:w-1/3">
                                                            {this.state.bankAccounts.map((b, i) => {
                                                                return <MenuItem key={i} value={b.id}>{b.code}</MenuItem>
                                                            })}
                                                        </Select>
                                                        <span className={"text-sm text-left transition-all ease-in-out duration-700 text-rufous h-auto overflow-hidden ml-3" + (this.state.showBankAccountError ? ' opacity-100 scale-y-100' : ' scale-y-0 opacity-0')}>Please select bank account</span>
                                                    </div>
                                                    <div className="flex flex-col lg:flex-row text-black mb-3 ml-6 mt-6 cursor-pointer">
                                                        <div className="text-left m-auto w-full lg:w-2/5 mb-4 lg:mb-0">
                                                            {this.filters.map((f) =>
                                                                <span key={f.key}
                                                                    className={"px-4 bg-white inline-flex items-center transition inline-block duration-700 hover:bg-vermilion text-sm h-8 "
                                                                        + (f.key == this.state.selectedFilter?.key ? "bg-vermilion" : "")}
                                                                    onClick={() => this.filterClick(f.key)}>
                                                                    {f.caption}
                                                                </span>
                                                            )}
                                                        </div>
                                                        <DateRangeComponent datesFilledHandler={this.rangeDatesHandler}></DateRangeComponent>
                                                    </div>
                                                </div>
                                                <div className="pb-10 h-64 overflow-y-scroll pr-4 lg:ml-4 lg:w-6/10 3xl:w-full">
                                                    <BaseList<PaymentModel> data={this.state.payments} template={this.renderTemplate} itemClickHandler={this.paymentEdit} narrowIcons={true} deleteItemHandler={this.deletePayment}></BaseList>
                                                </div>
                                            </div>
                                        </>
                                    </ComponentPanel>
                                </div>
                                <div className="xl:w-full 3xl:w-1/2 flex lg:flex-row flex-col">
                                    <ComponentPanel classStyle="xl:w-full lg:w-1/2">
                                        <div className='flex flex-col text-2xl text-white text-left px-4 justify-evenly h-full'>
                                            <div className='my-3'>
                                                <p>Totaly earned: {income}</p>
                                            </div>
                                            <div className='my-3'>
                                                <p>Totaly spent: {expenses}</p>
                                                <p className='text-xs'>Totaly spent without investments: {expenses - invested}</p>
                                            </div>
                                            <div className='my-3'>
                                                <p>Totaly saved: {saved} ({savedPct?.toFixed(1)}%)</p>
                                                <p className='text-xs'>Totaly saved including investments: {saved + invested}</p>
                                            </div>
                                            <div className='my-3'>
                                                <p>Totaly invested: {invested} ({investedPct?.toFixed(1)}%)</p>
                                            </div>
                                        </div>
                                    </ComponentPanel>
                                    <ComponentPanel classStyle="xl:w-full lg:w-1/2">
                                        <div className='flex flex-col text-2xl text-white text-left px-4 justify-evenly h-full'>
                                            <div className='my-3'>
                                                <p>Month average expenses: {this.state.averageMonthExpense.toFixed(0)}</p>
                                                <p className='text-xs'>Month average expenses without investments: {(this.state.averageMonthExpense - this.state.averageMonthInvestments).toFixed(0)}</p>
                                            </div>
                                            <div className='my-3'>
                                                <p>Month average revenue: {this.state.averageMonthRevenue.toFixed(0)}</p>
                                            </div>
                                            <div className='my-3'>
                                                <p>Month average investments: {this.state.averageMonthInvestments.toFixed(0)}</p>
                                            </div>
                                        </div>
                                    </ComponentPanel>
                                </div>
                            </div>
                            <div className="flex flex-col lg:flex-row">
                                <ComponentPanel classStyle="lg:w-1/2 h-80">
                                    <LineChart dataSets={this.state.expenseChartData.dataSets} chartProps={LineChartSettingManager.getPaymentChartSettingWithScale(0, 0, this.getExpensesMaxValue(), 25)}></LineChart>
                                </ComponentPanel>
                                <ComponentPanel classStyle="lg:w-1/2 h-80">
                                    <BarChart dataSets={this.state.barChartData} chartProps={BarChartSettingManager.getPaymentCategoryBarChartProps()}></BarChart>
                                </ComponentPanel>
                            </div>
                            <div className="flex flex-col lg:flex-row">
                                <ComponentPanel classStyle="lg:w-1/2 h-80 ">
                                    <CalendarChart dataSets={this.state.calendarChartData.dataSets} fromYear={new Date().getFullYear() - 1} toYear={new Date().getFullYear()}></CalendarChart>
                                </ComponentPanel>
                                <ComponentPanel classStyle="lg:w-1/2 h-80">
                                    <BarChart dataSets={this.state.monthlyGrouped} chartProps={BarChartSettingManager.getPaymentMonthlyGroupedBarChartProps(minGroupedPayment, maxGroupedPayment)}></BarChart>
                                </ComponentPanel>
                            </div>
                            <div className="flex flex-col lg:flex-row">
                                <ComponentPanel classStyle="lg:w-1/2 h-80">
                                    <ScoreList models={this.state.topPayments.map(m => ({ score: m.amount, title: m.name }))} />
                                </ComponentPanel>

                            </div>
                            <Dialog open={this.state.showPaymentFormModal} onClose={this.hideModal} aria-labelledby="Payment_detail"
                                maxWidth="md" fullWidth={true}>
                                <DialogTitle id="form-dialog-title" className="bg-prussianBlue">Payment detail</DialogTitle>
                                <DialogContent className="bg-prussianBlue">
                                    <PaymentForm key={this.state.formKey} paymentId={this.state.paymentId} bankAccountId={this.state.selectedBankAccount}
                                        handleClose={this.handleConfirmationClose} history={this.props.history} />
                                </DialogContent>
                            </Dialog>
                        </React.Fragment>
                    </MainFrame>
                </div >
            </React.Fragment>
        )
    }
}