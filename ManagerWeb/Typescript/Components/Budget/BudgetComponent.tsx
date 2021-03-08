import moment from 'moment';
import * as React from 'react'
import { Modal } from '../../Modal';
import { BudgetModel } from '../../Model/BudgetModel';
import DataLoader from '../../Services/DataLoader';
import { BaseList } from '../BaseList';
import { BudgetComponentProps } from './BudgetComponentProps';
import { BudgetComponentState } from './BudgetComponentState';
import BudgetForm from './BudgetForm';

export default class BudgetComponent extends React.Component<BudgetComponentProps, BudgetComponentState> {
    private dataLoader: DataLoader;

    constructor(props: BudgetComponentProps) {
        super(props);
        this.state = { showBudgetFormModal: false, budgetFormKey: Date.now(), budgets: [], selectedBudgetId: undefined };
        this.dataLoader = new DataLoader();
    }

    public componentDidMount() {
        this.loadData();
    }

    public loadData = async () => {
        await this.loadBudget();
    }

    private async loadBudget(): Promise<void> {
        let budgets = await this.dataLoader.getAllBudgets();
        this.setState({ budgets: budgets });
    }

    private hideBudgetModal = () => {
        this.setState({ showBudgetFormModal: false, budgetFormKey: Date.now() });
    }

    private budgetEdit(id: number) {
        this.setState({ selectedBudgetId: id, showBudgetFormModal: true, budgetFormKey: Date.now() });
    }

    private renderTemplate = (budgetModel: BudgetModel): JSX.Element => {
        return (
            <div key={budgetModel.id} className="paymentRecord bg-battleshipGrey rounded-r-full flex mr-6 mt-1 hover:bg-vermilion cursor-pointer" onClick={(_) => this.budgetEdit(budgetModel.id)}>
                <p className="mx-6 my-1 w-1/5">{moment(budgetModel.dateFrom).format('DD.MM.YYYY')}</p>
                <p className="mx-6 my-1 w-1/5">{moment(budgetModel.dateTo).format('DD.MM.YYYY')}</p>
                <p className="mx-6 my-1 w-1/5">{budgetModel.amount}</p>
                <p className="mx-6 my-1 w-2/5">{budgetModel.name}</p>
            </div>
        );
    }

    private renderHeader = (): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/5">Od</p>
                <p className="mx-6 my-1 w-1/5">Do</p>
                <p className="mx-6 my-1 w-1/5">Výše</p>
                <p className="mx-6 my-1 w-2/5">Název</p>
            </>
        );
    }

    private addNewItem = (): void => {
        this.setState({ showBudgetFormModal: true });
    }

    public render() {
        return (
            <React.Fragment>
                <BaseList<BudgetModel> title="Rozpočty" data={this.state.budgets} template={this.renderTemplate}
                    header={this.renderHeader()} addItemHandler={this.addNewItem}>
                </BaseList>
                <Modal show={this.state.showBudgetFormModal} handleClose={this.hideBudgetModal}>
                    <BudgetForm key={this.state.budgetFormKey} id={this.state.selectedBudgetId} handleClose={this.hideBudgetModal}></BudgetForm>
                </Modal>
            </React.Fragment>
        );
    }
}