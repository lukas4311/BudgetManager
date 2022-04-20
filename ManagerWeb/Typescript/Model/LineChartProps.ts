import { LineSvgProps } from "@nivo/line";
import { LineChartDataSets } from "./LineChartDataSets";

export class LineChartProps {
    dataSets: LineChartDataSets[];
    chartProps?: LineSvgProps
}