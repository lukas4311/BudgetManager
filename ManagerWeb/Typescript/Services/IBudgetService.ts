import { BudgetFormModel } from "../Components/Budget/BudgetForm";
import { BudgetViewModel } from "../Components/Budget/BudgetViewModel";

export interface IBudgetService {
    getAllBudgets(): Promise<BudgetViewModel[]>;
    updateBudget(model: BudgetFormModel): Promise<void>;
    createBudget(model: BudgetFormModel): Promise<void>;
}
