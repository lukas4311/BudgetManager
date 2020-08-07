import { PaymentType } from "./PaymentType";

interface PaymentTypeResponse {
    success: boolean;
    types: Array<PaymentType>;
}

export { PaymentTypeResponse };