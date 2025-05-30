import { LITOLOGIA_CLASS } from "@utils/litho-types-list";
import { DadosGerais } from "app/repositories/models/case";
import { Litologia } from "app/repositories/models/litologia";

export class LithologyUtils {

    public static lithologyPointsToData(lithology: Litologia, currCaseGeneralData: DadosGerais) {
        const tipo: {
            title: string;
            id: string;
            canvas: {
                className: string;
                name: string;
                data: { high: number; low: number; x: number }[];
                deep: boolean;
            }[];
        } = {
            title: lithology.classificação.nome,
            id: lithology.id,
            canvas: [],
        };
        const canvasPm = [];
        const canvasPv = [];

        if (lithology.pontos.length > 0) {
            if (currCaseGeneralData.minPm > lithology.pontos[0].pm.valor) {
                currCaseGeneralData.minPm = lithology.pontos[0].pm.valor;
            }
            if (currCaseGeneralData.maxPm < lithology.pontos[lithology.pontos.length - 1].pm.valor) {
                currCaseGeneralData.maxPm = lithology.pontos[lithology.pontos.length - 1].pm.valor;
            }
        }

        let sedimentos;
        if (currCaseGeneralData.geometria.categoriaPoço === 'OnShore') {
            sedimentos =
                currCaseGeneralData.geometria.mesaRotativa +
                currCaseGeneralData.geometria.onShore.alturaDeAntePoço;
        } else {
            sedimentos =
                currCaseGeneralData.geometria.mesaRotativa +
                currCaseGeneralData.geometria.offShore.laminaDagua;
        }

        const toRangeSeries = function (ponto, canvas, parameter) {
            if (
                canvas.length === 0 ||
                ponto.tipoRocha.nome !== canvas[canvas.length - 1].name
            ) {
                const serie = {
                    name: ponto.tipoRocha.nome,
                    data: [
                        { x: 0, low: ponto[parameter].valor, high: ponto[parameter].valor },
                    ],
                    className: LITOLOGIA_CLASS[ponto.tipoRocha.mnemonico.toUpperCase()],
                    code: ponto.tipoRocha.mnemonico,
                    deep: false,
                };
                if (parameter === 'pv') {
                    serie.deep = true;
                }
                if (canvas.length > 0) {
                    canvas[canvas.length - 1].data[0].low = serie.data[0].high;
                }
                canvas.push(serie);
            } else {
                canvas[canvas.length - 1].data[0].low = ponto[parameter].valor;
            }
        };

        lithology.pontos.sort((a, b) => a.pm.valor - b.pm.valor).forEach((ponto) => {
            toRangeSeries(ponto, canvasPm, 'pm');
        });
        lithology.pontos.sort((a, b) => a.pv.valor - b.pv.valor).forEach((ponto) => {
            toRangeSeries(ponto, canvasPv, 'pv');
        });

        const pushRangeToCanvas = function (p, deep) {
            const c = tipo.canvas.findIndex(
                (elem) => elem.name === p.name && elem.deep === deep
            );
            if (c > -1) {
                tipo.canvas[c].data.push(p.data[0]);
            } else {
                tipo.canvas.push(p);
            }
        };

        canvasPm.forEach((p) => {
            pushRangeToCanvas(p, false);
        });
        canvasPv.forEach((p) => {
            pushRangeToCanvas(p, true);
        });
        let agua = {
            name: 'Água',
            data: [
                {
                    low: currCaseGeneralData.geometria.mesaRotativa,
                    high: sedimentos,
                    x: 0,
                },
            ],
            className: LITOLOGIA_CLASS['H2O'],
            deep: true,
        };
        tipo.canvas.push(agua);

        agua = JSON.parse(JSON.stringify(agua));
        agua.deep = false;
        tipo.canvas.push(agua);

        return tipo;
    }
}