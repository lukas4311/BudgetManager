import { ResponsiveRadar } from '@nivo/radar'
import React from 'react'
import { RadarChartProps } from './Model/RadarChartProps';

function RadarChart({ dataSets }: RadarChartProps) {
  return (
    <ResponsiveRadar
      data={dataSets}
      keys={['value']}
      indexBy="key"
      maxValue="auto"
      margin={{ top: 40, right: 40, bottom: 40, left: 40 }}
      curve="linearClosed"
      borderWidth={0}
      borderColor={{ from: 'color' }}
      gridLevels={8}
      gridShape="circular"
      gridLabelOffset={27}
      enableDots={false}
      dotSize={10}
      dotBorderWidth={0}
      enableDotLabel={false}
      dotColor={{ theme: 'background' }}
      dotBorderColor={{ from: 'color' }}
      dotLabel="value"
      dotLabelYOffset={-17}
      colors={{ scheme: 'category10' }}
      fillOpacity={0.75}
      blendMode="normal"
      animate={true}
      motionStiffness={85}
      motionDamping={15}
      isInteractive={true}
      legends={[]}
      tooltipFormat={value =>
        `${Number(value).toLocaleString('cs-CZ', {
          minimumFractionDigits: 2,
        })},-`
      }
    />
  )
}

export { RadarChart };