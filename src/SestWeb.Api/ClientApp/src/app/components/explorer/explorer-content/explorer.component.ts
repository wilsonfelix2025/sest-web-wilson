import { Component } from '@angular/core';
import { ExplorerSearchService } from '@services/search.service';
import { Router, NavigationEnd, ActivatedRoute, ActivatedRouteSnapshot } from '@angular/router';
import { ExplorerDataService, ExplorerLevel } from '@services/explorer-data.service';
import { ExplorerNavigatorService, NavStackItem } from '@services/explorer-navigator.service';
import { OpUnit } from '../../../repositories/models/opunit';
import { Oilfield } from '../../../repositories/models/oilfield';
import { Well } from '../../../repositories/models/well';
import { DialogService } from '@services/dialog.service';
import { environment } from '../../../../environments/environment';

@Component({
    selector: 'app-explorer',
    templateUrl: './explorer.component.html',
    styleUrls: ['./explorer.component.scss']
})
export class ExplorerComponent {
    /**
     * The slice of content currently being displayed on the explorer's folder
     * view.
     */
    explorerContent: ExplorerContent[];

    selectedItem: any;

    navStack = [];

    constructor(
        public explorerData: ExplorerDataService,
        public search: ExplorerSearchService,
        private nav: ExplorerNavigatorService,
        private router: Router,
        private route: ActivatedRoute,
        public dialog: DialogService
    ) {
        this.explorerData.currentView = this.getContent();
        this.nav.onNavStackChange.subscribe(val => {
            this.explorerData.currentView = this.getContent();
            this.selectedItem = null;
            this.navStack = val;
        });

        this.explorerData.onLoadedItem.subscribe(val => {
            if (val.type === ExplorerLevel.UnitsList) {
                this.explorerData.currentView = this.getContent();
                this.navStack = [];
            }
        });

        this.explorerData.fileOpened.subscribe(id => {
            if (environment.production || environment.staging) {
                window.open(`${environment.appUrl}/editor/${id}`, '_blank'); // producao
            } else {
                window.open(`${environment.appUrl}/sestweb/editor/${id}`, '_blank'); // localhost
            }
        });

        this.explorerData.fileDeleted.subscribe(el => {
            this.selectedItem = null;
        });

        this.explorerData.treeUpdated.subscribe(el => {
            this.explorerData.currentView = this.getContent();
        });

        this.router.events.subscribe(evt => {
            if (evt instanceof NavigationEnd) {
                this.explorerData.onLoadedItem.subscribe(val => {
                    if (val.type === ExplorerLevel.UnitsList) {
                        const urlParams = this.extractUrlInfo(this.route.snapshot);
                        this.explorerData.loadFromUrl(urlParams).subscribe((stack: NavStackItem[]) => {
                            this.nav.replace(stack);
                        });
                    }
                });
            }
        });
    }

    selectItem(item) {
        this.selectedItem = item;
    }

    openItem(item, targetLevel) {
        this.explorerData.closeTree(item['id'], targetLevel);
        this.explorerData.loadItem(item, targetLevel);
        this.nav.push({ id: item['id'], name: item['name'] });
    }

    extractUrlInfo(routeSnapshot: ActivatedRouteSnapshot): { unit?: string, oilfield?: string, well?: string } {
        const urlInfo = {};
        const params = ['unit', 'oilfield', 'well'];

        for (let i = 0; i < params.length; i++) {
            if (!routeSnapshot.firstChild) {
                break;
            }

            routeSnapshot = routeSnapshot.firstChild;
            urlInfo[params[i]] = routeSnapshot.params[params[i]];
        }

        return urlInfo;
    }

    /**
     * Processes the breadcrumbs to determine which content should be on-screen.
     *
     * @returns the content which should be on-screen.
     */
    getContent() {
        const stack = this.nav.navStack.map(elem => elem.id);

        let list = this.explorerData.operationalUnits;

        for (let i = 0; i < stack.length; i++) {
            for (let j = 0; j < list.length; j++) {
                if (list[j]['id'] === stack[i]) {
                    this.explorerData.openMenuTab.next({ element: list[j] });
                    list = list[j]['children'];
                    break;
                }
            }
        }
        return list;
    }
}

type ExplorerContent = OpUnit | Oilfield | Well | File;
