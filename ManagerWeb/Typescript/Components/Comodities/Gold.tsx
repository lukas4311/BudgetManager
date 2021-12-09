import moment from "moment";
import React from "react";
import { GoldIngot } from "./GoldIngot";

const Gold = (props: {}) => {

    const goldIngots: GoldIngot[] = [
        { id: 1, boughtDate: moment().toDate(), company: "Argor-Heraus", weight: 10 },
        { id: 2, boughtDate: moment().toDate(), company: "Argor-Heraus", weight: 5 },
        { id: 3, boughtDate: moment().toDate(), company: "Argor-Heraus", weight: 5 },
        { id: 4, boughtDate: moment().toDate(), company: "Argor-Heraus", weight: 5 },
        { id: 5, boughtDate: moment().toDate(), company: "Argor-Heraus", weight: 5 },
        { id: 6, boughtDate: moment().toDate(), company: "Argor-Heraus", weight: 5 },
        { id: 7, boughtDate: moment().toDate(), company: "Pamp", weight: 20 }
    ];

    return (
        <div id="goldCards">
            <h3 className="text-xl">Gold</h3>
            <div className="mt-3 flex flex-row flex-nowrap text-center">
                {goldIngots.map((g, i) => (
                    <div className={"relative p-1 bg-gold-brighter rounded-xl inline-block goldCard shadow-2xl z-0 overflow-hidden" + (i == 0 ? "" : " cardOverlap")}>
                        <div className="w-11/12 z-negative1 bg-gold rotateBox">
                        </div>
                        <div className={"px-2 py-6 rounded-xl bg-gold z-10"}>
                            <p className="font-medium goldText">{g.company}</p>
                            <p className="text-2xl font-bold mt-4 goldText">{g.weight}g</p>
                            <p className="mt-6 goldText">{g.boughtDate.toLocaleDateString()}</p>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default Gold;