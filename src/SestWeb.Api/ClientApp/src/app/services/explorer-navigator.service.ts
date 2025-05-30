import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, Subject } from 'rxjs';

@Injectable({
    providedIn: 'root'
})
export class ExplorerNavigatorService {

    readonly NAVSTACK_MAX_LENGTH = 3;
    readonly NAVSTACK_MAX_INDEX = this.NAVSTACK_MAX_LENGTH - 1;

    private _navStack: NavStackItem[] = [];
    get navStack(): NavStackItem[] { return this._navStack; }

    onNavStackChange = new Subject<NavStackItem[]>();

    constructor(private router: Router) {
        window['nav'] = this;
    }

    update() {
        this.onNavStackChange.next(this.navStack);
        this.assembleUrl();
        this.redirect();
    }

    push(entry: NavStackItem) {
        if (this.navStack.length < this.NAVSTACK_MAX_LENGTH) {
            this.navStack.push(entry);
            this.update();
        } else {
            // TODO error
        }
    }

    pop(amount = 1) {
        if (this.navStack.length === 0) {
            // TODO error
        } else if (amount <= 0) {
            // TODO error
        } else if (amount > this.navStack.length) {
            // TODO error
        } else {
            this._navStack = this.navStack.slice(0, this.navStack.length - amount);
            this.update();
        }
    }

    replace(array: NavStackItem[]) {
        if (array.length > this.NAVSTACK_MAX_LENGTH) {
            // TODO error
        } else {
            this._navStack = array.slice();
            this.update();
        }
    }

    replaceAt(index: number, entry: NavStackItem) {
        if (index < 0 || index > this.NAVSTACK_MAX_INDEX) {
            // TODO error
        } else if (index >= this.navStack.length) {
            // TODO error
        } else {
            this._navStack = this.navStack.slice(0, index);
            this.push(entry);
        }
    }

    assembleUrl() {
        const pathParts = ['unidade', 'campo', 'poco'];
        const urlParts = [];

        for (let i = 0; i < this.navStack.length; i++) {
            urlParts.push(`${pathParts[i]}/${this.navStack[i].id}`);
        }

        return urlParts.join('/');
    }

    redirect() {
        const path = this.assembleUrl();

        if (path) {
            this.router.navigateByUrl(`explorer/${path}`);
        } else {
            this.router.navigateByUrl(`explorer`);
        }
    }
}

export interface NavStackItem {
    id: number;
    name: string;
}
