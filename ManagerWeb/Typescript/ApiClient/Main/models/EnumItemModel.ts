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
 * @class EnumItemModel
 */
export class EnumItemModel {
    /**
     * 
     * @type {number}
     * @memberof EnumItemModel
     */
    id?: number | null;
    /**
     * 
     * @type {string}
     * @memberof EnumItemModel
     */
    code?: string | null;
    /**
     * 
     * @type {string}
     * @memberof EnumItemModel
     */
    name?: string | null;
    /**
     * 
     * @type {number}
     * @memberof EnumItemModel
     */
    enumItemTypeId?: number;
    /**
     * 
     * @type {string}
     * @memberof EnumItemModel
     */
    metadata?: string | null;
}

export function EnumItemModelFromJSON(json: any): EnumItemModel {
    return EnumItemModelFromJSONTyped(json, false);
}

export function EnumItemModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): EnumItemModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'code': !exists(json, 'code') ? undefined : json['code'],
        'name': !exists(json, 'name') ? undefined : json['name'],
        'enumItemTypeId': !exists(json, 'enumItemTypeId') ? undefined : json['enumItemTypeId'],
        'metadata': !exists(json, 'metadata') ? undefined : json['metadata'],
    };
}

export function EnumItemModelToJSON(value?: EnumItemModel | null): any {
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
        'metadata': value.metadata,
    };
}