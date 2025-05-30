import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ToolbarService } from '@services/toolbar.service';
import { ExplorerDataService } from '@services/explorer-data.service';

@Component({
  selector: 'app-dialog-delete',
  templateUrl: './dialog-delete.component.html',
  styleUrls: ['./dialog-delete.component.scss']
})
export class DialogDeleteComponent implements OnInit {

  initalOpUnit;
  initalOilField;
  initialWell;

  loading = false;

  constructor(public dialogRef: MatDialogRef<DialogDeleteComponent>,
    private toolbar: ToolbarService,
    private explorer: ExplorerDataService,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  ngOnInit() {
    const aux = this.explorer.getCurrPath(this.data.item.id, this.data.itemType);

    this.initialWell = aux.currlWell;
    this.initalOilField = aux.currOilField;
    this.initalOpUnit = aux.currOpUnit;
  }

  submit() {
    this.loading = true;
    this.toolbar.delete(this.data.item, this.data.itemType).then(val => {
      this.initialWell.children = this.initialWell.children.filter(el => el !== this.data.item);
      this.initialWell.filesCount--;
      this.initalOilField.filesCount--;
      this.initalOpUnit.filesCount--;

      this.explorer.fileDeleted.next();
      this.explorer.treeUpdated.next();
      this.loading = false;
      this.dialogRef.close();
    }).catch(() => { this.loading = false; });
  }
}
