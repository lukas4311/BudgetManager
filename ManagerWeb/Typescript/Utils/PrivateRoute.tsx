import React, { useState } from 'react';
import { AuthResponseModel } from "../ApiClient/Auth";
import { Route, Redirect } from "react-router-dom";

const PrivateRoute = (props: any) => {
    const getUserData = (): AuthResponseModel => {
        const tokenString = localStorage.getItem('user');
        const userData: AuthResponseModel = JSON.parse(tokenString);
        return userData;
    };

    const [userData, _] = useState<AuthResponseModel>(getUserData());

    return (
        userData != undefined && userData != null ?
            <Route {...props.rest} component={props.component} /> :
            <Redirect to='/login' />
    );
}

export default PrivateRoute;