import * as React from 'react'
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
import { BaseList, EListStyle } from '../BaseList';
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

interface PaymentsOverviewStateV2 {
    payments: PaymentModel[];
    showPaymentFormModal: boolean;
    paymentId: number;
    formKey: number;
    // selectedFilter: DateFilter;
    // filterDateFrom: string;
    // filterDateTo: string;
    // bankAccounts: Array<BankAccountModel>;
    // selectedBankAccount?: number;
    // showBankAccountError: boolean;
    // apiError: string;
    // expenseChartData: LineChartProps;
    // balanceChartData: LineChartProps;
    // calendarChartData: CalendarChartProps;
    // barChartData: BarData[];
    // monthlyGrouped: any[];
    // averageMonthExpense: number;
    // averageMonthRevenue: number;
    // averageMonthInvestments: number;
    // topPayments: PaymentModel[];
}

interface DateFilter {
    caption: string;
    days: number;
    key: number;
}

const defaultSelectedBankAccount = -1;

export default class PaymentsOverview extends React.Component<RouteComponentProps, PaymentsOverviewStateV2> {
    private paymentService: PaymentService;
    private bankAccountApi: BankAccountApi;

    constructor(props: RouteComponentProps) {
        super(props);
        moment.locale('cs');

        this.state = {
            payments: [], paymentId: undefined, showPaymentFormModal: false, formKey: Date.now()
        };
    }

    public componentDidMount() {
        this.loadData();
    }

    private loadData = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.bankAccountApi = await apiFactory.getClient(BankAccountApi);
        const paymentApi = await apiFactory.getClient(PaymentApi);
        this.paymentService = new PaymentService(paymentApi);
        const payments = await this.getExactDateRangeDaysPaymentData(moment(Date.now()).subtract(1, 'months').toDate(), moment(Date.now()).toDate(), null);
        const fromLastOrderder = _.orderBy(payments, a => a.date, "desc");
        this.setState({ payments: fromLastOrderder });
    }

    private getExactDateRangeDaysPaymentData = async (dateFrom: Date, dateTo: Date, bankAccountId: number): Promise<PaymentModel[]> =>
        await this.paymentService.getExactDateRangeDaysPaymentData(dateFrom, dateTo, bankAccountId);

    private paymentEdit = (id: number): void =>
        this.setState({ paymentId: id, showPaymentFormModal: true, formKey: Date.now() });

    private addNewPayment = () => {
        this.setState({ showPaymentFormModal: true, paymentId: null, formKey: Date.now() });
    }

    private deletePayment = (id: number) => {
        this.paymentService.deletePayment(id);
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

    private clonePayment = (e: React.MouseEvent<HTMLSpanElement, MouseEvent>, id: number) => {
        console.log("clone: " + id);
        e.preventDefault();
        e.stopPropagation();
        this.paymentService.clonePayment(id);
    }

    private renderTemplate = (p: PaymentModel): JSX.Element => {
        let iconsData: IconsData = new IconsData();

        return (
            <div className="flex flex-row w-full">
                <div className="flex flex-row p-4 w-full">
                    <p className="mr-auto">{moment(p.date).format('DD.MM.YYYY')}</p>
                    <span className="ml-auto categoryIcon fill-white">{iconsData[p.paymentCategoryIcon]}</span>
                </div>

                {/* <span className={"min-h-full w-4 inline-block " + this.getPaymentColor(p.paymentTypeCode)}></span>
                <p className="mx-6 my-1 w-2/12">{p.amount},-</p>
                <p className="mx-6 my-1 w-2/12 truncate">{p.name}</p>
                <p className="mx-6 my-1 w-3/12">{moment(p.date).format('DD.MM.YYYY')}</p>
                <span className="mx-6 my-1 w-1/12 categoryIcon fill-white">{iconsData[p.paymentCategoryIcon]}</span>
                <span className="ml-auto my-1 w-2/12 categoryIcon fill-white" onClick={e => this.clonePayment(e, p.id)}>{iconsData.copy}</span> */}
            </div>
        );
    }

    public render() {
        return (
            <React.Fragment>
                <div className="">
                    <MainFrame header='Payments overview'>
                        <React.Fragment>
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
                                            <div className="pb-10 overflow-y-scroll pr-4 lg:ml-4 w-full">
                                                <BaseList<PaymentModel> data={this.state.payments} template={this.renderTemplate} itemClickHandler={this.paymentEdit}
                                                    narrowIcons={true} deleteItemHandler={this.deletePayment} style={EListStyle.CardStyle}></BaseList>
                                            </div>
                                        </>
                                    </ComponentPanel>
                                </div>
                            </div>

                            {/* <Dialog open={this.state.showPaymentFormModal} onClose={this.hideModal} aria-labelledby="Payment_detail"
                                maxWidth="md" fullWidth={true}>
                                <DialogTitle id="form-dialog-title" className="bg-prussianBlue">Payment detail</DialogTitle>
                                <DialogContent className="bg-prussianBlue">
                                    <PaymentForm key={this.state.formKey} paymentId={this.state.paymentId} bankAccountId={this.state.selectedBankAccount}
                                        handleClose={this.handleConfirmationClose} history={this.props.history} />
                                </DialogContent>
                            </Dialog> */}
                        </React.Fragment>
                    </MainFrame>
                </div >
            </React.Fragment>
        )
    }
}