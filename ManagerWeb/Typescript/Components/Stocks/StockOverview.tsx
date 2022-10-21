import React from "react";
import { RouteComponentProps } from "react-router-dom";
import { createMuiTheme, ThemeProvider } from "@material-ui/core/styles";
import { MainFrame } from "../MainFrame";
import { BaseList, IBaseModel } from "../BaseList";

const theme = createMuiTheme({
    palette: {
        type: 'dark',
        primary: {
            main: "#e03d15ff",
        }
    }
});

interface StockOverviewState {
    stocks: StockViewModel[];
}

class StockViewModel implements IBaseModel {
    id: number;
    tradeTimeStamp: string;
    stockTickerId: number;
    tradeSize: number;
    tradeValue: number;
    currencySymbolId: number;
    currencySymbol: string;
    onSave: (data: StockViewModel) => void;
}

class StockOverview extends React.Component<RouteComponentProps, StockOverviewState> {

    constructor(props: RouteComponentProps) {
        super(props);
    }

    private renderTemplate = (): JSX.Element => {
        return (<p>Template</p>);
    }

    private renderHeader = (): JSX.Element => {
        return (<p>Header</p>);
    }

    private addStockTrade = (): void => {
    }

    private editStock = (id: number): void => {
    }

    render() {
        return (
            <ThemeProvider theme={theme}>
                <MainFrame header='Stocks'>
                    <>
                        <div className="flex flex-row">
                            <div className="w-2/5">
                                <div className="m-5 overflow-y-scroll">
                                    <BaseList<StockViewModel> data={this.state.stocks} template={this.renderTemplate} header={this.renderHeader()}
                                        addItemHandler={this.addStockTrade} itemClickHandler={this.editStock} useRowBorderColor={true} hideIconRowPart={true}></BaseList>
                                </div>
                            </div>
                        </div>
                    </>
                </MainFrame>
            </ThemeProvider>
        );
    }
}

export default StockOverview;