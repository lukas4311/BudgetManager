import { Dialog, DialogContent, DialogTitle } from '@material-ui/core';
import moment from 'moment';
import * as React from 'react'
import { BudgetApi, BudgetModel, Configuration } from '../../ApiClient';
import { Modal } from '../../Modal';
import { BaseList } from '../BaseList';
import { BudgetComponentProps } from './BudgetComponentProps';
import { BudgetComponentState } from './BudgetComponentState';
import { BudgetForm2, BudgetFormModel } from './BudgetForm';
import { BudgetViewModel } from './BudgetViewModel';

export default class BudgetComponent extends React.Component<BudgetComponentProps, BudgetComponentState> {
    private budgetApi: BudgetApi;

    constructor(props: BudgetComponentProps) {
        super(props);
        this.state = { showBudgetFormModal: false, budgetFormKey: Date.now(), budgets: [], selectedBudgetId: undefined, selectedBudget: undefined };
        this.budgetApi = new BudgetApi();
    }

    public componentDidMount() {
        this.loadData();
    }

    public loadData = async (): Promise<void> => {
        await this.loadBudget();
    }

    private async loadBudget(): Promise<void> {
        let budgets = await this.budgetApi.budgetGetAllGet();
        let budgetViewModels: BudgetViewModel[] = budgets.map(b => ({
            id: b.id, amount: b.amount, dateFrom: moment(b.dateFrom).format('DD.MM.YYYY')
            , dateTo: moment(b.dateTo).format('DD.MM.YYYY'), name: b.name
        }));
        this.setState({ budgets: budgetViewModels });
    }

    private hideBudgetModal = (): void => {
        this.setState({ showBudgetFormModal: false, budgetFormKey: Date.now(), selectedBudgetId: undefined });
    }

    private addNewItem = (): void => {
        this.setState({ showBudgetFormModal: true, budgetFormKey: Date.now(), selectedBudget: undefined });
    }

    private budgetEdit = async (id: number): Promise<void> => {
        const budgetModel: BudgetModel = await this.budgetApi.budgetGetGet({ id: id });
        let budgetFormModel: BudgetFormModel = {
            amount: budgetModel.amount, from: moment(budgetModel.dateFrom).format("YYYY-MM-DD"),
            to: moment(budgetModel.dateTo).format("YYYY-MM-DD"), id: budgetModel.id, name: budgetModel.name, onSave: this.saveFormData
        };
        this.setState({ selectedBudgetId: id, showBudgetFormModal: true, budgetFormKey: Date.now(), selectedBudget: budgetFormModel });
    }

    private deleteItem = (id: number): void => {
        this.budgetApi.budgetDeleteDelete({ body: id });
    }

    private saveFormData = (model: BudgetFormModel) => {
        let budgetModel: BudgetModel = {
            amount: parseInt(model.amount.toString()), dateFrom: new Date(model.from),
            dateTo: new Date(model.to), id: model.id, name: model.name
        };

        if (model.id != undefined) {
            this.budgetApi.budgetUpdatePut({ budgetModel: budgetModel });
        } else {
            this.budgetApi.budgetAddPost({ budgetModel: budgetModel });
        }

        this.hideBudgetModal();
    }

    private renderTemplate = (budgetModel: BudgetViewModel): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-3/10">{budgetModel.dateFrom}</p>
                <p className="mx-6 my-1 w-3/10">{budgetModel.dateTo}</p>
                <p className="mx-6 my-1 w-2/10">{budgetModel.amount}</p>
                <p className="mx-6 my-1 w-2/10">{budgetModel.name}</p>
            </>
        );
    }

    private renderHeader = (): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-3/10">Od</p>
                <p className="mx-6 my-1 w-3/10">Do</p>
                <p className="mx-6 my-1 w-2/10">Výše</p>
                <p className="mx-6 my-1 w-2/10">Název</p>
            </>
        );
    }

    public render() {
        return (
            <React.Fragment>
                <BaseList<BudgetViewModel> title="Rozpočty" data={this.state.budgets} template={this.renderTemplate}
                    header={this.renderHeader()} addItemHandler={this.addNewItem} itemClickHandler={this.budgetEdit} deleteItemHandler={this.deleteItem}>
                </BaseList>
                <Dialog open={this.state.showBudgetFormModal} onClose={this.hideBudgetModal} aria-labelledby="Detail rozpočtu"
                    maxWidth="sm" fullWidth={true}>
                    <DialogTitle id="form-dialog-title">Detail rozpočtu</DialogTitle>
                    <DialogContent>
                        <div className="p-2 overflow-y-auto">
                            <BudgetForm2 key={this.state.budgetFormKey} onSave={this.saveFormData} {...this.state.selectedBudget} ></BudgetForm2>
                        </div>
                    </DialogContent>
                </Dialog>
            </React.Fragment>
        );
    }
}