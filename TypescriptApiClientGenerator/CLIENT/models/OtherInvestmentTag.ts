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
    OtherInvestment,
    OtherInvestmentFromJSON,
    OtherInvestmentFromJSONTyped,
    OtherInvestmentToJSON,
} from './OtherInvestment';
import {
    Tag,
    TagFromJSON,
    TagFromJSONTyped,
    TagToJSON,
} from './Tag';

/**
 * 
 * @export
 * @class OtherInvestmentTag
 */
export class OtherInvestmentTag {
    /**
     * 
     * @type {number}
     * @memberof OtherInvestmentTag
     */
    id?: number;
    /**
     * 
     * @type {number}
     * @memberof OtherInvestmentTag
     */
    otherInvestmentId?: number;
    /**
     * 
     * @type {OtherInvestment}
     * @memberof OtherInvestmentTag
     */
    otherInvestment?: OtherInvestment;
    /**
     * 
     * @type {number}
     * @memberof OtherInvestmentTag
     */
    tagId?: number;
    /**
     * 
     * @type {Tag}
     * @memberof OtherInvestmentTag
     */
    tag?: Tag;
}

export function OtherInvestmentTagFromJSON(json: any): OtherInvestmentTag {
    return OtherInvestmentTagFromJSONTyped(json, false);
}

export function OtherInvestmentTagFromJSONTyped(json: any, ignoreDiscriminator: boolean): OtherInvestmentTag {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'otherInvestmentId': !exists(json, 'otherInvestmentId') ? undefined : json['otherInvestmentId'],
        'otherInvestment': !exists(json, 'otherInvestment') ? undefined : OtherInvestmentFromJSON(json['otherInvestment']),
        'tagId': !exists(json, 'tagId') ? undefined : json['tagId'],
        'tag': !exists(json, 'tag') ? undefined : TagFromJSON(json['tag']),
    };
}

export function OtherInvestmentTagToJSON(value?: OtherInvestmentTag | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'otherInvestmentId': value.otherInvestmentId,
        'otherInvestment': OtherInvestmentToJSON(value.otherInvestment),
        'tagId': value.tagId,
        'tag': TagToJSON(value.tag),
    };
}
