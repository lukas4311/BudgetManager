import * as React from 'react'
import moment from 'moment';

interface IPaymentInfo {
    name: string,
    amount: number,
    date: string,
    id: number
}

interface PaymentsOverviewState {
    payments: Array<IPaymentInfo>
}

export default class PaymentsOverview extends React.Component<{}, PaymentsOverviewState>{

    constructor(props: {}) {
        super(props);
        moment.locale('cs');
        let now = moment().format('L');
        let testPayments: Array<IPaymentInfo> = new Array({ id: 1, name: "tset", amount: 100, date: now });
        this.state = { payments: testPayments };
    }

    render() {
        return (
            <div className="text-center mt-6">
                <h2 className="text-xl">Platby</h2>
                <table>
                    {this.state.payments.map(p =>
                        <tr key={p.id} className="bg-battleshipGrey p-2 rounded-xl">
                            <td>{p.amount}</td>
                            <td>{p.name}</td>
                            <td>{p.date.toString()}</td>
                        </tr>
                    )}

                </table>
            </div>
        )
    }
}