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
    BankAccount,
    BankAccountFromJSON,
    BankAccountFromJSONTyped,
    BankAccountToJSON,
} from './BankAccount';
import {
    PaymentCategory,
    PaymentCategoryFromJSON,
    PaymentCategoryFromJSONTyped,
    PaymentCategoryToJSON,
} from './PaymentCategory';
import {
    PaymentTag,
    PaymentTagFromJSON,
    PaymentTagFromJSONTyped,
    PaymentTagToJSON,
} from './PaymentTag';
import {
    PaymentType,
    PaymentTypeFromJSON,
    PaymentTypeFromJSONTyped,
    PaymentTypeToJSON,
} from './PaymentType';

/**
 * 
 * @export
 * @class Payment
 */
export class Payment {
    /**
     * 
     * @type {number}
     * @memberof Payment
     */
    id?: number;
    /**
     * 
     * @type {number}
     * @memberof Payment
     */
    amount?: number;
    /**
     * 
     * @type {string}
     * @memberof Payment
     */
    name?: string | null;
    /**
     * 
     * @type {string}
     * @memberof Payment
     */
    description?: string | null;
    /**
     * 
     * @type {Date}
     * @memberof Payment
     */
    date?: Date;
    /**
     * 
     * @type {number}
     * @memberof Payment
     */
    bankAccountId?: number;
    /**
     * 
     * @type {BankAccount}
     * @memberof Payment
     */
    bankAccount?: BankAccount;
    /**
     * 
     * @type {number}
     * @memberof Payment
     */
    paymentTypeId?: number;
    /**
     * 
     * @type {PaymentType}
     * @memberof Payment
     */
    paymentType?: PaymentType;
    /**
     * 
     * @type {number}
     * @memberof Payment
     */
    paymentCategoryId?: number;
    /**
     * 
     * @type {PaymentCategory}
     * @memberof Payment
     */
    paymentCategory?: PaymentCategory;
    /**
     * 
     * @type {Array<PaymentTag>}
     * @memberof Payment
     */
    paymentTags?: Array<PaymentTag> | null;
}

export function PaymentFromJSON(json: any): Payment {
    return PaymentFromJSONTyped(json, false);
}

export function PaymentFromJSONTyped(json: any, ignoreDiscriminator: boolean): Payment {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'amount': !exists(json, 'amount') ? undefined : json['amount'],
        'name': !exists(json, 'name') ? undefined : json['name'],
        'description': !exists(json, 'description') ? undefined : json['description'],
        'date': !exists(json, 'date') ? undefined : (new Date(json['date'])),
        'bankAccountId': !exists(json, 'bankAccountId') ? undefined : json['bankAccountId'],
        'bankAccount': !exists(json, 'bankAccount') ? undefined : BankAccountFromJSON(json['bankAccount']),
        'paymentTypeId': !exists(json, 'paymentTypeId') ? undefined : json['paymentTypeId'],
        'paymentType': !exists(json, 'paymentType') ? undefined : PaymentTypeFromJSON(json['paymentType']),
        'paymentCategoryId': !exists(json, 'paymentCategoryId') ? undefined : json['paymentCategoryId'],
        'paymentCategory': !exists(json, 'paymentCategory') ? undefined : PaymentCategoryFromJSON(json['paymentCategory']),
        'paymentTags': !exists(json, 'paymentTags') ? undefined : (json['paymentTags'] === null ? null : (json['paymentTags'] as Array<any>).map(PaymentTagFromJSON)),
    };
}

export function PaymentToJSON(value?: Payment | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'amount': value.amount,
        'name': value.name,
        'description': value.description,
        'date': value.date === undefined ? undefined : (value.date.toISOString()),
        'bankAccountId': value.bankAccountId,
        'bankAccount': BankAccountToJSON(value.bankAccount),
        'paymentTypeId': value.paymentTypeId,
        'paymentType': PaymentTypeToJSON(value.paymentType),
        'paymentCategoryId': value.paymentCategoryId,
        'paymentCategory': PaymentCategoryToJSON(value.paymentCategory),
        'paymentTags': value.paymentTags === undefined ? undefined : (value.paymentTags === null ? null : (value.paymentTags as Array<any>).map(PaymentTagToJSON)),
    };
}