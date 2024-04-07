import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { AuthResponseModel, UserApi } from "../../ApiClient/Auth";
import { AuthApi } from "../../ApiClient/Auth/apis/AuthApi";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { Snackbar } from "@mui/material";
import { Alert } from "@mui/material";

class RegistrationState {
    login: string;
    firstName: string;
    lastName: string;
    password: string;
    passwordConfirm: string;
    errorMessage: string;
    email: string;
    phone: string;
}

export default class Auth extends React.Component<RouteComponentProps, RegistrationState> {
    apiFactory: ApiClientFactory;

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { login: '', password: '', errorMessage: "", firstName: '', lastName: '', passwordConfirm: '', email: '', phone: null };
    }

    componentDidMount() {
        this.initServices();
    }

    private initServices = async () => {
        this.apiFactory = new ApiClientFactory(this.props.history);
    }

    private register = async (event: any) => {
        event.preventDefault();
        console.log(this.state);
        let userApi: UserApi = await this.apiFactory.getAuthClient(UserApi);

        try {
            await userApi.userRegisterPost({
                userCreateModel: {
                    login: this.state.login,
                    password: this.state.password,
                    firstName: this.state.firstName,
                    lastName: this.state.lastName,
                    email: this.state.email,
                    phone: this.state.phone
                }
            });
            this.props.history.push("/");
        } catch (error) {
            console.log(error);
            this.setState({ errorMessage: "Registration failed. Try it again later :(" });
        }
    }

    private onChange(propName: string, e: React.ChangeEvent<HTMLInputElement>) {
        let value = e.target.value;
        let stateObject = this.state;
        stateObject[propName] = value;
        this.setState(stateObject);
    }

    private handleClose = () => {
        this.setState({ errorMessage: "" });
    }

    private renderField = (propertyName: string, placeholder: string, params: any = {}) => {
        return (<div className="w-full mb-4">
            <div className="relative inline-block w-2/3 m-auto">
                <input className="effect-11 w-full" placeholder={placeholder} value={this.state[propertyName] ?? ''} onChange={e => this.onChange(propertyName, e)} {...params} />
                <span className="focus-bg"></span>
            </div>
        </div>)
    }

    render = () => {
        return (
            <div>
                <div className="w-1/2 m-auto">
                    <svg version="1.2" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 709 238">
                        <title>Logo</title>
                        <path id="&amp;" className="s0T" aria-label="&amp;" d="m2.2 111.2q0-9 2.6-15.8 2.6-6.8 6.8-12.2 4.4-5.4 9.8-9.4 5.6-4.2 11.6-7.6-4.6-7-7.8-14.4-3-7.4-3-14.8 0-7.4 2.8-13.4 3-6 7.8-10 5-4.2 11.4-6.4 6.4-2.2 13.4-2.2 7.2 0 13.4 2.6 6.4 2.4 11 6.8 4.8 4.4 7.6 10.6 2.8 6 2.8 13.2 0 6.6-2 11.8-1.8 5.2-5 9.4-3 4.2-7.4 7.6-4.2 3.4-9 6.4 3 3.6 5.6 6.6 2.6 3 4.2 5l14.4 17.4q2.2-8.6 2.6-19.6 0-1.4-0.2-2.6 0-1.4-0.2-2.8l23.8-3.4q0.2 2.4 0.4 4.8 0.2 2.4 0.2 4.8 0 22.4-9.4 39.6l16.6 20.4-18.8 14.8-13.4-16.2q-8.4 6.8-19 10.6-10.6 3.8-23.8 3.8-9.8 0-18.8-3.2-9-3.2-16-9-6.8-6-11-14.4-4-8.4-4-18.8zm77.4 12.6l-22.2-27.2q-2.2-2.8-4.8-5.6-2.4-2.8-5-6-4.4 2.6-8.4 5.4-3.8 2.6-6.8 5.8-2.8 3.2-4.6 7.2-1.6 3.8-1.6 8.6 0 5.2 2.4 9.2 2.4 3.8 6.2 6.4 4 2.6 8.6 3.8 4.8 1.2 9.4 1.2 8.4 0 15-2.4 6.6-2.4 11.8-6.4zm-25.2-69q6.2-3.6 10-7.6 4-4.2 4-9.8 0-5.4-3.6-8.4-3.6-3-8-3-4 0-7.4 3-3.2 2.8-3.2 9.2 0 2.8 2.2 7.2 2.4 4.2 6 9.4z" />
                        <path id="Budget" className="s1T" aria-label="Budget" d="m75.5 5.7h24.1q7.7 0 12.7 3.2 4.9 3.1 7.1 7.9 2.3 4.6 1.7 9.9-0.5 5.2-3.8 9.4 4 2.5 6.2 6.3 2.3 3.9 2.9 8.2 0.7 4.3-0.3 8.7-1 4.3-3.6 7.9-2.7 3.4-7.1 5.6-4.3 2.2-10.3 2.2h-29.6zm29.6 38.3h-17.7v19.9h17.7q3.2 0 5.3-1.5 2.1-1.5 3-3.6 1.1-2.2 1.1-4.8 0-2.7-1.1-4.8-0.9-2.2-3-3.7-2.1-1.5-5.3-1.5zm-5.5-27.3h-12.2v16.1h12.4q4.5-0.1 6.8-2.5 2.4-2.6 2.3-5.6 0-3.1-2.4-5.5-2.3-2.5-6.9-2.5zm39.3-11h11.9v43q0 4 1.4 7.1 1.3 3 3.6 5 2.3 2 5.2 3 2.9 1 5.9 1 3.1 0 5.9-1 2.9-1 5.2-3 2.3-2 3.7-5 1.3-3.1 1.3-7.1v-43h11.9v43q0 7.1-2.5 12.5-2.3 5.4-6.3 9-3.9 3.6-8.9 5.4-5.1 1.8-10.3 1.8-5.2 0-10.3-1.8-5-1.8-9-5.4-3.9-3.6-6.3-9-2.4-5.4-2.4-12.5v-22.3l11.9-9.6h-11.9zm70.1 20.7l11.9-9.6h-11.9v-11.1h20.5q8.9 0.1 15.5 3.1 6.6 2.9 11 7.9 4.5 4.8 6.6 11.1 2.3 6.1 2.3 12.5 0 6.5-2.3 12.7-2.1 6.1-6.6 11.1-4.4 4.8-11 7.8-6.6 3-15.5 3.1h-20.5zm20-9.6h-8.1v47h8.1q7.8 0 12.9-3.5 5.3-3.4 7.9-8.7 2.6-5.2 2.6-11.3 0-6.1-2.5-11.3-2.6-5.3-7.9-8.8-5.2-3.4-13-3.4zm105 20.4v31q-4.3 3.4-10.6 5.9-6.2 2.4-13 2.4-9 0-15.7-3.1-6.7-3.2-11.2-8.2-4.4-5.1-6.7-11.5-2.2-6.5-2.2-13.4 0-3.5 0.6-7 0.6-3.5 1.9-6.9h13.8q-2.2 3.6-3.2 7.8-0.9 4.2-0.6 8.4 0.4 4.3 2 8.3 1.7 4 4.5 7.1 3 3 7.1 4.9 4.3 1.8 9.8 1.8 4.1 0 6.9-0.9 2.8-1 4.6-2.2v-13.3h-16.2v-11.1zm-42.6-11.7l-8.6-9.2q4.5-5.5 11.3-8.7 6.9-3.4 16.2-3.4 5.6 0 10.9 1.9 5.4 1.8 9.8 5.1l-6.9 9.7q-2.2-1.9-5.7-3.4-3.5-1.5-8-1.5-6.6 0-11.3 2.7-4.8 2.5-7.7 6.8zm56.7-8.7v-11.1h44.8v11.1h-33v18h25.9v11.1h-25.9v18h33v11.1h-44.8v-48.6l11.8-9.6zm88.3 0v58.2h-11.8v-48.6zm22.6 0h-57v-11.1h57z" />
                        <path id="Investment" className="s1T" aria-label="Investment" d="m75.5 155v-69.3h11.9v69.3zm26.9-56.6v-12.7h4.1l40.6 43.7v-43.7h11.9v69.3h-4l-40.7-43.9v43.9h-11.9v-46.1h9.8zm89.8 35.5l-22.2-48.2h12.4l20.8 45.1 21-45.1h12.3l-32.2 70.3h-2.4l-4.9-11.7 4.8-10.4zm55.4-37.1v-11.1h44.8v11.1h-32.9v18h25.8v11.1h-25.8v18h32.9v11.1h-44.8v-48.6l11.9-9.6zm69.9 8.6l-11-8.8q1.1-3.5 3.3-6.1 2.3-2.5 5.4-4.2 3-1.8 6.6-2.6 3.6-0.9 7.4-0.9 6.9 0 13.2 2.8 6.4 2.7 11.2 7.4l-7.8 8.7q-3.5-3.4-7.9-5.3-4.2-1.9-9.1-1.9-2.2 0-4.3 0.5-2.1 0.4-3.8 1.5-1.6 1-2.6 2.8-1 1.6-1 4.2 0 1.1 0.4 1.9zm-11.7 1h12.2q1.4 2.2 4.7 3.8 3.4 1.7 7.5 3.4 4.3 1.7 8.7 3.7 4.5 1.9 8.1 4.7 3.8 2.8 6.2 6.7 2.5 3.8 2.5 9.4 0 5-2.3 8.8-2.3 3.6-5.9 6.1-3.5 2.5-8.1 3.7-4.4 1.2-8.9 1.2-3.8 0-7.7-0.7-3.8-0.7-7.3-2.2-3.5-1.5-6.6-3.8-3-2.2-5.4-5.4l9.2-7.2q3.3 4.3 8.1 6 5 1.6 10.1 1.6 2 0 4.3-0.4 2.2-0.4 4.2-1.4 2-1 3.3-2.6 1.3-1.7 1.3-4.1 0-3.2-2.6-5.4-2.6-2.3-6.5-4-4-1.9-8.7-3.7-4.7-1.9-9-4.3-4.2-2.4-7.4-5.7-3.1-3.4-4-8.2zm91.2-9.6v58.2h-11.8v-48.6zm22.6 0h-57v-11.1h57zm10.1 58.2l7-45.8h7.4l-12.4-23.5h12.4l26.2 49.8 26.3-49.8h3.9l10.4 69.3h-11.7l-6.1-39.6-20.8 39.6h-4l-20.8-39.3-6.1 39.3zm95.2-58.2v-11.1h44.9v11.1h-33v18h25.9v11.1h-25.9v18h33v11.1h-44.9v-48.6l11.9-9.6zm57.9 1.6v-12.7h4.1l40.6 43.7v-43.7h11.9v69.3h-4l-40.7-43.8v43.8h-11.9v-46.1h9.8zm102-1.6v58.2h-11.9v-48.6zm22.7 0h-57v-11.1h57z" />
                        <path id="Manager" className="s2T" aria-label="Manager" d="m11.5 234l10.4-69.3h4l26.2 49.8 26.3-49.8h3.9l10.4 69.3h-11.7l-6.1-39.6-20.8 39.6h-4l-20.8-39.6-6.1 39.6zm91.3 0l22.3-48.6h9.5l-9.5-20.7h12.4l31.7 69.3h-12.3l-4.4-9.7h-32.9l-4.4 9.7zm21.9-20.8h22.6l-11.3-24.5zm55.6-35.9v-12.7h4l40.6 43.7v-43.7h11.9v69.3h-4l-40.6-43.8v43.8h-11.9v-46.1h9.8zm67.6 56.6l22.3-48.6h9.5l-9.5-20.7h12.4l31.7 69.3h-12.3l-4.4-9.7h-32.9l-4.4 9.7zm21.9-20.7h22.6l-11.3-24.6zm107.5-17v31q-4.3 3.4-10.5 5.9-6.3 2.4-13.1 2.4-8.9 0-15.7-3.1-6.7-3.2-11.1-8.2-4.5-5.1-6.8-11.5-2.2-6.5-2.2-13.4 0-3.5 0.6-7 0.6-3.6 1.9-6.9h13.9q-2.3 3.5-3.3 7.8-0.9 4.2-0.6 8.4 0.4 4.3 2 8.3 1.7 4 4.6 7.1 2.9 3 7.1 4.9 4.2 1.8 9.7 1.8 4.1 0 6.9-0.9 2.8-1 4.7-2.2v-13.3h-16.3v-11.1zm-42.5-11.7l-8.7-9.2q4.5-5.5 11.3-8.7 7-3.4 16.3-3.4 5.5 0 10.9 1.9 5.3 1.8 9.8 5.1l-7 9.7q-2.1-1.9-5.7-3.4-3.5-1.5-7.9-1.5-6.7 0-11.4 2.7-4.8 2.5-7.6 6.8zm56.6-8.7v-11.1h44.8v11.1h-32.9v18h25.8v11.1h-25.8v18h32.9v11.1h-44.8v-48.6l11.9-9.6zm57.9-11.1h21.9q6.8 0 11.6 2.4 4.9 2.3 7.7 6.2 2.8 3.8 3.7 8.4 0.9 4.6-0.3 9-1.1 4.4-4.2 8.1-3.1 3.8-8.2 5.8l20.1 29.4h-14.2l-18.7-27.6h-7.5v27.6h-11.9v-48.6l11.7-9.5h-11.7zm21.9 11.2h-10v19.3h10q3.7 0 6.2-1.4 2.6-1.5 3.8-3.6 1.2-2.2 1.2-4.6 0-2.6-1.2-4.7-1.3-2.2-3.8-3.6-2.5-1.4-6.2-1.4z" />
                    </svg>
                </div>
                <div className="w-2/5 m-auto text-center my-12 px-4 py-12 bg-prussianBlue rounded-lg boxShadow">
                    <h1 className="text-2xl">Sign up</h1>
                    <div className="flex flex-col w-4/5 m-auto mt-8">
                        <form onSubmit={this.register}>
                            <div asp-validation-summary="All" className="text-red-600 mb-4"></div>
                            <div className="flex flex-col">
                                {this.renderField("login", "Login", { required: true })}
                                {this.renderField("lastName", "Last name", { required: true })}
                                {this.renderField("firstName", "First name", { required: true })}
                                {this.renderField("email", "Email", { reqired: true })}
                                {this.renderField("phone", "Phone")}
                                {this.renderField("password", "Pass", { type: "password", required: true })}
                                {this.renderField("passwordConfirm", "Pass confirmation", { type: "password", required: true })}
                            </div>
                            <div className="flex mt-8">
                                <input type="submit" className="m-auto bg-vermilion px-4 py-1 rounded-sm hover:text-vermilion hover:bg-white duration-500" value="Registrate" />
                            </div>
                        </form>
                    </div>
                </div>
                <Snackbar open={this.state.errorMessage != ""} autoHideDuration={6000} onClose={this.handleClose}>
                    <Alert onClose={this.handleClose} severity="error">{this.state.errorMessage}</Alert>
                </Snackbar>
            </div>
        );
    }
}