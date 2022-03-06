import * as React from 'react'
import { RouteComponentProps } from 'react-router-dom';
import PaymentsOverview from './Components/Payments/PaymentsOverview'

export default class Overview extends React.Component<RouteComponentProps, {}> {
    constructor(props: RouteComponentProps) {
        super(props);
    }

    render() {
        return (
            <div className="">
                <div className="w-full lg:p-4"><PaymentsOverview {...this.props} /></div>
            </div>
        )
    }
}