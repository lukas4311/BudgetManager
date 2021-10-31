import { AuthApi, AuthResponseModel } from '../ApiClient/Auth';
import { BaseAPI } from '../ApiClient/Main';
import { Configuration } from '../ApiClient/Main';
import ApiUrls from '../Model/Setting/ApiUrl';
import DataLoader from '../Services/DataLoader';

export default class ApiClientFactory {
    private setting: ApiUrls = undefined;

    public async getClient<TClient extends BaseAPI>(type: new (config: Configuration) => TClient): Promise<TClient> {
        let setting: ApiUrls = await this.getApiUrls();
        let apiUrl = setting.mainApi;
        let authHeader: { [key: string]: string } = null;
        authHeader = this.getAuthHeader(authHeader);

        if(type instanceof AuthApi)
            apiUrl = setting.authApi;

        let apiConfiguration = new Configuration({ headers: authHeader, basePath: apiUrl });
        let client: TClient = new type(apiConfiguration);

        return client;
    }

    public async getAuthClient<TClient extends BaseAPI>(type: new (config: Configuration) => TClient): Promise<TClient> {
        let setting: ApiUrls = await this.getApiUrls();
        let apiUrl = setting.authApi;
        let authHeader: { [key: string]: string } = null;
        authHeader = this.getAuthHeader(authHeader);

        let apiConfiguration = new Configuration({ headers: authHeader, basePath: apiUrl });
        let client: TClient = new type(apiConfiguration);

        return client;
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
