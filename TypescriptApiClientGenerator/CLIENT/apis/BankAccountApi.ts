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


import * as runtime from '../../runtime';
import {
    BankAccountModel,
    BankAccountModelFromJSON,
    BankAccountModelToJSON,
    BankBalanceModel,
    BankBalanceModelFromJSON,
    BankBalanceModelToJSON,
} from '../models';

export interface BankAccountsAllBalanceToDateGetRequest {
    toDate: Date | null;
}

export interface BankAccountsBankAccountIdBalanceToDateGetRequest {
    bankAccountId: number;
    toDate: Date | null;
}

export interface BankAccountsDeleteRequest {
    body?: number;
}

export interface BankAccountsPostRequest {
    bankAccountModel?: BankAccountModel;
}

export interface BankAccountsPutRequest {
    bankAccountModel?: BankAccountModel;
}

/**
 * BankAccountApi - interface
 * 
 * @export
 * @interface BankAccountApiInterface
 */
export interface BankAccountApiInterface {
    /**
     * 
     * @param {Date} toDate 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BankAccountApiInterface
     */
    bankAccountsAllBalanceToDateGetRaw(requestParameters: BankAccountsAllBalanceToDateGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<BankBalanceModel>>>;

    /**
     */
    bankAccountsAllBalanceToDateGet(requestParameters: BankAccountsAllBalanceToDateGetRequest, initOverrides?: RequestInit): Promise<Array<BankBalanceModel>>;

    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BankAccountApiInterface
     */
    bankAccountsAllGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<BankAccountModel>>>;

    /**
     */
    bankAccountsAllGet(initOverrides?: RequestInit): Promise<Array<BankAccountModel>>;

    /**
     * 
     * @param {number} bankAccountId 
     * @param {Date} toDate 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BankAccountApiInterface
     */
    bankAccountsBankAccountIdBalanceToDateGetRaw(requestParameters: BankAccountsBankAccountIdBalanceToDateGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<BankBalanceModel>>;

    /**
     */
    bankAccountsBankAccountIdBalanceToDateGet(requestParameters: BankAccountsBankAccountIdBalanceToDateGetRequest, initOverrides?: RequestInit): Promise<BankBalanceModel>;

    /**
     * 
     * @param {number} [body] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BankAccountApiInterface
     */
    bankAccountsDeleteRaw(requestParameters: BankAccountsDeleteRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    bankAccountsDelete(requestParameters: BankAccountsDeleteRequest, initOverrides?: RequestInit): Promise<void>;

    /**
     * 
     * @param {BankAccountModel} [bankAccountModel] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BankAccountApiInterface
     */
    bankAccountsPostRaw(requestParameters: BankAccountsPostRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    bankAccountsPost(requestParameters: BankAccountsPostRequest, initOverrides?: RequestInit): Promise<void>;

    /**
     * 
     * @param {BankAccountModel} [bankAccountModel] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BankAccountApiInterface
     */
    bankAccountsPutRaw(requestParameters: BankAccountsPutRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    bankAccountsPut(requestParameters: BankAccountsPutRequest, initOverrides?: RequestInit): Promise<void>;

}

/**
 * 
 */
export class BankAccountApi extends runtime.BaseAPI implements BankAccountApiInterface {
    processPathParam(param: any): string {
        if (param instanceof Date)
            return encodeURIComponent(String(param.toISOString()));

        return encodeURIComponent(String(param));
    }

    /**
     */
    async bankAccountsAllBalanceToDateGetRaw(requestParameters: BankAccountsAllBalanceToDateGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<BankBalanceModel>>> {
        if (requestParameters.toDate === null || requestParameters.toDate === undefined) {
            throw new runtime.RequiredError('toDate','Required parameter requestParameters.toDate was null or undefined when calling bankAccountsAllBalanceToDateGet.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/bankAccounts/all/balance/{toDate}`.replace(`{${"toDate"}}`, this.processPathParam(requestParameters.toDate)),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(BankBalanceModelFromJSON));
    }

    /**
     */
    async bankAccountsAllBalanceToDateGet(requestParameters: BankAccountsAllBalanceToDateGetRequest, initOverrides?: RequestInit): Promise<Array<BankBalanceModel>> {
        const response = await this.bankAccountsAllBalanceToDateGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async bankAccountsAllGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<BankAccountModel>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/bankAccounts/all`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(BankAccountModelFromJSON));
    }

    /**
     */
    async bankAccountsAllGet(initOverrides?: RequestInit): Promise<Array<BankAccountModel>> {
        const response = await this.bankAccountsAllGetRaw(initOverrides);
        return await response.value();
    }

    /**
     */
    async bankAccountsBankAccountIdBalanceToDateGetRaw(requestParameters: BankAccountsBankAccountIdBalanceToDateGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<BankBalanceModel>> {
        if (requestParameters.bankAccountId === null || requestParameters.bankAccountId === undefined) {
            throw new runtime.RequiredError('bankAccountId','Required parameter requestParameters.bankAccountId was null or undefined when calling bankAccountsBankAccountIdBalanceToDateGet.');
        }

        if (requestParameters.toDate === null || requestParameters.toDate === undefined) {
            throw new runtime.RequiredError('toDate','Required parameter requestParameters.toDate was null or undefined when calling bankAccountsBankAccountIdBalanceToDateGet.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/bankAccounts/{bankAccountId}/balance/{toDate}`.replace(`{${"bankAccountId"}}`, this.processPathParam(requestParameters.bankAccountId)).replace(`{${"toDate"}}`, this.processPathParam(requestParameters.toDate)),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => BankBalanceModelFromJSON(jsonValue));
    }

    /**
     */
    async bankAccountsBankAccountIdBalanceToDateGet(requestParameters: BankAccountsBankAccountIdBalanceToDateGetRequest, initOverrides?: RequestInit): Promise<BankBalanceModel> {
        const response = await this.bankAccountsBankAccountIdBalanceToDateGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async bankAccountsDeleteRaw(requestParameters: BankAccountsDeleteRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/bankAccounts`,
            method: 'DELETE',
            headers: headerParameters,
            query: queryParameters,
            body: requestParameters.body as any,
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async bankAccountsDelete(requestParameters: BankAccountsDeleteRequest, initOverrides?: RequestInit): Promise<void> {
        await this.bankAccountsDeleteRaw(requestParameters, initOverrides);
    }

    /**
     */
    async bankAccountsPostRaw(requestParameters: BankAccountsPostRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/bankAccounts`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: BankAccountModelToJSON(requestParameters.bankAccountModel),
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async bankAccountsPost(requestParameters: BankAccountsPostRequest, initOverrides?: RequestInit): Promise<void> {
        await this.bankAccountsPostRaw(requestParameters, initOverrides);
    }

    /**
     */
    async bankAccountsPutRaw(requestParameters: BankAccountsPutRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/bankAccounts`,
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: BankAccountModelToJSON(requestParameters.bankAccountModel),
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async bankAccountsPut(requestParameters: BankAccountsPutRequest, initOverrides?: RequestInit): Promise<void> {
        await this.bankAccountsPutRaw(requestParameters, initOverrides);
    }

}