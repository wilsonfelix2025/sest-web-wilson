import { Component, OnInit } from '@angular/core';

@Component({
    selector: 'app-switcher',
    templateUrl: './app-switcher.component.html',
    styleUrls: ['./app-switcher.component.scss']
})
export class AppSwitcherComponent implements OnInit {

    constructor() { }

    ngOnInit() {
        // This is done here to prevent concurrency problems
        const el = document.getElementById('appSwitcherMenu');
        // PocowebSwitcher is added to window via <script> on index.html
        window['PocowebSwitcher']('SESTTR', {
            pocoweb_url: 'https://pocoweb.petro.intelie.net/',
            target: el,
            load_style: true
        });
    }
}
