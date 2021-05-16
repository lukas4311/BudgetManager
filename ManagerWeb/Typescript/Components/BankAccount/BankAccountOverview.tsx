import React from "react";
import { Configuration, CryptoApi, CryptoApiInterface } from "../../ApiClient";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { BaseList } from "../BaseList";
import BankAccountViewModel from "../../Model/BankAccountViewModel";

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
    cryptoInterface: CryptoApiInterface;

    constructor(props: {}) {
        super(props);
        this.cryptoInterface = new CryptoApi(new Configuration({ basePath: "https://localhost:5001" }));
        this.state = { bankAccounts: [], openedForm: false, selectedBankAccount: undefined };
    }

    public componentDidMount() {
        this.load();
    }

    private async load(): Promise<void> {
        // let tradesData: TradeHistory[] = await this.cryptoInterface.cryptoGetAllGet();
        // this.setState({ bankAccounts: trades });
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