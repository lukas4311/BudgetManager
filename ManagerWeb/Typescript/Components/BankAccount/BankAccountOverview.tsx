import React from "react";
import { BankAccountApi, BankAccountApiInterface, Configuration, CryptoApi, CryptoApiInterface } from "../../ApiClient";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { BaseList } from "../BaseList";
import BankAccountViewModel from "../../Model/BankAccountViewModel";
import { BankAccountModel } from "../../ApiClient/models/BankAccountModel";

class BankAccountOverviewState {
    bankAccounts: BankAccountViewModel[];
    openedForm: boolean;
    selectedBankAccount: BankAccountViewModel;
}

const theme = createMuiTheme({
    palette: {
        type: 'dark',
    }
});

export default class BankAccountOverview extends React.Component<{}, BankAccountOverviewState> {
    bankAccountApi: BankAccountApiInterface;

    constructor(props: {}) {
        super(props);
        this.bankAccountApi = new BankAccountApi(new Configuration({ basePath: "https://localhost:5001" }));
        this.state = { bankAccounts: [], openedForm: false, selectedBankAccount: undefined };
    }

    public componentDidMount() {
        this.load();
    }

    private async load(): Promise<void> {
        let bankAccounts: BankAccountModel[] = await this.bankAccountApi.bankAccountGetAllGet();
        let bankViewModels: BankAccountViewModel[] = this.getMappedViewModels(bankAccounts);
        this.setState({ bankAccounts: bankViewModels });
    }

    private getMappedViewModels = (bankAccountModels: BankAccountModel[]): BankAccountViewModel[] => {
        return bankAccountModels.map(b => this.mapDataModelToViewModel(b));
    }

    private mapDataModelToViewModel = (bankAccountModels: BankAccountModel): BankAccountViewModel => {
        let viewModel = new BankAccountViewModel();
        viewModel.code = bankAccountModels.code;
        viewModel.id = bankAccountModels.id;
        viewModel.openingBalance = bankAccountModels.openingBalance;

        return viewModel;
    }

    private renderHeader = (): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/10">Název účtu</p>
            </>
        );
    }

    private renderTemplate = (p: BankAccountViewModel): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/10">{p.code.toUpperCase()}</p>
            </>
        );
    }

    private addNewItem = (): void => {
        // this.setState({ showBudgetFormModal: true, budgetFormKey: Date.now(), selectedBudget: undefined });
    }

    private budgetEdit = async (id: number): Promise<void> => {
        let selectedBankAccount = this.state.bankAccounts.filter(t => t.id == id)[0];
        this.setState({ selectedBankAccount: selectedBankAccount, openedForm: true });
    }

    render() {
        return (
            <div className="pr-5 h-full">
                <ThemeProvider theme={theme}>
                    <BaseList<BankAccountViewModel> title="Bankovní účet" data={this.state.bankAccounts} template={this.renderTemplate}
                        header={this.renderHeader()} addItemHandler={this.addNewItem} itemClickHandler={this.budgetEdit} dataAreaClass="h-70vh overflow-y-auto">
                    </BaseList>
                </ThemeProvider>
            </div>
        );
    }
}