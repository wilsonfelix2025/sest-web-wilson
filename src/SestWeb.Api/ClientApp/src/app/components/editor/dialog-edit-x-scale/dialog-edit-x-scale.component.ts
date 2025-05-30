import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-dialog-edit-x-scale',
  templateUrl: './dialog-edit-x-scale.component.html',
  styleUrls: ['./dialog-edit-x-scale.component.scss']
})
export class DialogEditXScaleComponent implements OnInit {

  min: number;
  max: number;
  interval: number;

  constructor(public dialogRef: MatDialogRef<DialogEditXScaleComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit() {
    this.min = this.data.parameters.minX;
    this.max = this.data.parameters.maxX;
    this.interval = this.data.parameters.intervalo;
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  submit() {
    const parameters = { minX: this.min, maxX: this.max, intervalo: this.interval }
    this.data.classe[this.data.operation](parameters);
    this.dialogRef.close();
  }

}
