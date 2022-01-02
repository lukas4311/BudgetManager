import _ from "lodash";
import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { ComodityApi } from "../../ApiClient/Main/apis";
import { ComodityTradeHistoryModel } from "../../ApiClient/Main/models";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import Gold from "./Gold";
import { GoldIngot } from "./GoldIngot";
import { Dialog, DialogContent, DialogTitle } from "@material-ui/core";

const theme = createMuiTheme({
    palette: {
        type: 'dark',
    }
});

class ComoditiesState {
    goldIngots: GoldIngot[];
    openedForm: boolean;
    dialogTitle: string
}

export default class Comodities extends React.Component<RouteComponentProps, ComoditiesState>{
    private comodityApi: ComodityApi;
    private goldCode: string = 'AU';

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { goldIngots: [], openedForm: false, dialogTitle: "" };
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

        const goldType = comodityType.filter(c => c.code == this.goldCode)[0];
        const goldIngots = data.filter(a => a.comodityTypeId == goldType.id).map<GoldIngot>(c => ({ id: c.id, company: c.company, weight: c.tradeSize, boughtDate: c.tradeTimeStamp, unit: c.comodityUnit, costs: c.tradeValue, currency: c.currencySymbol }));
        this.setState({ goldIngots: goldIngots });
    }

    private addNewGold = () => {
        this.setState({ openedForm: true, dialogTitle: "Přidat zlato" });
    }

    private editGold = (id: number) => {
        this.setState({ openedForm: true, dialogTitle: "Upravit zlato" });
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
                            <p>Form</p>
                        </DialogContent>
                    </Dialog>
                </ThemeProvider>
            </div>
        );
    }
}