import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject } from 'rxjs';
import { DatasetService } from '@services/dataset/dataset.service';

import { GraphicArea, State, Tab } from 'app/repositories/models/state';
import { ChartsDatasetService } from './charts-dataset.service';
import { StateService } from './state.service';

@Injectable({
    providedIn: 'root'
})
export class TabsDatasetService {

    constructor(
        private dataset: DatasetService,
        private chartsDatasetService: ChartsDatasetService,
        private stateService: StateService,
    ) { }

    create(name: string, caseId: string) {
        const tab: Tab = {
            name: name,
            fixa: false,
            data: []
        };

        this.stateService.saveState(caseId, { tab: tab }).then(res => {
            if (res && res.state) {
                res.state.tabs.forEach(tab => {
                    if (!this.dataset.hasById(tab.id)) {
                        this.add(tab, caseId);
                    }
                });
            }
        });
    }

    add(tab: Tab, caseId: string) {
        this.addJustDataset(tab, caseId);

        this.stateService.addTab(tab);
    }

    addJustDataset(tab: Tab, caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);

        tab.data.forEach(chart => {
            this.chartsDatasetService.addJustDataset(chart);
        });

        this.dataset.add(tab.id, tab);

        if (!datasetCase.tabsIds.includes(tab.id)) {
            datasetCase.tabsIds.push(tab.id);
        }
    }

    update(tab: Tab, caseId: string) {
        this.dataset.update(tab.id, tab);

        this.stateService.updateCurrTab(tab.id);
        // this.$tabChanged.next(tab.id);
    }

    remove(tabId: string, caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        datasetCase.tabsIds.splice(datasetCase.tabsIds.indexOf(tabId), 1);

        this.dataset.remove(tabId);

        this.stateService.removeTab(tabId);

        this.stateService.saveState(caseId).then(res => { });
    }

    createChart(tabId: string, caseId: string) {
        const chart: GraphicArea = this.chartsDatasetService.create();

        this.stateService.saveState(caseId, { chart: { ga: chart, tabId: tabId } }).then(res => {
            if (res && res.state) {
                res.state.tabs.forEach(tab => {
                    if (tab.id === tabId) {
                        tab.data.forEach(chart => {
                            if (!this.dataset.hasById(chart.id)) {
                                this.chartsDatasetService.addJustDataset(chart);
                            }
                        });
                        this.update(tab, caseId);
                        return;
                    }
                });
            }
        });
    }

    changeChartOrder(tabId: string, caseId: string, chartIndex: number, newIndex: number) {
        if (chartIndex !== newIndex) {
            const tab: Tab = this.dataset.getById(tabId);
            const chart = tab.data[chartIndex];
            tab.data.splice(chartIndex, 1);
            tab.data.splice(newIndex, 0, chart);

            this.update(tab, caseId);

            this.stateService.saveState(caseId, { tab: tab }).then(res => { });
        }
    }

    removeChart(chartId: string, tabId: string, caseId: string) {
        const tab: Tab = this.dataset.getById(tabId);
        tab.data.splice(tab.data.findIndex(el => el.id === chartId), 1);
        this.update(tab, caseId);

        this.chartsDatasetService.remove(caseId, chartId);

        this.stateService.saveState(caseId).then(res => { });
    }
}
