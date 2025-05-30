import { NgModule } from '@angular/core';

import { CommonPagesRoutingModule } from './common-pages-routing.module';

import { Error404Component } from './error-404/error-404.component';
import { SestCommonModule } from 'app/common-modules/sest-common.module';

@NgModule({
  imports: [
    SestCommonModule,
    CommonPagesRoutingModule,
  ],
  declarations: [
    Error404Component,
  ]
})
export class CommonPagesModule { }
