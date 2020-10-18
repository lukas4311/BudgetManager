import * as React from 'react'

class BudgetFormState {
    errorMessage: string;
    disabledConfirm: boolean;
    formErrors: {
        name: string;
        amount: string;
        date: string;
        description: string;
    };
}

export default class BudgetForm extends React.Component<{}, BudgetFormState> {
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

    private confirmBudget = () => {

    }

    private handleChange = (e: React.ChangeEvent<HTMLInputElement>, propertyName: string) => {
        this.setState(prevState => ({ ...prevState, [propertyName]: e.target.value }))
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
                            {this.generateInput("name", "Název výdaje", this.handleChange)}
                        </div>
                        <div className="w-1/2">
                            {this.generateInput("amount", "Výše výdaje", this.handleChange)}
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
                                <button type="submit" disabled={this.state.disabledConfirm} className="bg-vermilion px-4 py-1 rounded-sm hover:text-vermilion hover:bg-white duration-500">Potvrdit</button>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        );
    }
}