import { Component, Input, OnInit } from '@angular/core';
import { SynchronizeChartsService } from '@services/synchronize-charts.service';

import { DialogService } from '@services/dialog.service';
import { DialogDeleteHomeComponent } from '../dialog-delete-home/dialog-delete-home.component';
import { DatasetService } from '@services/dataset/dataset.service';
import { TabsDatasetService } from '@services/dataset/state/tabs-dataset.service';
import { Tab } from 'app/repositories/models/state';
import { StateService } from '@services/dataset/state/state.service';

@Component({
  selector: 'sest-graphic-area',
  templateUrl: './graphic-area.component.html',
  styleUrls: ['./graphic-area.component.scss']
})

export class GraphicAreaComponent implements OnInit {

  tabsTrackFn = (i: number, tab: Tab) => tab && tab.id;

  @Input() caseId: string;

  constructor(
    public sync: SynchronizeChartsService,
    public dialog: DialogService,

    public dataset: DatasetService,
    public stateService: StateService,
    private tabsDatasetService: TabsDatasetService,
  ) { }

  ngOnInit() {
    setTimeout(() => {
      this.sync.syncZoom();
    }, 10);
  }

  getCurrIndex(tabId: string) {
    const tabsIds = this.stateService.getTabsIds(this.caseId);
    return tabsIds.indexOf(tabId);
  }

  /**
   * Callback which will be executed whenever the user change tabs in the graphic area.
   *
   * @param {MatTabChangeEvent} e the object containing event information.
   */
  onTabChange(index: number) {
    // Update the index of the current tab
    this.stateService.setCurrTabId(this.caseId, index);
    // Await change and sync all charts on screen
    setTimeout(() => {
      this.sync.syncZoom();
    }, 0);
  }

  delete(tab: Tab) {
    this.dialog.openDialogGeneric(DialogDeleteHomeComponent, this, 'remove', { tabId: tab.id });
  }

  remove(parameters: { tabId: string }) {
    this.tabsDatasetService.remove(parameters.tabId, this.caseId);
  }
}
