import { ArrayUtils } from '@utils/array';
import { TiposFiltro } from './filtro';
import { Perfil } from './perfil';

interface Parametro {
    correlação: string;
    parâmetro: string;
    valor: number;
}

export interface CalculoPerfis {
    nome: string;
    idPoço: string;
    correlações: string[];
    listaIdPerfisEntrada: string[];
    parâmetros: Parametro[];
    trechos?: {
        tipoPerfil: string;
        pmTopo: number;
        pmBase: number;
        correlação: string;
        listaParametros: Parametro[];
    }[];
    idCálculo?: string;
}
const perfis = ArrayUtils.stringLitArray(['Perfis']);
export type TipoPerfil = (typeof perfis)[number];
export const isPerfis = (x: any): x is TipoPerfil => perfis.includes(x);

export interface CalculoSobrecarga {
    nome: string;
    idRhob: string;
    idPoço: string;
    idCálculoAntigo?: string;
}
const sobrecarga = ArrayUtils.stringLitArray(['Sobrecarga']);
export type TipoSobrecarga = (typeof sobrecarga)[number];
export const isSobrecarga = (x: any): x is TipoSobrecarga => sobrecarga.includes(x);

interface CalculoPPBasico {
    nome: string;
    idPoço: string;
    id?: string;
}

export interface Reservatorio {
    nome: string;
    referencia: {
        poco: string;
        cota: number;
        pp: number;
        gradiente: {
            gas: number;
            oleo: number;
            agua: number;
        };
        contatos: {
            gasOleo: number;
            oleoAgua: number;
        };
    };
    interesse: {
        topo: number;
        base: number;
    };
}

export interface CalculoPP extends CalculoPPBasico {
    tipo: 'PP';
    gn: number;
    exp: number;
    idPerfilFiltrado: string;
    idGradienteSobrecarga: string;
    calculoReservatorio: boolean;
    reservatorio?: Reservatorio;
}
export interface CalculoPPH extends CalculoPPBasico {
    tipo: 'PPh';
    gn: number;
}
export interface CalculoGPP extends CalculoPPBasico {
    tipo: 'GPP';
    gpph?: number;
    idPph?: string;
}

const pressaoPoros = ArrayUtils.stringLitArray(['PressãoPoros']);
export type TipoPressaoPoros = (typeof pressaoPoros)[number];
export const isPressaoPoros = (x: any): x is TipoPressaoPoros => pressaoPoros.includes(x);

interface ParametroResistencia {
    grupoLitológico: string;
    UCS: string;
    COESA: string;
    ANGAT: string;
    RESTR: string;
}

interface TrechoResistencia extends ParametroResistencia {
    pmTopo: number;
    pmBase: number;
}

export interface CalculoPropriedadesMecanicas {
    nome: string;
    idPoço: string;
    correlações: string[];
    listaIdPerfisEntrada: string[];
    regiões: ParametroResistencia[];
    trechos?: TrechoResistencia[];
    idCálculo?: string;
}
const propriedadesMecanicas = ArrayUtils.stringLitArray(['PropriedadesMecânicas']);
export type TipoPropriedadesMecanicas = (typeof propriedadesMecanicas)[number];
export const isPropriedadesMecanicas = (x: any): x is TipoPropriedadesMecanicas => propriedadesMecanicas.includes(x);

interface Lot {
    lda: number;
    mr: number;
    pv: number;
    lot: number;
    gradPressãoPoros: number;
    tVert: number;
}

export type TipoGraficoTensoes = 'NormalizaçãoLDA' | 'NormalizaçãoPP' | 'K0Acompanhamento' | 'K0';

export interface CalcularGraficoTensoes {
    idPoço: string;
    listaLot: Lot[];
    perfilTensãoVerticalId?: string;
    perfilGPOROId?: string;
    coeficiente?: number;
}

interface TrechoBreakout {
    pm: number;
    azimute: number;
    largura: number;
    pesoFluido: number;
}

interface TrechoFratura {
    pm: number;
    pesoFluido: number;
}

export interface CalculoTHorMax {
    nomeCálculo: string;
    tensãoHorizontalMaiorMetodologiaCálculo: 'RelaçãoEntreTensões' | 'BreakoutTrechosVerticais' | 'FraturasTrechosVerticais';
    relaçãoTensão?: {
        perfilRelaçãoTensãoId: string;
        tipoRelação: string;
        azthMenor: number;
        perfilTVERTId?: string;
        perfilGPOROId?: string;
    };
    breakout?: {
        perfilUCSId: string;
        perfilANGATId: string;
        perfilGPOROId: string;
        azimuth: string;
        trechosBreakout: TrechoBreakout[];
    };
    fraturasTrechosVerticais?: {
        perfilRESTRId: string;
        perfilGPOROId: string;
        azimuth: string;
        trechosFratura: TrechoFratura[];
    };
}

export interface CalculoTensoesInSitu extends CalcularGraficoTensoes, CalculoTHorMax {
    idCálculo?: string;
    perfilPoissonId?: string;
    perfilTHORminId?: string;
    depleção?: {
        gporoOriginalId: string;
        gporoDepletadaId: string;
        poissonId: string;
        biotId: string;
    };
    tensãoHorizontalMenorMetodologiaCálculo?: string;
}

export interface RetornoCalculoTensoesInSitu extends Calculo, CalculoTensoesInSitu {
    metodologiaTHORmax: 'RelaçãoEntreTensões' | 'BreakoutTrechosVerticais' | 'FraturasTrechosVerticais';
    metodologiaTHORmin?: 'ModeloElástico' | 'NormalizaçãoLDA' | 'NormalizaçãoPP' | 'K0Acompanhamento' | 'K0';

    coeficiente?: number;

    parâmetrosLotDTO?: {
        lda?: number, mesaRotativa?: number, profundidadeVertical?: number, lot?: number, gradPressãoPoros?: number, tvert?: number
    }[];
}
const tensoesInSitu = ArrayUtils.stringLitArray(['Tensões']);
export type TipoTensoesInSitu = (typeof tensoesInSitu)[number];
export const isTensoesInSitu = (x: any): x is TipoTensoesInSitu => tensoesInSitu.includes(x);

export interface ParametrosPoroElastico {
    kf: number;
    viscosidade: number;
    coeficienteReflexão: number;
    coeficienteInchamento: number;
    coeficienteDifusãoSoluto: number;
    densidadeFluidoFormação: number;
    temperaturaFormação: number;
    temperaturaFormaçãoFisicoQuimica: number;
    concentraçãoSolFluidoPerfuração: number;
    concentraçãoSolutoRocha: number;
    tipoSal: string;
    coeficienteDissociaçãoSoluto: number;
    massaMolarSoluto: number;
    expansãoTérmicaVolumeFluidoPoros: number;
    temperaturaPoço: number;
    propriedadeTérmicaTemperaturaFormação: number;
    difusidadeTérmica: number;
    expansãoTérmicaRocha: number;
    litologias: string[];
}

export interface CalculoGradiente {
    nomeCálculo: string;
    idPoço: string;
    tipoModelo: string;
    fluidoPenetrante?: boolean;
    areaPlastificada: number;
    tipoCritérioRuptura: string;
    tempo?: number;
    habilitarFiltroAutomatico?: boolean;
    incluirEfeitosFísicosQuímicos?: boolean;
    incluirEfeitosTérmicos?: boolean;
    calcularFraturasColapsosSeparadamente?: boolean;
    malhaRextRint: number;
    malhaNunDivAng: number;
    malhaNunDivRad: number;
    malhaRMaxRMin: number;

    parâmetrosPoroElástico?: ParametrosPoroElastico;
    id?: string;
    idCálculo?: string;
}
const gradiente = ArrayUtils.stringLitArray(['Gradientes']);
export type TipoGradiente = (typeof gradiente)[number];
export const isGradiente = (x: any): x is TipoGradiente => gradiente.includes(x);

export interface CalculoExpD {
    nome: string;
    idPoço: string;
    correlação: string;
    perfilROPId: string;
    perfilRPMId: string;
    perfilWOBId: string;
    perfilDIAM_BROCA: string;
    perfilECDId?: string;

    idCálculo?: string;
}
const expoenteD = ArrayUtils.stringLitArray(['ExpoenteD']);
export type TipoExpD = (typeof expoenteD)[number];
export const isExpD = (x: any): x is TipoExpD => expoenteD.includes(x);

export interface Calculo {
    grupoCálculo: TiposCalculo;
    id: string;
    nome: string;
    perfisEntrada: { idPerfis: string[], perfis: Perfil[] };
    perfisSaída: { idPerfis: string[], perfis: Perfil[] };
    temSaídaZerada: boolean;
    perfisEntradaPossuemPontos: boolean;
}


export type TiposCalculo = TiposFiltro | TipoSobrecarga | TipoPressaoPoros | TipoTensoesInSitu | TipoGradiente | TipoExpD;
export type TiposPerfuração = TipoExpD;
export const isPerfuracao = (x: any): x is TiposPerfuração => isExpD(x) || isExpD(x);
