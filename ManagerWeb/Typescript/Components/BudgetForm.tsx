import * as React from 'react'
import DataLoader from '../Services/DataLoader';

class BudgetFormState {
    id: number;
    errorMessage: string;
    disabledConfirm: boolean;
    name: string;
    amount: number;
    to: string;
    from: string;
    formErrors: {
        from: string;
        to: string;
        amount: string;
        name: string;
    };
}

class BudgetFormProps {
    id: number;
    handleClose: () => void;
}

export default class BudgetForm extends React.Component<BudgetFormProps, BudgetFormState> {
    dataLoader: DataLoader;

    constructor(props: BudgetFormProps) {
        super(props);
        this.dataLoader = new DataLoader();
        this.state = { id: undefined, name: '', amount: 0, to: '', from: '', errorMessage: '', disabledConfirm: false, formErrors: { from: '', to: '', amount: '', name: '' } }
    }

    private addErrorClassIfError(propertyName: string): string {
        if (this.state.formErrors[propertyName].length > 0)
            return " inputError";

        return '';
    }

    private generateErrorMessageIfError = (propertyName: string): JSX.Element | '' => {
        if (this.state.formErrors[propertyName].length > 0)
            return <span className="inline-block text-sm float-left ml-6">{this.state.formErrors[propertyName]}</span>;

        return '';
    }

    private generateInput = (propertyName: string, placeholder: string, handler: (e: React.ChangeEvent<HTMLInputElement>, propertyName: string) => void) => {
        return (
            <React.Fragment>
                <div className="relative inline-block float-left ml-6 w-2/3">
                    <input className={"effect-11 w-full" + this.addErrorClassIfError(propertyName)} placeholder={placeholder} value={this.state[propertyName]} onChange={e => handler(e, propertyName)}></input>
                    <span className="focus-bg"></span>
                </div>
                {this.generateErrorMessageIfError(propertyName)}
            </React.Fragment>
        );
    }

    private confirmBudget = (e: React.FormEvent<HTMLFormElement>): void => {
        e.preventDefault();
        this.setState({ disabledConfirm: true });

        if (this.state.id != undefined) {
            this.dataLoader.updateBudget({ name: this.state.name, amount: this.state.amount, dateFrom: this.state.from, dateTo: this.state.to, id: this.state.id });
        } else {
            this.dataLoader.addBudget({ name: this.state.name, amount: this.state.amount, dateFrom: this.state.from, dateTo: this.state.to, id: null });
        }

        this.props.handleClose();
    }

    private handleChange = (e: React.ChangeEvent<HTMLInputElement>, propertyName: string) => {
        let value = e.target.value;
        this.setState(prevState => ({ ...prevState, [propertyName]: value }));
    }

    private handleChangeNumber = (e: React.ChangeEvent<HTMLInputElement>, propertyName: string) => {
        let value:Number = Number.parseInt(e.target.value);
        this.setState(prevState => ({ ...prevState, [propertyName]: value }));
    }

    render() {
        return (
            <div className="bg-prussianBlue text-white">
                <div className={"transition-all ease-in-out duration-500 bg-rufous h-auto overflow-hidden" + (this.state.errorMessage != undefined ? ' opacity-100 scale-y-100' : ' scale-y-0 opacity-0')}>
                    <span className="text-sm text-left text-white">{this.state.errorMessage}</span>
                </div>
                <h2 className="text-2xl py-4 ml-6 text-left">Detail rozpočtu</h2>
                <form onSubmit={this.confirmBudget}>
                    <div className="flex mt-4">
                        <div className="w-1/2">
                            {this.generateInput("name", "Název rozpočtu", this.handleChange)}
                        </div>
                        <div className="w-1/2">
                            {this.generateInput("amount", "Výše rozpočtu", this.handleChangeNumber)}
                        </div>
                    </div>
                    <div className="flex mt-4">
                        <div className="w-1/2">
                            <div className="relative inline-block float-left ml-6 w-2/3">
                                <input type="date" className={"effect-11 w-full" + this.addErrorClassIfError("from")} placeholder="Datum od" value={this.state.from} onChange={e => this.handleChange(e, 'from')}></input>
                                <span className="focus-bg"></span>
                            </div>
                            {this.generateErrorMessageIfError("from")}
                        </div>
                        <div className="w-1/2">
                            <div className="relative inline-block float-left ml-6 w-2/3">
                                <input type="date" className={"effect-11 w-full" + this.addErrorClassIfError("to")} placeholder="Datum do" value={this.state.to} onChange={e => this.handleChange(e, 'to')}></input>
                                <span className="focus-bg"></span>
                            </div>
                            {this.generateErrorMessageIfError("to")}
                        </div>
                    </div>
                    <div className="flex mt-4">
                        <div className="w-full">
                            <div className="relative inline-block float-left ml-6 mb-6">
                                <button type="submit" disabled={this.state.disabledConfirm} className="bg-vermilion px-4 py-1 rounded-sm hover:text-vermilion hover:bg-white duration-500">Potvrdit</button>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        );
    }
}