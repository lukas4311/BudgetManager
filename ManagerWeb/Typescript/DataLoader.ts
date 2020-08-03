export default class DataLoader {
    getPayments(filterDate: string){
        return fetch("/Payment/GetPaymentsData?fromDate=" + filterDate)
    }

    addPayment(data: string) : Promise<Response> {
        return fetch('/Payment/AddPayment', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: data,
        });
    }

    getPaymentsData(filterDate: string): Promise<Response> {
        return fetch("/Payment/GetPaymentsData?fromDate=" + filterDate);
    }

    getPaymentTypes() : Promise<Response>{
        return fetch("/Payment/GetPaymentTypes");
    }

    getPaymentCategories() : Promise<Response>{
        return fetch("/Payment/GetPaymentCategories");
    }

    getBankAccounts() : Promise<Response>{
        return fetch("/Payment/GetBankAccounts");
    }

    getPayment(id: number): Promise<Response>{
        return fetch(`/Payment/GetPayment/${id}`);
    }
}