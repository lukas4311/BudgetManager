import { ResponsiveLine } from '@nivo/line'
import React from 'react'
import { LineChartProps } from './Model/LineChartProps';

function LineChart({ dataSets }: LineChartProps) {
  let allYData: number[] = [];
  dataSets.map(a => a.data.map(c => c.y)).forEach(c => allYData = allYData.concat(c));
  let minY = Math.min(...allYData);
  let maxY = Math.max(...allYData);
  let yScale = (minY / 100);

  if(yScale < 1000)
    yScale = 1000;

  return (<ResponsiveLine
    data={dataSets}
    margin={{ top: 50, right: 50, bottom: 50, left: 100 }}
    xScale={{
      type: 'time',
      format: '%Y-%m-%d',
      useUTC: false,
      precision: 'day',
    }}
    xFormat="time:%Y-%m-%d"
    yScale={{ type: 'linear', reverse: false, min: minY - yScale, max: maxY + yScale }}
    axisLeft={{
      legend: 'linear scale',
      legendOffset: 12,
      tickValues: 6,
      tickPadding: 15
    }}
    axisBottom={{
      format: '%Y-%m-%d',
      tickValues: 'every 2 days',
      legend: 'time scale',
      legendOffset: -12,
    }}
    colors={{ scheme: 'set1' }}
    curve='linear'
    enablePoints={false}
    enablePointLabel={false}
    pointSize={7}
    useMesh={true}
    enableArea={true}
    areaOpacity={0.5}
    areaBaselineValue={minY - yScale}
    enableSlices="y"
    sliceTooltip={({slice}) => {
      return (
        <div style={{background:'black', padding: '9px 12px'}}>
          {slice.points.map(point => (
            <div key={point.id} style={{color:'white', padding: '3px 0'}}>
               <span>{point.data.xFormatted}</span>
               <span style={{margin:'0px 8px'}}>{point.data.yFormatted}</span>
            </div>
          ))}
        </div>
      );
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