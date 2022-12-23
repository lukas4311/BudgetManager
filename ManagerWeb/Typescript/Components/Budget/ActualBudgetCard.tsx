import moment from 'moment';
import * as React from 'react'

class ActualBudgetCardProps {
    from: Date;
    to?: Date;
    spent: number;
    limit: number;
    name: string;
}

export default function ActualBudgetCard(props: ActualBudgetCardProps) {
    return (
        <div className="w-full bg-blueSapphire mt-2 budgetCard">
            <div className="flex">
                <span className="text-xs font-semibold">{moment(props.from).format("YYYY.MM.DD")}</span>
                <span className="text-sm ml-auto">{props.limit}</span>
            </div>
            <div className="w-full h-4 relative mt-2 bg-battleshipGrey shadow-lg mb-4">
                <div className="w-1/5 bg-vermilion h-full"></div>
            </div>
        </div>
    );
}

export { ActualBudgetCardProps };