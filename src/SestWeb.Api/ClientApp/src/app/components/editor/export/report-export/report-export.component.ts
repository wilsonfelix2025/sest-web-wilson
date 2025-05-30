import { Component, Input, OnInit } from '@angular/core';
import { DatasetService } from '@services/dataset/dataset.service';
import { StateService } from '@services/dataset/state/state.service';
import { UNSET } from '@utils/vazio';
import { ExportService } from 'app/repositories/export.service';
import { Case } from 'app/repositories/models/case';
import { Relatorio, RelatorioRequest } from 'app/repositories/models/export';
import { Tab } from 'app/repositories/models/state';

@Component({
  selector: 'sest-report-export',
  templateUrl: './report-export.component.html',
  styleUrls: ['./report-export.component.scss']
})
export class ReportExportComponent implements OnInit {

  /**
 * @title Radios with ngModel
 */

  /**
   * O caso de estudo aberto atualmente.
   */
  @Input() currCase: Case;

  /**
   * Lista de abas
   */
  abas: Tab[] = [];
  abaSelected: Tab;

  /**
   * Arquivos disponíveis para exportação
   */
  files: { text: string; value: 'JPEG' | 'PDF' }[] = [
    { text: 'Arquivo JPG', value: 'JPEG' },
    { text: 'Arquivo PDF', value: 'PDF' },
  ];

  fileSelected: { text: string; value: 'JPEG' | 'PDF' } = this.files[0];

  isValid: boolean = true;
  pv: boolean = false;

  dadosBasicos = [
    {
      name: 'Nome do poço',
      checked: true,
      back: 'nome',
    }, {
      name: 'Tipo de poço',
      checked: true,
      back: 'tipo',
    }, {
      name: 'Profundidade final',
      checked: true,
      back: 'profundidadeFinal',
    }, {
      name: 'Altura de mesa rotativa',
      checked: true,
      back: 'alturaMR',
    }, {
      name: 'Espessura da LDA',
      checked: true,
      back: 'lda',
    },
  ];

  inclusionList = [
    {
      name: 'Trajetória',
      checked: true,
      enabled: true,
    }, {
      name: 'Litologia',
      checked: true,
      enabled: true,
    }, {
      name: 'Estratigrafia',
      checked: true,
      enabled: true,
    }, {
      name: 'Sapatas',
      checked: true,
      enabled: true,
      traject: true,
    }, {
      name: 'Objetivos',
      checked: true,
      enabled: true,
      traject: true,
    },
  ];


  constructor(
    private stateService: StateService,
    private datasetService: DatasetService,
    private exportService: ExportService,
  ) { }

  ngOnInit() {
    const ids = this.stateService.getTabsIds(this.currCase.id);
    this.abas = ids.map(el => this.datasetService.getById(el));

    this.abaSelected = this.abas[0];
    if (this.currCase.dadosGerais.geometria.categoriaPoço === "OnShore") {
      this.dadosBasicos.pop();
    }
    if (UNSET(this.currCase.trajetória) || UNSET(this.currCase.trajetória.count) || this.currCase.trajetória.count === 0) {
      this.inclusionList[0].enabled = false;
      this.inclusionList[0].checked = false;
    }
    if (UNSET(this.currCase.litologias) || this.currCase.litologias.length === 0) {
      this.inclusionList[1].enabled = false;
      this.inclusionList[1].checked = false;
    }
    if (UNSET(this.currCase.estratigrafia.Itens) || Object.keys(this.currCase.estratigrafia.Itens).length === 0) {
      this.inclusionList[2].enabled = false;
      this.inclusionList[2].checked = false;
    }
    if (UNSET(this.currCase.sapatas) || this.currCase.sapatas.length === 0) {
      this.inclusionList[3].enabled = false;
      this.inclusionList[3].checked = false;
    }
    if (UNSET(this.currCase.objetivos) || this.currCase.objetivos.length === 0) {
      this.inclusionList[4].enabled = false;
      this.inclusionList[4].checked = false;
    }
  }

  trajectChecked() {
    return this.inclusionList[0].checked;
  }

  generateReport() {
    const request: RelatorioRequest = {
      trajetoria: this.inclusionList[0].checked,
      litologia: this.inclusionList[1].checked,
      estratigrafias: this.inclusionList[2].checked,
      graficos: this.abaSelected.data.map(el => el.id),
    }
    this.exportService.relatorio = {
      idPoço: this.currCase.id,
      graficos: [],
      estratigrafias: [],
      extensao: this.fileSelected.value,
    }
    this.dadosBasicos.forEach(el => {
      this.exportService.relatorio[el.back] = el.checked;
    });

    this.exportService.$relatorioRequestedChart.next(request);
    // console.log(this.exportService.relatorio);
  }

}
