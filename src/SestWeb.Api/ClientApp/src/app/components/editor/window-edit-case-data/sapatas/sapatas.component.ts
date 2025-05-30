import { Component, OnInit, ViewChild, AfterViewInit, Input } from '@angular/core';
import { NotybarService } from '@services/notybar.service';
import { ArrayUtils } from '@utils/array';
import { Case } from 'app/repositories/models/case';
import { Sapata } from 'app/repositories/models/trajetoria';
import { TableOptions } from '@utils/table-options-default';
import { NumberUtils } from '@utils/number';

@Component({
  selector: 'sest-sapatas',
  templateUrl: './sapatas.component.html',
  styleUrls: ['./sapatas.component.scss']
})
export class SapatasComponent implements OnInit, AfterViewInit {

  @Input() caseData: Case;

  /**
   * Referência para tabela na tela de litologias.
   */
  @ViewChild('table', { static: true }) tableComponent;

  /**
   * Tabela de litologias.
   */
  table: Handsontable;

  deepSelected: 'PM' | 'PV' = 'PM';
  diameters: string[] = [
    'Outros',
    '3 3/4',
    '4 1/8', '4 1/6', '4 3/4',
    '5 1/2', '5 5/8', '5 3/4', '5 7/8', '5 15/16',
    '6', '6 1/8', '6 1/4', '6 3/4',
    '7', '7 5/8',
    '8 1/8', '8 1/4', '8 3/8', '8 1/2', '8 3/4',
    '9 1/2', '9 5/8', '9 7/8',
    '10 1/2', '10 3/4',
    '12', '12 1/4',
    '13 3/8', '13 1/2', '13 5/8', '13 3/4',
    '14', '14 3/4',
    '15',
    '16',
    '17 1/2',
    '18', '18 1/2',
    '20',
    '22',
    '23',
    '24',
    '26',
    '28',
    '29',
    '30',
    '32',
    '36',
    '42',
  ];
  diameterSelected: string;
  diameterValue: string;
  deepValue: number;

  lineSelected = [];

  /**
   * Se está marcado para selecionar todos as sapatas.
   */
  selectedAll = false;

  /**
   * Titulos das colunas na tabela de sapatas.
   */
  colHeaders = [
    `<input type='checkbox'' + (this.selectedAll ? 'checked='checked'' : '') + '>`,
    'PM (m)',
    'Diâmetro (pol)',
  ];

  /**
   * Tipos das colunas na tabela de sapatas.
   */
  columnsTypes: any[] = [
    { readOnly: true, type: 'checkbox', },
    {},
    {},
  ];

  /**
   * Dados da tabela de sapatas.
   */
  tableData = [];
  tableDataInitial = [];

  /**
   * Opções de configuração da tabela de sapatas.
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
    minSpareRows: 1,
    manualColumnResize: [50, ,]
  });

  constructor(
    private notybarService: NotybarService,
  ) { }

  ngOnInit() {
    this.caseData.sapatas.forEach(el => {
      this.tableData.push([
        false,
        String(el.pm),
        el.diâmetro,
      ]);
      this.tableDataInitial.push([
        String(el.pm),
        el.diâmetro,
      ]);

    });
    this.tableData.sort((a, b) => a[1] - b[1]);
    this.tableData.push([null, null, null]);
    this.tableDataInitial.sort((a, b) => a[0] - b[0]);
  }

  ngAfterViewInit() {
    this.table = this.tableComponent.getHandsontableInstance();
  }

  sortData(a, b) {
    if (a[1] === null || a[1] === undefined) {
      return 1;
    }

    if (b[1] === null || b[1] === undefined) {
      return -1;
    }

    return a[1] - b[1];
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
    const data: Sapata[] = [];
    this.tableData.sort(this.sortData);

    const filteredData = this.tableData.map(arr => arr.slice(1)).filter(this.filter);

    if (ArrayUtils.equals(this.tableDataInitial, filteredData)) {
      return {};
    }

    filteredData.forEach(el => {
      data.push({
        pm: Number(el[0]),
        diâmetro: el[1],
      });
    });

    return {
      sapatas: data,
      profundidadeReferênciaSapata: this.deepSelected,
    };
  }

  addLine() {
    if (this.deepValue !== null && this.deepValue !== undefined &&
      this.diameterSelected !== null && this.diameterSelected !== undefined) {
      if (this.tableData.findIndex(el => el[1] === this.deepValue) >= 0) {
        this.notybarService.show('Já existe uma sapata nessa profundidade.', 'danger');
        return;
      }
      this.tableData.push([
        false,
        this.deepValue,
        this.diameterSelected === 'Outros' ? this.diameterValue : this.diameterSelected
      ]);
      this.tableData.sort(this.sortData);
      this.table.render();
    } else {
      this.notybarService.show('Preencha tudo antes de adicionar.', 'danger');
    }
  }

  changeDeepType() {
    this.colHeaders[1] = this.deepSelected;
    this.table.render();
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
