import moment from "moment";
import { OtherInvestmentApiInterface, OtherInvestmentBalaceHistoryModel, OtherInvestmentBalanceSummaryModel, OtherInvestmentModel, OtherInvestmentTagModel, PaymentModel } from "../ApiClient/Main";
import { OtherInvestmentBalaceHistoryViewModel } from "../Components/OtherInvestment/OtherInvestmentDetail";
import OtherInvestmentViewModel from "../Model/OtherInvestmentViewModel";
import { IOtherInvestmentService } from "./IOtherInvestmentService";
import _, { forEach } from "lodash";
import { NetWorthMonthGroupModel } from "./NetWorthService";

export default class OtherInvestmentService implements IOtherInvestmentService {
    private otherInvestmentApi: OtherInvestmentApiInterface;

    constructor(otherInvestmentApi: OtherInvestmentApiInterface) {
        this.otherInvestmentApi = otherInvestmentApi;
    }

    public async getBalanceHistory(id: number): Promise<OtherInvestmentBalaceHistoryViewModel[]> {
        const balanceHistory = await this.otherInvestmentApi.v1OtherInvestmentOtherInvestmentIdBalanceHistoryGet({ otherInvestmentId: id });
        let viewModels: OtherInvestmentBalaceHistoryViewModel[] = balanceHistory.map(d => this.mapBalanceHistoryDataModelToViewModel(d));
        return viewModels;
    }

    public async getTagConnectedWithInvetment(id: number): Promise<OtherInvestmentTagModel> {
        const linkedTag = await this.otherInvestmentApi.v1OtherInvestmentIdLinkedTagGet({ id: id });
        return linkedTag;
    }

    public async getPaymentLinkedToTagOfOtherInvestment(otherinvestmentid: number, tagId: number): Promise<PaymentModel[]> {
        let linkedPayments: PaymentModel[] = await this.otherInvestmentApi.v1OtherInvestmentIdTagedPaymentsTagIdGet({ id: otherinvestmentid, tagId: tagId });
        return linkedPayments;
    }

    public async getYearToYearProfit(id: number, years: number): Promise<number> {
        let otherInvestmentProfit = await this.otherInvestmentApi.v1OtherInvestmentIdProfitOverYearsYearsGet({ id: id, years: years });
        return otherInvestmentProfit;
    }

    public async getOverallProfit(id: number): Promise<number> {
        let overallProfit = await this.otherInvestmentApi.v1OtherInvestmentIdProfitOverallGet({ id: id });
        return overallProfit;
    }

    public async getSummary(): Promise<OtherInvestmentBalanceSummaryModel> {
        let summary = await this.otherInvestmentApi.v1OtherInvestmentSummaryGet();
        return summary;
    }

    public async getAll(): Promise<OtherInvestmentViewModel[]> {
        const otherInvestments = await this.otherInvestmentApi.v1OtherInvestmentAllGet();
        let viewModels: OtherInvestmentViewModel[] = otherInvestments.map(d => this.mapOtherInvetsmentDataModelToViewModel(d));
        return viewModels;
    }

    public async getMonthlyGroupedAccumulatedPayments(fromDate: Date, toDate: Date, otherInvestments: OtherInvestmentViewModel[]): Promise<NetWorthMonthGroupModel[]> {
        const months = this.getMonthsBetween(fromDate, toDate);
        const monthSummary: Map<string, number> = new Map<string, number>();
        forEach(months, m => monthSummary.set(m.date, 0));

        for (const otherInvestment of otherInvestments) {
            const baseLine = otherInvestment.openingBalance;
            const balanceHistory = await this.getBalanceHistory(otherInvestment.id);
            const orderedBalanceHistory = _.orderBy(balanceHistory, d => d.date, ['asc']);

            for (const month of months) {
                const balance = _.last(orderedBalanceHistory.filter(b => moment(b.date) < moment(month.date + "-1")))?.balance ?? 0;
                monthSummary.set(month.date, monthSummary.get(month.date) + balance + baseLine);
            }
        }

        const finalGroupedModel: NetWorthMonthGroupModel[] = Array.from(monthSummary.entries()).map(([key, value]) => ({ date: moment(key + "-1"), amount: value }));
        return finalGroupedModel;
    }

    private getMonthsBetween(fromDate: Date, toDate: Date) {
        const start = moment(fromDate);
        const end = moment(toDate);
        const months = [];

        while (start.isBefore(end)) {
            months.push({ date: start.format('YYYY-MM') });
            start.add(1, 'month');
        }

        return months;
    }

    public async updateOtherInvestmentBalanceHistory(otherInvestmentModel: OtherInvestmentBalaceHistoryModel): Promise<void> {
        await this.otherInvestmentApi.v1OtherInvestmentBalanceHistoryPut({ otherInvestmentBalaceHistoryModel: otherInvestmentModel });
    }

    public async updateOtherInvestment(otherInvestment: OtherInvestmentModel): Promise<void> {
        await this.otherInvestmentApi.v1OtherInvestmentPut({ otherInvestmentModel: otherInvestment });
    }

    public async createOtherInvestmentBalanceHistory(otherInvestmentId: number, otherInvestmentModel: OtherInvestmentBalaceHistoryModel): Promise<void> {
        await this.otherInvestmentApi.v1OtherInvestmentOtherInvestmentIdBalanceHistoryPost({ otherInvestmentId: otherInvestmentId, otherInvestmentBalaceHistoryModel: otherInvestmentModel });
    }

    public async createOtherInvestment(otherInvestment: OtherInvestmentModel): Promise<void> {
        await this.otherInvestmentApi.v1OtherInvestmentPost({ otherInvestmentModel: otherInvestment });
    }

    public async createConnectionWithPaymentTag(otherInvestmentId: number, tagId: number): Promise<void> {
        await this.otherInvestmentApi.v1OtherInvestmentIdTagedPaymentsTagIdPost({ tagId, id: otherInvestmentId });
    }

    public async deleteOtherInvestment(id: number): Promise<void> {
        await this.otherInvestmentApi.v1OtherInvestmentDelete({ body: id });
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