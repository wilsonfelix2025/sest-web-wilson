import { NgModule } from '@angular/core';

import { ExplorerComponent } from '@components/explorer/explorer-content/explorer.component';
import { BreadcrumbComponent } from '@components/explorer/explorer-breadcrumb/breadcrumb.component';
import { ExplorerRoutingModule } from './explorer-routing.module';
import { DialogDeleteComponent } from '@components/explorer/dialog-delete/dialog-delete.component';
import { DialogDuplicateComponent } from '@components/explorer/dialog-duplicate/dialog-duplicate.component';
import { DialogCreateComponent } from '@components/explorer/dialog-create/dialog-create.component';

// Pipes
import { DialogMoveComponent } from '@components/explorer/dialog-move/dialog-move.component';
import { DialogRenameComponent } from '@components/explorer/dialog-rename/dialog-rename.component';
import { CreateMenuComponent } from '@components/explorer/create-menu/create-menu.component';
import { SestScrollTopDirective } from '@directives/sest-scroll-top.directive';
import { MenuSidenavComponent } from '@components/explorer/menu-sidenav/menu-sidenav.component';
import { ExplorerPageComponent } from '../explorer.page';
import { SestCommonModule } from 'app/common-modules/sest-common.module';

@NgModule({
  imports: [
    SestCommonModule,
    ExplorerRoutingModule,
  ],
  declarations: [
    ExplorerComponent,
    BreadcrumbComponent,
    DialogDeleteComponent,
    DialogMoveComponent,
    DialogRenameComponent,
    DialogDuplicateComponent,
    DialogCreateComponent,
    CreateMenuComponent,
    MenuSidenavComponent,
    SestScrollTopDirective,
    ExplorerPageComponent,
  ],
  entryComponents: [
    DialogDeleteComponent,
    DialogDuplicateComponent,
    DialogMoveComponent,
    DialogRenameComponent,
    DialogCreateComponent
  ]
})
export class ExplorerModule {}
