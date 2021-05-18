import { IBaseModel } from "../Components/BaseList";

export default class BankAccountViewModel implements IBaseModel{
    id: number;
    code: string;
    openingBalance: number;
}