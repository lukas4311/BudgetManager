import { BudgetModel } from '../../../wwwroot/js/app';
import { BudgetFormModel } from './BudgetForm';
import { BudgetViewModel } from './BudgetViewModel';

export class BudgetComponentState {
    budgetFormKey: number;
    showBudgetFormModal: boolean;
    budgets: BudgetViewModel[];
    selectedBudgetId: number;
    selectedBudget: BudgetFormModel;
}
