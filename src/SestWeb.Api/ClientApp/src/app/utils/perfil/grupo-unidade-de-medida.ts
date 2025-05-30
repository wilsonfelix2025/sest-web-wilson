import { UNIDADE_DE_MEDIDA, UnidadeDeMedida } from './unidade-de-medida';

export interface GrupoUnidadeDeMedida {
    nome: string;
    descrição: string;
    unidadesDeMedida: UnidadeDeMedida[];
}

export const GRUPO_UNIDADE_DE_MEDIDA = {
    grupoÂngulo: {
        nome: 'UnidadesDeÂngulo', descrição: 'Unidades de ângulo',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.grau,
            UNIDADE_DE_MEDIDA.radiano,
            UNIDADE_DE_MEDIDA.segundoDeArco,
        ]
    },
    grupoDensidade: {
        nome: 'UnidadesDeDensidade', descrição: 'Unidades de densidade',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.gramasPorCentímetroCúbico,
        ]
    },
    grupoDiâmetro: {
        nome: 'UnidadesDeDiâmetro', descrição: 'Unidades de diâmetro',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.polegada,
            UNIDADE_DE_MEDIDA.pé,
            UNIDADE_DE_MEDIDA.metro,
            UNIDADE_DE_MEDIDA.centímetro,
            UNIDADE_DE_MEDIDA.milímetro,
        ]
    },
    grupoGenerico: {
        nome: 'Unidades', descrição: 'Unidades de genericos',
        unidadesDeMedida: Object.keys(UNIDADE_DE_MEDIDA).map(key => UNIDADE_DE_MEDIDA[key])
    },
    grupoSemDimensão: {
        nome: 'UnidadesSemDimensão', descrição: 'Unidades sem dimensão',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.semDimensão,
        ]
    },
    grupoRaiosGama: {
        nome: 'UnidadesDeRaiosGama', descrição: 'Unidades de raios gama',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.api,
        ]
    },
    grupoGradiente: {
        nome: 'UnidadesDeGradiente', descrição: 'Unidades de gradiente',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.librasPorGalão,
            UNIDADE_DE_MEDIDA.gramasPorCentímetroCúbico,
            UNIDADE_DE_MEDIDA.librasPorPolegadaQuadradaPorPé,
            UNIDADE_DE_MEDIDA.quilopascalPorMetro,
            UNIDADE_DE_MEDIDA.gravidadeEspecífica,
            UNIDADE_DE_MEDIDA.quilogramasPorMetroCúbico,
            UNIDADE_DE_MEDIDA.gramasPorLitro,
            UNIDADE_DE_MEDIDA.quilogramasPorLitro,
            UNIDADE_DE_MEDIDA.libraPorPéCúbico,
            UNIDADE_DE_MEDIDA.libraPorBarril,
            UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaPorCemPés,
        ]
    },
    grupoDistância: {
        nome: 'UnidadesDeDistância', descrição: 'Unidades de distância',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.metro,
            UNIDADE_DE_MEDIDA.pé,
            UNIDADE_DE_MEDIDA.centímetro,
            UNIDADE_DE_MEDIDA.milímetro,
            UNIDADE_DE_MEDIDA.polegada,
        ]
    },
    grupoPermeabilidade: {
        nome: 'UnidadesDePermeabilidade', descrição: 'Unidades de permeabilidade',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.milidarcy,
            UNIDADE_DE_MEDIDA.darcy,
        ]
    },
    grupoPressão: {
        nome: 'UnidadesDePressão', descrição: 'Unidades de pressão',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaAbsoluta,
            UNIDADE_DE_MEDIDA.libraPorPolegadaQuadradaManométrica,
            UNIDADE_DE_MEDIDA.atmosfera,
            UNIDADE_DE_MEDIDA.megaPascal,
            UNIDADE_DE_MEDIDA.bar,
        ]
    },
    grupoProporção: {
        nome: 'UnidadesDeProporção', descrição: 'Unidades de proporção',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.fator,
            UNIDADE_DE_MEDIDA.porcentagem,
        ]
    },
    grupoResistividade: {
        nome: 'UnidadesDeResistividade', descrição: 'Unidades de resistividade',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.ohmPorMetro,
        ]
    },
    grupoRotação: {
        nome: 'UnidadesDeRotação', descrição: 'Unidades de rotação',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.rotaçõesPorMinuto,
            UNIDADE_DE_MEDIDA.rotaçõesPorSegundo,
            UNIDADE_DE_MEDIDA.radianosPorSegundo,
            UNIDADE_DE_MEDIDA.rotaçõesPorHora,
        ]
    },
    grupoSônico: {
        nome: 'UnidadesDeSônico', descrição: 'Unidades de sônico',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.microssegundoPorPé,
            UNIDADE_DE_MEDIDA.segundoPorMetro,
        ]
    },
    grupoIrop: {
        nome: 'UnidadesDeTaxaDePenetraçãoParaIROP', descrição: 'Unidades de taxa de penetração para IROP',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.minutoPorMetro,
        ]
    },
    grupoVelSis: {
        nome: 'UnidadesDeVelocidadeSísmica', descrição: 'Unidades de velocidade sísmica',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.metroPorSegundo,
            UNIDADE_DE_MEDIDA.quilometroPorSegundo,
            UNIDADE_DE_MEDIDA.quilopéPorSegundo,
            UNIDADE_DE_MEDIDA.péPorSegundo,
        ]
    },
    grupoRop: {
        nome: 'UnidadesDeTaxaDePenetração', descrição: 'Unidades de taxa de penetração',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.metroPorHora,
            UNIDADE_DE_MEDIDA.metroPorMinuto,
            UNIDADE_DE_MEDIDA.metroPorSegundo,
            UNIDADE_DE_MEDIDA.péPorHora,
            UNIDADE_DE_MEDIDA.péPorMinuto,
            UNIDADE_DE_MEDIDA.péPorSegundo,
        ]
    },
    grupoPeso: {
        nome: 'UnidadesDePeso', descrição: 'Unidades de peso',
        unidadesDeMedida: [
            UNIDADE_DE_MEDIDA.tonelada,
            UNIDADE_DE_MEDIDA.libra,
            UNIDADE_DE_MEDIDA.newton,
            UNIDADE_DE_MEDIDA.quiloNewton,
            UNIDADE_DE_MEDIDA.quiloLibra,
            UNIDADE_DE_MEDIDA.quilogramaForça,
        ]
    },
};
