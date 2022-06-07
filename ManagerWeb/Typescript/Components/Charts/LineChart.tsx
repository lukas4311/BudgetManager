import { ResponsiveLine } from '@nivo/line'
import React from 'react'
import { LineChartProps } from '../../Model/LineChartProps';

const data = [
  {
    id: "hours",
    data: [
      { x: '2020-03-02 12:00', y: 200000 },
      { x: '2021-10-04 12:00', y: 220800 },
      { x: '2021-12-14 12:00', y: 221800 }
    ]
  }
];

function LineChart({ dataSets, chartProps }: LineChartProps) {
  let allYData: number[] = [];
  dataSets.map(a => a.data.map(c => c.y)).forEach(c => allYData = allYData.concat(c));

  if (chartProps == undefined)
    chartProps = { data: dataSets };
  else
    chartProps.data = dataSets;

  return (
    <ResponsiveLine margin={{ top: 50, right: 50, bottom: 50, left: 100 }} {...chartProps}
      xScale={{
        type: "time",
        format: "%Y-%m-%d %H:%M",
        precision: "day"
      }}
      xFormat="time:%Y-%m-%d"
      axisBottom={{
        format: "%Y-%m-%d",
        // legend: "day hour",
        // legendOffset: -80,
        // legendPosition: "middle"
      }}
    />

    // <ResponsiveLine
    //   data={chartProps.data}
    //   margin={{ top: 50, right: 60, bottom: 50, left: 120 }}
    //   yScale={{ type: "point" }}
    //   xScale={{
    //     type: "time",
    //     format: "%Y-%m-%d %H:%M",
    //     precision: "day"
    //   }}
    //   xFormat="time:%Y-%m-%d"
    //   axisBottom={{
    //     format: "%Y-%m-%d",
    //     // legend: "day hour",
    //     // legendOffset: -80,
    //     // legendPosition: "middle"
    //   }}
    //   pointSize={10}
    //   pointColor="white"
    //   pointBorderWidth={2}
    //   pointBorderColor={{ from: "serieColor" }}
    //   useMesh={true}
    // />
  )
}

export { LineChart };