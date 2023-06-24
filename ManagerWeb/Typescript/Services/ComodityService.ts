import moment from "moment";
import { ComodityApiInterface, ComodityTradeHistoryModel, ComodityTypeModel } from "../ApiClient/Main";
import { ComoditiesFormViewModel } from "../Components/Comodities/ComoditiesForm";
import { IComodityService } from "./IComodityService";

export default class ComodityService implements IComodityService {
    comodityApi: ComodityApiInterface;

    constructor(comodityApi: ComodityApiInterface) {
        this.comodityApi = comodityApi;
    }

    public async getComodityTypes() {
        const comodityTypes: ComodityTypeModel[] = await this.comodityApi.comoditiesComodityTypeAllGet();
        return comodityTypes.map(c => this.mapComodityTypeToViewModel(c))
    }

    public async getAllComodityTrades(comodityId: number) {
        const comodities = await this.comodityApi.comoditiesAllGet();
        const allComodityTradeData = comodities.filter(a => a.comodityTypeId == comodityId).map(g => this.mapDataModelToViewModel(g))
        return allComodityTradeData;
    }

    public async getGoldPriceInCurrency(currencyCode: string) {
        const currentPrice = await this.comodityApi.comoditiesGoldActualPriceCurrencyCodeGet({ currencyCode: currencyCode });
        return currentPrice;
    }

    public async createComodityTrade(tradeModel: ComodityTradeHistoryModel) {
        await this.comodityApi.comoditiesPost({ comodityTradeHistoryModel: tradeModel });
    }

    public async updateComodityTrade(tradeModel: ComodityTradeHistoryModel) {
        await this.comodityApi.comoditiesPut({ comodityTradeHistoryModel: tradeModel });
    }

    public async deleteComodityTrade(comodityId: number) {
        await this.comodityApi.comoditiesDelete({ body: comodityId });
    }

    private mapComodityTypeToViewModel(comodityType: ComodityTypeModel) {
        let comodityTypeModel = new ComodityTypeViewModel();
        comodityTypeModel.code = comodityType.code;
        comodityTypeModel.comodityUnit = comodityType.comodityUnit;
        comodityTypeModel.comodityUnitId = comodityType.comodityUnitId;
        comodityTypeModel.id = comodityType.id;
        comodityTypeModel.name = comodityType.name;
        return comodityTypeModel;
    }

    private mapDataModelToViewModel = (tradeHistory: ComodityTradeHistoryModel): ComoditiesFormViewModel => {
        let model: ComoditiesFormViewModel = new ComoditiesFormViewModel();
        model.currencySymbol = tradeHistory.currencySymbol;
        model.currencySymbolId = tradeHistory.currencySymbolId;
        model.id = tradeHistory.id;
        model.price = tradeHistory.tradeValue;
        model.buyTimeStamp = moment(tradeHistory.tradeTimeStamp).format("YYYY-MM-DD");
        model.comodityAmount = tradeHistory.tradeSize;
        model.company = tradeHistory.company;
        return model;
    }
}

export class ComodityTypeViewModel {
    id?: number | null;
    code?: string | null;
    name?: string | null;
    comodityUnitId?: number;
    comodityUnit?: string | null;
}