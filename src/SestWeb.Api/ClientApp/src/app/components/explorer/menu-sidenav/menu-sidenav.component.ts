import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { Router, RouterEvent, NavigationStart } from '@angular/router';
import { Component, OnDestroy, Input } from '@angular/core';
import { MatSidenav } from '@angular/material';
import { ExplorerSearchService } from '@services/search.service';
import { ExplorerDataService, ExplorerLevel } from '@services/explorer-data.service';
import { ExplorerNavigatorService } from '@services/explorer-navigator.service';
import { environment } from '../../../../environments/environment';

@Component({
    selector: 'sest-menu-sidenav',
    templateUrl: './menu-sidenav.component.html',
    styleUrls: ['./menu-sidenav.component.scss']
})
export class MenuSidenavComponent implements OnDestroy {
    /**
     * Import material sidenav so we can access open close functions.
     */
    @Input() sidenav: MatSidenav;
    /**
     * Subscription to the Angular router.
     */
    routerSubscription: Subscription;
    /**
     * Anything that the user types on the user search bar.
     */
    userSearch: string;

    constructor(
        private router: Router,
        public nav: ExplorerNavigatorService,
        public explorerData: ExplorerDataService,
        public search: ExplorerSearchService
    ) {
        /**
         * Snippet responsible for closing the sidebar on state changes.
         */
        this.routerSubscription = this.router.events.pipe(
            filter(event => event instanceof NavigationStart),
        ).subscribe((event: RouterEvent) => {
            if (this.sidenav && this.sidenav.mode === 'over' && this.sidenav.opened) {
                this.sidenav.close();
            }
        });

        this.explorerData.openMenuTab.subscribe(change => {
            setTimeout(() => {
                const elem = change.element;
                elem['collapsed'] = false;
            }, 1);
        });
    }

    handleTreeClick(item, targetLevel) {
        // this.explorerData.loadItem(item, targetLevel);
        if (!item['collapsed']) {
            this.nav.replace(item['parentIds']);
            item['collapsed'] = true;
        } else {
            this.explorerData.closeTree(item['id'], targetLevel);
            this.nav.replace(item['parentIds'].concat({ id: item['id'], name: item['name'] }));
            item['collapsed'] = false;
            item['opened'] = true;
        }
    }

    clearSearch() {
        this.userSearch = '';
        this.search.clearSearch();
    }

    selectSearchResult(item) {
        this.nav.replace(item['parentIds'].concat({ id: item['id'], name: item['name'] }));
    }

    ngOnDestroy(): void {
        this.routerSubscription.unsubscribe();
    }

    openFile(item) {
        console.log(item);
        if (environment.production || environment.staging) {
            window.open(`${environment.appUrl}/editor/${item.id}`, '_blank'); // producao
        } else {
            window.open(`${environment.appUrl}/sestweb/editor/${item.id}`, '_blank'); // localhost
        }
    }
}
