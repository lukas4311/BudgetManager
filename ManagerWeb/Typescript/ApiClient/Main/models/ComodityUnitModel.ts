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
 * @class ComodityUnitModel
 */
export class ComodityUnitModel {
    /**
     * 
     * @type {number}
     * @memberof ComodityUnitModel
     */
    id?: number | null;
    /**
     * 
     * @type {string}
     * @memberof ComodityUnitModel
     */
    code?: string | null;
    /**
     * 
     * @type {string}
     * @memberof ComodityUnitModel
     */
    name?: string | null;
}

export function ComodityUnitModelFromJSON(json: any): ComodityUnitModel {
    return ComodityUnitModelFromJSONTyped(json, false);
}

export function ComodityUnitModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): ComodityUnitModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'code': !exists(json, 'code') ? undefined : json['code'],
        'name': !exists(json, 'name') ? undefined : json['name'],
    };
}

export function ComodityUnitModelToJSON(value?: ComodityUnitModel | null): any {
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
    };
}
