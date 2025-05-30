export interface CorrelationSlim {
    nome: string;
    perfilSaída?: string;
    origem?: string;
    chaveAutor: string;
}
/**
 * The structure of a correlation in an API response.
 */
export interface Correlation extends CorrelationSlim {
    idPoço?: string;
    nomeAutor?: string;
    descrição?: string;
    expressão?: string;
    expressãoBruta?: string;
    perfisEntrada?: string[];
    variáveis?: string[];
    éPropriedadeMecânica?: boolean;
}

export interface ObjetoIdentificado {
    name: string;
    value?: string | number;
    valid?: boolean;
}
