import { Dialog, DialogContent, DialogTitle } from '@material-ui/core';
import moment from 'moment';
import * as React from 'react'
import { BudgetApi, BudgetModel, Configuration } from '../../ApiClient';
import ActualBudgetCard, { ActualBudgetCardProps } from './ActualBudgetCard';
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

    private renderCard = (budgetModel: BudgetViewModel): JSX.Element => {
        let actualProps: ActualBudgetCardProps = { from: budgetModel.dateFrom, limit: budgetModel.amount, name: budgetModel.name, spent: 20 };
        return (
            <div className="w-2/5 my-2 cursor-pointer">
                <ActualBudgetCard {...actualProps}></ActualBudgetCard>
            </div>
        );
    }

    public render() {
        return (
            <React.Fragment>
                <div className="flex flex-col mt-6">
                    <h2 className="ml-6 text-xl">Actual budgets</h2>
                    <div className="flex flex-row flex-wrap justify-around">
                        {this.state.budgets.map(b => this.renderCard(b))}
                    </div>
                </div>
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