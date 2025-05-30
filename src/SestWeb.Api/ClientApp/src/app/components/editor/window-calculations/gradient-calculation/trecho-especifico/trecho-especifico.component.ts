import { Component, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { DialogService } from '@services/dialog.service';
import { NotybarService } from '@services/notybar.service';
import { TableOptions } from '@utils/table-options-default';
import { Case } from 'app/repositories/models/case';

@Component({
  selector: 'app-trecho-especifico',
  templateUrl: './trecho-especifico.component.html',
  styleUrls: ['./trecho-especifico.component.scss']
})
export class TrechoEspecificoComponent implements OnInit {

  /**
   * Tabela Trecho específico
   */
  tableBase: Handsontable;

  /**
  * Referência para tabela
  */
  @ViewChild('tableBase', { static: true }) tableBaseComponent;

  /**
   * Titulos das colunas na tabela de Trecho específico.
   */
  colHeaders = [
    'PV topo (m)',
    'PV base (m)',
    'Critério de ruptura',
    'Área plastificada (%)',
  ];

  /**
  * Tipos das colunas na tabela de Trecho específico.
  */
 columnsTypes: any[] = [
  { data: 'PV topo (m)'},
  { data: 'PV base (m)'},
  { data: 'Critério de ruptura', type: 'dropdown' },
  { data: 'Área plastificada (%)'},
];

 /**
  * Opções dos Trecho específico
  */
 optionsParameters: any = TableOptions.createDefault({
  width: 650,
  height: 200,
  rowHeaderWidth: 50,
  manualColumnResize: [],
  autoColumnSize: true,
  filters: false,
  allowInvalid: false,
});

/**
  * dataset dos Trecho específico
  */
 datasetBase: any[] = [
  {  },
  {  },
  {  },
 ];

 isValid: boolean = true;

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    public dialog: DialogService,
    public notybarService: NotybarService,
    @Inject(MAT_DIALOG_DATA) public data: { data: { case: Case, calculo?} }
  ) { }

  ngAfterViewInit() {
    this.tableBase = this.tableBaseComponent.getHandsontableInstance();
  }

  ngOnInit() {
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  submit() {

  }

}
