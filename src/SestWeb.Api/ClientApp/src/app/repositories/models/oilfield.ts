import { Well } from "./well";

export interface Oilfield {
    url: string,
    name: string,
    opunit: {
        url: string,
        name: string
    },
    wells: Well[]
}

export interface OilfieldData {
    name: string,
    unit: string
}