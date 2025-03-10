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

/**
 * 
 * @export
 * @enum {string}
 */
export enum CurrencySymbol {
    Usd = 'USD',
    Czk = 'CZK',
    Gbp = 'GBP',
    Eur = 'EUR',
    Jpy = 'JPY'
}

export function CurrencySymbolFromJSON(json: any): CurrencySymbol {
    return CurrencySymbolFromJSONTyped(json, false);
}

export function CurrencySymbolFromJSONTyped(json: any, ignoreDiscriminator: boolean): CurrencySymbol {
    return json as CurrencySymbol;
}

export function CurrencySymbolToJSON(value?: CurrencySymbol | null): any {
    return value as any;
}

