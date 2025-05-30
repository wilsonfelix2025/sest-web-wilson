import { Component, OnInit, Output, EventEmitter, Input, ViewChild, AfterViewInit } from '@angular/core';
import { HotTableComponent } from 'ng2-handsontable';
import { DialogService } from '@services/dialog.service';
import { TableOptions } from '@utils/table-options-default';
import { DatasetService } from '@services/dataset/dataset.service';
import { Case } from 'app/repositories/models/case';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';

@Component({
  selector: 'app-data-set',
  templateUrl: './data-set.component.html',
  styleUrls: ['./data-set.component.scss']
})
export class DataSetComponent implements OnInit, AfterViewInit {

  @Output() afterChangeTable = new EventEmitter<any>();
  @Output() afterCreateCol = new EventEmitter<any>();
  @Output() changeColumn = new EventEmitter<any>();
  @Output() buttonClicked = new EventEmitter<any>();

  /**
   * Opcoes adicionais no menu de contexto
   */
  @Input() contextMenuOptions = {};

  /**
   * Tipo das celulas da tabela
   */
  @Input() tableType;

  /**
   * Formato das celulas se for numericos da tabela
   */
  @Input() numericFormat;

  /**
   * Validação geral da tabela
   */
  @Input() tableValidator;

  /**
   * Tipos de celulas que são fora do padrão.
   */
  @Input() fixedColumnsLeft: number = 0;

  /**
   * Tipos de celulas que são fora do padrão.
   */
  @Input() fixedRowsTop: number = 0;
  /**
   * Dados da tabela.
   */
  @Input() data: any[][];
  /**
   * Quais celulas da tabela não são editáveis.
   */
  @Input() tableReadOnlyData: any[][];
  /**
   * Tipos de celulas que são fora do padrão.
   */
  @Input() defaultCellTypes: any[];
  /**
   * Titulos das colunas na tabela.
   */
  @Input() colHeaders;
  @Input() colHeaderFormatter;
  /**
   * Tipos das colunas na tabela.
   */
  @Input() columnsTypes;
  /**
   * Titulos das linhas na tabela.
   */
  @Input() rowHeaders;
  /**
   * Se existe a opção de selecionar tipo cota.
   */
  @Input() cota = true;
  /**
   * Referência para tabela na tela.
   */
  @ViewChild(HotTableComponent, { static: true }) hotTableComponent;

  /**
   * Os dados do poço aberto atualmente.
   */
  currCase: Case;

  /**
   * Tabela.
   */
  hotTable: Handsontable;

  tableDeepTypes = ['PM', 'Cota'];
  tableDeepType = this.tableDeepTypes[0];

  @Input() rotaryTable: number = null;
  @Output() rotaryTableChange = new EventEmitter<number>();

  options: any = TableOptions.createDefault({
    fixedColumnsLeft: 0,
    fixedRowsTop: 0,
    rowHeaderWidth: 60,
    rowHeaders: [],
    minSpareRows: 1,
    contextMenu: {
      items: {
        'row_above': {
          disabled: function () {
            return this.getSelectedLast()[0] <= 1;
          }
        },
        'row_below': {
          disabled: function () {
            return this.getSelectedLast()[0] < 1;
          }
        },
        'remove_row': {
          disabled: function () {
            return this.getSelectedLast()[0] <= 1;
          }
        },
        'sep1': { name: '---------' },
        'cut': {
          disabled: function () {
            return this.getSelectedLast()[0] <= 1;
          }
        },
        'copy': {}
      }
    }
  });

  isValid = true;

  dadosPoco = {
    pmMax: 0,
    pmSup: 0,
    cotaFinal: 0,
    cotaSup: 0,
    cotaMax: 0,
  };

  constructor(
    public dialog: DialogService,

    private dataset: DatasetService,
    private caseDataset: CaseDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);
    this.dadosPoco = this.caseDataset.getLimiteCaso(this.dataset.currCaseId);

    if (this.defaultCellTypes) {
      this.options.cell = this.defaultCellTypes;
    } else {
      const cellReadOnly = [];
      for (let i = 0; i < this.tableReadOnlyData.length; i++) {
        for (let j = 0; j < this.tableReadOnlyData[0].length; j++) {
          cellReadOnly.push({ row: i, col: j, readOnly: true, type: 'text' });
        }
      }
      this.options.cell = cellReadOnly;
    }
    // this.options.startCols = this.colHeaders.length;
    this.options.colHeaders = this.colHeaders;
    this.options.rowHeaders = this.rowHeaders;
    this.options.columns = this.columnsTypes;
    this.options.data = this.data;
    // this.options.validator = this.tableValidator;
    this.options.contextMenu.items = Object.assign(this.contextMenuOptions, this.options.contextMenu.items);
    this.options.fixedColumnsLeft = this.fixedColumnsLeft;
    if (this.tableType) {
      this.options.type = this.tableType;
      this.options.numericFormat = this.numericFormat;
    }
    this.options.fixedRowsTop = this.fixedRowsTop;
  }

  ngAfterViewInit() {
    this.hotTable = this.hotTableComponent.getHandsontableInstance();
    this.hotTable.validateCells(() => { });
  }

  afterOnCellMouseDown(event) {
    const e = event[0];
    const coords = event[1];
    if (coords.row < 0 && e.target.attributes.id && e.target.classList.contains('button')) {
      const obj = {
        position: coords.col,
        name: e.target.attributes.id.nodeValue,
        context: this,
        table: this.hotTable
      };
      this.buttonClicked.emit(obj);
    }
  }

  setProperties(data: {
    table,
    importType: 'new' | 'append' | 'overwrite',
    position: number,
    name: string, type: string, typeDescription: string, unit: string, top: number, bottom: number
  }) {
    data.table = this.hotTable;
    this.changeColumn.emit(data);
  }

  changedTableDeepType() {
    this.hotTable.setDataAtCell(0, 0, this.tableDeepType);
  }

  afterCreateColEvent(event) {
    this.afterCreateCol.emit({ change: event, table: this.hotTable });
  }

  afterChangeTableEvent(event) {
    this.afterChangeTable.emit({ change: event, table: this.hotTable });
  }
}
