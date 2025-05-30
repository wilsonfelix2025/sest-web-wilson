import { NgModule } from '@angular/core';
import { CdkTableModule } from '@angular/cdk/table';

import {
  // Navigation.
  MatToolbarModule,
  MatSidenavModule,
  MatMenuModule,

  // Layout
  MatListModule,
  MatGridListModule,
  MatExpansionModule,
  MatCardModule,
  MatTabsModule,
  MatStepperModule,

  // Buttons & Indicators
  MatButtonModule,
  MatButtonToggleModule,
  MatChipsModule,
  MatIconModule,
  MatProgressSpinnerModule,
  MatProgressBarModule,

  // Form Controls.
  MatCheckboxModule,
  MatInputModule,
  MatRadioModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatAutocompleteModule,
  MatSliderModule,
  MatSlideToggleModule,
  MatSelectModule,

  // Popups & Modals
  MatDialogModule,
  MatTooltipModule,
  MatSnackBarModule,

  // Tables
  MatTableModule,
  MatSortModule,
  MatPaginatorModule,

  MatBadgeModule,
  MatTreeModule,

  // Services
  MatIconRegistry,
  MatDialog
} from '@angular/material';

const matModules = [
  // Navigation.
  MatToolbarModule,
  MatSidenavModule,
  MatMenuModule,

  // Layout
  MatListModule,
  MatGridListModule,
  MatExpansionModule,
  MatCardModule,
  MatTabsModule,
  MatStepperModule,

  // Buttons & Indicators
  MatButtonModule,
  MatButtonToggleModule,
  MatChipsModule,
  MatIconModule,
  MatProgressSpinnerModule,
  MatProgressBarModule,

  // Form Controls.
  MatCheckboxModule,
  MatInputModule,
  MatRadioModule,
  MatDatepickerModule,
  MatNativeDateModule,
  MatAutocompleteModule,
  MatSliderModule,
  MatSlideToggleModule,
  MatSelectModule,

  // Popups & Modals
  MatDialogModule,
  MatTooltipModule,
  MatSnackBarModule,

  // Table
  CdkTableModule,
  MatTableModule,
  MatSortModule,
  MatPaginatorModule,

  MatBadgeModule,
  MatTreeModule,
];

@NgModule({
  imports: matModules,
  exports: matModules,
  providers: [
    MatIconRegistry,
    MatDialog
  ]
})
export class MaterialCommonModule { }
