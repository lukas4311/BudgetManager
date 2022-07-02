import _ from "lodash";
import React from "react";

class Investments {
    name: string;
    investmentProgress: number;
}

class RankingProps {
    data: Investments[]
}

const Ranking = (props: RankingProps) => {
    const investments = _.orderBy(props.data, i => i.investmentProgress, "desc");
    const renderRankingColumn = (rankingNum: number) => {
        const height = (props.data.length - (rankingNum - 1)) / props.data.length;
        const heightPercent = height * 100 + "%";

        if (height > 0)
            return (
                <div className="w-1/5 bg-vermilion flex flex-col justify-around" style={{ height: heightPercent }}>
                    <p className="text-xl">{rankingNum}.</p>
                    <p className="text-2xl">{investments[rankingNum - 1]?.name}</p>
                    <p className="text-3xl">{(investments[rankingNum - 1]?.investmentProgress.toFixed(2) ?? " -")}%</p>
                </div>
            )

        return <></>;
    }

    return (
        <div className="flex flex-row h-full justify-around items-end text-xl">
            {renderRankingColumn(2)}
            {renderRankingColumn(1)}
            {renderRankingColumn(3)}
        </div>
    );
}

export { Ranking, Investments};