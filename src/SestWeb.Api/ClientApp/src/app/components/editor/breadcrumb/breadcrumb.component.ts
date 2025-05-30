import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';
import { DatasetService } from '@services/dataset/dataset.service';

@Component({
  selector: 'sest-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.scss']
})
export class BreadcrumbComponent implements OnInit {

  opUnit = { name: '', children: [], id: '' };
  oilField = { name: '', children: [], id: '' };
  well = { name: '', children: [], id: '' };
  file = { name: '', children: [], id: '' };

  constructor(
    private router: Router,
    private dataset: DatasetService,
    private caseDataset: CaseDatasetService,
  ) {
    this.getCurrPath();
    // Whenever the API responds
    this.caseDataset.$currCaseLoaded.subscribe(res => {
      this.getCurrPath();
    });
  }

  getCurrPath() {
    const path = this.caseDataset.getCasePath(this.dataset.currCaseId);
    if (path !== undefined) {
      this.opUnit = path;
      this.oilField = this.opUnit.children[0];
      this.well = this.oilField.children[0];
      this.file = this.well.children[0];
    }
  }

  goToExplorer(locate) {
    let caminho = '';
    if (locate.opUnit !== undefined) {
      caminho += `/unidade/${locate.opUnit}`;
      if (locate.oilField !== undefined) {
        caminho += `/campo/${locate.oilField}`;
        if (locate.well !== undefined) {
          caminho += `/poco/${locate.well}`;
        }
      }
    }
    this.router.navigateByUrl('explorer' + caminho);
  }

  ngOnInit() {
  }

}
