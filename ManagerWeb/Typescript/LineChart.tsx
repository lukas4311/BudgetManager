import { ResponsiveLine } from '@nivo/line'
import React from 'react'

const LineChart = ({ data }) => (
    <ResponsiveLine
        data={data}
        margin={{ top: 50, right: 110, bottom: 50, left: 60 }}
        xScale={{
            type: 'time',
            format: '%Y-%m-%d',
            useUTC: false,
            precision: 'day',
        }}
        xFormat="time:%Y-%m-%d"
        yScale={{
            type: 'linear'
        }}
        axisLeft={{
            legend: 'linear scale',
            legendOffset: 12,
        }}
        axisBottom={{
            format: '%b %d',
            tickValues: 'every 2 days',
            legend: 'time scale',
            legendOffset: -12,
        }}
        curve='linear'
        enablePointLabel={true}
        pointSize={7}
        useMesh={true}
        enableSlices={false}
        enableArea={true}
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
              }
            },
            dots: {
                text: {
                    fill: 'white',
                }
            }
          }}
    />
)

export { LineChart };