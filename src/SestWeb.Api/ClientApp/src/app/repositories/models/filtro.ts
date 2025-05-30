import { ArrayUtils } from "@utils/array";

export interface Filtro {
    idPerfil: string,
    nome: string,
    limiteInferior?: number,
    limiteSuperior?: number,
    tipoCorte?: TiposCorte,
}

export interface FiltroSimples extends Filtro {
    desvioMáximo: number,
}

export interface FiltroMediaMovel extends Filtro {
    númeroPontos: number,
}

export interface FiltroLitologia extends Filtro {
    litologiasSelecionadas: string[],
}

export interface FiltroLBF extends Filtro {
    idLBF: string,
}

export type TiposCorte = 'Truncar' | 'Eliminar';

const filtros = ArrayUtils.stringLitArray(['FiltroSimples', 'FiltroLitologia', 'FiltroLinhaBaseFolhelho', 'FiltroMédiaMóvel', 'FiltroCorte']);
export type TiposFiltro = (typeof filtros)[number];
export const isFiltro = (x: any): x is TiposFiltro => filtros.includes(x);
