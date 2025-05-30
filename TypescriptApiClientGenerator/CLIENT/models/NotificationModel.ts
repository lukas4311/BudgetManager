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
 * @class NotificationModel
 */
export class NotificationModel {
    /**
     * 
     * @type {number}
     * @memberof NotificationModel
     */
    id?: number | null;
    /**
     * 
     * @type {number}
     * @memberof NotificationModel
     */
    userIdentityId?: number;
    /**
     * 
     * @type {string}
     * @memberof NotificationModel
     */
    heading?: string | null;
    /**
     * 
     * @type {string}
     * @memberof NotificationModel
     */
    content?: string | null;
    /**
     * 
     * @type {boolean}
     * @memberof NotificationModel
     */
    isDisplayed?: boolean;
    /**
     * 
     * @type {Date}
     * @memberof NotificationModel
     */
    timestamp?: Date;
}

export function NotificationModelFromJSON(json: any): NotificationModel {
    return NotificationModelFromJSONTyped(json, false);
}

export function NotificationModelFromJSONTyped(json: any, ignoreDiscriminator: boolean): NotificationModel {
    if ((json === undefined) || (json === null)) {
        return json;
    }
    return {
        
        'id': !exists(json, 'id') ? undefined : json['id'],
        'userIdentityId': !exists(json, 'userIdentityId') ? undefined : json['userIdentityId'],
        'heading': !exists(json, 'heading') ? undefined : json['heading'],
        'content': !exists(json, 'content') ? undefined : json['content'],
        'isDisplayed': !exists(json, 'isDisplayed') ? undefined : json['isDisplayed'],
        'timestamp': !exists(json, 'timestamp') ? undefined : (new Date(json['timestamp'])),
    };
}

export function NotificationModelToJSON(value?: NotificationModel | null): any {
    if (value === undefined) {
        return undefined;
    }
    if (value === null) {
        return null;
    }
    return {
        
        'id': value.id,
        'userIdentityId': value.userIdentityId,
        'heading': value.heading,
        'content': value.content,
        'isDisplayed': value.isDisplayed,
        'timestamp': value.timestamp === undefined ? undefined : (value.timestamp.toISOString()),
    };
}
