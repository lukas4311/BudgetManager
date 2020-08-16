import { ResponsiveLine } from '@nivo/line'
import React from 'react'
import { LineChartProps } from './Model/LineChartProps';

function LineChart ({dataSets}: LineChartProps){
  return (<ResponsiveLine
    data={dataSets}
    margin={{ top: 50, right: 50, bottom: 50, left: 50 }}
    xScale={{
      type: 'time',
      format: '%Y-%m-%d',
      useUTC: false,
      precision: 'day',
    }}
    xFormat="time:%Y-%m-%d"
    yScale={{ type: 'linear', reverse: false }}
    axisLeft={{
      legend: 'linear scale',
      legendOffset: 12,
      tickValues: 6,
      tickPadding: 15
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
    enableArea={true}
    areaOpacity={0.25}
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
  />)
}

export { LineChart };