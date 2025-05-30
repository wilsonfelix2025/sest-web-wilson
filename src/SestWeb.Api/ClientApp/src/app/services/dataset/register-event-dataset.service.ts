import { Injectable } from '@angular/core';
import { DatasetService } from '@services/dataset/dataset.service';
import { TreeDatasetService } from './state/tree-dataset.service';

import { Node, NodeTypes } from 'app/repositories/models/state';
import { Evento, Registro } from 'app/repositories/models/registro-evento';
import { Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class RegisterEventDatasetService {

    $registerEventUpdated = new Subject();

    constructor(
        private dataset: DatasetService,
        private treeService: TreeDatasetService,
    ) { }

    getAll(caseId: string): (Registro | Evento)[] {
        const datasetCase = this.dataset.getDatasetCase(caseId);
        return datasetCase.registerEventsIds.map(el => this.dataset.getById(el));
    }

    addJustDataset(registerEvent: Registro | Evento, caseId: string) {
        const datasetCase = this.dataset.getDatasetCase(caseId);

        this.dataset.add(registerEvent.id, registerEvent);

        if (!datasetCase.registerEventsIds.includes(registerEvent.id)) {
            datasetCase.registerEventsIds.push(registerEvent.id);
        }
    }

    updateList(registersEvents: (Registro | Evento)[]) {
        registersEvents.forEach(registerEvent => {
            this.dataset.update(registerEvent.id, registerEvent);

            if (registerEvent.trechos.length > 0) {
                if (!this.treeService.has(this.dataset.currCaseId, registerEvent.id)) {
                    this.addToTree(registerEvent);
                }
            } else {
                if (this.treeService.has(this.dataset.currCaseId, registerEvent.id)) {
                    this.treeService.removeFromTree(this.dataset.currCaseId, registerEvent.id);
                }
            }
        });

        this.treeService.update(this.dataset.currCaseId);
        this.$registerEventUpdated.next();
    }

    update(registerEvent: (Registro | Evento)) {
        this.dataset.update(registerEvent.id, registerEvent);

        if (registerEvent.trechos.length > 0) {
            if (!this.treeService.has(this.dataset.currCaseId, registerEvent.id)) {
                this.addToTree(registerEvent);
            }
        } else {
            if (this.treeService.has(this.dataset.currCaseId, registerEvent.id)) {
                this.treeService.removeFromTree(this.dataset.currCaseId, registerEvent.id);
            }
        }

        this.treeService.update(this.dataset.currCaseId);
        this.$registerEventUpdated.next();
    }

    addToTree(registerEvent: Registro | Evento) {
        const newFolder: Node = {
            data: [],
            fixa: false,
            id: registerEvent.id,
            name: registerEvent.nome,
            tipo: NodeTypes.RegistroEvento
        };
        if (registerEvent.tipo === 'Evento') {
            this.treeService.addNode(this.dataset.currCaseId, ['Perfuração', 'Eventos de Perfuração'], newFolder);
        } else {
            this.treeService.addNode(this.dataset.currCaseId, ['Perfuração', 'Registros'], newFolder);
        }
    }
}
