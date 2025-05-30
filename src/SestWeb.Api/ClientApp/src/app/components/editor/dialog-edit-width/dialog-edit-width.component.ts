import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-dialog-edit-width',
  templateUrl: './dialog-edit-width.component.html',
  styleUrls: ['./dialog-edit-width.component.scss']
})
export class DialogEditWidthComponent implements OnInit {

  width: number;

  constructor(public dialogRef: MatDialogRef<DialogEditWidthComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit() {
    this.width = this.data.parameters.width;
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  submit() {
    const parameters = { index: this.data.parameters.index, width: this.width }
    this.data.classe[this.data.operation](parameters);
    this.dialogRef.close();
  }

}
