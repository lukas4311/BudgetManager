import { ResponsivePieCanvas } from '@nivo/pie'
import React from 'react'

class PieChartData {
    id: string;
    label: string;
    value: number;
}

class PieChartProps {
    data: PieChartData[];
    labelPostfix?: string;
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
            arcLinkLabelsTextColor="#ffffff"
            arcLabelsTextColor="ffffff"
            arcLinkLabelsSkipAngle={10}
            arcLabelsSkipAngle={10}
            sortByValue={true}
            tooltip={(p) => { return <p className="bg-black p-2 rounded-xl">{p.datum.label} : {p.datum.value} {props?.labelPostfix ?? ""}</p> }}
        />
    )
}

export { PieChart, PieChartData, PieChartProps };