import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { DatasetService } from '@services/dataset/dataset.service';
import { TreeDatasetService } from './state/tree-dataset.service';

import { Litologia } from 'app/repositories/models/litologia';
import { LithologyUtils } from '@utils/dataset/lithology';
import { Node, NodeTypes } from 'app/repositories/models/state';

@Injectable({
    providedIn: 'root'
})
export class LithologyDatasetService {

    $lithologyChanged = new Subject<Litologia>();

    constructor(
        private dataset: DatasetService,
        private treeService: TreeDatasetService,
    ) { }


    add(lithology: Litologia, caseId: string) {
        const oldLito = this.getAll(caseId).find(el => el.classificação.nome === lithology.classificação.nome);
        this.removeFromDataset(oldLito, caseId);

        this.addJustDataset(lithology, caseId);

        const newFolder: Node = {
            data: [],
            fixa: false,
            id: lithology.id,
            name: lithology.classificação.nome,
            tipo: NodeTypes.Litologia
        };
        this.treeService.addNode(this.dataset.currCaseId, ['Litologia'], newFolder);

        this.$lithologyChanged.next(lithology);
    }

    addJustDataset(lithology: Litologia, caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);

        Object.assign(lithology, LithologyUtils.lithologyPointsToData(lithology, this.dataset.getById(caseId).dadosGerais));
        this.dataset.add(lithology.id, lithology);

        if (!datasetCase.lithologiesIds.includes(lithology.id)) {
            datasetCase.lithologiesIds.push(lithology.id);
        }
    }

    getAll(caseId: string): Litologia[] {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        return datasetCase.lithologiesIds.map(el => this.dataset.getById(el));
    }

    update(lithology: Litologia, caseId: string) {
        Object.assign(lithology, LithologyUtils.lithologyPointsToData(lithology, this.dataset.getById(caseId).dadosGerais));
        this.dataset.update(lithology.id, lithology);
        this.$lithologyChanged.next(lithology);
    }

    remove(lithologyId: string, caseId: string) {
        const litho = this.dataset.getById(lithologyId);
        litho.pontos = [];

        this.update(litho, caseId);
    }

    private removeFromDataset(lithology: Litologia, caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        datasetCase.lithologiesIds.filter(el => el !== lithology.id);

        this.treeService.removeFromTree(caseId, lithology.id);
        this.dataset.remove(lithology.id);
    }
}
