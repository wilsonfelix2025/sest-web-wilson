export interface Estratigrafia {
    pm: { valor: number };
    pv: { valor: number };
    sigla: string;
    descricao: string;
    idade: number;
}

export interface StratigraphyChart {
    title: string;
    canvas: {
        name: string;
        pm;
        pv;
    }[];
}