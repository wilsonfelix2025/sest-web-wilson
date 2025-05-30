import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';

@Component({
  selector: 'app-rto-connection',
  templateUrl: './rto-connection.component.html',
  styleUrls: ['./rto-connection.component.scss']
})
export class RtoConnectionComponent implements OnInit {

   /**
   * URL
   *
   */
  url: { value: string, tooltip: string } = { value: '', tooltip: '' };

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
  ) { }

  ngOnInit() {
  }


  closeModal(): void {
    this.dialogRef.close();
  }

  submit() {}

}
