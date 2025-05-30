import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { DatasetService } from '@services/dataset/dataset.service';
import { RegisterEventDatasetService } from '@services/dataset/register-event-dataset.service';
import { NotybarService } from '@services/notybar.service';
import { TableOptions } from '@utils/table-options-default';
import { Case } from 'app/repositories/models/case';

@Component({
  selector: 'sest-records-export',
  templateUrl: './records-export.component.html',
  styleUrls: ['./records-export.component.scss']
})
export class RecordsExportComponent implements OnInit, AfterViewInit {

  /**
   * Referência para tabela na tela.
   */
  @ViewChild('table', { static: true }) tableComponent;

  /**
   * Tabela EXPORT PROFILES.
   */
  table: Handsontable;

  /**
   * Se está marcado para selecionar todos os perfis.
   */
  selectedAll = false;

  /**
   * Opções da tebela.
   */
  optionsParameters: any = TableOptions.createDefault({
    afterOnCellMouseDown: (event, coords) => {
      if (event.target.type === 'checkbox') {
        // Se clicou no checkbox do título, marca ou desmarca todos as linhas da tabela.
        if (coords.row < 0) {
          this.selectedAll = !event.target.checked;

          this.colHeaders[0] = `<input type='checkbox' ${this.selectedAll ? 'checked="checked"' : ''
            }>`;

          console.log('select all', this.selectedAll);
          for (let i = 0; i < this.table.countRows(); i += 1) {
            this.table.setDataAtCell(i, 0, this.selectedAll as any);
          }
        }
      }
    },
    width: 350,
    height: 410,
    rowHeaderWidth: 30,
    autoColumnSize: true,
    filters: false,
    disableVisualSelection: true,
    manualColumnResize: [30, , 75],
  });

  /**
   * Titulos das colunas na tabela.
   */
  colHeaders = [
    `<input type='checkbox' ${this.selectedAll ? 'checked="checked"' : ''}>`,
    'Nome',
    'Tipo',
  ];

  /**
   * Tipos das colunas na tabela.
   */
  columnsTypes: any[] = [
    { data: 'selected', type: 'checkbox' },
    { data: 'nome', readOnly: true },
    { data: 'tipo', readOnly: true },
  ];

  /**
   * dataset da tabela
   */
  tableData: { selected?: boolean; nome: string; tipo: string; id: string }[] = [];

  constructor(
    private dataset: DatasetService,
    private registerEventDataset: RegisterEventDatasetService,
    public notybarService: NotybarService
  ) { }

  ngOnInit() {
    this.tableData = this.registerEventDataset.getAll(this.dataset.currCaseId).filter(el => el.trechos.length > 0).map(el => {
      return { selected: false, nome: el.nome, tipo: el.tipo, id: el.id }
    });
  }

  ngAfterViewInit() {
    this.table = this.tableComponent.getHandsontableInstance();
  }

  getRegistros() {
    const registros: string[] = this.tableData.filter(el => el.selected).map(el => el.nome);

    console.log('Export', registros);

    if (registros.length < 1) {
      this.notybarService.show(
        'Nada foi selecionado para exportar...',
        'warning'
      );
      return;
    }

    return registros;
  }

}
