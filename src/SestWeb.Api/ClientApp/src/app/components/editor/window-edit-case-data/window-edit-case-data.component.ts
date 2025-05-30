import { Component, Inject, OnInit, ViewChildren } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { UpdateDadosGerais } from '@utils/interfaces';
import { Case } from 'app/repositories/models/case';
import { DatasetService } from '@services/dataset/dataset.service';
import { CaseService } from 'app/repositories/case.service';

@Component({
  selector: 'sest-window-edit-case-data',
  templateUrl: './window-edit-case-data.component.html',
  styleUrls: ['./window-edit-case-data.component.scss']
})
export class WindowEditCaseDataComponent implements OnInit {

  /**
   * O caso de estudo aberto atualmente
   */
  currCase: Case;

  currTab: number = 0;
  @ViewChildren('tab') tabs;

  loading = false;

  constructor(
    public dialogRef: MatDialogRef<WindowEditCaseDataComponent>,
    public caseService: CaseService,

    public dataset: DatasetService,
    @Inject(MAT_DIALOG_DATA) public dialogData: { data: { caseId: string } }
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dialogData.data.caseId);
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  onChangeTab(index: number) {
    this.currTab = index;
    this.tabs._results.forEach(tab => {
      if (tab && tab.refreshTable) {
        tab.refreshTable();
      }
    });
  }

  submit() {
    this.loading = true;
    const data: any = <UpdateDadosGerais>{};

    this.tabs._results.forEach(tab => {
      if (tab && tab.getData) {
        Object.assign(data, tab.getData());
      }
    });
    if (Object.keys(data).length > 0) {
      this.caseService.updateGeneralData(this.currCase.id, data).then(response => {
        this.loading = false;
        location.reload();
      }).catch(() => { this.loading = false; });
    } else {
      this.loading = false;
    }
  }

}
