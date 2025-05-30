import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

if (environment.production) {
  enableProdMode();
}

const compilerOptions: any = {};

platformBrowserDynamic().bootstrapModule(AppModule, compilerOptions)
  .catch(err => console.log(err));
