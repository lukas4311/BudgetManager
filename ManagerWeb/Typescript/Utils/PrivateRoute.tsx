import React, { useEffect, useState } from 'react';
import { AuthApi, AuthResponseModel } from "../ApiClient/Auth";
import { Route, Redirect } from "react-router-dom";
import ApiClientFactory from './ApiClientFactory';
import { SpinnerCircularSplit } from 'spinners-react';

const PrivateRoute = (props: any) => {
    const [isValid, setIsValid] = useState<boolean>(undefined);

    useEffect(() => {
        const tokenString = localStorage.getItem('user');

        if (tokenString) {
            const userData: AuthResponseModel = JSON.parse(tokenString);
            getToken(userData.token);
        }
        else {
            setIsValid(false);
        }
    }, []);

    const getToken = async (token: string) => {
        let apiFactory = new ApiClientFactory(props.history);
        let authApi = await apiFactory.getAuthClient(AuthApi);
        try {
            const isValid = await authApi.authValidatePost({ tokenModel: { token: token } });
            setIsValid(isValid);
        } catch (error) {
            setIsValid(false);
        }
    };

    if (isValid != undefined) {
        if (isValid)
            return <Route {...props.rest} component={props.component} />;
        else
            return <Redirect to='/login' />;
    }
    else {
        return (
            <div className="flex text-center justify-center h-full">
                <SpinnerCircularSplit size={150} thickness={110} speed={70} color="rgba(27, 39, 55, 1)" secondaryColor="rgba(224, 61, 21, 1)" />
            </div>
        )
    }
}

export default PrivateRoute;