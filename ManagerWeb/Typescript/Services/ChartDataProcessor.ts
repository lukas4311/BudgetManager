import { CalendarChartData } from "../Model/CalendarChartData";
import { IPaymentInfo } from "../Model/IPaymentInfo";
import moment from "moment";
import { LineChartData } from "../Model/LineChartData";
import { IBankAccountBalanceResponseModel } from "../Model/IBankAccountBalanceResponseModel";
import DataLoader from "./DataLoader";
import { RadarChartData } from "../Model/RadarChartData";
import { BankBalanceModel, PaymentModel } from "../ApiClient/Main";
import _ from "lodash";

export class ChartDataProcessor {
    dataLoader: DataLoader;

    constructor() {
        this.dataLoader = new DataLoader();
    }

    public prepareCalendarCharData(payments: Array<PaymentModel>): CalendarChartData[] {
        let calendarChartData: CalendarChartData[] = [];

        payments.filter(p => p.paymentTypeCode == "Expense").forEach(payment => {
            let paymentDay = calendarChartData.find(p => p.day == moment(payment.date).format("YYYY-MM-DD"));

            if (paymentDay) {
                paymentDay.value += payment.amount;
            } else {
                let data = new CalendarChartData();
                data.day = moment(payment.date).format("YYYY-MM-DD");
                data.value = payment.amount;
                calendarChartData.push(data);
            }
        });

        return calendarChartData;
    }

    public prepareExpenseChartData(payments: Array<PaymentModel>): LineChartData[] {
        let filteredPayments = payments.filter(a => a.paymentTypeCode == 'Expense');
        return this.mapPaymentsToLinearChartStructure(filteredPayments);
    }

    public prepareExpenseWithoutInvestmentsChartData(payments: Array<PaymentModel>): LineChartData[] {
        let filteredPayments = payments.filter(a => a.paymentTypeCode == 'Expense' &&
            a.paymentCategoryCode != "Invetsment");
        return this.mapPaymentsToLinearChartStructure(filteredPayments);
    }

    public prepareRevenuesChartData(payments: Array<PaymentModel>): LineChartData[] {
        let filteredPayments = payments.filter(a => a.paymentTypeCode == 'Revenue');
        return this.mapPaymentsToLinearChartStructure(filteredPayments);
    }

    private mapPaymentsToLinearChartStructure(payments: Array<PaymentModel>): LineChartData[] {
        let paymentsSum = 0;
        let mappedPayments: LineChartData[] = [];

        const groupedData = _.chain(payments)
            .groupBy((model) => moment(model.date).format("YYYY-MM-DD"))
            .reduce((result: GroupedCumulativeData[], models, dateStr) => {
                const totalValue = _.sumBy(models, 'amount');
                const lastCumulativeSum = _.last(result)?.cumulativeSum || 0;
                const cumulativeSum = lastCumulativeSum + totalValue;
                result.push({ paymentDate: dateStr, totalValue, cumulativeSum });
                return result;
            }, [])
            .value()

        groupedData
            .sort((a, b) => moment(a.paymentDate).format("YYYY-MM-DD") > moment(b.paymentDate).format("YYYY-MM-DD") ? 1 : -1)
            .forEach(a => {
                paymentsSum += a.cumulativeSum;
                mappedPayments.push({ x: moment(a.paymentDate).format("YYYY-MM-DD"), y: paymentsSum });
            });

        return mappedPayments;
    }

    public async prepareBalanceChartData(payments: Array<PaymentModel>, accountsBalance: BankBalanceModel[], selectedBankAccount: number): Promise<LineChartData[]> {
        let balance: number = 0;

        if (selectedBankAccount != undefined && selectedBankAccount != null) {
            const bankInfo = accountsBalance.filter(b => b.id == selectedBankAccount)[0];

            if (bankInfo != undefined)
                balance = bankInfo.openingBalance + bankInfo.balance;
        } else {
            accountsBalance.forEach(v => balance += v.openingBalance + v.balance);
        }

        let paymentChartData: LineChartData[] = [];
        payments
            .sort((a, b) => moment(a.date).format("YYYY-MM-DD") > moment(b.date).format("YYYY-MM-DD") ? 1 : -1)
            .forEach(a => {
                balance += a.amount * (a.paymentTypeCode == 'Revenue' ? 1 : -1);
                paymentChartData.push({ x: moment(a.date).format("YYYY-MM-DD"), y: balance });
            });

        return paymentChartData;
    }

    public prepareDataForRadarChart(payments: Array<PaymentModel>): RadarChartData[] {
        let categoryGroups: RadarChartData[] = [];
        payments.filter(a => a.paymentTypeCode == 'Expense').reduce(function (res, val) {
            if (!res[val.paymentCategoryCode]) {
                res[val.paymentCategoryCode] = { key: val.paymentCategoryCode, value: 0 };
                categoryGroups.push(res[val.paymentCategoryCode]);
            }
            res[val.paymentCategoryCode].value += val.amount;
            return res;
        }, {});
        categoryGroups = _.orderBy(categoryGroups, a => a.value, ['desc'])
        return categoryGroups;
    }
}

class GroupedData {
    paymentDate: string;
    totalValue: number;
}

class GroupedCumulativeData {
    paymentDate: string;
    totalValue: number;
    cumulativeSum: number;
}