import { OtherInvestmentBalaceHistoryModel, OtherInvestmentBalanceSummaryModel, OtherInvestmentModel, OtherInvestmentTagModel, PaymentModel } from "../ApiClient/Main";
import { OtherInvestmentBalaceHistoryViewModel } from "../Components/OtherInvestment/OtherInvestmentDetail";
import OtherInvestmentViewModel from "../Model/OtherInvestmentViewModel";


export interface IOtherInvestmentService {
    getBalanceHistory(id: number): Promise<OtherInvestmentBalaceHistoryViewModel[]>;
    getTagConnectedWithInvetment(id: number): Promise<OtherInvestmentTagModel>;
    getPaymentLinkedToTagOfOtherInvestment(otherinvestmentid: number, tagId: number): Promise<PaymentModel[]>;
    getYearToYearProfit(id: number, years: number): Promise<number>;
    getOverallProfit(id: number): Promise<number>;
    getSummary(): Promise<OtherInvestmentBalanceSummaryModel>;
    getAll(): Promise<OtherInvestmentViewModel[]>;
    updateOtherInvestmentBalanceHistory(otherInvestmentModel: OtherInvestmentBalaceHistoryModel): Promise<void>;
    updateOtherInvestment(otherInvestment: OtherInvestmentModel): Promise<void>;
    createOtherInvestmentBalanceHistory(otherInvestmentId: number, otherInvestmentModel: OtherInvestmentBalaceHistoryModel): Promise<void>;
    createOtherInvestment(otherInvestment: OtherInvestmentModel): Promise<void>;
    createConnectionWithPaymentTag(otherInvestmentId: number, tagId: number): Promise<void>;
    deleteOtherInvestment(id: number): Promise<void>;
}
