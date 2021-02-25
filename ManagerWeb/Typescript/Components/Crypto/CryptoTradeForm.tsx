import * as React from 'react'
import { useForm } from "react-hook-form";
import Button from "@material-ui/core/Button";
import TextField from "@material-ui/core/TextField";
import { createMuiTheme } from "@material-ui/core/styles";
import { ThemeProvider } from "@material-ui/styles";
import Paper from '@material-ui/core/Paper';

interface ICryptoTradeFormProps {
    id: number;
    tradeTimeStamp: Date;
    cryptoTickerId: number;
    cryptoTicker: string;
    tradeSize: number;
    tradeValue: number;
    currencySymbolId: number;
    currencySymbol: string;
}

const defaults = {
    tradeTimeStamp: "2021-01-01",
    cryptoTicker: "ticker test"
};

const theme = createMuiTheme({
    palette: {
      type: "dark"
    }
  });

const CryptoTradeForm = () => {
    const { register, control, handleSubmit } = useForm<ICryptoTradeFormProps>({ defaultValues: defaults });

    const onSubmit = (data: ICryptoTradeFormProps) => {
        console.log(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <ThemeProvider theme={theme}>
                <Paper className="p-8">
                    <h1 className="text-2xl mb-4" >Detail tradu</h1>

                    <div className="grid grid-cols-2 gap-4 mb-4">
                        <div>
                            <TextField
                                label="Datum tradu"
                                type="date"
                                name="tradeTimeStamp"
                                inputRef={register}
                            />
                        </div>
                        <div>
                            <TextField inputRef={register} name="cryptoTicker" className="place-self-end" label="Crypto ticker" />
                        </div>
                    </div>

                    <Button type="submit" variant="contained" color="primary">Uložit</Button>
                </Paper>
            </ThemeProvider>
        </form>
    );
};

export { CryptoTradeForm }