import _ from 'lodash';
import * as React from 'react';
import { IncomeExpenseCardProps } from './IncomeExpenseCardProps';

export const ExpenseCard = (props: IncomeExpenseCardProps) => {
    const expense = props.payments.filter(p => p.paymentTypeCode == "Expense");
    const sum = _.sumBy(expense, p => p.amount);

    return (
        <div className={`flex flex-col bg-battleshipGrey px-4 py-6 rounded-lg relative ${props?.cardClass ?? ""}`}>
            <span className="text-2xl text-left font-semibold categoryIcon fill-white">Expense</span>
            <p className='text-4xl text-center font-black mb-2'>{sum},-</p>
        </div>
    );
};
