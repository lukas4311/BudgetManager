import React from "react";
import ApiUrls from "../Model/Setting/ApiUrl";

export class AppContext {
    apiUrls: ApiUrls;
}

export const AppCtx = React.createContext<AppContext>({ apiUrls: { authApi: "aaa", mainApi: "bbb", finApi: "fff" } });