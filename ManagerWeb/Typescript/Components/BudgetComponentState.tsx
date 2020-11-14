import { BudgetModel } from '../Model/BudgetModel';

export class BudgetComponentState {
    budgetFormKey: number;
    showBudgetFormModal: boolean;
    budgets: BudgetModel[];
    selectedBudgetId: number;
}
