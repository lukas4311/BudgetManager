import BankAccountViewModel from "../Model/BankAccountViewModel";

export interface IBankAccountService {
    getAllBankAccounts(): Promise<BankAccountViewModel[]>;
    updateBankAccount(model: BankAccountViewModel): Promise<void>;
    createBankAccount(model: BankAccountViewModel): Promise<void>;
    deleteBankAccount: (id: number) => Promise<void>;
}
