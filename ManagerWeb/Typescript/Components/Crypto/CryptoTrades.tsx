import Button from "@material-ui/core/Button";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import TextField from "@material-ui/core/TextField";
import moment from "moment";
import React from "react";
import { Configuration, CryptoApi, CryptoApiInterface, TradeHistory } from "../../ApiClient";
import { CryptoTradeForm } from "./CryptoTradeForm";
import { createMuiTheme } from "@material-ui/core/styles";
import { ThemeProvider } from "@material-ui/styles";

class CryptoTradesState {
    trades: TradeHistory[];
    openedForm: boolean;
}

const theme = createMuiTheme({
    palette: {
        type: "dark"
    }
});

export default class CryptoTrades extends React.Component<{}, CryptoTradesState> {
    cryptoInterface: CryptoApiInterface;

    constructor(props: {}) {
        super(props);
        this.cryptoInterface = new CryptoApi(new Configuration({ basePath: "https://localhost:5001" }));
        this.state = { trades: undefined, openedForm: false };
    }

    public componentDidMount() {
        this.load();
    }

    private async load(): Promise<void> {
        let trades: TradeHistory[] = await this.cryptoInterface.cryptoGetAllGet();
        trades.sort((a, b) => moment(a.tradeTimeStamp).format("YYYY-MM-DD") > moment(b.tradeTimeStamp).format("YYYY-MM-DD") ? 1 : -1);
        this.setState({ trades });
    }

    handleClickOpen = () => {
        this.setState({ openedForm: true });
    };

    handleClose = () => {
        this.setState({ openedForm: false });
    };

    render() {
        return (
            <div className="">
                <ThemeProvider theme={theme}>
                    <h2 className="text-xl ml-12 mb-10">Seznam plateb</h2>
                    {this.state.trades != undefined ?
                        <div className="pb-10 max-h-96 overflow-x-auto">
                            <div className="font-bold bg-battleshipGrey rounded-r-full flex mr-6 mt-1 hover:bg-vermilion cursor-pointer">
                                <p className="mx-6 my-1 w-1/10">Ticker</p>
                                <p className="mx-6 my-1 w-3/10">Velikost tradu</p>
                                <p className="mx-6 my-1 w-2/10">Datum tradu</p>
                                <p className="mx-6 my-1 w-3/10">Celkova hodnota</p>
                                <p className="mx-6 my-1 w-1/10">Měna</p>
                            </div>
                            {this.state.trades.map(p =>
                                <div key={p.id} className="paymentRecord bg-battleshipGrey rounded-r-full flex mr-6 mt-1 hover:bg-vermilion cursor-pointer">
                                    <p className="mx-6 my-1 w-1/10">{p.cryptoTicker.toUpperCase()}</p>
                                    <p className="mx-6 my-1 w-3/10">{p.tradeSize}</p>
                                    <p className="mx-6 my-1 w-2/10">{moment(p.tradeTimeStamp).format('DD.MM.YYYY')}</p>
                                    <p className="mx-6 my-1 w-3/10">{p.tradeValue.toFixed(2)}</p>
                                    <p className="mx-6 my-1 w-1/10">{p.currencySymbol}</p>
                                </div>
                            )}
                        </div>
                        : <div>
                            <p>Probíhá načátíní</p>
                        </div>
                    }
                    <Button variant="outlined" color="primary" onClick={this.handleClickOpen}>
                        Open form dialog
                    </Button>
                    <Dialog open={this.state.openedForm} onClose={this.handleClose} aria-labelledby="form-dialog-title">
                        <DialogTitle id="form-dialog-title">Subscribe</DialogTitle>
                        <DialogContent>
                            <div className="p-2 overflow-y-auto">
                                <CryptoTradeForm
                                    tradeTimeStamp={moment(Date.now()).format("YYYY-MM-DD")}
                                    cryptoTicker="A"
                                    currencySymbolId={1}
                                    tradeSize={20}
                                    id={1}
                                    cryptoTickerId={2}
                                    currencySymbol={"s"}
                                    tradeValue={222}
                                    onSave={(data) => console.log(data)}
                                ></CryptoTradeForm>
                            </div>
                        </DialogContent>
                    </Dialog>
                </ThemeProvider>
            </div>
        );
    }
}