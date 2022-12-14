import React from "react";

const ComponentPanel = (props: { children: JSX.Element, classStyle?: string }) => {
    return (
        <div className={(props.classStyle ?? "") + " p-4 rounded-xl bg-prussianBlue m-5 boxShadow"}>
            <div className="flex flex-col h-full">
                {props.children}
            </div>
        </div>
    );
}

export { ComponentPanel }