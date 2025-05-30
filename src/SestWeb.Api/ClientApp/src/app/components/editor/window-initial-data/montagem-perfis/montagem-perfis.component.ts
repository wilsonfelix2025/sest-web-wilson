import { Component, OnInit, ViewChild, AfterViewInit } from '@angular/core';
import { DatasetService } from '@services/dataset/dataset.service';
import { MatDialog } from '@angular/material';
import { Case } from 'app/repositories/models/case';
import { HotTableComponent } from 'ng2-handsontable';
import { keys } from 'highcharts';
import { TableOptions } from '@utils/table-options-default';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';
import { DialogConfirmComponent } from '@components/editor/dialog-confirm/dialog-confirm.component';
import { NumberUtils } from '@utils/number';

@Component({
  selector: 'sest-montagem-perfis',
  templateUrl: './montagem-perfis.component.html',
  styleUrls: ['./montagem-perfis.component.scss']
})
export class MontagemPerfisComponent implements OnInit, AfterViewInit {

  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  cases: Case[];

  profInicial: number = 0;
  profFinal: number = 0;
  profMaxima: number = 0;

  /**
   * Referência para tabela na tela.
   */
  @ViewChild(HotTableComponent, { static: true }) hotTableComponent;

  /**
   * Tabela.
   */
  table: Handsontable;

  lineSelected = [];

  infoIcon = `<span #tooltip="matTooltip" matTooltip="Info about the action" matTooltipPosition="right">&#9432;</span>`;
  data = [
    { info: this.infoIcon, caso: '', cotaTopo1: null, cotaBase1: null, cotaTopo2: null, cotaBase2: null, },
  ];

  /**
   * Titulos das colunas na tabela de montagem de perfis.
   */
  colHeaders = [
    'Info',
    'Nome',
    'Cota Topo (m)',
    'Cota Base (m)',
    'Cota Topo (m)',
    'Cota Base (m)',
  ];

  /**
   * Tipos das colunas na tabela de montagem de perfis.
   */
  columnsTypes: any[] = [
    { readOnly: true, data: 'info', renderer: 'html' },
    { data: 'caso', type: 'dropdown', source: [] },
    { data: 'cotaTopo1', type: 'numeric', numericFormat: { pattern: '0.000' } },
    { data: 'cotaBase1', type: 'numeric', numericFormat: { pattern: '0.000' } },
    { data: 'cotaTopo2', type: 'numeric', numericFormat: { pattern: '0.000' } },
    { data: 'cotaBase2', type: 'numeric', numericFormat: { pattern: '0.000' } },
  ];

  /**
   * Opções de configuração da tabela de montagem de perfis.
   */
  tableOptions: any = TableOptions.createDefault({
    minSpareRows: 1,
    manualColumnResize: [50, 300,],
    height: 300,
    rowHeaderWidth: 25,
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

  comment;

  constructor(
    private dialog: MatDialog,

    private dataset: DatasetService,
    public caseService: CaseDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    this.cases = this.caseService.getAll();

    const limites = this.caseService.getLimiteCaso(this.currCase.id);
    this.profInicial = limites.cotaSup;
    this.profFinal = limites.cotaFinal;
    this.profMaxima = limites.cotaMax;

    // console.log('Montage', this.cases);

    this.columnsTypes[1].source = this.cases.filter(el => el.nome !== this.currCase.nome).map(el => el.nome);

  }

  ngAfterViewInit() {
    this.table = this.hotTableComponent.getHandsontableInstance();

    this.comment = this.table.getPlugin('comments');
  }

  mudarTabela(event) {
    // console.log('mudou', event, this.data);
    if (event[0] !== null && event[0] !== undefined) {
      event[0].forEach(el => {
        if (el[1] === 'caso') {
          this.mudarCasoSelecionado(el[0], el[3]);
        }
        // console.log(el);
        this.data[el[0]].info = this.infoIcon;
      });
      this.table.render();
    }
  }

  mudarCasoSelecionado(linha, novoCaso) {
    const newCase = this.cases.find(el => el.nome === novoCaso);

    this.comment.removeCommentAtCell(linha, 0);

    if (newCase !== undefined) {
      const limites = this.caseService.getLimiteCaso(newCase.id);
      let mensagem = `Cota Inicial: ${limites.cotaSup}\nCota Final: ${limites.cotaFinal}`;

      this.comment.setCommentAtCell(linha, 0, mensagem);
      this.comment.updateCommentMeta(linha, 0, { readOnly: true });
    }

  }

  invalidCell(row, col, message) {
    this.comment.setCommentAtCell(row, col, message);
    this.comment.updateCommentMeta(row, col, { readOnly: true });
    this.table.setCellMetaObject(row, col, { valid: false });
  }

  temSobreposicao(trechoA, trechoB) {
    // console.log('Tem sobreposicao', trechoA, trechoB);
    if (trechoA.base >= trechoB.topo || trechoA.topo <= trechoB.base) {
      return false;
    }
    return true;
  }

  async pegarCasos() {
    const trechos: {
      idCaso: string,
      trechoCorrelacao: { base: number, topo: number },
      trechoTrabalho: { base: number, topo: number },
    }[] = [];

    let valido = true;
    let cancelado = false;

    // this.table.validateCells(() => { });
    this.data.forEach((linha, index) => {
      if (linha.caso !== null && linha.caso !== undefined) {
        const caso = this.cases.find(c => c.nome === linha.caso);
        if (caso !== undefined) {
          this.comment.removeCommentAtCell(index, 1);

          const menor = 'Cota Topo deve ser maior que Cota Base';
          const nulo = 'Não pode ser vazio';
          const limite = 'Precisa estar dentro do trecho de sedimentos';
          const sobreposicao = 'Remova as sobreposicoes';
          keys(linha).forEach((key, i) => {
            if (i >= 2) {
              if (linha[key] === null || linha[key] === undefined) {
                this.invalidCell(index, i, nulo);
                valido = false;
              }
            }
          });
          if (valido) {
            if (linha.cotaTopo1 <= linha.cotaBase1) {
              this.invalidCell(index, 2, menor);
              this.invalidCell(index, 3, menor);
              valido = false;
            } else {
              const limites = this.caseService.getLimiteCaso(caso.id);
              if (linha.cotaTopo1 > limites.cotaSup) {
                this.invalidCell(index, 2, limite);
                valido = false;
              } else {
                this.comment.removeCommentAtCell(index, 2);
              }
              if (linha.cotaBase1 < limites.cotaFinal) {
                this.invalidCell(index, 3, limite);
                valido = false;
              } else {
                this.comment.removeCommentAtCell(index, 3);
              }
            }
            if (linha.cotaTopo2 <= linha.cotaBase2) {
              this.invalidCell(index, 4, menor);
              this.invalidCell(index, 5, menor);
              valido = false;
            } else {
              if (linha.cotaTopo2 > this.profInicial) {
                this.invalidCell(index, 4, limite);
                valido = false;
              } else {
                this.comment.removeCommentAtCell(index, 4);
              }
              if (linha.cotaBase2 < this.profMaxima) {
                this.invalidCell(index, 5, limite);
                valido = false;
              } else {
                this.comment.removeCommentAtCell(index, 5);
              }
            }
            const trecho = {
              idCaso: caso.id,
              trechoCorrelacao: { base: linha.cotaBase1, topo: linha.cotaTopo1 },
              trechoTrabalho: { base: linha.cotaBase2, topo: linha.cotaTopo2 },
            };
            if (valido) {
              trechos.forEach(el => {
                // if (this.temSobreposicao(trecho.trechoCorrelacao, el.trechoCorrelacao)) {
                //   this.invalidCell(index, 2, sobreposicao);
                //   this.invalidCell(index, 3, sobreposicao);
                //   valido = false;
                // }
                if (this.temSobreposicao(trecho.trechoTrabalho, el.trechoTrabalho)) {
                  this.invalidCell(index, 4, sobreposicao);
                  this.invalidCell(index, 5, sobreposicao);
                  valido = false;
                }
              });
            }
            trechos.push(trecho);
          }

        } else {
          this.invalidCell(index, 1, 'Caso inválido');
          valido = false;
        }
      }
    });

    if (valido) {
      trechos.sort((a, b) => b.trechoTrabalho.base - a.trechoTrabalho.base);
      const trechoFinal = trechos[trechos.length - 1].trechoTrabalho.base;
      if (trechoFinal !== this.profMaxima) {
        await this.dialog.open(DialogConfirmComponent, {
          width: '450px', disableClose: true, autoFocus: true,
          data: {
            title: 'Deseja Continuar?',
            description: `A profundidade final da montagem (${trechoFinal}) não coincide com a profundidade máxima da trajetória de projeto (${this.profMaxima}). Deseja continuar?`
          }
        }).afterClosed().toPromise().then(result => { if (!result) { cancelado = true; } });
      }
    }

    this.table.render();

    return { trechos: trechos, valido: valido, cancelado: cancelado };
  }

}
