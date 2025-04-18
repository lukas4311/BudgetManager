import * as React from 'react'
import DataLoader from '../../Services/DataLoader'
import { IPaymentModel } from '../../Model/IPaymentModel';
import moment from 'moment';
import PaymentTagManager from '../PaymentTagManager';
import { FormControl, InputLabel, MenuItem, Select, TextField } from '@mui/material';
import { IconsData } from '../../Enums/IconsEnum';
import { PaymentApi, PaymentCategoryModel, PaymentTypeModel, PaymentModel } from '../../ApiClient/Main';
import ApiClientFactory from '../../Utils/ApiClientFactory';
import * as H from 'history';
import PaymentService from '../../Services/PaymentService';

class PaymentFormProps {
    paymentId: number;
    bankAccountId: number;
    handleClose: () => void;
    history: H.History<any>;
}

export default class PaymentForm extends React.Component<PaymentFormProps, IPaymentModel> {
    private requiredMessage: string = "Zadejte hodnotu.";
    private paymentService: any;

    constructor(props: PaymentFormProps) {
        super(props);
        this.state = {
            name: '', amount: 0, date: moment(Date.now()).format("YYYY-MM-DD"), description: '', formErrors: { name: '', amount: '', date: '', description: '' }, paymentTypeId: -1,
            paymentTypes: [], paymentCategoryId: -1, paymentCategories: [], bankAccountId: this.props.bankAccountId, id: this.props.paymentId,
            disabledConfirm: false, errorMessage: undefined, tags: []
        };
    }

    private processPaymentTypesData = (data: PaymentTypeModel[]) => this.setState({ paymentTypeId: data[0].id, paymentTypes: data })

    private processPaymentCategoryData = (data: PaymentCategoryModel[]) =>
        this.setState({ paymentCategoryId: data[0].id, paymentCategories: data })

    private processPaymentData = (data: PaymentModel) => {
        this.setState({
            name: data.name, amount: data.amount, date: moment(data.date).format("YYYY-MM-DD"), description: data.description || '',
            paymentTypeId: data.paymentTypeId, paymentCategoryId: data.paymentCategoryId, bankAccountId: data.bankAccountId, tags: data.tags
        })
    }

    public async componentDidMount() {
        const apiFactory = new ApiClientFactory(this.props.history);
        const paymentApi = await apiFactory.getClient(PaymentApi);
        this.paymentService = new PaymentService(paymentApi);

        const types: PaymentTypeModel[] = await this.paymentService.getPaymentTypes();
        const categories: PaymentCategoryModel[] = await this.paymentService.getPaymentCategories();
        this.processPaymentTypesData(types);
        this.processPaymentCategoryData(categories);

        if (this.state.id != null) {
            let paymentResponse = await this.paymentService.getPaymentById(this.state.id);
            this.processPaymentData(paymentResponse);
        }
    }

    private confirmPayment = async (e: React.FormEvent<HTMLFormElement>): Promise<void> => {
        e.preventDefault();
        this.setState({ disabledConfirm: true });
        let dataModel = this.mapViewModelToDatModel();

        if (this.state.id != undefined) {
            await this.paymentService.updatePayment(dataModel);
        } else {
            await this.paymentService.createPayment(dataModel);
        }

        this.props.handleClose();
    }

    private mapViewModelToDatModel = (): PaymentModel => {
        let dataModel = new PaymentModel();
        dataModel.amount = parseInt(this.state.amount.toString());
        dataModel.bankAccountId = this.state.bankAccountId;
        dataModel.date = new Date(this.state.date);
        dataModel.description = this.state.description;
        dataModel.id = this.state.id;
        dataModel.name = this.state.name;
        dataModel.paymentCategoryId = this.state.paymentCategoryId;
        dataModel.paymentTypeId = this.state.paymentTypeId;
        dataModel.tags = this.state.tags;

        return dataModel;
    }

    private handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>, property: string, isRequired: boolean = false): void => {
        let errorMessage = '';
        let value = e.target.value;
        this.setState(prevState => ({
            ...prevState,
            [property]: value
        }));

        if (isRequired) {
            if (value == '' || value === undefined)
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

    private changeCategory = (e: any) =>
        this.setState({ paymentCategoryId: parseInt(e.target.value) });

    private tagsChange = (tags: string[]) =>
        this.setState({ tags: tags });

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
                    <div className='grid grid-cols-2 gap-x-10 gap-y-4 w-10/12 mx-auto mt-6'>
                        <div className="flex">
                            <div className="w-full">
                                <FormControl className="w-full">
                                    <InputLabel id="demo-simple-select-label">Category</InputLabel>
                                    <Select
                                        labelId="demo-simple-select-label"
                                        id="type"
                                        size='small'
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
                        <div className="amount row-span-3">
                            <div className="h-full place-self-center">
                                <TextField label="Amount" type="text" size='small' name="amount" className="w-full h-full" onChange={(e) => this.handleChange(e, "amount", true)} value={this.state["amount"]} inputProps={{ style: { fontSize: 40 } }} />
                            </div>
                        </div>
                        <div className="">
                            <div className="">
                                <TextField label="Name" type="text" size='small' name="name" className="w-full" onChange={(e) => this.handleChange(e, "name", true)} value={this.state["name"]} />
                            </div>
                        </div>
                        <div className="">
                            <div className="">
                                <div className="">
                                    <TextField label="Payment date" type="date" size='small' name="date" className="w-full" value={this.state.date} onChange={(e) => this.handleChange(e, "date", true)}
                                        InputLabelProps={{ shrink: true }}
                                    />
                                </div>
                                {this.generateErrorMessageIfError("date")}
                            </div>
                        </div>
                        <div className="relative inline-block col-span-2">
                            <TextField label="Description" type="text" size='small' name="description" className="w-full" onChange={(e) => this.handleChange(e, "description")} value={this.state["description"]} />
                        </div>
                        <div className="relative inline-block mb-6 col-span-2">
                            <button type="submit" disabled={this.state.disabledConfirm} className="bg-vermilion px-4 py-1 rounded-sm hover:text-vermilion hover:bg-white duration-500">Save payment</button>
                        </div>
                    </div>
                </form>
            </div>
        );
    }
}