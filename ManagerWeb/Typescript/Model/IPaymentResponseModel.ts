import { IPaymentModel } from './IPaymentModel';

export interface IPaymentResponseModel {
    success: boolean;
    payment: IPaymentModel;
}
