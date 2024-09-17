import React from "react";

const ComponentPanel = (props: { children: JSX.Element, classStyle?: string }) => {
    return (
        <div className={(props.classStyle ?? "") + " rounded-xl bg-prussianBlue boxShadow"}>
            {props.children}
        </div>
    );
}

export { ComponentPanel }