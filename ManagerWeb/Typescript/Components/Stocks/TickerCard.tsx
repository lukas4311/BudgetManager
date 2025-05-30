import React, { useState } from "react";
import { StockTickerModel } from "../../ApiClient/Main";
import { StockGroupModel, TickersWithPriceHistory } from "../../Services/StockService";
import { StockLineChart } from "./StockLineChart";
import WarningAmberOutlinedIcon from '@mui/icons-material/WarningAmberOutlined';
import ArrowDropUpIcon from '@mui/icons-material/ArrowDropUp';
import ArrowDropDownIcon from '@mui/icons-material/ArrowDropDown';
import _ from "lodash";

class TickerCardProp {
    ticker: StockGroupModel;
    tickers: StockTickerModel[];
    tickersPrice: TickersWithPriceHistory[];
    onWarningClick: (e: React.MouseEvent<SVGSVGElement, MouseEvent>, ticker: StockGroupModel) => void;
    onTickerCardClick: (companyTicker: string) => Promise<void>;
}

const TickerCard = (props: TickerCardProp) => {
    const calculareProfit = (actualPrice: number, buyPrice: number) => {
        if (buyPrice <= 0 || actualPrice <= 0)
            return 0;

        let profitOrLoss = ((actualPrice - buyPrice) / buyPrice) * 100;

        return profitOrLoss;
    }

    let hasPrice = false;
    let hasMetadata = false;
    const metadata = _.first(props.tickers.filter(t => t.id == props.ticker.tickerId))?.metadata;
    let profitOrLoss = 0
    let totalStockPrice = 0;

    if(metadata != undefined){
        const metadataObject = JSON.parse(metadata);
        hasMetadata = metadata != undefined;
        const tickerPrice = _.first(props.tickersPrice.filter(t => t.ticker == props.ticker.tickerName || t.ticker == metadataObject?.price_ticker));
        hasPrice = tickerPrice?.price.length != 0;

        if(hasPrice){
            totalStockPrice = props.ticker.size * (_.last(tickerPrice.price)?.price ?? 0);
            profitOrLoss = calculareProfit(totalStockPrice, props.ticker.stockSpentPrice);
        }
    }

    return (
        <>
            <div key={props.ticker.tickerId} className="w-3/12 bg-battleshipGrey border-2 border-vermilion p-4 mx-2 mb-6 rounded-xl relative" onClick={_ => props.onTickerCardClick(props.ticker.tickerName)}>
                {hasMetadata && hasPrice ?
                    <></> :
                    <WarningAmberOutlinedIcon className="fill-yellow-500 h-6 w-6 absolute z-40 bottom-1 right-2" onClick={e => props.onWarningClick(e, props.ticker)}></WarningAmberOutlinedIcon>
                }
                <div className="grid grid-cols-3 mb-2">
                    <div className="flex flex-row col-span-2">
                        {(profitOrLoss >= 0 ? <ArrowDropUpIcon className="fill-green-700 h-10 w-10" /> : <ArrowDropDownIcon className="fill-red-700 h-10 w-10" />)}
                        <div className="flex flex-col text-left">
                            <p className={"text-xl font-bold text-left mt-1"}>{props.ticker.tickerName}</p>
                            {totalStockPrice != 0 ? (<p className="text-2xl font-extrabold">{profitOrLoss.toFixed(2)} %</p>) : <></>}
                        </div>
                    </div>
                    <div className="text-right">
                        <p className="text-lg">{props.ticker.size.toFixed(3)}</p>
                        <p className="text-lg">{totalStockPrice.toFixed(2)} $</p>
                    </div>
                </div>
                <StockLineChart ticker={props.ticker.tickerName} tickersPrice={props.tickersPrice} />
            </div>
        </>
    );
}

export { TickerCardProp, TickerCard }