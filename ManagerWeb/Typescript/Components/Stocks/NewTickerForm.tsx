import { Button, TextField } from "@mui/material";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import NewTickerModel from "../../Model/NewTickerModel";

class NewTickerFormProps {
    onSave: (ticker: string) => void;
}

const NewTickerForm = (props: NewTickerFormProps) => {
    const { handleSubmit, control } = useForm<NewTickerModel>({ defaultValues: { ticker: '' } });

    const onSubmit = (data: NewTickerModel) => {
        props.onSave(data.ticker);
    };

    return (
        (<form onSubmit={handleSubmit(onSubmit)}>
            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Send request</Button>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div className="w-2/3">
                    <Controller render={({ field }) => <TextField label="Ticker" size='small' type="text" {...field} className="place-self-end w-full" />}
                        name="ticker" control={control} />
                </div>
            </div>
        </form>)
    );
};

export { NewTickerForm }