import * as React from 'react'
import { IPaymentInfo } from './PaymentsOverview'

interface IPaymentModel {
    name: string,
    amount: number,
    date: string,
    description: string,
    formErrors: {
        name: string,
        amount: string,
        date: string,
        description: string
    }
}

export default class PaymentForm extends React.Component<IPaymentInfo, IPaymentModel>{
    requiredMessage: string = "Zadejte hodnotu.";

    constructor(props: IPaymentInfo) {
        super(props);
        this.addPayment = this.addPayment.bind(this);
        this.handleChangeName = this.handleChangeName.bind(this);
        this.handleChangeName = this.handleChangeName.bind(this);
        this.handleChangeAmount = this.handleChangeAmount.bind(this);
        this.handleChangeDescription = this.handleChangeDescription.bind(this);
        this.generateErrorMessageIfError = this.generateErrorMessageIfError.bind(this);
        this.state = { name: props.name, amount: props.amount, date: props.date, description: props.description, formErrors: { name: '', amount: '', date: '', description: '' } };
    }

    addPayment(e: React.FormEvent<HTMLFormElement>): void {
        e.preventDefault();
        const data = this.state;
        let dataJson = JSON.stringify(data);
        fetch('/Payment/AddPayment', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: dataJson,
        })
            .then(response => response.json())
            .then(data => { console.log('Success:', data); })
            .catch((error) => { console.error('Error:', error); });
    }

    handleChangeName = (e: React.ChangeEvent<HTMLInputElement>): void => {
        this.setState({ name: e.target.value });

        if (e.target.value == '' || e.target.value === undefined)
            this.setState((prevState) => ({ formErrors: { ...prevState.formErrors, name: this.requiredMessage } }));
    }

    handleChangeDate = (e: React.ChangeEvent<HTMLInputElement>): void => {
        this.setState({ date: e.target.value });

        if (e.target.value == '' || e.target.value === undefined)
            this.setState((prevState) => ({ formErrors: { ...prevState.formErrors, date: this.requiredMessage } }));
    }

    handleChangeDescription = (e: React.ChangeEvent<HTMLInputElement>): void => {
        this.setState({ description: e.target.value });

        if (e.target.value == '' || e.target.value === undefined)
            this.setState((prevState) => ({ formErrors: { ...prevState.formErrors, description: this.requiredMessage } }));
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
            return <span className="collapsed inline-block text-sm float-left ml-6">{this.state.formErrors[propertyName]}</span>;

        return '';
    }

    generateInput(propertyName: string, placeholder: string, handler: (e: React.ChangeEvent<HTMLInputElement>) => void) {
        return (
            <React.Fragment>
                <div className="relative inline-block float-left ml-6">
                    <input className={"effect-11" + this.addErrorClassIfError(propertyName)} placeholder={placeholder} value={this.state[propertyName]} onChange={handler}></input>
                    <span className="focus-bg"></span>
                </div>
                {this.generateErrorMessageIfError(propertyName)}
            </React.Fragment>
        );
    }

    render() {
        return (
            <div className="bg-prussianBlue text-white">
                <h2 className="text-2xl py-4 ml-6 text-left">Detail platby</h2>
                <form onSubmit={this.addPayment}>
                    <div className="flex">
                        <div className="w-1/2">
                            {this.generateInput("name", "Název výdaje", this.handleChangeName)}
                        </div>
                        <div className="w-1/2">
                            {this.generateInput("amount", "Výše výdaje", this.handleChangeAmount)}
                        </div>
                    </div>
                    <div className="flex mt-4">
                        <div className="w-1/2">
                            <div className="relative inline-block float-left ml-6">
                                <input type="date" className={"effect-11" + this.addErrorClassIfError("date")} placeholder="Datum" value={this.state.date} onChange={this.handleChangeDate}></input>
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