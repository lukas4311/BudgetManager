import React, { useState } from 'react';
import { AuthResponseModel } from "../ApiClient/Auth";
import { Route, Redirect } from "react-router-dom";

const PrivateRoute = (props: any) => {
    const getUserData = (): AuthResponseModel => {
        const tokenString = sessionStorage.getItem('user');
        const userData: AuthResponseModel = JSON.parse(tokenString);
        return userData;
    };

    const [userData, _] = useState<AuthResponseModel>(getUserData());

    return (
        <Route {...props.rest} render={() => {
            return userData != undefined
                ? <props.component {...props} />
                : <Redirect to='/login' />
        }} />
    );
}

export default PrivateRoute;