import { Trend } from './trend';
import { Point } from './case';

interface PointPerfil extends Point {
    valor: number;
    origem: string;
}

export interface CriarPerfilRET {
    idPoço: string;
    nomePerfil: string;
    valores: {
        pmTopo: number;
        pmBase: number;
        valor: number;
    }[];
}

export interface Perfil {
    id: string;
    idCálculo?: string;
    idPoço: string;
    nome: string;
    name: string;
    descrição: string;
    mnemonico: string;
    data: number[][];
    pm: number[][];
    pv: number[][];
    type: string;
    trend: Trend;
    larg: { min: number; max: number; };
    estiloVisual: {
        type: string;
        corDaLinha: string;
        espessura: number;
        estiloLinha: string;
        marcador: string;
        corDoMarcador: string;
    };
    grupo: {
        nome: string;
        valor: number;
    };
    grupoDeUnidades: {
        nome: string;
        unidadePadrão: {
            nome: string;
            símbolo: string;
        }
    };

    podeSerUsadoParaComplementarTrecho?: boolean;
    podeTerTrendCompactacao?: boolean;
    podeSerEntradaCálculoPerfis?: boolean;
    podeTerTrendBaseFolhelho?: boolean;
    temTrendLBF?: boolean;
    temTrendCompactação?: boolean;

    // Converter pro formato do highcharts
    pontos: PointPerfil[];
    count: number;
    primeiroPonto: PointPerfil;
    ultimoPonto: PointPerfil;
    valorMáximo: number;
    valorMínimo: number;
    pmMáximo: { valor: number };
    pmMínimo: { valor: number };
}
