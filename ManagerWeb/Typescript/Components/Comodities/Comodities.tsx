import React from "react";
import Gold from "./Gold";

class GoldIngot {
    public company: string;
    public weight: number;
}

export default class Comodities extends React.Component<{}, {}>{
    private goldIngots: GoldIngot[];

    constructor(props: {}) {
        super(props);

    }

    public render() {
        return (
            <div className="">
                <p className="text-3xl text-center mt-6">Comodities overview</p>
                <div className="flex">
                    <div className="w-4/12 p-4 overflow-y-auto"><Gold /></div>
                    <div className="w-4/12 p-4 overflow-y-auto">Silver component</div>
                    <div className="w-4/12 p-4 overflow-y-auto">Others</div>
                </div>
            </div>
        );
    }
}