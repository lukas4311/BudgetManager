
import React from 'react'
import { Bar, BarSvgProps, ResponsiveBar } from '@nivo/bar'

export class BarChartProps {
    dataSets: any[];
    chartProps?: BarSvgProps<any>
}

function BarChart({ dataSets, chartProps }: any) {
    return (
        <ResponsiveBar
            {...chartProps}
            data={dataSets}
        />
    )
}

export { BarChart };