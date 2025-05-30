import { Component, EventEmitter, Input, OnChanges, OnInit, Output, ViewChild } from '@angular/core';
import { HighchartsChartComponent } from 'highcharts-angular';
import * as Highcharts from 'highcharts';
import * as HighchartsUtils from '@utils/highcharts';
import { Perfil } from 'app/repositories/models/perfil';
import { DatasetService } from '@services/dataset/dataset.service';
import { Subject, Subscription } from 'rxjs';

@Component({
  selector: 'app-composition-chart',
  templateUrl: './composition-chart.component.html',
  styleUrls: ['./composition-chart.component.scss']
})
export class CompositionChartComponent implements OnInit, OnChanges {

  @Input() todosPocos: boolean = false;
  @Input() perfil: Perfil;
  @Input() pmRange: { min: number, max: number };
  @Input() profundidade: boolean = false;
  @Input() trechos: { pmTopo: string, pmBase: string }[] = [];

  @Output() trecho = new EventEmitter<{ pmTopo: number, pmBase: number }>();

  /**
  * The Highcharts lib. Required to properly render the chart component.
  */
  Highcharts: typeof Highcharts = Highcharts;

  /**
   * The Highcharts chart instance used in this component.
   */
  componentChart: Highcharts.Chart;

  /**
   * The default options for plotting a chart on the graphic list.
   */
  plotOptions: Highcharts.Options = HighchartsUtils.compositionCanvasOptions;

  /**
  * Object containing the Highcharts option on how to render the chart.
  */

  @ViewChild('canvas', { static: true }) canvas: HighchartsChartComponent;

  /**
  * A unidade padrão do canvas.
  */
  unidadePadrao: string = undefined;

  plotBands: Highcharts.PlotLineOrBand[] = [];

  $observable: Subscription;

  nomePoco: string = undefined;

  constructor(
    private dataset: DatasetService,
    ) { }

  ngOnInit() {
  }

  ngAfterViewInit() {
    this.updateChart();
    this.$observable = HighchartsUtils.selectionObservable.subscribe(event => {
      if (event.point === this.componentChart) {
        const ret = { pmTopo: Number.parseInt(event.event.xAxis[0].min), pmBase: Number.parseInt(event.event.xAxis[0].max) };
        if (this.perfil.pmMínimo.valor > ret.pmTopo) {
          ret.pmTopo = this.perfil.pmMínimo.valor;
        }
        if (this.perfil.pmMáximo.valor < ret.pmBase) {
          ret.pmBase = this.perfil.pmMáximo.valor;
        }
        if (ret.pmBase < ret.pmTopo) {
          ret.pmBase = ret.pmTopo;
        }
        this.trecho.emit(ret);
      }
    });
  }

  updateChart() {
    this.componentChart = this.canvas['chart'];

    let curve = this.perfil;
    curve.data = curve.pm;

    const poco = this.dataset.getById(this.dataset.currCaseId);

    this.nomePoco = this.dataset.getById(curve.idPoço).nome;

    const customizedCurve = curve as Highcharts.SeriesLineOptions;
    customizedCurve.color = curve.estiloVisual.corDaLinha;
    customizedCurve.dashStyle = curve.estiloVisual
      .estiloLinha as Highcharts.DashStyleValue;
    customizedCurve.lineWidth = curve.estiloVisual.espessura;
    customizedCurve.marker = {
      enabled: curve.estiloVisual.marcador !== 'Nenhum',
      fillColor: curve.estiloVisual.corDoMarcador,
      symbol: 'circle',
    };

    if (curve.mnemonico === 'DIAM_BROCA') {
      customizedCurve['step'] = 'left';
      customizedCurve.data.push([
        poco.trajetória.últimoPonto.pm.valor,
        curve.ultimoPonto.valor,
      ]);
    }

    if (this.componentChart) {
      this.componentChart.addSeries(
        customizedCurve as Highcharts.SeriesLineOptions
      );

      this.componentChart.yAxis[0].setExtremes();
      this.componentChart.xAxis[0].setExtremes(this.pmRange.min, this.pmRange.max);
      this.componentChart.redraw();
      if (this.profundidade) {
        this.componentChart.xAxis[0].setTitle({ text: 'Profundidade(m)' }, true);
      }
    }
  }

  atualizarTrechos() {
    if (this.trechos && this.componentChart) {
      while (this.plotBands.length > 0) {
        this.plotBands.shift().destroy();
      }
      this.trechos.forEach(trecho => {
        this.plotBands.push(
          this.componentChart.xAxis[0].addPlotBand({
            borderWidth: 2,
            borderColor: '#aaaa',
            from: Number(trecho.pmTopo),
            to: Number(trecho.pmBase)
          })
        );
      });
      this.componentChart.redraw();
    }

  }

  ngOnChanges(changes) {
    if (changes.trechos) {
      this.atualizarTrechos();
    }
    if (changes.pmRange || changes.perfil) {
      this.updateChart();
    } else {
      this.componentChart.yAxis[0].setExtremes();
      this.componentChart.xAxis[0].setExtremes(this.pmRange.min, this.pmRange.max);
      this.componentChart.redraw();
    }
  }
}
