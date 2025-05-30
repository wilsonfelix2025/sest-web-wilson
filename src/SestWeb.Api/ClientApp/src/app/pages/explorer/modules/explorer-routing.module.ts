import { Routes, RouterModule } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';

import { AuthGuard } from '../../../auth/auth.guard';
import { ExplorerComponent } from '@components/explorer/explorer-content/explorer.component';

const routes: Routes = [
{
  path: '',
  component: ExplorerComponent,
  canActivate: [AuthGuard],
  data: {
    title: 'Explorer'
  },
  children: [{
    path: 'unidade/:unit',
    component: ExplorerComponent,
    children: [{
        path: 'campo/:oilfield',
        component: ExplorerComponent,
        children: [{
            path: 'poco/:well',
            component: ExplorerComponent
        }]
    }]
  }]
}];

export const ExplorerRoutingModule: ModuleWithProviders = RouterModule.forChild(routes);
