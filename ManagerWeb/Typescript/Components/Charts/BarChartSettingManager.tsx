import { BarSvgProps } from '@nivo/bar';

export class BarChartSettingManager {
    static getPaymentCategoryBarChartProps(): any {
        return {
            data: undefined,
            margin: { top: 60, right: 80, bottom: 60, left: 80 },
            indexBy: "category",
            keys: ["amount"],
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
    }
}
