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
        this.state = { payments: [] };
    }

    componentDidMount(){
        fetch("/Payment/GetPaymentsData")
            .then(res => {
                if (res.ok)
                    return res.json()
            })
            .then(
                (result) => {
                    if (result != undefined) {
                        this.setState({
                            payments: result
                        });
                    }
                },
                (error) => {}
            )
    }

    render() {
        return (
            <div className="text-center mt-6 bg-prussianBlue rounded-lg">
                <div className="py-4 flex">
                    <h2 className="text-xl ml-12">Platby</h2>
                    <span className="inline-block ml-auto mr-5">
                        <svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 0 24 24" width="24" className="fill-current text-white hover:text-vermilion transition ease-out duration-700 cursor-pointer">
                            <path d="M0 0h24v24H0z" fill="none"/>
                            <path d="M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-2 10h-4v4h-2v-4H7v-2h4V7h2v4h4v2z"/>
                        </svg>
                    </span>
                </div>
                <div className="pb-10">
                    {this.state.payments.map(p =>
                        <div key={p.id} className="paymentRecord bg-battleshipGrey p-2 rounded-r-full flex mr-6 mt-1 hover:bg-vermilion cursor-pointer">
                            <p className="mx-6 w-1/3">{p.amount},-</p>
                            <p className="mx-6 w-1/3">{p.name}</p>
                            <p className="mx-6 w-1/3">{moment(p.date).format('DD.MM.YYYY HH:mm')}</p>
                        </div>
                    )}
                </div>
            </div>
        )
    }
}