import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { DialogService } from '@services/dialog.service';
import { NotybarService } from '@services/notybar.service';
import { TableOptions } from '@utils/table-options-default';

@Component({
  selector: 'app-no-convergence',
  templateUrl: './no-convergence.component.html',
  styleUrls: ['./no-convergence.component.scss']
})
export class NoConvergenceComponent implements OnInit {

  /**
   * Titulos das colunas na tabela de Trecho específico.
   */
  colHeaders = [
    'PM (m)',
    'PV (m)',
    'GCOLI (lb/gal)',
    'GCOLS (lb/gal)',
    'GQuebra (lb/gal)',
    'Observação',
  ];

  /**
  * Tipos das colunas na tabela de Trecho específico.
  */
  columnsTypes: any[] = [
    { data: 'pm', type: 'numeric', numericFormat: { pattern: '0[.]00' }, readOnly: true },
    { data: 'pv', type: 'numeric', numericFormat: { pattern: '0[.]00' }, readOnly: true },
    { data: 'gcoli', type: 'numeric', numericFormat: { pattern: '0[.]00' }, readOnly: true },
    { data: 'gcols', type: 'numeric', numericFormat: { pattern: '0[.]00' }, readOnly: true },
    { data: 'gquebra', type: 'numeric', numericFormat: { pattern: '0[.]00' }, readOnly: true },
    { data: 'obs', readOnly: true },
  ];

  /**
   * Opções dos Trecho específico
   */
  optionsParameters: any = TableOptions.createDefault({
    width: 1000,
    height: 340,
    rowHeaderWidth: 50,
    manualColumnResize: [],
    autoColumnSize: true,
    filters: false,
    allowInvalid: false,
    className: 'htLeft htMiddle',
  });

  /**
    * dataset dos Trecho específico
    */
  datasetBase: any[] = [];

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    public dialog: DialogService,
    public notybarService: NotybarService,
    @Inject(MAT_DIALOG_DATA) public data: { data: { info } }
  ) { }

  ngOnInit() {
    this.datasetBase = this.data.data.info;
  }

  closeModal(): void {
    this.dialogRef.close();
  }

}
