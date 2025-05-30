import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { DatasetService } from '@services/dataset/dataset.service';
import { TreeDatasetService } from './state/tree-dataset.service';

import { Perfil } from 'app/repositories/models/perfil';
import { ProfileUtils } from '@utils/dataset/profile';
import { Node, NodeTypes } from 'app/repositories/models/state';

@Injectable({
    providedIn: 'root'
})
export class ProfileDatasetService {

    $profileAdded = new Subject<Perfil>();
    $profileChanged = new Subject<Perfil>();
    $profileRemoved = new Subject<string>();

    $trendChanged = new Subject<string>();
    $trendRemoved = new Subject<string>();

    constructor(
        private dataset: DatasetService,
        private treeService: TreeDatasetService,
    ) { }

    getAll(caseId: string): Perfil[] {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        return datasetCase.profilesIds.map(el => this.dataset.getById(el));
    }

    add(profile: Perfil, caseId: string) {
        this.addJustDataset(profile, caseId);

        const newFolder: Node = {
            data: [],
            fixa: false,
            id: profile.id,
            name: profile.nome,
            tipo: NodeTypes.Perfil
        };
        this.treeService.addNode(this.dataset.currCaseId, [profile['grupoPerfis'].nome], newFolder);

        this.$profileAdded.next(profile);
    }

    addJustDataset(profile: Perfil, caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);

        ProfileUtils.profilePointsToData(profile);
        this.dataset.add(profile.id, profile);

        if (!datasetCase.profilesIds.includes(profile.id)) {
            datasetCase.profilesIds.push(profile.id);
        }
    }

    updateList(profiles: Perfil[]) {
        if (profiles.length > 0) {
            profiles.forEach(profile => {
                if (!this.dataset.hasById(profile.id)) {
                    return;
                }
                ProfileUtils.profilePointsToData(profile);
                this.dataset.update(profile.id, profile);
                this.$profileChanged.next(profile);
            });

            this.treeService.update(profiles[0].idPoço);
        }
    }

    update(profile: Perfil) {
        ProfileUtils.profilePointsToData(profile);
        this.dataset.update(profile.id, profile);
        this.$profileChanged.next(profile);

        this.treeService.update(profile.idPoço);
    }

    remove(id: string, caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        datasetCase.profilesIds.splice(datasetCase.profilesIds.indexOf(id), 1);

        this.treeService.removeFromTree(caseId, id);

        this.dataset.remove(id);
        this.$profileRemoved.next(id);
    }

    editTrend(id: string) {
        this.$trendChanged.next(id);
    }

    removeTrend(id: string) {
        const profile: Perfil = this.dataset.getById(id);
        profile.trend = undefined;

        this.$trendRemoved.next(profile.id);
    }
}
