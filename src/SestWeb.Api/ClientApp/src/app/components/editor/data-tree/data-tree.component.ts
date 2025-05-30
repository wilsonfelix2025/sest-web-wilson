import { Component, OnInit, Input } from '@angular/core';
import { MatSidenav } from '@angular/material';
import { SynchronizeChartsService } from '@services/synchronize-charts.service';
import { DialogService } from '@services/dialog.service';
import { ImportCaseComponent } from '../import/import-case/import-case.component';
import { Case } from 'app/repositories/models/case';

@Component({
  selector: 'sest-data-tree',
  templateUrl: './data-tree.component.html',
  styleUrls: ['./data-tree.component.scss']
})
export class DataTreeComponent {

  /**
  * A lista de casos de estudo aberta atualmente
  */
  openedCases: Case[];

  /**
   * O caso de estudo aberto atualmente
   */
  currCase: Case;

  /**
   * A reference to Material's sidenav object.
   */
  @Input() sidenav: MatSidenav;
  constructor(
    public sync: SynchronizeChartsService,
    public dialog: DialogService,
  ) {

  }

  openWindowImportCase() {
    this.dialog.openPageDialog(ImportCaseComponent, { minHeight: 520, minWidth: 600 });
  }
}
