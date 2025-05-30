import { Case } from "app/repositories/models/case";
import { Estratigrafia, StratigraphyChart } from "app/repositories/models/estratigrafia";

export class StratigraphyUtils {

    private static aux(item, index, serie, parameter, maxParameter, _case) {
        if (item[index][parameter] !== undefined) {
            serie[parameter][0].push(item[index][parameter].valor);
            if (index < item.length - 1 && item[index + 1][parameter] !== undefined) {
                serie[parameter][0].push(item[index + 1][parameter].valor);
            } else {
                if (_case.dadosGerais[maxParameter] < item[index][parameter].valor + 1) {
                    _case.dadosGerais[maxParameter] = item[index][parameter].valor + 1;
                }
                serie[parameter][0].push(_case.dadosGerais[maxParameter]);
            }
        }
    };

    public static loadStratigraphy(stratigraphy: Estratigrafia[], key: string, _case: Case) {
        const chartData: StratigraphyChart = {
            title: key,
            canvas: [],
        };
        if (_case.dadosGerais.minPm > stratigraphy[0].pm.valor) {
            _case.dadosGerais.minPm = stratigraphy[0].pm.valor;
        }

        stratigraphy.forEach((e, index) => {
            const serie = { name: e.sigla, pm: [[]], pv: [[]] };
            StratigraphyUtils.aux(stratigraphy, index, serie, 'pv', 'maxPv', _case);
            StratigraphyUtils.aux(stratigraphy, index, serie, 'pm', 'maxPm', _case);
            chartData.canvas.push(serie);
        });
        return chartData;
    }

    public static loadStratigraphies(_case: Case) {
        const estratigrafia = [];

        Object.keys(_case.estratigrafia.Itens).forEach(key => {
            estratigrafia.push(StratigraphyUtils.loadStratigraphy(_case.estratigrafia.Itens[key], key, _case));
        });

        return estratigrafia;
    }
}