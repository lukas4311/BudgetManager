﻿import * as React from 'react'
import moment from 'moment';
import { Modal } from '../../Modal'
import PaymentForm from './PaymentForm'
import DataLoader from '../../Services/DataLoader';
import { IPaymentInfo } from "../../Model/IPaymentInfo"
import { BankAccount } from '../../Model/BankAccount';
import { BankAccountReponse } from '../../Model/BankAccountReponse';
import { IconsData } from '../../Enums/IconsEnum';
import { LineChart } from '../Charts/LineChart';
import { IBankAccountBalanceResponseModel } from '../../Model/IBankAccountBalanceResponseModel';
import { LineChartProps } from '../../Model/LineChartProps';
import { CalendarChartProps } from '../../Model/CalendarChartProps';
import { CalendarChart } from '../Charts/CalendarChart';
import { RadarChartProps } from '../../Model/RadarChartProps';
import { RadarChart } from '../Charts/RadarChart';
import { ChartDataProcessor } from '../../Services/ChartDataProcessor';
import DateRangeComponent from '../../Utils/DateRangeComponent';
import BudgetComponent from '../Budget/BudgetComponent';
import ErrorBoundary from '../../Utils/ErrorBoundry';

interface PaymentsOverviewState {
    payments: Array<IPaymentInfo>,
    selectedFilter: DateFilter,
    filterDateTo: string,
    showPaymentFormModal: boolean,
    bankAccounts: Array<BankAccount>
    selectedBankAccount?: number,
    showBankAccountError: boolean,
    paymentId: number,
    formKey: number,
    apiError: string,
    expenseChartData: LineChartProps,
    balanceChartData: LineChartProps,
    calendarChartData: CalendarChartProps,
    radarChartData: RadarChartProps
}

interface DateFilter {
    caption: string,
    days: number,
    key: number
}

export default class PaymentsOverview extends React.Component<{}, PaymentsOverviewState>{
    private defaultBankOption: string = "Vše";
    private filters: DateFilter[];
    private dataLoader: DataLoader;
    private apiErrorMessage: string = "Při získnání data došlo k chybě.";
    private chartDataProcessor: ChartDataProcessor;

    constructor(props: {}) {
        super(props);
        moment.locale('cs');
        this.filters = [{ caption: "7d", days: 7, key: 1 }, { caption: "1m", days: 30, key: 2 }, { caption: "3m", days: 90, key: 3 }];
        this.state = {
            payments: [], selectedFilter: this.filters[0], showPaymentFormModal: false, bankAccounts: [], selectedBankAccount: undefined,
            showBankAccountError: false, paymentId: null, formKey: Date.now(), apiError: undefined,
            expenseChartData: { dataSets: [] }, balanceChartData: { dataSets: [] }, calendarChartData: { dataSets: [] }, radarChartData: { dataSets: [] },
            filterDateTo: ''
        };

        this.dataLoader = new DataLoader();
        this.chartDataProcessor = new ChartDataProcessor();
    }

    private async getPaymentData(dateFrom: Date, dateTo: Date, bankAccountId: number) {
        const payments = await this.getExactDateRangeDaysPaymentData(dateFrom, dateTo, bankAccountId);
        this.setPayments(payments);
    }

    private async getExactDateRangeDaysPaymentData(dateFrom: Date, dateTo: Date, bankAccountId: number): Promise<IPaymentInfo[]> {
        let filterDate: string = moment(dateFrom).format("YYYY-MM-DD");
        return await this.dataLoader.getPayments(filterDate, moment(dateTo).format("YYYY-MM-DD"), bankAccountId, this.onRejected);
    }

    private onRejected = () => {
        this.setState({ apiError: this.apiErrorMessage });
    }

    private setPayments = async (payments: Array<IPaymentInfo>) => {
        if (payments != undefined) {
            const expenses = this.chartDataProcessor.prepareExpenseChartData(payments);
            const chartData = this.chartDataProcessor.prepareCalendarCharData(payments);
            const radarData = this.chartDataProcessor.prepareDataForRadarChart(payments);
            let dateTo: string;

            if (this.state.selectedFilter != undefined) {
                dateTo = (moment(Date.now()).subtract(this.state.selectedFilter.days, 'days').format("YYYY-MM-DD"));
            } else {
                dateTo = this.state.filterDateTo;
            }

            let bankAccountBalanceResponse: IBankAccountBalanceResponseModel = await this.dataLoader.getBankAccountsBalanceToDate(dateTo, this.onRejected)
            const balance = await this.chartDataProcessor.prepareBalanceChartData(payments, bankAccountBalanceResponse, this.state.selectedBankAccount);
            this.setState({
                payments: payments, expenseChartData: { dataSets: [{ id: 'Výdej', data: expenses }] },
                balanceChartData: { dataSets: [{ id: 'Balance', data: balance }] }, calendarChartData: { dataSets: chartData },
                radarChartData: { dataSets: radarData }
            });
        } else {
            this.setState({ apiError: this.apiErrorMessage });
        }
    }

    private filterClick = (filterKey: number) => {
        let selectedFilter = this.filters.find(f => f.key == filterKey);

        if (this.state.selectedFilter != selectedFilter) {
            this.setState({ selectedFilter: selectedFilter });
            this.getPaymentData(moment(Date.now()).subtract(selectedFilter.days, 'days').toDate(), moment(Date.now()).toDate(), this.state.selectedBankAccount);
        }
    }

    private addNewPayment = () => {
        if (this.state.selectedBankAccount != undefined) {
            this.setState({ showPaymentFormModal: true, showBankAccountError: false, paymentId: null, formKey: Date.now() });
        }
        else {
            this.setState({ showBankAccountError: true });
        }
    }

    private paymentEdit(id: number) {
        this.setState({ paymentId: id, showPaymentFormModal: true, formKey: Date.now() });
    }

    private hideModal = () => {
        this.setState({ showPaymentFormModal: false, paymentId: null, formKey: Date.now() });
    };

    private handleConfirmationClose = () => {
        this.hideModal();
        this.getPaymentData(moment(Date.now()).subtract(this.state.selectedFilter.days, 'days').toDate(), moment(Date.now()).toDate(), this.state.selectedBankAccount);
    }

    private bankAccountChange = (e: React.ChangeEvent<HTMLSelectElement>) => {
        let selectedbankId: number = parseInt(e.target.value);
        this.setState({ selectedBankAccount: (isNaN(selectedbankId) ? undefined : selectedbankId) });
        this.getPaymentData(moment(Date.now()).subtract(this.state.selectedFilter.days, 'days').toDate(), moment(Date.now()).toDate(), selectedbankId);
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

    private setBankAccounts = (data: BankAccountReponse) => {
        if (data.success) {
            let bankAccounts: Array<BankAccount> = data.bankAccounts;
            bankAccounts.unshift({ code: this.defaultBankOption, id: undefined });
            this.setState({ bankAccounts: bankAccounts });
        }
    }

    public async componentDidMount() {
        const bankAccounts: BankAccountReponse = await this.dataLoader.getBankAccounts(this.onRejected);
        this.getPaymentData(moment(Date.now()).subtract(this.state.selectedFilter.days, 'days').toDate(), moment(Date.now()).toDate(), null);
        this.setBankAccounts(bankAccounts);
    }

    private rangeDatesHandler = (dateFrom: string, dateTo: string): void => {
        this.getPaymentData(moment(dateFrom).toDate(), moment(dateTo).toDate(), this.state.selectedBankAccount);
        this.setState({ selectedFilter: undefined, filterDateTo: dateTo });
    }

    public render() {
        let iconsData: IconsData = new IconsData();

        return (
            <div className="text-center mt-6 bg-prussianBlue rounded-lg">
                {this.showErrorMessage()}
                <div className="flex flex-row">
                    <div className="w-2/5">
                        <div className="py-4 flex">
                            <h2 className="text-xl ml-12">Platby</h2>
                            <span className="inline-block ml-auto mr-5" onClick={this.addNewPayment}>
                                <svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 0 24 24" width="24" className="fill-current text-white hover:text-vermilion transition ease-out duration-700 cursor-pointer">
                                    <path d="M0 0h24v24H0z" fill="none" />
                                    <path d="M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-2 10h-4v4h-2v-4H7v-2h4V7h2v4h4v2z" />
                                </svg>
                            </span>
                        </div>
                        <div className="flex flex-col mb-3 ml-6">
                            <span className={"text-sm text-left transition-all ease-in-out duration-700 text-rufous h-auto overflow-hidden" + (this.state.showBankAccountError ? ' opacity-100 scale-y-100' : ' scale-y-0 opacity-0')}>Prosím vyberte kontkrétní účet</span>
                            <select className="effect-11 py-1 w-1/3" onChange={this.bankAccountChange} value={this.state.selectedBankAccount}>
                                {this.state.bankAccounts.map((b, i) => {
                                    return <option key={i} value={b.id}>{b.code}</option>
                                })}
                            </select>
                        </div>
                        <div className="flex flex-tow text-black mb-3 ml-6 cursor-pointer">
                            <div className="text-left mr-4">
                                {this.filters.map((f) =>
                                    <span key={f.key} className="px-4 bg-white transition duration-700 hover:bg-vermilion text-sm" onClick={() => this.filterClick(f.key)}>{f.caption}</span>
                                )}
                            </div>
                            <DateRangeComponent datesFilledHandler={this.rangeDatesHandler}></DateRangeComponent>
                        </div>
                        <div className="pb-10 h-64 overflow-y-scroll">
                            {this.state.payments.map(p =>
                                <div key={p.id} className="paymentRecord bg-battleshipGrey rounded-r-full flex mr-6 mt-1 hover:bg-vermilion cursor-pointer" onClick={(_) => this.paymentEdit(p.id)}>
                                    <span className={"min-h-full w-4 inline-block " + this.getPaymentColor(p.paymentTypeCode)}></span>
                                    <p className="mx-6 my-1 w-1/5">{p.amount},-</p>
                                    <p className="mx-6 my-1 w-2/5">{p.name}</p>
                                    <p className="mx-6 my-1 w-1/5">{moment(p.date).format('DD.MM.YYYY')}</p>
                                    <span className="mx-6 my-1 w-1/5 categoryIcon">{iconsData[p.paymentCategoryIcon]}</span>
                                </div>
                            )}
                        </div>
                    </div>
                    <div className="w-3/5 h-64 mt-auto calendar">
                        <CalendarChart dataSets={this.state.calendarChartData.dataSets}></CalendarChart>
                    </div>
                </div>
                <div className="flex flex-row">
                    <div className="w-1/3 h-64">
                        <LineChart dataSets={this.state.balanceChartData.dataSets}></LineChart>
                    </div>
                    <div className="w-1/3 h-64">
                        <LineChart dataSets={this.state.expenseChartData.dataSets}></LineChart>
                    </div>
                    <div className="w-1/3 h-64 calendar text-black">
                        <RadarChart dataSets={this.state.radarChartData.dataSets}></RadarChart>
                    </div>
                </div>
                <div className="flex flex-row p-6">
                    <div className="w-1/3">
                        <BudgetComponent></BudgetComponent>
                    </div>
                </div>
                <Modal show={this.state.showPaymentFormModal} handleClose={this.hideModal}>
                    <ErrorBoundary>
                        <PaymentForm key={this.state.formKey} paymentId={this.state.paymentId} bankAccountId={this.state.selectedBankAccount} handleClose={this.handleConfirmationClose}></PaymentForm>
                    </ErrorBoundary>
                </Modal>
            </div >
        )
    }
}