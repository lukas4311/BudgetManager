
import React from 'react'
import { Bar, BarSvgProps } from '@nivo/bar'

export class BarChartProps {
    dataSets: any[];
    chartProps?: BarSvgProps<any>
}

function BarChart({ dataSets, chartProps }: BarChartProps) {
    return (
        // <ResponsiveLine margin={{ top: 50, right: 50, bottom: 50, left: 100 }} {...chartProps}
        // />
        <Bar
            {...chartProps}
            data={dataSets}
        />

    )
}

export { BarChart };