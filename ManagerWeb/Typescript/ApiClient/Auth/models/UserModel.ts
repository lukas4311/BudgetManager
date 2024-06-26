/* tslint:disable */
/* eslint-disable */
/**
 * BudgetManager.AuthApi
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
 * @class UserModel
 */
export class UserModel {
    /**
     * 
     * @type {string}
     * @memberof UserModel
     */
    userName?: string | null;
    /**
     * 
     * @type {string}
     * @memberof UserModel
     */
    password?: string | null;
}

export function UserModelFromJSON(json: any): UserModel {
    return UserModelFromJSONTyped(json, false);
}

export function UserModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): UserModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'userName': !exists(json, 'userName') ? undefined : json['userName'],
        'password': !exists(json, 'password') ? undefined : json['password'],
    };
}

export function UserModelToJSON(value?: UserModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'userName': value.userName,
        'password': value.password,
    };
}
