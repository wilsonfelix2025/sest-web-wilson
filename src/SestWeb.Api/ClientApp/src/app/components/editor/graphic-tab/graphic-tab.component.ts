import { Component, Input, AfterViewInit } from '@angular/core';
import { CdkDragDrop } from '@angular/cdk/drag-drop';
import { SynchronizeChartsService } from '@services/synchronize-charts.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { GraphicArea, Tab } from 'app/repositories/models/state';
import { TabsDatasetService } from '@services/dataset/state/tabs-dataset.service';
import { StateService } from '@services/dataset/state/state.service';

@Component({
  selector: 'sest-graphic-tab',
  templateUrl: './graphic-tab.component.html',
  styleUrls: ['./graphic-tab.component.scss']
})
export class GraphicTabComponent implements AfterViewInit {

  chartsTrackFn = (i: number, chart: GraphicArea) => chart.id + chart.largura;

  /**
   * An object containing relevant information for components inside this one.
   */
  @Input() tabId: any;

  @Input() caseId: string;

  constructor(
    public sync: SynchronizeChartsService,
    private tabsDatasetService: TabsDatasetService,

    public stateService: StateService,
    public dataset: DatasetService,
  ) { }

  /**
   * Create a new, blank chart track.
   */
  addCanvasInstance() {
    this.tabsDatasetService.createChart(this.tabId, this.caseId);
  }

  removeTab() {
    this.tabsDatasetService.remove(this.tabId, this.caseId);
  }

  /**
   * Callback called when the user drops a dragged chart inside the tab.
   *
   * @param {CdkDragDrop} event an object containing information about the event.
   */
  drop(event: CdkDragDrop<string[]>) {
    if (event.previousIndex === event.currentIndex) {
      return;
    }
    // Change the position of the track inside the tab when the chart is dropped
    this.tabsDatasetService.changeChartOrder(this.tabId, this.caseId, event.previousIndex, event.currentIndex);
  }

  ngAfterViewInit() {
    // This is necessary for fix visualization bug in canvas component on firefox browser.
    const resizeEvent = new Event('resize');
    window.dispatchEvent(resizeEvent);
  }
}
