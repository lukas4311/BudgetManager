import { CalendarChartData } from "../Model/CalendarChartData";
import { IPaymentInfo } from "../Model/IPaymentInfo";
import moment from "moment";
import { LineChartData } from "../Model/LineChartData";
import { IBankAccountBalanceResponseModel } from "../Model/IBankAccountBalanceResponseModel";
import DataLoader from "./DataLoader";
import { RadarChartData } from "../Model/RadarChartData";
import { BankBalanceModel, PaymentModel } from "../ApiClient/Main";
import _ from "lodash";

export class ChartDataProcessor{
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
        let expenseSum = 0;
        let expenses: LineChartData[] = [];
        payments.filter(a => a.paymentTypeCode == 'Expense')
            .sort((a, b) => moment(a.date).format("YYYY-MM-DD") > moment(b.date).format("YYYY-MM-DD") ? 1 : -1)
            .forEach(a => {
                expenseSum += a.amount;
                expenses.push({ x: moment(a.date).format("YYYY-MM-DD"), y: expenseSum });
            });

        return expenses;
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

