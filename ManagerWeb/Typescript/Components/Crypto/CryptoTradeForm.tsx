import * as React from 'react'
import { useForm } from "react-hook-form";
import Button from "@material-ui/core/Button";
import TextField from "@material-ui/core/TextField";
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

const CryptoTradeForm = () => {
    const { register, control, handleSubmit } = useForm<ICryptoTradeFormProps>();

    const onSubmit = (data: ICryptoTradeFormProps) => {
        console.log(data)
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <Paper className="p-8">
                <h1 className="text-2xl mb-4" >Detail tradu</h1>

                <div className="grid grid-cols-2 gap-4 mb-4">
                    <div>
                        <TextField
                            id="date"
                            label="Datum tradu"
                            type="date"
                            defaultValue="2017-05-24"
                            inputRef={register}
                            InputLabelProps={{
                                shrink: true,
                            }}
                        />
                    </div>
                    <div>
                        <TextField inputRef={register} name="cryptoTicker" className="place-self-end" label="Crypto ticker"/>
                    </div>
                </div>

                <Button type="submit" variant="contained" color="primary">Uložit</Button>
            </Paper>
        </form>
    );
};

export { CryptoTradeForm }