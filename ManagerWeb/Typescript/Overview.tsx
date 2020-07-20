import * as React from 'react'
import * as ReactDOM from 'react-dom';
import PaymentsOverview from './PaymentsOverview'

class Overview extends React.Component {
    render() {
        return (
            <div className="">
                <p className="text-3xl text-center mt-6">Základní přehled</p>
                <div className="w-1/3 p-4"><PaymentsOverview /></div>
                <div className="w-1/3"></div>
                <div className="w-1/3"></div>
            </div>
        )
    }
}

ReactDOM.render(<Overview />, document.getElementById('overview'));