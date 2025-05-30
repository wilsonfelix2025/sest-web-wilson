import { Component, OnInit } from '@angular/core';
import * as Highcharts from 'highcharts';
import * as HighchartsUtils from '@utils/highcharts';
import { MatTabChangeEvent } from '@angular/material';
import { SynchronizeChartsService } from '@services/synchronize-charts.service';
import { DialogService } from '@services/dialog.service';
import { Case } from 'app/repositories/models/case';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';
import { ActivatedRoute } from '@angular/router';
import { DatasetService } from '@services/dataset/dataset.service';
import { StateService } from '@services/dataset/state/state.service';
import { CdkDragEnd, CdkDragMove } from '@angular/cdk/drag-drop';

@Component({
  selector: 'sest-home-content',
  templateUrl: './home-content.component.html',
  styleUrls: ['./home-content.component.scss']
})

export class HomeContentComponent implements OnInit {
  /**
   * Expose the Highcharts library to the component's template.
   */
  Highcharts: typeof Highcharts = Highcharts;

  /**
   * The list of files in the current session.
   */
  openFiles: Case[];

  /**
   * The currently selected tab. It's initialized at 0 by default.
   */
  selected: string;

  trajetWidth: number = 1.5;

  deepView = false;

  size = 0;
  savedSize = 0;
  initialSize = 0;

  constructor(
    private route: ActivatedRoute,
    public sync: SynchronizeChartsService,
    public dialog: DialogService,

    private state: StateService,
    public dataset: DatasetService,

    private caseDataset: CaseDatasetService,
  ) {
    this.getInfo();

    this.caseDataset.$currCaseLoaded.subscribe((res) => {
      this.getInfo();
      this.selected = this.openFiles[0].id;

    });
    this.caseDataset.$caseAdded.subscribe((res) => {
      this.getInfo();
    });
    this.caseDataset.$caseRemoved.subscribe((res) => {
      this.getInfo();
    });
  }

  ngOnInit() {
    this.caseDataset.loadCurrCase(this.route.snapshot.paramMap.get('id'));

    HighchartsUtils.reflowCharts();
  }

  getInfo() {
    this.openFiles = this.caseDataset.getAll();

    this.deepView = this.state.currentDeepView === 'PV';
    this.savedSize = this.initialSize = this.state.currentGraphicAreaSize;

    HighchartsUtils.reflowCharts();
  }

  /**
   * Callback called whenever the user selects a different file to work on.
   *
   * @param {MatTabChangeEvent} event object containing event information.
   */
  onTabChange(event: MatTabChangeEvent) {
    this.selected = this.openFiles[event.index].id;
    // Reflow all charts to prevent range weirdness
    HighchartsUtils.reflowCharts();
    // Update the currently selected file index
    this.state.setCurrState(this.selected);
  }

  mudarVisaoProfundidade(mostrarEmPv: boolean, caseId: string) {
    // console.log('mudarVisaoProfundidade', mostrarEmPv);
    this.state.setDeepView(mostrarEmPv ? 'PV' : 'PM', caseId);
  }

  onResizeMove(event: CdkDragMove) {
    this.size = event.distance.x;
  }

  ended(caseId: string) {
    this.savedSize += this.size;
    this.size = 0;

    this.state.setCurrentGraphicAreaSize(this.savedSize, caseId);
  }

}
