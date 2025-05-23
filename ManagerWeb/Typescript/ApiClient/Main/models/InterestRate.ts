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

import { exists, mapValues } from '../../runtime';
import {
    BankAccount,
    BankAccountFromJSON,
    BankAccountFromJSONTyped,
    BankAccountToJSON,
} from './BankAccount';

/**
 * 
 * @export
 * @class InterestRate
 */
export class InterestRate {
    /**
     * 
     * @type {number}
     * @memberof InterestRate
     */
    id?: number;
    /**
     * 
     * @type {number}
     * @memberof InterestRate
     */
    rangeFrom?: number;
    /**
     * 
     * @type {number}
     * @memberof InterestRate
     */
    rangeTo?: number | null;
    /**
     * 
     * @type {number}
     * @memberof InterestRate
     */
    value?: number;
    /**
     * 
     * @type {number}
     * @memberof InterestRate
     */
    bankAccountId?: number;
    /**
     * 
     * @type {BankAccount}
     * @memberof InterestRate
     */
    bankAccount?: BankAccount;
    /**
     * 
     * @type {Date}
     * @memberof InterestRate
     */
    payoutDate?: Date;
}

export function InterestRateFromJSON(json: any): InterestRate {
    return InterestRateFromJSONTyped(json, false);
}

export function InterestRateFromJSONTyped(json: any, ignoreDiscriminator: boolean): InterestRate {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'rangeFrom': !exists(json, 'rangeFrom') ? undefined : json['rangeFrom'],
        'rangeTo': !exists(json, 'rangeTo') ? undefined : json['rangeTo'],
        'value': !exists(json, 'value') ? undefined : json['value'],
        'bankAccountId': !exists(json, 'bankAccountId') ? undefined : json['bankAccountId'],
        'bankAccount': !exists(json, 'bankAccount') ? undefined : BankAccountFromJSON(json['bankAccount']),
        'payoutDate': !exists(json, 'payoutDate') ? undefined : (new Date(json['payoutDate'])),
    };
}

export function InterestRateToJSON(value?: InterestRate | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'rangeFrom': value.rangeFrom,
        'rangeTo': value.rangeTo,
        'value': value.value,
        'bankAccountId': value.bankAccountId,
        'bankAccount': BankAccountToJSON(value.bankAccount),
        'payoutDate': value.payoutDate === undefined ? undefined : (value.payoutDate.toISOString()),
    };
}
