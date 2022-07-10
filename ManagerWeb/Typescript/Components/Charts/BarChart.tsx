
import React from 'react'
import { Bar, BarSvgProps } from '@nivo/bar'

export class BarChartProps {
    dataSets: any[];
    chartProps?: BarSvgProps<any>
}

function BarChart({ dataSets, chartProps }: BarChartProps) {
    return (
        <Bar
            {...chartProps}
            data={dataSets}
        />
    )
}

export { BarChart };