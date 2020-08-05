import * as React from 'react'
import DataLoader from './DataLoader'

interface IPaymentFormProps{
    paymentId: number
    ,bankAccountId: number
}

interface PaymentType {
    id: number,
    name: string
}

interface PaymentCategory {
    id: number,
    name: string
}

export interface IPaymentModel {
    id?: number
    name: string,
    amount: number,
    date: string,
    description: string,
    paymentTypeId: number,
    paymentTypes: Array<PaymentType>,
    paymentCategoryId: number
    paymentCategories: Array<PaymentCategory>,
    bankAccountId: number
    formErrors: {
        name: string,
        amount: string,
        date: string,
        description: string
    }
}

export default class PaymentForm extends React.Component<IPaymentFormProps, IPaymentModel>{
    requiredMessage: string = "Zadejte hodnotu.";
    dataLoader: DataLoader;

    constructor(props: IPaymentFormProps) {
        super(props);
        this.confirmPayment = this.confirmPayment.bind(this);
        this.handleChangeName = this.handleChangeName.bind(this);
        this.handleChangeName = this.handleChangeName.bind(this);
        this.handleChangeAmount = this.handleChangeAmount.bind(this);
        this.handleChangeDescription = this.handleChangeDescription.bind(this);
        this.generateErrorMessageIfError = this.generateErrorMessageIfError.bind(this);
        this.changeCategory = this.changeCategory.bind(this);
        this.changeType = this.changeType.bind(this);
        this.state = {
            name: '', amount: 0, date: '', description: '',
            formErrors: { name: '', amount: '', date: '', description: '' },
            paymentTypeId: -1, paymentTypes: [], paymentCategoryId: -1, paymentCategories: [],
            bankAccountId: this.props.bankAccountId, id: this.props.paymentId
        };
        this.dataLoader = new DataLoader();
    }

    componentDidUpdate(prevProps:IPaymentFormProps):void{
        if (this.props.paymentId != null && prevProps.paymentId != this.props.paymentId) {
            this.dataLoader.getPayment(this.props.paymentId)
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        this.setState({
                            name: data.payment.name, amount: data.payment.amount, date: data.payment.date, description: data.payment.description || '', paymentTypeId: data.payment.paymentTypeId,
                            paymentCategoryId: data.payment.paymentCategoryId, bankAccountId: data.payment.bankAccountId
                        })
                    }
                })
                .catch((error) => { console.error('Error:', error); });
        }
    }

    componentDidMount() {
        this.dataLoader.getPaymentTypes()
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    this.setState({ paymentTypeId: data.types[0].id, paymentTypes: data.types })
                }
            })
            .catch((error) => { console.error('Error:', error); });

        this.dataLoader.getPaymentCategories()
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    this.setState({ paymentCategoryId: data.categories[0].id, paymentCategories: data.categories })
                }
            })
            .catch((error) => { console.error('Error:', error); });

        if (this.state.id != null) {
            this.dataLoader.getPayment(this.state.id)
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        this.setState({
                            name: data.Name, amount: data.Amount, date: data.Date, description: data.Description, paymentTypeId: data.PaymentTypeId,
                            paymentCategoryId: data.PaymentCategoryId, bankAccountId: data.BankAccountId
                        })
                    }
                })
                .catch((error) => { console.error('Error:', error); });
        }
    }

    confirmPayment(e: React.FormEvent<HTMLFormElement>): void {
        e.preventDefault();
        const data = this.state;
        let dataJson = JSON.stringify(data);
        let promise: Promise<Response>;

        if (this.state.id != undefined) {
            promise = this.dataLoader.updatePayment(dataJson)

        } else {
            promise = this.dataLoader.addPayment(dataJson)
        }

        promise
            .then(response => response.json())
            .then(data => { console.log('Success:', data); })
            .catch((error) => { console.error('Error:', error); });
    }

    handleChangeName = (e: React.ChangeEvent<HTMLInputElement>): void => {
        let errorMessage = '';
        this.setState({ name: e.target.value });

        if (e.target.value == '' || e.target.value === undefined)
            errorMessage = this.requiredMessage;

        this.setState((prevState) => ({ formErrors: { ...prevState.formErrors, name: errorMessage } }));
    }

    handleChangeDate = (e: React.ChangeEvent<HTMLInputElement>): void => {
        let errorMessage = '';
        this.setState({ date: e.target.value });

        if (e.target.value == '' || e.target.value === undefined)
            errorMessage = this.requiredMessage;

        this.setState((prevState) => ({ formErrors: { ...prevState.formErrors, date: errorMessage } }));
    }

    handleChangeDescription = (e: React.ChangeEvent<HTMLInputElement>): void => {
        this.setState({ description: e.target.value });
    }

    handleChangeAmount = (e: React.ChangeEvent<HTMLInputElement>): void => {
        let parsed = parseInt(e.target.value);

        if (isNaN(parsed)) {
            this.setState({ amount: null })
            this.setState((prevState) => ({ formErrors: { ...prevState.formErrors, amount: "Zadejte číselnou hodnotu." } }));
        } else {
            this.setState({ amount: parsed })
            this.setState((prevState) => ({ formErrors: { ...prevState.formErrors, amount: "" } }));
        }
    }

    addErrorClassIfError(propertyName: string): string {
        if (this.state.formErrors[propertyName].length > 0)
            return " inputError";

        return '';
    }

    generateErrorMessageIfError(propertyName: string): JSX.Element | '' {
        if (this.state.formErrors[propertyName].length > 0)
            return <span className="inline-block text-sm float-left ml-6">{this.state.formErrors[propertyName]}</span>;

        return '';
    }

    generateInput(propertyName: string, placeholder: string, handler: (e: React.ChangeEvent<HTMLInputElement>) => void) {
        return (
            <React.Fragment>
                <div className="relative inline-block float-left ml-6 w-2/3">
                    <input className={"effect-11 w-full" + this.addErrorClassIfError(propertyName)} placeholder={placeholder} value={this.state[propertyName]} onChange={handler}></input>
                    <span className="focus-bg"></span>
                </div>
                {this.generateErrorMessageIfError(propertyName)}
            </React.Fragment>
        );
    }

    changeType(e: React.MouseEvent<HTMLAnchorElement, MouseEvent>, id: number) {
        e.preventDefault();
        this.setState({ paymentTypeId: id });
    }

    changeCategory(e: React.ChangeEvent<HTMLSelectElement>) {
        this.setState({ paymentCategoryId: parseInt(e.target.value) });
    }

    render() {
        return (
            <div className="bg-prussianBlue text-white">
                <h2 className="text-2xl py-4 ml-6 text-left">Detail platby</h2>
                <form onSubmit={this.confirmPayment}>
                    <div className="w-full">
                        <div className="inline-flex w-11/12">
                            {this.state.paymentTypes.map(p => {
                                return <a key={p.id}
                                    className={"w-full bg-prussianBlue border-blueSapphire border-b-2 border-r-2 border-l-2 px-8 py-2 hover:bg-blueSapphire duration-500 cursor-pointer" + (this.state.paymentTypeId == p.id ? " activeType" : "")}
                                    onClick={(e) => this.changeType(e, p.id)}>{p.name}</a>
                            })}
                        </div>
                    </div>
                    <div className="flex mt-4">
                        <div className="w-1/2">
                            <div className="relative inline-block float-left ml-6 w-2/3">
                                <select name="type" id="type" className="effect-11 w-full" onChange={this.changeCategory}>
                                    {this.state.paymentCategories.map(p => {
                                        return <option key={p.id} value={p.id}>{p.name}</option>
                                    })}
                                </select>
                            </div>
                        </div>
                    </div>
                    <div className="flex mt-4">
                        <div className="w-1/2">
                            {this.generateInput("name", "Název výdaje", this.handleChangeName)}
                        </div>
                        <div className="w-1/2">
                            {this.generateInput("amount", "Výše výdaje", this.handleChangeAmount)}
                        </div>
                    </div>
                    <div className="flex mt-4">
                        <div className="w-1/2">
                            <div className="relative inline-block float-left ml-6 w-2/3">
                                <input type="date" className={"effect-11 w-full" + this.addErrorClassIfError("date")} placeholder="Datum" value={this.state.date} onChange={this.handleChangeDate}></input>
                                <span className="focus-bg"></span>
                            </div>
                            {this.generateErrorMessageIfError("date")}
                        </div>
                    </div>
                    <div className="flex my-4">
                        <div className="w-full">
                            <div className="relative inline-block w-4/5 float-left ml-6">
                                <input className={"effect-11 w-full" + this.addErrorClassIfError("description")} placeholder="Popis" value={this.state.description} onChange={this.handleChangeDescription}></input>
                                <span className="focus-bg"></span>
                            </div>
                        </div>
                    </div>
                    <div className="flex">
                        <div className="w-full">
                            <div className="relative inline-block float-left ml-6 mb-6">
                                <button type="submit" className="bg-vermilion px-4 py-1 rounded-sm hover:text-vermilion hover:bg-white duration-500">Potvrdit</button>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        );
    }
}