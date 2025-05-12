import _ from 'lodash';
import moment from 'moment';
import * as React from 'react';
import { PaymentsStatsProps } from './PaymentsStatsProps';

export const PaymentsStats = (props: PaymentsStatsProps) => {
    let sumRevenues = 0;
    let sumExpense = 0;
    let sumSaved = 0;
    let months = 1;

    if (props.payments.length > 0) {
        const minDate = moment(props.payments[0].date);
        const maxDate = moment(props.payments[props.payments.length - 1].date);
        months = minDate.diff(maxDate, 'months') + 1;

        if (months == 0)
            months = 1;

        const revenues = props.payments.filter(p => p.paymentTypeCode == "Revenue");
        sumRevenues = _.sumBy(revenues, p => p.amount);

        const expense = props.payments.filter(p => p.paymentTypeCode == "Expense");
        sumExpense = _.sumBy(expense, p => p.amount);
        sumSaved = sumRevenues - sumExpense;
    }

    return (
        <div>
            <h2 className="text-2xl mb-8 text-left">Payment stats</h2>

            <div className='grid grid-cols-3 gap-y-5 text-left text-2xl'>
                <p className='col-span-2'>Monthly average income</p>
                <p className='font-bold'>{(sumRevenues / months).toFixed(0)}</p>
                <p className='col-span-2'>Monthly average expense</p>
                <p className='font-bold'>{(sumExpense / months).toFixed(0)}</p>
                <p className='col-span-2'>Monthly average saved</p>
                <p className='font-bold'>{(sumSaved / months).toFixed(0)}</p>
            </div>
        </div>
    );
};
