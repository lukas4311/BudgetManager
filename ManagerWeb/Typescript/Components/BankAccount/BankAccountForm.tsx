import * as React from 'react'
import { Controller, useForm } from "react-hook-form";
import { Button, Input } from "@mui/material";
import { TextField } from "@mui/material";
import { IBaseModel } from '../BaseList';

class BankAccountFromViewModel implements IBaseModel {
    id: number;
    code: string;
    openingBalance: number;
    onSave: (data: BankAccountFromViewModel) => void;
}

const BankAccountForm = (props: BankAccountFromViewModel) => {
    const { handleSubmit, control } = useForm<BankAccountFromViewModel>({ defaultValues: { ...props } });

    const onSubmit = (data: BankAccountFromViewModel) => {
        data.id = props.id;
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div>
                    <Controller
                        render={({ field }) => <TextField label="Account name" {...field} size='small' className="materialUIInput w-full" />}
                        name="code"
                        control={control}
                    />
                </div>
                <div>
                    <Controller
                        render={({ field }) => <TextField label="Initial balance" {...field} size='small' className="materialUIInput w-full" />}
                        name="openingBalance"
                        control={control}
                    />
                </div>
            </div>

            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Save</Button>
        </form>
    );
};

export { BankAccountForm, BankAccountFromViewModel }