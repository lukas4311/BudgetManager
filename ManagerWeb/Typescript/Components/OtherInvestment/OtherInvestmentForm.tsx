import { Button, FormControl, InputLabel, MenuItem, Select, TextField } from "@mui/material";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import OtherInvestmentViewModel from "../../Model/OtherInvestmentViewModel";
import CurrencyTickerSelectModel from "../Crypto/CurrencyTickerSelectModel";

class OtherInvestmentFormProps {
    viewModel: OtherInvestmentViewModel;
    currencies: CurrencyTickerSelectModel[];
    onSave: (data: OtherInvestmentViewModel) => void;
}

const OtherInvestmentForm = (props: OtherInvestmentFormProps) => {
    const viewModel = props.viewModel;
    const { handleSubmit, control } = useForm<OtherInvestmentViewModel>({ defaultValues: { ...viewModel } });

    const onSubmit = (data: OtherInvestmentViewModel) => {
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div className="col-span-2 w-1/3">
                    <Controller render={({ field }) => <TextField label="Invested on" size='small' type="date" value={field.value} {...field} className="place-self-end w-full" InputLabelProps={{ shrink: true }} />}
                        name="created" defaultValue={viewModel.created} control={control} />
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) => <TextField label="Code" size='small' type="text" {...field} className="place-self-end w-full" />}
                        name="code" control={control} />
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) => <TextField label="Investment name" size='small' type="text" {...field} className="place-self-end w-full" />}
                        name="name" control={control} />
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) => <TextField label="Opening balance" size='small' type="text" {...field} className="place-self-end w-full" />}
                        name="openingBalance" control={control} />
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) =>
                        <FormControl className="w-full">
                            <InputLabel id="demo-simple-select-label">Investment currency</InputLabel>
                            <Select
                                {...field}
                                labelId="demo-simple-select-label"
                                id="type"
                                size='small'
                                value={field.value}
                            >
                                {props.currencies?.map(p => {
                                    return <MenuItem key={p.id} value={p.id}>
                                        <span>{p.ticker}</span>
                                    </MenuItem>
                                })}
                            </Select>
                        </FormControl>
                    } name="currencySymbolId" control={control}></Controller>
                </div>
            </div>

            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Save</Button>
        </form>
    );
};

export { OtherInvestmentForm }