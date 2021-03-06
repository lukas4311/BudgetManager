import * as React from 'react'

class DateRangeState {
    filterDateFrom: string;
    filterDateTo: string;
}

class DateRangeProps {
    datesFilledHandler: (dateFrom: string, dateTo: string) => void;
}

export default class DateRangeComponent extends React.Component<DateRangeProps, DateRangeState>{
    constructor(props: DateRangeProps) {
        super(props);
        this.state = { filterDateFrom: '', filterDateTo: '' };
    }

    private handleChangeDateFrom = (e: React.ChangeEvent<HTMLInputElement>): void => {
        this.setState({ filterDateFrom: e.target.value }, this.findByExplicitDataIfSet);
    }

    private handleChangeDateTo = (e: React.ChangeEvent<HTMLInputElement>): void => {
        this.setState({ filterDateTo: e.target.value }, this.findByExplicitDataIfSet);
    }

    private findByExplicitDataIfSet = (): void => {
        if (this.state.filterDateFrom != '' && this.state.filterDateTo != '') {
            this.props.datesFilledHandler(this.state.filterDateFrom, this.state.filterDateTo);
        }
    }

    render() {
        return (
            <div className="exactDates w-full flex flex-row text-white">
                <input type="date" className="effect-11 w-full mr-4 h-8" placeholder="Datum od"
                    value={this.state.filterDateFrom} onChange={this.handleChangeDateFrom}></input>
                <input type="date" className="effect-11 w-full h-8" placeholder="Datum do"
                    value={this.state.filterDateTo} onChange={this.handleChangeDateTo}></input>
            </div>
        );
    }
}