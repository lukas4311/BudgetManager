import { ResponsiveRadar } from '@nivo/radar';
import _ from 'lodash';
import * as React from 'react';
import { CategoryGroupedPaymentsProps } from './CategoryGroupedPaymentsProps';

export const CategoryGroupedPayments = (props: CategoryGroupedPaymentsProps) => {
    const data = _.chain(props.payments).filter(a => a.paymentTypeCode == "Expense")
        .map(r => ({ label: r.paymentCategoryCode, spend: r.amount }))
        .groupBy(p => p.label).value();

    const result = _.chain(data)
        .map(group => ({ label: group[0].label, value: _.sumBy(group, g => g.spend) })
        )
        .value();
    const orderredResult = _.orderBy(result, ['value'], ['desc']);

    return (
        <div>
            <h2 className="text-2xl mb-4 text-left">Monthly grouped</h2>
            <div className='h-64 paymentsRadar'>
                <ResponsiveRadar
                    data={orderredResult}
                    keys={['value']}
                    indexBy="label"
                    margin={{ top: 40, right: 40, bottom: 40, left: 40 }}
                    borderColor={{ from: 'color' }}
                    dotSize={2}
                    dotColor={{ theme: 'background' }}
                    dotBorderWidth={2}
                    colors={['#920000']} />
            </div>
        </div>
    );
};
