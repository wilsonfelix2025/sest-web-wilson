import { Routes, RouterModule } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';

import { HomeContentComponent } from '@components/editor/home-content/home-content.component';


const routes: Routes = [
  {
    path: ':id',
    component: HomeContentComponent,
    data: {
      title: 'Home'
    }
  }
];

export const EditorRoutingModule: ModuleWithProviders = RouterModule.forChild(routes);
