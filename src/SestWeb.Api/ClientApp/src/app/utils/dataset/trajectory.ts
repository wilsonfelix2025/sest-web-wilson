import { MathUtils } from '../math';
import { Trend } from 'app/repositories/models/trend';
import { Case } from 'app/repositories/models/case';
import {
  Objetivo,
  Sapata,
  Trajetoria,
} from 'app/repositories/models/trajetoria';

export class TrajectoryUtils {

  /**
   * Atualiza a trajétoria atual.
   *
   * @param poco poco que contém a nova trajetoria a ser exibida.
   */
  public static loadTrajetoria(
    poco: Case,
    novaTrajetoria: Trajetoria
  ) {
    const pontosChave = [];
    poco.sapatas.forEach((p) => {
      pontosChave.push(TrajectoryUtils.createSapataPoint(p));
    });
    poco.objetivos.forEach((p) => {
      pontosChave.push(TrajectoryUtils.createObjetivoPoint(p));
    });
    pontosChave.sort((a, b) => a[0] - b[0]);

    const baseTrajetoria = TrajectoryUtils.getTrajetóriaBase();
    novaTrajetoria.pontos.forEach((ponto, index) => {
      // Adiciona o ponto projetado na trajétoria do highcharts
      // baseTrajetoria[1].data.push({ x: ponto.pv.valor, y: ponto.pvProj, azimute: ponto.azimute, inclinacao: ponto.inclinação });
      // baseTrajetoria[3].data.push({ x: ponto.pm.valor, y: ponto.pmProj, azimute: ponto.azimute, inclinacao: ponto.inclinação });
      baseTrajetoria[1].data.push([ponto.pv.valor, ponto.pvProj, {},
      { azimute: ponto.azimute, inclinacao: ponto.inclinação, ponto: Math.trunc(ponto.pm.valor) }]);
      baseTrajetoria[3].data.push([ponto.pm.valor, ponto.pmProj, {},
      { azimute: ponto.azimute, inclinacao: ponto.inclinação, ponto: Math.trunc(ponto.pv.valor) }]);

      // Se tiver ponto chave e o menor ponto chave for menor ou igual ao ponto atual
      while (pontosChave.length > 0 && pontosChave[0][0] <= ponto.pv.valor) {
        const pontoChavePv = pontosChave.splice(0, 1)[0];
        const pontoChavePm = [].concat(pontoChavePv);
        // Altera o X para ficar na posição correta no highcharts
        if (pontoChavePv[0] === ponto.pv.valor) {
          pontoChavePv[1] = ponto.pvProj;
        } else {
          const p0 = baseTrajetoria[1].data[index - 1];
          pontoChavePv[1] = MathUtils.linearInterpolate(
            p0[1],
            p0[0],
            ponto.pvProj,
            ponto.pv.valor,
            pontoChavePv[0]
          );
        }
        baseTrajetoria[0].data.push(pontoChavePv);
        baseTrajetoria[2].data.push(pontoChavePm);
      }
    });

    return baseTrajetoria;
  }

  private static getTrajetóriaBase() {
    return [
      {
        name: 'Sapatas e Objetivos', // PV
        data: [],
        lineWidth: 0,
        dataLabels: {
          align: 'left',
          verticalAlign: 'middle',
          enabled: true,
          style: {
            fontWeight: 'light',
          },
        },
        label: true,
        tooltip: {
          pointFormatter: function () {
            return `PV = ${this.x}<br/>${this.dado}`;
          },
        },
      },
      {
        name: 'Trajetória', // PV
        deep: 'PV',
        data: [],
        marker: { enabled: false },
        pointStart: 0,
        pointEnd: 4000,
        color: '#187BC7',
        enableMouseTracking: false,
      },
      {
        name: 'Sapatas e Objetivos', // PM
        data: [],
        lineWidth: 0,
        dataLabels: {
          align: 'left',
          verticalAlign: 'middle',
          enabled: true,
          style: {
            fontWeight: 'light',
          },
        },
        label: true,
        tooltip: {
          pointFormatter: function () {
            return `PV = ${this.x}<br/>${this.dado}`;
          },
        },
      },
      {
        name: 'Trajetória', // PM
        deep: 'PM',
        data: [],
        marker: { enabled: false },
        pointStart: 0,
        pointEnd: 4000,
        color: '#187BC7',
        enableMouseTracking: false,
      },
    ];
  }

  /**
   * Criar um ponto chave de sapata, com simbolo adequado e label configurada.
   *
   * @param p ponto na qual a sapata se encontra
   */
  private static createSapataPoint(p: Sapata) {
    const markerConfig = {
      enabled: true,
      symbol: 'url(assets/images/icons/sapata.png)',
      width: 16,
      height: 40,
    };
    // Informações do ponto que serão mostradas além do PV
    const data = `PM = ${p.pm}<br/>Diâmetro = ${p.diâmetro}''`;
    const dataLabel = {
      x: 16,
      formatter: function () {
        return this.series.userOptions.label ? `${p.diâmetro}''` : '';
      },
    };

    return [p.pv, 0, markerConfig, data, dataLabel];
  }

  /**
   * Criar um ponto chave de objetivo, com simbolo adequado e label configurada.
   *
   * @param p ponto no qual o objetivo se encontra
   */
  private static createObjetivoPoint(p: Objetivo) {
    const markerConfig = {
      enabled: true,
      radius: 5,
      fillColor: 'transparent',
      lineWidth: 2,
      lineColor: 'blue',
      symbol: 'circle',
    };
    // Informações do ponto que serão mostradas além do PV
    const data = `PM = ${p.pm}<br/>Tipo = ${p.tipoObjetivo}`;
    const dataLabel = { enabled: false };

    return [p.pv, 0, markerConfig, data, dataLabel];
  }
}
