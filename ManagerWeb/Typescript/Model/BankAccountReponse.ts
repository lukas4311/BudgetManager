import { BankAccount } from './BankAccount';

interface BankAccountReponse {
    success: boolean;
    bankAccounts: Array<BankAccount>;
}

export { BankAccountReponse };