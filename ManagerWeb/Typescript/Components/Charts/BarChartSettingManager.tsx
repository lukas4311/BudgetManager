import { BarSvgProps } from '@nivo/bar';

export class BarChartSettingManager {
    static getPaymentCategoryBarChartProps(): BarSvgProps<any> {
        return {
            data: undefined,
            width: 600,
            height: 400,
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
