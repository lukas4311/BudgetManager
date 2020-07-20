import * as React from 'react'

export default class PaymentForm extends React.Component<{},{}>{
    render(){
        return (
            <div className="flex">
                <div className="w-1/2">
                    <input value="" placeholder="Název výdaje"></input>
                </div>
                <div className="w-1/2">

                </div>
            </div>
        );
    }
}