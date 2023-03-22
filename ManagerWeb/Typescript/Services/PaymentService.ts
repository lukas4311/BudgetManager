import { PaymentApi, PaymentTypeModel, PaymentCategoryModel, PaymentModel } from "../ApiClient/Main";

export default class PaymentService {
    private paymentApi: PaymentApi;

    constructor(paymentApi: PaymentApi) {
        this.paymentApi = paymentApi;
    }

    async getPaymentTypes() {
        const types: PaymentTypeModel[] = await this.paymentApi.paymentsTypesGet();
        return types;
    }

    async getPaymentCategories() {
        const categories: PaymentCategoryModel[] = await this.paymentApi.paymentsCategoriesGet();
        return categories;
    }

    async getPaymentById(id: number) {
        const paymentResponse = await this.paymentApi.paymentsDetailGet({ id: id });
        return paymentResponse;
    }

    async createPayment(paymentModel: PaymentModel) {
        const response = await this.paymentApi.paymentsPost({ paymentModel: paymentModel });
        return response;
    }

    async updatePayment(paymentModel: PaymentModel) {
        const response = await this.paymentApi.paymentsPut({ paymentModel: paymentModel });
        return response;
    }
}