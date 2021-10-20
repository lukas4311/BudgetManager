import { ResponsivePieCanvas } from '@nivo/pie'
import React from 'react'

class PieChartData {
    id: string;
    label: string;
    value: number;
}

class PieChartProps {
    data: PieChartData[];
}

function PieChart(props: PieChartProps) {
    return (
        <ResponsivePieCanvas
            data={(props.data ?? [])}
            margin={{ top: 40, right: 200, bottom: 40, left: 80 }}
            innerRadius={0.6}
            padAngle={0}
            isInteractive={true}
            cornerRadius={0}
            colors={{ scheme: 'paired' }}
            borderColor={{ from: 'color', modifiers: [['darker', 0.6]] }}
            // enableRadialLabels={true}
            // radialLabel={d => `${d.id} (${d.formattedValue})`}
            // radialLabelsTextColor="#ffffff"
            // radialLabelsLinkColor={{ from: 'color' }}
            sortByValue={true}
        />
    )
}

export { PieChart, PieChartData };