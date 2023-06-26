
import React from 'react'
import { Bar, BarSvgProps, ResponsiveBar } from '@nivo/bar'

export class BarData {
    key: string;
    value: number;
}

export class BarChartProps {
    dataSets: BarData[];
    chartProps?: BarSvgProps<any>
}

function BarChart({ dataSets, chartProps }: any) {
    return (
        <ResponsiveBar
            {...chartProps}
            data={dataSets}
        // axisBottom={{renderTick: <p>AAA</p>}}
        />
    )
}

export { BarChart };