import _ from "lodash";
import { PaymentApi, PaymentTypeModel, PaymentCategoryModel, PaymentModel } from "../ApiClient/Main";
import { IPaymentService } from "./IPaymentService";
import moment from "moment";

export class MonthlyGroupedPayments {
    dateGroup: string;
    amountSum: number;
}

export default class PaymentService implements IPaymentService {
    private paymentApi: PaymentApi;
    private revenueCode: string = "Revenue";
    private expenseCode: string = "Expense";

    constructor(paymentApi: PaymentApi) {
        this.paymentApi = paymentApi;
    }

    // Get payment types from API
    public async getPaymentTypes() {
        const types: PaymentTypeModel[] = await this.paymentApi.paymentsTypesGet();
        return types;
    }

    // This code gets the payment categories from the server.
    public async getPaymentCategories() {
        const categories: PaymentCategoryModel[] = await this.paymentApi.paymentsCategoriesGet();
        return categories;
    }

    // This function retrieves a payment by its id.
    public async getPaymentById(id: number) {
        const paymentResponse = await this.paymentApi.paymentsDetailGet({ id: id });
        return paymentResponse;
    }

    // This code creates a payment for the specified payment model.
    public async createPayment(paymentModel: PaymentModel) {
        const response = await this.paymentApi.paymentsPost({ paymentModel: paymentModel });
        return response;
    }

    // This function updates the payment data for the specified payment model.
    public async updatePayment(paymentModel: PaymentModel) {
        const response = await this.paymentApi.paymentsPut({ paymentModel: paymentModel });
        return response;
    }

    // Get the payment data for the exact date range
    public getExactDateRangeDaysPaymentData = async (dateFrom: Date, dateTo: Date, bankAccountId: number): Promise<PaymentModel[]> => {
        return await this.paymentApi.paymentsGet({ fromDate: dateFrom, toDate: dateTo, bankAccountId });
    }

    // This code gets the sum of payments grouped by month.
    public getPaymentsSumGroupedByMonth = async (dateFrom: Date, dateTo: Date, bankAccountId: number): Promise<MonthlyGroupedPayments[]> => {
        const payments = await this.getExactDateRangeDaysPaymentData(dateFrom, dateTo, bankAccountId);
        const grouped = _.chain(payments.map(m => ({ ...m, date: moment(m.date).format("YYYY-MM"), amount: m.paymentTypeCode == this.expenseCode ? m.amount * -1 : m.amount })))
            .groupBy(g => moment(g.date).format("YYYY-MM"))
            .map((value, key) => ({ dateGroup: key, amountSum: _.sumBy(value, 'amount') }))
            .value();

        return grouped;
    }

    /* Gets the average expense per month for a given set of payments */
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

    public getTopPaymentsByAmount(payments: PaymentModel[], count: number, paymentType?: string): PaymentModel[] {
        if (paymentType)
            payments = payments.filter(p => p.paymentTypeCode == paymentType);

        const sortedPayments = _.orderBy(payments, ['amount'], ['desc']);
        const topPayments = _.slice(sortedPayments, 0, count);
        return topPayments;
    }

    public getMonthlyGroupedAccumulatedPayments(payments: PaymentModel[]) {
        const paymentGroupedData = [];

        _.chain(payments)
        .groupBy(s => moment(s.date).format('YYYY-MM'))
        .map((value, key) => ({ date: moment(key + "-1"), amount: _.sumBy(value, s => s.amount) }))
        .orderBy(f => f.date, ['asc'])
        .reduce((acc, model) => {
            const amount = acc.prev + model.amount;
            paymentGroupedData.push({ date: model.date, amount: amount });
            acc.prev = amount;
            return acc;
        }, { prev: 0 });

        return paymentGroupedData;
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