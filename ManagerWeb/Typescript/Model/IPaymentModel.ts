import { PaymentType } from './PaymentType';
import { PaymentCategory } from './PaymentCategory';
import { PaymentCategoryModel, PaymentTypeModel } from '../ApiClient';

export interface IPaymentModel {
    id?: number;
    name: string;
    amount: number;
    date: string;
    description: string;
    paymentTypeId: number;
    paymentTypes: PaymentTypeModel[];
    paymentCategoryId: number;
    paymentCategories: PaymentCategoryModel[];
    bankAccountId: number;
    formErrors: {
        name: string;
        amount: string;
        date: string;
        description: string;
    };
    disabledConfirm: boolean;
    errorMessage: string;
    tags: string[]
}
