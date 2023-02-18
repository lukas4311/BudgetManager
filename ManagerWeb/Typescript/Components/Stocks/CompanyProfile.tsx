import React from "react";
import { CompanyProfileModel } from "../../ApiClient/Main/models";

class CompanyProfileProps {
    companyProfile: CompanyProfileModel;
}


export const CompanyProfile = (props: CompanyProfileProps) => {
    return (
        <div className="p-4">
            {props.companyProfile ? (
                <>
                    <div className="flex flex-row">
                        <h2 className="text-xl font-semibold mb-2 mr-2">Company profile</h2>
                        <h3 className="text-md font-semibold text-vermilion">({props.companyProfile.symbol})</h3>
                    </div>
                    <div>
                        <h4>{props.companyProfile.companyName}</h4>

                        <div className="flex flex-row justify-evenly text-center mt-4">
                            <div className="w-1/3">
                                <p className="text-xs">Sector</p>
                                <p className="text-md">{props.companyProfile.sector}</p>
                            </div>
                            <div className="w-1/3">
                                <p className="text-xs">Exchange</p>
                                <p className="text-md">{props.companyProfile.exchangeShortName}</p>
                            </div>
                            <div className="w-1/3">
                                <p className="text-xs">Currency</p>
                                <p className="text-md">{props.companyProfile.currency}</p>
                            </div>
                        </div>
                        <p className="mt-4">{props.companyProfile.description}</p>
                    </div>
                </>
            ) : <></>}
        </div>
    );
}