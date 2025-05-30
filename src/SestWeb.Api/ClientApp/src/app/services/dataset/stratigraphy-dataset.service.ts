import { Injectable } from '@angular/core';
import { DatasetService } from '@services/dataset/dataset.service';

import { Estratigrafia, StratigraphyChart } from 'app/repositories/models/estratigrafia';
import { StratigraphyUtils } from '@utils/dataset/stratigraphy';

@Injectable({
    providedIn: 'root'
})
export class StratigraphyDatasetService {

    constructor(private dataset: DatasetService) { }

    add(stratigraphies: Estratigrafia[], key: string, caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        const strat: StratigraphyChart = StratigraphyUtils.loadStratigraphy(stratigraphies, key, this.dataset.getById(caseId));
        const stratId: string = StratigraphyDatasetService.getStratigraphyId(key, caseId)

        this.dataset.add(stratId, strat);
        datasetCase.stratigraphiesIds.push(stratId);
    }

    getAllIds(caseId: string): string[] {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        return datasetCase.stratigraphiesIds;
    }

    public static getStratigraphyId(key: string, caseId: string) {
        return key + caseId;
    }
}
