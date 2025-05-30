import { FormGroup, FormBuilder } from "@angular/forms";
import { MatDialogRef } from "@angular/material";

export class DialogBaseComponent {
  form: FormGroup;
  description: string;

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    private formBuilder: FormBuilder
  ) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  ngOnInit() {
    this.form = this.formBuilder.group({
      description: [this.description, []],
    })
  }

  submit(form) {
    this.dialogRef.close();
  }
}