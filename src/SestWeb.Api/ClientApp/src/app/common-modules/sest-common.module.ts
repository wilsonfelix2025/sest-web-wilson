import { NgModule } from '@angular/core';
import { TopHorizontalMenuComponent } from '@components/common/top-horizontal-menu/top-horizontal-menu.component';
import { LayoutLoaderComponent } from '@components/common/layout-loader/layout-loader.component';
import { AppSwitcherComponent } from '@components/common/app-switcher/app-switcher.component';
import { CommonModule } from '@angular/common';
import { UserMenuComponent } from '@components/common/user-menu/user-menu.component';
import { MaterialCommonModule } from './material-common.module';
import { NotybarComponent } from '@components/common/notybar/notybar.component';
import { SideScrollDirective } from '@directives/side-scroll.directive';
import { NotificationSidenavComponent } from '@components/common/notification-sidenav/notification-sidenav.component';
import { HttpClientModule } from '@angular/common/http';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { FlexLayoutModule } from '@angular/flex-layout';

const moduleDeclarations = [
  UserMenuComponent,
  TopHorizontalMenuComponent,
  LayoutLoaderComponent,
  AppSwitcherComponent,
  NotybarComponent,
  NotificationSidenavComponent,
  SideScrollDirective,
];

const moduleImports: any[] = [
  CommonModule,
  HttpClientModule,
  FormsModule,
  ReactiveFormsModule,
  FlexLayoutModule,
  MaterialCommonModule,
];

@NgModule({
  imports: moduleImports,
  declarations: moduleDeclarations,
  exports: [...moduleDeclarations, ...moduleImports]
})

export class SestCommonModule {}
