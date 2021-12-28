import { GoldIngot } from "./GoldIngot";
import * as H from 'history';

export class GoldListProps {
    goldIngots: GoldIngot[];
    routeComponent: H.History<any>;
    addNewIngot?: () => void;
    deleteIngot?: (id: number) => void;
}
