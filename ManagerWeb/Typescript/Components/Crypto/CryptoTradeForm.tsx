import * as React from 'react'
import { Controller, useForm } from "react-hook-form";
import { Button, FormControl, InputLabel, MenuItem, Select } from "@mui/material";
import { TextField } from "@mui/material";
import { IBaseModel } from '../BaseList';
import CryptoTickerSelectModel from './CryptoTickerSelectModel';
import CurrencyTickerSelectModel from './CurrencyTickerSelectModel';

class CryptoTradeViewModel implements IBaseModel {
    id: number;
    tradeTimeStamp: string;
    cryptoTickerId: number;
    cryptoTicker: string;
    // cryptoTickers: CryptoTickerSelectModel[];
    tradeSize: number;
    tradeValue: number;
    currencySymbolId: number;
    currencySymbol: string;
    // currencies: CurrencyTickerSelectModel[];
    // onSave: (data: CryptoTradeViewModel) => void;
}

class CryptoTradeFromProps {
    viewModel: CryptoTradeViewModel;
    currencies: CurrencyTickerSelectModel[];
    cryptoTickers: CryptoTickerSelectModel[];
    onSave: (data: CryptoTradeViewModel) => void;
}

const CryptoTradeForm = (props: CryptoTradeFromProps) => {
    const viewModel = props.viewModel;
    const { handleSubmit, control } = useForm<CryptoTradeViewModel>({ defaultValues: { ...viewModel } });

    const onSubmit = (data: CryptoTradeViewModel) => {
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div className="col-span-2 w-1/3">
                    <Controller render={({ field }) => <TextField label="Datum tradu" size='small' type="date" value={field.value} {...field} className="place-self-end w-full" InputLabelProps={{ shrink: true }} />}
                        name="tradeTimeStamp" defaultValue={viewModel.tradeTimeStamp} control={control} />
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) =>
                        <FormControl className="w-full">
                            <InputLabel id="demo-simple-select-label">Crypto ticker</InputLabel>
                            <Select
                                {...field}
                                labelId="demo-simple-select-label"
                                id="type"
                                size='small'
                                value={field.value}
                            >
                                {props.cryptoTickers?.map(p => {
                                    return <MenuItem key={p.id} value={p.id}>
                                        <span>{p.ticker}</span>
                                    </MenuItem>
                                })}
                            </Select>
                        </FormControl>
                    } name="cryptoTickerId" control={control}></Controller>
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) => <TextField label="Trade size" size='small' type="text" {...field} className="place-self-end w-full" />}
                        name="tradeSize" control={control} />
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) => <TextField label="Trade value" size='small' type="text" {...field} className="place-self-end w-full" />}
                        name="tradeValue" control={control} />
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) =>
                        <FormControl className="w-full">
                            <InputLabel id="demo-simple-select-label">Zdrojová měna tradu</InputLabel>
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

            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Uložit</Button>
        </form>
    );
};

export { CryptoTradeForm, CryptoTradeViewModel }