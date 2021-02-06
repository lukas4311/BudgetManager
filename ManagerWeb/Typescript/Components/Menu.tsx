import React from "react";
import ReactDOM from "react-dom";

interface MenuState {
    isClosed: boolean;
}

class Menu extends React.Component<{}, MenuState> {
    constructor(props: {}) {
        super(props);
        this.menuClick = this.menuClick.bind(this);
        this.state = { isClosed: true };
    }

    menuClick() {
        this.setState((prev) => ({ isClosed: !prev.isClosed }));
    }

    redirectToPage = (link: string) => {
        window.location.href = '/' + link;
    }

    public render() {

        return (
            <nav className="border-bottom box-shadow">
                <div className="flex flex-col spin circle button justify-center w-12 h-12" onClick={this.menuClick}>
                    <div className="w-3/5 bg-white mb-2 h-1 rounded-lg mx-auto"></div>
                    <div className="w-3/5 bg-white mb-2 h-1 rounded-lg mx-auto"></div>
                    <div className="w-3/5 bg-white h-1 rounded-lg mx-auto"></div>
                </div>
                <div className={(this.state.isClosed ? "containerClosed" : "containerOpen") + " menu absolute text-white rounded-lg bg-black mt-4 w-48 text-xl"}>
                    <a className="block mt-2 ml-10 cursor-pointer" onClick={_ => this.redirectToPage("")}>Home</a>
                    <a className="block mt-2 ml-10 cursor-pointer" onClick={_ => this.redirectToPage("payments")}>Platby</a>
                    <a className="block mt-2 ml-10 cursor-pointer" onClick={_ => this.redirectToPage("crypto-overview")}>Crypto</a>
                    <a className="block mt-2 ml-10 cursor-pointer" onClick={_ => this.redirectToPage("budget")}>Rozpočet</a>
                    <a className="block mt-2 ml-10 cursor-pointer" onClick={_ => this.redirectToPage("debts")}>Dluhy</a>
                    <a className="block mt-2 ml-10 mb-2 cursor-pointer" onClick={_ => this.redirectToPage("stats")}>Statistiky</a>
                </div>
            </nav>
        );
    }
}

ReactDOM.render(<Menu />, document.getElementById('navMenu'));