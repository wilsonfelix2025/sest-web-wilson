import { Component, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-dialog-delete-home',
  templateUrl: './dialog-delete-home.component.html',
  styleUrls: ['./dialog-delete-home.component.scss']
})
export class DialogDeleteHomeComponent {

  description: string;

  constructor(public dialogRef: MatDialogRef<DialogDeleteHomeComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  onNoClick() {
    this.dialogRef.close();
  }

  submit() {
    this.data.classe[this.data.operation](this.data.parameters);
    this.dialogRef.close();
  }

}
