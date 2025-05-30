import { Component, OnInit, Inject, ViewChild, AfterViewInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { TrendService } from 'app/repositories/trend.service';
import { NotybarService } from '@services/notybar.service';
import { EditTrendService } from '@services/edit-trend.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { Trend } from 'app/repositories/models/trend';
import { Perfil } from 'app/repositories/models/perfil';
import { TableOptions } from '@utils/table-options-default';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { Case } from 'app/repositories/models/case';

@Component({
  selector: 'app-window-edit-trend',
  templateUrl: './window-edit-trend.component.html',
  styleUrls: ['./window-edit-trend.component.scss']
})
export class WindowEditTrendComponent implements OnInit, AfterViewInit {

  nomeTrend: string;
  tipoTrend: 'LBF' | 'Compactação' | 'Gradiente';
  travarType: 'Profundidade' | 'Inclinacao' = 'Inclinacao';

  tableValidator = tableValidator;

  /**
   * Referência para tabela na tela de litologias.
   */
  @ViewChild('table', { static: true }) tableComponent;

  /**
   * Tabela.
   */
  table: Handsontable;
  tipoProfundidade = { 'LBF': 'PM', 'Compactação': 'PV' };

  colHeadersByTrendTypes = {
    'Compactação': [
      'PV topo (m)',
      'PV base (m)',
      'Valor topo',
      'Valor base',
      'Inclinação',
    ],
    'LBF': [
      'PM topo (m)',
      'PM base (m)',
      'Valor topo',
      'Valor base',
    ]
  };
  /**
   * Titulos das colunas na tabela de trend.
   */
  colHeaders = [];

  columnsTypesByTrendTypes = {
    'Compactação': [
      { data: 'pvTopo', type: 'numeric', validator: this.tableValidator },
      { data: 'pvBase', type: 'numeric', validator: this.tableValidator },
      { data: 'valorTopo', readOnly: true, type: 'numeric' },
      { data: 'valorBase', readOnly: true, type: 'numeric' },
      { data: 'inclinação', type: 'numeric', validator: this.tableValidator, numericFormat: { pattern: '0.000' } },
    ],
    'LBF': [
      { data: 'pvTopo', type: 'numeric', validator: this.tableValidator },
      { data: 'pvBase', type: 'numeric', validator: this.tableValidator },
      { data: 'valorTopo', type: 'numeric', validator: this.tableValidator },
      { data: 'valorBase', type: 'numeric', validator: this.tableValidator },
    ]
  };
  /**
   * Tipos das colunas na tabela de trend.
   */
  columnsTypes: any[] = [];

  options: any = TableOptions.createDefault({
    height: 140,
    rowHeaderWidth: 15,
    rowHeaders: true,
    filters: false,
    contextMenu: {
      items: {
        'row_above': {
          name: 'Inserir linha acima'
        },
        'row_below': {
          name: 'Inserir linha abaixo'
        },
        'remove_row': {
          name: 'Remover linha'
        },
      }
    },
  });

  tableData: any[] = [];

  perfil: Perfil;
  perfilOriginal: Perfil;
  trendValido: Trend;

  isValid = true;

  sedimentos;
  ultimoPv;

  loading = false;

  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  constructor(
    private trendService: TrendService,
    private editTrendService: EditTrendService,
    public dataset: DatasetService,
    private notybarService: NotybarService,
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    private profileDataset: ProfileDatasetService,
    @Inject(MAT_DIALOG_DATA) public dialogData: { data: { idPerfil: string, caseId: string } }
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dialogData.data.caseId);

    this.perfilOriginal = this.dataset.getById(this.dialogData.data.idPerfil);
    this.trendValido = this.perfilOriginal.trend;

    this.perfil = JSON.parse(JSON.stringify(this.perfilOriginal));

    this.nomeTrend = this.perfil.trend.nome;
    this.tipoTrend = this.perfil.trend.tipoTrend;

    this.colHeaders = this.colHeadersByTrendTypes[this.tipoTrend];
    this.columnsTypes = this.columnsTypesByTrendTypes[this.tipoTrend];

    this.tableData = JSON.parse(JSON.stringify(this.perfil.trend.trechos));

    if (this.currCase.dadosGerais.geometria.categoriaPoço === 'OnShore') {
      this.sedimentos = this.currCase.dadosGerais.geometria.mesaRotativa + this.currCase.dadosGerais.geometria.onShore.alturaDeAntePoço;
    } else {
      this.sedimentos = this.currCase.dadosGerais.geometria.mesaRotativa + this.currCase.dadosGerais.geometria.offShore.laminaDagua;
    }
    if (this.tipoTrend === 'LBF') {
      this.ultimoPv = this.currCase.trajetória.últimoPonto.pm.valor;
    } else {
      this.ultimoPv = this.currCase.trajetória.últimoPonto.pv.valor;
    }
  }

  ngAfterViewInit() {
    this.table = this.tableComponent.getHandsontableInstance();
    this.changeType();
  }


  changeType() {
    for (let i = 0; i < this.tableData.length; i++) {
      const i0: any = this.table.getCellMeta(i, 0);
      i0.readOnly = this.travarType === 'Profundidade';
      const i1: any = this.table.getCellMeta(i, 1);
      i1.readOnly = this.travarType === 'Profundidade';
      const i4: any = this.table.getCellMeta(i, 4);
      i4.readOnly = this.travarType === 'Inclinacao';
    }
    this.table.render();
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  submit() {
    this.aplicar();
    this.closeModal();
  }

  aplicar() {
    this.loading = true;
    if (!this.isValid) {
      this.notybarService.show('Conserte os erros na tabela', 'danger');
      this.loading = false;
      return;
    }
    setTimeout(() => {
      this.perfil.trend.trechos = this.tableData.filter(el => el.valorTopo !== null && el.valorBase !== null);
      this.perfil.trend.nome = this.nomeTrend;
      this.trendService.edit(this.perfil).then(res => {
        this.trendValido = this.perfil.trend;
        this.loading = false;
      }).catch(() => {
        this.loading = false;
        this.perfilOriginal.trend = this.trendValido;
        this.profileDataset.editTrend(this.perfil.id);
      });
      this.perfilOriginal.trend = this.perfil.trend;
      this.profileDataset.editTrend(this.perfil.id);
    }, 1);
  }

  afterCreateRow(event) {
    // console.log('created', event);
    // console.log(this.dataset[event[0]].pvTopo, this.dataset);
    if (event[0] === 0) {
      this.tableData[event[0]] = {
        pvTopo: this.tableData[event[0] + 1].pvTopo - 1,
        pvBase: this.tableData[event[0] + 1].pvTopo,
        valorTopo: this.tableData[event[0] + 1].valorTopo,
        valorBase: this.tableData[event[0] + 1].valorTopo,
      };
      this.editTrendService.calcularInclinacao(this.tableData[event[0]]);
      // console.log('no inicio');
    } else if (event[0] === this.tableData.length - 1) {
      this.tableData[event[0]] = {
        pvTopo: this.tableData[event[0] - 1].pvBase,
        pvBase: this.tableData[event[0] - 1].pvBase + 1,
        valorTopo: this.tableData[event[0] - 1].valorBase,
        valorBase: this.tableData[event[0] - 1].valorBase,
      };
      this.editTrendService.calcularInclinacao(this.tableData[event[0]]);
      // console.log('no fim');
    } else {
      this.tableData[event[0]] = {
        pvTopo: this.tableData[event[0] - 1].pvBase,
        pvBase: this.tableData[event[0] + 1].pvTopo + 1,
        valorTopo: this.tableData[event[0] - 1].valorBase,
        valorBase: this.tableData[event[0] + 1].valorTopo,
      };
      this.editTrendService.calcularInclinacao(this.tableData[event[0]]);
      this.afterChangeTable([[[event[0], 'pvBase', this.tableData[event[0]].pvBase - 1, this.tableData[event[0]].pvBase]], 'edit']);
      // console.log('no meio');
    }

    this.changeType();
  }

  afterRemoveRow(event) {
    // console.log('removed', event);
    // console.log(this.dataset[event[0]].pvTopo, this.dataset);
    if (this.tableData.length === 0) {
      const trechoPadrao = {
        pvTopo: this.perfil.primeiroPonto.pm.valor,
        pvBase: this.perfil.ultimoPonto.pm.valor,
        valorTopo: this.perfil.primeiroPonto.valor,
        valorBase: this.perfil.primeiroPonto.valor,
        inclinação: 0
      };
      if (this.tipoTrend === 'Compactação') {
        trechoPadrao.valorBase = this.perfil.ultimoPonto.valor;
        this.editTrendService.calcularInclinacao(trechoPadrao);
      }

      if (trechoPadrao.pvTopo < this.sedimentos) {
        trechoPadrao.pvTopo = this.sedimentos;
      }
      if (trechoPadrao.pvBase > this.ultimoPv) {
        trechoPadrao.pvBase = this.ultimoPv;
      }
      this.tableData.push(trechoPadrao);
      this.changeType();
      this.table.render();
    } else if (event[0] > 0 && this.tableData.length > 1 && this.tableData[event[0]].pvTopo !== null &&
      this.tableData[event[0]].pvTopo !== undefined) {
      const linha = this.tableData[event[0] - 1];

      // recalcula apenas o valor da base, a partir do topo, para a mesma inclinação
      const corte = this.editTrendService.calcularCorte(linha.pvTopo, linha.pvBase, linha.valorTopo,
        linha.valorBase, this.tableData[event[0]].pvTopo, false);
      linha.pvBase = this.tableData[event[0]].pvTopo;
      linha.valorBase = linha.valorTopo - corte;
      this.table.render();
    }
  }

  afterValidate(event) {
    // console.log('afterValidate', event);
    this.isValid = event[0] && this.isValid;
  }

  afterChangeTable(event) {
    // console.log('Changed', event);
    this.isValid = true;
    if (event !== undefined && event[0] !== undefined && event[0] !== null && event[0].length > 0) {
      event[0].forEach(e => {
        const row = e[0], col = e[1], oldValue = e[2], newValue = e[3];
        if (isNaN(newValue) || newValue === null || newValue === '') {
          this.isValid = false;
        } else if (col === 'pvBase' || col === 'pvTopo') {
          if (newValue < this.sedimentos) {
            this.isValid = false;
          }
          if (newValue > this.ultimoPv) {
            this.isValid = false;
          }
          // na mesma linha
          const valor = this.editTrendService.calcularValor(this.tableData[row]);
          if (col === 'pvBase') {
            if (newValue <= this.tableData[row].pvTopo) {
              this.isValid = false;
            }
            // recalcula apenas o valor da base, a partir do topo, para a mesma inclinação
            this.tableData[row].valorBase = this.tableData[row].valorTopo + valor;

            // na linha seguinte
            if (row < this.tableData.length - 1) {
              // edita a profundidade de topo para que seja idêntica à da base da linha anterior
              this.tableData[row + 1].pvTopo = newValue;
              // recalcula o valor de topo a partir da inclinação e do valor da base
              const valorProximo = this.editTrendService.calcularValor(this.tableData[row + 1]);

              this.tableData[row + 1].valorTopo = this.tableData[row + 1].valorBase - valorProximo;
            }
          } else if (col === 'pvTopo') {
            if (newValue >= this.tableData[row].pvBase) {
              this.isValid = false;
            }
            // recalcula apenas o valor de topo, a partir da base, para a mesma inclinação
            this.tableData[row].valorTopo = this.tableData[row].valorBase - valor;

            // na linha anterior
            if (row > 0) {
              // edita a profundidade de base para que seja idêntica à do topo da linha seguinte
              this.tableData[row - 1].pvBase = newValue;
              // recalcula o valor da base a partir da inclinação e do valor de topo
              const valorAnterior = this.editTrendService.calcularValor(this.tableData[row - 1]);

              this.tableData[row - 1].valorBase = this.tableData[row - 1].valorTopo + valorAnterior;
            }
          }
        } else if (col === 'inclinação') {
          // recalcula apenas o valor de base em função da nova inclinação
          const valor = this.editTrendService.calcularValor(this.tableData[row]);
          this.tableData[row].valorBase = this.tableData[row].valorTopo + valor;
        } else if (col === 'valorTopo' || col === 'valorBase') {
          if (newValue <= 0) {
            this.isValid = false;
          }
        }
      });
      // console.log('end', this.dataset);
      this.table.render();
    }
    this.table.validateCells(() => { });
  }

}

function tableValidator(value, callback) {
  const comment = this.instance.getPlugin('comments');
  let error = '';
  if (value !== null) {
    if (!isNaN(value)) {
      // console.log(this.instance)
      if (this.prop === 'inclinação') {
        comment.removeCommentAtCell(this.row, this.col);
        callback(true);
        return;
      } else if (this.prop === 'valorTopo' || this.prop === 'valorBase') {
        if (value <= 0) {
          error = 'Valor precisa ser positivo.';
        } else {
          comment.removeCommentAtCell(this.row, this.col);
          callback(true);
          return;
        }
      } else if (this.prop === 'pvTopo' || this.prop === 'pvBase') {
        const min = parseFloat(this.instance.rootElement.parentElement.parentElement.childNodes[1].textContent);
        const max = parseFloat(this.instance.rootElement.parentElement.parentElement.childNodes[2].textContent);
        const deep = this.instance.rootElement.parentElement.parentElement.childNodes[3].textContent;
        if (value < min) {
          error = `${deep} deve ser maior ou igual a superfície de sedimentos, que é ${min}.`;
        } else if (value > max) {
          error = `${deep} deve ser menor ou igual ao último ${deep} da trajétoria, que é ${max}.`;
        } else if ((this.prop === 'pvTopo' && value >= this.instance.getDataAtCell(this.row, 1)) ||
          (this.prop === 'pvBase' && value <= this.instance.getDataAtCell(this.row, 0))) {
          error = `${deep} topo deve ser menor que ${deep} base.`;
        } else {
          comment.removeCommentAtCell(this.row, this.col);
          callback(true);
          return;
        }

      } else {
        comment.removeCommentAtCell(this.row, this.col);
        callback(true);
        return;
      }
    } else {
      error = 'Precisa ser um número.';
    }
  } else {
    error = 'Não pode estar vazia.';
  }
  comment.setCommentAtCell(this.row, this.col, error);
  comment.updateCommentMeta(this.row, this.col, { readOnly: true });
  callback(false);
}
