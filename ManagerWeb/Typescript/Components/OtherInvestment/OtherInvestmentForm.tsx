import { Button, FormControl, InputLabel, MenuItem, Select, TextField } from "@material-ui/core";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import OtherInvestmentViewModel from "../../Model/OtherInvestmentViewModel";

const OtherInvestmentForm = (props: OtherInvestmentViewModel) => {
    const { handleSubmit, control } = useForm<OtherInvestmentViewModel>({ defaultValues: { ...props } });

    const onSubmit = (data: OtherInvestmentViewModel) => {
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div className="col-span-2 w-1/3">
                    <Controller render={({ field }) => <TextField label="Invested on" type="date" value={field.value} {...field} className="place-self-end w-full" InputLabelProps={{ shrink: true }} />}
                        name="created" defaultValue={props.created} control={control} />
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) => <TextField label="Code" type="text" {...field} className="place-self-end w-full" />}
                        name="code" control={control} />
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) => <TextField label="Investment name" type="text" {...field} className="place-self-end w-full" />}
                        name="name" control={control} />
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) => <TextField label="Opening balance" type="text" {...field} className="place-self-end w-full" />}
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