import * as React from 'react'

interface IBaseModel {
    id: number;
}
interface IBaseListProps<T extends IBaseModel> {
    data: T[];
    template: (T) => JSX.Element;
    title: string;
    header?: JSX.Element;
    addItemHandler?: () => void;
    itemClickHandler?: (id: number) => void;
}

const BaseList = <T extends IBaseModel,>(props: React.PropsWithChildren<IBaseListProps<T>>) => {
    return (
        <div className="flex w-ful flex-col">
            <div className="py-4 flex w-full">
                <h1 className="ml-6 text-xl">{props.title}</h1>
                {props.addItemHandler != undefined ? (
                    <span className="inline-block ml-auto mr-5" onClick={props.addItemHandler}>
                        <svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 0 24 24" width="24" className="fill-current text-white hover:text-vermilion transition ease-out duration-700 cursor-pointer">
                            <path d="M0 0h24v24H0z" fill="none" />
                            <path d="M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-2 10h-4v4h-2v-4H7v-2h4V7h2v4h4v2z" />
                        </svg>
                    </span>
                ) : <></>}
            </div>
            <div className="text-center flex">
                {props.header}
            </div>
            <div>
                {props.data.map(d => (
                    <div key={d.id} className="paymentRecord bg-battleshipGrey rounded-r-full flex mr-6 mt-1 hover:bg-vermilion cursor-pointer" onClick={(_) => props.itemClickHandler(d.id)}>
                        {props.template(d)}
                        <button className="inline-block mx-4" onClick={() => alert("delete")}>X</button>
                    </div>
                ))}
            </div>
        </div>
    );
}

export { BaseList, IBaseListProps, IBaseModel }