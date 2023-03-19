import moment from "moment";
import { OtherInvestmentApiInterface, OtherInvestmentBalaceHistoryModel } from "../ApiClient/Main";
import { OtherInvestmentBalaceHistoryViewModel } from "../Components/OtherInvestment/OtherInvestmentDetail";

export default class CryptoService {
    private otherInvestmentApi: OtherInvestmentApiInterface;

    constructor(otherInvestmentApi: OtherInvestmentApiInterface) {
        this.otherInvestmentApi = otherInvestmentApi;
    }

    public getBalanceHistory = async (id: number) => {
        const balanceHistory = await this.otherInvestmentApi.otherInvestmentOtherInvestmentIdBalanceHistoryGet({ otherInvestmentId: id });
        let viewModels: OtherInvestmentBalaceHistoryViewModel[] = balanceHistory.map(d => this.mapDataModelToViewModel(d));
        return viewModels;
    }

    private mapDataModelToViewModel = (otherInvestmentBalance: OtherInvestmentBalaceHistoryModel): OtherInvestmentBalaceHistoryViewModel => {
        let model: OtherInvestmentBalaceHistoryViewModel = new OtherInvestmentBalaceHistoryViewModel();
        model.id = otherInvestmentBalance.id;
        model.date = moment(otherInvestmentBalance.date).format("YYYY-MM-DD");
        model.balance = otherInvestmentBalance.balance;
        model.otherInvestmentId = otherInvestmentBalance.otherInvestmentId;
        // model.onSave = this.saveBalance;
        return model;
    }
}