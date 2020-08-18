import { ResponsiveLine } from '@nivo/line'
import React from 'react'
import { LineChartProps } from './Model/LineChartProps';
import { LineChartDataSets } from './Model/LineChartDataSets';

function LineChart({ dataSets }: LineChartProps) {
  var allYData: number[] = [];
  dataSets.map(a => a.data.map(c => c.y)).forEach(c => allYData = allYData.concat(c));
  var minY = Math.min(...allYData);
  var maxY = Math.max(...allYData);

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
    yScale={{ type: 'linear', reverse: false, min: minY - (minY / 100), max: maxY + (minY / 100) }}
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
    colors={{ scheme: 'category10' }}
    curve='linear'
    enablePointLabel={true}
    pointSize={7}
    useMesh={true}
    enableArea={true}
    areaOpacity={0.25}
    areaBaselineValue={minY - (minY / 100)}
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