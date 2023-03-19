import { Button, TextField } from "@material-ui/core";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import { OtherInvestmentBalaceHistoryViewModel } from "./OtherInvestmentDetail";

class OtherInvestmentBalanceFormProps {
    viewModel: OtherInvestmentBalaceHistoryViewModel;
    onSave: (otherInvestmentData: OtherInvestmentBalaceHistoryViewModel) => void;
}

const OtherInvestmentBalanceForm = (props: OtherInvestmentBalanceFormProps) => {
    const viewModel = props.viewModel;
    const { handleSubmit, control } = useForm<OtherInvestmentBalaceHistoryViewModel>({ defaultValues: { ...viewModel } });

    const onSubmit = (data: OtherInvestmentBalaceHistoryViewModel) => {
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div className="w-1/2">
                    <Controller render={({ field }) => <TextField label="Balance record date" type="date" value={field.value} {...field} className="place-self-end w-full" InputLabelProps={{ shrink: true }} />}
                        name="date" defaultValue={viewModel.date} control={control} />
                </div>
                <div className="w-1/2">
                    <Controller render={({ field }) => <TextField label="Balance" type="text" {...field} className="place-self-end w-full" />}
                        name="balance" control={control} />
                </div>
            </div>

            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Save</Button>
        </form>
    );
};

export { OtherInvestmentBalanceForm }