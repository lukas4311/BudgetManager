import { Button, TextField } from "@mui/material";
import React from "react";
import { Controller, useForm } from "react-hook-form";

class FixFormProps {
    onSave: (priceTicker: string, metadataTicker: string) => void;
    hasMetadata: boolean;
    hasPrice: boolean;
    tickerId: number;
}

const FixTickerForm = (props: FixFormProps) => {
    const { handleSubmit, control } = useForm<FixTickerModel>({ defaultValues: { priceTicker: '', priceMetadata: '' } });

    const onSubmit = (data: FixTickerModel) => {
        props.onSave(data.priceTicker, data.priceMetadata);
    };

    return (
        (<form onSubmit={handleSubmit(onSubmit)}>
            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Fix tickers</Button>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                {!props.hasMetadata ? <></> :
                    <div className="w-2/3">
                        <p>Fill ticker from Tradingview to get metadata. After saving wait 1 day to get metadata for this ticker.</p>
                        <Controller render={({ field }) => <TextField label="Ticker for metadata" size='small' type="text" {...field} className="place-self-end w-full" />}
                            name="priceMetadata" control={control} />
                    </div>
                }
                {!props.hasPrice ? <></> :
                    <div className="w-2/3">
                        <p>Fill ticker from Yahoo Finance to get price.</p>
                        <Controller render={({ field }) => <TextField label="Ticker for price" size='small' type="text" {...field} className="place-self-end w-full" />}
                            name="priceTicker" control={control} />
                    </div>
                }
            </div>
        </form>)
    );
};

export { FixTickerForm, FixFormProps }

export class FixTickerModel {
    public priceTicker: string;
    public priceMetadata: string;
}