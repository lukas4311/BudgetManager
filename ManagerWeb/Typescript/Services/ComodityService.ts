import moment from "moment";
import { ComodityApiInterface, ComodityTradeHistoryModel, ComodityTypeModel } from "../ApiClient/Main";
import { ComoditiesFormViewModel } from "../Components/Comodities/ComoditiesForm";
import { IComodityService } from "./IComodityService";
import _ from "lodash";
import { ComodityEndpointsApiInterface } from "../ApiClient/Fin";

const ounce = 28.34;
const goldCode = 'AU';
const czkSymbol = 'CZK';

export default class ComodityService implements IComodityService {
    comodityApi: ComodityApiInterface;
    comodityFinApi: ComodityEndpointsApiInterface;

    constructor(comodityApi: ComodityApiInterface, comodityFinApi: ComodityEndpointsApiInterface) {
        this.comodityApi = comodityApi;
        this.comodityFinApi = comodityFinApi;
    }

    public async getComodityTypes(): Promise<ComodityTypeModel[]> {
        const comodityTypes: ComodityTypeModel[] = await this.comodityApi.v1ComoditiesComodityTypeAllGet();
        return comodityTypes.map(c => this.mapComodityTypeToViewModel(c))
    }

    public async getAllComodityTrades(comodityId: number): Promise<ComoditiesFormViewModel[]> {
        const comodities = await this.comodityApi.v1ComoditiesAllGet();
        return comodities.filter(a => a.comodityTypeId == comodityId).map(g => this.mapDataModelToViewModel(g))
    }

    public async getGoldPriceInCurrency(currencyCode: string) {
        const currentPrice = await this.comodityFinApi.getCurrentGoldPriceForOunceForSpecificCurrency({ currencyCode: currencyCode });
        return currentPrice;
    }

    public async createComodityTrade(tradeModel: ComodityTradeHistoryModel) {
        await this.comodityApi.v1ComoditiesPost({ comodityTradeHistoryModel: tradeModel });
    }

    public async updateComodityTrade(tradeModel: ComodityTradeHistoryModel) {
        await this.comodityApi.v1ComoditiesPut({ comodityTradeHistoryModel: tradeModel });
    }

    public async deleteComodityTrade(comodityId: number) {
        await this.comodityApi.v1ComoditiesDelete({ body: comodityId });
    }

    public async getComodityNetWorth(): Promise<number> {
        let netWorth = 0;
        const allComodityTypes = await this.getComodityTypes();

        for (const comodityType of allComodityTypes.filter(c => c.code == goldCode)) {
            const tradeData = await this.getAllComodityTrades(comodityType.id);

            const price = await this.getGoldPriceInCurrency(czkSymbol);
            const totalWeight = _.sumBy(tradeData ?? [], (g) => g.comodityAmount);
            const actualTotalPrice = totalWeight * price / ounce;
            netWorth += actualTotalPrice;
        }

        return netWorth;
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