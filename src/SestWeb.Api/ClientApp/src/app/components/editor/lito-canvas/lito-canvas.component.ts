import { Component, Input, OnDestroy } from '@angular/core';
import * as Highcharts from 'highcharts';
import * as HighchartsUtils from '@utils/highcharts';
import { SynchronizeChartsService } from '@services/synchronize-charts.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { LithologyDatasetService } from '@services/dataset/lithology-dataset.service';
import { ExportService } from 'app/repositories/export.service';
import { Grafico } from 'app/repositories/models/export';
import { Case } from 'app/repositories/models/case';
import { StateService } from '@services/dataset/state/state.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'sest-lito-canvas',
  templateUrl: './lito-canvas.component.html',
  styleUrls: ['./lito-canvas.component.scss']
})
export class LitoCanvasComponent implements OnDestroy {

  /**
   * Object containing information required to render the component.
   */
  @Input() lithologyId: string;
  @Input() caseId: string;

  title: string = '';

  /**
   * The Highcharts lib. Required to properly render the chart component.
   */
  Highcharts: typeof Highcharts = Highcharts;

  /**
   * Object containing the Highcharts option on how to render the chart.
   */
  lithologyOptions = HighchartsUtils.lithologyOptions;

  /**
   * The Highcharts chart object associated with this component.
   */
  componentChart: Highcharts.Chart;

  subscriptions: Subscription[] = [];

  constructor(
    public sync: SynchronizeChartsService,

    private stateService: StateService,
    private dataset: DatasetService,
    private lithologyDataset: LithologyDatasetService,
    private exportService: ExportService,
  ) {
    this.subscriptions.push(this.lithologyDataset.$lithologyChanged.subscribe(lithology => {
      if (lithology.id === this.lithologyId) {
        this.title = this.dataset.getById(this.lithologyId).title;
        this.setChartContent(this.dataset.getById(lithology.id).canvas, this.stateService.currentDeepView);
      }
    }));
    this.subscriptions.push(this.stateService.deepViewChanged.subscribe(el => {
      this.setChartContent(this.dataset.getById(this.lithologyId).canvas, el);
    }));

    this.subscriptions.push(this.exportService.$relatorioRequestedChart.subscribe((request) => {
      const caso: Case = this.dataset.getById(this.dataset.currCaseId);
      if (this.caseId === this.dataset.currCaseId) {
        if ((caso.tipoPoço === 'Projeto' && this.title === 'Adaptada') ||
          (caso.tipoPoço === 'Monitoramento' && this.title === 'Interpretada') ||
          (caso.tipoPoço === 'Retroanalise' && this.title === 'Interpretada')) {
          if (request.litologia) {
            const g: Grafico = {
              data: HighchartsUtils.exportChart(this.componentChart),
              titulo: this.title,
              curvas: [],
              registros: []
            };
            this.exportService.relatorio.litologia = g;
          }
        }
      }
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => {
      sub.unsubscribe();
    })
  }

  /**
   * Callback called when a chart is loaded.
   *
   * @param {Chart} chart a reference to the Highcharts' chart object.
   */
  onLoad(chart: Highcharts.Chart) {
    // Assign the received chart to the components attribute
    this.componentChart = chart;
    // Fetch the chart series from the input
    const series = this.dataset.getById(this.lithologyId).canvas;
    this.title = this.dataset.getById(this.lithologyId).title;
    this.setChartContent(series, this.stateService.currentDeepView);
  }

  setChartContent(canvasContent, deepView: 'PM' | 'PV') {
    while (this.componentChart.series.length) {
      this.componentChart.series[0].remove();
    }
    // For every entry, add it to the component
    canvasContent.forEach(seriesEntry => {
      if ((deepView === 'PV') === seriesEntry.deep) {
        this.componentChart.addSeries(seriesEntry, false);
      }
    });

    // console.log('lito', canvasContent);

    setTimeout(() => {
      this.componentChart.redraw();
    }, 100);
  }
}
