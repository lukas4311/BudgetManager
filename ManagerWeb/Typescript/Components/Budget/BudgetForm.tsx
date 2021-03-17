import * as React from 'react'
import { useForm } from "react-hook-form";
import { Button, TextField } from '@material-ui/core';

class BudgetFormModel {
    id: number;
    name: string;
    amount: number;
    to: string;
    from: string;
    onSave: (model: BudgetFormModel) => void;
}

const BudgetForm = (props: BudgetFormModel) => {
    const { register, handleSubmit } = useForm<BudgetFormModel>({ defaultValues: props });

    const onSubmit = (data: BudgetFormModel) => {
        data.id = props.id;
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div className="w-3/5">
                    <TextField label="Název" type="text" name="name" inputRef={register} className="w-full"/>
                </div>
                <div className="w-3/5">
                    <TextField label="Velikost" type="text" name="amount" inputRef={register} className="w-full"/>
                </div>
                <div className="w-3/5">
                    <TextField label="Od" type="date" name="from" inputRef={register} className="w-full"
                        InputLabelProps={{
                            shrink: true,
                        }}
                    />
                </div>
                <div className="w-3/5">
                    <TextField label="Do" type="date" name="to" inputRef={register} className="w-full"
                        InputLabelProps={{
                            shrink: true,
                        }}
                    />
                </div>
            </div>

            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Uložit</Button>
        </form>
    );
};

export { BudgetFormModel, BudgetForm as BudgetForm2 }