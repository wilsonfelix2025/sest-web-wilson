import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ToolbarService } from '@services/toolbar.service';
import { ExplorerDataService } from '@services/explorer-data.service';

@Component({
  selector: 'app-dialog-move',
  templateUrl: './dialog-move.component.html',
  styleUrls: ['./dialog-move.component.scss']
})
export class DialogMoveComponent implements OnInit {

  operationalUnits = [];
  selectedOpUnit;
  selectedOilField;
  selectedWell;
  initalOpUnit;
  initalOilField;
  initialWell;

  loading = false;

  constructor(public dialogRef: MatDialogRef<DialogMoveComponent>,
    public toolbar: ToolbarService,
    public explorer: ExplorerDataService,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  ngOnInit() {
    this.operationalUnits = this.explorer.operationalUnits;
    const aux = this.explorer.getCurrPath(this.data.item.id, this.data.itemType);

    this.selectedWell = aux.currlWell;
    this.selectedOilField = aux.currOilField;
    this.selectedOpUnit = aux.currOpUnit;
    this.initialWell = aux.currlWell;
    this.initalOilField = aux.currOilField;
    this.initalOpUnit = aux.currOpUnit;
  }

  resetSelection(e) {
    if (e === 'opUnit') {
      this.selectedOilField = this.selectedOpUnit.children[0];
    }
    this.selectedWell = this.selectedOilField.children[0];
  }

  submit() {
    this.loading = true;
    this.toolbar.move(this.data.item, this.data.itemType, this.selectedWell.id).then(val => {
      this.initialWell.children = this.initialWell.children.filter(el => el !== this.data.item);
      this.initialWell.filesCount--;
      this.initalOilField.filesCount--;
      this.initalOpUnit.filesCount--;

      this.selectedWell.children.push(this.data.item);
      this.selectedWell.filesCount++;
      this.selectedOilField.filesCount++;
      this.selectedOpUnit.filesCount++;

      this.explorer.treeUpdated.next();
      this.loading = false;
      this.dialogRef.close();
    }).catch(() => { this.loading = false; });

  }
}
