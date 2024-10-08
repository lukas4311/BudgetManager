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
    CryptoTickerModel,
    CryptoTickerModelFromJSON,
    CryptoTickerModelToJSON,
    TradeHistory,
    TradeHistoryFromJSON,
    TradeHistoryToJSON,
    TradesGroupedMonth,
    TradesGroupedMonthFromJSON,
    TradesGroupedMonthToJSON,
} from '../models';

export interface CryptosActualExchangeRateFromCurrencyToCurrencyGetRequest {
    fromCurrency: string;
    toCurrency: string;
}

export interface CryptosBrokerReportBrokerIdPostRequest {
    brokerId: number;
    file?: Blob;
}

export interface CryptosDeleteRequest {
    body?: number;
}

export interface CryptosExchangeRateFromCurrencyToCurrencyAtDateGetRequest {
    fromCurrency: string;
    toCurrency: string;
    atDate: Date;
}

export interface CryptosPostRequest {
    tradeHistory?: TradeHistory;
}

export interface CryptosPutRequest {
    tradeHistory?: TradeHistory;
}

export interface CryptosTradeDetailTradeIdGetRequest {
    tradeId: number;
}

/**
 * CryptoApi - interface
 * 
 * @export
 * @interface CryptoApiInterface
 */
export interface CryptoApiInterface {
    /**
     * 
     * @param {string} fromCurrency 
     * @param {string} toCurrency 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof CryptoApiInterface
     */
    cryptosActualExchangeRateFromCurrencyToCurrencyGetRaw(requestParameters: CryptosActualExchangeRateFromCurrencyToCurrencyGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>>;

    /**
     */
    cryptosActualExchangeRateFromCurrencyToCurrencyGet(requestParameters: CryptosActualExchangeRateFromCurrencyToCurrencyGetRequest, initOverrides?: RequestInit): Promise<number>;

    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof CryptoApiInterface
     */
    cryptosAllGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<TradeHistory>>>;

    /**
     */
    cryptosAllGet(initOverrides?: RequestInit): Promise<Array<TradeHistory>>;

    /**
     * 
     * @param {number} brokerId 
     * @param {Blob} [file] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof CryptoApiInterface
     */
    cryptosBrokerReportBrokerIdPostRaw(requestParameters: CryptosBrokerReportBrokerIdPostRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    cryptosBrokerReportBrokerIdPost(requestParameters: CryptosBrokerReportBrokerIdPostRequest, initOverrides?: RequestInit): Promise<void>;

    /**
     * 
     * @param {number} [body] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof CryptoApiInterface
     */
    cryptosDeleteRaw(requestParameters: CryptosDeleteRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    cryptosDelete(requestParameters: CryptosDeleteRequest, initOverrides?: RequestInit): Promise<void>;

    /**
     * 
     * @param {string} fromCurrency 
     * @param {string} toCurrency 
     * @param {Date} atDate 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof CryptoApiInterface
     */
    cryptosExchangeRateFromCurrencyToCurrencyAtDateGetRaw(requestParameters: CryptosExchangeRateFromCurrencyToCurrencyAtDateGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>>;

    /**
     */
    cryptosExchangeRateFromCurrencyToCurrencyAtDateGet(requestParameters: CryptosExchangeRateFromCurrencyToCurrencyAtDateGetRequest, initOverrides?: RequestInit): Promise<number>;

    /**
     * 
     * @param {TradeHistory} [tradeHistory] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof CryptoApiInterface
     */
    cryptosPostRaw(requestParameters: CryptosPostRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    cryptosPost(requestParameters: CryptosPostRequest, initOverrides?: RequestInit): Promise<void>;

    /**
     * 
     * @param {TradeHistory} [tradeHistory] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof CryptoApiInterface
     */
    cryptosPutRaw(requestParameters: CryptosPutRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    cryptosPut(requestParameters: CryptosPutRequest, initOverrides?: RequestInit): Promise<void>;

    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof CryptoApiInterface
     */
    cryptosTickersGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<CryptoTickerModel>>>;

    /**
     */
    cryptosTickersGet(initOverrides?: RequestInit): Promise<Array<CryptoTickerModel>>;

    /**
     * 
     * @param {number} tradeId 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof CryptoApiInterface
     */
    cryptosTradeDetailTradeIdGetRaw(requestParameters: CryptosTradeDetailTradeIdGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<TradeHistory>>;

    /**
     */
    cryptosTradeDetailTradeIdGet(requestParameters: CryptosTradeDetailTradeIdGetRequest, initOverrides?: RequestInit): Promise<TradeHistory>;

    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof CryptoApiInterface
     */
    cryptosTradeMonthlygroupedGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<TradesGroupedMonth>>>;

    /**
     */
    cryptosTradeMonthlygroupedGet(initOverrides?: RequestInit): Promise<Array<TradesGroupedMonth>>;

    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof CryptoApiInterface
     */
    cryptosTradeTickergroupedGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<TradesGroupedMonth>>>;

    /**
     */
    cryptosTradeTickergroupedGet(initOverrides?: RequestInit): Promise<Array<TradesGroupedMonth>>;

    /**
     * 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof CryptoApiInterface
     */
    cryptosTradeTradedategroupedGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<TradesGroupedMonth>>>;

    /**
     */
    cryptosTradeTradedategroupedGet(initOverrides?: RequestInit): Promise<Array<TradesGroupedMonth>>;

}

/**
 * 
 */
export class CryptoApi extends runtime.BaseAPI implements CryptoApiInterface {
    processPathParam(param: any): string {
        if (param instanceof Date)
            return encodeURIComponent(String(param.toISOString()));

        return encodeURIComponent(String(param));
    }

    /**
     */
    async cryptosActualExchangeRateFromCurrencyToCurrencyGetRaw(requestParameters: CryptosActualExchangeRateFromCurrencyToCurrencyGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>> {
        if (requestParameters.fromCurrency === null || requestParameters.fromCurrency === undefined) {
            throw new runtime.RequiredError('fromCurrency','Required parameter requestParameters.fromCurrency was null or undefined when calling cryptosActualExchangeRateFromCurrencyToCurrencyGet.');
        }

        if (requestParameters.toCurrency === null || requestParameters.toCurrency === undefined) {
            throw new runtime.RequiredError('toCurrency','Required parameter requestParameters.toCurrency was null or undefined when calling cryptosActualExchangeRateFromCurrencyToCurrencyGet.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/cryptos/actualExchangeRate/{fromCurrency}/{toCurrency}`.replace(`{${"fromCurrency"}}`, this.processPathParam(requestParameters.fromCurrency)).replace(`{${"toCurrency"}}`, this.processPathParam(requestParameters.toCurrency)),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.TextApiResponse(response) as any;
    }

    /**
     */
    async cryptosActualExchangeRateFromCurrencyToCurrencyGet(requestParameters: CryptosActualExchangeRateFromCurrencyToCurrencyGetRequest, initOverrides?: RequestInit): Promise<number> {
        const response = await this.cryptosActualExchangeRateFromCurrencyToCurrencyGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async cryptosAllGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<TradeHistory>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/cryptos/all`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(TradeHistoryFromJSON));
    }

    /**
     */
    async cryptosAllGet(initOverrides?: RequestInit): Promise<Array<TradeHistory>> {
        const response = await this.cryptosAllGetRaw(initOverrides);
        return await response.value();
    }

    /**
     */
    async cryptosBrokerReportBrokerIdPostRaw(requestParameters: CryptosBrokerReportBrokerIdPostRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        if (requestParameters.brokerId === null || requestParameters.brokerId === undefined) {
            throw new runtime.RequiredError('brokerId','Required parameter requestParameters.brokerId was null or undefined when calling cryptosBrokerReportBrokerIdPost.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const consumes: runtime.Consume[] = [
            { contentType: 'multipart/form-data' },
        ];
        // @ts-ignore: canConsumeForm may be unused
        const canConsumeForm = runtime.canConsumeForm(consumes);

        let formParams: { append(param: string, value: any): any };
        let useForm = false;
        // use FormData to transmit files using content-type "multipart/form-data"
        useForm = canConsumeForm;
        if (useForm) {
            formParams = new FormData();
        } else {
            formParams = new URLSearchParams();
        }

        if (requestParameters.file !== undefined) {
            formParams.append('file', requestParameters.file as any);
        }

        const response = await this.request({
            path: `/cryptos/brokerReport/{brokerId}`.replace(`{${"brokerId"}}`, this.processPathParam(requestParameters.brokerId)),
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: formParams,
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async cryptosBrokerReportBrokerIdPost(requestParameters: CryptosBrokerReportBrokerIdPostRequest, initOverrides?: RequestInit): Promise<void> {
        await this.cryptosBrokerReportBrokerIdPostRaw(requestParameters, initOverrides);
    }

    /**
     */
    async cryptosDeleteRaw(requestParameters: CryptosDeleteRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/cryptos`,
            method: 'DELETE',
            headers: headerParameters,
            query: queryParameters,
            body: requestParameters.body as any,
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async cryptosDelete(requestParameters: CryptosDeleteRequest, initOverrides?: RequestInit): Promise<void> {
        await this.cryptosDeleteRaw(requestParameters, initOverrides);
    }

    /**
     */
    async cryptosExchangeRateFromCurrencyToCurrencyAtDateGetRaw(requestParameters: CryptosExchangeRateFromCurrencyToCurrencyAtDateGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<number>> {
        if (requestParameters.fromCurrency === null || requestParameters.fromCurrency === undefined) {
            throw new runtime.RequiredError('fromCurrency','Required parameter requestParameters.fromCurrency was null or undefined when calling cryptosExchangeRateFromCurrencyToCurrencyAtDateGet.');
        }

        if (requestParameters.toCurrency === null || requestParameters.toCurrency === undefined) {
            throw new runtime.RequiredError('toCurrency','Required parameter requestParameters.toCurrency was null or undefined when calling cryptosExchangeRateFromCurrencyToCurrencyAtDateGet.');
        }

        if (requestParameters.atDate === null || requestParameters.atDate === undefined) {
            throw new runtime.RequiredError('atDate','Required parameter requestParameters.atDate was null or undefined when calling cryptosExchangeRateFromCurrencyToCurrencyAtDateGet.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/cryptos/exchangeRate/{fromCurrency}/{toCurrency}/{atDate}`.replace(`{${"fromCurrency"}}`, this.processPathParam(requestParameters.fromCurrency)).replace(`{${"toCurrency"}}`, this.processPathParam(requestParameters.toCurrency)).replace(`{${"atDate"}}`, this.processPathParam(requestParameters.atDate)),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.TextApiResponse(response) as any;
    }

    /**
     */
    async cryptosExchangeRateFromCurrencyToCurrencyAtDateGet(requestParameters: CryptosExchangeRateFromCurrencyToCurrencyAtDateGetRequest, initOverrides?: RequestInit): Promise<number> {
        const response = await this.cryptosExchangeRateFromCurrencyToCurrencyAtDateGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async cryptosPostRaw(requestParameters: CryptosPostRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/cryptos`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: TradeHistoryToJSON(requestParameters.tradeHistory),
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async cryptosPost(requestParameters: CryptosPostRequest, initOverrides?: RequestInit): Promise<void> {
        await this.cryptosPostRaw(requestParameters, initOverrides);
    }

    /**
     */
    async cryptosPutRaw(requestParameters: CryptosPutRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/cryptos`,
            method: 'PUT',
            headers: headerParameters,
            query: queryParameters,
            body: TradeHistoryToJSON(requestParameters.tradeHistory),
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async cryptosPut(requestParameters: CryptosPutRequest, initOverrides?: RequestInit): Promise<void> {
        await this.cryptosPutRaw(requestParameters, initOverrides);
    }

    /**
     */
    async cryptosTickersGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<CryptoTickerModel>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/cryptos/tickers`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(CryptoTickerModelFromJSON));
    }

    /**
     */
    async cryptosTickersGet(initOverrides?: RequestInit): Promise<Array<CryptoTickerModel>> {
        const response = await this.cryptosTickersGetRaw(initOverrides);
        return await response.value();
    }

    /**
     */
    async cryptosTradeDetailTradeIdGetRaw(requestParameters: CryptosTradeDetailTradeIdGetRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<TradeHistory>> {
        if (requestParameters.tradeId === null || requestParameters.tradeId === undefined) {
            throw new runtime.RequiredError('tradeId','Required parameter requestParameters.tradeId was null or undefined when calling cryptosTradeDetailTradeIdGet.');
        }

        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/cryptos/tradeDetail/{tradeId}`.replace(`{${"tradeId"}}`, this.processPathParam(requestParameters.tradeId)),
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => TradeHistoryFromJSON(jsonValue));
    }

    /**
     */
    async cryptosTradeDetailTradeIdGet(requestParameters: CryptosTradeDetailTradeIdGetRequest, initOverrides?: RequestInit): Promise<TradeHistory> {
        const response = await this.cryptosTradeDetailTradeIdGetRaw(requestParameters, initOverrides);
        return await response.value();
    }

    /**
     */
    async cryptosTradeMonthlygroupedGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<TradesGroupedMonth>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/cryptos/trade/monthlygrouped`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(TradesGroupedMonthFromJSON));
    }

    /**
     */
    async cryptosTradeMonthlygroupedGet(initOverrides?: RequestInit): Promise<Array<TradesGroupedMonth>> {
        const response = await this.cryptosTradeMonthlygroupedGetRaw(initOverrides);
        return await response.value();
    }

    /**
     */
    async cryptosTradeTickergroupedGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<TradesGroupedMonth>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/cryptos/trade/tickergrouped`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(TradesGroupedMonthFromJSON));
    }

    /**
     */
    async cryptosTradeTickergroupedGet(initOverrides?: RequestInit): Promise<Array<TradesGroupedMonth>> {
        const response = await this.cryptosTradeTickergroupedGetRaw(initOverrides);
        return await response.value();
    }

    /**
     */
    async cryptosTradeTradedategroupedGetRaw(initOverrides?: RequestInit): Promise<runtime.ApiResponse<Array<TradesGroupedMonth>>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        if (this.configuration && this.configuration.apiKey) {
            headerParameters["Authorization"] = this.configuration.apiKey("Authorization"); // Bearer authentication
        }

        const response = await this.request({
            path: `/cryptos/trade/tradedategrouped`,
            method: 'GET',
            headers: headerParameters,
            query: queryParameters,
        }, initOverrides);

        return new runtime.JSONApiResponse(response, (jsonValue) => jsonValue.map(TradesGroupedMonthFromJSON));
    }

    /**
     */
    async cryptosTradeTradedategroupedGet(initOverrides?: RequestInit): Promise<Array<TradesGroupedMonth>> {
        const response = await this.cryptosTradeTradedategroupedGetRaw(initOverrides);
        return await response.value();
    }

}
