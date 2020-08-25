interface IPaymentInfo {
    name: string,
    amount: number,
    date: string,
    id: number,
    bankAccountId: number,
    paymentTypeCode: string,
    paymentCategoryIcon: string,
    paymentCategoryCode: string,
}

export { IPaymentInfo };