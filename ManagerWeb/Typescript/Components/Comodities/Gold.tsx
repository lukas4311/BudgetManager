import _ from "lodash";
import React, { useEffect, useState } from "react";
import { ComodityApi } from "../../ApiClient/Main";
import ApiClientFactory from "../../Utils/ApiClientFactory";
import { GoldListProps } from "./GoldListProps";

const Gold = (props: GoldListProps) => {
    const ounce = 28.34;
    const goldIngots = props.goldIngots ?? [];
    const totalWeight = _.sumBy(goldIngots, (g) => g.weight);
    const totalCosts = _.sumBy(goldIngots, (g) => g.costs);
    const goldUnit = props.goldIngots[0]?.unit;
    const currency = props.goldIngots[0]?.currency;
    const [actualTotalPrice, setActualTotalPrice] = useState("");

    useEffect(() => {
        async function calculateTotalActualPrice() {
            const apiFactory = new ApiClientFactory(null);
            const totalWeight = _.sumBy(props.goldIngots ?? [], (g) => g.weight);
            let comodityApi = await apiFactory.getClient(ComodityApi);
            const price = await comodityApi.comoditiesGoldActualPriceCurrencyCodeGet({ currencyCode: "CZK" });
            const actualTotalPrice = totalWeight * price / ounce;
            setActualTotalPrice(actualTotalPrice.toFixed(1));
        }

        calculateTotalActualPrice()
    }, [props.goldIngots])

    return (
        <div id="goldCards">
            <h3 className="text-xl">Gold</h3>
            <div className="mt-3 flex flex-row flex-nowrap text-center cursor-default">
                {goldIngots.map((g, i) => (
                    <div key={g.id} className={"relative p-1 bg-gold-brighter rounded-xl inline-block goldCard shadow-2xl z-0 overflow-hidden" + (i == 0 ? "" : " cardOverlap")}>
                        <div className="w-11/12 z-negative1 bg-gold rotateBox">
                        </div>
                        <div className={"px-2 py-6 rounded-xl bg-gold z-10"}>
                            <p className="font-medium goldText">{g.company}</p>
                            <p className="text-2xl font-bold mt-4 goldText">{g.weight}g</p>
                            <p className="mt-6 goldText">{g.boughtDate.toLocaleDateString()}</p>
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
            <div>
                <h4 className="text-lg mt-6">Summary info</h4>
                <p className="mt-2">Total weight: {totalWeight} {goldUnit}</p>
                <p className="">Total costs: {totalCosts} {currency}</p>
                <p>Actual total price: {actualTotalPrice} {currency}</p>
            </div>
        </div>
    );
}

export default Gold;