import * as React from 'react'
import DataLoader from '../../Services/DataLoader'
import { PaymentTypeResponse } from '../../Model/PaymentTypeResponse';
import { PaymentCategoryResponse } from '../../Model/PaymentCategoryResponse';
import { IPaymentModel } from '../../Model/IPaymentModel';
import { IPaymentResponseModel } from '../../Model/IPaymentResponseModel';
import moment from 'moment';
import PaymentTagManager from '../PaymentTagManager';
import { useForm } from 'react-hook-form';
import { Button, FormControl, InputLabel, MenuItem, Select, TextField } from '@material-ui/core';
import { PaymentType } from '../../Model/PaymentType';
import { PaymentCategory } from '../../Model/PaymentCategory';
import { IconsData } from '../../Enums/IconsEnum';

interface IPaymentFormProps {
    paymentId: number,
    bankAccountId: number,
    handleClose: () => void
}

export default class PaymentForm extends React.Component<IPaymentFormProps, IPaymentModel>{
    private requiredMessage: string = "Zadejte hodnotu.";
    private dataLoader: DataLoader;

    constructor(props: IPaymentFormProps) {
        super(props);
        this.state = {
            name: '', amount: 0, date: moment(Date.now()).format("YYYY-MM-DD"), description: '', formErrors: { name: '', amount: '', date: '', description: '' }, paymentTypeId: -1, paymentTypes: [],
            paymentCategoryId: -1, paymentCategories: [], bankAccountId: this.props.bankAccountId, id: this.props.paymentId, disabledConfirm: false, errorMessage: undefined,
            tags: []
        };
        this.dataLoader = new DataLoader();
    }

    private processPaymentTypesData = (data: PaymentTypeResponse) => {
        if (data.success) {
            this.setState({ paymentTypeId: data.types[0].id, paymentTypes: data.types })
        }
    }

    private processPaymentCategoryData = (data: PaymentCategoryResponse) => {
        if (data.success) {
            this.setState({ paymentCategoryId: data.categories[0].id, paymentCategories: data.categories })
        }
    }

    private processPaymentData = (data: IPaymentModel) => {
        this.setState({
            name: data.name, amount: data.amount, date: data.date, description: data.description || '',
            paymentTypeId: data.paymentTypeId, paymentCategoryId: data.paymentCategoryId, bankAccountId: data.bankAccountId
        })
    }

    public async componentDidMount() {
        const paymentReponse = await this.dataLoader.getPaymentTypes(this.onError);
        this.processPaymentTypesData(paymentReponse);
        const categories = await this.dataLoader.getPaymentCategories(this.onError);
        this.processPaymentCategoryData(categories);

        if (this.state.id != null) {
            let paymentResponse = await this.dataLoader.getPayment(this.state.id, this.onError);
            this.processPaymentData(paymentResponse);
        }
    }

    private confirmPayment = (e: React.FormEvent<HTMLFormElement>): void => {
        e.preventDefault();
        this.setState({ disabledConfirm: true });
        const data = this.state;
        let dataJson = JSON.stringify(data);

        if (this.state.id != undefined) {
            // this.dataLoader.updatePayment(dataJson, this.onError)
            console.log(data);
        } else {
            console.log(data);
            // this.dataLoader.addPayment(dataJson, this.onError)
        }

        this.props.handleClose();
    }

    private onError = () => {
        this.setState({ errorMessage: 'Při uložení záznamu došlo k chybě' });
    }

    private handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>, property: string, isRequired: boolean = false): void => {
        let errorMessage = '';
        this.setState(prevState => ({
            ...prevState,
            [property]: e.target.value // No error here, but can't ensure that key is in StateKeys
        }));

        if (isRequired) {
            if (e.target.value == '' || e.target.value === undefined)
                errorMessage = this.requiredMessage;

            this.setState((prevState) => ({ formErrors: { ...prevState.formErrors, [property]: errorMessage } }));
        }
    }

    private generateErrorMessageIfError = (propertyName: string): JSX.Element | '' => {
        if (this.state.formErrors[propertyName].length > 0)
            return <span className="inline-block text-sm float-left ml-6">{this.state.formErrors[propertyName]}</span>;

        return '';
    }

    private changeType = (e: React.MouseEvent<HTMLAnchorElement, MouseEvent>, id: number) => {
        e.preventDefault();
        this.setState({ paymentTypeId: id });
    }

    private changeCategory = (e: React.ChangeEvent<HTMLSelectElement>) => {
        this.setState({ paymentCategoryId: parseInt(e.target.value) });
    }

    private tagsChange = (tags: string[]) => {
        this.setState({ tags: tags });
    }

    public render() {
        let iconsData = new IconsData();

        return (
            <div className="text-white">
                <div className={"transition-all ease-in-out duration-500 bg-rufous h-auto overflow-hidden" + (this.state.errorMessage != undefined ? ' opacity-100 scale-y-100' : ' scale-y-0 opacity-0')}>
                    <span className="text-sm text-left text-white">{this.state.errorMessage}</span>
                </div>
                <form onSubmit={this.confirmPayment} className="paymentForm">
                    <PaymentTagManager tags={this.state.tags} tagsChange={this.tagsChange} />
                    <div className="w-full">
                        <div className="flex w-10/12 m-auto">
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
                                <FormControl className="w-full">
                                    <InputLabel id="demo-simple-select-label">Kategorie</InputLabel>
                                    <Select
                                        labelId="demo-simple-select-label"
                                        id="type"
                                        value={this.state.paymentCategoryId}
                                        onChange={this.changeCategory}
                                    >
                                        {this.state.paymentCategories.map(p => {
                                            return <MenuItem key={p.id} value={p.id}>
                                                <span>{p.name}</span>
                                                <span className="ml-6 w-5 categoryIconSelectbox">{iconsData[p.icon]}</span>
                                            </MenuItem>
                                        })}
                                    </Select>
                                </FormControl>
                            </div>
                        </div>
                    </div>
                    <div className="flex mt-4">
                        <div className="w-1/2">
                            <div className="relative inline-block float-left ml-6 w-2/3">
                                <TextField label="Název" type="text" name="name" className="w-full" onChange={(e) => this.handleChange(e, "name", true)} value={this.state["name"]} />
                            </div>
                        </div>
                        <div className="w-1/2">
                            <div className="relative inline-block float-left ml-6 w-2/3">
                                <TextField label="Výše" type="text" name="amount" className="w-full" onChange={(e) => this.handleChange(e, "amount", true)} value={this.state["amount"]} />
                            </div>
                        </div>
                    </div>
                    <div className="flex mt-4">
                        <div className="w-1/2">
                            <div className="relative inline-block float-left ml-6 w-2/3">
                                <TextField label="Datum" type="date" name="date" className="w-full" value={this.state.date} onChange={(e) => this.handleChange(e, "date", true)}
                                    InputLabelProps={{
                                        shrink: true,
                                    }}
                                />
                            </div>
                            {this.generateErrorMessageIfError("date")}
                        </div>
                    </div>
                    <div className="flex my-4">
                        <div className="w-full">
                            <div className="relative inline-block w-4/5 float-left ml-6">
                                <TextField label="Popis" type="text" name="description" className="w-full" onChange={(e) => this.handleChange(e, "description")} value={this.state["description"]} />
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