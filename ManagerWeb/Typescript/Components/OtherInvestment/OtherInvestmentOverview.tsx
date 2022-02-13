import React from "react";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { BaseList } from "../BaseList";
import moment from "moment";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { RouteComponentProps } from 'react-router-dom';
import { OtherInvestmentApi, OtherInvestmentModel } from "../../ApiClient/Main";

const theme = createMuiTheme({
    palette: {
        type: 'dark',
        primary: {
            main: "#e03d15ff",
        }
    }
});


class OtherInvestmentOverviewState {
    otherInvesmnets: OtherInvestmentModel[]
}

export default class OtherInvestmentOverview extends React.Component<RouteComponentProps, OtherInvestmentOverviewState>{
    private otherInvestmentApi: OtherInvestmentApi;

    constructor(props: RouteComponentProps) {
        super(props);
        this.state = { otherInvesmnets: [] };
    }

    public componentDidMount = () => this.init();

    private init = async () => {
        const apiFactory = new ApiClientFactory(this.props.history);
        this.otherInvestmentApi = await apiFactory.getClient(OtherInvestmentApi);
        const data: OtherInvestmentModel[] = await this.otherInvestmentApi.otherInvestmentAllGet();
        this.setState({ otherInvesmnets: data });
    }

    private renderTemplate = (p: OtherInvestmentModel): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/2">{p.name},-</p>
                <p className="mx-6 my-1 w-1/2">{p.openingBalance}</p>
            </>
        );
    }

    private renderHeader = (): JSX.Element => {
        return (
            <>
                <p className="mx-6 my-1 w-1/2">Investment name</p>
                <p className="mx-6 my-1 w-1/2">Opening balance</p>
            </>
        );
    }

    public render() {
        return (
            <ThemeProvider theme={theme}>
                <h2 className="text-xl p-4 text-center">Other investments</h2>
                <div className="text-center mt-4 bg-prussianBlue rounded-lg">
                    <h2 className="text-2xl"></h2>
                    <div className="grid grid-cols-2">
                        <div>
                            <div className="pb-10 h-64 overflow-y-scroll pr-4">
                                <BaseList<OtherInvestmentModel> data={this.state.otherInvesmnets} template={this.renderTemplate} header={this.renderHeader()}></BaseList>
                            </div>
                        </div>
                        <div>Detail</div>
                        <div className="col-span-2">Overview</div>
                    </div>
                </div>
            </ThemeProvider>
        );
    }
}