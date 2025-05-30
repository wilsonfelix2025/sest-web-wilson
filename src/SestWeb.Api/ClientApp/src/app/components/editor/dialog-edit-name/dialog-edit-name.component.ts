import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-dialog-edit-name',
  templateUrl: './dialog-edit-name.component.html',
  styleUrls: ['./dialog-edit-name.component.scss']
})
export class DialogEditNameComponent implements OnInit {

  name: string;

  constructor(public dialogRef: MatDialogRef<DialogEditNameComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit() {
    this.name = this.data.parameters.name;
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  submit() {
    const parameters = { name: this.name }
    this.data.classe[this.data.operation](parameters);
    this.dialogRef.close();
  }

}
