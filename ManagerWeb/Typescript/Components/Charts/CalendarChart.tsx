import { Calendar, ResponsiveCalendar } from '@nivo/calendar'
import React from 'react'
import { CalendarChartProps } from '../../Model/CalendarChartProps'

function CalendarChart(props: CalendarChartProps) {

    const czechMonths = [
        'Leden',
        'Únor',
        'Březen',
        'Duben',
        'Květen',
        'Ćerven',
        'Červenec',
        'Srpen',
        'Září',
        'Říjen',
        'Listopad',
        'Prosinec',
    ]

    return (
        <ResponsiveCalendar
            data={props.dataSets}
            from="2020-01-01"
            to="2020-12-31"
            emptyColor="#eeeeee"
            colors={['#61cdbb', '#97e3d5', '#e8c1a0', '#f47560']}
            margin={{ top: 20, right: 30, bottom: 10, left: 30 }}
            yearSpacing={10}
            monthBorderColor="#000000"
            dayBorderWidth={2}
            dayBorderColor="#000000"
            isInteractive={true}
            monthLegend={(year, month) => czechMonths[month]}
            tooltip={({ day, value, color }) => (
                <strong style={{ color }}>
                    {day}: {value}
                </strong>
            )}
            theme={{
                tooltip: {
                    container: {
                        background: '#333',
                    },
                }
            }}
        />
    )
}

export { CalendarChart }