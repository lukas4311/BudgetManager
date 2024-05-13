import { PaymentTypeModel, PaymentCategoryModel, PaymentModel } from "../ApiClient/Main";


export interface IPaymentService {
    getPaymentTypes(): Promise<PaymentTypeModel[]>;
    getPaymentCategories(): Promise<PaymentCategoryModel[]>;
    getPaymentById(id: number): Promise<any>;
    createPayment(paymentModel: PaymentModel): Promise<any>;
    updatePayment(paymentModel: PaymentModel): Promise<any>;
    getExactDateRangeDaysPaymentData(dateFrom: Date, dateTo: Date, bankAccountId: number): Promise<PaymentModel[]>;
    getAverageMonthExpense(payments: PaymentModel[]): number;
    getAverageMonthRevenues(payments: PaymentModel[]): number;
    getAverageMonthInvestment(payments: PaymentModel[]): number;
    getMeanExpense(payments: PaymentModel[]): number;
    clonePayment(paymentId: number): void;
    getTopPaymentsByAmount(payments: PaymentModel[], count: number, paymentType?: string): PaymentModel[];
    getPaymentsSumGroupedByMonth(dateFrom: Date, dateTo: Date, bankAccountId: number);
    deletePayment(paymentId: number): Promise<void>;
}
