import { AuthApi, AuthResponseModel } from '../ApiClient/Auth';
import { BaseAPI } from '../ApiClient/Main';
import { Configuration } from '../ApiClient/Main';
import ApiUrls from '../Model/Setting/ApiUrl';
import DataLoader from '../Services/DataLoader';
import * as H from 'history';
import { Middleware } from '../ApiClient/runtime'
import UnauthorizedMiddleware from './UnauthorizedMiddleware'

export default class ApiClientFactory {
    private setting: ApiUrls = undefined;
    private history: H.History<any>;

    constructor(history: H.History<any>) {
        this.history = history;
    }

    public async getClient<TClient extends BaseAPI>(type: new (config: Configuration) => TClient): Promise<TClient> {
        let setting: ApiUrls = await this.getApiUrls();
        let apiUrl = setting.mainApi;
        let apiConfiguration = this.getApiConfiguration(apiUrl);
        let client: TClient = new type(apiConfiguration);

        return client;
    }

    public async getAuthClient<TClient extends BaseAPI>(type: new (config: Configuration) => TClient): Promise<TClient> {
        let setting: ApiUrls = await this.getApiUrls();
        let apiUrl = setting.authApi;
        let apiConfiguration = this.getApiConfiguration(apiUrl);
        let client: TClient = new type(apiConfiguration);

        return client;
    }

    public getClientWithSetting<TClient extends BaseAPI>(type: new (config: Configuration) => TClient, setting: ApiUrls): TClient {
        let apiUrl = setting.mainApi;
        let apiConfiguration = this.getApiConfiguration(apiUrl);
        let client: TClient = new type(apiConfiguration);
        return client;
    }

    private getApiConfiguration = (baseUrl: string) => {
        let authHeader: { [key: string]: string } = null;
        authHeader = this.getAuthHeader(authHeader);

        let unauthorizedMiddleware: UnauthorizedMiddleware = new UnauthorizedMiddleware(this.history);
        let middleware: Middleware[] = [unauthorizedMiddleware];
        let apiConfiguration = new Configuration({ headers: authHeader, basePath: baseUrl, middleware: middleware });
        return apiConfiguration;
    }

    private getApiUrls = async (): Promise<ApiUrls> => {
        if (this.setting == undefined) {
            let settingLoader = new DataLoader();
            this.setting = await settingLoader.getSetting();
        }

        return this.setting;
    }

    private getAuthHeader = (authHeader: { [key: string]: string }): { [key: string]: string } => {
        let token: string = localStorage.getItem("user");

        if (token != null) {
            let tokenModel: AuthResponseModel = JSON.parse(token);
            authHeader = {};
            authHeader["Authorization"] = `Bearer ${tokenModel.token}`;
        }

        return authHeader;
    }
}
