import React from "react";
import { ErrorBoundaryState } from "./ErrorBoundaryState";

export default class ErrorBoundary extends React.Component<any, ErrorBoundaryState> {
    constructor(props:any) {
        super(props);
        this.state = { hasError: false };
    }

    componentDidCatch(error: any, info: any) {
        this.setState({ hasError: true });
        console.log(error);
    }

    render() {
        if (this.state.hasError) 
            return <h1>Something went wrong.</h1>;
            
        return this.props.children;
    }
}