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
import {
    OtherInvestment,
    OtherInvestmentFromJSON,
    OtherInvestmentFromJSONTyped,
    OtherInvestmentToJSON,
} from './OtherInvestment';

/**
 * 
 * @export
 * @class OtherInvestmentBalaceHistory
 */
export class OtherInvestmentBalaceHistory {
    /**
     * 
     * @type {number}
     * @memberof OtherInvestmentBalaceHistory
     */
    id?: number;
    /**
     * 
     * @type {Date}
     * @memberof OtherInvestmentBalaceHistory
     */
    date?: Date;
    /**
     * 
     * @type {number}
     * @memberof OtherInvestmentBalaceHistory
     */
    balance?: number;
    /**
     * 
     * @type {number}
     * @memberof OtherInvestmentBalaceHistory
     */
    otherInvestmentId?: number;
    /**
     * 
     * @type {OtherInvestment}
     * @memberof OtherInvestmentBalaceHistory
     */
    otherInvestment?: OtherInvestment;
}

export function OtherInvestmentBalaceHistoryFromJSON(json: any): OtherInvestmentBalaceHistory {
    return OtherInvestmentBalaceHistoryFromJSONTyped(json, false);
}

export function OtherInvestmentBalaceHistoryFromJSONTyped(json: any, ignoreDiscriminator: boolean): OtherInvestmentBalaceHistory {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'date': !exists(json, 'date') ? undefined : (new Date(json['date'])),
        'balance': !exists(json, 'balance') ? undefined : json['balance'],
        'otherInvestmentId': !exists(json, 'otherInvestmentId') ? undefined : json['otherInvestmentId'],
        'otherInvestment': !exists(json, 'otherInvestment') ? undefined : OtherInvestmentFromJSON(json['otherInvestment']),
    };
}

export function OtherInvestmentBalaceHistoryToJSON(value?: OtherInvestmentBalaceHistory | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'date': value.date === undefined ? undefined : (value.date.toISOString()),
        'balance': value.balance,
        'otherInvestmentId': value.otherInvestmentId,
        'otherInvestment': OtherInvestmentToJSON(value.otherInvestment),
    };
}