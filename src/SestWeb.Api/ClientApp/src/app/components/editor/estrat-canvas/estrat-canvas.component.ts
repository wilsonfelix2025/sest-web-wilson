import { Component, Input, OnDestroy } from '@angular/core';
import { SynchronizeChartsService } from '@services/synchronize-charts.service';
import * as Highcharts from 'highcharts';
import * as HighchartsUtils from '@utils/highcharts';
import { DatasetService } from '@services/dataset/dataset.service';
import { ExportService } from 'app/repositories/export.service';
import { Grafico } from 'app/repositories/models/export';
import { StateService } from '@services/dataset/state/state.service';
import { Subscription } from 'rxjs';

declare var require: any;
require('highcharts/highcharts-more')(Highcharts);
@Component({
  selector: 'sest-estrat-canvas',
  templateUrl: './estrat-canvas.component.html',
  styleUrls: ['./estrat-canvas.component.scss']
})
export class EstratCanvasComponent implements OnDestroy {

  /**
   * Object containing information required to render the component.
   */
  @Input() stratigraphyId: string;
  @Input() caseId: string;

  title: string = '';

  /**
   * The Highcharts lib. Required to properly render the chart component.
   */
  Highcharts: typeof Highcharts = Highcharts;

  /**
   * Object containing the Highcharts option on how to render the chart.
   */
  stratigraphyOptions = Object.assign({}, HighchartsUtils.stratigraphyOptions);

  /**
   * The Highcharts chart object associated with this component.
   */
  componentChart: Highcharts.Chart;

  series;

  subscriptions: Subscription[] = [];

  constructor(
    public sync: SynchronizeChartsService,
    private stateService: StateService,
    private dataset: DatasetService,
    private exportService: ExportService,
  ) {
    this.subscriptions.push(this.stateService.deepViewChanged.subscribe(el => {
      this.setChartContent(this.series, el);
    }));

    this.subscriptions.push(this.exportService.$relatorioRequestedChart.subscribe((request) => {
      if (this.caseId === this.dataset.currCaseId) {
        if (request.estratigrafias) {
          const g: Grafico = {
            data: HighchartsUtils.exportChart(this.componentChart),
            titulo: this.title,
            curvas: [],
            registros: []
          };
          this.exportService.relatorio.estratigrafias.push(g);
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
    // Postprocess the series after retrieving it from the back-end
    this.title = this.dataset.getById(this.stratigraphyId).title;
    this.series = HighchartsUtils.postProcessStratigraphySeries(this.dataset.getById(this.stratigraphyId).canvas);
    this.setChartContent(this.series, this.stateService.currentDeepView);
  }

  setChartContent(series, deepView: 'PM' | 'PV') {
    // console.log('estrat', this.content, series);
    while (this.componentChart.series.length) {
      this.componentChart.series[0].remove();
    }
    // Add it to the chart
    series.forEach(seriesEntry => {
      if ((deepView === 'PV') === seriesEntry.deep) {
        this.componentChart.addSeries(seriesEntry, false);
      }
    });
    this.componentChart.redraw();
  }
}
