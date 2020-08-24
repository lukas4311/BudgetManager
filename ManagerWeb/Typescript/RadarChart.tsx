import { ResponsiveRadar } from '@nivo/radar'
import React from 'react'
import { RadarChartProps } from './Model/RadarChartProps';

function RadarChart({ dataSets }: RadarChartProps) {
  return (
    <ResponsiveRadar
      data={dataSets}
      keys={['chardonay', 'carmenere', 'syrah']}
      indexBy="taste"
      maxValue="auto"
      margin={{ top: 70, right: 80, bottom: 40, left: 80 }}
      curve="linearClosed"
      borderWidth={0}
      borderColor={{ from: 'color' }}
      gridLevels={8}
      gridShape="circular"
      gridLabelOffset={27}
      enableDots={true}
      dotSize={2}
      dotColor={{ theme: 'background' }}
      dotBorderWidth={2}
      dotBorderColor={{ from: 'color' }}
      enableDotLabel={false}
      dotLabel="value"
      dotLabelYOffset={-17}
      colors={{ scheme: 'paired' }}
      fillOpacity={0.75}
      blendMode="normal"
      animate={true}
      motionStiffness={85}
      motionDamping={15}
      isInteractive={true}
      legends={[
        {
          anchor: 'top-left',
          direction: 'column',
          translateX: -50,
          translateY: -40,
          itemWidth: 80,
          itemHeight: 20,
          itemTextColor: '#999',
          symbolSize: 12,
          symbolShape: 'circle',
          effects: [
            {
              on: 'hover',
              style: {
                itemTextColor: '#000'
              }
            }
          ]
        }
      ]}
    />
  )
}

export { RadarChart };