import _ from "lodash";
import moment from "moment";
import React, { useEffect, useState } from "react";
import { ComodityApi } from "../../ApiClient/Main";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { GoldListProps } from "./GoldListProps";

const Gold = (props: GoldListProps) => {
    const ounce = 28.34;
    const goldIngots = props.comoditiesViewModels ?? [];
    const totalWeight = _.sumBy(goldIngots, (g) => g.comodityAmount);
    const totalCosts = _.sumBy(goldIngots, (g) => g.price);
    const goldUnit = props.comoditiesViewModels[0]?.comodityUnit;
    const currency = props.comoditiesViewModels[0]?.currencySymbol;
    const [actualTotalPrice, setActualTotalPrice] = useState("");

    useEffect(() => {
        async function calculateTotalActualPrice() {
            const apiFactory = new ApiClientFactory(null);
            const totalWeight = _.sumBy(props.comoditiesViewModels ?? [], (g) => g.comodityAmount);
            let comodityApi = await apiFactory.getClient(ComodityApi);
            const price = await comodityApi.comoditiesGoldActualPriceCurrencyCodeGet({ currencyCode: "CZK" });
            const actualTotalPrice = totalWeight * price / ounce;
            setActualTotalPrice(actualTotalPrice.toFixed(1));
        }

        calculateTotalActualPrice()
    }, [props.comoditiesViewModels])

    return (
        <div id="goldCards">
            <h3 className="text-xl text-center">Gold</h3>
            <div className="mt-3 flex flex-row justify-center flex-nowrap text-center cursor-default">
                {goldIngots.map((g, i) => (
                    <div key={g.id} className={"relative p-1 bg-gold-brighter rounded-xl inline-block goldCard shadow-2xl z-0 overflow-hidden" + (i == 0 ? "" : " cardOverlap")}
                        onClick={() => props.editIngot(g.id)}>
                        <div className="w-11/12 z-negative1 bg-gold rotateBox">
                        </div>
                        <div className={"px-2 py-6 rounded-xl bg-gold z-10"}>
                            <p className="font-medium goldText">{g.company}</p>
                            <p className="text-2xl font-bold mt-4 goldText">{g.comodityAmount}g</p>
                            <p className="mt-6 goldText">{moment(g.buyTimeStamp).toDate().toLocaleDateString()}</p>
                        </div>
                    </div>
                ))}
                <div className="relative p-1 bg-gold-brighter rounded-xl inline-block goldCard shadow-2xl z-0 overflow-hidden cardOverlap" onClick={props.addNewIngot}>
                    <div className="w-11/12 z-negative1 bg-gold rotateBox">
                    </div>
                    <div className="px-2 py-6 rounded-xl bg-gold z-10 h-full flex items-center justify-center">
                        <p className="font-medium goldText text-7xl font-black">+</p>
                    </div>
                </div>
            </div>
            <div className="text-center block">
                <h4 className="text-xl mt-12">Summary info</h4>
                <p className="mt-2">Total weight: {totalWeight} {goldUnit}</p>
                <p className="">Total costs: {totalCosts} {currency}</p>
                <p>Actual total price: {actualTotalPrice} {currency}</p>
            </div>
        </div>
    );
}

export default Gold;