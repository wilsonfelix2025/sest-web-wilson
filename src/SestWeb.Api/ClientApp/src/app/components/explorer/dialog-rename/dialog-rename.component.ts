import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { ToolbarService } from '@services/toolbar.service';

@Component({
  selector: 'app-dialog-rename',
  templateUrl: './dialog-rename.component.html',
  styleUrls: ['./dialog-rename.component.scss']
})
export class DialogRenameComponent implements OnInit {

  newName: string;

  loading = false;

  constructor(public dialogRef: MatDialogRef<DialogRenameComponent>,
    private toolbar: ToolbarService,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  ngOnInit() {
    this.newName = this.data.item.name;
  }

  submit() {
    this.loading = true;
    if (this.newName !== this.data.item.name) {
      this.toolbar.rename(this.data.item, this.data.itemType, this.newName).then(val => {
        this.data.item.name = val['name'];
        this.loading = false;
        this.dialogRef.close();
      }).catch(() => { this.loading = false; });
    } else {
      this.loading = false;
      this.dialogRef.close();
    }
  }
}
