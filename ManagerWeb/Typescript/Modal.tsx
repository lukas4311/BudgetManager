import * as React from 'react';

export interface IModalProps {
    handleClose(): void; show: boolean; children: React.ReactNode;
}

export let Modal = function (props: IModalProps) {
    const showHideClassName = props.show ? "modal block" : "modal hidden";

    return (
        <div className={showHideClassName}>
            <div className="modal-main text-black flex flex-col w-1/3 bg-battleshipGrey">
                <div className="ml-auto mr-4">
                    <button onClick={props.handleClose}>X</button>
                </div>
                <div>
                    {props.children}
                </div>
            </div>
        </div>
    );
}