import { Component, DoCheck, Inject, Input, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { DatasetService } from '@services/dataset/dataset.service';
import { DialogService } from '@services/dialog.service';
import { NotybarService } from '@services/notybar.service';
import { TableOptions } from '@utils/table-options-default';
import { HighchartsChartComponent } from 'highcharts-angular';
import * as Highcharts from 'highcharts';
import * as HighchartsUtils from '@utils/highcharts';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { Perfil } from 'app/repositories/models/perfil';
import { ArrayUtils } from '@utils/array';
import { HotTableComponent } from 'ng2-handsontable';
import { ComposicaoPerfisService } from 'app/repositories/composicao-perfis.service';
import { ComposicaoPerfil } from 'app/repositories/models/composicao-perfis';
import { NumberUtils } from '@utils/number';
import { Case } from 'app/repositories/models/case';

@Component({
  selector: 'app-window-profile-composition',
  templateUrl: './window-profile-composition.component.html',
  styleUrls: ['./window-profile-composition.component.scss']
})
export class WindowProfileCompositionComponent implements OnInit, DoCheck {

  /**
   * O caso de estudo aberto atualmente
   */
  currCase: Case;

  /**
  * The Highcharts lib. Required to properly render the chart component.
  */
  Highcharts: typeof Highcharts = Highcharts;

  /**
   * The Highcharts chart instance used in this component.
   */
  componentChart: Highcharts.Chart;

  /**
  * The default options for plotting a chart on the result area.
  */
  plotOptions: Highcharts.Options = HighchartsUtils.compositionCanvasOptions;

  /**
  * Object containing the Highcharts option on how to render the chart.
  */

  @ViewChild('canvas', { static: false }) canvas: HighchartsChartComponent;

  /**
  * A unidade padrão do canvas.
  */
  unidadePadrao: string = undefined;

  /**
   * Referência para tabela na tela.
   */
  @ViewChild(HotTableComponent, { static: true }) hotTableComponent;

  /**
   * Tabela Composição de curvas.
   */
  table: Handsontable;

  /**
   * Titulos das colunas na tabela Composição.
   */
  baseColHeaders = [
    'Perfil',
    'PM topo (m)',
    'PM base (m)',
  ];

  /**
  * Tipos das colunas na tabela Composição.
  */
  baseColumnsTypes: any[] = [
    { data: 'perfil', type: 'dropdown', source: [] },
    { data: 'pmTopo', },
    { data: 'pmBase', },
  ];

  /**
  * dataset da tabela Composição
  */
  baseDataset: { perfil: string, pmTopo: string | number, pmBase: string | number }[] = [];

  /**
   * Opções da tabela Composição
   */
  baseOptions: any = TableOptions.createDefault({
    height: 400,
    rowHeaderWidth: 10,
    minSpareRows: 1,
    manualColumnResize: [],
    filters: false,
    afterChange: () => {
      if (this.validateTable()) {
        this.atualizarNovoPerfil();
      }
    },
    afterCreateRow: () => {
      if (this.validateTable()) {
        this.atualizarNovoPerfil();
      }
    },
    afterRemoveRow: () => {
      if (this.validateTable()) {
        this.atualizarNovoPerfil();
      }
    },
    contextMenu: {
      items: {
        'remove_row': {},
        'cut': {},
        'copy': {}
      }
    },
    fillHandle: {
      autoInsertRow: false,
    }
  });

  /**
   * Tipos de curvas
   */
  tipoCurvas = [];

  /**
   * Tipo de curvas selecionado
   */
  tipoSelecionado;

  /**
   * Se vai exibir todas as curvas
   */
  allCurves: boolean = false;

  /**
   * Nome do perfil que será gerado
   */
  nome: { value: string, tooltip: string, placeholder: string } = { value: 'xx_comp', tooltip: '', placeholder: '' };

  /**
   * Lista de perfil no caso de estudo aberto
   */
  perfisCasoAtual: Perfil[];

  perfisTodosCaso: Perfil[];

  perfisAtuais: Perfil[] = [];

  trechos: any = {};

  nomesEmUso: string[] = [];

  isValid: boolean = true;

  novoPerfil: Highcharts.SeriesLineOptions;

  pmRange: { min: number, max: number } = { min: 0, max: 0 };

  dadosPoco = {
    pmMax: 0,
    pmSup: 0,
  };

  loading = false;

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { data: {} },
    public dialog: DialogService,

    private composicaoService: ComposicaoPerfisService,
    private profileDataset: ProfileDatasetService,
    private dataset: DatasetService,
    private notybarService: NotybarService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);
    this.dadosPoco = {
      pmMax: this.currCase.trajetória.últimoPonto.pm.valor,
      pmSup: this.currCase.trajetória.primeiroPonto.pm.valor,
    };

    this.perfisCasoAtual = this.profileDataset.getAll(this.dataset.currCaseId).sort((a, b) => a.nome.localeCompare(b.nome));
    this.perfisTodosCaso = [];
    this.dataset.getCasesIds().forEach(id => {
      this.perfisTodosCaso = this.perfisTodosCaso.concat(this.profileDataset.getAll(id));
    });
    this.perfisTodosCaso = this.perfisTodosCaso.sort((a, b) => a.nome.localeCompare(b.nome));

    this.nomesEmUso = this.perfisCasoAtual.map(el => el.nome);

    this.tipoCurvas = ArrayUtils.removeDups(this.perfisTodosCaso.map(el => el.mnemonico));
    this.tipoSelecionado = this.tipoCurvas[0];

    this.mudouTipo();
  }

  ngAfterViewInit() {
    this.table = this.hotTableComponent.getHandsontableInstance();

    this.componentChart = this.canvas['chart'];

    this.adicionarNovoPerfil();

    this.mudouTipo();
  }

  ngDoCheck() {
    this.isValid = this.canSubmit();
  }

  canSubmit() {
    let isValid = true;
    if (this.nome.value === undefined || this.nome.value === null || this.nome.value === '') {
      this.nome.tooltip = 'Nome precisa estar preenchido';
      isValid = false;
    } else if (this.nomesEmUso.includes(this.nome.value)) {
      this.nome.tooltip = 'Nome em uso';
      isValid = false;
    } else {
      this.nome.tooltip = '';
    }

    return isValid;
  }


  submit() {
    this.loading = true;
    if (!this.validateTable()) {
      this.notybarService.show('Tabela precisa estar válida.', 'warning');
      this.loading = false;
      return;
    }
    let trechos = this.baseDataset.map(el => {
      if (el.perfil) {
        return {
          idPerfil: this.perfisAtuais.find(p => p.nome === el.perfil).id,
          pmTopo: Number(el.pmTopo),
          pmBase: Number(el.pmBase),
        }
      }
    });

    trechos = trechos.filter(el => el);
    if (trechos.length === 0) {
      this.notybarService.show('Necessário pelo menos um trecho.', 'warning');
      this.loading = false;
      return;
    }

    const compor: ComposicaoPerfil = {
      nomePerfil: this.nome.value,
      idPoço: this.dataset.currCaseId,
      tipoPerfil: this.tipoSelecionado,
      listaTrechos: trechos
    };

    console.log('compor', compor);
    this.composicaoService.comporPerfil(compor).then(res => {
      console.log('FOI', res);
      const perfil: Perfil = res.perfil;
      this.profileDataset.add(perfil, this.dataset.currCaseId);

      this.loading = false;
      this.closeModal();
    }).catch(() => { this.loading = false; });
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  mudouTipo() {
    this.nome.value = `${this.tipoSelecionado}_comp`;
    if (this.allCurves) {
      this.perfisAtuais = this.perfisTodosCaso.filter(el => el.mnemonico === this.tipoSelecionado);
      if (this.tipoSelecionado === 'GPPI') {
        this.perfisAtuais = this.perfisAtuais.concat(this.perfisTodosCaso.filter(el => el.mnemonico === 'GPORO'));
      }
    } else {
      this.perfisAtuais = this.perfisCasoAtual.filter(el => el.mnemonico === this.tipoSelecionado);
      if (this.tipoSelecionado === 'GPPI') {
        this.perfisAtuais = this.perfisAtuais.concat(this.perfisCasoAtual.filter(el => el.mnemonico === 'GPORO'));
      }
    }
    this.pmRange.max = 0;
    this.perfisAtuais.forEach(p => {
      if (p.pmMáximo.valor > this.pmRange.max) {
        this.pmRange.max = p.pmMáximo.valor;
      }
      if (p.pmMínimo.valor < this.pmRange.min) {
        this.pmRange.min = p.pmMínimo.valor;
      }
    });
    this.pmRange.max *= 1.05;

    if (this.table) {
      if (this.allCurves) {
        this.baseColumnsTypes[0].source = this.perfisAtuais.map(el => this.dataset.getById(el.idPoço).nome + ' | ' + el.nome);
      } else {
        this.baseColumnsTypes[0].source = this.perfisAtuais.map(el => el.nome);
      }
      this.table.updateSettings({ columns: this.baseColumnsTypes }, false);

      this.atualizarNovoPerfil();

      this.table.render();
    }

    const perfil = this.perfisAtuais[0];
    if (perfil && perfil.grupoDeUnidades && perfil.grupoDeUnidades.unidadePadrão && perfil.grupoDeUnidades.unidadePadrão.símbolo) {
      this.unidadePadrao = this.perfisAtuais[0].grupoDeUnidades.unidadePadrão.símbolo;
    }
  }

  adicionarNovoPerfil(data?) {
    if (this.componentChart) {
      while (this.componentChart.series.length > 0) {
        this.componentChart.series[0].remove();
      }

      this.novoPerfil = {
        type: 'line',
        color: 'black',
        dashStyle: 'Solid',
        lineWidth: 1,
        data: data
      };

      this.componentChart.addSeries(
        this.novoPerfil as Highcharts.SeriesLineOptions
      );

      this.componentChart.yAxis[0].setExtremes();
      this.componentChart.xAxis[0].setExtremes(this.pmRange.min, this.pmRange.max);
      this.componentChart.redraw();
    }
  }

  atualizarNovoPerfil() {
    let data = [];
    this.baseDataset.forEach(linha => {
      if (linha.perfil && linha.pmBase && linha.pmTopo) {
        let perfil;
        if (!this.allCurves) {
          perfil = this.perfisAtuais.find(p => p.nome === linha.perfil);
        } else {
          perfil = this.perfisAtuais.find(p => this.dataset.getById(p.idPoço).nome + ' | ' + p.nome === linha.perfil);
        }
        if (perfil) {
          let start = 0, end = perfil.pm.length - 1;
          for (let i = 0; i < perfil.pm.length; i++) {
            if (perfil.pm[i][0] <= Number(linha.pmTopo)) {
              start = i;
            } else {
              break;
            }
          }
          for (let i = perfil.pm.length - 1; i >= 0; i--) {
            if (perfil.pm[i][0] >= Number(linha.pmBase)) {
              end = i;
            } else {
              break;
            }
          }
          if (start !== end) {
            data = data.concat(perfil.pm.slice(start, end));
          }
        }
      }
    });
    if (this.novoPerfil) {
      this.novoPerfil.data = data.sort((a, b) => (a[0] > b[0]) ? 1 : ((b[0] > a[0]) ? -1 : 0));

      if (this.tipoSelecionado === 'DIAM_BROCA' && this.novoPerfil.data.length > 0) {
        const poco = this.dataset.getById(this.dataset.currCaseId);
        this.novoPerfil['step'] = 'left';
        this.novoPerfil.data.push([
          poco.trajetória.últimoPonto.pm.valor,
          this.novoPerfil.data[this.novoPerfil.data.length - 1][1],
        ]);
      }

      this.adicionarNovoPerfil(data);
    }
  }

  removeError(row, prop) {
    if (this.table) {
      const comment = this.table.getPlugin('comments');
      const col = this.table.propToCol(prop)

      comment.removeCommentAtCell(row, col);
      this.table.setCellMetaObject(row, col, { valid: true });
    }
  }

  setError(row, prop, error) {
    if (this.table) {
      const comment = this.table.getPlugin('comments');
      const col = this.table.propToCol(prop)

      comment.setCommentAtCell(row, col, error);
      comment.updateCommentMeta(row, col, { readOnly: true });
      this.table.setCellMetaObject(row, col, { valid: false });
    }
  }

  adicionaLinha(perfil: Perfil, trecho: { pmTopo: number, pmBase: number }) {
    this.baseDataset = this.baseDataset.filter(el => !this.vazio(el.pmTopo) || !this.vazio(el.pmBase) || !this.vazio(el.perfil));
    for (let i = 0; i < this.baseDataset.length; i++) {
      if (!this.vazio(this.baseDataset[i].pmTopo) && !this.vazio(this.baseDataset[i].pmBase)) {
        if (Number(this.baseDataset[i].pmTopo) < trecho.pmBase && Number(this.baseDataset[i].pmTopo) > trecho.pmTopo) {
          trecho.pmBase = Number(this.baseDataset[i].pmTopo);
        } else if (Number(this.baseDataset[i].pmBase) > trecho.pmTopo && Number(this.baseDataset[i].pmBase) < trecho.pmBase) {
          trecho.pmTopo = Number(this.baseDataset[i].pmBase);
        }
      }
    }
    if (trecho.pmBase > this.dadosPoco.pmMax) {
      trecho.pmBase = this.dadosPoco.pmMax;
    }
    this.baseDataset.push({
      perfil: this.allCurves ? this.dataset.getById(perfil.idPoço).nome + ' | ' + perfil.nome : perfil.nome,
      pmTopo: trecho.pmTopo,
      pmBase: trecho.pmBase
    });
    this.baseDataset.push({ perfil: undefined, pmBase: undefined, pmTopo: undefined });
  }

  validateTable() {
    this.baseDataset.sort((a, b) => this.vazio(a.pmBase) ? 1 : this.vazio(b.pmBase) ? -1 : Number(a.pmBase) - Number(b.pmBase));
    let valid = true;
    for (let i = 0; i < this.baseDataset.length; i++) {
      if (!this.vazio(this.baseDataset[i].pmTopo) || !this.vazio(this.baseDataset[i].pmBase) || !this.vazio(this.baseDataset[i].perfil)) {
        this.removeError(i, 'pmTopo');
        this.removeError(i, 'pmBase');
        this.removeError(i, 'perfil');
        let error = '';
        if (this.vazio(this.baseDataset[i].pmTopo)) {
          error = 'Precisa estar preenchido';
          this.setError(i, 'pmTopo', error);
        } else if (!NumberUtils.isNumber(this.baseDataset[i].pmTopo)) {
          error = 'Precisa ser um número';
          this.setError(i, 'pmTopo', error);
        } else if (this.baseDataset[i].pmTopo < this.dadosPoco.pmSup) {
          error = 'PM Topo não pode ser menor que PM Sup';
          this.setError(i, 'pmTopo', error);
        }
        if (this.vazio(this.baseDataset[i].pmBase)) {
          error = 'Precisa estar preenchido';
          this.setError(i, 'pmBase', error);
        } else if (!NumberUtils.isNumber(this.baseDataset[i].pmBase)) {
          error = 'Precisa ser um número';
          this.setError(i, 'pmBase', error);
        } else if (this.baseDataset[i].pmBase > this.dadosPoco.pmMax) {
          error = 'PM Base não pode ser maior que PM Max';
          this.setError(i, 'pmBase', error);
        }
        if (error === '' && Number(this.baseDataset[i].pmTopo) >= Number(this.baseDataset[i].pmBase)) {
          error = 'PM Topo deve ser menor que PM Base';
          this.setError(i, 'pmTopo', error);
          this.setError(i, 'pmBase', error);
        }
        if (error === '') {
          for (let f = 0; f < this.baseDataset.length; f++) {
            if (Number(this.baseDataset[i].pmTopo) > Number(this.baseDataset[f].pmTopo)) {
              if (Number(this.baseDataset[i].pmTopo) < Number(this.baseDataset[f].pmBase)) {
                error = 'Não pode haver sobreposição';
                this.setError(i, 'pmTopo', error);
                this.setError(i, 'pmBase', error);
              }
            } else if (Number(this.baseDataset[i].pmBase) < Number(this.baseDataset[f].pmBase)) {
              if (Number(this.baseDataset[i].pmBase) > Number(this.baseDataset[f].pmTopo)) {
                error = 'Não pode haver sobreposição';
                this.setError(i, 'pmTopo', error);
                this.setError(i, 'pmBase', error);
              }
            }
          }
        }
        if (this.vazio(this.baseDataset[i].perfil)) {
          error = 'Precisa estar preenchido';
          this.setError(i, 'perfil', error);
        } else if (!this.allCurves && !this.perfisAtuais.some(p => p.nome === this.baseDataset[i].perfil)) {
          error = 'Precisa ser um dos perfis do tipo atual';
          this.setError(i, 'perfil', error);
        } else if (this.allCurves && !this.perfisAtuais.some(p => this.dataset.getById(p.idPoço).nome + ' | ' + p.nome === this.baseDataset[i].perfil)) {
          error = 'Precisa ser um dos perfis do tipo atual';
          this.setError(i, 'perfil', error);
        }

        if (error !== '') {
          valid = false;
        }
      }
    }
    if (this.table) {
      this.table.render();
    }
    if (valid) {
      this.trechos = [];
      this.perfisAtuais.forEach(perfil => {
        let trechos: { pmTopo: string | number, pmBase: string | number }[] = [];
        this.baseDataset.forEach(linha => {
          if ((!this.allCurves && linha.perfil === perfil.nome) || (this.allCurves && linha.perfil === this.dataset.getById(perfil.idPoço).nome + ' | ' + perfil.nome)) {
            trechos.push({ pmTopo: linha.pmTopo, pmBase: linha.pmBase });
          }
        });
        this.trechos[perfil.id] = trechos;
      });
    }

    return valid;
  }

  vazio(valor) {
    return valor === undefined || valor === null || valor === '';
  }
}
