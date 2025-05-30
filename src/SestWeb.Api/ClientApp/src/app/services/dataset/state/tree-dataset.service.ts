import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { DatasetService } from '@services/dataset/dataset.service';
import { TrajectoryDatasetService } from '@services/dataset/trajectory-dataset.service';

import { Node, NodeTypes } from 'app/repositories/models/state';

@Injectable({
    providedIn: 'root'
})
export class TreeDatasetService {

    private _trees: { [id: string]: Node[] } = {};

    $treeChanged: Subject<string> = new Subject();

    constructor(
        private dataset: DatasetService,
    ) { }

    add(tree: Node[], caseId: string) {
        this._trees[caseId] = tree;

        this.removeCalculationFilterFolder(caseId);
    }

    get(caseId: string) {
        return this._trees[caseId];
    }

    update(caseId: string) {
        this.updateTreeDataset(this._trees[caseId], caseId);
        this.$treeChanged.next(caseId);
    }

    has(caseId: string, id: string): boolean {
        const tree = this._trees[caseId];
        for (let i = 0; i < tree.length; i++) {
            const e = this.searchById(tree[i], id);
            if (e) {
                return e;
            }
        }
        return false;
    }

    private searchById(node: Node, id: string): boolean {
        if (node.id === id) {
            return true;
        }
        if (node.tipo === NodeTypes.Folder && node.data instanceof Array) {
            const content = node.data;
            for (let i = 0; i < content.length; i++) {
                let b = this.searchById(content[i], id);
                if (b) {
                    return b;
                }
            }
        }
        return false;
    }

    getUsedNames(caseId: string, path: string[]): string[] {
        const tree = this._trees[caseId];
        if (path.length === 0) {
            return tree.map(el => el.name);
        }
        let node: Node;
        for (let i = 0; i < tree.length; i++) {
            if (tree[i].name === path[0]) {
                node = this.search(tree[i], path.slice(1));
            }
        }
        if (node !== undefined) {
            return (node.data as Node[]).map(el => el.name);
        }
        return [];
    }

    removeFromTree(caseId: string, itemId: string) {
        for (const item of this._trees[caseId]) {
            if (item.tipo === NodeTypes.Folder) {
                this.searchAndRemove(itemId, item);
            }
        }

        this.$treeChanged.next(caseId);
    }

    addNode(caseId: string, path: string[], newNode: Node) {
        const tree = this._trees[caseId];
        if (path.length === 0) {
            tree.push(newNode);
            return;
        }
        let found = false;
        for (let i = 0; i < tree.length; i++) {
            if (tree[i].name === path[0]) {
                this.addNodeInFolder(tree[i], path.slice(1), newNode);
                found = true;
                break;
            }
        }
        if (!found) {
            tree.push({
                fixa: false,
                name: path[0],
                tipo: NodeTypes.Folder,
                data: []
            });
            this.addNodeInFolder(tree[tree.length - 1], path.slice(1), newNode);
        }

        this.$treeChanged.next(caseId);
    }

    private removeCalculationFilterFolder(caseId: string) {
        const tree = this._trees[caseId];
        const calcI = tree.findIndex(el => el.name === 'Cálculos');
        if (calcI >= 0) {
            const filtroI = (tree[calcI].data as Node[]).findIndex(el => el.name === 'Filtros');
            if (filtroI >= 0) {
                (tree[calcI].data as Node[]).splice(filtroI, 1);
            }
        }
    }

    private addNodeInFolder(currFolder: Node, path: string[], newNode: Node) {
        if (currFolder.data instanceof Array) {
            if (path.length === 0) {
                currFolder.data.push(newNode);
                return;
            }
            let found = false;
            for (let i = 0; i < currFolder.data.length; i++) {
                if (currFolder.data[i].name === path[0]) {
                    this.addNodeInFolder(currFolder.data[i], path.slice(1), newNode);
                    found = true;
                    break;
                }
            }
            if (!found) {
                currFolder.data.push({
                    fixa: false,
                    name: path[0],
                    tipo: NodeTypes.Folder,
                    data: []
                });
                this.addNodeInFolder(currFolder.data[currFolder.data.length - 1], path.slice(1), newNode);
            }
        }
    }

    private search(currFolder: Node, path: string[]): Node | undefined {
        if (path.length === 0) {
            return currFolder;
        }
        if (currFolder.tipo === NodeTypes.Folder && currFolder.data instanceof Array) {
            const content = currFolder.data;
            for (let i = 0; i < content.length; i++) {
                if (content[i].name === path[0]) {
                    return this.search(content[i], path.slice(1));
                }
            }
        }
        return undefined;
    }

    private searchAndRemove(id: string, currFolder: Node) {
        if (currFolder.tipo === NodeTypes.Folder && currFolder.data instanceof Array) {
            const content = currFolder.data;
            for (let i = 0; i < content.length; i++) {
                if (content[i].id === id) {
                    content.splice(i, 1);
                    return;
                } else if (content[i].tipo === NodeTypes.Folder) {
                    this.searchAndRemove(id, content[i]);
                }
            }
        }
    }

    private updateTreeDataset(tree: Node[], caseId) {
        for (const item of tree) {
            if (item.tipo === NodeTypes.Folder && item.data instanceof Array) {
                this.updateTreeDataset(item.data, caseId);
            } else if (item.tipo === NodeTypes.Trajetória) {
                item.id = TrajectoryDatasetService.getTrajectoryId(caseId);
                if (this.dataset.hasById(item.id)) {
                    item.data = this.dataset.getById(item.id);
                }
            } else {
                if (this.dataset.hasById(item.id)) {
                    item.data = this.dataset.getById(item.id);
                    if (item.data['name']) {
                        item.name = item.data['name'];
                    }
                } else {
                    item.data = undefined;
                    console.error('Item invalid on tree', item);
                }
            }
        }
    }
}
