import * as React from 'react';
import { LineSvgProps } from '@nivo/line';
import { ScaleSpec } from '@nivo/scales'

export class LineChartSettingManager {
    static getPaymentChartSetting(): LineSvgProps {
        return this.getPaymentChartSettingWithScale(0, 0, 0, 0);
    }

    static getPaymentChartSettingWithScale(minValue: number, minPercentValueScale: number, maxValue: number, maxPercentValueScale: number): LineSvgProps {
        let yScaleSetting: ScaleSpec = { type: 'linear', reverse: false };

        if (minPercentValueScale != 0)
            yScaleSetting["min"] = minValue - (minValue * (minPercentValueScale / 100))

        if (maxPercentValueScale != 0)
            yScaleSetting["max"] = maxValue + (maxValue * (maxPercentValueScale / 100))

        return {
            data: undefined,
            xScale: {
                type: 'time',
                format: '%Y-%m-%d',
                useUTC: false,
                precision: 'day',
            },
            xFormat: "time:%Y-%m-%d",
            yScale: yScaleSetting,
            axisLeft: {
                legend: 'linear scale',
                legendOffset: 12,
                tickValues: 6,
                tickPadding: 15
            },
            axisBottom: {
                format: '%Y-%m-%d',
                tickValues: 'every 1 month',
                legend: 'time scale',
                legendOffset: -12,
            },
            colors: { scheme: 'set1' }, curve: 'linear', enablePoints: false, enablePointLabel: false,
            pointSize: 7, useMesh: true, enableArea: true, areaOpacity: 0.5, enableSlices: "y",
            sliceTooltip: ({ slice }) => {
                return (
                    <div style={{ background: 'black', padding: '9px 12px' }}>
                        {slice.points.map(point => (
                            <div key={point.id} style={{ color: 'white', padding: '3px 0' }}>
                                <span>{point.data.xFormatted}</span>
                                <span style={{ margin: '0px 8px' }}>{point.data.yFormatted}</span>
                            </div>
                        ))}
                    </div>
                );
            },
            theme: {
                axis: {
                    ticks: {
                        line: { stroke: "white" },
                        text: { fill: "white" }
                    }
                },
                grid: {
                    line: { stroke: "white" }
                },
                dots: {
                    text: { fill: 'white' }
                }
            }
        };
    }

    static getOtherInvestmentChartSetting(): LineSvgProps {
        return {
            data: undefined, enableArea: true, areaOpacity: 0.50, isInteractive: true, useMesh: true, enablePoints: true,
            colors: { scheme: 'set1' }, enableSlices: "y", enableCrosshair: false, enableGridX: false, enableGridY: false,
            pointSize: 8, pointBorderWidth: 1, pointLabelYOffset: -12, pointColor: "black",
            sliceTooltip: ({ slice }) => {
                return (
                    <div style={{ background: 'black', padding: '9px 12px' }}>
                        {slice.points.map(point => (
                            <div key={point.id} style={{ color: 'white', padding: '3px 0' }}>
                                <span>{point.data.xFormatted}</span>
                                <span style={{ margin: '0px 8px' }}>{point.data.yFormatted}</span>
                            </div>
                        ))}
                    </div>
                );
            }
        }
    }

    static getOtherInvestmentSummarySetting(min: number, max: number): LineSvgProps {
        let yScaleSetting: ScaleSpec = { type: 'linear' };
        const calculatedMin = min - (min * (10 / 100));
        yScaleSetting["min"] = calculatedMin;
        yScaleSetting["max"] = max + (max * (10 / 100));

        let setting = this.getOtherInvestmentChartSetting();
        setting.areaBaselineValue = calculatedMin;
        setting.enableGridX = setting.enableGridY = true;
        setting.xScale = {
            type: "time",
            format: "%Y-%m-%d %H:%M",
            precision: "day"
        };
        setting.yScale = yScaleSetting;
        setting.xFormat = "time:%Y-%m-%d"
        setting.axisBottom = {
            format: "%Y-%m-%d",
        }
        setting.enableSlices = 'x';
        setting.theme = {
            axis: {
                ticks: {
                    line: { stroke: "white" },
                    text: { fill: "white" }
                }
            },
            grid: {
                line: { stroke: "white" }
            }
        }

        return setting;
    }
}
