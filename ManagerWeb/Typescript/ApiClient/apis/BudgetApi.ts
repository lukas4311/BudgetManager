/* tslint:disable */
/* eslint-disable */
/**
 * ManagerWeb
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


import * as runtime from '../runtime';
import {
    BudgetModel,
    BudgetModelFromJSON,
    BudgetModelToJSON,
} from '../models';

export interface BudgetAddPostRequest {
    budgetModel?: BudgetModel;
}

export interface BudgetGetGetRequest {
    id?: number;
}

export interface BudgetUpdatePutRequest {
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
     * @param {BudgetModel} [budgetModel] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BudgetApiInterface
     */
    budgetAddPostRaw(requestParameters: BudgetAddPostRequest): Promise<runtime.ApiResponse<void>>;

    /**
     */
    budgetAddPost(requestParameters: BudgetAddPostRequest): Promise<void>;

    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BudgetApiInterface
     */
    budgetGetAllGetRaw(): Promise<runtime.ApiResponse<void>>;

    /**
     */
    budgetGetAllGet(): Promise<void>;

    /**
     * 
     * @param {number} [id] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BudgetApiInterface
     */
    budgetGetGetRaw(requestParameters: BudgetGetGetRequest): Promise<runtime.ApiResponse<void>>;

    /**
     */
    budgetGetGet(requestParameters: BudgetGetGetRequest): Promise<void>;

    /**
     * 
     * @param {BudgetModel} [budgetModel] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof BudgetApiInterface
     */
    budgetUpdatePutRaw(requestParameters: BudgetUpdatePutRequest): Promise<runtime.ApiResponse<void>>;

    /**
     */
    budgetUpdatePut(requestParameters: BudgetUpdatePutRequest): Promise<void>;

}

/**
 * 
 */
export class BudgetApi extends runtime.BaseAPI implements BudgetApiInterface {

    /**
     */
    async budgetAddPostRaw(requestParameters: BudgetAddPostRequest): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        const response = await this.request({
            path: `/budget/add`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: BudgetModelToJSON(requestParameters.budgetModel),
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async budgetAddPost(requestParameters: BudgetAddPostRequest): Promise<void> {
        await this.budgetAddPostRaw(requestParameters);
    }

    /**
     */
    async budgetGetAllGetRaw(): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/budget/getAll`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async budgetGetAllGet(): Promise<void> {
        await this.budgetGetAllGetRaw();
    }

    /**
     */
    async budgetGetGetRaw(requestParameters: BudgetGetGetRequest): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        if (requestParameters.id !== undefined) {
            queryParameters['id'] = requestParameters.id;
        }

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/budget/get`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async budgetGetGet(requestParameters: BudgetGetGetRequest): Promise<void> {
        await this.budgetGetGetRaw(requestParameters);
    }

    /**
     */
    async budgetUpdatePutRaw(requestParameters: BudgetUpdatePutRequest): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        const response = await this.request({
            path: `/budget/update`,
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: BudgetModelToJSON(requestParameters.budgetModel),
        });

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async budgetUpdatePut(requestParameters: BudgetUpdatePutRequest): Promise<void> {
        await this.budgetUpdatePutRaw(requestParameters);
    }

}