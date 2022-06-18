import _ from "lodash";
import React from "react";

class Investments {
    name: string;
    investmentProgress: number;
}

const Ranking = (props: Investments[]) => {

    const investments = _.orderBy(props, i => i.investmentProgress, "desc");

    return (
        <div className="flex flex-row h-full justify-around items-end text-xl">
            <div className="w-1/5 bg-vermilion h-4/6 flex flex-col justify-around">
                <p className="text-xl">2.</p>
                <p className="text-2xl">{investments[1]?.name}</p>                
                <p className="text-3xl">{(investments[1]?.investmentProgress ?? " -")}%</p>
            </div>
            <div className="w-1/5 bg-vermilion h-full flex flex-col justify-around">
                <p className="text-xl">1.</p>
                <p className="text-2xl">{investments[0]?.name}</p>                
                <p className="text-3xl">{(investments[0]?.investmentProgress ?? " -")}%</p>
            </div>
            <div className="w-1/5 bg-vermilion h-2/6 flex flex-col justify-around">
                <p className="text-xl">3.</p>
                <p className="text-2xl">{investments[3]?.name}</p>                
                <p className="text-3xl">{(investments[3]?.investmentProgress ?? " -")}%</p>
            </div>
        </div>
    );
}

export { Ranking };