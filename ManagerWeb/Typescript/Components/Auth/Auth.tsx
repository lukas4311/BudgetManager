import React from "react";
import ReactDOM from "react-dom";
import { RouteComponentProps } from "react-router-dom";
import { AuthResponseModel } from "../../ApiClient/Auth";
import { AuthApi } from "../../ApiClient/Auth/apis/AuthApi";
import ApiClientFactory from "../../Utils/ApiClientFactory";

class AuthState {
    login: string;
    password: string;
}

export default class Auth extends React.Component<RouteComponentProps, AuthState>{
    apiFactory: ApiClientFactory;

    constructor(state: RouteComponentProps) {
        super(state);
        this.state = { login: '', password: '' };
    }

    componentDidMount(){
        this.initServies();
    }

    private initServies = async () => {
        this.apiFactory = new ApiClientFactory();
    }

    private login = async () => {
        let authApi: AuthApi = await this.apiFactory.getAuthClient(AuthApi);

        try {
            let authModel: AuthResponseModel = await authApi.authAuthenticatePost({ userModel: { password: this.state.password, userName: this.state.login } });
            localStorage.setItem("user", JSON.stringify(authModel));
            this.props.history.push("/");
        } catch (error) {
            console.log(error);
        }
    }

    private onChangeLogin = (e: React.ChangeEvent<HTMLInputElement>) => {
        let login = e.target.value;
        this.setState({ login: login });
    }

    private onChangePassword = (e: React.ChangeEvent<HTMLInputElement>) => {
        let pass = e.target.value;
        this.setState({ password: pass });
    }

    render = () => {
        return (
            <div className="m-auto text-center">
                <h1 className="text-2xl">Přihlášení</h1>
                <div className="flex flex-col w-2/5 m-auto mt-8">
                    <div asp-validation-summary="All" className="text-red-600 mb-4"></div>
                    <div className="flex">
                        <div className="w-1/2">
                            <div className="relative inline-block w-2/3 m-auto">
                                <input className="effect-11 w-full" placeholder="Login" value={this.state.login} onChange={e => this.onChangeLogin(e)} />
                                <span className="focus-bg"></span>
                            </div>
                        </div>
                        <div className="w-1/2">
                            <div className="relative inline-block w-2/3 m-auto">
                                <input type="password" className="effect-11 w-full" placeholder="Heslo" value={this.state.password} onChange={(e) => this.onChangePassword(e)} />
                                <span className="focus-bg"></span>
                            </div>
                        </div>
                    </div>
                    <div className="flex mt-8">
                        <button onClick={this.login} className="m-auto bg-vermilion px-4 py-1 rounded-sm hover:text-vermilion hover:bg-white duration-500">Potvrdit</button>
                    </div>
                </div>
            </div>
        );
    }
}