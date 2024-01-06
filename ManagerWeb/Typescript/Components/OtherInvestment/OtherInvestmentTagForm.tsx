import { Button, FormControl, InputLabel, MenuItem, Select, TextField } from "@mui/material";
import React from "react";
import { Controller, useForm } from "react-hook-form";
import { TagFormViewModel } from "../../Model/TagFormViewModel";

class OtherInvestmentTagFormProps {
    viewModel: TagFormViewModel;
    onSave: (tagId: number) => void;
}

const OtherInvestmentTagForm = (props: OtherInvestmentTagFormProps) => {
    const viewModel = props.viewModel;
    const { handleSubmit, control } = useForm<TagFormViewModel>({ defaultValues: { ...viewModel } });

    const onSubmit = (data: TagFormViewModel) => {
        props.onSave(data.tagId);
    };

    return (
        <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-2 gap-4 mb-6 place-items-center">
                <div className="w-2/3">
                    <Controller render={({ field }) =>
                        <FormControl className="w-full">
                            <InputLabel id="demo-simple-select-label">Connect with tag</InputLabel>
                            <Select
                                {...field}
                                labelId="demo-simple-select-label"
                                id="type"
                                size='small'
                                value={field.value}
                            >
                                {viewModel.tags?.map(p => {
                                    return <MenuItem key={p.id} value={p.id}>
                                        <span>{p.code}</span>
                                    </MenuItem>
                                })}
                            </Select>
                        </FormControl>
                    } name="tagId" control={control}></Controller>
                </div>
            </div>
            <Button type="submit" variant="contained" color="primary" className="block ml-auto">Save</Button>
        </form>
    );
};

export { OtherInvestmentTagForm }