import moment from "moment";
import { BudgetApiInterface, BudgetModel } from "../ApiClient/Main";
import { BudgetFormModel } from "../Components/Budget/BudgetForm";
import { BudgetViewModel } from "../Components/Budget/BudgetViewModel";
import { IBudgetService } from "./IBudgetService";

export default class BudgetService implements IBudgetService {
    private budgetApi: BudgetApiInterface;

    constructor(budgetApi: BudgetApiInterface) {
        this.budgetApi = budgetApi;
    }

    public async getAllBudgets(): Promise<BudgetViewModel[]> {
        let budgets = await this.budgetApi.budgetsActualGet();
        let budgetViewModels: BudgetViewModel[] = this.getMappedViewModels(budgets);
        return budgetViewModels;
    }

    public async updateBudget(model: BudgetFormModel) {
        let budgetModel: BudgetModel = {
            amount: parseInt(model.amount.toString()), dateFrom: new Date(model.from),
            dateTo: new Date(model.to), id: model.id, name: model.name
        };

        await this.budgetApi.budgetsPut({ budgetModel: budgetModel });
    }

    public async createBudget(model: BudgetFormModel) {
        let budgetModel: BudgetModel = {
            amount: parseInt(model.amount.toString()), dateFrom: new Date(model.from),
            dateTo: new Date(model.to), id: model.id, name: model.name
        };

        await this.budgetApi.budgetsPost({ budgetModel: budgetModel });
    }

    private getMappedViewModels = (bankAccountModels: BudgetModel[]): BudgetViewModel[] =>
        bankAccountModels.map(b => ({
            id: b.id, amount: b.amount, dateFrom: moment(b.dateFrom).toDate(),
            dateTo: moment(b.dateTo).toDate(), name: b.name
        }));
}