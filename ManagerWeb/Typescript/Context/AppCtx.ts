import React from "react";
import ApiUrls from "../Model/Setting/ApiUrl";
import { SnackbarMessageModel } from "../App";

export class AppContext {
    apiUrls: ApiUrls;
    setSnackbarMessage: (model: SnackbarMessageModel) => void;
}

export const AppCtx = React.createContext<AppContext>({ apiUrls: { authApi: "aaa", mainApi: "bbb", finApi: "fff" }, setSnackbarMessage: undefined });