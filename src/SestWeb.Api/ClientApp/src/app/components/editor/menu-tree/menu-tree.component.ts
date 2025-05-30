import { Component, ViewChild } from '@angular/core';
import { DialogService } from '@services/dialog.service';
import { WindowEditCaseDataComponent } from '../window-edit-case-data/window-edit-case-data.component';
import { WindowInitialDataComponent } from '../window-initial-data/window-initial-data.component';
import { MatMenuTrigger } from '@angular/material';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';

@Component({
  selector: 'sest-menu-tree',
  templateUrl: './menu-tree.component.html',
  styleUrls: ['./menu-tree.component.scss']
})
export class MenuTreeComponent {

  /**
   * A list with all currently opened files.
   */
  openFiles: any = [];

  @ViewChild(MatMenuTrigger, { static: false })
  contextMenu: MatMenuTrigger;
  contextMenuPosition = { x: '0px', y: '0px' };

  constructor(
    public dialog: DialogService,

    private caseDataset: CaseDatasetService,
  ) {
    this.getInfo();

    this.caseDataset.$currCaseLoaded.subscribe((res) => {
      this.getInfo();
    });
    this.caseDataset.$caseAdded.subscribe((res) => {
      this.getInfo();
    });
    this.caseDataset.$caseRemoved.subscribe((res) => {
      this.getInfo();
    });
  }

  getInfo() {
    this.openFiles = this.caseDataset.getAll();
  }

  openDialogEditData() {
    this.dialog.openPageDialog(WindowEditCaseDataComponent, { minHeight: 0, minWidth: 900 });
  }
  openDialogInitData() {
    this.dialog.openPageDialog(WindowInitialDataComponent, { minHeight: 0, minWidth: 900 });
  }
}
