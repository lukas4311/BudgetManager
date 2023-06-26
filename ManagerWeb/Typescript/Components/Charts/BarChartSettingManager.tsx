import { BarSvgProps } from '@nivo/bar';

export class BarChartSettingManager {
    static getPaymentCategoryBarChartProps(): any {
        return {
            data: undefined,
            margin: { top: 20, right: 50, bottom: 120, left: 50 },
            indexBy: "key",
            colorBy: "indexValue",
            keys: ["value"],
            padding: 0.45,
            axisBottom: {
                tickRotation: 35,
            },
            isInteractive: false,
            theme: {
                axis: {
                    ticks: {
                        text: {
                            fill: "white"
                        }
                    }
                }
            },
        };
    }

    static getPaymentMonthlyGroupedBarChartProps(axisMin?: number, axisMax?: number): any {
        let config = {
            data: undefined,
            margin: { top: 20, right: 50, bottom: 50, left: 50 },
            indexBy: "key",
            keys: ['positive', 'negative'],
            colors: ['#007E04', '#920000'],
            padding: 0.55,
            axisBottom: {
                tickRotation: 35,
            },
            isInteractive: false,
            enableLabel: false,
            theme: {
                axis: {
                    ticks: {
                        text: {
                            fill: "white"
                        }
                    }
                }
            }
        };

        if (axisMin !== undefined)
            config["minValue"] = axisMin * 1.2;

        if (axisMax !== undefined)
            config["maxValue"] = axisMax * 1.2;

        return config;
    }
}
