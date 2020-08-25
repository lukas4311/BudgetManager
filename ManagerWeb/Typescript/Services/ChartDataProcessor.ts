import { CalendarChartData } from "../Model/CalendarChartData";
import { IPaymentInfo } from "../Model/IPaymentInfo";
import moment from "moment";
import { LineChartData } from "../Model/LineChartData";
import { IBankAccountBalanceResponseModel } from "../Model/IBankAccountBalanceResponseModel";
import DataLoader from "../DataLoader";
import { RadarChartData } from "../Model/RadarChartData";

export class ChartDataProcessor{
    dataLoader: DataLoader;

    constructor() {
        this.dataLoader = new DataLoader();
    }

    public prepareCalendarCharData(payments: Array<IPaymentInfo>): CalendarChartData[] {
        let calendarChartData: CalendarChartData[] = [];

        payments.filter(p => p.paymentTypeCode == "Expense").forEach(payment => {
            let paymentDay = calendarChartData.find(p => p.day == payment.date);

            if (paymentDay) {
                paymentDay.value += payment.amount;
            } else {
                let data = new CalendarChartData();
                data.day = payment.date;
                data.value = payment.amount;
                calendarChartData.push(data);
            }
        });

        return calendarChartData;
    }

    public prepareExpenseChartData(payments: Array<IPaymentInfo>): LineChartData[] {
        let expenseSum = 0;
        let expenses: LineChartData[] = [];
        payments.filter(a => a.paymentTypeCode == 'Expense')
            .sort((a, b) => moment(a.date).format("YYYY-MM-DD") > moment(b.date).format("YYYY-MM-DD") ? 1 : -1)
            .forEach(a => {
                expenseSum += a.amount;
                expenses.push({ x: a.date, y: expenseSum });
            });

        return expenses;
    }

    public async prepareBalanceChartData(payments: Array<IPaymentInfo>, accountBalance: IBankAccountBalanceResponseModel, selectedBankAccount: number): Promise<LineChartData[]> {
        let balance: number = 0;

        if (selectedBankAccount != undefined && selectedBankAccount != null) {
            const bankInfo = accountBalance.bankAccountsBalance.filter(b => b.id == selectedBankAccount)[0];

            if (bankInfo != undefined)
                balance = bankInfo.openingBalance + bankInfo.balance;
        } else {
            accountBalance.bankAccountsBalance.forEach(v => balance += v.openingBalance + v.balance);
        }

        let paymentChartData: LineChartData[] = [];
        payments
            .sort((a, b) => moment(a.date).format("YYYY-MM-DD") > moment(b.date).format("YYYY-MM-DD") ? 1 : -1)
            .forEach(a => {
                balance += a.amount * (a.paymentTypeCode == 'Revenue' ? 1 : -1);
                paymentChartData.push({ x: a.date, y: balance });
            });

        return paymentChartData;
    }

    public prepareDataForRadarChart(payments: Array<IPaymentInfo>): RadarChartData[] {
        let categoryGroups: RadarChartData[] = [];
        payments.filter(a => a.paymentTypeCode == 'Expense').reduce(function (res, val) {
            if (!res[val.paymentCategoryCode]) {
                res[val.paymentCategoryCode] = { key: val.paymentCategoryCode, value: 0 };
                categoryGroups.push(res[val.paymentCategoryCode]);
            }
            res[val.paymentCategoryCode].value += val.amount;
            return res;
        }, {});

        return categoryGroups;
    }
}

