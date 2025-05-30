import { Component, Input, OnChanges, OnInit, ViewChild } from '@angular/core';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { NumberUtils } from '@utils/number';
import { TipoPerfil } from '@utils/perfil/tipo-perfil';
import { TableOptions } from '@utils/table-options-default';
import { UNSET } from '@utils/vazio';
import { TrechoRegistroUpdate } from 'app/repositories/models/registro-evento';
import { HotTableComponent } from 'ng2-handsontable';

@Component({
  selector: 'sest-records-table',
  templateUrl: './records-table.component.html',
  styleUrls: ['./records-table.component.scss']
})
export class RecordsTableComponent implements OnInit, OnChanges {

  @Input() info: {
    nome: string,
    trechos: (TrechoRegistroUpdate & { nome: string })[],
    unidadeGeral?: string,
    tipos: { tipo?, nome?: string, id: string, unidade?: string }[],

    valid: boolean,
    colHeaders: string[],
    colTypes: any[],
    pv: boolean,
    unidade?: boolean
  };

  /**
   * Opções das tabelas
   */
  baseOptions: any = TableOptions.createDefault({
    height: 350,
    rowHeaderWidth: 10,
    manualColumnResize: [],
    filters: false,
    minSpareRows: 1,
    contextMenu: {
      items: {
        'row_above': { name: 'Inserir linha acima' },
        'row_below': { name: 'Inserir linha abaixo' },
        'remove_row': { name: 'Remover linha' },
        'cut': { name: 'Cortar' },
        'copy': { name: 'Copiar' }
      }
    },
    afterChange: (changes) => {
      if (!UNSET(changes) && this.info.unidade && !UNSET(this.table)) {
        changes.forEach(([row, prop, oldValue, newValue]) => {
          if (prop === 'nome') {
            this.atualizarListaUnidades(row);
          } else if (prop === 'unidade') {
            this.atualizarUnidadeRegistro(row, newValue);
          }
        });
        this.table.render();
      }
    },
  });

  /**
  * Boleanos
  */
  unidadeGlobal: boolean = true;

  isValid: boolean = true;
  psi: boolean = false;

  /**
   * Referência para tabela na tela.
   */
  @ViewChild(HotTableComponent, { static: true }) hotTableComponent;

  /**
   * Tabela de Registros de Pressão de Poros
   */
  table: Handsontable;

  dadosPoco = {
    pmMax: 0,
    pmSup: 0,
    pvMax: 0,
    pvSup: 0,
  };


  constructor(
    private dataset: DatasetService,
    private caseDataset: CaseDatasetService,
  ) { }

  ngAfterViewInit() {
    this.table = this.hotTableComponent.getHandsontableInstance();
    setTimeout(() => {
      this.atualizar();
      this.validateTable();
    }, 200);
  }

  ngOnInit() {
    this.dadosPoco = this.caseDataset.getLimiteCaso(this.dataset.currCaseId);
    this.mudarPv();
  }

  ngOnChanges(changes) {
    if (changes.info) {
      if (this.info.unidadeGeral) {
        this.unidadeGlobal = true;
        this.psi = this.info.unidadeGeral === 'psi';
      } else {
        this.unidadeGlobal = false;
      }
    }
  }

  mudarPv() {
    this.info.colTypes.find(el => el.data === 'pm').readOnly = this.info.pv;
    this.info.colTypes.find(el => el.data === 'pv').readOnly = !this.info.pv;
    if (this.table) {
      this.table.updateSettings({ columns: this.info.colTypes }, true);
      this.table.render();
    }
  }

  mudarUnidade() {
    this.info.unidadeGeral = this.psi ? 'psi' : 'lb/gal';
  }

  atualizar() {
    if (!UNSET(this.table) && this.info.unidade) {
      this.info.trechos.forEach((trecho, nRow) => {
        this.atualizarListaUnidades(nRow);
      });
      this.table.render();
    }
  }

  atualizarListaUnidades(nRow) {
    const trecho = this.info.trechos[nRow];
    if (!UNSET(trecho.nome)) {
      const tipo = this.info.tipos.find(el => {
        return el.tipo.descrição === trecho.nome;
      });
      if (!UNSET(tipo)) {
        if (tipo.unidade) {
          trecho.unidade = tipo.unidade;
        } else {
          trecho.unidade = '';
        }
        const tipoPerfil: TipoPerfil = tipo.tipo;
        const unidades = tipoPerfil.grupoUnidade.unidadesDeMedida.map(el => el.símbolo);
        this.table.setCellMetaObject(nRow, this.table.propToCol('unidade'), { source: unidades });
      }
    }
  }

  atualizarUnidadeRegistro(nRow, unidade) {
    const trecho = this.info.trechos[nRow];
    if (!UNSET(trecho.nome)) {
      const tipo = this.info.tipos.find(el => {
        return el.tipo.descrição === trecho.nome;
      });
      if (!UNSET(tipo)) {
        tipo.unidade = unidade;

        this.info.trechos.forEach(el => {
          if (el.nome === trecho.nome) {
            el.unidade = unidade;
          }
        })
      }
    }
  }

  validateTable() {
    this.info.trechos.sort((a, b) => UNSET(a.pm) ? 1 : UNSET(b.pm) ? -1 : Number(a.pm) - Number(b.pm));
    let valid = true;
    for (let i = 0; i < this.info.trechos.length; i++) {
      if (!UNSET(this.info.trechos[i].nome) || !UNSET(this.info.trechos[i].pm) || !UNSET(this.info.trechos[i].pv) || !UNSET(this.info.trechos[i].valor)) {
        TableOptions.removeError(this.table, i, 'nome');
        TableOptions.removeError(this.table, i, 'pm');
        TableOptions.removeError(this.table, i, 'pv');
        TableOptions.removeError(this.table, i, 'valor');
        TableOptions.removeError(this.table, i, 'unidade');
        let error = '';
        if (this.info.pv) {
          if (UNSET(this.info.trechos[i].pv)) {
            error = 'Precisa estar preenchido';
            TableOptions.setError(this.table, i, 'pv', error);
          } else if (!NumberUtils.isNumber(this.info.trechos[i].pv)) {
            error = 'Precisa ser um número';
            TableOptions.setError(this.table, i, 'pv', error);
          } else if (this.info.trechos[i].pv < this.dadosPoco.pvSup || this.info.trechos[i].pv > this.dadosPoco.pvMax) {
            error = 'PV deve estar dentro dos limites de sedimentos';
            TableOptions.setError(this.table, i, 'pv', error);
          }
        } else {
          if (UNSET(this.info.trechos[i].pm)) {
            error = 'Precisa estar preenchido';
            TableOptions.setError(this.table, i, 'pm', error);
          } else if (!NumberUtils.isNumber(this.info.trechos[i].pm)) {
            error = 'Precisa ser um número';
            TableOptions.setError(this.table, i, 'pm', error);
          } else if (this.info.trechos[i].pm < this.dadosPoco.pmSup || this.info.trechos[i].pm > this.dadosPoco.pmMax) {
            error = 'PM deve estar dentro dos limites de sedimentos';
            TableOptions.setError(this.table, i, 'pm', error);
          }
        }
        if (this.info.colTypes[0].data === 'nome') {
          if (UNSET(this.info.trechos[i].nome)) {
            error = 'Precisa estar preenchido';
            TableOptions.setError(this.table, i, 'nome', error);
          } else if (!this.info.colTypes[0].source.includes(this.info.trechos[i].nome)) {
            error = 'Precisa ser um valor válido';
            TableOptions.setError(this.table, i, 'nome', error);
          }

          else if (this.info.unidade) {
            const tipo: TipoPerfil = this.info.tipos.find(el => {
              return el.tipo.descrição === this.info.trechos[i].nome;
            }).tipo;
            const unidades = tipo.grupoUnidade.unidadesDeMedida.map(el => el.símbolo);
            if (UNSET(this.info.trechos[i].unidade)) {
              error = 'Precisa estar preenchido';
              TableOptions.setError(this.table, i, 'unidade', error);
            } else if (!unidades.includes(this.info.trechos[i].unidade)) {
              error = 'Precisa ser uma unidade válida';
              TableOptions.setError(this.table, i, 'unidade', error);
            }
          }
        }

        if (UNSET(this.info.trechos[i].valor)) {
          error = 'Precisa estar preenchido';
          TableOptions.setError(this.table, i, 'valor', error);
        } else if (!NumberUtils.isNumber(this.info.trechos[i].valor)) {
          error = 'Precisa ser um número';
          TableOptions.setError(this.table, i, 'valor', error);
        }
        if (error !== '') {
          valid = false;
        }
      }
      if (this.table) {
        this.table.render();
      }
    }

    this.info.valid = valid;
    return valid;
  }

}
