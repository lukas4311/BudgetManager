import { ResponsiveStream } from '@nivo/stream'
import React from 'react'

function AreaChart(props) {
    return (
        <ResponsiveStream
            data={(props.chartData ?? [])}
            keys={['Stav účtu']}
            margin={{ top: 50, right: 110, bottom: 50, left: 60 }}
            axisTop={null}
            axisRight={null}
            axisBottom={{
                // orient: 'bottom',
                tickSize: 5,
                tickPadding: 5,
                tickRotation: 0,
                legend: '',
                legendOffset: 36
            }}
            theme={{
                axis: {
                    ticks: {
                        line: {
                            stroke: "white"
                        },
                        text: {
                            fill: "white"
                        }
                    }
                },
                grid: {
                    line: {
                        stroke: "white",
                        strokeWidth: 1,
                    }
                }
            }}
            axisLeft={{ tickSize: 5, tickPadding: 5, tickRotation: 0, legend: '', legendOffset: -40 }}
            enableGridY={true}
            curve="linear"
            offsetType="none"
            order="ascending"
            colors={{ scheme: 'red_blue' }}
            borderColor={{ theme: 'background' }}
            dotSize={8}
            dotColor={{ from: 'color' }}
            dotBorderWidth={2}
            dotBorderColor={{ from: 'color', modifiers: [['darker', 0.7]] }}
            animate={true}
            // motionStiffness={90}
            // motionDamping={15}
            legends={[
                {
                    "anchor": "top-right",
                    "direction": "column",
                    "translateY": -25,
                    "translateX": 100,
                    "itemWidth": 100,
                    "itemHeight": 48.2,
                    "symbolSize": 12,
                    "symbolShape": "circle",
                    "itemTextColor": "#eee", // <= this worked for me in the end
                },
            ]}
        />
    )
}

export { AreaChart };