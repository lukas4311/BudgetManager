import React from "react";

const ComponentPanel = (props: { children: JSX.Element, classStyle?: string }) => {
    return (
        <div className={(props.classStyle ?? "") + " rounded-xl bg-prussianBlue boxShadow"}>
            <div className="flex flex-col h-full w-full">
                {props.children}
            </div>
        </div>
    );
}

export { ComponentPanel }