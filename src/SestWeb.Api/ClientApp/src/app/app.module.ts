import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule, DomSanitizer } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MatIconRegistry } from '@angular/material';
import { HttpClient, HTTP_INTERCEPTORS } from '@angular/common/http';

// Modules
import { AppRoutingModule } from './app-routing.module';
import { OAuthModule } from 'angular-oauth2-oidc';

// Services
import { AuthenticationService } from './services/authentication.service';
import { DialogService } from './services/dialog.service';

// Repositories
import { OpunitService } from './repositories/opunit.service';

// Interceptors
import { DEFAULT_TIMEOUT, HttpConfigInterceptor } from './interceptors/httpconfig.interceptor';

// Components
import { AppComponent } from './app.component';

import 'hammerjs';
import { ExplorerNavigatorService } from './services/explorer-navigator.service';
import { LoaderService } from './services/loader.service';
import { AuthModule } from './auth/auth.module';
import { EditorModule } from './pages/editor/modules/editor.module';
import { ExplorerModule } from './pages/explorer/modules/explorer.module';
import { environment } from 'environments/environment';

export function initApp(http: HttpClient) {
  return () => {
    // return http.get('/api/FrontEndConfig').toPromise().then((resp) => {
    //   environment.pocoWeb = resp['pocoWebUrl'];
    // });
  };
}
@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    OAuthModule.forRoot(),
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    EditorModule,
    ExplorerModule,
    AuthModule
  ],
  providers: [
    AuthenticationService,
    DialogService,
    OpunitService,
    ExplorerNavigatorService,
    LoaderService,
    { provide: HTTP_INTERCEPTORS, useClass: HttpConfigInterceptor, multi: true },
    { provide: DEFAULT_TIMEOUT, useValue: 600000 * 12 },
    { provide: APP_INITIALIZER, useFactory: initApp, multi: true, deps: [HttpClient] },
  ],
  bootstrap: [
    AppComponent
  ]
})
export class AppModule {
  constructor(
    private matIconRegistry: MatIconRegistry,
    private sanitizer: DomSanitizer
  ) {
    // Register Font Awesome
    matIconRegistry.registerFontClassAlias('fontawesome', 'fa');

    // Register layout icon SVGs
    matIconRegistry.addSvgIcon('classic',
      sanitizer.bypassSecurityTrustResourceUrl('assets/images/icons/classic.svg')
    );
  }
}
