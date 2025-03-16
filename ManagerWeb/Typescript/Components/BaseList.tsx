import { Button, Dialog, DialogActions, DialogTitle } from '@mui/material';
import * as React from 'react'
import { IconsData } from '../Enums/IconsEnum';

interface IBaseModel {
    id?: number;
}

export enum EListStyle {
    RowStyle,
    CardStyle
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
    useRowBorderColor?: boolean;
    hideIconRowPart?: boolean;
    narrowIcons?: boolean;
    style?: EListStyle;
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

    const showIcons = (): boolean => props.hideIconRowPart == undefined || props.hideIconRowPart == false;

    const getWidthRation = [(props.narrowIcons ? "w-11/12 " : "w-9/12 "), (props.narrowIcons ? "w-1/12 " : "w-3/12 ")];
    const style = props.style ?? EListStyle.RowStyle;

    return (
        <React.Fragment>
            <div className={`flex w-full flex-col ${(style != EListStyle.RowStyle ? "" : "bg-battleshipGrey")} rounded-t-md`}>
                <div className={(props.addItemHandler != undefined ? "pt-4" : "") + " flex w-full"}>
                    {props.title != undefined ? (<h1 className="ml-6 text-xl">{props.title}</h1>) : <></>}
                    {props.addItemHandler != undefined ? (
                        <span className="inline-block ml-auto mr-5 " onClick={props.addItemHandler}>
                            <svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 0 24 24" width="24" className="fill-current text-mainDarkBlue hover:text-vermilion transition ease-out duration-700 cursor-pointer">
                                <path d="M0 0h24v24H0z" fill="none" />
                                <path d="M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-2 10h-4v4h-2v-4H7v-2h4V7h2v4h4v2z" />
                            </svg>
                        </span>
                    ) : <></>}
                </div>
                {style == EListStyle.RowStyle ?
                    (<>
                        <div className="text-center flex pr-5">
                            <div className={(showIcons() ? getWidthRation[0] : "w-full ") + "flex flex-row text-sm"}>
                                {props.header}
                            </div>
                        </div>
                        <div className={"pr-5 " + props.dataAreaClass}>
                            {props.data.map(d => (
                                <div key={d.id} className="paymentRecord bg-mainDarkBlue rounded-r-full flex hover:bg-vermilion cursor-pointer" onClick={(_) => props.itemClickHandler(d.id)}>
                                    <div className={(showIcons() ? getWidthRation[0] : "w-full ") + "flex flex-row"}>
                                        {props.template(d)}
                                    </div>
                                    {showIcons() ? (
                                        <div className={`${getWidthRation[1]} flex items-center rounded-r-full ` + (props.useRowBorderColor ? "border border-vermilion" : "")}>
                                            {
                                                props.deleteItemHandler != undefined ? (
                                                    <div onClick={(e) => onDeleteClick(e, d.id)} className="w-6 m-auto">
                                                        {renderBinIcon()}
                                                    </div>
                                                ) : <></>
                                            }
                                        </div>
                                    ) : <></>}
                                </div>
                            ))}
                        </div>
                    </>)
                    : style == EListStyle.CardStyle ?
                        (<>
                            <div className={props.dataAreaClass ?? ""}>
                                {props.data.map(d => (
                                    <div key={d.id} className="bg-white rounded-lg mb-3 flex hover:bg-battleshipGrey transition-all duration-200 cursor-pointer">
                                        <div className="flex-grow" onClick={(_) => props.itemClickHandler(d.id)}                                        >
                                            {props.template(d)}
                                        </div>
                                        <div className="flex items-center justify-center px-1 group transition-all duration-200 hover:bg-red-700 hover:px-3 rounded-r-lg"
                                            onClick={(e) => onDeleteClick(e, d.id)}>
                                            <div className='w-6 m-auto'>
                                                {renderBinIcon()}
                                            </div>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        </>)
                        : <></>}
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