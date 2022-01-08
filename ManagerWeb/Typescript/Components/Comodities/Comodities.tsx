import _ from "lodash";
import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { ComodityApi } from "../../ApiClient/Main/apis";
import { ComodityTradeHistoryModel, ComodityTypeModel } from "../../ApiClient/Main/models";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import Gold from "./Gold";
import { GoldIngot } from "./GoldIngot";
import { Dialog, DialogContent, DialogTitle } from "@material-ui/core";
import { ComoditiesForm, ComoditiesFormViewModel } from "./ComoditiesForm";
import moment from "moment";

const theme = createMuiTheme({
    palette: {
        type: 'dark',
    }
});

class ComoditiesState {
    goldIngots: GoldIngot[];
    openedForm: boolean;
    dialogTitle: string;
    selectedModel: ComoditiesFormViewModel;
    formKey: number;
}

export default class Comodities extends React.Component<RouteComponentProps, ComoditiesState>{
    private comodityApi: ComodityApi;
    private goldCode: string = 'AU';
    private goldType: ComodityTypeModel;

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { goldIngots: [], openedForm: false, dialogTitle: "", selectedModel: undefined, formKey: Date.now() };
    }

    componentDidMount(): void {
        this.init();
    }

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.comodityApi = await apiFactory.getClient(ComodityApi);
        this.loadData();
    }

    private loadData = async () => {
        let data: ComodityTradeHistoryModel[] = await this.comodityApi.comoditiesAllGet();
        let comodityType = await this.comodityApi.comoditiesComodityTypeAllGet();

        this.goldType = comodityType.filter(c => c.code == this.goldCode)[0];
        const goldIngots = data.filter(a => a.comodityTypeId == this.goldType.id).map<GoldIngot>(c => ({ id: c.id, company: c.company, weight: c.tradeSize, boughtDate: c.tradeTimeStamp, unit: c.comodityUnit, costs: c.tradeValue, currency: c.currencySymbol }));
        this.setState({ goldIngots: goldIngots });
    }

    private addNewGold = () => {
        let model: ComoditiesFormViewModel = new ComoditiesFormViewModel();
        model.onSave = () => console.log('Saved');
        model.buyTimeStamp = moment().format("YYYY-MM-DD");
        model.comodityTypeName = "Gold";
        model.comodityUnit = this.goldType.comodityUnit;
        model.price = 0;
        model.comodityAmount = 0;
        this.setState({ openedForm: true, formKey: Date.now(), selectedModel: model });
    }

    private editGold = (id: number) => {
        this.setState({ openedForm: true, dialogTitle: "Upravit zlato" });
    }

    private budgetEdit = async (id: number): Promise<void> => {
        // let tradeHistory = this.state.trades.filter(t => t.id == id)[0];
        // this.setState({ selectedTrade: tradeHistory, openedForm: true });
    }

    private handleClose = () => this.setState({ openedForm: false });

    public render() {
        return (
            <div className="">
                <ThemeProvider theme={theme}>
                    <p className="text-3xl text-center mt-6">Comodities overview</p>
                    <div className="flex">
                        <div className="w-4/12 p-4 overflow-y-auto"><Gold goldIngots={this.state.goldIngots} routeComponent={this.props.history}
                            addNewIngot={() => this.addNewGold()} editIngot={this.editGold} /></div>
                        <div className="w-4/12 p-4 overflow-y-auto">Silver component</div>
                        <div className="w-4/12 p-4 overflow-y-auto">Others</div>
                    </div>
                    <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="Detail transakce"
                        maxWidth="md" fullWidth={true}>
                        <DialogTitle id="form-dialog-title">Zlatý slitek</DialogTitle>
                        <DialogContent>
                            <ComoditiesForm {...this.state.selectedModel} />
                        </DialogContent>
                    </Dialog>
                </ThemeProvider>
            </div>
        );
    }
}