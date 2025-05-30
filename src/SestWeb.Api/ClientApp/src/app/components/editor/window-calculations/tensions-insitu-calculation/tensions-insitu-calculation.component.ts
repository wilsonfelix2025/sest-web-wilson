import { Component, DoCheck, Inject, OnInit, ViewChild } from '@angular/core';
import * as Highcharts from 'highcharts';
import * as HighchartsUtils from '@utils/highcharts';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { DialogService } from '@services/dialog.service';
import { TableOptions } from '@utils/table-options-default';
import { Case } from 'app/repositories/models/case';
import { DatasetService } from '@services/dataset/dataset.service';
import { Perfil } from 'app/repositories/models/perfil';
import { CalculoService } from 'app/repositories/calculo.service';
import { CalcularGraficoTensoes, CalculoTHorMax, CalculoTensoesInSitu, RetornoCalculoTensoesInSitu, Calculo } from 'app/repositories/models/calculo';
import { NotybarService } from '@services/notybar.service';
import { TensionStepTwoComponent } from './tension-step-two/tension-step-two.component';
import { HighchartsChartComponent } from 'highcharts-angular';
import { TreeDatasetService } from '@services/dataset/state/tree-dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { CalculationDatasetService } from '@services/dataset/calculation-dataset.service';
import { NumberUtils } from '@utils/number';
import { Observable } from 'rxjs';
import { LoaderService } from '@services/loader.service';

@Component({
  selector: 'app-tensions-insitu-calculation',
  templateUrl: './tensions-insitu-calculation.component.html',
  styleUrls: ['./tensions-insitu-calculation.component.scss']
})
export class TensionsInsituCalculationComponent implements OnInit, DoCheck {

  @ViewChild('stepTwo', { static: false }) stepTwo: TensionStepTwoComponent;
  /**
  * The Highcharts lib. Required to properly render the chart component.
  */
  Highcharts: typeof Highcharts = Highcharts;

  /**
   * Object containing the Highcharts option on how to render the chart.
   */

  @ViewChild('canvas', { static: false }) canvas: HighchartsChartComponent;
  /**
   * The Highcharts chart object associated with this component.
   */
  componentChart: Highcharts.Chart;

  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  /**
  * Tabela.
  */
  hotTable: Handsontable;


  /**
  * Opções das tabelas de CALCULO DE COEFICIENTE
  */
  options: any = TableOptions.createDefault({
    height: 120,
    minSpareRows: 1,
    rowHeaderWidth: 10,
    manualColumnResize: [],
    filters: false,
    fillHandle: {
      autoInsertRow: false,
    }
  });

  /**
   * Titulos das colunas na tabela de (CALCULO DE COEFICIENTE) - Normalização LDA
   */
  colHeadersLda = ['LDA (m)', 'MR (m)', 'PV (m)', 'LOT (lb/gal)'];

  /**
  * Tipos das colunas na tabela de (CALCULO DE COEFICIENTE) - Normalização LDA
  */
  columnsTypesLda = [
    { data: 'lda' },
    { data: 'mr' },
    { data: 'pv' },
    { data: 'lot' },
  ];

  /**
   * Titulos das colunas na tabela de CALCULO DE COEFICIENTE - Normalização PP
   */
  colHeadersPp = ['LDA (m)', 'MR (m)', 'PV (m)', 'LOT (lb/gal)', 'GPORO (lb/gal)'];

  /**
  * Tipos das colunas na tabela de CALCULO DE COEFICIENTE - Normalização PP
  */
  columnsTypesPp = [
    { data: 'lda' },
    { data: 'mr' },
    { data: 'pv' },
    { data: 'lot' },
    { data: 'gradPressãoPoros' },
  ];

  /**
   * Titulos das colunas na tabela de K0 Acompanhamento
   */
  colHeadersKa = ['PV (m)', 'LOT (lb/gal)'];

  /**
  * Tipos das colunas na tabela de K0 Acompanhamento
  */
  columnsTypesKa: any[] = [
    { data: 'pv' },
    { data: 'lot' },
  ];

  /**
 * Titulos das colunas na tabela de k0
 */
  colHeadersK = ['PV (m)', 'LOT (lb/gal)', 'PPORO (psi)', 'TVERT (psi)'];

  /**
  * Tipos das colunas na tabela de k0
  */
  columnsTypesK: any[] = [
    { data: 'pv' },
    { data: 'lot' },
    { data: 'gradPressãoPoros' },
    { data: 'tVert' },
  ];

  /**
  * MODO DE OBTENÇÃO DE SH
  */
  modoSh: 'calcularSh' | 'fornecerSH' = 'calcularSh';

  /**
  * STEP DA PAGINA DE TENSÕES
  */
  stepTensoes: number = 0;

  /**
  * GERAR GRAFICO ON/OFF
  */
  mostrarGrafico: boolean = false;

  /**
   * Se tem depleção ou não
   */
  aplicarDeplecao: boolean = false;

  deplecao = ['GPORO', 'POISS', 'BIOT'];

  /**
   * Lista de perfil no caso de estudo aberto
   */
  perfisDoPoco: Perfil[];

  /**
   * Tipos metodologia
   */
  tiposMetodologia = {
    ModeloElastico: {
      label: 'Modelo elástico', value: 'ModeloElástico',
      perfis: ['GPORO', 'POISS', 'TVERT']
    },
    NormalizacaoLDA: {
      label: 'Normalização pela LDA', value: 'NormalizaçãoLDA',
      lda: true, perfis: ['TVERT'], coeficiente: true,
      tabela: {
        ativada: true, options: this.options, data: [{}, {}],
        colHeaders: this.colHeadersLda, columns: this.columnsTypesLda
      },
      graficoOpcoes: HighchartsUtils.ldaOptions,
    },
    NormalizacaoPressao: {
      label: 'Normalização pela pressão de poros', value: 'NormalizaçãoPP',
      lda: true, perfis: ['GPORO', 'TVERT'], coeficiente: true,
      tabela: {
        ativada: true, options: this.options, data: [{}, {}],
        colHeaders: this.colHeadersPp, columns: this.columnsTypesPp
      },
      graficoOpcoes: HighchartsUtils.ppOptions,
    },
    K0Acompanhamento: {
      label: 'K0 acompanhamento', value: 'K0Acompanhamento',
      perfis: ['GPORO', 'TVERT'],
      tabela: {
        ativada: true, options: this.options, data: [{}, {}],
        colHeaders: this.colHeadersKa, columns: this.columnsTypesKa
      },
      graficoOpcoes: HighchartsUtils.kaOptions,
    },
    K0: {
      label: 'K0', value: 'K0',
      perfis: ['GPORO', 'TVERT'],
      tabela: {
        ativada: true, options: this.options, data: [{}, {}],
        colHeaders: this.colHeadersK, columns: this.columnsTypesK
      },
      graficoOpcoes: HighchartsUtils.kOptions,
    },
  }
  metodologias = Object.keys(this.tiposMetodologia);

  metodologiaSelecionada: any = this.tiposMetodologia.ModeloElastico;

  perfis = {
    GPORO: {
      lista: [],
      selecionado: undefined,
      tooltip: '',
      titulo: 'GPORO',
      depletada: undefined,
      depletadaTooltip: '',
    },
    POISS: { lista: [], selecionado: undefined, tooltip: '', titulo: 'Poisson' },
    ANGAT: { lista: [], selecionado: undefined, tooltip: '', titulo: 'Angat' },
    UCS: { lista: [], selecionado: undefined, tooltip: '', titulo: 'UCS' },
    RET: { lista: [], selecionado: undefined, tooltip: '', titulo: 'Relação entre as tensões' },
    RESTR: { lista: [], selecionado: undefined, tooltip: '', titulo: 'RESTR' },
    TVERT: { lista: [], selecionado: undefined, tooltip: '', titulo: 'Tensão vertical' },
    BIOT: { lista: [], selecionado: undefined, tooltip: '', titulo: 'Biot' },
    THORmin: { lista: [], selecionado: undefined, tooltip: '', titulo: 'Tensão Horizontal Menor' }
  };

  lda = [
    { value: 0, titulo: 'Den.d`água (g/cm3)' },
    { value: 0, titulo: 'Lâmina d`água (m)' },
    { value: 0, titulo: 'Mesa rotativa (m)' },
  ];

  coeficiente: { value: string, tooltip: string } = { value: '0', tooltip: '' };

  tableDatasetValid = [];

  isValid: boolean = true;

  editando: boolean = false;

  oldCalc: RetornoCalculoTensoesInSitu;

  _loading: boolean = false;
  get loading(): boolean { return this._loading; }
  set loading(val: boolean) {
    this._loading = val;
    val ? this.loaderService.addLoading() : this.loaderService.removeLoading();
  }

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { data: { calculo?: RetornoCalculoTensoesInSitu } },
    public dialog: DialogService,
    public notybarService: NotybarService,
    private loaderService: LoaderService,

    public calculoService: CalculoService,

    private dataset: DatasetService,
    private treeService: TreeDatasetService,
    private profileDataset: ProfileDatasetService,
    private calculationDataset: CalculationDatasetService,
  ) {
    profileDataset.$profileAdded.subscribe(el => {
      this.perfis[el.mnemonico].lista.push(el);
    });
  }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    this.lda[0].value = this.currCase.dadosGerais.area.densidadeAguaMar;
    this.lda[1].value = this.currCase.dadosGerais.geometria.offShore.laminaDagua;
    this.lda[2].value = this.currCase.dadosGerais.geometria.mesaRotativa;

    this.perfisDoPoco = this.profileDataset.getAll(this.dataset.currCaseId).sort((a, b) => a.nome.localeCompare(b.nome));

    Object.keys(this.perfis).forEach(key => {
      this.perfis[key].lista = this.perfisDoPoco.filter(el => el.mnemonico === key);
      if (key === 'GPORO') {
        this.perfis[key].lista = this.perfis[key].lista.concat(this.perfisDoPoco.filter(el => el.mnemonico === 'GPPI'));
      }
      if (this.perfis[key].lista.length > 0) {
        this.perfis[key].selecionado = this.perfis[key].lista[0];
      }
    });

    if (this.data.data && this.data.data.calculo !== undefined) {
      this.oldCalc = this.data.data.calculo;
      console.log(this.oldCalc);

      this.editando = true;

      if (this.oldCalc.metodologiaTHORmin) {
        this.metodologiaSelecionada = this.tiposMetodologia[this.metodologias.find(el => this.tiposMetodologia[el].value === this.oldCalc.metodologiaTHORmin)];
      }
      this.oldCalc.perfisEntrada.idPerfis.forEach(id => {
        const p = this.dataset.getById(id);
        if (p && p.mnemonico) {
          if (p.mnemonico === 'GPPI') {
            this.perfis.GPORO.selecionado = p;
          } else {
            this.perfis[p.mnemonico].selecionado = p;
          }
        }
      });

      if (this.oldCalc.depleção) {
        this.aplicarDeplecao = true;

        const p = this.dataset.getById(this.oldCalc.depleção.gporoDepletadaId)
        this.perfis.GPORO.depletada = p;
      }

      if (this.oldCalc.parâmetrosLotDTO) {
        this.metodologiaSelecionada.tabela.data = this.oldCalc.parâmetrosLotDTO.map(el => {
          return { lda: el.lda, mr: el.mesaRotativa, pv: el.profundidadeVertical, lot: el.lot, gradPressãoPoros: el.gradPressãoPoros, tVert: el.tvert };
        })
      }

      if (this.oldCalc.coeficiente) {
        this.coeficiente.value = String(this.oldCalc.coeficiente);
        this.metodologiaSelecionada.tabela.ativada = false;
      }
    }
  }

  ngDoCheck() {
    this.isValid = this.canSubmit();
  }

  canSubmit() {
    let isValid = true;

    if (this.modoSh === 'calcularSh') {
      this.metodologiaSelecionada.perfis.forEach(p => {
        if (this.perfis[p].selecionado === undefined) {
          this.perfis[p].tooltip = 'Precisa escolher um perfil';
          isValid = false;
        } else {
          this.perfis[p].tooltip = '';
        }
      });

      if (this.metodologiaSelecionada.coeficiente && !this.metodologiaSelecionada.tabela.ativada) {
        if (this.coeficiente.value === undefined || this.coeficiente.value === null || this.coeficiente.value === '') {
          this.coeficiente.tooltip = 'Precisa estar preenchido';
          isValid = false;
        } else if (!NumberUtils.isNumber(this.coeficiente.value)) {
          this.coeficiente.tooltip = 'Precisa ser um número';
          isValid = false;
        } else {
          this.coeficiente.tooltip = '';
        }
      }
    } else {
      if (this.perfis.THORmin.selecionado === undefined) {
        this.perfis.THORmin.tooltip = 'Precisa escolher um perfil';
        isValid = false;
      } else {
        this.perfis.THORmin.tooltip = '';
      }
    }

    if (this.aplicarDeplecao) {
      this.deplecao.forEach(p => {
        if (this.perfis[p].selecionado === undefined) {
          this.perfis[p].tooltip = 'Precisa escolher um perfil';
          isValid = false;
        } else {
          this.perfis[p].tooltip = '';
        }
      });

      if (this.perfis.GPORO.depletada === undefined) {
        this.perfis.GPORO.depletadaTooltip = 'Precisa escolher um perfil';
        isValid = false;
      } else {
        this.perfis.GPORO.depletadaTooltip = '';
      }
    }

    return isValid;
  }

  submit() {
    this.loading = true;

    setTimeout(() => {
      const dados: { calculo: CalculoTHorMax, backObj: string } | undefined = this.stepTwo.pegarDados();

      console.log('submit', dados);
      if (dados === undefined) {
        this.loading = false;
        return;
      }
      const calculo: CalculoTensoesInSitu = {
        idCálculo: this.editando ? this.data.data.calculo.id : undefined,
        nomeCálculo: dados.calculo.nomeCálculo,
        idPoço: this.currCase.id,
        listaLot: [],
        tensãoHorizontalMaiorMetodologiaCálculo: dados.calculo.tensãoHorizontalMaiorMetodologiaCálculo,
        [dados.backObj]: dados.calculo[dados.backObj]
      };
      if (this.modoSh === 'calcularSh') {
        if (this.metodologiaSelecionada.perfis.includes('GPORO')) {
          calculo.perfilGPOROId = this.perfis.GPORO.selecionado.id;
        }
        if (this.metodologiaSelecionada.perfis.includes('POISS')) {
          calculo.perfilPoissonId = this.perfis.POISS.selecionado.id;
        }
        if (this.metodologiaSelecionada.perfis.includes('TVERT')) {
          calculo.perfilTensãoVerticalId = this.perfis.TVERT.selecionado.id;
        }
        if (this.metodologiaSelecionada.tabela) {
          if (this.metodologiaSelecionada.tabela.ativada) {
            calculo.listaLot = this.tableDatasetValid;
          } else {
            calculo.coeficiente = Number.parseFloat(this.coeficiente.value);
          }
        }
        calculo.tensãoHorizontalMenorMetodologiaCálculo = this.metodologiaSelecionada.value;
      } else {
        calculo.perfilTHORminId = this.perfis.THORmin.selecionado.id;
      }
      if (Object.keys(dados.calculo[dados.backObj]).includes('perfilGPOROId')) {
        calculo.perfilGPOROId = this.perfis.GPORO.selecionado.id;
      }
      if (this.aplicarDeplecao) {
        calculo.perfilGPOROId = this.perfis.GPORO.selecionado.id;
        calculo.perfilPoissonId = this.perfis.POISS.selecionado.id;
        calculo.depleção = {
          gporoOriginalId: this.perfis.GPORO.selecionado.id,
          gporoDepletadaId: this.perfis.GPORO.depletada.id,
          poissonId: this.perfis.POISS.selecionado.id,
          biotId: this.perfis.BIOT.selecionado.id
        }
      }

      const metodo: Promise<{ cálculo: Calculo, perfisAlterados?: Perfil[] }> = this.editando ?
        this.calculoService.editarCalculoTensoesInSitu(calculo) :
        this.calculoService.criarCalculoTensoesInSitu(calculo);

      // console.log('criar calculo', calculo);
      metodo.then(res => {
        // console.log('FOI', res);
        if (this.editando) {
          this.calculationDataset.update(res.cálculo, calculo.idCálculo, res.perfisAlterados);
        } else {
          this.calculationDataset.add(res.cálculo, this.dataset.currCaseId);
        }

        this.loading = false;
        this.closeModal();
      }).catch(() => { this.loading = false; });
    }, 10);
  }

  /**
   * Fechar diálogo
   */
  closeModal(): void {
    this.dialogRef.close();
  }

  /**
   * Mudança do passo 1 para 2 na página.
   */
  changeStep(newStep: string): void {
    if (newStep == "increment" && this.stepTensoes < 1) {
      if (this.metodologiaSelecionada.tabela && this.metodologiaSelecionada.tabela.ativada) {
        const cols = this.metodologiaSelecionada.tabela.columns.map(c => c.data);
        const verificador = this.metodologiaSelecionada.tabela.data.filter(el => {
          return cols.some(col => el[col] !== undefined && el[col] !== null && el[col] !== '');
        });
        const tableDatasetValid = this.metodologiaSelecionada.tabela.data.filter(el => {
          return cols.every(col => el[col] !== undefined && el[col] !== null && el[col] !== '');
        });
        if (verificador.length === 0) {
          this.notybarService.show('Tabela precisa ter valores.', 'warning');
          return;
        }
        if (tableDatasetValid.length !== verificador.length) {
          this.notybarService.show('Tabela precisa estar completa.', 'warning');
          return;
        }
        this.tableDatasetValid = tableDatasetValid;
      }

      this.stepTensoes = this.stepTensoes + 1;
    }
    if (newStep == "decrement" && this.stepTensoes > 0) {
      this.stepTensoes = this.stepTensoes - 1;
    }
  }

  /**
   * Mostrar/esconder o grafico
   */
  generate(): void {
    if (!this.perfis.TVERT.selecionado) {
      this.notybarService.show('TVERT precisa estar selecionado.', 'warning');
      return;
    }
    const opcoesGrafico: CalcularGraficoTensoes = {
      idPoço: this.currCase.id,
      perfilTensãoVerticalId: this.perfis.TVERT.selecionado.id,
      listaLot: []
    };
    if (!this.metodologiaSelecionada.tabela.ativada) {
      if (this.coeficiente.value === undefined || this.coeficiente.value === null || this.coeficiente.value === '') {
        this.notybarService.show('Coeficiente precisa estar preenchido.', 'warning');
        return;
      } else if (!NumberUtils.isNumber(this.coeficiente.value)) {
        this.notybarService.show('Coeficiente precisa ser um número.', 'warning');
        return;
      } else {
        opcoesGrafico.coeficiente = Number.parseFloat(this.coeficiente.value);
      }
    } else {
      const cols = this.metodologiaSelecionada.tabela.columns.map(c => c.data);
      const verificador = this.metodologiaSelecionada.tabela.data.filter(el => {
        return cols.some(col => el[col] !== undefined && el[col] !== null && el[col] !== '');
      });
      opcoesGrafico.listaLot = this.metodologiaSelecionada.tabela.data.filter(el => {
        return cols.every(col => el[col] !== undefined && el[col] !== null && el[col] !== '');
      });
      if (verificador.length === 0) {
        this.notybarService.show('Tabela precisa ter valores.', 'warning');
        return;
      }
      if (opcoesGrafico.listaLot.length !== verificador.length) {
        this.notybarService.show('Tabela precisa estar completa.', 'warning');
        return;
      }
    }
    if (this.metodologiaSelecionada.perfis.includes('GPORO')) {
      if (this.perfis.GPORO.selecionado) {
        opcoesGrafico.perfilGPOROId = this.perfis.GPORO.selecionado.id;
      } else {
        this.notybarService.show('GPORO precisa estar selecionado.', 'warning');
        return;
      }
    }

    this.calculoService.criarGraficoTensoesInSitu(opcoesGrafico, this.metodologiaSelecionada.value).then(res => {

      this.mostrarGrafico = true;

      setTimeout(() => {
        this.componentChart = this.canvas['chart'];

        while (this.componentChart.series.length) {
          this.componentChart.series[0].remove();
        }
        if (this.metodologiaSelecionada.value === this.tiposMetodologia.NormalizacaoLDA.value ||
          this.metodologiaSelecionada.value === this.tiposMetodologia.NormalizacaoPressao.value ||
          this.metodologiaSelecionada.value === this.tiposMetodologia.K0.value) {
          if (res.retorno) {
            this.adicionarGraficoPontosLinha(res.retorno.coeficiente, res.retorno.pontosDTO);
          } else {
            this.adicionarGraficoPontosLinha(1, [{ valorX: 1, valorY: 1 }]);
          }
        } else {
          this.adicionarK0Acomp(res.perfil, opcoesGrafico.listaLot);
        }
      }, 1);

    });
  }

  adicionarGraficoPontosLinha(coeficiente: number, pontosDTO: { valorX: number, valorY: number }[]) {
    const data: [number, number][] = pontosDTO.map(el => [el.valorX, el.valorY]);

    const pointLine: Highcharts.SeriesLineOptions = {
      type: 'line',
      lineWidth: 0,
      marker: {
        enabled: true,
        radius: 2
      },
      data: data,
    };
    this.componentChart.addSeries(
      pointLine as Highcharts.SeriesLineOptions
    );

    const line: Highcharts.SeriesLineOptions = {
      type: 'line',
      marker: { enabled: false, },
      data: [
        [0, 0],
        [pontosDTO[pontosDTO.length - 1].valorX, pontosDTO[pontosDTO.length - 1].valorX * coeficiente],
      ],
    };
    this.componentChart.addSeries(
      line as Highcharts.SeriesLineOptions
    );
    this.componentChart.redraw();
  }

  adicionarK0Acomp(perfil: Perfil, pontosMarcados: { pv: number, lot: number }[]) {
    const data: [number, number][] = perfil.pontos.map(el => [el.pv.valor, el.valor]);

    const pointLine: Highcharts.SeriesLineOptions = {
      type: 'line',
      marker: { enabled: false, },
      data: data,
      allowPointSelect: false,
      states: { hover: { enabled: false } },
    };
    this.componentChart.addSeries(
      pointLine as Highcharts.SeriesLineOptions
    );

    const pontos = [];
    for (let i = 0; i < pontosMarcados.length; i++) {
      const p = pontosMarcados[i];
      pontos.push(data.find(el => el[0] >= p.pv));
    }

    const line: Highcharts.SeriesLineOptions = {
      type: 'line',
      marker: {
        enabled: true,
        radius: 4,
      },
      states: { hover: { lineWidthPlus: 0 } },
      lineWidth: 0,
      data: pontos,
    };
    this.componentChart.addSeries(
      line as Highcharts.SeriesLineOptions
    );
    this.componentChart.redraw();
  }

  /**
   * Esconde o grafico caso o usuário troque metodologiaSelecionada
   */
  changeChart(): void {
    this.mostrarGrafico = false;
  }

}
