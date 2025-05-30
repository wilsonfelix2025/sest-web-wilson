import { Component, OnInit, ViewChild, AfterViewInit, Input } from '@angular/core';
import { NotybarService } from '@services/notybar.service';
import { estratigrafiaTypes, estratigrafiaTypesSigla, Estratigrafia } from '@utils/interfaces';
import { ArrayUtils } from '@utils/array';
import { Case } from 'app/repositories/models/case';
import { TableOptions, } from '@utils/table-options-default';
import { NumberUtils } from '@utils/number';

@Component({
  selector: 'sest-estratigrafia',
  templateUrl: './estratigrafia.component.html',
  styleUrls: ['./estratigrafia.component.scss']
})
export class EstratigrafiaComponent implements OnInit, AfterViewInit {
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
  types = estratigrafiaTypes;
  typesKeys = Object.keys(this.types);
  typeSelected: string;
  initials: string;
  description: string = '';
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
    'Tipo',
    'Sigla',
    'Descrição',
  ];

  /**
   * Tipos das colunas na tabela de sapatas.
   */
  columnsTypes: any[] = [
    { readOnly: true, type: 'checkbox', },
    {},
    {},
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
    height: 400,
    width: 560,
    minSpareRows: 1,
    manualColumnResize: [50, , , ,]
  });

  constructor(
    private notybarService: NotybarService,
  ) { }

  ngOnInit() {
    for (const key in this.caseData.estratigrafia.Itens) {
      if (this.caseData.estratigrafia.Itens.hasOwnProperty(key)) {
        this.caseData.estratigrafia.Itens[key].forEach(el => {
          this.tableData.push([
            false,
            String(el.pm.valor),
            estratigrafiaTypes[key],
            el.sigla,
            el.descricao,
          ]);
          this.tableDataInitial.push([
            String(el.pm.valor),
            estratigrafiaTypes[key],
            el.sigla,
            el.descricao,
          ]);
        });
      }
    }
    this.tableData.sort((a, b) => a[1] - b[1]);
    this.tableData.push([null, null, null]);
    this.tableDataInitial.sort((a, b) => a[0] - b[0]);
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
    for (let i = 0; i < 3; i++) {
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

  ngAfterViewInit() {
    this.table = this.tableComponent.getHandsontableInstance();
    this.refreshTable();
  }

  getData() {
    const data: Estratigrafia[] = [];
    this.tableData.sort(this.sortData);

    const filteredData = this.tableData.map(arr => arr.slice(1)).filter(this.filter);

    if (ArrayUtils.equals(this.tableDataInitial, filteredData)) {
      return {};
    }

    filteredData.forEach(el => {
      data.push({
        profundidadeValor: Number(el[0]),
        tipo: estratigrafiaTypesSigla[el[1]],
        sigla: el[2],
        descrição: el[3],
      });
    });

    return {
      estratigrafias: data,
      profundidadeReferênciaEstratigrafia: this.deepSelected,
    };
  }

  addLine() {
    if (this.deepValue !== null && this.deepValue !== undefined &&
      this.typeSelected !== null && this.typeSelected !== undefined &&
      this.initials !== null && this.initials !== undefined) {
      if (this.tableData.findIndex(el => el[1] === this.deepValue && el[2] === this.typeSelected) >= 0) {
        this.notybarService.show('Já existe uma estratigrafia nessa profundidade com esse tipo.', 'danger');
        return;
      }
      this.tableData.push([
        false,
        this.deepValue,
        this.typeSelected,
        this.initials,
        this.description
      ]);
      this.tableData.sort(this.sortData);
      this.table.render();
    } else {
      this.notybarService.show('Profundidade, tipo e iniciais são obrigatórios.', 'danger');
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
