/* tslint:disable */
/* eslint-disable */
/**
 * BudgetManager.Api
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * The version of the OpenAPI document: 1.0
 * 
 *
 * NOTE: This class is auto generated by OpenAPI Generator (https://openapi-generator.tech).
 * https://openapi-generator.tech
 * Do not edit the class manually.
 */


import * as runtime from '../../runtime';
import {
    ComodityTradeHistoryModel,
    ComodityTradeHistoryModelFromJSON,
    ComodityTradeHistoryModelToJSON,
    ComodityTypeModel,
    ComodityTypeModelFromJSON,
    ComodityTypeModelToJSON,
    ComodityUnitModel,
    ComodityUnitModelFromJSON,
    ComodityUnitModelToJSON,
    ProblemDetails,
    ProblemDetailsFromJSON,
    ProblemDetailsToJSON,
} from '../models';

export interface V1ComoditiesDeleteRequest {
    body?: number;
}

export interface V1ComoditiesGoldActualPriceCurrencyCodeGetRequest {
    currencyCode: string;
}

export interface V1ComoditiesPostRequest {
    comodityTradeHistoryModel?: ComodityTradeHistoryModel;
}

export interface V1ComoditiesPutRequest {
    comodityTradeHistoryModel?: ComodityTradeHistoryModel;
}

/**
 * ComodityApi - interface
 * 
 * @export
 * @interface ComodityApiInterface
 */
export interface ComodityApiInterface {
    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof ComodityApiInterface
     */
    v1ComoditiesAllGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<ComodityTradeHistoryModel>>>;

    /**
     */
    v1ComoditiesAllGet(initOverrides?: RequestInit): Promise<Array<ComodityTradeHistoryModel>>;

    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof ComodityApiInterface
     */
    v1ComoditiesComodityTypeAllGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<ComodityTypeModel>>>;

    /**
     */
    v1ComoditiesComodityTypeAllGet(initOverrides?: RequestInit): Promise<Array<ComodityTypeModel>>;

    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof ComodityApiInterface
     */
    v1ComoditiesComodityUnitAllGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<ComodityUnitModel>>>;

    /**
     */
    v1ComoditiesComodityUnitAllGet(initOverrides?: RequestInit): Promise<Array<ComodityUnitModel>>;

    /**
     * 
     * @param {number} [body] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof ComodityApiInterface
     */
    v1ComoditiesDeleteRaw(requestParameters: V1ComoditiesDeleteRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    v1ComoditiesDelete(requestParameters: V1ComoditiesDeleteRequest, initOverrides?: RequestInit): Promise<void>;

    /**
     * 
     * @param {string} currencyCode 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof ComodityApiInterface
     */
    v1ComoditiesGoldActualPriceCurrencyCodeGetRaw(requestParameters: V1ComoditiesGoldActualPriceCurrencyCodeGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>>;

    /**
     */
    v1ComoditiesGoldActualPriceCurrencyCodeGet(requestParameters: V1ComoditiesGoldActualPriceCurrencyCodeGetRequest, initOverrides?: RequestInit): Promise<number>;

    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof ComodityApiInterface
     */
    v1ComoditiesGoldActualPriceGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>>;

    /**
     */
    v1ComoditiesGoldActualPriceGet(initOverrides?: RequestInit): Promise<number>;

    /**
     * 
     * @param {ComodityTradeHistoryModel} [comodityTradeHistoryModel] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof ComodityApiInterface
     */
    v1ComoditiesPostRaw(requestParameters: V1ComoditiesPostRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    v1ComoditiesPost(requestParameters: V1ComoditiesPostRequest, initOverrides?: RequestInit): Promise<void>;

    /**
     * 
     * @param {ComodityTradeHistoryModel} [comodityTradeHistoryModel] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof ComodityApiInterface
     */
    v1ComoditiesPutRaw(requestParameters: V1ComoditiesPutRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    v1ComoditiesPut(requestParameters: V1ComoditiesPutRequest, initOverrides?: RequestInit): Promise<void>;

}

/**
 * 
 */
export class ComodityApi extends runtime.BaseAPI implements ComodityApiInterface {
    processPathParam(param: any): string {
        if (param instanceof Date)
            return encodeURIComponent(String(param.toISOString()));

        return encodeURIComponent(String(param));
    }

    /**
     */
    async v1ComoditiesAllGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<ComodityTradeHistoryModel>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/v1/comodities/all`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(ComodityTradeHistoryModelFromJSON));
    }

    /**
     */
    async v1ComoditiesAllGet(initOverrides?: RequestInit): Promise<Array<ComodityTradeHistoryModel>> {
        const response = await this.v1ComoditiesAllGetRaw(initOverrides);
        return await response.value();
    }

    /**
     */
    async v1ComoditiesComodityTypeAllGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<ComodityTypeModel>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/v1/comodities/comodityType/all`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(ComodityTypeModelFromJSON));
    }

    /**
     */
    async v1ComoditiesComodityTypeAllGet(initOverrides?: RequestInit): Promise<Array<ComodityTypeModel>> {
        const response = await this.v1ComoditiesComodityTypeAllGetRaw(initOverrides);
        return await response.value();
    }

    /**
     */
    async v1ComoditiesComodityUnitAllGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<ComodityUnitModel>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/v1/comodities/comodityUnit/all`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(ComodityUnitModelFromJSON));
    }

    /**
     */
    async v1ComoditiesComodityUnitAllGet(initOverrides?: RequestInit): Promise<Array<ComodityUnitModel>> {
        const response = await this.v1ComoditiesComodityUnitAllGetRaw(initOverrides);
        return await response.value();
    }

    /**
     */
    async v1ComoditiesDeleteRaw(requestParameters: V1ComoditiesDeleteRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json; ver=1.0';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/v1/comodities`,
            method: 'DELETE',
            headers: headerParameters,
            query: queryParameters,
            body: requestParameters.body as any,
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async v1ComoditiesDelete(requestParameters: V1ComoditiesDeleteRequest, initOverrides?: RequestInit): Promise<void> {
        await this.v1ComoditiesDeleteRaw(requestParameters, initOverrides);
    }

    /**
     */
    async v1ComoditiesGoldActualPriceCurrencyCodeGetRaw(requestParameters: V1ComoditiesGoldActualPriceCurrencyCodeGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>> {
        if (requestParameters.currencyCode === null || requestParameters.currencyCode === undefined) {
            throw new runtime.RequiredError('currencyCode','Required parameter requestParameters.currencyCode was null or undefined when calling v1ComoditiesGoldActualPriceCurrencyCodeGet.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/v1/comodities/gold/actualPrice/{currencyCode}`.replace(`{${"currencyCode"}}`, this.processPathParam(requestParameters.currencyCode)),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.TextApiResponse(response) as any;
    }

    /**
     */
    async v1ComoditiesGoldActualPriceCurrencyCodeGet(requestParameters: V1ComoditiesGoldActualPriceCurrencyCodeGetRequest, initOverrides?: RequestInit): Promise<number> {
        const response = await this.v1ComoditiesGoldActualPriceCurrencyCodeGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async v1ComoditiesGoldActualPriceGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/v1/comodities/gold/actualPrice`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.TextApiResponse(response) as any;
    }

    /**
     */
    async v1ComoditiesGoldActualPriceGet(initOverrides?: RequestInit): Promise<number> {
        const response = await this.v1ComoditiesGoldActualPriceGetRaw(initOverrides);
        return await response.value();
    }

    /**
     */
    async v1ComoditiesPostRaw(requestParameters: V1ComoditiesPostRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json; ver=1.0';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/v1/comodities`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: ComodityTradeHistoryModelToJSON(requestParameters.comodityTradeHistoryModel),
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async v1ComoditiesPost(requestParameters: V1ComoditiesPostRequest, initOverrides?: RequestInit): Promise<void> {
        await this.v1ComoditiesPostRaw(requestParameters, initOverrides);
    }

    /**
     */
    async v1ComoditiesPutRaw(requestParameters: V1ComoditiesPutRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json; ver=1.0';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/v1/comodities`,
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: ComodityTradeHistoryModelToJSON(requestParameters.comodityTradeHistoryModel),
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async v1ComoditiesPut(requestParameters: V1ComoditiesPutRequest, initOverrides?: RequestInit): Promise<void> {
        await this.v1ComoditiesPutRaw(requestParameters, initOverrides);
    }

}
