import { Trajetoria, Sapata, Objetivo } from './trajetoria';
import { Perfil } from './perfil';
import { Litologia } from './litologia';
import { Estratigrafia } from './estratigrafia';
import { Calculo } from './calculo';
import { State } from './state';
import { Evento, Registro } from './registro-evento';

export interface CaseSlim {
    id: string;
    nome: string;
    tipoPoço?: string;
}

export interface Point {
    pm: { valor: number };
    pv: { valor: number };
}

export interface Identificação {
    nome: string;
    nomePoço: string;
    nomePoçoWeb: string;
    sonda: string;
    campo: string;
    companhia: string;
    bacia: string;

    finalidade: string;
    analista: string;
    nívelProteção: string;
    classificaçãoPoço: string;
    tipoCompletação: string;
    complexidadePoço: number;
    criticidadePoço: boolean;
    vidaÚtilPrevista: number;
    intervençãoWorkover: boolean;

    poçoWebUrl?: string;
    poçoWebDtÚltimaAtualização?: string;
    poçoWebAtualizado?: boolean;
    poçoWebRevisãoUrl?: string;
}

export interface Area {
    densidadeAguaMar: number;
    densidadeSuperficie: number;
    sonicoSuperficie: number;
    dtsSuperficie: number;
}

export interface Geometria {
    onShore: {
        lençolFreático: number;
        elevação: number;
        alturaDeAntePoço: number;
    };
    offShore: {
        laminaDagua: number;
    };
    coordenadas: {
        utMx: number;
        utMy: number;
    };
    categoriaPoço: 'OnShore' | 'OffShore';
    mesaRotativa: number;
}

export interface DadosGerais {
    minPv: number;
    minPm: number;
    maxPv: number;
    maxPm: number;
    identificação: Identificação;
    area: Area;
    geometria: Geometria;
    éVertical: boolean;
    pmFinal: number;
}

export interface Case extends CaseSlim {
    registrosDePerfuração;
    cálculos: Calculo[];
    idsPoçosApoio: string[];
    dadosGerais: DadosGerais;
    perfis: Perfil[];
    trajetória: Trajetoria;
    litologias: Litologia[];
    sapatas: Sapata[];
    objetivos: Objetivo[];
    estratigrafia: {
        Itens: {
            [key: string]: Estratigrafia[];
        }
    };
    registrosEventos: (Registro | Evento)[];
    state: State;
}
