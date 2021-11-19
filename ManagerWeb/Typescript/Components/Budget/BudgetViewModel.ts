import { IBaseModel } from '../BaseList';

export class BudgetViewModel implements IBaseModel {
    id: number;
    amount: number;
    dateFrom: Date;
    dateTo: Date;
    name: string;
}
