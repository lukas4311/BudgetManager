import React from "react";
import OtherInvestmentViewModel from "../../Model/OtherInvestmentViewModel";

class OtherInvestmentDetailProps {
    selectedInvestment: OtherInvestmentViewModel;
}

export default class OtherInvestmentDetail extends React.Component<OtherInvestmentDetailProps, {}>{
    constructor(props: OtherInvestmentDetailProps) {
        super(props);
    }

    render = () => {
        return (
            <div className="bg-battleshipGrey rounded-xl m-6 p-4">
                <div className="flex flex-row justify-center">
                    <h2 className="text-vermilion text-3xl font-bold">{this.props.selectedInvestment?.code} detail</h2>
                    <p className="self-end ml-4 mr-2">Initial invest</p>
                    <h2 className="text-vermilion text-2xl font-bold self-center">{this.props.selectedInvestment?.openingBalance}</h2>
                </div>
                <div className="grid grid-cols-2 gap-4">
                    <div>
                        <p>Curent value</p>
                        <p>Overall progress</p>
                        <p>Y/Y progress</p>
                    </div>
                    <div>GRAF</div>
                </div>

            </div>
        );
    }
}