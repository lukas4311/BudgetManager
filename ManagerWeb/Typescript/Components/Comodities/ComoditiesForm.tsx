import * as React from 'react'
import { Controller, useForm } from "react-hook-form";
import { Button, FormControl, InputLabel, MenuItem, Select } from "@material-ui/core";
import { TextField } from "@material-ui/core";
import { IBaseModel } from '../BaseList';
import CurrencyTickerSelectModel from '../Crypto/CurrencyTickerSelectModel';

class ComoditiesFormProps {
    viewModel: ComoditiesFormViewModel;
    currencies: CurrencyTickerSelectModel[];
    onSave: (data: ComoditiesFormViewModel) => void;
    onDelete: (id: number) => Promise<void>;
}

class ComoditiesFormViewModel implements IBaseModel {
    id: number;
    buyTimeStamp: string;
    comodityTypeName: string
    comodityAmount: number;
    comodityUnit: string;
    price: number;
    company: string;
    currencySymbolId: number;
    currencySymbol: string;
    // currencies: CurrencyTickerSelectModel[];
    // onSave: (data: ComoditiesFormViewModel) => void;
    // onDelete: (id: number) => Promise<void>;
}

const ComoditiesForm = (props: ComoditiesFormProps) => {
    const viewModel = props.viewModel;
    const { handleSubmit, control } = useForm<ComoditiesFormViewModel>({ defaultValues: { ...viewModel } });

    const onSubmit = (data: ComoditiesFormViewModel) => {
        props.onSave(data);
    };

    return (
        <div>
            <form onSubmit={handleSubmit(onSubmit)}>
                <h1 className='text-center text-3xl mb-5'>{viewModel.comodityTypeName}</h1>
                <div className="grid grid-cols-2 gap-4 mb-6 place-items-center gap-y-8">
                    <div className="col-span-2 w-2/3 flex flex-row items-center">
                        <Controller render={({ field }) => <TextField label="Company" type="text" {...field} className="place-self-end w-full" />}
                            name="company" control={control} />
                    </div>
                    <div className="w-2/3 flex justify-start">
                        <Controller render={({ field }) =>
                            <TextField label="Datum nákupu" type="date" value={field.value} {...field} className="place-self-end w-full" InputLabelProps={{ shrink: true }} />}
                            name="buyTimeStamp" defaultValue={viewModel.buyTimeStamp} control={control} />
                    </div>
                    <div className="w-2/3 flex flex-row items-center">
                        <Controller render={({ field }) => <TextField label="Amount" type="text" {...field} className="place-self-end w-full" />}
                            name="comodityAmount" control={control} />
                        <p className='ml-3'>{viewModel.comodityUnit}</p>
                    </div>
                    <div className="w-2/3">
                        <Controller render={({ field }) => <TextField label="Price" type="text" {...field} className="place-self-end w-full" />}
                            name="price" control={control} />
                    </div>
                    <div className="w-2/3">
                        <Controller render={({ field }) =>
                            <FormControl className="w-full">
                                <InputLabel id="demo-simple-select-label">Zdrojová měna tradu</InputLabel>
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

                <Button type="submit" variant="contained" className="block m-auto w-1/3 bg-vermilion text-white">Save</Button>
            </form>
            <Button className='bg-red-600' onClick={() => props.onDelete(viewModel.id)}>Delete</Button>
        </div>
    );
};

export { ComoditiesForm, ComoditiesFormViewModel }