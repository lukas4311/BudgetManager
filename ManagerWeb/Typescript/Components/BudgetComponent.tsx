import * as React from 'react'

export default class BudgetComponent extends React.Component<{}, {}> {
    render() {
        return (
            <div className="flex w-full">
                <div className="py-4 flex w-full">
                    <h1 className="ml-6 text-xl">Rozpočty</h1>
                    <span className="inline-block ml-auto mr-5">
                        <svg xmlns="http://www.w3.org/2000/svg" height="24" viewBox="0 0 24 24" width="24" className="fill-current text-white hover:text-vermilion transition ease-out duration-700 cursor-pointer">
                            <path d="M0 0h24v24H0z" fill="none" />
                            <path d="M19 3H5c-1.11 0-2 .9-2 2v14c0 1.1.89 2 2 2h14c1.1 0 2-.9 2-2V5c0-1.1-.9-2-2-2zm-2 10h-4v4h-2v-4H7v-2h4V7h2v4h4v2z" />
                        </svg>
                    </span>
                </div>
            </div>
        );
    }
}