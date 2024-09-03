import { Button, TextField } from "@mui/material";
import React from "react";
import { Controller, useForm } from "react-hook-form";

class FixFormProps {
    onSave: (priceTicker: string, metadataTicker: string) => void;
    hasMetadata: boolean;
    hasPrice: boolean;
}

const FixForm = (props: FixFormProps) => {
    const { handleSubmit, control } = useForm<FixTickerModel>({ defaultValues: { priceTicker: '', priceMetadata: '' } });

    const onSubmit = (data: FixTickerModel) => {
        props.onSave(data.priceTicker, data.priceMetadata);
    };

    return (
        (<form onSubmit={handleSubmit(onSubmit)}>
            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Send request</Button>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                {!props.hasPrice ? <></> :
                    <div className="w-2/3">
                        <Controller render={({ field }) => <TextField label="Ticker" size='small' type="text" {...field} className="place-self-end w-full" />}
                            name="priceTicker" control={control} />
                    </div>
                }
                {!props.hasMetadata ? <></> :
                    <div className="w-2/3">
                        <Controller render={({ field }) => <TextField label="Ticker" size='small' type="text" {...field} className="place-self-end w-full" />}
                            name="priceMetadata" control={control} />
                    </div>
                }
            </div>
        </form>)
    );
};

export { FixFormProps }

export class FixTickerModel {
    public priceTicker: string;
    public priceMetadata: string;
}