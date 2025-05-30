import { Component, OnInit, Output } from '@angular/core';
import { DialogService } from '@services/dialog.service';
import { ImportSestWebComponent } from '../import/import-sest-web/import-sest-web.component';
import { SelectCorrelationComponent } from '../window-correlaction/select-correlation/select-correlation.component';
import { ImportComponent } from '../import/import.component';
import { SelectTypeComponent } from '../import/import-external-file/select-type/select-type.component';
import { WindowFilterComponent } from '../window-filter/window-filter.component';
import { ProfileCalculationsComponent } from '../window-calculations/profile-calculations/profile-calculations.component';
import { OverloadCalculationComponent } from '../window-calculations/overload-calculation/overload-calculation.component';
import { Case } from 'app/repositories/models/case';
import { MechanicalCalculationsComponent } from '../window-calculations/mechanical-calculations/mechanical-calculations.component';
import { PorePressureCalculationComponent } from '../window-calculations/pore-pressure-calculation/pore-pressure-calculation.component';
import { ExportComponent } from '../export/export.component';
import { TensionsInsituCalculationComponent } from '../window-calculations/tensions-insitu-calculation/tensions-insitu-calculation.component';
import { GradientCalculationComponent } from '../window-calculations/gradient-calculation/gradient-calculation.component';
import { DatasetService } from '@services/dataset/dataset.service';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';
import { WindowProfileCompositionComponent } from '../window-profile-composition/window-profile-composition.component';
import { ExponentDComponent } from '../window-perforations/exponent-d/exponent-d.component';
import { RtoConnectionComponent } from '../rto-connection/rto-connection.component';
import { LinkDataComponent } from '../rto-connection/link-data/link-data.component';
import { Event } from '@angular/router';
import { EventEmitter } from '@angular/core';
import { NotybarService } from '@services/notybar.service';
import { EventsComponent } from '../window-perforations/events/events.component';
import { RecordsComponent } from '../window-perforations/records/records.component';

@Component({
  selector: 'sest-toolbar',
  templateUrl: './toolbar.component.html',
  styleUrls: ['./toolbar.component.scss'],
})
export class ToolbarComponent implements OnInit {

  /**
   * O caso de estudo aberto atualmente
   */
  currCase: Case;

  /**
   * GIF se estiver conectando ao rto
   */
  connectingGif: boolean = false;

  /**
   * Output do evento com o status da conexao rto para o componente statusbar
   */
  @Output() eventChange = new EventEmitter();

  constructor(
    public dialog: DialogService,
    private notybarService: NotybarService,
    private dataset: DatasetService,
    private caseDataset: CaseDatasetService,
  ) {
    if (this.dataset.currCaseId !== undefined && this.dataset.currCaseId !== null && this.dataset.hasById(this.dataset.currCaseId)) {
      this.currCase = this.dataset.getById(this.dataset.currCaseId);
    }

    this.caseDataset.$currCaseLoaded.subscribe((res) => {
      this.currCase = this.dataset.getById(res);
    });
  }

  ngOnInit() {
    this.toolbarScrolling();
  }

  toolbarScrolling() {
    setTimeout(() => {
      const el: any = document.getElementsByClassName('sest-toolbar-menu');
      const element: any = el.item(0);

      element.addEventListener('wheel', function (e) {
        if (e.deltaY > 0) {
          element.scrollLeft += 150;
        } else {
          element.scrollLeft -= 150;
        }
      });
    }, 100);
  }

  openDialogImportSestWeb() {
    this.dialog.openPageDialog(
      ImportComponent,
      { minHeight: 520, minWidth: 600 },
      { initialComponent: ImportSestWebComponent }
    );
  }

  openDialogImportExternalFile() {
    this.dialog.openPageDialog(
      ImportComponent,
      { minHeight: 520, minWidth: 600 },
      { initialComponent: SelectTypeComponent }
    );
  }

  openWindowSelectCorrelaction() {
    this.dialog.openPageDialog(
      SelectCorrelationComponent,
      { minHeight: 450, minWidth: 450 },
    );
  }

  openWindowFilter() {
    this.dialog.openPageDialog(
      WindowFilterComponent,
      { minHeight: 450, minWidth: 450 },
    );
  }

  openWindowCalculosPerfis() {
    this.dialog.openPageDialog(
      ProfileCalculationsComponent,
      { minHeight: 520, minWidth: 1024 },
    );
  }

  openWindowCalculosSobrecarga() {
    this.dialog.openPageDialog(
      OverloadCalculationComponent,
      { minHeight: 300, minWidth: 450 },
    );
  }

  openWindowCalculosMecanicos() {
    this.dialog.openPageDialog(
      MechanicalCalculationsComponent,
      { minHeight: 450, minWidth: 1024 },
    );
  }

  openWindowPoroPressure() {
    this.dialog.openPageDialog(
      PorePressureCalculationComponent,
      { minHeight: 400, minWidth: 450 },
    );
  }

  openWindowGradientes() {
    this.dialog.openPageDialog(
      GradientCalculationComponent,
      { minHeight: 400, minWidth: 450 },
    );
  }

  openWindowTensoesInsitu() {
    this.dialog.openPageDialog(
      TensionsInsituCalculationComponent,
      { minHeight: 100, minWidth: 450 },
    );
  }

  openWindowExport() {
    this.dialog.openPageDialog(
      ExportComponent,
      { minHeight: 300, minWidth: 450 },
    );
  }

  openWindowProfileComposition() {
    this.dialog.openPageDialog(
      WindowProfileCompositionComponent,
      { minHeight: 520, minWidth: 450 },
    );
  }

  openWindowExpoentD() {
    this.dialog.openPageDialog(
      ExponentDComponent,
      { minHeight: 300, minWidth: 450 },
    );
  }

  openWindowRtoConnection() {
    this.dialog.openPageDialog(
      RtoConnectionComponent,
      { minHeight: 300, minWidth: 450 },
    );
  }

  openWindowLinkData() {
    this.dialog.openPageDialog(
      LinkDataComponent,
      { minHeight: 300, minWidth: 1024 },
    );
  }

  openWindowRegistry() {
    this.dialog.openPageDialog(
      RecordsComponent,
      { minHeight: 300, minWidth: 1024 },
    );
  }

  openWindowEvents() {
    this.dialog.openPageDialog(
      EventsComponent,
      { minHeight: 300, minWidth: 1024 },
    );
  }

  salvar() {
    
  }

  /**
   * Função temporária de conexão RTO (ativa o gif conectando & emite um evento para o componente statusbar)
   */
  connectingRto(event) {
    event.preventDefault();
    this.connectingGif = true;
    this.eventChange.emit(event);
    setTimeout(() => {
      this.connectingGif = false;
      this.eventChange.emit(event);
      this.notybarService.show(
        'Não foi possível conectar ao servidor RTO. Verifique as configurações',
        'warning'
      );
    }, 8000);
  }


}
