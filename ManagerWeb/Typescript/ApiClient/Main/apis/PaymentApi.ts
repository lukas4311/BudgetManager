/* tslint:disable */
/* eslint-disable */
/**
 * BudgetManager.Api
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: v1
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


import * as runtime from '../runtime';
import {
    PaymentCategoryModel,
    PaymentCategoryModelFromJSON,
    PaymentCategoryModelToJSON,
    PaymentModel,
    PaymentModelFromJSON,
    PaymentModelToJSON,
    PaymentTypeModel,
    PaymentTypeModelFromJSON,
    PaymentTypeModelToJSON,
} from '../models';

export interface PaymentsCloneIdPostRequest {
    id: number;
}

export interface PaymentsDeleteRequest {
    id?: number;
}

export interface PaymentsDetailGetRequest {
    id?: number;
}

export interface PaymentsGetRequest {
    fromDate?: Date | null;
    toDate?: Date | null;
    bankAccountId?: number | null;
}

export interface PaymentsPaymentIdTagTagIdDeleteRequest {
    tagId: number;
    paymentId: number;
}

export interface PaymentsPostRequest {
    paymentModel?: PaymentModel;
}

export interface PaymentsPutRequest {
    paymentModel?: PaymentModel;
}

/**
 * PaymentApi - interface
 * 
 * @export
 * @interface PaymentApiInterface
 */
export interface PaymentApiInterface {
    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof PaymentApiInterface
     */
    paymentsCategoriesGetRaw(): Promise<runtime.ApiResponse<Array<PaymentCategoryModel>>>;

    /**
     */
    paymentsCategoriesGet(): Promise<Array<PaymentCategoryModel>>;

    /**
     * 
     * @param {number} id 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof PaymentApiInterface
     */
    paymentsCloneIdPostRaw(requestParameters: PaymentsCloneIdPostRequest): Promise<runtime.ApiResponse<void>>;

    /**
     */
    paymentsCloneIdPost(requestParameters: PaymentsCloneIdPostRequest): Promise<void>;

    /**
     * 
     * @param {number} [id] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof PaymentApiInterface
     */
    paymentsDeleteRaw(requestParameters: PaymentsDeleteRequest): Promise<runtime.ApiResponse<void>>;

    /**
     */
    paymentsDelete(requestParameters: PaymentsDeleteRequest): Promise<void>;

    /**
     * 
     * @param {number} [id] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof PaymentApiInterface
     */
    paymentsDetailGetRaw(requestParameters: PaymentsDetailGetRequest): Promise<runtime.ApiResponse<PaymentModel>>;

    /**
     */
    paymentsDetailGet(requestParameters: PaymentsDetailGetRequest): Promise<PaymentModel>;

    /**
     * 
     * @param {Date} [fromDate] 
     * @param {Date} [toDate] 
     * @param {number} [bankAccountId] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof PaymentApiInterface
     */
    paymentsGetRaw(requestParameters: PaymentsGetRequest): Promise<runtime.ApiResponse<Array<PaymentModel>>>;

    /**
     */
    paymentsGet(requestParameters: PaymentsGetRequest): Promise<Array<PaymentModel>>;

    /**
     * 
     * @param {number} tagId 
     * @param {number} paymentId 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof PaymentApiInterface
     */
    paymentsPaymentIdTagTagIdDeleteRaw(requestParameters: PaymentsPaymentIdTagTagIdDeleteRequest): Promise<runtime.ApiResponse<void>>;

    /**
     */
    paymentsPaymentIdTagTagIdDelete(requestParameters: PaymentsPaymentIdTagTagIdDeleteRequest): Promise<void>;

    /**
     * 
     * @param {PaymentModel} [paymentModel] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof PaymentApiInterface
     */
    paymentsPostRaw(requestParameters: PaymentsPostRequest): Promise<runtime.ApiResponse<void>>;

    /**
     */
    paymentsPost(requestParameters: PaymentsPostRequest): Promise<void>;

    /**
     * 
     * @param {PaymentModel} [paymentModel] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof PaymentApiInterface
     */
    paymentsPutRaw(requestParameters: PaymentsPutRequest): Promise<runtime.ApiResponse<void>>;

    /**
     */
    paymentsPut(requestParameters: PaymentsPutRequest): Promise<void>;

    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof PaymentApiInterface
     */
    paymentsTypesGetRaw(): Promise<runtime.ApiResponse<Array<PaymentTypeModel>>>;

    /**
     */
    paymentsTypesGet(): Promise<Array<PaymentTypeModel>>;

}

/**
 * 
 */
export class PaymentApi extends runtime.BaseAPI implements PaymentApiInterface {

    /**
     */
    async paymentsCategoriesGetRaw(): Promise<runtime.ApiResponse<Array<PaymentCategoryModel>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/payments/categories`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(PaymentCategoryModelFromJSON));
    }

    /**
     */
    async paymentsCategoriesGet(): Promise<Array<PaymentCategoryModel>> {
        const response = await this.paymentsCategoriesGetRaw();
        return await response.value();
    }

    /**
     */
    async paymentsCloneIdPostRaw(requestParameters: PaymentsCloneIdPostRequest): Promise<runtime.ApiResponse<void>> {
        if (requestParameters.id === null || requestParameters.id === undefined) {
            throw new runtime.RequiredError('id','Required parameter requestParameters.id was null or undefined when calling paymentsCloneIdPost.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/payments/clone/{id}`.replace(`{${"id"}}`, encodeURIComponent(String(requestParameters.id))),
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async paymentsCloneIdPost(requestParameters: PaymentsCloneIdPostRequest): Promise<void> {
        await this.paymentsCloneIdPostRaw(requestParameters);
    }

    /**
     */
    async paymentsDeleteRaw(requestParameters: PaymentsDeleteRequest): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        if (requestParameters.id !== undefined) {
            queryParameters['id'] = requestParameters.id;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/payments`,
            method: 'DELETE',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async paymentsDelete(requestParameters: PaymentsDeleteRequest): Promise<void> {
        await this.paymentsDeleteRaw(requestParameters);
    }

    /**
     */
    async paymentsDetailGetRaw(requestParameters: PaymentsDetailGetRequest): Promise<runtime.ApiResponse<PaymentModel>> {
        const queryParameters: any = {};

        if (requestParameters.id !== undefined) {
            queryParameters['id'] = requestParameters.id;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/payments/detail`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => PaymentModelFromJSON(jsonValue));
    }

    /**
     */
    async paymentsDetailGet(requestParameters: PaymentsDetailGetRequest): Promise<PaymentModel> {
        const response = await this.paymentsDetailGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async paymentsGetRaw(requestParameters: PaymentsGetRequest): Promise<runtime.ApiResponse<Array<PaymentModel>>> {
        const queryParameters: any = {};

        if (requestParameters.fromDate !== undefined) {
            queryParameters['fromDate'] = (requestParameters.fromDate as any).toISOString();
        }

        if (requestParameters.toDate !== undefined) {
            queryParameters['toDate'] = (requestParameters.toDate as any).toISOString();
        }

        if (requestParameters.bankAccountId !== undefined) {
            queryParameters['bankAccountId'] = requestParameters.bankAccountId;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/payments`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(PaymentModelFromJSON));
    }

    /**
     */
    async paymentsGet(requestParameters: PaymentsGetRequest): Promise<Array<PaymentModel>> {
        const response = await this.paymentsGetRaw(requestParameters);
        return await response.value();
    }

    /**
     */
    async paymentsPaymentIdTagTagIdDeleteRaw(requestParameters: PaymentsPaymentIdTagTagIdDeleteRequest): Promise<runtime.ApiResponse<void>> {
        if (requestParameters.tagId === null || requestParameters.tagId === undefined) {
            throw new runtime.RequiredError('tagId','Required parameter requestParameters.tagId was null or undefined when calling paymentsPaymentIdTagTagIdDelete.');
        }

        if (requestParameters.paymentId === null || requestParameters.paymentId === undefined) {
            throw new runtime.RequiredError('paymentId','Required parameter requestParameters.paymentId was null or undefined when calling paymentsPaymentIdTagTagIdDelete.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/payments/{paymentId}/tag/{tagId}`.replace(`{${"tagId"}}`, encodeURIComponent(String(requestParameters.tagId))).replace(`{${"paymentId"}}`, encodeURIComponent(String(requestParameters.paymentId))),
            method: 'DELETE',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async paymentsPaymentIdTagTagIdDelete(requestParameters: PaymentsPaymentIdTagTagIdDeleteRequest): Promise<void> {
        await this.paymentsPaymentIdTagTagIdDeleteRaw(requestParameters);
    }

    /**
     */
    async paymentsPostRaw(requestParameters: PaymentsPostRequest): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/payments`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: PaymentModelToJSON(requestParameters.paymentModel),
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async paymentsPost(requestParameters: PaymentsPostRequest): Promise<void> {
        await this.paymentsPostRaw(requestParameters);
    }

    /**
     */
    async paymentsPutRaw(requestParameters: PaymentsPutRequest): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/payments`,
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: PaymentModelToJSON(requestParameters.paymentModel),
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async paymentsPut(requestParameters: PaymentsPutRequest): Promise<void> {
        await this.paymentsPutRaw(requestParameters);
    }

    /**
     */
    async paymentsTypesGetRaw(): Promise<runtime.ApiResponse<Array<PaymentTypeModel>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/payments/types`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(PaymentTypeModelFromJSON));
    }

    /**
     */
    async paymentsTypesGet(): Promise<Array<PaymentTypeModel>> {
        const response = await this.paymentsTypesGetRaw();
        return await response.value();
    }

}