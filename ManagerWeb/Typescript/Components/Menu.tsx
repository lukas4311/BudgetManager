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
            <>
                <a className="menuTogglerParent" onClick={this.menuClick}>
                    <span className={"menu-toggler " + (this.state.isClosed ? "" : "checked") } id="menu-toggler"></span>
                </a>
                <ul>
                    <li className="menu-item">
                        <a className="fa fa-facebook" href="https://www.facebook.com/" target="_blank"></a>
                    </li>
                    <li className="menu-item">
                        <a className="fa fa-google" href="https://www.google.com/" target="_blank"></a>
                    </li>
                    <li className="menu-item">
                        <a className="fa fa-dribbble" href="https://dribbble.com/" target="_blank"></a>
                    </li>
                    <li className="menu-item">
                        <a className="fa fa-codepen" href="https://codepen.io/" target="_blank"></a>
                    </li>
                    <li className="menu-item">
                        <a className="fa fa-linkedin" href="https://www.linkedin.com/" target="_blank"></a>
                    </li>
                    <li className="menu-item">
                        <a className="fa fa-github" href="https://github.com/" target="_blank"></a>
                    </li>
                </ul>
            </>
        );
    }
}

ReactDOM.render(<Menu />, document.getElementById('navMenu'));