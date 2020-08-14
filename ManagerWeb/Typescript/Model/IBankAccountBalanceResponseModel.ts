import { IBankAccountsBalanceModel } from './IBankAccountsBalanceModel';

export interface IBankAccountBalanceResponseModel{
    success: boolean;
    bankAccountsBalance: IBankAccountsBalanceModel[];
}
