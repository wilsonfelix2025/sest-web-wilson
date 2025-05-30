import { Component, ViewChild, OnInit } from '@angular/core';
import { MatAccordion, MatMenuTrigger } from '@angular/material';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { DialogService } from '@services/dialog.service';
import { CaseService } from 'app/repositories/case.service';
import { NewDialogDeleteComponent } from '../new-dialog-delete/new-dialog-delete.component';
import { WindowEditCaseDataComponent } from '../window-edit-case-data/window-edit-case-data.component';
import { WindowInitialDataComponent } from '../window-initial-data/window-initial-data.component';

@Component({
  selector: 'sest-accordion-tree',
  templateUrl: './accordion-tree.component.html',
  styleUrls: ['./accordion-tree.component.scss']
})

export class AccordionTreeComponent implements OnInit {
  /**
   * Expose the accordion's template element to the class.
   */
  @ViewChild('accordion', { static: true }) accordion: MatAccordion;
  /**
   * Determine the display mode of the accordion on the template.
   *
   * A literal could be used directly, but I'm keeping this for possible
   * scenarios where it might be necessary to programatically change
   * its value.
   */
  displayMode: string = 'default';
  /**
   * A list with all currently opened files.
   */
  openFiles: any = [];

  @ViewChild(MatMenuTrigger, { static: false })
  contextMenu: MatMenuTrigger;
  contextMenuPosition = { x: '0px', y: '0px' };

  indexCase: string;

  constructor(
    public dialog: DialogService,

    private caseService: CaseService,
    private dataset: DatasetService,
    private caseDataset: CaseDatasetService,
  ) {
    this.openFiles = this.caseDataset.getAll();
    this.indexCase = this.dataset.currCaseId;

    this.caseDataset.$currCaseLoaded.subscribe((res) => {
      this.openFiles = this.caseDataset.getAll();
      this.indexCase = res;
    });
    this.caseDataset.$caseAdded.subscribe((res) => {
      this.openFiles.push(res);
    });
    this.caseDataset.$caseRemoved.subscribe((res) => {
      this.openFiles = this.openFiles.filter(el => el.id !== res);
    });
  }

  ngOnInit() {
    this.openFiles = this.caseDataset.getAll();
    this.indexCase = this.dataset.currCaseId;
  }

  openDialogEditData(id: string) {
    this.dialog.openPageDialog(
      WindowEditCaseDataComponent,
      { minHeight: 0, minWidth: 900 },
      { caseId: id }
    );
  }
  openDialogInitData(id: string) {
    this.dialog.openPageDialog(
      WindowInitialDataComponent,
      { minHeight: 0, minWidth: 900 },
      { caseId: id }
    );
  }
  openDialogRemoveCase(id) {
    const callback = () => {
      return this.caseService.removeSupport(this.indexCase, id);
    };
    const postCallback = () => {
      this.caseDataset.remove(id);
    };

    this.dialog.openDialog(NewDialogDeleteComponent, {
      submitCallback: callback,
      postSubmitCallback: postCallback
    });
  }

}
