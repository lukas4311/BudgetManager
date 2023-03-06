import React from "react";
import { BankAccountApi } from "../../ApiClient/Main";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { BaseList } from "../BaseList";
import BankAccountViewModel from "../../Model/BankAccountViewModel";
import { Dialog, DialogContent, DialogTitle } from '@material-ui/core';
import { BankAccountForm } from "./BankAccountForm";
import { BankAccount } from "../../Model/BankAccount";
import { RouteComponentProps } from "react-router-dom";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { MainFrame } from "../MainFrame";
import { ComponentPanel } from "../../Utils/ComponentPanel";
import BankAccountService from "../../Services/BankAccountService";

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

export default class BankAccountOverview extends React.Component<RouteComponentProps, BankAccountOverviewState> {
    private bankAccountService: BankAccountService;

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { bankAccounts: [], selectedBankAccount: undefined, showForm: false, formKey: Date.now(), selectedId: undefined };
    }

    public componentDidMount = () => this.init();

    private async init(): Promise<void> {
        const apiFactory = new ApiClientFactory(this.props.history);
        const bankAccountApi = await apiFactory.getClient(BankAccountApi);
        this.bankAccountService = new BankAccountService(bankAccountApi);
        await this.loadBankAccounts();
    }

    private loadBankAccounts = async () => {
        let bankViewModels = await this.bankAccountService.getAllBankAccounts();
        this.setState({ bankAccounts: bankViewModels });
    }

    private renderHeader = (): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-2/3 text-left">Account name</p>
                <p className="mx-6 my-1 w-1/3 text-left">Initial balance</p>
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

    private addNewItem = (): void =>
        this.setState({ showForm: true, formKey: Date.now(), selectedBankAccount: undefined });

    private bankEdit = async (id: number): Promise<void> => {
        let selectedBankAccount = this.state.bankAccounts.filter(t => t.id == id)[0];
        this.setState({ showForm: true, selectedBankAccount: selectedBankAccount, selectedId: id });
    }

    private hideForm = (): void =>
        this.setState({ showForm: false, formKey: Date.now(), selectedId: undefined });

    private saveFormData = async (model: BankAccountViewModel) => {
        try {
            if (model.id != undefined)
                await this.bankAccountService.updateBankAccount(model);
            else
                await this.bankAccountService.createBankAccount(model);
        } catch (error) {
            console.log(error);
        }

        this.hideForm();
        await this.loadBankAccounts();
    }

    private deleteBank = (id: number) =>
        this.bankAccountService.deleteBankAccount(id);

    render() {
        return (
            <ThemeProvider theme={theme}>
                <MainFrame header='Bank accounts overview'>
                    <ComponentPanel classStyle="w-2/3 mx-auto">
                        <div className="flex">
                            <div className="w-full overflow-y-auto">
                                <div className="h-full">
                                    <div className="">
                                        <BaseList<BankAccountViewModel> title="Bank account" data={this.state.bankAccounts} template={this.renderTemplate}
                                            header={this.renderHeader()} addItemHandler={this.addNewItem} itemClickHandler={this.bankEdit}
                                            deleteItemHandler={this.deleteBank} dataAreaClass="h-70vh overflow-y-auto">
                                        </BaseList>
                                    </div>
                                    <Dialog open={this.state.showForm} onClose={this.hideForm} aria-labelledby="Bank account detail"
                                        maxWidth="sm" fullWidth={true}>
                                        <DialogTitle id="form-dialog-title" className="bg-prussianBlue">Bank account detail</DialogTitle>
                                        <DialogContent className="bg-prussianBlue">
                                            <div className="p-2 overflow-y-auto">
                                                <BankAccountForm key={this.state.formKey} {...this.state.selectedBankAccount} onSave={this.saveFormData}></BankAccountForm>
                                            </div>
                                        </DialogContent>
                                    </Dialog>
                                </div>
                            </div>
                        </div>
                    </ComponentPanel>
                </MainFrame>
            </ThemeProvider>
        );
    }
}