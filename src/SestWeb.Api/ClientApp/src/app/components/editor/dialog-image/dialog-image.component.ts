import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-dialog-image',
  templateUrl: './dialog-image.component.html',
  styleUrls: ['./dialog-image.component.scss']
})
export class DialogImageComponent implements OnInit {

  path: string;
  title: string;

  constructor(public dialogRef: MatDialogRef<DialogImageComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit() {
    this.path = this.data.parameters.imagePath;
    this.title = this.data.parameters.title;
  }

  close(): void {
    this.dialogRef.close();
  }
}
