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

import { exists, mapValues } from '../../runtime';
/**
 * 
 * @export
 * @class ComodityTradeHistoryModel
 */
export class ComodityTradeHistoryModel {
    /**
     * 
     * @type {number}
     * @memberof ComodityTradeHistoryModel
     */
    id?: number;
    /**
     * 
     * @type {Date}
     * @memberof ComodityTradeHistoryModel
     */
    tradeTimeStamp?: Date;
    /**
     * 
     * @type {number}
     * @memberof ComodityTradeHistoryModel
     */
    comodityTypeId?: number;
    /**
     * 
     * @type {string}
     * @memberof ComodityTradeHistoryModel
     */
    comodityType?: string | null;
    /**
     * 
     * @type {number}
     * @memberof ComodityTradeHistoryModel
     */
    comodityUnitId?: number;
    /**
     * 
     * @type {string}
     * @memberof ComodityTradeHistoryModel
     */
    comodityUnit?: string | null;
    /**
     * 
     * @type {number}
     * @memberof ComodityTradeHistoryModel
     */
    tradeSize?: number;
    /**
     * 
     * @type {number}
     * @memberof ComodityTradeHistoryModel
     */
    tradeValue?: number;
    /**
     * 
     * @type {number}
     * @memberof ComodityTradeHistoryModel
     */
    currencySymbolId?: number;
    /**
     * 
     * @type {string}
     * @memberof ComodityTradeHistoryModel
     */
    currencySymbol?: string | null;
    /**
     * 
     * @type {number}
     * @memberof ComodityTradeHistoryModel
     */
    userIdentityId?: number;
}

export function ComodityTradeHistoryModelFromJSON(json: any): ComodityTradeHistoryModel {
    return ComodityTradeHistoryModelFromJSONTyped(json, false);
}

export function ComodityTradeHistoryModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): ComodityTradeHistoryModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'tradeTimeStamp': !exists(json, 'tradeTimeStamp') ? undefined : (new Date(json['tradeTimeStamp'])),
        'comodityTypeId': !exists(json, 'comodityTypeId') ? undefined : json['comodityTypeId'],
        'comodityType': !exists(json, 'comodityType') ? undefined : json['comodityType'],
        'comodityUnitId': !exists(json, 'comodityUnitId') ? undefined : json['comodityUnitId'],
        'comodityUnit': !exists(json, 'comodityUnit') ? undefined : json['comodityUnit'],
        'tradeSize': !exists(json, 'tradeSize') ? undefined : json['tradeSize'],
        'tradeValue': !exists(json, 'tradeValue') ? undefined : json['tradeValue'],
        'currencySymbolId': !exists(json, 'currencySymbolId') ? undefined : json['currencySymbolId'],
        'currencySymbol': !exists(json, 'currencySymbol') ? undefined : json['currencySymbol'],
        'userIdentityId': !exists(json, 'userIdentityId') ? undefined : json['userIdentityId'],
    };
}

export function ComodityTradeHistoryModelToJSON(value?: ComodityTradeHistoryModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'tradeTimeStamp': value.tradeTimeStamp === undefined ? undefined : (value.tradeTimeStamp.toISOString()),
        'comodityTypeId': value.comodityTypeId,
        'comodityType': value.comodityType,
        'comodityUnitId': value.comodityUnitId,
        'comodityUnit': value.comodityUnit,
        'tradeSize': value.tradeSize,
        'tradeValue': value.tradeValue,
        'currencySymbolId': value.currencySymbolId,
        'currencySymbol': value.currencySymbol,
        'userIdentityId': value.userIdentityId,
    };
}