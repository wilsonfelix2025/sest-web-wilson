import { Case } from "app/repositories/models/case";
import { Perfil } from "app/repositories/models/perfil";
import { Trend } from "app/repositories/models/trend";

export class ProfileUtils {
    public static profilePointsToData(profile: Perfil) {
        profile.pm = new Array<number[]>();
        profile.pv = new Array<number[]>();
        profile.larg = {
            min: profile.valorMínimo,
            max: profile.valorMáximo,
        };

        profile.pontos.sort((a, b) => a.pm.valor - b.pm.valor).forEach((ponto) => {
            profile.pm.push([
                parseFloat(ponto.pm.valor.toString()),
                parseFloat(ponto.valor.toString()),
            ]);
        });
        profile.pontos.sort((a, b) => a.pv.valor - b.pv.valor).forEach((ponto) => {
            profile.pv.push([
                parseFloat(ponto.pv.valor.toString()),
                parseFloat(ponto.valor.toString()),
            ]);
        });

        profile.name = profile.nome;
        profile.type = 'line';
    }

    public static trendPointsToData(trend: Trend, idPerfil: string, poco: Case) {
        trend.series = [];
        let sedimentos;
        if (poco.dadosGerais.geometria.categoriaPoço === 'OnShore') {
            sedimentos =
                poco.dadosGerais.geometria.mesaRotativa +
                poco.dadosGerais.geometria.onShore.alturaDeAntePoço;
        } else {
            sedimentos =
                poco.dadosGerais.geometria.mesaRotativa +
                poco.dadosGerais.geometria.offShore.laminaDagua;
        }
        trend.trechos.sort((a, b) => a.pvTopo - b.pvTopo);

        let deep = 'pv';
        if (trend.tipoTrend === 'LBF') {
            deep = 'pm';
        }

        trend.trechos.forEach((trecho, i) => {
            const serie = {
                type: 'line',
                data: [],
                larg: {
                    min: trecho.valorBase,
                    max: trecho.valorTopo,
                },
                idPerfil: idPerfil,
                indexTrecho: i,
                marker: {
                    enabled: true,
                    // fillColor: ,
                    symbol: 'circle',
                },
                color: 'red',
                dragDrop: {
                    draggableY: true,
                    dragMinX: i === 0 ? sedimentos : trend.trechos[i - 1].pvBase,
                    dragMaxX:
                        i === trend.trechos.length - 1
                            ? poco.trajetória.últimoPonto[deep].valor
                            : trend.trechos[i + 1].pvTopo,
                },
                states: {
                    hover: {
                        enabled: true,
                    },
                },
                name: trend.tipoTrend === 'LBF' ? 'LBF' : 'Trend',
            } as Highcharts.SeriesLineOptions;
            serie.data.push([trecho.pvTopo, trecho.valorTopo]);
            serie.data.push([trecho.pvBase, trecho.valorBase]);

            trend.series.push(serie);
        });

        // console.log(profile.data);
    }
}