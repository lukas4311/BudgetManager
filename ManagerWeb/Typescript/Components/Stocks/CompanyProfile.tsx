import React from "react";

class CompanyProfileProps {
    ticker: string;
}


export const CompanyProfile = (props: CompanyProfileProps) => {
    return (
        <div>
            <h2 className="text-xl font-semibold">Company profile</h2>
            <h3 className="text-md font-semibold text-vermilion">{props.ticker}</h3>
            <div>
                
            </div>
        </div>
    );
}