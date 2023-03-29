import _, { last } from "lodash";
import { PaymentApi, PaymentTypeModel, PaymentCategoryModel, PaymentModel } from "../ApiClient/Main";

export default class PaymentService {
    private paymentApi: PaymentApi;
    private expenseCode: string = "Expense";
    private revenueCode: string = "Revenue";

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
        const expenses = payments.filter(f => f.paymentTypeCode == this.expenseCode);

        if (!expenses || expenses.length == 0)
            return 0;

        return this.getAverageAmountFromPayments(expenses);
    }

    public getAverageMonthRevenues = (payments: PaymentModel[]) => {
        const revenues = payments.filter(f => f.paymentTypeCode == this.revenueCode);

        if (!revenues || revenues.length == 0)
            return 0;

        return this.getAverageAmountFromPayments(revenues);
    }

    public getAverageMonthInvestment = (payments: PaymentModel[]) => {
        const investments = payments.filter(f => f.paymentTypeCode == this.expenseCode && f.paymentCategoryCode == "Invetsment");

        if (!investments || investments.length == 0)
            return 0;

        return this.getAverageAmountFromPayments(investments);
    }

    public getMeanExpense = (payments: PaymentModel[]) => {
        const expenses = payments.filter(f => f.paymentTypeCode == this.expenseCode);

        if (!expenses || expenses.length == 0)
            return 0;

        return this.getMeanValueFromPayments(expenses);
    }

    public clonePayment(paymentId: number) {
        this.paymentApi.paymentsCloneIdPost({ id: paymentId });
    }

    public getTopPaymentsByAmount(payments: PaymentModel[], count: number): PaymentModel[] {
        const sortedPayments = _.orderBy(payments, ['amount'], ['desc']);
        const topPayments = _.slice(sortedPayments, 0, count);
        return topPayments;
    }

    private getAverageAmountFromPayments = (payments: PaymentModel[]) => {
        if (!payments || payments.length == 0)
            return 0;

        const monthCount = this.getMonthCountFromPayments(payments);
        const sumExpenses = _.sumBy(payments, s => s.amount);
        return sumExpenses / (!monthCount || monthCount == 0 ? 1 : monthCount);
    }

    private getMeanValueFromPayments = (payments: PaymentModel[]) => {
        if (!payments || payments.length == 0)
            return 0;

        const monthCount = this.getMonthCountFromPayments(payments);
        return _.meanBy(payments, s => s.amount);
    }

    private getMonthCountFromPayments = (payments: PaymentModel[]) => {
        const orderedPayments = _.orderBy(payments, o => o.date);
        const firstPayment = _.first(orderedPayments);
        const lastPayment = _.last(orderedPayments);
        return this.calculateMonthCount(firstPayment.date, lastPayment.date);
    }

    private calculateMonthCount(fromDate: Date, toDate: Date): number {
        const fromYear = fromDate.getFullYear();
        const fromMonth = fromDate.getMonth();
        const toYear = toDate.getFullYear();
        const toMonth = toDate.getMonth();

        return (toYear - fromYear) * 12 + (toMonth - fromMonth) + 1;
    }
}