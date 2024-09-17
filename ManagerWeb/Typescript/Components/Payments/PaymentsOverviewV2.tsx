import * as React from 'react'
import moment, { unitOfTime } from 'moment';
import PaymentForm from './PaymentForm'
import { IconsData } from '../../Enums/IconsEnum';
import { Dialog, DialogTitle, DialogContent, Button } from '@mui/material';
import { BaseList, EListStyle } from '../BaseList';
import ApiClientFactory from '../../Utils/ApiClientFactory'
import { BankAccountApi, PaymentApi, PaymentModel } from '../../ApiClient/Main';
import { RouteComponentProps } from 'react-router-dom';
import _ from 'lodash';
import { ComponentPanel } from '../../Utils/ComponentPanel';
import { MainFrame } from '../MainFrame';
import PaymentService from '../../Services/PaymentService';
import { BankAccountSelector } from '../BankAccount/BankAccountSelector';
import { BankAccountBalanceCard } from '../BankAccount/BankAccountBalanceCard';
import DateRangeComponent from '../../Utils/DateRangeComponent';


interface PaymentsOverviewStateV2 {
    payments: PaymentModel[];
    showPaymentFormModal: boolean;
    paymentId: number;
    formKey: number;
    selectedBankAccountId: number;
    filterDateFrom: Date;
    filterDateTo: Date;
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

export default class PaymentsOverview extends React.Component<RouteComponentProps, PaymentsOverviewStateV2> {
    private paymentService: PaymentService;

    constructor(props: RouteComponentProps) {
        super(props);
        moment.locale('cs');

        this.state = {
            payments: [], paymentId: undefined, showPaymentFormModal: false, formKey: Date.now(), selectedBankAccountId: undefined,
            filterDateFrom: undefined, filterDateTo: undefined
        };
    }

    public componentDidMount() {
        this.init();
    }

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        const paymentApi = await apiFactory.getClient(PaymentApi);
        this.paymentService = new PaymentService(paymentApi);
        await this.loadData();
    }

    private loadData = async () => {
        const payments = await this.getExactDateRangeDaysPaymentData(moment(Date.now()).subtract(1, 'months').toDate(), moment(Date.now()).toDate(), null);
        const fromLastOrderder = _.orderBy(payments, a => a.date, "desc");
        this.setState({ payments: fromLastOrderder });
    }

    private getExactDateRangeDaysPaymentData = async (dateFrom: Date, dateTo: Date, bankAccountId: number): Promise<PaymentModel[]> =>
        await this.paymentService.getExactDateRangeDaysPaymentData(dateFrom, dateTo, bankAccountId);

    private paymentEdit = (id: number): void => {
        this.setState({ paymentId: id, showPaymentFormModal: true, formKey: Date.now() });
    }

    private addNewPayment = () => {
        this.setState({ showPaymentFormModal: true, paymentId: null, formKey: Date.now() });
    }

    private deletePayment = (id: number) => {
        this.paymentService.deletePayment(id);
    }

    private getPaymentColor(paymentTypeCode: string): string {
        switch (paymentTypeCode) {
            case "Revenue":
                return " text-green-800";
            case "Expense":
                return " text-red-600";
            case "Transfer":
                return " text-blue-500";
        }
    }

    private clonePayment = (e: React.MouseEvent<HTMLSpanElement, MouseEvent>, id: number) => {
        console.log("clone: " + id);
        e.preventDefault();
        e.stopPropagation();
        this.paymentService.clonePayment(id);
    }

    private hideModal = () => this.setState({ showPaymentFormModal: false, paymentId: null, formKey: Date.now() });

    private handleConfirmationClose = () => {
        this.hideModal();
    }

    private filterPayments = async (from: Date, to: Date) => {
        const payments = await this.getExactDateRangeDaysPaymentData(from, to, null);
        const fromLastOrderder = _.orderBy(payments, a => a.date, "desc");
        this.setState({ payments: fromLastOrderder });
    }

    private renderTemplate = (p: PaymentModel): JSX.Element => {
        let iconsData: IconsData = new IconsData();

        return (
            <div className="flex flex-col w-full py-2 px-4 text-prussianBlue">
                <div className="flex flex-row w-full">
                    <p className="mr-auto text-sm font-medium">{moment(p.date).format('DD.MM.YYYY')}</p>
                    <span className="ml-auto categoryIcon fill-prussianBlue">{iconsData[p.paymentCategoryIcon]}</span>
                </div>
                <div className='text-left'>
                    <p className="text-3xl font-bold">{p.amount},-</p>
                    <p className="text-md truncate">{p.name}</p>
                </div>
            </div>
        );
    }

    public render() {
        return (
            <React.Fragment>
                <div className="">
                    <MainFrame header='Payments overview'>
                        <React.Fragment>
                            <div className='grid grid-cols-4 gap-6'>
                                <div className='col-span-4 justify-self-end w-2/5'>
                                    <DateFilter onFilter={this.filterPayments}></DateFilter>
                                </div>
                                <div className='col-span-3'>
                                    <ComponentPanel classStyle="w-full px-5 py-5">
                                        <div className='flex flex-col'>
                                            <h2 className="text-2xl mb-4 text-left">Balance info</h2>
                                            <div className='flex flex-row'>
                                                <div className='w-1/2 px-16 text-left'>
                                                    <BankAccountBalanceCard cardClass='mb-4' />
                                                    <BankAccountSelector onBankAccountSelect={b => this.setState({ selectedBankAccountId: b.bankAccountId })} />
                                                </div>
                                                <div className='w-1/2 px-16 text-left'>
                                                    <IncomeCard payments={this.state.payments} cardClass='mb-4' />
                                                    <ExpenseCard payments={this.state.payments} />
                                                </div>
                                            </div>
                                        </div>
                                    </ComponentPanel>
                                </div>
                                <div className="flex flex-col lg:flex-row lg:flex-wrap 2xl:flex-nowrap w-full">
                                    <div className="w-full">
                                        <ComponentPanel classStyle="px-5 py-5">
                                            <>
                                                <div className="py-4 flex text-left">
                                                    <h2 className="text-2xl">Income/expense</h2>
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
                            </div>

                            <Dialog open={this.state.showPaymentFormModal} onClose={this.hideModal} aria-labelledby="Payment_detail"
                                maxWidth="md" fullWidth={true}>
                                <DialogTitle id="form-dialog-title" className="bg-prussianBlue">Payment detail</DialogTitle>
                                <DialogContent className="bg-prussianBlue">
                                    <PaymentForm key={this.state.formKey} paymentId={this.state.paymentId} bankAccountId={this.state.selectedBankAccountId}
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

class IncomeExpenseCardProps {
    payments: PaymentModel[];
    cardClass?: string;
}

const IncomeCard = (props: IncomeExpenseCardProps) => {
    const revenues = props.payments.filter(p => p.paymentTypeCode == "Revenue");
    const sum = _.sumBy(revenues, p => p.amount);

    return (
        <div className={`flex flex-col bg-battleshipGrey px-4 py-6 rounded-lg relative ${props?.cardClass ?? ""}`}>
            <span className="text-2xl text-left font-semibold categoryIcon fill-white">Income</span>
            <p className='text-4xl text-center font-black mb-2'>{sum},-</p>
        </div>
    );
}

const ExpenseCard = (props: IncomeExpenseCardProps) => {
    const revenues = props.payments.filter(p => p.paymentTypeCode == "Expense");
    const sum = _.sumBy(revenues, p => p.amount);

    return (
        <div className={`flex flex-col bg-battleshipGrey px-4 py-6 rounded-lg relative ${props?.cardClass ?? ""}`}>
            <span className="text-2xl text-left font-semibold categoryIcon fill-white">Expense</span>
            <p className='text-4xl text-center font-black mb-2'>{sum},-</p>
        </div>
    );
}

class DateFilterProps {
    onFilter: (from: Date, to: Date) => void;
    className?: string;
}

const DateFilter = (props: DateFilterProps) => {
    const onSelectDateClick = (momentCode: unitOfTime.Base | unitOfTime._quarter) => {
        const from = moment(Date.now()).subtract(1, momentCode).toDate();
        const to = moment(Date.now()).toDate();
        props?.onFilter(from, to);
    }

    const onRangeChange = (from: string, to: string) => {
        props?.onFilter(moment(from).toDate(), moment(to).toDate());
    }

    return (
        <div className={`flex flex-row justify-between ${props.className ?? ""}`}>
            <Button type="button" variant="contained" color="primary" className="block mr-4" onClick={_ => onSelectDateClick('w')}>This week</Button>
            <Button type="button" variant="contained" color="primary" className="block mr-4" onClick={_ => onSelectDateClick('M')}>This month</Button>
            <Button type="button" variant="contained" color="primary" className="block mr-6" onClick={_ => onSelectDateClick('y')}>This year</Button>
            <DateRangeComponent datesFilledHandler={onRangeChange} className='w-1/2'></DateRangeComponent>
        </div>
    );
}