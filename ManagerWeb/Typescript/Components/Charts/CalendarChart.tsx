import { Calendar, ResponsiveCalendar } from '@nivo/calendar'
import moment from 'moment'
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
            from={moment(`${props.fromYear}-01-01`).toISOString()}
            to={moment(`${props.toYear}-12-31`).toISOString()}
            emptyColor="#eeeeee"
            colors={[ '#61cdbb', '#97e3d5', '#e8c1a0', '#f47560' ]}
            margin={{ top: 20, right: 30, bottom: 10, left: 30 }}
            yearSpacing={40}
            monthSpacing={5}
            daySpacing={2}
            isInteractive={true}
            monthLegend={(year, month) => czechMonths[month]}
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