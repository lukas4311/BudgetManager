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
    ComodityType,
    ComodityTypeFromJSON,
    ComodityTypeFromJSONTyped,
    ComodityTypeToJSON,
} from './ComodityType';

/**
 * 
 * @export
 * @class ComodityUnit
 */
export class ComodityUnit {
    /**
     * 
     * @type {number}
     * @memberof ComodityUnit
     */
    id?: number;
    /**
     * 
     * @type {string}
     * @memberof ComodityUnit
     */
    code?: string | null;
    /**
     * 
     * @type {string}
     * @memberof ComodityUnit
     */
    name?: string | null;
    /**
     * 
     * @type {Array<ComodityType>}
     * @memberof ComodityUnit
     */
    comodityTypes?: Array<ComodityType> | null;
}

export function ComodityUnitFromJSON(json: any): ComodityUnit {
    return ComodityUnitFromJSONTyped(json, false);
}

export function ComodityUnitFromJSONTyped(json: any, ignoreDiscriminator: boolean): ComodityUnit {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'code': !exists(json, 'code') ? undefined : json['code'],
        'name': !exists(json, 'name') ? undefined : json['name'],
        'comodityTypes': !exists(json, 'comodityTypes') ? undefined : (json['comodityTypes'] === null ? null : (json['comodityTypes'] as Array<any>).map(ComodityTypeFromJSON)),
    };
}

export function ComodityUnitToJSON(value?: ComodityUnit | null): any {
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
        'comodityTypes': value.comodityTypes === undefined ? undefined : (value.comodityTypes === null ? null : (value.comodityTypes as Array<any>).map(ComodityTypeToJSON)),
    };
}
