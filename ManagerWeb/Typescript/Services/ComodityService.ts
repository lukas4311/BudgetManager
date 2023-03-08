import { ComodityApiInterface, ComodityTypeModel } from "../ApiClient/Main";

export default class ComodityService {
    comodityApi: ComodityApiInterface;

    constructor(comodityApi: ComodityApiInterface) {
        this.comodityApi = comodityApi;
    }

    public async GetComodityTypes() {
        const comodityTypes: ComodityTypeModel[] = await this.comodityApi.comoditiesComodityTypeAllGet();
        return comodityTypes.map(c => this.mapComodityTypeToViewModel(c))
    }

    private mapComodityTypeToViewModel(comodityType: ComodityTypeModel){
        let comodityTypeModel = new ComodityTypeViewModel();
        comodityType.code = comodityType.code;
        comodityType.comodityUnit = comodityType.comodityUnit;
        comodityType.comodityUnitId = comodityType.comodityUnitId;
        comodityType.id = comodityType.id;
        comodityType.name = comodityType.name;
        return comodityTypeModel;
    }
}

class ComodityTypeViewModel {
    id?: number | null;
    code?: string | null;
    name?: string | null;
    comodityUnitId?: number;
    comodityUnit?: string | null;
}