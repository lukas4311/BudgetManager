import { Dialog, DialogContent, DialogTitle } from '@material-ui/core';
import moment from 'moment';
import * as React from 'react'
import { BudgetApi, BudgetModel, Configuration } from '../../ApiClient/Main';
import ApiClientFactory from '../../Utils/ApiClientFactory';
import ActualBudgetCard, { ActualBudgetCardProps } from './ActualBudgetCard';
import { BudgetComponentProps } from './BudgetComponentProps';
import { BudgetComponentState } from './BudgetComponentState';
import { BudgetForm2, BudgetFormModel } from './BudgetForm';
import { BudgetViewModel } from './BudgetViewModel';
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";

const theme = createMuiTheme({
    palette: {
        type: 'dark',
    }
});

export default class BudgetComponent extends React.Component<BudgetComponentProps, BudgetComponentState> {
    private budgetApi: BudgetApi;

    constructor(props: BudgetComponentProps) {
        super(props);
        this.state = { showBudgetFormModal: false, budgetFormKey: Date.now(), budgets: [], selectedBudgetId: undefined, selectedBudget: undefined };
    }

    public componentDidMount() {
        this.loadData();
    }

    public loadData = async (): Promise<void> => {
        await this.loadBudget();
    }

    private async loadBudget(): Promise<void> {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.budgetApi = await apiFactory.getClient(BudgetApi);
        let budgets = await this.budgetApi.budgetsActualGet();
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
            this.budgetApi.budgetsPut({ budgetModel: budgetModel });
        } else {
            this.budgetApi.budgetsPost({ budgetModel: budgetModel });
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

    private addNewBudget = () => {
        this.setState({ showBudgetFormModal: true, selectedBudget: null, selectedBudgetId: null, budgetFormKey: Date.now() });
    }

    public render() {
        return (
            <React.Fragment>
                <ThemeProvider theme={theme}>
                    <div>
                        <div className="flex flex-col mt-6 w-2/3 m-auto">
                            <h2 className="ml-6 text-xl text-left">Actual budgets</h2>
                            <span className="inline-block ml-auto mr-5" onClick={this.addNewBudget}>
                                <svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 0 24 24" width="24" className="fill-current text-white hover:text-vermilion transition ease-out duration-700 cursor-pointer">
                                    <path d="M0 0h24v24H0z" fill="none" />
                                    <path d="M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-2 10h-4v4h-2v-4H7v-2h4V7h2v4h4v2z" />
                                </svg>
                            </span>
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
                    </div>
                </ThemeProvider>
            </React.Fragment>
        );
    }
}