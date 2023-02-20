import _ from "lodash";
import moment from "moment";
import React from "react";
import { LineChartData } from "../../Model/LineChartData";
import { LineChartDataSets } from "../../Model/LineChartDataSets";
import { StockViewModel } from "../../Model/StockViewModel";
import { BaseList } from "../BaseList";
import { LineChart } from "../Charts/LineChart";
import { LineChartSettingManager } from "../Charts/LineChartSettingManager";
import { BuySellBadge } from "../Crypto/CryptoTrades";
import { StockComplexModel } from "./StockOverview";

class CompanyProfileProps {
    companyProfile: StockComplexModel;
}


export const CompanyProfile = (props: CompanyProfileProps) => {

    let profile = props?.companyProfile?.companyInfo;
    let priceChart: LineChartDataSets[] = [{ id: 'Price', data: [] }];

    if (props?.companyProfile?.company5YPrice.length > 5) {
        let prices = props.companyProfile.company5YPrice;
        const sortedArray = _.orderBy(prices, [(obj) => new Date(obj.time)], ['asc'])
        let priceData: LineChartData[] = sortedArray.map(b => ({ x: moment(b.time).format('YYYY-MM-DD'), y: b.price }))
        priceChart = [{ id: 'Price', data: priceData }];
    }

    const renderTemplate = (s: StockViewModel): JSX.Element => {
        return (
            <>
                <p className="w-1/4 h-full border border-vermilion flex items-center justify-center">{s.tradeSize}</p>
                <p className="w-1/4 h-full border border-vermilion flex items-center justify-center">{Math.abs(s.tradeValue).toFixed(2)} ({s.currencySymbol})</p>
                <p className="w-1/4 h-full border border-vermilion flex items-center justify-center">{s.tradeTimeStamp}</p>
                <p className="w-1/4 h-full border border-vermilion flex items-center justify-center py-1"><BuySellBadge tradeValue={s.tradeValue} /></p>
            </>
        );
    }

    return (
        <div className="p-4">
            <div className="flex flex-col">
                {profile ? (
                    <>
                        <div className="flex flex-row">
                            <h2 className="text-xl font-semibold mb-2 mr-2">Company profile</h2>
                            <h3 className="text-md font-semibold text-vermilion">({profile.symbol})</h3>
                        </div>
                        <div>
                            <h4>{profile.companyName}</h4>

                            <div className="flex flex-row justify-evenly text-center mt-4">
                                <div className="w-1/3">
                                    <p className="text-xs">Sector</p>
                                    <p className="text-md">{profile.sector}</p>
                                </div>
                                <div className="w-1/3">
                                    <p className="text-xs">Exchange</p>
                                    <p className="text-md">{profile.exchangeShortName}</p>
                                </div>
                                <div className="w-1/3">
                                    <p className="text-xs">Currency</p>
                                    <p className="text-md">{profile.currency}</p>
                                </div>
                            </div>
                            <p className="mt-4">{profile.description}</p>
                        </div>
                    </>
                ) : <></>}
                <div className="h-52 mt-6 mb-12">
                    <LineChart dataSets={priceChart} chartProps={LineChartSettingManager.getStockChartSettingForCompanyInfo()}></LineChart>
                </div>
                <div className="flex flex-col justify-center">
                    <BaseList<StockViewModel> data={props.companyProfile?.trades ?? []} template={renderTemplate} hideIconRowPart={true}></BaseList>
                </div>
            </div>
        </div>
    );
}