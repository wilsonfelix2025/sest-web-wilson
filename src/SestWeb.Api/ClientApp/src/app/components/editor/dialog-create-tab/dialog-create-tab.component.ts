import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { DatasetService } from '@services/dataset/dataset.service';
import { TabsDatasetService } from '@services/dataset/state/tabs-dataset.service';

@Component({
  selector: 'app-dialog-create-tab',
  templateUrl: './dialog-create-tab.component.html',
  styleUrls: ['./dialog-create-tab.component.scss']
})
export class DialogCreateTabComponent {

  newTabName: string = '';

  constructor(
    public dialogRef: MatDialogRef<DialogCreateTabComponent>,
    private tabsDatasetService: TabsDatasetService,
    private datasetService: DatasetService,
  ) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  submit() {
    this.tabsDatasetService.create(this.newTabName, this.datasetService.currCaseId);
    this.dialogRef.close();
  }

}
