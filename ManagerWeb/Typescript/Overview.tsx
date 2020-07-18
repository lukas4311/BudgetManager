import * as React from 'react'
import * as ReactDOM from 'react-dom';
import PaymentsOverview from './PaymentsOverview'

class Overview extends React.Component {
    render() {
        return (
            <div>
                <p> Základní přehled</p>
                <PaymentsOverview />
            </div>
        )
    }
}

ReactDOM.render(<Overview />, document.getElementById('overview'));