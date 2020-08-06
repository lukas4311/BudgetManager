interface IPaymentInfo {
    name: string,
    amount: number,
    date: string,
    id: number,
    bankAccountId: number,
    paymentTypeCode: string
}

export { IPaymentInfo };