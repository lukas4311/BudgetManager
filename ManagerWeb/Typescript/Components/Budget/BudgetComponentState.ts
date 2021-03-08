import { BudgetViewModel } from './BudgetViewModel';

export class BudgetComponentState {
    budgetFormKey: number;
    showBudgetFormModal: boolean;
    budgets: BudgetViewModel[];
    selectedBudgetId: number;
}
