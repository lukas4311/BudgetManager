export class ProgressCalculatorService {
    public calculareProgress(startValue: number, endValue: number): number {
        if (startValue == endValue)
            return 0;

        if (startValue == 0)
            startValue = 1;

        return (endValue / startValue) * 100 - 100;
    }
}