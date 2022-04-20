import { ResponsiveLine } from '@nivo/line'
import React from 'react'
import { LineChartProps } from '../../Model/LineChartProps';

function LineChart({ dataSets, chartProps }: LineChartProps) {
  let allYData: number[] = [];
  dataSets.map(a => a.data.map(c => c.y)).forEach(c => allYData = allYData.concat(c));
  let minY = Math.min(...allYData);
  let maxY = Math.max(...allYData);
  let yScale = (minY / 100);

  if(chartProps == undefined)
    chartProps = {data: dataSets};
  else
    chartProps.data = dataSets;

  if (yScale < 1000)
    yScale = 1000;

  return (<ResponsiveLine margin={{ top: 50, right: 50, bottom: 50, left: 100 }} {...chartProps} />)
}

export { LineChart };