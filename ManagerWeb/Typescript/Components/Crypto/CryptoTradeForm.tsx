import * as React from 'react'
import { Controller, useForm } from "react-hook-form";
import { Button } from "@material-ui/core";
import { TextField } from "@material-ui/core";
import { IBaseModel } from '../BaseList';

class CryptoTradeViewModel implements IBaseModel {
    id: number;
    tradeTimeStamp: string;
    cryptoTickerId: number;
    cryptoTicker: string;
    tradeSize: number;
    tradeValue: number;
    currencySymbolId: number;
    currencySymbol: string;
    onSave: (data: CryptoTradeViewModel) => void;
}

const CryptoTradeForm = (props: CryptoTradeViewModel) => {
    const { handleSubmit, control } = useForm<CryptoTradeViewModel>({ defaultValues: { ...props } });

    const onSubmit = (data: CryptoTradeViewModel) => {
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div className="col-span-2">
                    <Controller render={({ field }) => <TextField label="Datum tradu" type="date" value={field.value} {...field} className="place-self-end" InputLabelProps={{ shrink: true }} />}
                        name="tradeTimeStamp" defaultValue={props.tradeTimeStamp} control={control} />
                </div>
                <div>
                    <Controller render={({ field }) => <TextField label="Crypto ticker" type="text" {...field} className="place-self-end" />}
                        name="cryptoTicker" control={control} />
                </div>
                <div>
                    <Controller render={({ field }) => <TextField label="Velikost tradu" type="text" {...field} className="place-self-end" />}
                        name="tradeSize" control={control} />
                </div>
                <div>
                    <Controller render={({ field }) => <TextField label="Hodnota tradu" type="text" {...field} className="place-self-end" />}
                        name="tradeValue" control={control} />
                </div>
                <div>
                    <Controller render={({ field }) => <TextField label="Zdrojová měna tradu" type="text" {...field} className="place-self-end" />}
                        name="currencySymbol" control={control} />
                </div>
            </div>

            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Uložit</Button>
        </form>
    );
};

export { CryptoTradeForm, CryptoTradeViewModel }