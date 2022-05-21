import React from "react";

class OtherInvestmentSummaryState{

}

export default class OtherInvestmentSummary extends React.Component<{}, OtherInvestmentSummaryState>{

    constructor(props: {}) {
        super(props);
    }

    componentDidMount(): void {
        this.initData();
    }

    private initData = async () => {
        
    }

    public render() {
        return (
            <div>
                <h3 className="text-xl p-4 text-center">Other investment summary</h3>
            </div>
        );
    }
}

