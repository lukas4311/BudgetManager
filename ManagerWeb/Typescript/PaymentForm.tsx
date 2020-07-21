import * as React from 'react'

export default class PaymentForm extends React.Component<{}, {}>{
    render() {
        return (
            <div className="bg-prussianBlue text-white">
                <h2 className="text-2xl py-4 ml-6 text-left">Detail platby</h2>
                <div className="flex">
                    <div className="w-1/2">
                        <div className="relative inline-block float-left ml-6">
                            <input className="effect-11" placeholder="Název výdaje"></input>
                            <span className="focus-bg"></span>
                        </div>
                    </div>
                    <div className="w-1/2">
                        <div className="relative inline-block float-left ml-6">
                            <input className="effect-11" placeholder="Výše výdaje"></input>
                            <span className="focus-bg"></span>
                        </div>
                    </div>
                </div>
                <div className="flex mt-4">
                    <div className="w-1/2">
                        <div className="relative inline-block float-left ml-6">
                            <input type="date" className="effect-11" placeholder="Datum"></input>
                            <span className="focus-bg"></span>
                        </div>
                    </div>
                </div>
                <div className="flex my-4">
                    <div className="w-full">
                        <div className="relative inline-block w-4/5 float-left ml-6">
                            <input className="effect-11 w-full" placeholder="Popis"></input>
                            <span className="focus-bg"></span>
                        </div>
                    </div>
                </div>
                <div className="flex">
                    <div className="w-full">
                        <div className="relative inline-block float-left ml-6 mb-6">
                            <a type="button" href="#" className="bg-vermilion px-4 py-1 rounded-sm">Potvrdit</a>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}