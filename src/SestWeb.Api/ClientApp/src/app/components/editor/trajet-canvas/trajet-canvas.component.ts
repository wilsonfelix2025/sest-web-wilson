import { Component, Input, Output, EventEmitter, OnDestroy } from '@angular/core';
import * as Highcharts from 'highcharts';
import * as HighchartsUtils from '@utils/highcharts';
import { SynchronizeChartsService } from '@services/synchronize-charts.service';
import { SafeUrl, DomSanitizer } from '@angular/platform-browser';
import { DialogEditWidthComponent } from '../dialog-edit-width/dialog-edit-width.component';
import { DialogService } from '@services/dialog.service';
import { TrajectoryUtils } from '@utils/dataset/trajectory';
import { TrajectoryDatasetService } from '@services/dataset/trajectory-dataset.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { ExportService } from 'app/repositories/export.service';
import { Grafico } from 'app/repositories/models/export';
import { StateService } from '@services/dataset/state/state.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'sest-trajet-canvas',
  templateUrl: './trajet-canvas.component.html',
  styleUrls: ['./trajet-canvas.component.scss']
})
export class TrajetCanvasComponent implements OnDestroy {

  /**
   * Object containing information required to render the component.
   */
  @Input() caseId: string;
  @Input() width: number = 0;
  @Output() widthChange = new EventEmitter<number>();

  /**
   * The Highcharts lib. Required to properly render the chart component.
   */
  Highcharts: typeof Highcharts = Highcharts;

  /**
   * Object containing the Highcharts option on how to render the chart.
   */
  trajectoryOptions = HighchartsUtils.trajectoryOptions;

  /**
   * The Highcharts chart object associated with this component.
   */
  componentChart: Highcharts.Chart;

  csv: SafeUrl;

  subscriptions: Subscription[] = [];

  constructor(public sync: SynchronizeChartsService,
    private dialog: DialogService,
    private sanitizer: DomSanitizer,

    private stateService: StateService,
    private dataset: DatasetService,
    private trajetoryDataset: TrajectoryDatasetService,
    private exportService: ExportService,
  ) {
    this.subscriptions.push(this.trajetoryDataset.$trajectoryChanged.subscribe(traj => {
      this.getCurrTraj();
    }));

    this.subscriptions.push(this.stateService.deepViewChanged.subscribe((deepView) => {
      this.setDeepView(deepView);
    }));

    this.subscriptions.push(this.exportService.$relatorioRequestedChart.subscribe((request) => {
      if (this.caseId === this.dataset.currCaseId) {
        if (request.trajetoria) {
          const g: Grafico = {
            data: HighchartsUtils.exportChart(this.componentChart),
            titulo: "Trajetória",
            curvas: [],
            registros: []
          };
          this.exportService.relatorio.trajetoria = g;
        }
      }
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => {
      sub.unsubscribe();
    })
  }

  getCurrTraj() {
    const traj = this.trajetoryDataset.get(this.caseId);
    const currCase = this.dataset.getById(this.caseId);

    this.sync.setsGlobalDepth(traj.primeiroPonto.pm.valor, traj.últimoPonto.pm.valor);
    const formattedTrajectory = TrajectoryUtils.loadTrajetoria(currCase, traj);
    this.updateChartContent(formattedTrajectory, this.stateService.currentDeepView);
  }

  /**
   * Callback called when a chart is loaded.
   *
   * @param {Chart} chart a reference to the Highcharts' chart object.
   */
  onLoad(chart: Highcharts.Chart) {
    // Assign the received chart to the components attribute
    this.componentChart = chart;

    this.getCurrTraj();

    this.getCsv();
  }

  setDeepView(deepView: 'PV' | 'PM') {
    if (deepView === 'PV') {
      this.componentChart.series[0].show();
      this.componentChart.series[1].show();
      this.componentChart.series[2].hide();
      this.componentChart.series[3].hide();
    } else {
      this.componentChart.series[0].hide();
      this.componentChart.series[1].hide();
      this.componentChart.series[2].show();
      this.componentChart.series[3].show();
    }
  }

  updateChartContent(content, deepView) {
    if (this.componentChart === undefined) {
      return;
    }
    while (this.componentChart.series.length) {
      this.componentChart.series[0].remove();
    }

    this.componentChart.redraw();
    this.componentChart.reflow();

    content.forEach(serie => {
      this.componentChart.addSeries(serie, false);
    });

    setTimeout(() => {
      this.componentChart.redraw();
      this.componentChart.reflow();

      this.setDeepView(deepView);
    }, 100);
  }

  hideDiameters() {
    this.componentChart.series[1]['userOptions'].label = !this.componentChart.series[1]['userOptions'].label;
    this.componentChart.series[1].update({ type: 'line', dataLabels: { nullFormat: '' } });
  }

  exportChart(type: Highcharts.ExportingMimeTypeValue) {
    this.componentChart.exportChartLocal({
      type: type,
      filename: 'trajetoria',
      sourceWidth: this.componentChart.plotWidth * 10,
      sourceHeight: this.componentChart.plotHeight * 10
    }, {});
  }

  getCsv() {
    setTimeout(() => {
      this.csv = this.sanitizer.bypassSecurityTrustUrl('data:text/csv;charset=utf-8,' + this.componentChart.getCSV());
    }, 0);
  }

  openDialogEditWidth() {
    this.dialog.openDialogGeneric(DialogEditWidthComponent, this, 'changeChartWidth', { width: this.width });
  }

  changeChartWidth(parameters: { width: number }) {
    this.width = parameters.width;
    this.widthChange.emit(this.width);
    setTimeout(() => {
      HighchartsUtils.reflowCharts();
    }, 10);
  }
}
