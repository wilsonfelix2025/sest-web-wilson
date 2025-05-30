import { Injectable } from '@angular/core';
import { DatasetService } from '@services/dataset/dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { TreeDatasetService } from './state/tree-dataset.service';

import { Calculo, isPerfuracao } from 'app/repositories/models/calculo';
import { Perfil } from 'app/repositories/models/perfil';
import { Node, NodeTypes } from 'app/repositories/models/state';
import { UNSET } from '@utils/vazio';

@Injectable({
    providedIn: 'root'
})
export class CalculationDatasetService {

    constructor(
        private dataset: DatasetService,
        private profileService: ProfileDatasetService,
        private treeService: TreeDatasetService,
    ) { }

    getAll(caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        return datasetCase.calculationsIds.map(el => this.dataset.getById(el));
    }


    add(calculation: Calculo, caseId: string) {
        this.addJustDataset(calculation, caseId);

        this.createCalculationFolder(calculation, caseId);
    }

    addJustDataset(calculation: Calculo, caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);

        this.dataset.add(calculation.id, calculation);
        datasetCase.calculationsIds.push(calculation.id);

        const profiles: Perfil[] = calculation.perfisSaída.perfis;
        if (profiles !== undefined) {
            profiles.forEach(el => {
                this.profileService.addJustDataset(el, caseId);
            });
        }
    }
    remove(calculationId: string, caseId: string) {
        this.dataset.getById(calculationId).perfisSaída.idPerfis.forEach(id => {
            this.profileService.remove(id, caseId);
        });

        this.removeCalc(calculationId, this.dataset.currCaseId);
    }

    update(updatedCalculation: Calculo, oldCalculationId: string, profilesChanged: Perfil[]) {
        // Alterar para editar os calculos atuais ao inves de remover e adicionar
        this.removeCalc(oldCalculationId, this.dataset.currCaseId);
        this.addCalc(updatedCalculation, this.dataset.currCaseId);

        if (!UNSET(profilesChanged)) {
            this.profileService.updateList(profilesChanged);
        }
    }

    addFilter(calculation: Calculo, caseId: string) {
        this.addJustDataset(calculation, caseId);

        calculation.perfisSaída.perfis.forEach(el => {
            const node = {
                data: [],
                fixa: false,
                id: el.id,
                name: el.nome,
                tipo: NodeTypes.Perfil
            };
            this.treeService.addNode(caseId, ['Filtros'], node);
        });
    }

    updateFilter(calculation, caseId: string) {
        this.addJustDataset(calculation, caseId);

        this.profileService.updateList(calculation.perfisSaída.perfis);
        this.treeService.update(caseId);
    }

    private addCalc(calculation: Calculo, caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);

        this.dataset.add(calculation.id, calculation);
        datasetCase.calculationsIds.push(calculation.id);

        this.profileService.updateList(calculation.perfisSaída.perfis);

        this.createCalculationFolder(calculation, caseId);

    }

    private removeCalc(calculationId: string, caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        datasetCase.calculationsIds.splice(datasetCase.calculationsIds.indexOf(calculationId), 1);

        this.treeService.removeFromTree(this.dataset.currCaseId, calculationId);
        this.dataset.remove(calculationId);
    }

    private createCalculationFolder(calculation: Calculo, caseId: string) {
        const newFolder: Node = {
            data: [],
            id: calculation.id,
            fixa: false,
            name: calculation.nome,
            tipo: NodeTypes.Folder
        };
        const profilesIds: string[] = calculation.perfisSaída.idPerfis;
        profilesIds.forEach(id => {
            (newFolder.data as Node[]).push({
                data: [],
                fixa: false,
                id: id,
                name: this.dataset.getById(id).name,
                tipo: NodeTypes.Perfil
            });
        });

        if (isPerfuracao(calculation.grupoCálculo)) {
            this.treeService.addNode(caseId, ['Perfuração', calculation.grupoCálculo], newFolder);
        } else {
            this.treeService.addNode(caseId, ['Cálculos', calculation.grupoCálculo], newFolder);
        }
    }
}
