import { IPaymentInfo } from "../Model/IPaymentInfo"
import { BankAccountReponse } from '../Model/BankAccountReponse'
import { IBankAccountBalanceResponseModel } from "../Model/IBankAccountBalanceResponseModel";
import { BudgetModel } from "../Model/BudgetModel";
import { IPaymentModel } from "../Model/IPaymentModel";
import ApiUrls from "../Model/Setting/ApiUrl";

export default class DataLoader {

    async getSetting(): Promise<ApiUrls> {
        const res = await fetch(`/setting/apiRoutes`);
        let responseData: ApiUrls = await res.json();
        // let apiUrls: ApiUrls = JSON.parse(responseData);
        return responseData;
    }

    async getPayments(fromDate: string, toDate: string, bankAccountId: number, onRejected: () => void): Promise<IPaymentInfo[]> {
        let response: IPaymentInfo[];
        let url = '/payment/data';

        try {
            const res = await fetch(`/payment/data?${this.queryParams({ fromDate: fromDate, toDate: toDate, bankAccountId: bankAccountId })}`);
            response = await res.json();
        }
        catch (_) {
            onRejected();
        }

        return response;
    }

    async getBankAccounts(onRejected: () => void): Promise<BankAccountReponse> {
        let response: BankAccountReponse;

        try {
            const res = await fetch("/payment/bankAccounts");
            response = await res.json();
        }
        catch (_) {
            onRejected();
        }

        return response;
    }

    async addPayment(data: string, onRejected: () => void): Promise<void> {
        try {
            await fetch('/payment', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: data,
            });
        }
        catch (_) {
            onRejected();
        }
    }

    async updatePayment(data: string, onRejected: () => void): Promise<void> {
        try {
            fetch('/payment', {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: data,
            });
        }
        catch (_) {
            onRejected();
        }
    }

    async getPayment(id: number, onRejected: () => void): Promise<IPaymentModel> {
        let response: IPaymentModel;

        try {
            const res = await fetch(`/payment/detail?id=${id}`);
            response = await res.json();
        }
        catch (_) {
            onRejected();
        }

        return response
    }

    async getBankAccountsBalanceToDate(toDate: string, onRejected: () => void): Promise<IBankAccountBalanceResponseModel> {
        let response: IBankAccountBalanceResponseModel;

        try {
            const res = await fetch("/bankAccount/getAllAccountBalance?toDate=" + toDate);
            response = await res.json();
        }
        catch (_) {
            onRejected();
        }

        return response
    }

    async addTagToPayment(code: string, paymentId: number): Promise<void> {
        let dataJson = JSON.stringify({ code, paymentId });

        fetch('/tag', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: dataJson,
        });
    }

    async getAllBudgets(): Promise<BudgetModel[]> {
        const res = await fetch(`/budget/getAll/`);
        return await res.json();
    }

    async getBudget(id: number): Promise<BudgetModel> {
        const res = await fetch("/budget/get?id=" + id);
        return await res.json();
    }

    async addBudget(budgetModel: BudgetModel) {
        const dataJson = JSON.stringify(budgetModel);

        await fetch('/budget/add', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: dataJson,
        });
    }

    async updateBudget(budgetModel: BudgetModel) {
        const dataJson = JSON.stringify(budgetModel);

        await fetch('/budget/update', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: dataJson,
        });
    }

    private queryParams(params: any): string {
        return Object.keys(params)
            .filter(k => params[k] != null && params[k] != undefined)
            .map(k => encodeURIComponent(k) + '=' + encodeURIComponent(params[k]))
            .join('&');
    }
}