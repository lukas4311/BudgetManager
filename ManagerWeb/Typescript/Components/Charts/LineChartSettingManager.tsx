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
            pointSize: 7, useMesh: true, enableArea: false, areaOpacity: 0.5, enableSlices: false, isInteractive: false,
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

    static getStockChartSetting(): LineSvgProps {
        return {
            data: undefined, enableArea: false, isInteractive: false, useMesh: true, enablePoints: false,
            colors: { scheme: 'set1' }, enableSlices: "y", enableCrosshair: false, enableGridX: false, enableGridY: false,
            axisLeft: null, axisBottom: null, axisRight: null, axisTop: null, margin: { bottom: 0, left: 0, right: 0, top: 0 },
            yScale: { type: 'linear', min: 'auto', max: 'auto', reverse: false }
        }
    }

    static getNetWorthChartSettingForCompanyInfo(): LineSvgProps {
        return {
            data: undefined, enableArea: false, isInteractive: true, useMesh: true, enablePoints: false,
            colors: { scheme: 'set1' }, enableSlices: 'x', enableCrosshair: true, enableGridX: true, enableGridY: true,
            margin: { bottom: 40, left: 60, right: 10, top: 10 },
            axisLeft: { legend: 'linear scale', legendOffset: 5 },
            xScale: {
                type: 'time',
                format: '%Y-%m-%d',
                precision: 'month',
            },
            xFormat: "time:%Y-%m",
            axisBottom: {
                format: '%Y-%m',
                tickValues: 'every 3 months',
                legend: 'time scale',
                legendOffset: -12,
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
                }
            },
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

    static getStockChartSettingForCompanyInfo(): LineSvgProps {
        return {
            data: undefined, enableArea: false, isInteractive: false, useMesh: true, enablePoints: false,
            colors: { scheme: 'set1' }, enableSlices: "y", enableCrosshair: false, enableGridX: false, enableGridY: false,
            margin: { bottom: 40, left: 40, right: 10, top: 10 },
            axisLeft: { legend: 'linear scale', legendOffset: 5 },
            axisBottom: { format: '%Y', tickValues: 'every year', legend: 'time scale', legendOffset: -5, },
            axisRight: null,
            axisTop: null,
            yScale: { type: 'linear', min: 'auto', max: 'auto', reverse: false },
            xScale: { type: "time", format: "%Y-%m-%d", precision: "day" }, xFormat: "time:%Y-%m-%d",
            theme: {
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
        }
    }

    static getStockChartSettingForStockValueHistory(min: number, max: number): LineSvgProps {
        let yScaleSetting: ScaleSpec = { type: 'linear' };
        yScaleSetting["min"] = 0;
        yScaleSetting["max"] = max + (max * (10 / 100));

        return {
            data: undefined, enableArea: false, isInteractive: false, useMesh: true, enablePoints: false,
            colors: { scheme: 'set1' }, enableSlices: "y", enableCrosshair: false, enableGridX: false, enableGridY: true,
            margin: { bottom: 40, left: 40, right: 10, top: 10 },
            axisLeft: { legend: 'linear scale', legendOffset: 4, tickRotation: 35, tickPadding: 4 },
            axisBottom: { format: '%Y-%m', tickValues: 'every month', legend: 'time scale', legendOffset: -5, },
            axisRight: null,
            axisTop: null,
            yScale: yScaleSetting,
            xScale: { type: "time", format: "%Y-%m-%d", precision: "day" }, xFormat: "time:%Y-%m-%d",
            theme: {
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
