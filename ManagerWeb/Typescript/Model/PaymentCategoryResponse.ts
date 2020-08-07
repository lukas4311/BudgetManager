import { PaymentCategory } from "./PaymentCategory";

interface PaymentCategoryResponse {
    success: boolean;
    categories: Array<PaymentCategory>;
}

export { PaymentCategoryResponse };