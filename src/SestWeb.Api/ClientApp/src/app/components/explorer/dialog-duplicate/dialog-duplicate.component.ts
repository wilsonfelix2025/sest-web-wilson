import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ToolbarService } from '@services/toolbar.service';
import { ExplorerDataService } from '@services/explorer-data.service';

@Component({
  selector: 'app-dialog-duplicate',
  templateUrl: './dialog-duplicate.component.html',
  styleUrls: ['./dialog-duplicate.component.scss']
})
export class DialogDuplicateComponent implements OnInit {

  initalOpUnit;
  initalOilField;
  initialWell;

  loading = false;

  constructor(public dialogRef: MatDialogRef<DialogDuplicateComponent>,
    private toolbar: ToolbarService,
    private explorer: ExplorerDataService,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

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
    this.toolbar.duplicate(this.data.item, this.data.itemType).then(resp => {
      const obj = {
        name: resp.name,
        id: resp['id'],
        type: resp['fileType'],
        url: `//${resp['id']}/`
      };
      this.initialWell.children.push(obj);
      this.initialWell.filesCount++;
      this.initalOilField.filesCount++;

      this.explorer.treeUpdated.next();

      this.loading = false;
      this.dialogRef.close();
    }).catch(() => { this.loading = false; });
  }
}
