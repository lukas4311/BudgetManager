import _, { last } from "lodash";
import { PaymentApi, PaymentTypeModel, PaymentCategoryModel, PaymentModel } from "../ApiClient/Main";

export default class PaymentService {
    private paymentApi: PaymentApi;

    constructor(paymentApi: PaymentApi) {
        this.paymentApi = paymentApi;
    }

    public async getPaymentTypes() {
        const types: PaymentTypeModel[] = await this.paymentApi.paymentsTypesGet();
        return types;
    }

    public async getPaymentCategories() {
        const categories: PaymentCategoryModel[] = await this.paymentApi.paymentsCategoriesGet();
        return categories;
    }

    public async getPaymentById(id: number) {
        const paymentResponse = await this.paymentApi.paymentsDetailGet({ id: id });
        return paymentResponse;
    }

    public async createPayment(paymentModel: PaymentModel) {
        const response = await this.paymentApi.paymentsPost({ paymentModel: paymentModel });
        return response;
    }

    public async updatePayment(paymentModel: PaymentModel) {
        const response = await this.paymentApi.paymentsPut({ paymentModel: paymentModel });
        return response;
    }

    public getExactDateRangeDaysPaymentData = async (dateFrom: Date, dateTo: Date, bankAccountId: number): Promise<PaymentModel[]> => {
        return await this.paymentApi.paymentsGet({ fromDate: dateFrom, toDate: dateTo, bankAccountId });
    }

    public getAverageMonthExpense = (payments: PaymentModel[]) => {
        const expenses = payments.filter(f => f.paymentTypeCode == 'Expense');

        if (!expenses || expenses.length == 0)
            return 0;

        const orderedPayments = _.orderBy(payments, o => o.date);
        const firstPayment = _.first(orderedPayments);
        const lastPayment = _.last(orderedPayments);

        const monthCount = this.calculateMonthCount(firstPayment.date, lastPayment.date);
        const sumExpenses = _.sumBy(expenses, s => s.amount);

        return sumExpenses / (!monthCount || monthCount == 0 ? 1 : monthCount);
    }

    private calculateMonthCount(fromDate: Date, toDate: Date): number {
        const fromYear = fromDate.getFullYear();
        const fromMonth = fromDate.getMonth();
        const toYear = toDate.getFullYear();
        const toMonth = toDate.getMonth();

        return (toYear - fromYear) * 12 + (toMonth - fromMonth) + 1;
    }
}