import { Component, OnInit, ViewChild, ViewChildren } from '@angular/core';
import { MatDialogRef, MatSelectionList, MatSelectionListChange } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { TableOptions } from '@utils/table-options-default';

@Component({
  selector: 'app-link-data',
  templateUrl: './link-data.component.html',
  styleUrls: ['./link-data.component.scss']
})
export class LinkDataComponent implements OnInit {

  /**
   * Referência para tabela de perfis conectados
   */
  @ViewChild('table', { static: true }) tableComponent;

  /**
   * Tabela.
   */
  table: Handsontable;

  /**
   * Linhas selecionadas
   */

  lineSelected = [];

  /**
   * Se está marcado para selecionar todos os itens
   */
  selectedAll = false;

  /**
   * Titulos das colunas na tabela Composição.
   */
  baseColHeaders = [
    `<input type='checkbox'' + (this.selectedAll ? 'checked='checked'' : '') + '>`,
    'Tipo',
    'Nome',
  ];

  /**
  * Tipos das colunas na tabela Composição.
  */
  baseColumnsTypes: any[] = [
    { readOnly: true, type: 'checkbox', },
    { data: 'Tipo', },
    { data: 'Nome', },
  ];

  /**
  * dataset da tabela Composição
  */
  baseDataset: {}[] = [
    {},
    {}
  ];

  /**
   * Opções da tabela Composição
   */
  baseOptions: any = TableOptions.createDefault({
    height: 400,
    rowHeaderWidth: 10,
    manualColumnResize: [],
    filters: false,
    fillHandle: {
      autoInsertRow: false,
    }
  });

  /**
   * Dados da tabela
   */
  tableData = [];
  tableDataInitial = [];

  /**
   * Opções de configuração da tabela.
   */
  tableOptions: any = TableOptions.createDefault({
    afterOnCellMouseDown: (event, coords, TD) => {
      // Se clicou no checkbox do título, marca ou desmarca todos as sapatas da tabela.
      if (coords.row < 0 && event.target.type === 'checkbox') {
        this.selectedAll = !event.target.checked;

        this.baseColHeaders[0] = `<input type='checkbox'' + (this.selectedAll ? 'checked='checked'' : '') + '>`;

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
    manualColumnResize: [50, ,],
    width: 300,
  }); 

  /**
   * Nome
   */
  nomePerfil: { value: string, tooltip: string } = { value: '', tooltip: '' };

  /**
   * Descricao
   */
  descricaoPerfil: { value: string, tooltip: string } = { value: '', tooltip: '' };

  /**
   * Perfis
   */ 
  tipoPerfil: any = [
    { label: '', value: '', placeholder: '' },
  ];

  /**
   * Tipos de pocos
   */
  tipoPocos: any = [
    { label: '', value: '', placeholder: '' },
  ];

  /**
   * Tipos de Cabeça de poço
   */
  tipoCabeca: any = [
    { label: '', value: '', placeholder: '' },
  ];

  /**
   * Tipos pasta de perfis
   */
  tipoPastaPerfis: any = [
    { label: '', value: '', placeholder: '' },
  ];

  /**
   * Tipos referencias
   */ 
  tipoReferencias: any = [
    { label: '', value: '', placeholder: '' },
  ];

  /**
   * Tipos Perfis
   */ 
  tipoPerfis = [
    { label: 'ANGAT', value: 'ANGAT', placeholder: '' },
    { label: 'BIOT', value: 'BIOT', placeholder: '' },
    { label: 'DIAM_BROCA', value: 'DIAM_BROCA', placeholder: '' },
    { label: 'CALIP', value: 'CALIP', placeholder: '' },
    { label: 'COESA', value: 'COESA', placeholder: '' },
    { label: 'DTC', value: 'DTC', placeholder: '' },
    { label: 'DTMC', value: 'DTMC', placeholder: '' },
    { label: 'DTMS', value: 'DTMS', placeholder: '' },
    { label: 'DTS', value: 'DTS', placeholder: '' },
  ];

  /**
   * Poco selecionado
   */
  pocoSelecionado = this.tipoPocos[0]

  /**
   * Cabeca Poco selecionado
   */
  cabecaPocoSelecionado = this.tipoCabeca[0]

  /**
   * Pasta Perfis selecionado
   */
  pastaPerfisSelecionado = this.tipoPastaPerfis[0]

  /**
   * Perfis selecionado
   */
  perfisSelecionado = this.tipoPerfis[0]

  /**
   * Referencias selecionado
   */
  referenciasSelecionado = this.tipoReferencias[0]

  nomeCurva: string;
  unidadeCurva: string;
  statusCurva: string;
  valorMin: number;
  valorMax: number;

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    
  ) { }

  @ViewChild(MatSelectionList, {static: true}) perfis: MatSelectionList;

  ngOnInit() {
    this.perfis.selectionChange.subscribe((s: MatSelectionListChange) => {          
        
      this.perfis.deselectAll();
      s.option.selected = true;
    });

  }

  closeModal(): void {
    this.dialogRef.close();
  }

  submit() {

  }

  addLine() {

  }

  removeLine() {

  }

  updateScreenInfo(perfisSelecionado) {
    this.valorMin = 1;
    this.valorMax = 1;
    this.nomeCurva = perfisSelecionado.label;
    this.unidadeCurva = "1";
    this.statusCurva = "1";
  }
  

}
