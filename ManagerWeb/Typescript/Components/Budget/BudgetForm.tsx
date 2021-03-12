import * as React from 'react'
import DataLoader from '../../Services/DataLoader';
import { BudgetApi, BudgetModel, Configuration } from '../../ApiClient';
import moment from 'moment';
import { useForm } from "react-hook-form";
import { Button, TextField } from '@material-ui/core';

class BudgetFormState {
    id: number;
    errorMessage: string;
    disabledConfirm: boolean;
    name: string;
    amount: number;
    to: Date;
    from: Date;
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
    onSave: (model: BudgetFormState) => void;
}

export default class BudgetForm extends React.Component<BudgetFormProps, BudgetFormState> {
    private dataLoader: DataLoader;
    private budgetApi: BudgetApi;

    constructor(props: BudgetFormProps) {
        super(props);
        this.dataLoader = new DataLoader();
        this.budgetApi = new BudgetApi();
        this.state = { id: undefined, name: '', amount: 0, to: undefined, from: undefined, errorMessage: '', disabledConfirm: false, formErrors: { from: '', to: '', amount: '', name: '' } }
    }

    async componentDidMount() {
        if (this.props.id != null) {
            const budgetModel = await this.budgetApi.budgetGetGet({ id: this.props.id });
            this.setState({ id: this.props.id, name: budgetModel.name, amount: budgetModel.amount, to: budgetModel.dateTo, from: budgetModel.dateFrom });
        }
        else {
            this.setState({ id: undefined, name: "", amount: 0, to: undefined, from: undefined });
        }
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
        let budgetModel: BudgetModel = {
            amount: this.state.amount, dateFrom: new Date(this.state.from),
            dateTo: new Date(this.state.to), id: this.state.id, name: this.state.name
        };

        if (this.state.id != undefined) {
            this.budgetApi.budgetUpdatePut({ budgetModel: budgetModel });
        } else {
            budgetModel.id = null;
            this.budgetApi.budgetAddPost({ budgetModel: budgetModel });
        }

        this.props.handleClose();
    }

    private handleChange = (e: React.ChangeEvent<HTMLInputElement>, propertyName: string) => {
        let value = e.target.value;
        this.setState(prevState => ({ ...prevState, [propertyName]: value }));
    }

    private handleChangeNumber = (e: React.ChangeEvent<HTMLInputElement>, propertyName: string) => {
        let value: Number = Number.parseInt(e.target.value);
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
                                <input type="date" className={"effect-11 w-full" + this.addErrorClassIfError("from")} placeholder="Datum od" value={moment(this.state.from).format('YYYY-MM-DD')} onChange={e => this.handleChange(e, 'from')}></input>
                                <span className="focus-bg"></span>
                            </div>
                            {this.generateErrorMessageIfError("from")}
                        </div>
                        <div className="w-1/2">
                            <div className="relative inline-block float-left ml-6 w-2/3">
                                <input type="date" className={"effect-11 w-full" + this.addErrorClassIfError("to")} placeholder="Datum do" value={moment(this.state.to).format('YYYY-MM-DD')} onChange={e => this.handleChange(e, 'to')}></input>
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

class BudgetFormModel {
    id: number;
    name: string;
    amount: number;
    to: Date;
    from: Date;
    onSave: (model: BudgetFormModel) => void;
}

const BudgetForm2 = (props: BudgetFormModel) => {
    const { register, handleSubmit } = useForm<BudgetFormModel>({ defaultValues: props });

    const onSubmit = (data: BudgetFormModel) => {
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div>
                    <TextField
                        label="Název"
                        type="text"
                        name="name"
                        inputRef={register}
                    />
                </div>
                <div>
                    <TextField
                        label="Velikost"
                        type="text"
                        name="amount"
                        inputRef={register}
                    />
                </div>
                <div>
                    <TextField
                        label="Od"
                        type="date"
                        name="from"
                        inputRef={register}
                    />
                </div>
                <div>
                    <TextField
                        label="Do"
                        type="date"
                        name="to"
                        inputRef={register}
                    />
                </div>
            </div>

            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Uložit</Button>
        </form>
    );
};