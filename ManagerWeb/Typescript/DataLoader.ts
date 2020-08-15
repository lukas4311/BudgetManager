import { IPaymentInfo } from "./Model/IPaymentInfo"
import { BankAccountReponse } from './Model/BankAccountReponse'
import { PaymentTypeResponse } from './Model/PaymentTypeResponse'
import { PaymentCategoryResponse } from "./Model/PaymentCategoryResponse";
import { IPaymentResponseModel } from "./Model/IPaymentResponseModel";
import { IBankAccountBalanceResponseModel } from "./Model/IBankAccountBalanceResponseModel";

export default class DataLoader {
    async getPayments(filterDate: string, onRejected: any): Promise<IPaymentInfo[]> {
        let response: IPaymentInfo[];

        try {
            const res = await fetch("/Payment/GetPaymentsData?fromDate=" + filterDate);
            response = await res.json();
        }
        catch (_) {
            onRejected();
        }

        return response;
    }

    async getBankAccounts(onRejected: any): Promise<BankAccountReponse> {
        let response: BankAccountReponse;

        try {
            const res = await fetch("/Payment/GetBankAccounts");
            response = await res.json();
        }
        catch (_) {
            onRejected();
        }

        return response;
    }

    async addPayment(data: string, onRejected: any): Promise<void> {
        try {
            await fetch('/Payment/AddPayment', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: data,
            });
        }
        catch (_) {
            onRejected();
        }
    }

    async updatePayment(data: string, onRejected: any): Promise<void> {
        try {
            fetch('/Payment/UpdatePayment', {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: data,
            });
        }
        catch (_) {
            onRejected();
        }
    }

    async getPaymentTypes(onRejected: any): Promise<PaymentTypeResponse> {
        let response: PaymentTypeResponse;

        try {
            const res = await fetch("/Payment/GetPaymentTypes");
            response = await res.json();
        }
        catch (_) {
            onRejected();
        }

        return response;
    }

    async getPaymentCategories(onRejected: any): Promise<PaymentCategoryResponse> {
        let response: PaymentCategoryResponse;

        try {
            const res = await fetch("/Payment/GetPaymentCategories");
            response = await res.json();
        }
        catch (_) {
            onRejected();
        }

        return response;
    }

    async getPayment(id: number, onRejected: any): Promise<IPaymentResponseModel> {
        let response: IPaymentResponseModel;

        try {
            const res = await fetch(`/Payment/GetPayment/${id}`);
            response = await res.json();
        }
        catch (_) {
            onRejected();
        }

        return response
    }

    async getBankAccountsBalanceToDate(toDate: string, onRejected: any): Promise<IBankAccountBalanceResponseModel> {
        let response: IBankAccountBalanceResponseModel;

        try {
            const res = await fetch("/BankAccount/GetBankAccountsBalanceToDate?toDate=" + toDate);
            response = await res.json();
        }
        catch (_) {
            onRejected();
        }

        return response
    }
}