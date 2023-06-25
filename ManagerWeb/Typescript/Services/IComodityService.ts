import { ComodityTradeHistoryModel } from "../ApiClient/Main";
import { ComoditiesFormViewModel } from "../Components/Comodities/ComoditiesForm";
import { ComodityTypeViewModel } from "./ComodityService";

export interface IComodityService {
    getComodityTypes(): Promise<ComodityTypeViewModel[]>;
    getAllComodityTrades(comodityId: number): Promise<ComoditiesFormViewModel[]>;
    getGoldPriceInCurrency(currencyCode: string): Promise<number>;
    createComodityTrade(tradeModel: ComodityTradeHistoryModel): Promise<void>;
    updateComodityTrade(tradeModel: ComodityTradeHistoryModel): Promise<void>;
    deleteComodityTrade(comodityId: number): Promise<void>;
    getComodityNetWorth(): Promise<number>;
}
