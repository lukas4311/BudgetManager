import * as React from 'react';

export interface IModalProps {
    handleClose(): void; show: boolean; children: React.ReactNode;
}

export let Modal = function (props: IModalProps) {
    const showHideClassName = props.show ? "modal display-block" : "modal display-none";

    return (
        <div className={showHideClassName}>
            <div className="modal-main text-black flex flex-col">
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