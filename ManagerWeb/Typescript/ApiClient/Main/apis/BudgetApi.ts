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
    BudgetModel,
    BudgetModelFromJSON,
    BudgetModelToJSON,
} from '../models';

export interface BudgetsDeleteRequest {
    body?: number;
}

export interface BudgetsGetRequest {
    id?: number;
}

export interface BudgetsPostRequest {
    budgetModel?: BudgetModel;
}

export interface BudgetsPutRequest {
    budgetModel?: BudgetModel;
}

/**
 * BudgetApi - interface
 * 
 * @export
 * @interface BudgetApiInterface
 */
export interface BudgetApiInterface {
    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BudgetApiInterface
     */
    budgetsActualGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<BudgetModel>>>;

    /**
     */
    budgetsActualGet(initOverrides?: RequestInit): Promise<Array<BudgetModel>>;

    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BudgetApiInterface
     */
    budgetsAllGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<BudgetModel>>>;

    /**
     */
    budgetsAllGet(initOverrides?: RequestInit): Promise<Array<BudgetModel>>;

    /**
     * 
     * @param {number} [body] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BudgetApiInterface
     */
    budgetsDeleteRaw(requestParameters: BudgetsDeleteRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    budgetsDelete(requestParameters: BudgetsDeleteRequest, initOverrides?: RequestInit): Promise<void>;

    /**
     * 
     * @param {number} [id] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BudgetApiInterface
     */
    budgetsGetRaw(requestParameters: BudgetsGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<BudgetModel>>;

    /**
     */
    budgetsGet(requestParameters: BudgetsGetRequest, initOverrides?: RequestInit): Promise<BudgetModel>;

    /**
     * 
     * @param {BudgetModel} [budgetModel] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BudgetApiInterface
     */
    budgetsPostRaw(requestParameters: BudgetsPostRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    budgetsPost(requestParameters: BudgetsPostRequest, initOverrides?: RequestInit): Promise<void>;

    /**
     * 
     * @param {BudgetModel} [budgetModel] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BudgetApiInterface
     */
    budgetsPutRaw(requestParameters: BudgetsPutRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    budgetsPut(requestParameters: BudgetsPutRequest, initOverrides?: RequestInit): Promise<void>;

}

/**
 * 
 */
export class BudgetApi extends runtime.BaseAPI implements BudgetApiInterface {
    processPathParam(param: any): string {
        if (param instanceof Date)
            return encodeURIComponent(String(param.toISOString()));

        return encodeURIComponent(String(param));
    }

    /**
     */
    async budgetsActualGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<BudgetModel>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/budgets/actual`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(BudgetModelFromJSON));
    }

    /**
     */
    async budgetsActualGet(initOverrides?: RequestInit): Promise<Array<BudgetModel>> {
        const response = await this.budgetsActualGetRaw(initOverrides);
        return await response.value();
    }

    /**
     */
    async budgetsAllGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<BudgetModel>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/budgets/all`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(BudgetModelFromJSON));
    }

    /**
     */
    async budgetsAllGet(initOverrides?: RequestInit): Promise<Array<BudgetModel>> {
        const response = await this.budgetsAllGetRaw(initOverrides);
        return await response.value();
    }

    /**
     */
    async budgetsDeleteRaw(requestParameters: BudgetsDeleteRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/budgets`,
            method: 'DELETE',
            headers: headerParameters,
            query: queryParameters,
            body: requestParameters.body as any,
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async budgetsDelete(requestParameters: BudgetsDeleteRequest, initOverrides?: RequestInit): Promise<void> {
        await this.budgetsDeleteRaw(requestParameters, initOverrides);
    }

    /**
     */
    async budgetsGetRaw(requestParameters: BudgetsGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<BudgetModel>> {
        const queryParameters: any = {};

        if (requestParameters.id !== undefined) {
            queryParameters['id'] = requestParameters.id;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/budgets`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => BudgetModelFromJSON(jsonValue));
    }

    /**
     */
    async budgetsGet(requestParameters: BudgetsGetRequest, initOverrides?: RequestInit): Promise<BudgetModel> {
        const response = await this.budgetsGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async budgetsPostRaw(requestParameters: BudgetsPostRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/budgets`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: BudgetModelToJSON(requestParameters.budgetModel),
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async budgetsPost(requestParameters: BudgetsPostRequest, initOverrides?: RequestInit): Promise<void> {
        await this.budgetsPostRaw(requestParameters, initOverrides);
    }

    /**
     */
    async budgetsPutRaw(requestParameters: BudgetsPutRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/budgets`,
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: BudgetModelToJSON(requestParameters.budgetModel),
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async budgetsPut(requestParameters: BudgetsPutRequest, initOverrides?: RequestInit): Promise<void> {
        await this.budgetsPutRaw(requestParameters, initOverrides);
    }

}
