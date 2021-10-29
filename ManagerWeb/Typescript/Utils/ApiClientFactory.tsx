import { AuthResponseModel } from '../ApiClient/Auth';
import { BaseAPI } from '../ApiClient/Main';
import { Configuration } from '../ApiClient/Main';

export default class ApiClientFactory {
    public async getClient<TClient extends BaseAPI>(type: new (config: Configuration) => TClient): Promise<TClient> {
        //let setting = await this.getSetting();
        let authHeader: { [key: string]: string } = null;
        authHeader = this.getAuthHeader(authHeader);

        let apiConfiguration = new Configuration({headers: authHeader });
        let client: TClient = new type(apiConfiguration);

        return client;
    }

    //private getSetting = async (): Promise<ApiSetting> => {
    //    if (this.setting == undefined) {
    //        let settingLoader = new SettingLoader();
    //        this.setting = await settingLoader.getApiSetting();
    //    }

    //    return this.setting;
    //}

    private getAuthHeader = (authHeader: { [key: string]: string }): { [key: string]: string } => {
        let token: string = sessionStorage.getItem("user");

        if (token != null) {
            let tokenModel: AuthResponseModel = JSON.parse(token);
            authHeader = {};
            authHeader["Authorization"] = `Bearer ${tokenModel.token}`;
        }

        return authHeader;
    }
}
