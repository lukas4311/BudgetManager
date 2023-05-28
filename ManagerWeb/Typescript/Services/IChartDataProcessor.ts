import { CalendarChartData } from "../Model/CalendarChartData";
import { LineChartData } from "../Model/LineChartData";
import { RadarChartData } from "../Model/RadarChartData";
import { BankBalanceModel, PaymentModel } from "../ApiClient/Main";
import { PieChartData } from "../Components/Charts/PieChart";

export interface IChartDataProcessor {
    prepareCalendarCharData(payments: PaymentModel[]): CalendarChartData[];
    prepareExpenseChartData(payments: PaymentModel[]): LineChartData[];
    prepareExpenseWithoutInvestmentsChartData(payments: PaymentModel[]): LineChartData[];
    prepareRevenuesChartData(payments: PaymentModel[]): LineChartData[];
    prepareBalanceChartData(payments: PaymentModel[], accountsBalance: BankBalanceModel[], selectedBankAccount: number): Promise<LineChartData[]>;
    prepareDataForRadarChart(payments: PaymentModel[]): RadarChartData[];
    prepareDataForPieChart(payments: PaymentModel[]): PieChartData[];
}
