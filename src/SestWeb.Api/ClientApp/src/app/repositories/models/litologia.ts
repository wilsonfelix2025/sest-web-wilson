import { Point } from './case';

export interface GrupoLitologico {
    nome: string;
    valor: number;
}
export interface TipoRocha {
    grupo: GrupoLitologico;
    mnemonico: string;
    nome: string;
    numero: number;
    dtmc: number;
    rhog: number;
}

interface PointLito extends Point {
    valor: number;
    origem: string;
    tipoRocha: TipoRocha;
    tipoProfundidade: string;
}

export interface Litologia {
    id: string;
    classificação: {
        nome: string;
        valor: number;
    };
    pontos: PointLito[];
    primeiroPonto: PointLito;
    ultimoPonto: PointLito;
}
