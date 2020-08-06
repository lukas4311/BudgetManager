import { IPaymentInfo } from "./Model/IPaymentInfo"

export default class DataLoader {
    getPayments(filterDate: string, onSuccess: (payments: IPaymentInfo[]) => void, onRejected: any) {
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

    addPayment(data: string): Promise<Response> {
        return fetch('/Payment/AddPayment', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: data,
        });
    }

    updatePayment(data: string): Promise<Response> {
        return fetch('/Payment/AddPayment', {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: data,
        });
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

    getBankAccounts(onSuccess: any, onRejected: any): Promise<Response> {
        return fetch("/Payment/GetBankAccounts")
            .then(res => {
                if (res.ok)
                    return res.json()
                else
                    onRejected();
            })
            .then(
                (result) => {
                    onSuccess(result);
                },
                (_) => onRejected()
            );;
    }

    getPayment(id: number): Promise<Response> {
        return fetch(`/Payment/GetPayment/${id}`);
    }
}