import { Component, Input, ViewChild, ChangeDetectorRef, OnInit, OnDestroy } from '@angular/core';
import { HighchartsChartComponent } from 'highcharts-angular';
import { SynchronizeChartsService } from '@services/synchronize-charts.service';
import { DropEvent } from 'angular-draggable-droppable';
import * as Highcharts from 'highcharts';
import * as HighchartsUtils from '@utils/highcharts';
import { DialogService } from '@services/dialog.service';
import { DialogEditXScaleComponent } from '../dialog-edit-x-scale/dialog-edit-x-scale.component';
import { DialogDeleteHomeComponent } from '../dialog-delete-home/dialog-delete-home.component';
import { DialogEditWidthComponent } from '../dialog-edit-width/dialog-edit-width.component';
import { NotybarService } from '@services/notybar.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { DatasetService } from '@services/dataset/dataset.service';
import { MatMenuTrigger } from '@angular/material';
import { Trend } from 'app/repositories/models/trend';
import { WindowEditTrendComponent } from '../window-edit-trend/window-edit-trend.component';
import { WindowEditDeepDataComponent } from '../window-edit-deep-data/window-edit-deep-data.component';
import { Perfil } from 'app/repositories/models/perfil';
import { EditTrendService } from '@services/edit-trend.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { ProfileUtils } from '@utils/dataset/profile';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';
import { Evento, MARCADORES, Registro } from 'app/repositories/models/registro-evento';
import { UNSET } from '@utils/vazio';
import { RegisterEventDatasetService } from '@services/dataset/register-event-dataset.service';
import { ExportService } from 'app/repositories/export.service';
import { Grafico } from 'app/repositories/models/export';
import { GraphicArea } from 'app/repositories/models/state';
import { ChartsDatasetService } from '@services/dataset/state/charts-dataset.service';
import { TabsDatasetService } from '@services/dataset/state/tabs-dataset.service';
import { StateService } from '@services/dataset/state/state.service';
import { Subscription } from 'rxjs';
import { DialogEditNameComponent } from '../dialog-edit-name/dialog-edit-name.component';
import { environment } from 'environments/environment';

/**
 * Import Highchart map module for zoom functions.
 */
const HC_map = require('highcharts/modules/map');
HC_map(Highcharts);

@Component({
  selector: 'sest-graphic-canvas',
  templateUrl: './graphic-canvas.component.html',
  styleUrls: ['./graphic-canvas.component.scss'],
})
export class GraphicCanvasComponent implements OnInit, OnDestroy {

  @Input() caseId: string;
  @Input() tabId: string;
  @Input() chartId: string;

  chart: GraphicArea;

  /**
   * The list of curves currently plotted in the canvas.
   */
  profilesList: Perfil[] = [];
  /**
   * The list os registers/events currently plotted in the canvas.
   */
  registersEventsList: (Registro | Evento)[] = [];

  /**
   * Link the local component with the Highcharts library.
   */
  @ViewChild('canvas', { static: true })
  canvas: HighchartsChartComponent;

  /**
   * Expose the Highcharts library so it can be used on the template.
   */
  Highcharts: typeof Highcharts = Highcharts;
  /**
   * The Highcharts chart instance used in this component.
   */
  componentChart: Highcharts.Chart;

  csv: SafeUrl;
  /**
   * A unidade padrão do canvas.
   */
  unidadePadrao: string = undefined;
  /**
   * Se o canvas está usando escala logaritma.
   */
  logScale: boolean = false;
  /**
   * Se o canvas está com a escala automática no eixo X.
   */
  defaultXScale: boolean = true;
  /**
   * Se o canvas contem um trend.
   */
  hasAddedTrend: string | undefined = undefined;
  canEditChart: boolean = false;
  /**
   * Se o canvas contem um trend.
   */
  trendOrLBF: 'Compactação' | 'LBF' | 'Gradiente' = 'Compactação';
  /**
   * The default options for plotting a chart on the graphic area.
   */
  plotOptions: Highcharts.Options = HighchartsUtils.graphicCanvasOptions;

  clicked: boolean = false;
  editChart: boolean = false;
  hasGPPI: boolean = false;

  @ViewChild(MatMenuTrigger, { static: false })
  contextMenu: MatMenuTrigger;
  contextMenuPosition = { x: '0px', y: '0px' };

  afterFirstLoad: boolean = true;

  subscriptions: Subscription[] = [];

  constructor(
    public sync: SynchronizeChartsService,
    private notybarService: NotybarService,
    private changeDetector: ChangeDetectorRef,
    private dialog: DialogService,
    private cdRef: ChangeDetectorRef,
    private sanitizer: DomSanitizer,

    public dataset: DatasetService,
    public stateService: StateService,
    private profileDataset: ProfileDatasetService,
    private caseDataset: CaseDatasetService,
    private registerEventDataset: RegisterEventDatasetService,
    private chartsDatasetService: ChartsDatasetService,
    private tabsDatasetService: TabsDatasetService,

    private exportService: ExportService,
    // Inicializa o serviço de edição de trend que observa eventos nos gráficos
    private editTrendService: EditTrendService,
  ) {
    this.subscriptions.push(this.stateService.deepViewChanged.subscribe((deepView) => {
      if (this.profilesList.length > 0 || this.registersEventsList.length > 0) {
        this.refreshHighcharts();
      }
    }));

    this.subscriptions.push(this.caseDataset.$caseAdded.subscribe((newCase) => {
      let chart = this.dataset.getById(this.chartId);
      if (chart && chart.items && chart.items.length > 0 && chart.items.findIndex(el => el.idPoço === newCase.id) >= 0) {
        this.loadAllLists();
        this.refreshHighcharts();
      }
    }));

    this.subscriptions.push(this.caseDataset.$caseRemoved.subscribe((id) => {
      if (this.profilesList.length > 0) {
        this.profilesList.forEach((profile, i) => {
          if (profile.idPoço === id) {
            this.removeCurve(profile.id);
          }
        });
      }
    }));

    this.subscriptions.push(this.profileDataset.$profileChanged.subscribe((profile) => {
      this.debug('profileChanged', this.chart, profile)
      if (this.profilesList.length > 0 && this.profilesList.findIndex(el => el.id === profile.id) >= 0) {
        this.loadAllLists();
        this.refreshHighcharts();
      }
    }));

    this.subscriptions.push(this.registerEventDataset.$registerEventUpdated.subscribe(() => {
      if (this.registersEventsList.length > 0) {
        this.loadAllLists();
        this.refreshHighcharts();
      }
    }));

    this.subscriptions.push(this.profileDataset.$profileRemoved.subscribe((profileId) => {
      this.profilesList.forEach((profile, i) => {
        if (profile.id === profileId) {
          this.removeCurve(profileId);
        }
      });
    }));

    this.subscriptions.push(this.exportService.$relatorioRequestedChart.subscribe((request) => {
      if (this.caseId === this.dataset.currCaseId) {
        if (request.graficos.includes(this.chartId)) {
          const g: Grafico = {
            data: HighchartsUtils.exportChart(this.componentChart),
            titulo: this.chart.name,
            curvas: this.profilesList.map(el => el.id),
            registros: this.registersEventsList.map(el => el.id)
          };
          this.exportService.relatorio.graficos.push(g);
        }
      }

    }));

    this.subscriptions.push(this.profileDataset.$trendChanged.subscribe((profileId) => {
      if (this.hasAddedTrend && this.profilesList.length > 0) {
        this.profilesList.forEach((curve) => {
          if (curve.id === profileId) {
            const perfil: Perfil = this.dataset.getById(
              profileId
            );
            curve.trend = perfil.trend;
            this.removeTrend(true);
            this.addTrend(curve.trend, curve.id, curve.idPoço, true);
            this.componentChart.redraw();
            return;
          }
        });
        this.updateEditChart();
      }
    }));

    this.subscriptions.push(this.profileDataset.$trendRemoved.subscribe((profileId) => {
      if (this.hasAddedTrend) {
        this.profilesList.forEach((curve) => {
          if (curve.id === profileId) {
            this.removeTrend();
            this.componentChart.redraw();
            return;
          }
        });
      }
    }));
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => {
      sub.unsubscribe();
    })
  }

  debug(...messages) {
    // console.log('DEBUG', messages);
  }

  ngOnInit() {
    this.debug('ngOnInit');
    this.loadAllLists();
  }

  loadAllLists() {
    this.debug('loadAllLists');
    this.chart = JSON.parse(JSON.stringify(this.dataset.getById(this.chartId)));
    this.registersEventsList = [];
    this.profilesList = [];

    if (this.chart && this.chart.items && this.chart.items.length > 0) {
      if (this.chart.intervalo !== 0 || this.chart.maxX !== 0 || this.chart.minX !== 0) {
        this.defaultXScale = false;
      }

      this.chart.items.forEach((item) => {
        if (this.dataset.hasById(item.id)) {
          const curve = this.dataset.getById(item.id);

          if (this.isRegisterEvent(curve)) {
            this.registersEventsList.unshift(curve);
          } else {
            this.profilesList.unshift(curve);
            if (item.adicionadoTrend) {
              this.hasAddedTrend = curve.id;
            }
          }
        }
      });

      if (this.profilesList.length > 0) {
        this.unidadePadrao = this.profilesList[0].grupoDeUnidades['unidadePadrão'].símbolo;
      }
    }
  }

  refreshLists() {
    this.debug('refreshLists');
    this.removeAllCurvesFromComponent();
    this.loadAllLists();
  }

  onLoad(chart: Highcharts.Chart) {
    this.debug('onLoad');
    // Assign the received chart to the components attribute
    this.componentChart = chart;

    this.afterFirstLoad = true;

    this.addBaseSerie();
    this.loadAllCharts();
  }

  refreshHighcharts() {
    this.debug('refreshHighcharts');
    this.removeAllCurvesFromHighcharts();
    this.loadAllCharts();
  }

  loadAllCharts() {
    this.debug('loadAllCharts');
    if (this.profilesList.length > 0) {
      this.profilesList.forEach(el => {
        this.addPerfilToHighcharts(el, this.stateService.currentDeepView);
      });
    }

    if (this.registersEventsList.length > 0) {
      this.registersEventsList.forEach(el => {
        this.addRegistroEventoToHighcharts(el, this.stateService.currentDeepView);
      });
    }

    if (this.hasAddedTrend) {
      const prof = this.profilesList.find(el => el.id === this.hasAddedTrend);
      this.addTrend(prof.trend, prof.id, prof.idPoço, true);
    }

    setTimeout(() => {
      this.sync.syncZoom();
      this.updateXScale(this.defaultXScale, true);
      this.getCsv();
    }, 1);

    // To prevent angular exception
    this.cdRef.detectChanges();
  }

  /**
   * Add a basic series so that the chart is not empty and avoids errors
   */
  addBaseSerie() {
    this.debug('addBaseSerie');
    this.componentChart.addSeries({
      pointStart: 1,
      data: [[0, 0]],
      name: '',
      type: 'line',
      color: 'rgba(0, 0, 0, 0)',
    });
  }

  addCurve(curve: Perfil | Registro | Evento) {
    this.debug('addCurve');
    if (this.profilesList.some(el => el.id === curve.id) || this.registersEventsList.some(el => el.id === curve.id)) {
      this.notybarService.show(
        `${curve.nome} já plotado no gráfico`,
        'warning'
      );
      return;
    }

    if (this.isRegisterEvent(curve)) {
      this.addRegistroEventoToHighcharts(curve, this.stateService.currentDeepView);
      this.registersEventsList.push(curve);
    } else {
      this.unidadePadrao = curve.grupoDeUnidades.unidadePadrão.símbolo;
      curve.name = curve.nome;
      if (curve.mnemonico === 'GPPI') {
        this.hasGPPI = true;
        this.canEditChart = curve.idPoço === this.dataset.currCaseId;
        this.updateEditChart();
      }
      this.profilesList.push(curve);
    }

    this.chartsDatasetService.addCurve(this.caseId, this.chartId, curve.id, curve['adicionadoTrend']);

    this.refreshHighcharts();
  }

  removeCurve(id: string) {
    this.debug('removeCurve');
    this.registersEventsList = this.registersEventsList.filter(el => el.id !== id);
    this.profilesList = this.profilesList.filter(el => el.id !== id);
    this.removeCurveFromHighcharts(id);

    if (this.hasAddedTrend && this.hasAddedTrend === id) {
      this.removeTrend();
      this.hasAddedTrend = undefined;
    }

    if (this.profilesList.length < 1) {
      this.unidadePadrao = undefined;
    } else if (this.hasGPPI && this.profilesList.findIndex(el => el.mnemonico === 'GPPI') < 0) {
      this.hasGPPI = false;
    }

    // Update the state
    this.chartsDatasetService.removeCurve(this.caseId, this.chartId, id);

    // Force change detection to prevent Angular error
    if (!this.changeDetector['destroyed']) {
      this.changeDetector.detectChanges();
    }
    // Espera a atualização do highchart
    setTimeout(() => {
      this.updateXScale(this.defaultXScale, true);
      this.getCsv();
    }, 1);
  }

  toggleCurve(name: string, hide: boolean) {
    this.debug('toggleCurve');
    const curve = this.componentChart.series.filter((serie) => serie.name === name);
    if (hide) { curve.forEach((serie) => { serie.hide(); }); }
    else { curve.forEach((serie) => { serie.show(); }); }
  }

  getCsv() {
    this.debug('getCsv');
    setTimeout(() => {
      this.csv = this.sanitizer.bypassSecurityTrustUrl('data:text/csv;charset=utf-8,' + this.componentChart.getCSV());
    }, 0);
  }

  perfilToTrend(perfil: Perfil) {
    this.debug('perfilToTrend');
    const trendBase = {
      id: perfil.id,
      nome: `Trend_${perfil.nome}`,
      tipoTrend: 'Gradiente',
      perfil: perfil.id,
      trechos: [],
      podeSerUsadoEmCálculo: false,
      series: [],
    } as Trend;

    for (let i = 0; i < perfil.pontos.length - 1; i++) {
      const pontoAtual = perfil.pontos[i];
      const proxPonto = perfil.pontos[i + 1];

      trendBase.trechos.push({
        valorTopo: pontoAtual.valor,
        pvTopo: pontoAtual.pv.valor,
        valorBase: proxPonto.valor,
        pvBase: proxPonto.pv.valor,
        inclinação: -1,
      });
    }

    const poco = this.dataset.getById(this.dataset.currCaseId);
    ProfileUtils.trendPointsToData(trendBase, perfil.id, poco);
    return trendBase;
  }

  /**
   * Updates the chart reference on the Highcharts library by removing the received curve.
   *
   * @param {string} id the id of the curve to remove from the chart.
   */
  removeCurveFromHighcharts(id: string) {
    this.debug('removeCurveFromHighcharts');
    if (this.componentChart && this.componentChart.series) {
      let i = this.componentChart.series.findIndex(el => el['userOptions'].id === id);
      while (i >= 0) {
        this.componentChart.series[i].remove();
        i = this.componentChart.series.findIndex(el => el['userOptions'].id === id);
      }
    }
  }

  /**
   * Wrapper function that calls more specialized functions.
   */
  removeAllCurves() {
    this.debug('removeAllCurves');
    this.hasAddedTrend = undefined;
    this.hasGPPI = false;
    this.removeAllCurvesFromComponent();
    this.removeAllCurvesFromHighcharts();

    // Update the state
    this.chartsDatasetService.removeAllCurves(this.caseId, this.chartId);

    setTimeout(() => {
      this.updateXScale(true);
      this.getCsv();
    }, 1);
  }

  /**
   * Updates the chart reference on the Highcharts library by removing all curves.
   */
  removeAllCurvesFromHighcharts() {
    if (this.componentChart && this.componentChart.series) {
      while (this.componentChart.series.length > 0) {
        this.componentChart.series[0].remove();
      }
      this.addBaseSerie();
    }
  }

  /**
   * Updates control variables and the title of the canvas with a valid information.
   */
  removeAllCurvesFromComponent() {
    this.debug('removeAllCurvesFromComponent');
    // Remove all curves and the title
    this.profilesList = [];
    this.registersEventsList = [];
    this.unidadePadrao = undefined;
    // Force change detection to prevent Angular error
    this.changeDetector.detectChanges();
  }

  /**
   * Updates the chart reference on the Highcharts library with the series being added.
   *
   * @param {perfil} profile the curve to add to the chart.
   */
  addPerfilToHighcharts(profile: Perfil, deepView: 'PM' | 'PV') {
    this.debug('addPerfilToHighcharts');
    profile['marker'] = { enabled: false };

    if (deepView === 'PV') { profile.data = profile.pv; }
    else { profile.data = profile.pm; }

    const limites = this.caseDataset.getLimiteCaso(this.dataset.currCaseId);

    const customizedCurve = profile as Highcharts.SeriesLineOptions;
    customizedCurve.color = profile.estiloVisual.corDaLinha;
    customizedCurve.dashStyle = profile.estiloVisual.estiloLinha as Highcharts.DashStyleValue;
    customizedCurve.lineWidth = profile.estiloVisual.espessura;
    customizedCurve.marker = {
      enabled: profile.estiloVisual.marcador !== 'Nenhum',
      fillColor: profile.estiloVisual.corDoMarcador,
      symbol: this.mapToHighchartsMarker(profile.estiloVisual.marcador),
    };

    if (profile.mnemonico === 'DIAM_BROCA') {
      customizedCurve['step'] = 'left';
      if (deepView === 'PV') {
        customizedCurve.data.push([limites.pvMax, profile.ultimoPonto.valor,]);
      } else {
        customizedCurve.data.push([limites.pmMax, profile.ultimoPonto.valor,]);
      }
    } else if (profile.mnemonico === 'GPPI') {
      customizedCurve.dragDrop = { draggableY: true, draggableX: true };
    }

    this.componentChart.addSeries(customizedCurve as Highcharts.SeriesLineOptions);

    // Update the title inside the chart
    this.componentChart.setTitle(
      { text: profile.nome, },
      {
        style: { display: 'none' },
        text: profile.grupoDeUnidades.unidadePadrão.símbolo !== '-' ? profile.grupoDeUnidades.unidadePadrão.símbolo : null,
      }
    );
  }

  addRegistroEventoToHighcharts(registerEvent: Registro | Evento, deepView: 'PM' | 'PV') {
    this.debug('addRegistroEventoToHighcharts');
    let dots = [];
    if (registerEvent.tipo === 'Evento') {
      registerEvent.trechos.sort((a, b) => a.topo.pm.valor - b.topo.pm.valor);
      const series = [];
      registerEvent.trechos.forEach(el => {
        if (el.topo.pm.valor === el.base.pm.valor) {
          dots.push({ x: el.topo.pm.valor, y: registerEvent.valorPadrão, description: el.comentário });
        } else {
          const serie = {
            type: 'line',
            data: [
              { x: el.topo.pm.valor, y: registerEvent.valorPadrão, description: el.comentário },
              { x: el.base.pm.valor, y: registerEvent.valorPadrão, description: el.comentário }
            ],
            id: registerEvent.id,
            marker: {
              enabled: true,
              lineColor: registerEvent.estiloVisual.contornoDoMarcador,
              lineWidth: 2,
              fillColor: registerEvent.estiloVisual.corDoMarcador,
              symbol: this.mapToHighchartsMarker(registerEvent.estiloVisual.marcador),
              radius: 5,
              width: 20,
              height: 20
            },
            color: registerEvent.estiloVisual.contornoDoMarcador,
            states: {
              hover: {
                enabled: false,
              },
            },
            name: registerEvent.nome,
          } as Highcharts.SeriesLineOptions;
          series.push(serie);
        }
      });
      series.forEach(serie => {
        this.componentChart.addSeries(serie as Highcharts.SeriesLineOptions);
      });
    } else {
      registerEvent.trechos.sort((a, b) => a.ponto.pm.valor - b.ponto.pm.valor);
      if (deepView === 'PV') {
        dots = registerEvent.trechos.map(el => { return { x: el.ponto.pv.valor, y: el.valor, description: el.comentário } });
      } else {
        dots = registerEvent.trechos.map(el => { return { x: el.ponto.pm.valor, y: el.valor, description: el.comentário } });
      }
    }
    if (dots.length > 0) {
      const serie = {
        type: 'line',
        data: dots,
        id: registerEvent.id,
        marker: {
          enabled: true,
          lineColor: registerEvent.estiloVisual.contornoDoMarcador,
          lineWidth: 2,
          fillColor: registerEvent.estiloVisual.corDoMarcador,
          symbol: this.mapToHighchartsMarker(registerEvent.estiloVisual.marcador),
          radius: 5,
          width: 20,
          height: 20
        },
        color: `rgba(255,255,255,0)`,
        states: {
          hover: {
            enabled: false,
          },
        },
        name: registerEvent.nome,
      } as Highcharts.SeriesLineOptions;

      this.componentChart.addSeries(serie);
    }
  }


  openHowToDialog() {
    this.debug('openHowToDialog');
    let title = '', imagePath;

    this.clicked = true;

    setTimeout(() => (this.clicked = false), 300);

    if (this.hasGPPI) {
      title = 'Edição de GPPI';
      imagePath = `${environment.appUrl}/assets/images/instrucoes-gppi.jpg`;
    } else {
      if (this.trendOrLBF === 'LBF') {
        title = 'Edição de LBF';
        imagePath = `${environment.appUrl}/assets/images/instrucoes-lbf.jpg`;
      } else {
        title = 'Edição de Trend';
        imagePath = `${environment.appUrl}/assets/images/instrucoes-trend.jpg`;
      }
    }
    this.dialog.openImageDialog({ title: title, imagePath: imagePath });
  }

  addTrend(trend: Trend, idPerfil: string, idCase: string, withoutSave?: boolean) {
    this.debug('addTrend');
    const poco = this.dataset.getById(idCase);

    // console.log('adicionaremos aq', trend);
    ProfileUtils.trendPointsToData(trend, idPerfil, poco);
    trend.series.forEach((serie, index) => {
      const _trend = serie as Highcharts.SeriesLineOptions;
      this.componentChart.addSeries(_trend as Highcharts.SeriesLineOptions);
    });

    const curve = this.profilesList.find((el) => el.id === idPerfil);
    this.trendOrLBF = trend.tipoTrend;

    this.toggleCurve('Trend', this.stateService.currentDeepView === 'PM');
    this.toggleCurve('LBF', this.stateService.currentDeepView === 'PV');

    if (!withoutSave) {
      this.chartsDatasetService.addTrend(this.caseId, this.chartId, curve.id);
    }

    this.hasAddedTrend = idPerfil;
    this.canEditChart = idCase === this.dataset.currCaseId;
    this.updateEditChart();
  }

  removeTrend(withoutSave?: boolean) {
    this.debug('removeTrend');
    const series = this.componentChart.series.filter((el) => el.name === 'Trend' || el.name === 'LBF');
    series.forEach((curve) => { curve.remove(); });

    if (withoutSave) {
      return;
    }
    if (this.profilesList[0]) {
      this.chartsDatasetService.removeTrend(this.caseId, this.chartId);
    }
    this.hasAddedTrend = undefined;
  }

  /**
   * Converts the enum value received from the back-end to the Highcharts format.
   *
   * @param markerName the string representing the marker, as received from the API.
   */
  mapToHighchartsMarker(markerName: string) {
    this.debug('mapToHighchartsMarker');
    if (markerName === 'Nenhum') {
      return '';
    } else if (markerName === 'Circulo') {
      return 'circle';
    } else if (markerName === 'Quadrado') {
      return 'square';
    } else if (markerName === 'Diamante') {
      return 'diamond';
    } else if (markerName === 'Triangulo') {
      return 'triangle';
    } else if (markerName === 'TrianguloInvertido' || markerName === 'Triangulo-invertido') {
      return 'triangle-down';
    } else if (MARCADORES.some(el => el.value === markerName)) {
      return `url(${MARCADORES.find(el => el.value === markerName).avatar})`
    }
    console.warn(`Nome inválido de marcador recebido: '${markerName}'.`);
    return '';
  }

  openDialogRemoveChart() {
    this.dialog.openDialogGeneric(DialogDeleteHomeComponent, this, 'removeChart', {});
  }

  removeChart() {
    this.tabsDatasetService.removeChart(this.chartId, this.tabId, this.caseId);
  }

  openDialogChangeXScale() {
    this.dialog.openDialogGeneric(DialogEditXScaleComponent, this, 'changeXScale',
      {
        minX: Math.trunc(this.chart.minX * 100) / 100,
        maxX: Math.ceil(this.chart.maxX * 100) / 100,
        intervalo: Math.ceil(this.chart.intervalo * 100) / 100,
      }
    );
  }

  changeXScale(parameters: { minX: number; maxX: number; intervalo: number }) {
    console.log('changeXScale')
    this.chart.minX = parameters.minX;
    this.chart.maxX = parameters.maxX;
    this.chart.intervalo = isNaN(parameters.intervalo) ? (this.chart.maxX - this.chart.minX) / 5 : parameters.intervalo;

    this.defaultXScale = false;
    this.updateXScale(this.defaultXScale);
  }

  openDialogEditWidth() {
    this.dialog.openDialogGeneric(DialogEditWidthComponent, this, 'changeChartWidth', { width: this.chart.largura });
  }

  changeChartWidth(parameters: { width: number }) {
    this.chart.largura = parameters.width;
    this.chartsDatasetService.updateProperties(this.caseId, this.chart);
  }

  openDialogEditName() {
    this.dialog.openDialogGeneric(DialogEditNameComponent, this, 'changeName', { name: this.chart.name });
  }

  changeName(parameters: { name: string }) {
    this.chart.name = parameters.name;
    this.chartsDatasetService.updateProperties(this.caseId, this.chart);
  }

  getDefaultXScale() {
    this.debug('getDefaultXScale');
    let min, max;
    if (this.componentChart.series && this.componentChart.series.length > 1) {
      // Get the minimum and maximum of all series
      min = this.componentChart.series[1]['dataMin'];
      max = this.componentChart.series[1]['dataMax'];
      for (let i = 2; i < this.componentChart.series.length; i++) {
        if (this.componentChart.series[i]['dataMin'] < min) {
          min = this.componentChart.series[i]['dataMin'];
        }
        if (this.componentChart.series[i]['dataMax'] > max) {
          max = this.componentChart.series[i]['dataMax'];
        }
      }
      const margin = (max - min) * HighchartsUtils.chartMarginX;
      min -= margin;
      max += margin;
    } else {
      min = 0;
      max = 0;
    }
    return { intervalo: (max - min) / 5, minX: min, maxX: max };
  }

  updateXScale(toDefault: boolean, withoutSave?: boolean, changeLog?: boolean) {
    this.debug('updateXScale');
    if (toDefault) {
      this.defaultXScale = true;
      this.chart.intervalo = 0;
      this.chart.maxX = 0;
      this.chart.minX = 0;
    } else {
      if (this.chart.intervalo > this.chart.maxX - this.chart.minX) {
        this.chart.intervalo = 0;
      }
      if (this.logScale && this.chart.minX <= 0) {
        this.chart.minX = 1;
      }
    }

    if (!withoutSave) {
      this.chartsDatasetService.updateProperties(this.caseId, this.chart);
    }

    setTimeout(() => {
      // Force change detection to prevent Angular error
      this.changeDetector.detectChanges();
      // Reflow the chart to render curves correctly
      this.componentChart.reflow();
      if (this.componentChart && this.componentChart.yAxis && this.componentChart.yAxis.length > 0) {

        this.componentChart.yAxis[0].options.gridLineWidth = 1;
        let p: { intervalo: number, minX: number, maxX: number } = this.chart;
        if (this.defaultXScale) {
          p = this.getDefaultXScale();
          this.componentChart.yAxis[0].options.gridLineWidth = 0;
        }
        this.to2Decimal(p);
        this.componentChart.yAxis[0].update({ tickInterval: p.intervalo });
        this.componentChart.yAxis[0].setExtremes(p.minX, p.maxX, true, false, { trigger: 'syncExtremes', });
        if (changeLog) {
          if (this.logScale) { this.componentChart.yAxis[0].update({ type: 'logarithmic' }); }
          else { this.componentChart.yAxis[0].update({ type: 'linear' }); }
        }
      }
    }, 0);
  }

  to2Decimal(p: { intervalo: number, minX: number, maxX: number }) {
    p.minX = Math.trunc(p.minX * 100) / 100;
    p.maxX = Math.ceil(p.maxX * 100) / 100;
    p.intervalo = Math.ceil(p.intervalo * 100) / 100;
  }

  /**
   * Change chart scale between linear and logarithmic
   */
  changeScale() {
    this.debug('changeScale');
    this.logScale = !this.logScale;
    this.updateXScale(this.defaultXScale, true, true);
  }

  exportChart(type: Highcharts.ExportingMimeTypeValue) {
    this.debug('exportChart');
    this.componentChart.exportChartLocal(
      {
        type: type,
        filename: this.chart.name,
        sourceWidth: this.componentChart.plotWidth * 10,
        sourceHeight: this.componentChart.plotHeight * 10,
      },
      {
        legend: { enabled: true },
      }
    );
  }

  /**
   * Put a curve on top of the chart.
   *
   * @param {number} index the index of the curve in the chart.
   */
  changeToTop(profile: Perfil) {
    const index = this.profilesList.findIndex(el => el.id === profile.id);

    const curve: Perfil = this.profilesList.splice(index, 1)[0];
    this.profilesList.push(curve);

    this.chartsDatasetService.putCurveOnTop(this.caseId, this.chartId, curve.id);

    this.refreshHighcharts();
  }

  /**
   * Callback called when the user drops a draggable element on the canvas.
   *
   * @param {DropEvent} e the object containing the event information.
   */
  onDropCurve(e: DropEvent) {
    this.debug('onDropCurve', e);
    if (this.caseId !== this.dataset.currCaseId) {
      this.notybarService.show(
        'Só possível plotar curvas no caso atual.',
        'warning'
      );
      return;
    }

    // Extract the relevant part from the event object
    const curveInfo = e.dropData;

    // Se dropou um registro / evento
    if (this.isRegisterEvent(curveInfo)) {
      this.addCurve(curveInfo);
      return;
    }

    // Se dropou um trend
    if (curveInfo.addTrend) {
      if (this.hasAddedTrend) {
        if (curveInfo.perfil.trend.tipoTrend === 'Compactação') {
          this.notybarService.show('Já existe um trend nesse gráfico.', 'warning');
        } else {
          this.notybarService.show('Já existe uma LBF nesse gráfico.', 'warning');
        }
        return;
      } else if (curveInfo.perfil.trend.tipoTrend === 'Compactação' && this.stateService.currentDeepView === 'PM') {
        this.notybarService.show('Trend só pode ser plotado em PV.', 'warning');
        return;
      } else if (curveInfo.perfil.trend.tipoTrend === 'LBF' && this.stateService.currentDeepView === 'PV') {
        this.notybarService.show('LBF só pode ser plotado em PM.', 'warning');
        return;
      }

      if (this.profilesList.length < 1 || this.profilesList.findIndex(el => el.id === curveInfo.perfil.id) < 0) {
        this.addCurve(curveInfo.perfil);
      }
      this.addTrend(curveInfo.perfil.trend, curveInfo.perfil.id, curveInfo.perfil.idPoço);
      return;
    }

    // Se a curva estiver vazia
    if (curveInfo.count === 0) {
      this.notybarService.show('Este perfil está vazio.', 'warning');
      return;
    }

    // Se a unidade da curva for diferente da unidade da curva no grafico
    if (this.unidadePadrao !== undefined && curveInfo.grupoDeUnidades.unidadePadrão.símbolo !== this.unidadePadrao) {
      this.notybarService.show('Este perfil possui uma unidade de medida incompatível.', 'warning');
      return;
    }

    // Add the curve to the canvas
    this.addCurve(curveInfo);
  }

  /**
   * Preprocessor function that appends the Highcharts index of the last
   * series added to the chart.
   *
   * @param {*} e the object containing event information.
   */
  synchronize(e) {
    // If there's at least one curve on the chart
    if (this.profilesList.length > 0 || this.registersEventsList.length > 0) {
      // Create a new key with the index of the curve on the Highcharts chart object
      e['lastSeriesIndex'] = 0;
      // Proceed to call the SyncService synchronize() passing the new event object
      this.sync.synchronize(e);
    }
  }

  onContextMenu(event: MouseEvent) {
    this.debug('onContextMenu');
    event.preventDefault();
    this.contextMenuPosition.x = event.clientX + 'px';
    this.contextMenuPosition.y = event.clientY + 'px';
    // this.contextMenu.menuData = { 'node': node };

    this.contextMenu.menu.focusFirstItem('mouse');
    this.contextMenu.openMenu();
  }

  openWindowEditProfile(item) {
    // console.log(item);
    this.dialog.openPageDialog(
      WindowEditDeepDataComponent,
      { minHeight: 450, minWidth: 400 },
      { tipo: 'Perfil', id: item.id, caseId: this.caseId }
    );
  }

  openDialogEditTrend(item) {
    this.dialog.openPageDialog(
      WindowEditTrendComponent,
      { minHeight: 450, minWidth: 450 },
      { idPerfil: item.id, caseId: this.caseId }
    );
  }

  openWindowLegend() {
    this.dialog.openLegendDialog(
      { perfis: this.profilesList, registrosEventos: this.registersEventsList }
    );
  }

  resetZoom() {
    this.debug('resetZoom');
    this.sync.resetZoom();
  }

  isRegisterEvent(curve): curve is Registro | Evento {
    return !UNSET(curve.tipo) && (curve.tipo === 'Registro' || curve.tipo === 'Evento');
  }

  updateEditChart() {
    this.debug('updateEditChart');
    this.componentChart = this.canvas['chart'];
    this.componentChart.update({ chart: { zoomType: this.editChart ? undefined : 'x' } });

    const series = this.componentChart.series.filter((el) => el.name === 'Trend' || el.name === 'LBF' || el['userOptions'].mnemonico === 'GPPI');

    series.forEach((curve) => {
      curve.update({
        type: 'line',
        dragDrop: { draggableX: this.editChart, draggableY: this.editChart },
        states: { hover: { enabled: this.editChart } }
      });
    });
  }

  toggleEditChart() {
    this.editChart = !this.editChart;
    this.clicked = true;

    setTimeout(() => (this.clicked = false), 300);

    this.updateEditChart();
  }
}
