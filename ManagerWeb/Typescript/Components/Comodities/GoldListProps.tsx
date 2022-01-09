import * as H from 'history';
import { ComoditiesFormViewModel } from "./ComoditiesForm";

export class GoldListProps {
    comoditiesViewModels: ComoditiesFormViewModel[];
    routeComponent: H.History<any>;
    addNewIngot?: () => void;
    editIngot?: (id: number) => void;
}
