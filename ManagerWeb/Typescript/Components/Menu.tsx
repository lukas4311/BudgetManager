import React from "react";
import ReactDOM from "react-dom";
import { IconsData } from "../Enums/IconsEnum";

interface MenuState {
    isClosed: boolean;
}

class MenuItemModel {
    icon: JSX.Element;
    linkUri: string;
}

class Menu extends React.Component<{}, MenuState> {
    private menuItems: MenuItemModel[];

    constructor(props: {}) {
        super(props);
        this.menuClick = this.menuClick.bind(this);
        this.state = { isClosed: true };
        this.renderMenuItems();
    }

    renderMenuItems() {
        let icons = new IconsData();
        this.menuItems = [
            { icon: icons.home, linkUri: "" },
            { icon: icons.payments, linkUri: "payments" },
            { icon: icons.crypto, linkUri: "crypto-overview" },
            { icon: icons.budget, linkUri: "budget" },
            { icon: icons.debts, linkUri: "debts" },
            { icon: icons.statistics, linkUri: "stats" },
        ];
    }

    menuClick() {
        this.setState((prev) => ({ isClosed: !prev.isClosed }));
    }

    redirectToPage = (link: string) => {
        window.location.href = '/' + link;
    }

    private renderMenuItem = (item: MenuItemModel): JSX.Element => {
        return (
            <li className="menu-item" key={item.linkUri}>
                <a className="block cursor-pointer flex" onClick={_ => this.redirectToPage(item.linkUri)}>
                    <span className="w-10 m-auto">
                        {item.icon}
                    </span>
                </a>
            </li>
        );
    }

    public render() {
        return (
            <div className={"mainWrapper" + (this.state.isClosed ? "" : " opened")}>
                <a className="menuTogglerParent" onClick={this.menuClick}>
                    <span className={"menu-toggler " + (this.state.isClosed ? "" : "checked")} id="menu-toggler"></span>
                </a>
                <ul className={(this.state.isClosed ? "" : "openedMenuItems")}>
                    {this.menuItems.map(m => this.renderMenuItem(m))}
                </ul>
            </div>
        );
    }
}

ReactDOM.render(<Menu />, document.getElementById('navMenu'));