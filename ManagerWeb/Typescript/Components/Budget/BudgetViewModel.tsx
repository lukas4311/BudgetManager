import { IBaseModel } from '../BaseList';

export class BudgetViewModel implements IBaseModel {
    id: number;
    amount: number;
    dateFrom: string;
    dateTo: string;
    name: string;
}
