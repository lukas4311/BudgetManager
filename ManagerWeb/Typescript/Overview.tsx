import * as React from 'react'
import * as ReactDOM from 'react-dom';

class Overview extends React.Component{
    render() {
        return <p>Ahoj jak se mas</p>
    }
}

ReactDOM.render(<Overview />, document.getElementById('overview'));