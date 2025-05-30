import { Injectable } from '@angular/core';

import { DatasetCase } from './interfaces';

@Injectable({
    providedIn: 'root'
})
export class DatasetService {

    /**
     * The dictionary containing the registered datasets.
     */
    private _datasets: any = {};

    /**
     * The dictionary containing the dataset ids for each case 
     */
    private _datasetCases: { [id: string]: DatasetCase } = {};

    private _currCaseId: string;
    get currCaseId() { return this._currCaseId; }
    set currCaseId(index: string) { this._currCaseId = index; }

    constructor() { }

    private isNotEmptyDatasetId(datasetId: string): boolean {
        if (datasetId === null || datasetId === undefined) {
            console.error(`Dataset ID cannot be ${datasetId}.`);
            return false;
        }
        return true;
    }

    private _has(datasetId: string): boolean {
        if (this.isNotEmptyDatasetId(datasetId)) {
            if (!this._datasets[datasetId]) {
                console.error(`No dataset with ID '${datasetId}' was found.`);
                return false;
            }
            return true;

        }
        return false;
    }

    hasById(datasetId: string) {
        return this._datasets[datasetId] !== undefined;
    }

    getById(datasetId: string) {
        if (this._has(datasetId)) {
            return this._datasets[datasetId];
        }
        return undefined;
    }

    add(datasetId: string, payload) {
        if (this.isNotEmptyDatasetId(datasetId)) {
            this._datasets[datasetId] = payload;
        }
    }

    update(datasetId: string, payload) {
        if (this._has(datasetId)) {
            this._datasets[datasetId] = payload;
        }
    }

    remove(datasetId: string) {
        if (this._datasets[datasetId]) {
            delete this._datasets[datasetId];
        }
    }

    getDatasetCase(id: string): DatasetCase | undefined {
        if (this._datasetCases[id]) {
            return this._datasetCases[id];
        }
        return undefined;
    }

    addDatasetCase(datasetCase: DatasetCase) {
        this._datasetCases[datasetCase.caseId] = datasetCase;
    }

    removeDatasetCase(id: string) {
        if (this._datasetCases[id]) {
            delete this._datasetCases[id];
        }
    }

    getCasesIds() {
        return Object.keys(this._datasetCases);
    }
}
