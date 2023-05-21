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
}
