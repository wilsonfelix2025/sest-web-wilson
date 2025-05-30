import { NgModule } from '@angular/core';

import { EditorRoutingModule } from './editor-routing.module';
import { EditorPageComponent } from '../editor.page';

import { HomeContentComponent } from '@components/editor/home-content/home-content.component';
import { SestCommonModule } from 'app/common-modules/sest-common.module';
import { BreadcrumbComponent } from '@components/editor/breadcrumb/breadcrumb.component';
import { WindowEditCaseDataComponent } from '@components/editor/window-edit-case-data/window-edit-case-data.component';
import { DadosGeraisComponent } from '@components/editor/window-edit-case-data/dados-gerais/dados-gerais.component';
import { GeometriaComponent } from '@components/editor/window-edit-case-data/geometria/geometria.component';
import { SapatasComponent } from '@components/editor/window-edit-case-data/sapatas/sapatas.component';
import { ObjetivosComponent } from '@components/editor/window-edit-case-data/objetivos/objetivos.component';
import { EstratigrafiaComponent } from '@components/editor/window-edit-case-data/estratigrafia/estratigrafia.component';
import { MenuTreeComponent } from '@components/editor/menu-tree/menu-tree.component';
import { EstratCanvasComponent } from '@components/editor/estrat-canvas/estrat-canvas.component';
import { LitoCanvasComponent } from '@components/editor/lito-canvas/lito-canvas.component';
import { TrajetCanvasComponent } from '@components/editor/trajet-canvas/trajet-canvas.component';
import { GraphicTabComponent } from '@components/editor/graphic-tab/graphic-tab.component';
import { CanvasMenuComponent } from '@components/editor/canvas-menu/canvas-menu.component';
import { DialogCreateTabComponent } from '@components/editor/dialog-create-tab/dialog-create-tab.component';
import { DialogDeleteHomeComponent } from '@components/editor/dialog-delete-home/dialog-delete-home.component';
import { DataTreeComponent } from '@components/editor/data-tree/data-tree.component';
import { AccordionTreeComponent } from '@components/editor/accordion-tree/accordion-tree.component';
import { NodeTreeComponent } from '@components/editor/node-tree/node-tree.component';
import { ToolbarComponent } from '@components/editor/toolbar/toolbar.component';
import { StatusbarComponent } from '@components/editor/statusbar/statusbar.component';
import { GraphicAreaComponent } from '@components/editor/graphic-area/graphic-area.component';
import { GraphicCanvasComponent } from '@components/editor/graphic-canvas/graphic-canvas.component';
import { DialogEditXScaleComponent } from '@components/editor/dialog-edit-x-scale/dialog-edit-x-scale.component';
import { DialogEditWidthComponent } from '@components/editor/dialog-edit-width/dialog-edit-width.component';
import { ImportComponent } from '@components/editor/import/import.component';
import { ImportSestWebComponent } from '@components/editor/import/import-sest-web/import-sest-web.component';
import { DialogColumnPropertiesComponent } from '@components/editor/dialog-column-properties/dialog-column-properties.component';
import { SelectTypeComponent } from '@components/editor/import/import-external-file/select-type/select-type.component';
import { DataSetComponent } from '@components/editor/import/import-external-file/data-set/data-set.component';
import { InfoSpreadsheetLithologyComponent } from '@components/editor/import/import-external-file/info-spreadsheet-lithology/info-spreadsheet-lithology.component';
import { InfoSpreadsheetProfileComponent } from '@components/editor/import/import-external-file/info-spreadsheet-profile/info-spreadsheet-profile.component';
import { InfoSpreadsheetTrajectoryComponent } from '@components/editor/import/import-external-file/info-spreadsheet-trajectory/info-spreadsheet-trajectory.component';
import { FileInfoComponent } from '@components/editor/import/shared/file-info/file-info.component';
import { DragAndDropModule } from 'angular-draggable-droppable';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { HighchartsChartModule } from 'highcharts-angular';
import { HotTableModule } from 'ng2-handsontable';
import { SelectCorrelationComponent } from '@components/editor/window-correlaction/select-correlation/select-correlation.component';
import { EditCorrelationComponent } from '@components/editor/window-correlaction/edit-correlation/edit-correlation.component';
import { DataSummaryComponent } from '@components/editor/window-correlaction/data-summary/data-summary.component';
import { WindowInitialDataComponent } from '@components/editor/window-initial-data/window-initial-data.component';
import { ComplementoCurvaComponent } from '@components/editor/window-initial-data/complemento-curva/complemento-curva.component';

import { ColorPickerModule } from 'ngx-color-picker';
import { NewDialogDeleteComponent } from '@components/editor/new-dialog-delete/new-dialog-delete.component';
import { WindowEditTrendComponent } from '@components/editor/window-edit-trend/window-edit-trend.component';
import { WindowFilterComponent } from '@components/editor/window-filter/window-filter.component';
import { DialogImageComponent } from '@components/editor/dialog-image/dialog-image.component';
import { ProfileCalculationsComponent } from '@components/editor/window-calculations/profile-calculations/profile-calculations.component';
import { OverloadCalculationComponent } from '@components/editor/window-calculations/overload-calculation/overload-calculation.component';
import { ImportCaseComponent } from '@components/editor/import/import-case/import-case.component';
import { WindowEditDeepDataComponent } from '@components/editor/window-edit-deep-data/window-edit-deep-data.component';
import { DeepDataTableComponent } from '@components/editor/window-edit-deep-data/deep-data-table/deep-data-table.component';
import { EditProfileStyleComponent } from '@components/editor/window-edit-deep-data/edit-profile-style/edit-profile-style.component';
import { InsertDataComponent } from '@components/editor/window-edit-deep-data/insert-data/insert-data.component';
import { MontagemPerfisComponent } from '@components/editor/window-initial-data/montagem-perfis/montagem-perfis.component';
import { SelecaoPerfisComponent } from '@components/editor/window-initial-data/selecao-perfis/selecao-perfis.component';
import { MechanicalCalculationsComponent } from '@components/editor/window-calculations/mechanical-calculations/mechanical-calculations.component';
import { PorePressureCalculationComponent } from '@components/editor/window-calculations/pore-pressure-calculation/pore-pressure-calculation.component';
import { PpReservatorioComponent } from '@components/editor/window-calculations/pore-pressure-calculation/pp-reservatorio/pp-reservatorio.component';
import { ExportComponent } from '@components/editor/export/export.component';
import { ProfilesExportComponent } from '@components/editor/export/profiles-export/profiles-export.component';
import { RecordsExportComponent } from '@components/editor/export/records-export/records-export.component';
import { TensionsInsituCalculationComponent } from '@components/editor/window-calculations/tensions-insitu-calculation/tensions-insitu-calculation.component';
import { TensionStepTwoComponent } from '@components/editor/window-calculations/tensions-insitu-calculation/tension-step-two/tension-step-two.component';
import { RelationTensionsComponent } from '@components/editor/window-calculations/tensions-insitu-calculation/relation-tensions/relation-tensions.component';
import { WellSelectorComponent } from '@components/editor/import/import-external-file/well-selector/well-selector.component';
import { RightClickDirective } from '@directives/right-click.directive';
import { GradientCalculationComponent } from '@components/editor/window-calculations/gradient-calculation/gradient-calculation.component';
import { PoroelasticParametersComponent } from '@components/editor/window-calculations/gradient-calculation/poroelastic-parameters/poroelastic-parameters.component';
import { TrechoEspecificoComponent } from '@components/editor/window-calculations/gradient-calculation/trecho-especifico/trecho-especifico.component';
import { WindowProfileCompositionComponent } from '@components/editor/window-profile-composition/window-profile-composition.component';
import { ExponentDComponent } from '@components/editor/window-perforations/exponent-d/exponent-d.component';
import { RtoConnectionComponent } from '@components/editor/rto-connection/rto-connection.component';
import { LinkDataComponent } from '@components/editor/rto-connection/link-data/link-data.component';
import { CompositionChartComponent } from '@components/editor/window-profile-composition/composition-chart/composition-chart.component';
import { EventsComponent } from '@components/editor/window-perforations/events/events.component';
import { RecordsComponent } from '@components/editor/window-perforations/records/records.component';
import { RecordsTableComponent } from '@components/editor/window-perforations/records/records-table/records-table.component';
import { MarkersComponent } from '@components/editor/window-perforations/shared/markers/markers.component';
import { WindowLegendComponent } from '@components/editor/window-legend/window-legend.component';
import { ReportExportComponent } from '@components/editor/export/report-export/report-export.component';
import { NoConvergenceComponent } from '@components/editor/window-calculations/gradient-calculation/no-convergence/no-convergence.component';
import { DialogEditNameComponent } from '@components/editor/dialog-edit-name/dialog-edit-name.component';
import { DialogConfirmComponent } from '@components/editor/dialog-confirm/dialog-confirm.component';

@NgModule({
  imports: [
    SestCommonModule,
    EditorRoutingModule,
    DragDropModule,
    DragAndDropModule,
    HighchartsChartModule,
    HotTableModule,
    ColorPickerModule,
  ],
  declarations: [
    EditorPageComponent,
    HomeContentComponent,
    DataTreeComponent,
    BreadcrumbComponent,
    DadosGeraisComponent,
    GeometriaComponent,
    SapatasComponent,
    ObjetivosComponent,
    EstratigrafiaComponent,
    MenuTreeComponent,
    EstratCanvasComponent,
    LitoCanvasComponent,
    TrajetCanvasComponent,
    GraphicTabComponent,
    CanvasMenuComponent,
    DialogCreateTabComponent,
    DialogImageComponent,
    DialogDeleteHomeComponent,
    AccordionTreeComponent,
    NodeTreeComponent,
    ToolbarComponent,
    StatusbarComponent,
    GraphicAreaComponent,
    GraphicCanvasComponent,
    DialogEditXScaleComponent,
    DialogEditWidthComponent,
    DialogEditNameComponent,
    DialogConfirmComponent,
    ImportComponent,
    DialogColumnPropertiesComponent,
    SelectTypeComponent,
    DataSetComponent,
    InfoSpreadsheetLithologyComponent,
    InfoSpreadsheetProfileComponent,
    InfoSpreadsheetTrajectoryComponent,
    FileInfoComponent,
    SelectCorrelationComponent,
    EditCorrelationComponent,
    WindowEditCaseDataComponent,
    ImportSestWebComponent,
    DataSummaryComponent,
    DataSummaryComponent,
    WindowInitialDataComponent,
    ComplementoCurvaComponent,
    WindowEditDeepDataComponent,
    DeepDataTableComponent,
    InsertDataComponent,
    EditProfileStyleComponent,
    NewDialogDeleteComponent,
    WindowEditTrendComponent,
    WindowFilterComponent,
    ProfileCalculationsComponent,
    OverloadCalculationComponent,
    ExponentDComponent,
    ImportCaseComponent,
    MontagemPerfisComponent,
    SelecaoPerfisComponent,
    MechanicalCalculationsComponent,
    PorePressureCalculationComponent,
    PpReservatorioComponent,
    ExportComponent,
    ProfilesExportComponent,
    RecordsExportComponent,
    TensionsInsituCalculationComponent,
    TensionStepTwoComponent,
    RelationTensionsComponent,
    RightClickDirective,
    GradientCalculationComponent,
    NoConvergenceComponent,
    WellSelectorComponent,
    RightClickDirective,
    PoroelasticParametersComponent,
    TrechoEspecificoComponent,
    WindowProfileCompositionComponent,
    CompositionChartComponent,
    RtoConnectionComponent,
    LinkDataComponent,
    EventsComponent,
    RecordsComponent,
    MarkersComponent,
    RecordsTableComponent,
    WindowLegendComponent,
    ReportExportComponent,
  ],
  entryComponents: [
    DialogCreateTabComponent,
    DialogImageComponent,
    DialogDeleteHomeComponent,
    DialogEditXScaleComponent,
    DialogEditWidthComponent,
    DialogEditNameComponent,
    DialogConfirmComponent,
    ImportComponent,
    DialogColumnPropertiesComponent,
    InfoSpreadsheetLithologyComponent,
    InfoSpreadsheetProfileComponent,
    InfoSpreadsheetTrajectoryComponent,
    SelectTypeComponent,
    FileInfoComponent,
    SelectCorrelationComponent,
    EditCorrelationComponent,
    WindowEditCaseDataComponent,
    ImportSestWebComponent,
    EditCorrelationComponent,
    WindowInitialDataComponent,
    ComplementoCurvaComponent,
    WindowEditDeepDataComponent,
    DeepDataTableComponent,
    InsertDataComponent,
    EditProfileStyleComponent,
    NewDialogDeleteComponent,
    WindowEditTrendComponent,
    WindowFilterComponent,
    ProfileCalculationsComponent,
    OverloadCalculationComponent,
    ExponentDComponent,
    ImportCaseComponent,
    MontagemPerfisComponent,
    SelecaoPerfisComponent,
    MechanicalCalculationsComponent,
    PorePressureCalculationComponent,
    PpReservatorioComponent,
    ExportComponent,
    ProfilesExportComponent,
    RecordsExportComponent,
    TensionsInsituCalculationComponent,
    TensionStepTwoComponent,
    RelationTensionsComponent,
    GradientCalculationComponent,
    NoConvergenceComponent,
    WellSelectorComponent,
    PoroelasticParametersComponent,
    TrechoEspecificoComponent,
    WindowProfileCompositionComponent,
    CompositionChartComponent,
    RtoConnectionComponent,
    LinkDataComponent,
    EventsComponent,
    RecordsComponent,
    MarkersComponent,
    RecordsTableComponent,
    WindowLegendComponent,
    ReportExportComponent,
  ],
})
export class EditorModule { }
