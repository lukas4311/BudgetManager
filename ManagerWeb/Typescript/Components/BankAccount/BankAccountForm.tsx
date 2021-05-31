import * as React from 'react'
import { useForm } from "react-hook-form";
import { Button } from "@material-ui/core";
import { TextField } from "@material-ui/core";
import { IBaseModel } from '../BaseList';

class BankAccountFromViewModel implements IBaseModel {
    id: number;
    code: string;
    openingBalance: number;
    onSave: (data: BankAccountFromViewModel) => void;
}

const BankAccountForm = (props: BankAccountFromViewModel) => {
    const { register, handleSubmit } = useForm<BankAccountFromViewModel>({ defaultValues: props });

    const onSubmit = (data: BankAccountFromViewModel) => {
        data.id = props.id;
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div>
                    <TextField label="Název účtu" name="code" inputRef={register} />
                </div>
                <div>
                    <TextField label="Počáteční stav" type="text" name="openingBalance" inputRef={register} className="w-full" />
                </div>
            </div>

            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Uložit</Button>
        </form>
    );
};

export { BankAccountForm, BankAccountFromViewModel }