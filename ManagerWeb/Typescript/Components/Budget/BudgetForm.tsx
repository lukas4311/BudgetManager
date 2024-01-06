import * as React from 'react'
import { Controller, useForm } from "react-hook-form";
import { Button, TextField } from '@mui/material';

class BudgetFormModel {
    id: number;
    name: string;
    amount: number;
    from: string;
    to: string;
    onSave: (model: BudgetFormModel) => void;
}

const BudgetForm = (props: BudgetFormModel) => {
    const { register, handleSubmit, control } = useForm<BudgetFormModel>({ defaultValues: { ...props } });

    const onSubmit = (data: BudgetFormModel) => {
        data.id = props.id;
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div className="w-3/5">
                    <Controller
                        render={({ field }) => <TextField label="Název" type="text" {...field} className="w-full" />}
                        name="name"
                        control={control}
                    />
                </div>
                <div className="w-3/5">
                    <Controller
                        render={({ field }) => <TextField label="Velikost" type="text" {...field} className="w-full" />}
                        name="amount"
                        control={control}
                    />
                </div>
                <div className="w-3/5">
                    <Controller
                        render={({ field }) => <TextField label="Od" type="date" value={field.value} {...field} className="w-full" InputLabelProps={{ shrink: true }} />}
                        name="from"
                        defaultValue={props.from}
                        control={control}
                    />
                </div>
                <div className="w-3/5">
                    <Controller
                        render={({ field }) => <TextField label="Do" type="date" value={field.value} {...field} className="w-full" InputLabelProps={{ shrink: true }} />}
                        name="to"
                        defaultValue={props.to}
                        control={control}
                    />
                </div>
            </div>

            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Uložit</Button>
        </form>
    );
};

export { BudgetFormModel, BudgetForm as BudgetForm2 }