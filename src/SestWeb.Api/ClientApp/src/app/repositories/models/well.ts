export interface Well {
    url: string,
    name: string,
    oilfield: {
        url: string,
        name: string
    },
    files: File[];
}

export interface WellData {
    name: string,
    oilFieldId: string
}