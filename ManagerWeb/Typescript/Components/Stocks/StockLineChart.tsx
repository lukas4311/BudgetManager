import React from "react";
import { LineChartData } from "../../Model/LineChartData";
import { LineChartDataSets } from "../../Model/LineChartDataSets";
import { TickersWithPriceHistory } from "../../Services/StockService";
import { LineChart } from "../Charts/LineChart";
import { LineChartSettingManager } from "../Charts/LineChartSettingManager";
import moment from "moment";
import _ from "lodash";

class StockLineChartProp {
    ticker: string;
    tickersPrice: TickersWithPriceHistory[];
}

const StockLineChart = (props: StockLineChartProp) => {
    let lineChartData: LineChartDataSets[] = [{ id: 'Price', data: [] }];
    let tradeHistory = _.first(props.tickersPrice.filter(f => f.ticker == props.ticker));

    if (tradeHistory != undefined && tradeHistory.price.length > 5) {
        let prices = tradeHistory.price;
        const sortedArray = _.orderBy(prices, [(obj) => new Date(obj.time)], ['asc']);
        let priceData: LineChartData[] = sortedArray.map(b => ({ x: moment(b.time).format('YYYY-MM-DD'), y: b.price }));
        lineChartData = [{ id: 'Price', data: priceData }];
    }

    return (
        <div className="h-8">
            <LineChart dataSets={lineChartData} chartProps={LineChartSettingManager.getStockChartSetting()}></LineChart>
        </div>
    );
}

export { StockLineChartProp, StockLineChart }