export interface Trecho {
    idPerfil: string;
    pmTopo: number;
    pmBase: number;
}

export interface ComposicaoPerfil {
    idPoço: string;
    nomePerfil: string;
    tipoPerfil: string;
    listaTrechos: Trecho[];
}
