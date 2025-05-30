import { Injectable } from '@angular/core';
import { OpunitService } from '../repositories/opunit.service';
import { OpUnit } from '../repositories/models/opunit';
import { Subject, of, Observable, BehaviorSubject } from 'rxjs';
import { OilfieldService } from '../repositories/oilfield.service';
import { Oilfield } from '../repositories/models/oilfield';
import { WellService } from '../repositories/well.service';
import { Well } from '../repositories/models/well';
import { NavStackItem } from './explorer-navigator.service';
import { OAuthTokenService } from './oauth.service';
import { HttpClient } from '@angular/common/http';
import { E_TYPES as types } from '@utils/explorerTypesEnum';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ExplorerDataService {

    /**
     * List of all operational units loaded.
     */
    operationalUnits: any[];

    loadedItems = [];

    onLoadedItem = new BehaviorSubject<ILoadedItem>({ type: null, id: 0 });

    openMenuTab = new Subject<{ element: any }>();

    fileOpened = new Subject<number>();

    fileDeleted = new Subject();

    treeUpdated = new Subject();

    currentView = [];

    constructor(
        private oauth: OAuthTokenService,
        private opunits: OpunitService,
        private oilfields: OilfieldService,
        private wells: WellService,
        private http: HttpClient) {
        this.onLoadedItem.subscribe(evt => {
            this.loadedItems[evt.type] = this.loadedItems[evt.type] || {};
            this.loadedItems[evt.id] = true;
        });

        this.oauth.$userAuthenticated.subscribe(isAuthenticated => {
            if (isAuthenticated) {
                this.loadTree();
            }
        });

        this.oauth.$userAuthenticated.next(true);
    }

    closeTree(id: number, targetLevel: types) {
        let openedOilfield = false;
        let openedOpunit = false;
        this.operationalUnits.forEach(unit => {
            unit['children'].forEach(oilfield => {
                oilfield['children'].forEach(well => {
                    if (targetLevel === types.Case && well['id'] === id) {
                        well['collapsed'] = false;
                        openedOilfield = true;
                    } else {
                        well['collapsed'] = true;
                    }
                });
                if ((targetLevel === types.OilField && oilfield['id'] === id) || openedOilfield) {
                    oilfield['collapsed'] = false;
                    openedOilfield = false;
                    openedOpunit = true;
                } else {
                    oilfield['collapsed'] = true;
                }
            });
            if ((targetLevel === types.OilField && unit['id'] === id) || openedOpunit) {
                unit['collapsed'] = false;
                openedOpunit = false;
            } else {
                unit['collapsed'] = true;
            }
        });
    }

    getCurrPath(id: number, targetLevel: types) {
        let openedWell = false;
        let openedOilfield = false;
        let openedOpunit = false;
        let currOpUnit;
        let currOilField;
        let currlWell;

        this.operationalUnits.forEach(unit => {
            unit['children'].forEach(oilfield => {
                oilfield['children'].forEach(well => {
                    well['children'].forEach(file => {
                        if (targetLevel === types.Case && file['id'] === id) {
                            openedWell = true;
                            return;
                        }
                    });
                    if ((targetLevel === types.Well && well['id'] === id) || openedWell) {
                        currlWell = well;
                        openedWell = false;
                        openedOilfield = true;
                        return;
                    }
                });
                if ((targetLevel === types.OilField && oilfield['id'] === id) || openedOilfield) {
                    currOilField = oilfield;
                    openedOilfield = false;
                    openedOpunit = true;
                    return;
                }
            });
            if ((targetLevel === types.OpUnit && unit['id'] === id) || openedOpunit) {
                currOpUnit = unit;
                openedOpunit = false;
                return;
            }
        });
        return { currOpUnit, currOilField, currlWell };
    }

    loadTree() {
        this.http.get(`${environment.appUrl}/api/pocoweb/tree`).subscribe((resp: any) => {
            const units = resp['tree'];
            units.sort((a, b) => a.name.localeCompare(b.name));
            for (const unit of units) {
                unit['id'] = this.extractId(unit);
                unit.children.sort((a, b) => a.name.localeCompare(b.name));
                this.postProcessItem(unit, 'children');

                for (const oilfield of unit.children) {
                    oilfield['id'] = this.extractId(oilfield);
                    this.postProcessItem(oilfield, 'children', unit);

                    for (const well of oilfield.children) {
                        well['id'] = this.extractId(well);
                        this.postProcessItem(well, 'children', oilfield);
                        this.onLoadedItem.next({ type: ExplorerLevel.Well, id: well['id'] });
                    }
                    this.onLoadedItem.next({ type: ExplorerLevel.Oilfield, id: oilfield['id'] });
                }

                this.onLoadedItem.next({ type: ExplorerLevel.Unit, id: unit['id'] });
            }
            this.operationalUnits = units;
            this.onLoadedItem.next({ type: ExplorerLevel.UnitsList, id: null });
        });
    }

    extractId(item) {
        const urlParts = item.url.split('/');
        const idIndexAfterSplit = urlParts.length - 2;

        return parseInt(urlParts[idIndexAfterSplit]);
    }

    postProcessItem(item, child?: string, parent?) {
        item['parentIds'] = [];
        item['collapsed'] = item['collapsed'] !== undefined ? item['collapsed'] : true;

        if (parent) {
            item['parentIds'] = parent['parentIds'].concat({ id: parent['id'], name: parent['name'] });
        }

        if (child) {
            item[child].sort((a, b) => a.name.localeCompare(b.name));
            item['hasChildren'] = item[child] && item[child].length > 0;
        }
    }

    /**
     * Loads every operational unit in the system.
     */
    loadOperationalUnits() {
        // Fetch all operational units
        return this.opunits.getAll().then(units => {
            // Sort units alphabetically
            units.sort((a, b) => a.name.localeCompare(b.name));
            // Remove operational units that don't have any oilfields
            for (const unit of units) {
                unit['id'] = this.extractId(unit);
                this.postProcessItem(unit, 'oilfields');
                unit.oilfields.sort((a, b) => a.name.localeCompare(b.name));
                this.onLoadedItem.next({ type: ExplorerLevel.Unit, id: unit['id'] });
            }

            this.operationalUnits = units;
            this.onLoadedItem.next({ type: ExplorerLevel.UnitsList, id: null });
        }, err => {
            console.error('Erro ao listar as unidades operacionais: ', err);
        });
    }

    /**
      * Loads every oilfield of a given operational unit.
      *
      * @param {OpUnit} unit the unit of which the oilfields must be loaded.
      */
    loadOilfields(unit: OpUnit) {
        unit['children'].forEach(oilfield => {
            oilfield['id'] = this.extractId(oilfield);

            if (!oilfield['hasLoaded']) {
                this.oilfields.getById(oilfield['id']).then(oilfieldInfo => {
                    oilfield.wells = oilfieldInfo.wells;
                    this.postProcessItem(oilfield, 'wells', unit);
                    this.onLoadedItem.next({ type: ExplorerLevel.Oilfield, id: oilfield['id'] });
                    oilfield['hasLoaded'] = true;
                });
            }
        });
    }

    /**
     * Loads every well of a given oilfield.
     *
     * @param {Oilfield} oilfield the oilfield of which the wells must be loaded.
     */
    loadWells(oilfield: Oilfield) {
        oilfield.wells.forEach(well => {
            well['id'] = this.extractId(well);

            if (!well['hasLoaded']) {
                this.wells.getById(well['id']).then(wellInfo => {
                    well.files = wellInfo.files;
                    this.postProcessItem(well, 'files', oilfield);
                    this.onLoadedItem.next({ type: ExplorerLevel.Well, id: well['id'] });
                    well['hasLoaded'] = true;
                });
            }
        });
    }

    /**
     * Open the given file in a new tab.
     *
     * @param {File} file the file to be loaded.
     */
    openFile(file: File) {
        const id = this.extractId(file);
        this.fileOpened.next(id);
    }

    loadItem(item, targetLevel) {
        switch (targetLevel) {
            // case ExplorerLevel.Unit: {
            //     this.loadOilfields(item);
            //     break;
            // }
            // case ExplorerLevel.Oilfield: {
            //     this.loadWells(item);
            //     break;
            // }
            case ExplorerLevel.File: {
                this.openFile(item);
                break;
            }
        }
    }

    loadFromUrl(urlParams) {
        return new Observable((observer) => {
            let unit: OpUnit, oilfield: Oilfield, well: Well;

            let unitNavStackItem: NavStackItem,
                oilfieldNavStackItem: NavStackItem,
                wellNavStackItem: NavStackItem;

            if (!urlParams['unit']) {
                // observer.next(this.operationalUnits.slice());
                observer.next([]);
                observer.unsubscribe();
                return;
            }

            this.onLoadedItem.subscribe(item => {
                if (item.type === ExplorerLevel.UnitsList) {
                    unit = this.operationalUnits.filter(el => el['id'] === parseInt(urlParams['unit']))[0];
                    unitNavStackItem = { id: unit['id'], name: unit['name'] };

                    if (!urlParams['oilfield']) {
                        // observer.next(unit.oilfields);
                        observer.next([unitNavStackItem]);
                        observer.unsubscribe();
                    } else {
                        oilfield = unit['children'].filter(el => el['id'] === parseInt(urlParams['oilfield']))[0];
                        oilfieldNavStackItem = { id: oilfield['id'], name: oilfield['name'] };
                        if (!urlParams['well']) {
                            // observer.next(oilfield.wells);
                            observer.next([unitNavStackItem, oilfieldNavStackItem]);
                            observer.unsubscribe();
                        } else {
                            well = oilfield['children'].filter(el => el['id'] === parseInt(urlParams['well']))[0];
                            wellNavStackItem = { id: well['id'], name: well['name'] };
                            // observer.next(well.files);
                            observer.next([unitNavStackItem, oilfieldNavStackItem, wellNavStackItem]);
                            observer.unsubscribe();
                        }
                    }
                }
            });
        });
    }
}

export enum ExplorerLevel {
    UnitsList,
    Unit,
    Oilfield,
    Well,
    File
}

interface ILoadedItem {
    type: ExplorerLevel;
    id: number;
}
