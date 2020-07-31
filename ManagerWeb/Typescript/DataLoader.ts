export default class DataLoader {
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
}