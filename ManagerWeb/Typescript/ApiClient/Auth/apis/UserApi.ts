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


import * as runtime from '../../runtime';
import {
    UserCreateModel,
    UserCreateModelFromJSON,
    UserCreateModelToJSON,
} from '../models';

export interface UserRegisterPostRequest {
    userCreateModel?: UserCreateModel;
}

/**
 * UserApi - interface
 * 
 * @export
 * @interface UserApiInterface
 */
export interface UserApiInterface {
    /**
     * 
     * @param {UserCreateModel} [userCreateModel] 
     * @param {*} [options] Override http request option.
     * @throws {RequiredError}
     * @memberof UserApiInterface
     */
    userRegisterPostRaw(requestParameters: UserRegisterPostRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>>;

    /**
     */
    userRegisterPost(requestParameters: UserRegisterPostRequest, initOverrides?: RequestInit): Promise<void>;

}

/**
 * 
 */
export class UserApi extends runtime.BaseAPI implements UserApiInterface {
    processPathParam(param: any): string {
        if (param instanceof Date)
            return encodeURIComponent(String(param.toISOString()));

        return encodeURIComponent(String(param));
    }

    /**
     */
    async userRegisterPostRaw(requestParameters: UserRegisterPostRequest, initOverrides?: RequestInit): Promise<runtime.ApiResponse<void>> {
        const queryParameters: any = {};

        const headerParameters: runtime.HTTPHeaders = {};

        headerParameters['Content-Type'] = 'application/json';

        const response = await this.request({
            path: `/user/register`,
            method: 'POST',
            headers: headerParameters,
            query: queryParameters,
            body: UserCreateModelToJSON(requestParameters.userCreateModel),
        }, initOverrides);

        return new runtime.VoidApiResponse(response);
    }

    /**
     */
    async userRegisterPost(requestParameters: UserRegisterPostRequest, initOverrides?: RequestInit): Promise<void> {
        await this.userRegisterPostRaw(requestParameters, initOverrides);
    }

}
