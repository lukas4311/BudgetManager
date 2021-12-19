import _ from "lodash";
import React from "react";
import { GoldListProps } from "./GoldListProps";

const Gold = (props: GoldListProps) => {
    const goldIngots = props.goldIngots ?? [];
    const totalWeight = _.sumBy(goldIngots, (g) => g.weight);
    const totalCosts = _.sumBy(goldIngots, (g) => g.costs);
    const goldUnit = props.goldIngots[0]?.unit;
    const currency = props.goldIngots[0]?.currency;

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
            <div>
                <h4 className="text-lg mt-6">Summary info</h4>
                <p className="mt-2">Total weight: {totalWeight} {goldUnit}</p>
                <p className="">Total costs: {totalCosts} {currency}</p>
            </div>
        </div>
    );
}

export default Gold;