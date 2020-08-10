import { IPaymentInfo } from "./Model/IPaymentInfo"
import { BankAccountReponse } from './Model/BankAccountReponse'
import { PaymentTypeResponse } from './Model/PaymentTypeResponse'
import { PaymentCategoryResponse } from "./Model/PaymentCategoryResponse";
import { IPaymentResponseModel } from "./Model/IPaymentResponseModel";

export default class DataLoader {
    getPayments(filterDate: string, onSuccess: (payments: IPaymentInfo[]) => void, onRejected: any): Promise<Response> {
        return fetch("/Payment/GetPaymentsData?fromDate=" + filterDate)
            .then(res => {
                if (res.ok)
                    return res.json()
                else
                    onRejected();
            })
            .then(
                (result: IPaymentInfo[]) => {
                    onSuccess(result);
                },
                (_) => onRejected()
            );
    }

    getBankAccounts(onSuccess: (bankAccountResponse: BankAccountReponse) => void, onRejected: any): Promise<Response> {
        return fetch("/Payment/GetBankAccounts")
            .then(res => {
                if (res.ok)
                    return res.json()
                else
                    onRejected();
            })
            .then(
                (result: BankAccountReponse) => {
                    onSuccess(result);
                },
                (_) => onRejected()
            );
    }

    async addPayment(data: string, onSuccess: (payments: IPaymentInfo[]) => void, onRejected: any): Promise<void> {
        try {
            const response = await fetch('/Payment/AddPayment', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: data,
            });
            const responseData = await response.json();
            onSuccess(responseData);
        }
        catch (_) {
            return onRejected();
        }
    }

    async updatePayment(data: string, onSuccess: (payments: IPaymentInfo[]) => void, onRejected: any): Promise<void> {
        try {
            const response = await fetch('/Payment/UpdatePayment', {
                method: 'PUT',
                headers: { 'Content-Type': 'application/json' },
                body: data,
            });
            const responseData = await response.json();
            onSuccess(responseData);
        }
        catch (_) {
            return onRejected();
        }
    }

    getPaymentTypes(onSuccess: (response: PaymentTypeResponse) => void, onRejected: any): Promise<Response> {
        return fetch("/Payment/GetPaymentTypes")
            .then(res => {
                if (res.ok)
                    return res.json()
                else
                    onRejected();
            })
            .then(
                onSuccess,
                (_) => onRejected()
            )
            .catch(onRejected);
    }

    getPaymentCategories(onSuccess: (response: PaymentCategoryResponse) => void, onRejected: any): Promise<Response> {
        return fetch("/Payment/GetPaymentCategories")
            .then(res => {
                if (res.ok)
                    return res.json()
                else
                    onRejected();
            })
            .then(
                onSuccess,
                (_) => onRejected()
            )
            .catch(onRejected);
    }

    getPayment(id: number, onSuccess: (response: IPaymentResponseModel) => void, onRejected: any): Promise<Response> {
        return fetch(`/Payment/GetPayment/${id}`)
            .then(res => {
                if (res.ok)
                    return res.json()
                else
                    onRejected();
            })
            .then(
                onSuccess,
                (_) => onRejected()
            )
            .catch(onRejected);;
    }
}