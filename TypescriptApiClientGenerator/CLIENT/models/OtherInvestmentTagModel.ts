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
/**
 * 
 * @export
 * @class OtherInvestmentTagModel
 */
export class OtherInvestmentTagModel {
    /**
     * 
     * @type {number}
     * @memberof OtherInvestmentTagModel
     */
    id?: number | null;
    /**
     * 
     * @type {number}
     * @memberof OtherInvestmentTagModel
     */
    otherInvestmentId?: number;
    /**
     * 
     * @type {number}
     * @memberof OtherInvestmentTagModel
     */
    tagId?: number;
}

export function OtherInvestmentTagModelFromJSON(json: any): OtherInvestmentTagModel {
    return OtherInvestmentTagModelFromJSONTyped(json, false);
}

export function OtherInvestmentTagModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): OtherInvestmentTagModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'otherInvestmentId': !exists(json, 'otherInvestmentId') ? undefined : json['otherInvestmentId'],
        'tagId': !exists(json, 'tagId') ? undefined : json['tagId'],
    };
}

export function OtherInvestmentTagModelToJSON(value?: OtherInvestmentTagModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'otherInvestmentId': value.otherInvestmentId,
        'tagId': value.tagId,
    };
}
