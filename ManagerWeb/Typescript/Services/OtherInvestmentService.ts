import moment from "moment";
import { OtherInvestmentApiInterface, OtherInvestmentBalaceHistoryModel, OtherInvestmentModel, OtherInvestmentTagModel, PaymentModel } from "../ApiClient/Main";
import { OtherInvestmentBalaceHistoryViewModel } from "../Components/OtherInvestment/OtherInvestmentDetail";
import OtherInvestmentViewModel from "../Model/OtherInvestmentViewModel";

export default class OtherInvestmentService {
    private otherInvestmentApi: OtherInvestmentApiInterface;

    constructor(otherInvestmentApi: OtherInvestmentApiInterface) {
        this.otherInvestmentApi = otherInvestmentApi;
    }

    public async getBalanceHistory(id: number) {
        const balanceHistory = await this.otherInvestmentApi.otherInvestmentOtherInvestmentIdBalanceHistoryGet({ otherInvestmentId: id });
        let viewModels: OtherInvestmentBalaceHistoryViewModel[] = balanceHistory.map(d => this.mapBalanceHistoryDataModelToViewModel(d));
        return viewModels;
    }

    public async getTagConnectedWithInvetment(id: number): Promise<OtherInvestmentTagModel> {
        const linkedTag = await this.otherInvestmentApi.otherInvestmentIdLinkedTagGet({ id: id });
        return linkedTag;
    }

    public async getPaymentLinkedToTagOfOtherInvestment(otherinvestmentid: number, tagId: number): Promise<PaymentModel[]> {
        let linkedPayments: PaymentModel[] = await this.otherInvestmentApi.otherInvestmentIdTagedPaymentsTagIdGet({ id: otherinvestmentid, tagId: tagId });
        return linkedPayments;
    }

    public async getYearToYearProfit(id: number, years: number) {
        let otherInvestmentProfit = await this.otherInvestmentApi.otherInvestmentIdProfitOverYearsYearsGet({ id: id, years: years });
        return otherInvestmentProfit;
    }

    public async getOverallProfit(id: number) {
        let overallProfit = await this.otherInvestmentApi.otherInvestmentIdProfitOverallGet({ id: id });
        return overallProfit;
    }

    public async getSummary() {
        let summary = await this.otherInvestmentApi.otherInvestmentSummaryGet();
        return summary;
    }

    public async getAll() {
        const otherInvestments = await this.otherInvestmentApi.otherInvestmentAllGet();
        let viewModels: OtherInvestmentViewModel[] = otherInvestments.map(d => this.mapOtherInvetsmentDataModelToViewModel(d));
        return viewModels;
    }

    public async updateOtherInvestmentBalanceHistory(otherInvestmentModel: OtherInvestmentBalaceHistoryModel) {
        await this.otherInvestmentApi.balanceHistoryPut({ otherInvestmentBalaceHistoryModel: otherInvestmentModel });
    }

    public async updateOtherInvestment(otherInvestment: OtherInvestmentModel) {
        await this.otherInvestmentApi.otherInvestmentPut({ otherInvestmentModel: otherInvestment });
    }

    public async createOtherInvestmentBalanceHistory(otherInvestmentId: number, otherInvestmentModel: OtherInvestmentBalaceHistoryModel) {
        await this.otherInvestmentApi.otherInvestmentOtherInvestmentIdBalanceHistoryPost({ otherInvestmentId: otherInvestmentId, otherInvestmentBalaceHistoryModel: otherInvestmentModel });
    }

    public async createOtherInvestment(otherInvestment: OtherInvestmentModel) {
        await this.otherInvestmentApi.otherInvestmentPost({ otherInvestmentModel: otherInvestment });
    }

    public async createConnectionWithPaymentTag(otherInvestmentId: number, tagId: number) {
        await this.otherInvestmentApi.otherInvestmentIdTagedPaymentsTagIdPost({ tagId, id: otherInvestmentId });
    }

    public async deleteOtherInvestment(id: number) {
        await this.otherInvestmentApi.otherInvestmentDelete({ body: id });
    }

    private mapBalanceHistoryDataModelToViewModel = (otherInvestmentBalance: OtherInvestmentBalaceHistoryModel): OtherInvestmentBalaceHistoryViewModel => {
        let model: OtherInvestmentBalaceHistoryViewModel = new OtherInvestmentBalaceHistoryViewModel();
        model.id = otherInvestmentBalance.id;
        model.date = moment(otherInvestmentBalance.date).format("YYYY-MM-DD");
        model.balance = otherInvestmentBalance.balance;
        model.otherInvestmentId = otherInvestmentBalance.otherInvestmentId;
        return model;
    }

    private mapOtherInvetsmentDataModelToViewModel = (otherInvestment: OtherInvestmentModel): OtherInvestmentViewModel => {
        let model: OtherInvestmentViewModel = new OtherInvestmentViewModel();
        model.currencySymbolId = otherInvestment.currencySymbolId;
        model.id = otherInvestment.id;
        model.created = moment(otherInvestment.created).format("YYYY-MM-DD");
        model.name = otherInvestment.name;
        model.code = otherInvestment.code;
        model.openingBalance = otherInvestment.openingBalance;
        return model;
    }
}