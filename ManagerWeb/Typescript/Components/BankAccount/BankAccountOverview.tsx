import React from "react";
import { BankAccountApi, BankAccountApiInterface, Configuration, CryptoApi, CryptoApiInterface } from "../../ApiClient";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { BaseList } from "../BaseList";
import BankAccountViewModel from "../../Model/BankAccountViewModel";
import { BankAccountModel } from "../../ApiClient/models/BankAccountModel";
import { Dialog, DialogContent, DialogTitle } from '@material-ui/core';
import { BankAccountForm, BankAccountFromViewModel } from "./BankAccountForm";
import { BankAccount } from "../../Model/BankAccount";

class BankAccountOverviewState {
    bankAccounts: BankAccountViewModel[];
    selectedBankAccount: BankAccountViewModel;
    selectedId: number;
    showForm: boolean;
    formKey: number;
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
        this.state = { bankAccounts: [], selectedBankAccount: undefined, showForm: false, formKey: Date.now(), selectedId: undefined };
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
                <p className="mx-6 my-1 w-2/3 text-left">Název účtu</p>
                <p className="mx-6 my-1 w-1/3 text-left">Počáteční stav</p>
            </>
        );
    }

    private renderTemplate = (p: BankAccountViewModel): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-2/3">{p.code.toUpperCase()}</p>
                <p className="mx-6 my-1 w-1/3">{p.openingBalance}</p>
            </>
        );
    }

    private addNewItem = (): void => {
        this.setState({ showForm: true, formKey: Date.now(), selectedBankAccount: undefined });
    }

    private bankEdit = async (id: number): Promise<void> => {
        let selectedBankAccount = this.state.bankAccounts.filter(t => t.id == id)[0];
        this.setState({ showForm: true, selectedBankAccount: selectedBankAccount, selectedId: id });
    }

    private hideForm = (): void => {
        this.setState({ showForm: false, formKey: Date.now(), selectedId: undefined });
    }

    private saveFormData = async (model: BankAccountViewModel) => {
        let bankModel: BankAccount = {
            code: model.code, id: model.id, openingBalance: parseInt(model.openingBalance.toString())
        };

        try {
            if (model.id != undefined)
                await this.bankAccountApi.bankAccountUpdatePut({ bankAccountModel: bankModel });
            else
                await this.bankAccountApi.bankAccountAddPost({ bankAccountModel: bankModel });
        } catch (error) {
            console.log(error);
        }

        this.hideForm();
    }

    private deleteBank = (id: number) => {
        this.bankAccountApi.bankAccountDeleteDelete({ body: id });
    }

    render() {
        return (
            <div className="h-full">
                <ThemeProvider theme={theme}>
                    <div className="w-full lg:w-1/2">
                        <BaseList<BankAccountViewModel> title="Bankovní účet" data={this.state.bankAccounts} template={this.renderTemplate}
                            header={this.renderHeader()} addItemHandler={this.addNewItem} itemClickHandler={this.bankEdit}
                            deleteItemHandler={this.deleteBank} dataAreaClass="h-70vh overflow-y-auto">
                        </BaseList>
                    </div>
                    <Dialog open={this.state.showForm} onClose={this.hideForm} aria-labelledby="Detail rozpočtu"
                        maxWidth="sm" fullWidth={true}>
                        <DialogTitle id="form-dialog-title">Detail rozpočtu</DialogTitle>
                        <DialogContent>
                            <div className="p-2 overflow-y-auto">
                                <BankAccountForm key={this.state.formKey} {...this.state.selectedBankAccount} onSave={this.saveFormData}></BankAccountForm>
                            </div>
                        </DialogContent>
                    </Dialog>
                </ThemeProvider>
            </div>
        );
    }
}