import { Routes, RouterModule } from '@angular/router';
import { ModuleWithProviders } from '@angular/core';

import { Error404Component } from './error-404/error-404.component';

const routes: Routes = [
{
  path: '404',
  component: Error404Component,
  data: {
    title: 'Error 404'
  }
}];

export const CommonPagesRoutingModule: ModuleWithProviders = RouterModule.forChild(routes);
