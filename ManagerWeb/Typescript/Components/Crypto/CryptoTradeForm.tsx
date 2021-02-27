import * as React from 'react'
import { useForm } from "react-hook-form";
import Button from "@material-ui/core/Button";
import TextField from "@material-ui/core/TextField";

class CryptoTradeFormModel {
    id: number;
    tradeTimeStamp: string;
    cryptoTickerId: number;
    cryptoTicker: string;
    tradeSize: number;
    tradeValue: number;
    currencySymbolId: number;
    currencySymbol: string;
    onSave: (data: CryptoTradeFormModel) => void;
}

const CryptoTradeForm = (props: CryptoTradeFormModel) => {
    const { register, handleSubmit } = useForm<CryptoTradeFormModel>({ defaultValues: props });

    const onSubmit = (data: CryptoTradeFormModel) => {
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
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
        </form>
    );
};

export { CryptoTradeForm, CryptoTradeFormModel }