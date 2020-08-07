import { PaymentType } from './PaymentType';
import { PaymentCategory } from './PaymentCategory';

export interface IPaymentModel {
    id?: number;
    name: string;
    amount: number;
    date: string;
    description: string;
    paymentTypeId: number;
    paymentTypes: Array<PaymentType>;
    paymentCategoryId: number;
    paymentCategories: Array<PaymentCategory>;
    bankAccountId: number;
    formErrors: {
        name: string;
        amount: string;
        date: string;
        description: string;
    };
    disabledConfirm: boolean;
    errorMessage: string;
}
