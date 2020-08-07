import { IPaymentInfo } from "./Model/IPaymentInfo"
import { BankAccountReponse } from './Model/BankAccountReponse'

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

    getBankAccounts(onSuccess: (BankAccountReponse) => void, onRejected: any): Promise<Response> {
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

    addPayment(data: string, onSuccess: (payments: IPaymentInfo[]) => void, onRejected: any): Promise<void> {
        return fetch('/Payment/AddPayment', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: data,
        })
            .then(response => response.json())
            .then(data => { onSuccess(data)})
            .catch(_ => onRejected());
    }

    updatePayment(data: string, onSuccess: (payments: IPaymentInfo[]) => void, onRejected: any): Promise<void> {
        return fetch('/Payment/AddPayment', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: data,
        })
            .then(response => response.json())
            .then(data => { onSuccess(data)})
            .catch(_ => onRejected());
    }

    getPaymentsData(filterDate: string): Promise<Response> {
        return fetch("/Payment/GetPaymentsData?fromDate=" + filterDate);
    }

    getPaymentTypes(): Promise<Response> {
        return fetch("/Payment/GetPaymentTypes");
    }

    getPaymentCategories(): Promise<Response> {
        return fetch("/Payment/GetPaymentCategories");
    }

    getPayment(id: number): Promise<Response> {
        return fetch(`/Payment/GetPayment/${id}`);
    }
}