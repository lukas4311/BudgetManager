import { Button, Dialog, DialogActions, DialogTitle } from '@material-ui/core';
import * as React from 'react'
import { IconsData } from '../Enums/IconsEnum';

interface IBaseModel {
    id?: number;
}
interface IBaseListProps<T extends IBaseModel> {
    data: T[];
    template: (model: T) => JSX.Element;
    title?: string;
    header?: JSX.Element;
    addItemHandler?: () => void;
    itemClickHandler?: (id: number) => void;
    deleteItemHandler?: (id: number) => void;
    dataAreaClass?: string;
}

const BaseList = <T extends IBaseModel,>(props: React.PropsWithChildren<IBaseListProps<T>>) => {
    const [open, setOpen] = React.useState<boolean>(false);
    const [id, setId] = React.useState<number>(undefined);

    const handleClickOpen = (id: number) => {
        setOpen(true);
        setId(id);
    };

    const handleClose = () => {
        setOpen(false);
    };

    const onDeleteClick = (e: React.MouseEvent<HTMLDivElement, MouseEvent>, id: number) => {
        e.stopPropagation();
        handleClickOpen(id);
    }

    const renderBinIcon = (): JSX.Element => {
        let iconsData: IconsData = new IconsData();
        return iconsData.bin;
    }

    return (
        <React.Fragment>

            <div className="flex w-ful flex-col">
                <div className="py-4 flex w-full">
                    {props.title != undefined ? (<h1 className="ml-6 text-xl">{props.title}</h1>) : <></>}
                    {props.addItemHandler != undefined ? (
                        <span className="inline-block ml-auto mr-5" onClick={props.addItemHandler}>
                            <svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 0 24 24" width="24" className="fill-current text-white hover:text-vermilion transition ease-out duration-700 cursor-pointer">
                                <path d="M0 0h24v24H0z" fill="none" />
                                <path d="M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-2 10h-4v4h-2v-4H7v-2h4V7h2v4h4v2z" />
                            </svg>
                        </span>
                    ) : <></>}
                </div>
                <div className="text-center flex pr-5">
                    <div className="w-9/12 flex flex-row text-sm">
                        {props.header}
                    </div>
                </div>
                <div className={"pr-5 " + props.dataAreaClass}>
                    {props.data.map(d => (
                        <div key={d.id} className="paymentRecord bg-battleshipGrey rounded-r-full flex mt-1 hover:bg-vermilion cursor-pointer" onClick={(_) => props.itemClickHandler(d.id)}>
                            <div className="w-9/12 flex flex-row">
                                {props.template(d)}
                            </div>
                            <div className="w-3/12 flex items-center">
                                {
                                    props.deleteItemHandler != undefined ? (
                                        <div onClick={(e) => onDeleteClick(e, d.id)} className="w-6 m-auto">
                                            {renderBinIcon()}
                                        </div>
                                    ) : <></>
                                }
                            </div>
                        </div>
                    ))}
                </div>
            </div>
            <Dialog
                open={open} onClose={handleClose} aria-labelledby="alert-dialog-title" aria-describedby="alert-dialog-description">
                <DialogTitle id="alert-dialog-title">Opravdu si přejete smazat záznam?</DialogTitle>
                <DialogActions>
                    <Button onClick={handleClose} variant="contained" color="primary">
                        Ne
                    </Button>
                    <Button onClick={(_) => {
                        props.deleteItemHandler(id);
                        handleClose();
                    }}
                        variant="contained" color="primary" autoFocus>
                        Ano
                    </Button>
                </DialogActions>
            </Dialog>
        </React.Fragment >
    );
}

export { BaseList, IBaseListProps, IBaseModel }