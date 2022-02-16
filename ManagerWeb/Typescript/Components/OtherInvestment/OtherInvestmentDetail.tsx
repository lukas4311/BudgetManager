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
            <div>
                <h2>Detail</h2>
                <p>{this.props.selectedInvestment?.code}</p>
            </div>
        );
    }
}