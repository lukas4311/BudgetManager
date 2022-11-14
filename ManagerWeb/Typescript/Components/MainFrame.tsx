import React from "react";

const MainFrame = (props: { children: JSX.Element, classStyle?: string, header: string }) => {
    return (
        <>
            <h2 className={(props.classStyle ?? "") + "text-5xl p-4 text-center"}>{props.header}</h2>
            <div className="text-center mt-6 p-4 bg-prussianBlue rounded-lg">
                {props.children}
            </div>
        </>
    );
}

export { MainFrame }