import * as React from 'react'
import * as ReactDOM from 'react-dom';
import ErrorBoundary from './Utils/ErrorBoundry';
import PaymentsOverview from './Components/Payments/PaymentsOverview'

export default class Overview extends React.Component {
    render() {
        return (
            <div className="">
                <p className="text-3xl text-center mt-6">Základní přehled</p>
                {/* <div className="w-full lg:p-4"><PaymentsOverview /></div> */}
            </div>
        )
    }
}

// ReactDOM.render(<ErrorBoundary><Overview /></ErrorBoundary>, document.getElementById('overview'));