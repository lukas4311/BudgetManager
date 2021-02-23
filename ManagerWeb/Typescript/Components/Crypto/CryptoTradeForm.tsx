import * as React from 'react'
import { useForm, Controller } from "react-hook-form";
import { Button } from "@material-ui/core";


interface ICryptoTradeFormProps {
    firstName: string;
    lastName: string;
    iceCreamType: string;
}

const CryptoTradeForm = () => {
    const { control, handleSubmit } = useForm<ICryptoTradeFormProps>();

    const onSubmit = (data: ICryptoTradeFormProps) => {
        console.log(data)
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <h1>Detail tradu</h1>
            <Button type="submit" value="Uložit" />
        </form>
    );
};