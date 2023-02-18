import { Button, FormControl, InputLabel, MenuItem, Select, TextField } from "@material-ui/core";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import { CurrencySymbol, StockTickerModel } from "../../ApiClient/Main/models";
import { StockViewModel } from "../../Model/StockViewModel";
import { createMuiTheme } from "@material-ui/core/styles";

const theme = createMuiTheme({
    palette: {
        type: 'dark',
        primary: {
            main: "#e03d15ff",
        }
    }
});

class StockTradeFormProps {
    stockTradeViewModel: StockViewModel;
    stockTickers: StockTickerModel[];
    currencies: CurrencySymbol[];
    onSave: (data: StockViewModel) => void;
}

const StockTradeForm = (props: StockTradeFormProps) => {
    const { handleSubmit, control } = useForm<StockViewModel>({ defaultValues: { ...props.stockTradeViewModel } });

    const onSubmit = (data: StockViewModel) => {
        props.onSave(data);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div className="col-span-2 w-1/3">
                    <Controller render={({ field }) => <TextField label="Datum tradu" type="date" value={field.value} {...field} className="place-self-end w-full" InputLabelProps={{ shrink: true }} />}
                        name="tradeTimeStamp" defaultValue={props.stockTradeViewModel?.tradeTimeStamp} control={control} />
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) =>
                        <FormControl className="w-full">
                            <InputLabel id="demo-simple-select-label">Stock ticker</InputLabel>
                            <Select
                                {...field}
                                labelId="demo-simple-select-label"
                                id="type"
                                value={field.value}
                            >
                                {props.stockTickers?.map(p => {
                                    return <MenuItem key={p.id} value={p.id}>
                                        <span>{p.ticker}</span>
                                    </MenuItem>
                                })}
                            </Select>
                        </FormControl>
                    } name="stockTickerId" control={control}></Controller>
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) => <TextField label="Velikost tradu" type="text" {...field} className="place-self-end w-full" />}
                        name="tradeSize" control={control} />
                </div>
                <div className="w-2/3">
                    <Controller render={({ field }) => <TextField label="Hodnota tradu" type="text" {...field} className="place-self-end w-full" />}
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
                                value={field.value}
                            >
                                {props.currencies?.map(p => {
                                    return <MenuItem key={p.id} value={p.id}>
                                        <span>{p.symbol}</span>
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

export { StockTradeForm }