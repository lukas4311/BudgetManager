import { TextField } from '@mui/material';
import * as React from 'react'

class DateRangeState {
    filterDateFrom: string;
    filterDateTo: string;
}

class DateRangeProps {
    datesFilledHandler: (dateFrom: string, dateTo: string) => void;
    className?: string;
}

export default class DateRangeComponent extends React.Component<DateRangeProps, DateRangeState> {
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
            <div className={`exactDates flex flex-row text-white ${this.props.className ?? ""}`}>
                <TextField
                    id="dateFrom"
                    label="Date from"
                    type="date"
                    size='small'
                    defaultValue={this.state.filterDateFrom}
                    InputLabelProps={{
                        shrink: true,
                    }}
                    className="w-full mr-4"
                    onChange={this.handleChangeDateFrom}
                />
                <TextField
                    id="dateTo"
                    label="Date to"
                    type="date"
                    size='small'
                    defaultValue={this.state.filterDateTo}
                    InputLabelProps={{
                        shrink: true,
                    }}
                    className="w-full mr-4"
                    onChange={this.handleChangeDateTo}
                />
            </div>
        );
    }
}