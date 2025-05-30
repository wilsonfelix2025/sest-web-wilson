import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-new-dialog-delete',
  templateUrl: './new-dialog-delete.component.html',
  styleUrls: ['./new-dialog-delete.component.scss']
})
export class NewDialogDeleteComponent implements OnInit {

  loading = false;

  constructor(public dialogRef: MatDialogRef<NewDialogDeleteComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  ngOnInit() {

  }

  closeModal() {
    this.dialogRef.close();
  }

  submit() {
    this.loading = true;
    console.log(this.data);
    this.data.submitCallback().then(resp => {
      this.data.postSubmitCallback(resp);
      this.loading = false;
      this.closeModal();
    }).catch(() => { this.loading = false; });
  }
}
