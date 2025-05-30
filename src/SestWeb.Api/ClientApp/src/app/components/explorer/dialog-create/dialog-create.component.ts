import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FileService } from '../../../repositories/file.service';
import { FileData } from '../../../repositories/models/file';
import { ExplorerDataService } from '@services/explorer-data.service';
import { WellData } from '../../../repositories/models/well';
import { WellService } from '../../../repositories/well.service';

@Component({
  selector: 'app-dialog-create',
  templateUrl: './dialog-create.component.html',
  styleUrls: ['./dialog-create.component.scss']
})
export class DialogCreateComponent implements OnInit {

  name = '';
  initalOpUnit;
  initalOilField;
  initialWell;

  loading = false;

  constructor(public dialogRef: MatDialogRef<DialogCreateComponent>,
    private files: FileService,
    private wells: WellService,
    private explorer: ExplorerDataService,
    @Inject(MAT_DIALOG_DATA) public data: any) { }

  onNoClick(): void {
    this.dialogRef.close();
  }

  ngOnInit() {
    const aux = this.explorer.getCurrPath(this.data.parent, this.data.itemType - 1);

    this.initialWell = aux.currlWell;
    this.initalOilField = aux.currOilField;
    this.initalOpUnit = aux.currOpUnit;
  }

  submit() {
    this.loading = true;
    let info: WellData | FileData;
    let service;

    if (this.data.itemType === 3) {
      info = {
        name: this.name,
        description: 'Novo caso criado',
        fileType: `sesttr.${this.data['fileTypeInfo'].name}`,
        content: {},
        wellId: this.data['parent']
      };
      service = this.files;
    } else if (this.data.itemType === 2) {
      info = {
        name: this.name,
        oilFieldId: this.data['parent']
      };
      service = this.wells;
    }
    service.create(info).then(resp => {
      const obj = {
        name: resp.name,
        id: resp['id'],
        url: `//${resp['id']}/`,
        type: resp['fileType'],
        children: this.data.itemType < 3 ? [] : undefined
      };
      if (this.data.itemType === 3) {
        this.initialWell.children.push(obj);
        this.initialWell.filesCount++;
        this.initalOilField.filesCount++;
        this.initalOpUnit.filesCount++;
      } else {
        this.initalOilField.children.push(obj);
      }

      this.explorer.treeUpdated.next();
      this.loading = false;
      this.dialogRef.close();
    }).catch(() => { this.loading = false; });
  }
}
