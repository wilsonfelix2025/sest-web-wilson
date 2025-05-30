import { Component, OnInit } from '@angular/core';
import { ExplorerNavigatorService } from '@services/explorer-navigator.service';

@Component({
    selector: 'app-breadcrumb',
    templateUrl: './breadcrumb.component.html',
    styleUrls: ['./breadcrumb.component.scss']
})
export class BreadcrumbComponent implements OnInit {
    /**
     * The list of breadcrumbs currently being displayed on the explorer.
     *
     * The label is the text being displayed, while the index is the position
     * of that particular item on its respective list of entries. For instance,
     * after selecting a particular unit called 'UO-BA' which may be the second
     * item on the list of units, a new breadcrumb will be added with label
     * 'UO-BA' and index 1 (array indexes are 0-based).
     */
    breadcrumbs: string[] = [];

    /**
     * The icons of each breadcrumb level.
     */
    readonly levelIcons = [
        'fa-folder-open',
        'fa-cube',
        'fa-tint'
    ];

    constructor(private nav: ExplorerNavigatorService) {
        this.nav.onNavStackChange.subscribe(stack => {
            this.breadcrumbs = stack.map(elem => elem.name);
        });
    }

    jumpBackToLevel(level: number) {
        const stack = this.nav.navStack;
        this.nav.replace(stack.slice(0, level));
    }

    ngOnInit() { }
}
