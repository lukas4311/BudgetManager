import { RouteComponentProps } from "react-router-dom";
import { GoldIngot } from "./GoldIngot";
import * as H from 'history';

export class GoldListProps {
    goldIngots: GoldIngot[];
    routeComponent: H.History<any>;
}
