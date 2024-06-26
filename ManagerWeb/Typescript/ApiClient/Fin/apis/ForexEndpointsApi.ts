/* tslint:disable */
/* eslint-disable */
/**
 * BudgetManager.FinancialApi
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
    CurrencySymbol,
    CurrencySymbolFromJSON,
    CurrencySymbolToJSON,
} from '../models';

export interface GetForexPairPriceRequest {
    from: CurrencySymbol;
    to: CurrencySymbol;
}

export interface GetForexPairPriceAtDateRequest {
    from: CurrencySymbol;
    to: CurrencySymbol;
    date: Date;
}

/**
 * ForexEndpointsApi - interface
 * 
 * @export
 * @interface ForexEndpointsApiInterface
 */
export interface ForexEndpointsApiInterface {
    /**
     * 
     * @param {CurrencySymbol} from 
     * @param {CurrencySymbol} to 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof ForexEndpointsApiInterface
     */
    getForexPairPriceRaw(requestParameters: GetForexPairPriceRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>>;

    /**
     */
    getForexPairPrice(requestParameters: GetForexPairPriceRequest, initOverrides?: RequestInit): Promise<number>;

    /**
     * 
     * @param {CurrencySymbol} from 
     * @param {CurrencySymbol} to 
     * @param {Date} date 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof ForexEndpointsApiInterface
     */
    getForexPairPriceAtDateRaw(requestParameters: GetForexPairPriceAtDateRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>>;

    /**
     */
    getForexPairPriceAtDate(requestParameters: GetForexPairPriceAtDateRequest, initOverrides?: RequestInit): Promise<number>;

}

/**
 * 
 */
export class ForexEndpointsApi extends runtime.BaseAPI implements ForexEndpointsApiInterface {
    processPathParam(param: any): string {
        if (param instanceof Date)
            return encodeURIComponent(String(param.toISOString()));

        return encodeURIComponent(String(param));
    }

    /**
     */
    async getForexPairPriceRaw(requestParameters: GetForexPairPriceRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>> {
        if (requestParameters.from === null || requestParameters.from === undefined) {
            throw new runtime.RequiredError('from','Required parameter requestParameters.from was null or undefined when calling getForexPairPrice.');
        }

        if (requestParameters.to === null || requestParameters.to === undefined) {
            throw new runtime.RequiredError('to','Required parameter requestParameters.to was null or undefined when calling getForexPairPrice.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/forex/exchangerate/{from}/{to}`.replace(`{${"from"}}`, this.processPathParam(requestParameters.from)).replace(`{${"to"}}`, this.processPathParam(requestParameters.to)),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.TextApiResponse(response) as any;
    }

    /**
     */
    async getForexPairPrice(requestParameters: GetForexPairPriceRequest, initOverrides?: RequestInit): Promise<number> {
        const response = await this.getForexPairPriceRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async getForexPairPriceAtDateRaw(requestParameters: GetForexPairPriceAtDateRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>> {
        if (requestParameters.from === null || requestParameters.from === undefined) {
            throw new runtime.RequiredError('from','Required parameter requestParameters.from was null or undefined when calling getForexPairPriceAtDate.');
        }

        if (requestParameters.to === null || requestParameters.to === undefined) {
            throw new runtime.RequiredError('to','Required parameter requestParameters.to was null or undefined when calling getForexPairPriceAtDate.');
        }

        if (requestParameters.date === null || requestParameters.date === undefined) {
            throw new runtime.RequiredError('date','Required parameter requestParameters.date was null or undefined when calling getForexPairPriceAtDate.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        const response = await this.request({
            path: `/forex/exchangerate/{from}/{to}/{date}`.replace(`{${"from"}}`, this.processPathParam(requestParameters.from)).replace(`{${"to"}}`, this.processPathParam(requestParameters.to)).replace(`{${"date"}}`, this.processPathParam(requestParameters.date)),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.TextApiResponse(response) as any;
    }

    /**
     */
    async getForexPairPriceAtDate(requestParameters: GetForexPairPriceAtDateRequest, initOverrides?: RequestInit): Promise<number> {
        const response = await this.getForexPairPriceAtDateRaw(requestParameters, initOverrides);
        return await response.value();
    }

}
