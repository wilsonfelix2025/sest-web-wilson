import { Component, OnInit, ViewChild, AfterViewInit, Input } from '@angular/core';
import { NotybarService } from '@services/notybar.service';
import { ArrayUtils } from '@utils/array';
import { Objetivo } from 'app/repositories/models/trajetoria';
import { Case } from 'app/repositories/models/case';
import { TableOptions } from '@utils/table-options-default';
import { NumberUtils } from '@utils/number';

@Component({
  selector: 'sest-objetivos',
  templateUrl: './objetivos.component.html',
  styleUrls: ['./objetivos.component.scss']
})
export class ObjetivosComponent implements OnInit, AfterViewInit {

  @Input() caseData: Case;

  /**
   * Referência para tabela na tela de litologias.
   */
  @ViewChild('table', { static: true }) tableComponent;

  /**
   * Tabela de litologias.
   */
  table: Handsontable;

  types: string[] = [
    'Primário',
    'Secundário',
  ];
  typeSelected: string;
  deepValue: number;

  lineSelected = [];

  /**
   * Se está marcado para selecionar todos as objetivos.
   */
  selectedAll = false;

  /**
   * Titulos das colunas na tabela de objetivos.
   */
  colHeaders = [
    `<input type='checkbox'' + (this.selectedAll ? 'checked='checked'' : '') + '>`,
    'Tipo',
    'PM (m)',
  ];

  /**
   * Tipos das colunas na tabela de objetivos.
   */
  columnsTypes: any[] = [
    { readOnly: true, type: 'checkbox', },
    {},
    {},
  ];

  /**
   * Dados da tabela de objetivos.
   */
  tableData = [];
  tableDataInitial = [];

  /**
   * Opções de configuração da tabela de Objetivos.
   */
  tableOptions: any = TableOptions.createDefault({
    afterOnCellMouseDown: (event, coords, TD) => {
      // Se clicou no checkbox do título, marca ou desmarca todos as sapatas da tabela.
      if (coords.row < 0 && event.target.type === 'checkbox') {
        this.selectedAll = !event.target.checked;

        this.colHeaders[0] = `<input type='checkbox'' + (this.selectedAll ? 'checked='checked'' : '') + '>`;

        for (let i = 0; i < this.table.countRows() - 1; i += 1) {
          this.table.setDataAtCell(i, 0, this.selectedAll as any);
          if (this.selectedAll) {
            this.lineSelected.push(i);
          } else {
            this.lineSelected = this.lineSelected.filter(el => el !== i);
          }
        }
      } else if (coords.col === 0 && event.target.type === 'checkbox') {
        this.tableData[coords.row][0] = !event.target.checked;
        if (this.tableData[coords.row][0]) {
          this.lineSelected.push(coords.row);
        } else {
          this.lineSelected = this.lineSelected.filter(el => el !== coords.row);
        }
        this.table.render();
      }
    },
    // disableVisualSelection: true,
    minSpareRows: 1,
    manualColumnResize: [50, ,]
  });

  constructor(
    private notybarService: NotybarService,
  ) { }

  ngOnInit() {
    this.caseData.objetivos.forEach(el => {
      this.tableData.push([
        false,
        el.tipoObjetivo,
        String(el.pm),
      ]);
      this.tableDataInitial.push([
        el.tipoObjetivo,
        String(el.pm),
      ]);
    });
    this.tableData.sort((a, b) => a[2] - b[2]);
    this.tableData.push([null, null, null]);
    this.tableDataInitial.sort((a, b) => a[1] - b[1]);
  }

  ngAfterViewInit() {
    this.table = this.tableComponent.getHandsontableInstance();
  }

  sortData(a, b) {
    if (a[2] === null || a[2] === undefined) {
      return 1;
    }

    if (b[2] === null || b[2] === undefined) {
      return -1;
    }

    return a[2] - b[2];
  }

  filter(arr) {
    for (let i = 0; i < arr.length; i++) {
      if (arr[i] === null || arr[i] === undefined) {
        return false;
      }
    }
    return true;
  }

  refreshTable() {
    setTimeout(() => {
      this.table.render();
    }, 1);
  }

  getData() {
    const data: Objetivo[] = [];
    this.tableData.sort(this.sortData);

    const filteredData = this.tableData.map(arr => arr.slice(1)).filter(this.filter);

    if (ArrayUtils.equals(this.tableDataInitial, filteredData)) {
      return {};
    }

    filteredData.forEach(el => {
      data.push({
        tipoObjetivo: el[0],
        pm: Number(el[1]),
      });
    });

    return {
      objetivos: data,
      profundidadeReferênciaObjetivo: 'PM',
    };
  }

  addLine() {
    if (this.deepValue !== null && this.deepValue !== undefined &&
      this.typeSelected !== null && this.typeSelected !== undefined) {
      if (this.tableData.findIndex(el => el[2] === this.deepValue) >= 0) {
        this.notybarService.show('Já existe um objetivo nessa profundidade.', 'danger');
        return;
      }
      this.tableData.push([
        false,
        this.typeSelected,
        this.deepValue,
      ]);
      this.tableData.sort(this.sortData);
      this.table.render();
    } else {
      this.notybarService.show('Preencha tudo antes de adicionar.', 'danger');
    }
  }

  removeLine() {
    this.lineSelected.forEach(el => {
      this.tableData[el] = 'REMOVER';
    });
    this.tableData = this.tableData.filter(el => el !== 'REMOVER');
    this.lineSelected = [];
    this.table.render();
  }

  onKeyPress(event: KeyboardEvent, value: string | number) {
    let inputChar = event.key;

    if (value !== undefined) {
      inputChar = value + inputChar;
    }

    if (!NumberUtils.isNumber(inputChar)) {
      event.preventDefault();
    }
  }
}
