import React from "react";

const ComponentPanel = (props: { children: JSX.Element, classStyle?: string }) => {
    return (
        <div className={(props.classStyle ?? "") + " rounded-xl bg-prussianBlue m-5 p-4 boxShadow"}>
            <div className="flex flex-col h-full">
                {props.children}
            </div>
        </div>
    );
}

export { ComponentPanel }