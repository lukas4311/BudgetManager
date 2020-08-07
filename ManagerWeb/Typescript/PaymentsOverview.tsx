import * as React from 'react'
import moment from 'moment';
import { Modal } from './Modal'
import PaymentForm from './PaymentForm'
import DataLoader from './DataLoader';
import { IPaymentInfo } from "./Model/IPaymentInfo"
import { BankAccount } from './Model/BankAccount';
import { BankAccountReponse } from './Model/BankAccountReponse';

interface PaymentsOverviewState {
    payments: Array<IPaymentInfo>,
    selectedFilter: DateFilter,
    showPaymentFormModal: boolean,
    bankAccounts: Array<BankAccount>
    selectedBankAccount?: number,
    showBankAccountError: boolean,
    paymentId: number,
    formKey: number,
    apiError: string
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

    constructor(props: {}) {
        super(props);
        moment.locale('cs');
        this.filters = [{ caption: "7d", days: 7, key: 1 }, { caption: "1m", days: 30, key: 2 }, { caption: "3m", days: 90, key: 3 }];
        this.state = {
            payments: [], selectedFilter: this.filters[0], showPaymentFormModal: false, bankAccounts: [], selectedBankAccount: undefined,
            showBankAccountError: false, paymentId: null, formKey: Date.now(), apiError: undefined
        };
        this.filterClick = this.filterClick.bind(this);
        this.addNewPayment = this.addNewPayment.bind(this);
        this.hideModal = this.hideModal.bind(this);
        this.bankAccountChange = this.bankAccountChange.bind(this);
        this.handleConfirmationClose = this.handleConfirmationClose.bind(this);
        this.onRejected = this.onRejected.bind(this);
        this.setPayments = this.setPayments.bind(this);
        this.setBankAccounts = this.setBankAccounts.bind(this);
        this.dataLoader = new DataLoader();
    }

    public componentDidMount() {
        this.dataLoader.getBankAccounts(this.setBankAccounts, this.onRejected);
        this.getPaymentData(this.state.selectedFilter.days);
    }

    private getPaymentData(daysBack: number) {
        let filterDate: string = moment(Date.now()).subtract(daysBack, 'days').format("YYYY-MM-DD");
        this.dataLoader.getPayments(filterDate, this.setPayments, this.onRejected);
    }

    setBankAccounts(data: BankAccountReponse) {
        if (data.success) {
            let bankAccounts: Array<BankAccount> = data.bankAccounts;
            bankAccounts.unshift({ code: this.defaultBankOption, id: null });
            this.setState({ bankAccounts: bankAccounts });
        }
    }

    private onRejected(_: any) {
        this.setState({ apiError: this.apiErrorMessage });
    }

    private setPayments(response: any) {
        if (response != undefined) {
            this.setState({ payments: response });
        } else {
            this.setState({ apiError: this.apiErrorMessage })
        }
    }

    private filterClick(filterKey: number) {
        let selectedFilter = this.filters.find(f => f.key == filterKey);

        if (this.state.selectedFilter != selectedFilter) {
            this.setState({ selectedFilter: selectedFilter });
            this.getPaymentData(selectedFilter.days);
        }
    }

    private addNewPayment() {
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
        this.getPaymentData(this.state.selectedFilter.days);
    }

    private bankAccountChange(e: React.ChangeEvent<HTMLSelectElement>) {
        let selectedbankId: number = parseInt(e.target.value);
        this.setState({ selectedBankAccount: selectedbankId });
        this.getPaymentData(this.state.selectedFilter.days);
    }

    private showErrorMessage() {
        let tag: JSX.Element = <React.Fragment></React.Fragment>;
        if (this.state.apiError != undefined) {
            tag = <span className="errorMessage inline-block px-6 py-2 mt-2 bg-red-700 rounded-full w-2/3">{this.state.apiError}</span>
        }

        return tag;
    }

    private getPaymentColor(paymentTypeCode: string): string {
        switch (paymentTypeCode) {
            case "Revenue":
                return "bg-green-800"
            case "Expenses":
                return "bg-red-600"
            case "Transfer":
                return "bg-blue-500"
        }
    }

    public render() {
        return (
            <div className="text-center mt-6 bg-prussianBlue rounded-lg">
                {this.showErrorMessage()}
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
                        {this.state.bankAccounts.map(b => {
                            return <option key={b.id} value={b.id}>{b.code}</option>
                        })}
                    </select>
                </div>
                <div className="flex text-black mb-3 ml-6 cursor-pointer">
                    {this.filters.map((f) =>
                        <span key={f.key} className="px-4 bg-white transition duration-700 hover:bg-vermilion text-sm" onClick={() => this.filterClick(f.key)}>{f.caption}</span>
                    )}
                </div>
                <div className="pb-10">
                    {this.state.payments.map(p =>
                        <div key={p.id} className="paymentRecord bg-battleshipGrey rounded-r-full flex mr-6 mt-1 hover:bg-vermilion cursor-pointer" onClick={(_) => this.paymentEdit(p.id)}>
                            <span className={"min-h-full w-4 inline-block " + this.getPaymentColor(p.paymentTypeCode)}></span>
                            <p className="mx-6 my-2 w-1/5">{p.amount},-</p>
                            <p className="mx-6 my-2 w-2/5">{p.name}</p>
                            <p className="mx-6 my-2 w-1/5">{moment(p.date).format('DD.MM.YYYY')}</p>
                            <p className="mx-6 my-2 w-1/5">Ikona</p>
                        </div>
                    )}
                </div>
                <Modal show={this.state.showPaymentFormModal} handleClose={this.hideModal}>
                    <PaymentForm key={this.state.formKey} paymentId={this.state.paymentId} bankAccountId={this.state.selectedBankAccount} handleClose={this.handleConfirmationClose}></PaymentForm>
                </Modal>
            </div >
        )
    }
}