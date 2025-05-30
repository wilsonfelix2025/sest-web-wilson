/**
 * The structure of a trend in an API response.
 */
export interface Trend {
    id: string,
    nome: string
    tipoTrend: 'LBF' | 'Compactação' | 'Gradiente',
    perfil: string,
    trechos: trecho[],
    podeSerUsadoEmCálculo: boolean,
    series: Highcharts.SeriesLineOptions[];
}

interface trecho {
    valorTopo: number;
    pvTopo: number;
    valorBase: number;
    pvBase: number;
    inclinação: number;
}
