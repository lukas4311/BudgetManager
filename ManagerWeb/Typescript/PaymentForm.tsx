import * as React from 'react'

interface IPaymentModel {
    name: string,
    amount: string,
    date: string,
    description: string,
    formErrors: {
        name: string,
        amount: string,
        date: string,
        description: string
    }
}

export default class PaymentForm extends React.Component<{}, IPaymentModel>{
    requiredMessage: string = "Zadejte hodnotu.";

    constructor(props: {}) {
        super(props);
        this.addPayment = this.addPayment.bind(this);
        this.handleChangeName = this.handleChangeName.bind(this);
        this.handleChangeName = this.handleChangeName.bind(this);
        this.handleChangeAmount = this.handleChangeAmount.bind(this);
        this.state = { name: '', amount: '', date: '', description: '', formErrors: { name: '', amount: '', date: '', description: '' } };
    }

    addPayment(e: React.FormEvent<HTMLFormElement>) {
        e.preventDefault();
        const data = this.state;

        fetch('/Payment/AddPayment', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data),
        })
            .then(response => response.json())
            .then(data => { console.log('Success:', data); })
            .catch((error) => { console.error('Error:', error); });
    }

    handleChangeName = (e: React.ChangeEvent<HTMLInputElement>) => {
        this.setState({ name: e.target.value });
        this.setState((prevState) => ({formErrors: {...prevState.formErrors, name: this.requiredMessage}}));
    }

    handleChangeAmount = (e: React.ChangeEvent<HTMLInputElement>) => {
        let parsed = parseInt(e.target.value);

        if (isNaN(parsed)) {
            this.setState((prevState) => ({formErrors: {...prevState.formErrors, amount: "Zadejte číselnou hodnotu."}}));
        } else {
            this.setState({amount: e.target.value})
            this.setState((prevState) => ({formErrors: {...prevState.formErrors, amount: ""}}));
        }
    }

    addErrorClassIfError(propertyName: string){
        if(this.state.formErrors[propertyName].length > 0)
            return "inputError";
    }

    render() {
        return (
            <div className="bg-prussianBlue text-white">
                <h2 className="text-2xl py-4 ml-6 text-left">Detail platby</h2>
                <form onSubmit={this.addPayment}>
                    <div className="flex">
                        <div className="w-1/2">
                            <div className="relative inline-block float-left ml-6">
                                <input className={"effect-11 " + this.addErrorClassIfError("name")} placeholder="Název výdaje" value={this.state.name} onChange={this.handleChangeName}></input>
                                <span className="focus-bg"></span>
                            </div>
                        </div>
                        <div className="w-1/2">
                            <div className="relative inline-block float-left ml-6">
                                <input className={"effect-11" + this.addErrorClassIfError("amount")} placeholder="Výše výdaje" value={this.state.amount} onChange={this.handleChangeAmount}></input>
                                <span className="focus-bg"></span>
                            </div>
                        </div>
                    </div>
                    <div className="flex mt-4">
                        <div className="w-1/2">
                            <div className="relative inline-block float-left ml-6">
                                <input type="date" className="effect-11" placeholder="Datum"></input>
                                <span className="focus-bg"></span>
                            </div>
                        </div>
                    </div>
                    <div className="flex my-4">
                        <div className="w-full">
                            <div className="relative inline-block w-4/5 float-left ml-6">
                                <input className="effect-11 w-full" placeholder="Popis"></input>
                                <span className="focus-bg"></span>
                            </div>
                        </div>
                    </div>
                    <div className="flex">
                        <div className="w-full">
                            <div className="relative inline-block float-left ml-6 mb-6">
                                <button value="Potvrdit" className="bg-vermilion px-4 py-1 rounded-sm" />
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        );
    }
}