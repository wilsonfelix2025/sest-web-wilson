import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-dialog-confirm',
  templateUrl: './dialog-confirm.component.html',
  styleUrls: ['./dialog-confirm.component.scss']
})
export class DialogConfirmComponent implements OnInit {

  title: string;
  description: string;

  constructor(public dialogRef: MatDialogRef<DialogConfirmComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { title: string, description: string }) { }

  ngOnInit() {
    this.title = this.data.title;
    this.description = this.data.description;
  }

  onNoClick() {
    this.dialogRef.close();
  }

  submit() {
    this.dialogRef.close(true);
  }

}
