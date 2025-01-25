import { ResponsiveBar } from '@nivo/bar';
import * as React from 'react';
import { useState, useEffect } from 'react';
import { useHistory } from 'react-router-dom';
import { PaymentApi } from '../../ApiClient/Main';
import PaymentService from '../../Services/PaymentService';
import ApiClientFactory from '../../Utils/ApiClientFactory';
import { MonthlyGroupedPaymentsProps } from './MonthlyGroupedPaymentsProps';

export const MonthlyGroupedPayments = (props: MonthlyGroupedPaymentsProps) => {
    const history = useHistory();
    const [data, setData] = useState<any>([]);

    useEffect(() => {

        const loadData = async () => {
            const apiFactory = new ApiClientFactory(history);
            const paymentApi = await apiFactory.getClient(PaymentApi);
            const paymentService = new PaymentService(paymentApi);
            const groupedPayments = paymentService.groupPaymentsAndExpenseByMonth(props.payments);
            const data = groupedPayments.map(g => ({ key: g.dateGroup, revenue: g.revenueSum, expense: Math.abs(g.expenseSum), savings: g.revenueSum + g.expenseSum }));
            setData(data);
        };

        loadData();
    }, [props]);

    return (
        <div>
            <h2 className="text-2xl mb-4 text-left">Monthly grouped</h2>
            <div className='h-64'>
                <ResponsiveBar
                    data={data}
                    keys={[
                        'revenue',
                        'expense',
                        'savings'
                    ]}
                    indexBy="key"
                    enableLabel={false}
                    margin={{ top: 50, right: 60, bottom: 50, left: 60 }}
                    padding={0.6}
                    groupMode="grouped"
                    valueScale={{ type: 'linear' }}
                    indexScale={{ type: 'band', round: true }}
                    colors={['#007E04', '#920000', '#3572EF']}
                    borderColor={{
                        from: 'color',
                        modifiers: [
                            [
                                'darker',
                                1.6
                            ]
                        ]
                    }}
                    theme={{
                        axis: {
                            ticks: {
                                line: { stroke: "white" },
                                text: { fill: "white" }
                            }
                        },
                        grid: {
                            line: { stroke: "white" }
                        }
                    }}
                    role="application"
                    tooltip={({
                        id, value,
                    }) => <div className='px-4 py-2 bg-prussianBlue text-white border border-white'>
                            <strong>
                                {id}: {value}
                            </strong>
                        </div>} />
            </div>
        </div>
    );
};
