export interface Caminho {
    filesCount: number;
    id: string;
    name: string;
    url: string;
    children: Caminho[] | Caso[];
}

export interface Caso {
    id: string;
    name: string;
    url: string;
    type: string;
}