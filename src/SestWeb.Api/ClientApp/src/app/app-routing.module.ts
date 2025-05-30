import { NgModule } from '@angular/core';
import { Routes, RouterModule, Router } from '@angular/router';

import { ExplorerPageComponent } from './pages/explorer/explorer.page';
import { OAuthHandlerComponent } from './auth/oauth-handler/oauth-handler.component';
import { EditorPageComponent } from './pages/editor/editor.page';

const routes: Routes =
[
  {
    path: 'explorer',
    component: ExplorerPageComponent,
    loadChildren: () => import('./pages/explorer/modules/explorer.module').then(m => m.ExplorerModule)
  },
  {
    path: 'editor',
    component: EditorPageComponent,
    loadChildren: () => import('./pages/editor/modules/editor.module').then(m => m.EditorModule)
  },

  {
    path: 'external',
    loadChildren: () => import('./pages/common/common-pages.module').then(m => m.CommonPagesModule)
  },
  {
    path: '',
    redirectTo: 'explorer',
    pathMatch: 'full',
    data: {
      base: true
    }
  },
  {
    path: '**',
    redirectTo: '/external/404'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
  entryComponents: []
})
export class AppRoutingModule {
  constructor(private router: Router) {

  }
}
