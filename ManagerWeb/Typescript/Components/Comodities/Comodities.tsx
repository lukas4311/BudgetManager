import _ from "lodash";
import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { ComodityApi } from "../../ApiClient/Main/apis";
import { ComodityTradeHistoryModel } from "../../ApiClient/Main/models";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import Gold from "./Gold";
import { GoldIngot } from "./GoldIngot";

class ComoditiesState {
    goldIngots: GoldIngot[];
}

export default class Comodities extends React.Component<RouteComponentProps, ComoditiesState>{
    private comodityApi: ComodityApi;
    private goldCode: string = 'AU';

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { goldIngots: [] };
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

    public render() {
        return (
            <div className="">
                <p className="text-3xl text-center mt-6">Comodities overview</p>
                <div className="flex">
                    <div className="w-4/12 p-4 overflow-y-auto"><Gold goldIngots={this.state.goldIngots} routeComponent={this.props.history} addNewIngot={() => console.log("New ingot")}/></div>
                    <div className="w-4/12 p-4 overflow-y-auto">Silver component</div>
                    <div className="w-4/12 p-4 overflow-y-auto">Others</div>
                </div>
            </div>
        );
    }
}