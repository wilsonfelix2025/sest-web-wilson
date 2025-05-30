import { Caminho } from "app/repositories/models/caminho";
import { State } from "app/repositories/models/state";

export interface DatasetCase {
    trajectoryId: string;
    lithologiesIds: string[];
    stratigraphiesIds: string[];
    profilesIds: string[];
    calculationsIds: string[];
    registerEventsIds: string[];
    tabsIds: string[],
    caseId: string;
    state: State;
    path: Caminho;
}
