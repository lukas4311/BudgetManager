import Button from "@material-ui/core/Button";
import Dialog from "@material-ui/core/Dialog";
import DialogContent from "@material-ui/core/DialogContent";
import DialogTitle from "@material-ui/core/DialogTitle/DialogTitle";
import React from "react";

enum ConfirmationResult {
    Ok,
    Cancel
}

class ConfirmationFormProps {
    onClose: () => void;
    onConfirm: (confirmationResult: ConfirmationResult) => void;
    isOpen: boolean;
}

const ConfirmationForm = (props: ConfirmationFormProps) => {
    return (
        <Dialog open={props.isOpen} onClose={props.onClose} aria-labelledby="ConfirmationDetail"
            maxWidth="sm" fullWidth={true}>
            <DialogTitle id="form-dialog-title">Confirmation dialog</DialogTitle>
            <DialogContent>
                <div>
                    <div className="flex flex-row w-3/5 m-auto">
                        <Button className='bg-vermilion' onClick={() => props.onConfirm(ConfirmationResult.Ok)}>Ok</Button>
                        <Button className='bg-gray-700 ml-auto' onClick={() => props.onConfirm(ConfirmationResult.Cancel)}>Cancel</Button>
                    </div>
                </div>
            </DialogContent>
        </Dialog>


    );
}

export { ConfirmationForm, ConfirmationResult }