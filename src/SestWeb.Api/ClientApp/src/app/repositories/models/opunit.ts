import { Oilfield } from "./oilfield";
import { Well } from "./well";

export interface OpUnit {
    url: string,
    name: string,
    oilfields: Oilfield[],
    wells: Well[]
}

export interface OpUnitData {
    name: string
}