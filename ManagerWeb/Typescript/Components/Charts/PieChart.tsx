import { ResponsivePieCanvas } from '@nivo/pie'
import React from 'react'

function PieChart(props) {
    return (
        <ResponsivePieCanvas
            data={(props.chartData ?? [])}
            margin={{ top: 40, right: 200, bottom: 40, left: 80 }}
            innerRadius={0.5}
            padAngle={0.7}
            cornerRadius={3}
            colors={{ scheme: 'paired' }}
            borderColor={{ from: 'color', modifiers: [ [ 'darker', 0.6 ] ] }}
            radialLabelsSkipAngle={10}
            radialLabelsTextColor="#333333"
            radialLabelsLinkColor={{ from: 'color' }}
            sliceLabelsSkipAngle={10}
            sliceLabelsTextColor="#333333"
            legends={[
                {
                    anchor: 'right',
                    direction: 'column',
                    justify: false,
                    translateX: 140,
                    translateY: 0,
                    itemsSpacing: 2,
                    itemWidth: 60,
                    itemHeight: 14,
                    itemTextColor: '#999',
                    itemDirection: 'left-to-right',
                    itemOpacity: 1,
                    symbolSize: 14,
                    symbolShape: 'circle'
                }
            ]}
        />
    )
}

export { PieChart };