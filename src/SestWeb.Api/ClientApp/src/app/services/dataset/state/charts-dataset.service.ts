import { Injectable } from '@angular/core';
import * as HighchartsUtils from '@utils/highcharts';
import { DatasetService } from '@services/dataset/dataset.service';

import { GraphicArea } from 'app/repositories/models/state';
import { StateService } from './state.service';

@Injectable({
    providedIn: 'root'
})
export class ChartsDatasetService {

    // $chartAdded = new Subject<string>();
    // $chartChanged = new Subject<string>();
    // $chartRemoved = new Subject<string>();

    constructor(
        private dataset: DatasetService,
        private stateService: StateService,
    ) { }

    addJustDataset(chart: GraphicArea) {
        chart.items = chart.items.filter(el => el.id !== undefined);

        this.dataset.add(chart.id, chart);
    }

    update(caseId: string, chart: GraphicArea) {
        this.dataset.update(chart.id, chart);

        this.stateService.saveState(caseId).then(res => { });
    }

    remove(caseId: string, chartId: string) {
        this.dataset.remove(chartId);

        this.stateService.saveState(caseId).then(res => { });
    }

    addCurve(caseId: string, chartId: string, curveId: string, adicionadoTrend?: boolean) {
        const chart: GraphicArea = this.dataset.getById(chartId);
        const curve = this.dataset.getById(curveId);

        if (chart && chart.items && chart.items.findIndex(el => el.id === curveId) < 0) {
            chart.items.push({
                id: curveId,
                idPoço: curve.idPoço,
                adicionadoTrend: adicionadoTrend ? true : false
            });
            this.update(caseId, chart);
        }
    }

    removeCurve(caseId: string, chartId: string, curveId: string) {
        const chart: GraphicArea = this.dataset.getById(chartId);

        if (chart.items.findIndex(el => el.id === curveId) >= 0) {
            chart.items = chart.items.filter(el => el.id !== curveId);
            this.update(caseId, chart);
        }
    }

    removeAllCurves(caseId: string, chartId: string) {
        const chart: GraphicArea = this.dataset.getById(chartId);

        if (chart.items.length >= 0) {
            chart.items = [];
            chart.maxX = 0;
            chart.minX = 0;
            chart.intervalo = 0;
            this.update(caseId, chart);
        }
    }

    addTrend(caseId: string, chartId: string, curveId: string) {
        const chart: GraphicArea = this.dataset.getById(chartId);
        const index = chart.items.findIndex(el => el.id === curveId);
        const curve = this.dataset.getById(curveId);

        if (this.dataset.hasById(curveId) && index >= 0) {
            chart.items[index] = {
                id: curveId,
                idPoço: curve.idPoço,
                adicionadoTrend: true
            };
            this.update(caseId, chart);
        }
    }

    removeTrend(caseId: string, chartId: string) {
        const chart: GraphicArea = this.dataset.getById(chartId);

        if (chart.items.length > 0) {
            chart.items.forEach(el => {
                el.adicionadoTrend = false;
            });
            this.update(caseId, chart);
        }
    }

    create(): GraphicArea {
        return { id: null, name: '', items: [], largura: 1, maxX: 0, minX: 0, intervalo: 0 };
    }

    changeCurveOrder(caseId: string, chartId: string, curveIndex: number, newIndex: number) {
        if (curveIndex !== newIndex) {
            const chart: GraphicArea = this.dataset.getById(chartId);
            const curve = chart.items[curveIndex];
            chart.items.splice(curveIndex, 1);
            chart.items.splice(newIndex, 0, curve);

            this.update(caseId, chart);
        }
    }

    putCurveOnTop(caseId: string, chartId: string, curveId: string) {
        const chart: GraphicArea = this.dataset.getById(chartId);
        const curveIndex = chart.items.findIndex(el => el.id === curveId);
        const curve = chart.items[curveIndex];
        chart.items.splice(curveIndex, 1);
        chart.items.unshift(curve);

        this.update(caseId, chart);
    }

    updateProperties(caseId: string, chart: GraphicArea) {
        const oldChart: GraphicArea = this.dataset.getById(chart.id);
        if (oldChart.intervalo === chart.intervalo && oldChart.largura === chart.largura &&
            oldChart.maxX === chart.maxX && oldChart.minX === chart.minX && oldChart.name === chart.name) {
            return;
        }
        oldChart.intervalo = chart.intervalo;
        oldChart.largura = chart.largura;
        oldChart.maxX = chart.maxX;
        oldChart.minX = chart.minX;
        oldChart.name = chart.name;


        this.stateService.saveState(caseId).then(res => { });
        setTimeout(() => {
            HighchartsUtils.reflowCharts();
        }, 10);
    }
}
