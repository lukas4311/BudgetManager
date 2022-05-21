import { IBaseModel } from "../Components/BaseList";
import CurrencyTickerSelectModel from "../Components/Crypto/CurrencyTickerSelectModel";

export default class OtherInvestmentViewModel implements IBaseModel {
    id: number;
    created: string;
    code: string;
    name: string;
    openingBalance: number;
    oneYearProgress: number;
    currencySymbolId: number;
    currencySymbol: string;
    currencies: CurrencyTickerSelectModel[];
    onSave: (data: OtherInvestmentViewModel) => void;
}
