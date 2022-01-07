import * as React from 'react'
import { Controller, useForm } from "react-hook-form";
import { Button } from "@material-ui/core";
import { TextField } from "@material-ui/core";
import { IBaseModel } from '../BaseList';

class ComoditiesFormViewModel implements IBaseModel {
    id: number;
    tradeTimeStamp: string;
    onSave: (data: ComoditiesFormViewModel) => void;
}

const ComoditiesForm = (props: ComoditiesFormViewModel) => {
    const { handleSubmit, control } = useForm<ComoditiesFormViewModel>({ defaultValues: { ...props } });

    const onSubmit = (data: ComoditiesFormViewModel) => {
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div className="col-span-2 w-1/3">
                    <Controller render={({ field }) => <TextField label="Datum tradu" type="date" value={field.value} {...field} className="place-self-end w-full" InputLabelProps={{ shrink: true }} />}
                        name="tradeTimeStamp" defaultValue={props.tradeTimeStamp} control={control} />
                </div>
                
            </div>

            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Uložit</Button>
        </form>
    );
};

export { ComoditiesForm, ComoditiesFormViewModel }