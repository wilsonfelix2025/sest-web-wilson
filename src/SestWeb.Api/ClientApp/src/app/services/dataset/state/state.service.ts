import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, Subject, EMPTY } from 'rxjs';
import { DatasetService } from '@services/dataset/dataset.service';

import { GraphicArea, State, Tab } from 'app/repositories/models/state';
import { StateApiService } from 'app/repositories/state-api.service';
@Injectable({
    providedIn: 'root'
})
export class StateService {

    private _currentDeepView: 'PV' | 'PM' = 'PV';
    deepViewChanged = new Subject<'PV' | 'PM'>();
    public get currentDeepView(): 'PV' | 'PM' { return this._currentDeepView; }
    public set currentDeepView(val: 'PV' | 'PM') { this._currentDeepView = val; this.deepViewChanged.next(val); }

    private _currentGraphicAreaSize: number = 0;
    graphicAreaSizeChanged = new Subject<number>();
    public get currentGraphicAreaSize(): number { return this._currentGraphicAreaSize; }
    public set currentGraphicAreaSize(val: number) { this._currentGraphicAreaSize = val; this.graphicAreaSizeChanged.next(val); }

    private readonly _currentTab = new BehaviorSubject<Tab>(undefined);
    public get currentTab$(): Observable<Tab> { return this._currentTab.asObservable(); }
    private get currentTab(): Tab { return this._currentTab.getValue(); }
    private set currentTab(val: Tab) { this._currentTab.next(val); }

    private readonly _currentTabs = new BehaviorSubject<Tab[]>([]);
    public get currentTabs$(): Observable<Tab[]> { return this._currentTabs.asObservable(); }
    private get currentTabs(): Tab[] { return this._currentTabs.getValue(); }
    private set currentTabs(val: Tab[]) { this._currentTabs.next(val); }

    private readonly _currentCharts = new BehaviorSubject<GraphicArea[]>([]);
    public get currentCharts$(): Observable<GraphicArea[]> { return this._currentCharts.asObservable(); }
    private get currentCharts(): GraphicArea[] { return this._currentCharts.getValue(); }
    private set currentCharts(val: GraphicArea[]) { this._currentCharts.next(val); }

    constructor(
        private dataset: DatasetService,
        private stateApiService: StateApiService,
    ) { }

    setDeepView(deepView: 'PV' | 'PM', caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        datasetCase.state.profundidadeExibição = deepView;

        this.currentDeepView = deepView;
        this.saveState(caseId).then(res => { });
    }

    setCurrentGraphicAreaSize(size: number, caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        datasetCase.state.posicaoDivisaoAreaGrafica = size;

        this.currentGraphicAreaSize = size;
        this.saveState(caseId).then(res => { });
    }

    setCurrState(caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        this.currentDeepView = datasetCase.state.profundidadeExibição;
        this.currentGraphicAreaSize = datasetCase.state.posicaoDivisaoAreaGrafica;

        this.currentTabs = this.getTabsIds(caseId).map(id => this.dataset.getById(id));

        this.updateCurrTab(datasetCase.state.idAbaAtual);
    }

    updateCurrTab(tabId: string) {
        this.currentTab = this.dataset.getById(tabId);
        this.currentCharts = this.currentTab.data.map(el => this.dataset.getById(el.id));
    }

    addTab(tab: Tab) {
        const currentValue = this.currentTabs;
        this.currentTabs = [...currentValue, tab];
    }

    removeTab(tabId: string) {
        this.currentTabs = this.currentTabs.filter(el => el.id !== tabId);
    }

    setCurrTabId(caseId: string, tabIndex: number) {
        const tabId = this.currentTabs[tabIndex].id;
        if (this.dataset.hasById(tabId)) {
            this.currentTab = this.dataset.getById(tabId);
            this.currentCharts = this.currentTab.data.map(el => this.dataset.getById(el.id));

            this.saveState(caseId).then(res => { });
        }

        return this.currentTab;
    }

    getCurrTab(): Tab {
        return this.currentTab;
    }

    getTabsIds(caseId: string): string[] {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        return datasetCase.tabsIds;
    }

    /**
     * Save state changes
     */
    saveState(caseId: string, newItems?: { tab?: Tab, chart?: { ga: GraphicArea, tabId: string } }) {
        if (this.dataset.currCaseId !== caseId) {
            return EMPTY.pipe().toPromise();
        }
        const datasetCase = this.dataset.getDatasetCase(caseId);

        const state: State = {
            tabs: [],
            profundidadeExibição: datasetCase.state.profundidadeExibição,
            posicaoDivisaoAreaGrafica: datasetCase.state.posicaoDivisaoAreaGrafica,
            idAbaAtual: this.currentTab.id,
        };

        const tabsIds = this.getTabsIds(caseId);

        tabsIds.forEach(tabId => {
            const tab: Tab = this.dataset.getById(tabId);
            const newTab: Tab = { id: tab.id, fixa: tab.fixa, name: tab.name, data: [] };

            if (tab.data.length > 0) {
                tab.data.forEach(data => {
                    const chart: GraphicArea = this.dataset.getById(data.id);

                    if (chart) {
                        const newChart: GraphicArea = {
                            id: chart.id, name: chart.name, intervalo: chart.intervalo,
                            largura: chart.largura, maxX: chart.maxX, minX: chart.minX,
                            items: [],
                        };
                        if (chart.items) {
                            chart.items.forEach(item => {
                                newChart.items.push({ id: item.id, idPoço: item.idPoço, adicionadoTrend: item.adicionadoTrend });
                            });
                        }
                        newTab.data.push(newChart);
                    }

                });
            }
            if (newItems && newItems.chart && tabId === newItems.chart.tabId) {
                newTab.data.push(newItems.chart.ga);
            }
            state.tabs.push(newTab);
        });
        if (newItems && newItems.tab) {
            state.tabs.push(newItems.tab);
        }
        console.log('saving', state);

        return this.stateApiService.update(caseId, state);
    }
}
