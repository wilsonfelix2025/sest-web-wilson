import { GRUPO_UNIDADE_DE_MEDIDA, GrupoUnidadeDeMedida } from './grupo-unidade-de-medida';
import { GRUPO_PERFIS, GrupoPerfil } from './grupo-perfis';
import { UnidadeDeMedida, UNIDADE_DE_MEDIDA } from './unidade-de-medida';

export interface TipoPerfil {
    mnemônico: string;
    descrição: string;
    grupoUnidade: GrupoUnidadeDeMedida;
    grupoPerfil: GrupoPerfil;
    unidadePadrão: UnidadeDeMedida;
}

export const TIPOS_PERFIS = {
    ANGAT: {
        mnemônico: 'ANGAT', descrição: 'Ângulo de Atrito',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoÂngulo,
        grupoPerfil: GRUPO_PERFIS.grupoPropMec,
        unidadePadrão: UNIDADE_DE_MEDIDA.grau
    },
    AREA_PLASTIFICADA: {
        mnemônico: 'AREA_PLASTIFICADA', descrição: 'Área Plastificada',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoProporção,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.porcentagem
    },
    AZTHmin: {
        mnemônico: 'AZTHmin', descrição: 'Ângulo de azimute da tensão horizontal menor',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoÂngulo,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.grau
    },
    BIOT: {
        mnemônico: 'BIOT', descrição: 'Coeficiente Biot',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoSemDimensão,
        grupoPerfil: GRUPO_PERFIS.grupoPropMec,
        unidadePadrão: UNIDADE_DE_MEDIDA.semDimensão
    },
    // BITSIZE: {
    //     mnemônico: 'BITSIZE', descrição: 'Diâmetro da broca',
    //     grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoDiâmetro,
    //     grupoPerfil: GRUPO_PERFIS.grupoPropMec,
    //     unidadePadrão: UNIDADE_DE_MEDIDA.polegada
    // },
    CALIP: {
        mnemônico: 'CALIP', descrição: 'Caliper',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoDiâmetro,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.polegada
    },
    COESA: {
        mnemônico: 'COESA', descrição: 'Coesão',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPressão,
        grupoPerfil: GRUPO_PERFIS.grupoPropMec,
        unidadePadrão: UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta
    },
    DIAM_BROCA: {
        mnemônico: 'DIAM_BROCA', descrição: 'Diâmetro de Broca',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoDiâmetro,
        grupoPerfil: GRUPO_PERFIS.grupoPropMec,
        unidadePadrão: UNIDADE_DE_MEDIDA.polegada
    },
    DTC: {
        mnemônico: 'DTC', descrição: 'Tempo de Trânsito Compressional',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoSônico,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.microssegundoPorPé
    },
    DTMC: {
        mnemônico: 'DTMC', descrição: 'Tempo de Trânsito Compressional da Matriz',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoSônico,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.microssegundoPorPé
    },
    DTMS: {
        mnemônico: 'DTMS', descrição: 'Tempo de Trânsito Cisalhante da Matriz',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoSônico,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.microssegundoPorPé
    },
    DTS: {
        mnemônico: 'DTS', descrição: 'Tempo de Trânsito Cisalhante',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoSônico,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.microssegundoPorPé
    },
    ExpoenteD: {
        mnemônico: 'ExpoenteD', descrição: 'Expoente D',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoSemDimensão,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.semDimensão
    },
    GCOLI: {
        mnemônico: 'GCOLI', descrição: 'Gradiente de Colapso Inferior',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoGradiente,
        grupoPerfil: GRUPO_PERFIS.grupoGradientes,
        unidadePadrão: UNIDADE_DE_MEDIDA.librasPorGalão
    },
    GCOLS: {
        mnemônico: 'GCOLS', descrição: 'Gradiente de Colapso Superior',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoGradiente,
        grupoPerfil: GRUPO_PERFIS.grupoGradientes,
        unidadePadrão: UNIDADE_DE_MEDIDA.librasPorGalão
    },
    GECD: {
        mnemônico: 'GECD', descrição: 'Densidade Equivalente de Circulação',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoGradiente,
        grupoPerfil: GRUPO_PERFIS.grupoGradientes,
        unidadePadrão: UNIDADE_DE_MEDIDA.librasPorGalão
    },
    GENERICO: {
        mnemônico: 'GENERICO', descrição: 'Genérico',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoGenerico,
        grupoPerfil: GRUPO_PERFIS.genérico,
        unidadePadrão: UNIDADE_DE_MEDIDA.semDimensão
    },
    GFRAT: {
        mnemônico: 'GFRAT', descrição: 'Gradiente de Fratura',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoGradiente,
        grupoPerfil: GRUPO_PERFIS.grupoGradientes,
        unidadePadrão: UNIDADE_DE_MEDIDA.librasPorGalão
    },
    GFRAT_σh: {
        mnemônico: 'GFRAT_σh', descrição: 'Gradiente da Tensão Horizontal Menor',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoGradiente,
        grupoPerfil: GRUPO_PERFIS.grupoGradientes,
        unidadePadrão: UNIDADE_DE_MEDIDA.librasPorGalão
    },
    GLAMA: {
        mnemônico: 'GLAMA', descrição: 'Gradiente de Peso de Lama',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoGradiente,
        grupoPerfil: GRUPO_PERFIS.grupoGradientes,
        unidadePadrão: UNIDADE_DE_MEDIDA.librasPorGalão
    },
    GPORO: {
        mnemônico: 'GPORO', descrição: 'Gradiente de Pressão de Poros',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoGradiente,
        grupoPerfil: GRUPO_PERFIS.grupoGradientes,
        unidadePadrão: UNIDADE_DE_MEDIDA.librasPorGalão
    },
    GPPI: {
        mnemônico: 'GPPI', descrição: 'Gradiente de Pressão de Poros Interpretado',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoGradiente,
        grupoPerfil: GRUPO_PERFIS.grupoGradientes,
        unidadePadrão: UNIDADE_DE_MEDIDA.librasPorGalão
    },
    GQUEBRA: {
        mnemônico: 'GQUEBRA', descrição: 'Gradiente de Quebra',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoGradiente,
        grupoPerfil: GRUPO_PERFIS.grupoGradientes,
        unidadePadrão: UNIDADE_DE_MEDIDA.librasPorGalão
    },
    GRAY: {
        mnemônico: 'GRAY', descrição: 'Gamma Ray',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoRaiosGama,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.api
    },
    GSOBR: {
        mnemônico: 'GSOBR', descrição: 'Gradiente de Sobrecarga',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoGradiente,
        grupoPerfil: GRUPO_PERFIS.grupoGradientes,
        unidadePadrão: UNIDADE_DE_MEDIDA.librasPorGalão
    },
    GTHORmax: {
        mnemônico: 'GTHORmax', descrição: 'Gradiente da Tensão Horizontal Maior',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoGradiente,
        grupoPerfil: GRUPO_PERFIS.grupoGradientes,
        unidadePadrão: UNIDADE_DE_MEDIDA.librasPorGalão
    },
    IROP: {
        mnemônico: 'IROP', descrição: 'Taxa de penetração invertida',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoIrop,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.minutoPorMetro
    },
    K0: {
        mnemônico: 'K0', descrição: 'Perfil de K0',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoSemDimensão,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.semDimensão
    },
    KS: {
        mnemônico: 'KS', descrição: 'Módulo de Compressibilidade dos Grãos',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPressão,
        grupoPerfil: GRUPO_PERFIS.grupoPropMec,
        unidadePadrão: UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta
    },
    PCOLI: {
        mnemônico: 'PCOLI', descrição: 'Pressão de Colapso Inferior',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPressão,
        grupoPerfil: GRUPO_PERFIS.grupoPressões,
        unidadePadrão: UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta
    },
    PCOLS: {
        mnemônico: 'PCOLS', descrição: 'Pressão de Colapso Superior',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPressão,
        grupoPerfil: GRUPO_PERFIS.grupoPressões,
        unidadePadrão: UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta
    },
    PERM: {
        mnemônico: 'PERM', descrição: 'Permeabilidade',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPermeabilidade,
        grupoPerfil: GRUPO_PERFIS.grupoPropMec,
        unidadePadrão: UNIDADE_DE_MEDIDA.milidarcy
    },
    POISS: {
        mnemônico: 'POISS', descrição: 'Coeficiente de Poisson',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoSemDimensão,
        grupoPerfil: GRUPO_PERFIS.grupoPropMec,
        unidadePadrão: UNIDADE_DE_MEDIDA.semDimensão
    },
    PORO: {
        mnemônico: 'PORO', descrição: 'Porosidade',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoProporção,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.porcentagem
    },
    PPORO: {
        mnemônico: 'PPORO', descrição: 'Pressão de Poros',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPressão,
        grupoPerfil: GRUPO_PERFIS.grupoPressões,
        unidadePadrão: UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta
    },
    PQUEBRA: {
        mnemônico: 'PQUEBRA', descrição: 'Pressão de Quebra',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPressão,
        grupoPerfil: GRUPO_PERFIS.grupoPressões,
        unidadePadrão: UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta
    },
    RESIST: {
        mnemônico: 'RESIST', descrição: 'Resistividade',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoResistividade,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.ohmPorMetro
    },
    RESTR: {
        mnemônico: 'RESTR', descrição: 'Resistência à Tração',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPressão,
        grupoPerfil: GRUPO_PERFIS.grupoPropMec,
        unidadePadrão: UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta
    },
    RHOB: {
        mnemônico: 'RHOB', descrição: 'Densidade da Formação',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoDensidade,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.gramasPorCentímetroCúbico
    },
    RHOG: {
        mnemônico: 'RHOG', descrição: 'Densidade dos Grãos',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoDensidade,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.gramasPorCentímetroCúbico
    },
    ROP: {
        mnemônico: 'ROP', descrição: 'Taxa de Penetração',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoRop,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.metroPorHora
    },
    RPM: {
        mnemônico: 'RPM', descrição: 'Revoluções por Minuto',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoRotação,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.rotaçõesPorMinuto
    },
    THORmax: {
        mnemônico: 'THORmax', descrição: 'Tensão Horizontal Maior',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPressão,
        grupoPerfil: GRUPO_PERFIS.grupoTensões,
        unidadePadrão: UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta
    },
    THORmin: {
        mnemônico: 'THORmin', descrição: 'Tensão Horizontal Menor',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPressão,
        grupoPerfil: GRUPO_PERFIS.grupoTensões,
        unidadePadrão: UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta
    },
    RET: {
        mnemônico: 'RET', descrição: 'Relação entre tensões',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoSemDimensão,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.semDimensão
    },
    TVERT: {
        mnemônico: 'TVERT', descrição: 'Tensão Vertical',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPressão,
        grupoPerfil: GRUPO_PERFIS.grupoTensões,
        unidadePadrão: UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta
    },
    UCS: {
        mnemônico: 'UCS', descrição: 'Resistência à Compressão Uniaxial',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPressão,
        grupoPerfil: GRUPO_PERFIS.grupoPropMec,
        unidadePadrão: UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta
    },
    VCL: {
        mnemônico: 'VCL', descrição: 'Volume de Argila',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoProporção,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.porcentagem
    },
    VP: {
        mnemônico: 'VP', descrição: 'Velocidade Sísmica Compressional',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoVelSis,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.metroPorSegundo
    },
    WOB: {
        mnemônico: 'WOB', descrição: 'Peso Sobre Broca',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPeso,
        grupoPerfil: GRUPO_PERFIS.grupoPerfil,
        unidadePadrão: UNIDADE_DE_MEDIDA.tonelada
    },
    YOUNG: {
        mnemônico: 'YOUNG', descrição: 'Módulo de Young',
        grupoUnidade: GRUPO_UNIDADE_DE_MEDIDA.grupoPressão,
        grupoPerfil: GRUPO_PERFIS.grupoPropMec,
        unidadePadrão: UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta
    }
};

export const LISTA_MNEMÔNICOS = Object.keys(TIPOS_PERFIS);
