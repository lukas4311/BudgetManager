import { Middleware, ResponseContext } from '../ApiClient/runtime';
import * as H from 'history';

export default class UnauthorizedMiddleware implements Middleware {
    history: H.History<any>;

    constructor(history: H.History<any>) {
        this.history = history;
    }

    post = (context: ResponseContext): Promise<void | Response> => {
        if (context.response.status == 401){
            localStorage.removeItem("user");
            this.history.push("/login");
        }

        return new Promise<Response>((resolve, reject) => {
            resolve(context.response);
            reject(new Error('Unauthorized middlware failed'));
        });
    };
}
