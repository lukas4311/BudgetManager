import { Select, MenuItem, Button } from "@mui/material";
import PublishIcon from '@mui/icons-material/Publish';
import React from "react";

export class BrokerUploadProps {
    onUploadBrokerReport: (e: React.ChangeEvent<HTMLInputElement>) => Promise<void>;
    stockBrokerParsers: Map<number, string>;
    onBrokerSelect: (e: any) => void;
    selectedBroker: number;
}

export const BrokerUpload = (props: BrokerUploadProps) => {
    return (
        <div className="flex flex-col">
            <Select
                labelId="demo-simple-select-label"
                id="demo-simple-select"
                size="small"
                value={props.selectedBroker}
                onChange={e => props.onBrokerSelect(e.target.value)}
                className="w-full lg:w-2/3 mx-auto">
                {Array.from(props.stockBrokerParsers.entries()).map(([key, value]) => (
                    <MenuItem key={key} value={key}>{value}</MenuItem>
                ))}
            </Select >
            <Button
                component="label"
                variant="outlined"
                color="primary"
                className="block ml-auto bg-vermilion text-white mb-3 mt-4 w-full lg:w-2/3 mx-auto">
                <div className="flex flex-row justify-center">
                    <PublishIcon />
                    <span className="ml-4">Upload crypto report</span>
                </div>
                <input type="file" accept=".csv" hidden onChange={props.onUploadBrokerReport} />
            </Button>
        </div>
    );
}