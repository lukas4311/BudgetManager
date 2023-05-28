import { BankAccountApiInterface } from "../ApiClient/Main/apis";
import { BankAccount, BankAccountModel } from "../ApiClient/Main/models";
import BankAccountViewModel from "../Model/BankAccountViewModel";
import { IBankAccountService } from "./IBankAccountService";

export default class BankAccountService implements IBankAccountService {
    private bankAccountApi: BankAccountApiInterface;

    constructor(bankAccountApi: BankAccountApiInterface) {
        this.bankAccountApi = bankAccountApi;
    }

    public async getAllBankAccounts(): Promise<BankAccountViewModel[]> {
        let bankAccounts: BankAccountModel[] = await this.bankAccountApi.bankAccountsAllGet();
        let bankViewModels: BankAccountViewModel[] = this.getMappedViewModels(bankAccounts);
        return bankViewModels;
    }

    public async updateBankAccount(model: BankAccountViewModel) {
        let bankModel: BankAccount = {
            code: model.code, id: model.id, openingBalance: parseInt(model.openingBalance.toString())
        };

        await this.bankAccountApi.bankAccountsPut({ bankAccountModel: bankModel });
    }

    public async createBankAccount(model: BankAccountViewModel) {
        let bankModel: BankAccount = {
            code: model.code, id: model.id, openingBalance: parseInt(model.openingBalance.toString())
        };

        await this.bankAccountApi.bankAccountsPost({ bankAccountModel: bankModel });
    }

    public deleteBankAccount = async (id: number) =>
        await this.bankAccountApi.bankAccountsDelete({ body: id });

    private getMappedViewModels = (bankAccountModels: BankAccountModel[]): BankAccountViewModel[] =>
        bankAccountModels.map(b => this.mapDataModelToViewModel(b));


    private mapDataModelToViewModel = (bankAccountModels: BankAccountModel): BankAccountViewModel => {
        let viewModel = new BankAccountViewModel();
        viewModel.code = bankAccountModels.code;
        viewModel.id = bankAccountModels.id;
        viewModel.openingBalance = bankAccountModels.openingBalance;
        return viewModel;
    }
}