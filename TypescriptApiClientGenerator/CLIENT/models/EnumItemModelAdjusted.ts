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
 * @class EnumItemModelAdjusted
 */
export class EnumItemModelAdjusted {
    /**
     * 
     * @type {number}
     * @memberof EnumItemModelAdjusted
     */
    id?: number | null;
    /**
     * 
     * @type {string}
     * @memberof EnumItemModelAdjusted
     */
    code?: string | null;
    /**
     * 
     * @type {string}
     * @memberof EnumItemModelAdjusted
     */
    name?: string | null;
    /**
     * 
     * @type {number}
     * @memberof EnumItemModelAdjusted
     */
    enumItemTypeId?: number;
    /**
     * 
     * @type {string}
     * @memberof EnumItemModelAdjusted
     */
    enumItemTypeCode?: string | null;
    /**
     * 
     * @type {string}
     * @memberof EnumItemModelAdjusted
     */
    enumItemTypeName?: string | null;
    /**
     * 
     * @type {string}
     * @memberof EnumItemModelAdjusted
     */
    metadata?: string | null;
}

export function EnumItemModelAdjustedFromJSON(json: any): EnumItemModelAdjusted {
    return EnumItemModelAdjustedFromJSONTyped(json, false);
}

export function EnumItemModelAdjustedFromJSONTyped(json: any, ignoreDiscriminator: boolean): EnumItemModelAdjusted {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'code': !exists(json, 'code') ? undefined : json['code'],
        'name': !exists(json, 'name') ? undefined : json['name'],
        'enumItemTypeId': !exists(json, 'enumItemTypeId') ? undefined : json['enumItemTypeId'],
        'enumItemTypeCode': !exists(json, 'enumItemTypeCode') ? undefined : json['enumItemTypeCode'],
        'enumItemTypeName': !exists(json, 'enumItemTypeName') ? undefined : json['enumItemTypeName'],
        'metadata': !exists(json, 'metadata') ? undefined : json['metadata'],
    };
}

export function EnumItemModelAdjustedToJSON(value?: EnumItemModelAdjusted | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'code': value.code,
        'name': value.name,
        'enumItemTypeId': value.enumItemTypeId,
        'enumItemTypeCode': value.enumItemTypeCode,
        'enumItemTypeName': value.enumItemTypeName,
        'metadata': value.metadata,
    };
}