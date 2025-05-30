import {
  Component,
  OnInit,
  Input,
  ViewChild,
  AfterViewInit,
  DoCheck,
} from '@angular/core';
import { DatasetService } from '@services/dataset/dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { NotybarService } from '@services/notybar.service';
import { NumberUtils } from '@utils/number';
import { TableOptions } from '@utils/table-options-default';
import { Case } from 'app/repositories/models/case';
import { Export } from 'app/repositories/models/export';

@Component({
  selector: 'sest-profiles-export',
  templateUrl: './profiles-export.component.html',
  styleUrls: ['./profiles-export.component.scss'],
})
export class ProfilesExportComponent implements OnInit, AfterViewInit, DoCheck {
  /**
   * O caso de estudo aberto atualmente.
   */

  @Input() currCase: Case;

  /**
   * Referência para tabela na tela.
   */
  @ViewChild('table', { static: true }) tableComponent;

  /**
   * Tabela EXPORT PROFILES.
   */
  table: Handsontable;

  /**
   * Se está marcado para selecionar todos os perfis.
   */
  selectedAll = false;

  /**
   * Titulos das colunas na tabela.
   */
  colHeaders = [
    `<input type='checkbox' ${this.selectedAll ? 'checked="checked"' : ''}>`,
    'Nome',
    'Unidade',
  ];

  /**
   * Tipos das colunas na tabela.
   */
  columnsTypes: any[] = [
    { data: 'selected', type: 'checkbox' },
    { data: 'name', readOnly: true },
    { data: 'unit', readOnly: true },
  ];

  /**
   * Opções da tebela.
   */
  optionsParameters: any = TableOptions.createDefault({
    afterOnCellMouseDown: (event, coords) => {
      if (event.target.type === 'checkbox') {
        // Se clicou no checkbox do título, marca ou desmarca todos as linhas da tabela.
        if (coords.row < 0) {
          this.selectedAll = !event.target.checked;

          this.colHeaders[0] = `<input type='checkbox' ${this.selectedAll ? 'checked="checked"' : ''
            }>`;

          for (let i = 0; i < this.table.countRows(); i += 1) {
            this.table.setDataAtCell(i, 0, this.selectedAll as any);
          }
        }
      }
    },
    width: 350,
    height: 410,
    rowHeaderWidth: 30,
    autoColumnSize: true,
    filters: false,
    disableVisualSelection: true,
    manualColumnResize: [30, , 75],
  });

  /**
   * dataset da tabela
   */
  tableData: { selected: boolean; name: string; unit: string; id: string }[] = [];

  /**
   * Linhas selecionadas.
   */
  lineSelected = [];

  /**
   * Opções de exportação
   */
  exportTrajectory: boolean = false;
  exportLitology: boolean = false;

  pmTopo: number = 250;
  pmBase: number = 5000;

  includePv: boolean = false;
  includeCota: boolean = false;

  /**
   * Arquivos disponíveis para exportação
   */
  files: { text: string; value: 'CSV' | 'LAS' }[] = [
    { text: 'Tabela CSV', value: 'CSV' },
    { text: 'Arquivo LAS', value: 'LAS' },
  ];

  fileSelected: { text: string; value: 'CSV' | 'LAS' } = this.files[0];

  /**
   * Parâmetros de exportação
   */

  exportParameters: {
    title: string;
    unit: string;
    value;
    tooltip: string;
  }[] = [
      { title: 'PM topo', unit: 'm', value: undefined, tooltip: '' },
      { title: 'PM base', unit: 'm', value: undefined, tooltip: '' },
      { title: 'Intervalo', unit: 'm', value: 1, tooltip: '' },
    ];

  isValid: boolean = true;

  constructor(
    private dataset: DatasetService,
    private profileDataset: ProfileDatasetService,
    public notybarService: NotybarService
  ) { }

  ngOnInit() {
    if (this.currCase.dadosGerais.geometria.categoriaPoço === 'OnShore') {
      this.pmTopo = this.exportParameters[p.Top].value =
        this.currCase.dadosGerais.geometria.mesaRotativa +
        this.currCase.dadosGerais.geometria.onShore.alturaDeAntePoço;
    } else {
      this.pmTopo = this.exportParameters[p.Top].value =
        this.currCase.dadosGerais.geometria.mesaRotativa +
        this.currCase.dadosGerais.geometria.offShore.laminaDagua;
    }
    this.pmBase = this.exportParameters[
      p.Base
    ].value = this.currCase.trajetória.últimoPonto.pm.valor;
    this.exportParameters[p.Interval].value = 1;

    const perfis = this.profileDataset.getAll(this.dataset.currCaseId);
    this.tableData = perfis.map((el) => {
      return {
        selected: false,
        name: el.name,
        unit: el.grupoDeUnidades.unidadePadrão.símbolo,
        id: el.id,
      };
    });
  }

  ngAfterViewInit() {
    this.table = this.tableComponent.getHandsontableInstance();
  }

  ngDoCheck() {
    this.isValid = this.canSubmit();
  }

  canSubmit() {
    let isValid = true;
    this.exportParameters.forEach((parameter) => {
      parameter.tooltip = '';
      if (
        parameter.value === null ||
        parameter.value === undefined ||
        parameter.value === ''
      ) {
        parameter.tooltip = 'Precisa estar preenchido.';
        isValid = false;
      } else if (!NumberUtils.isNumber(parameter.value)) {
        parameter.tooltip = 'Precisa ser número';
        isValid = false;
      }
    });
    if (
      this.exportParameters[p.Top].tooltip === '' &&
      this.exportParameters[p.Base].tooltip === ''
    ) {
      if (
        parseFloat(this.exportParameters[p.Top].value) >
        parseFloat(this.exportParameters[p.Base].value)
      ) {
        this.exportParameters[p.Top].tooltip = this.exportParameters[
          p.Base
        ].tooltip = 'Topo precisa ser menor que base.';
        isValid = false;
      }
      if (parseFloat(this.exportParameters[p.Top].value) < this.pmTopo) {
        this.exportParameters[p.Top].tooltip =
          'Não pode ser menor que inicio dos sedimentos.';
        isValid = false;
      }
      if (parseFloat(this.exportParameters[p.Base].value) > this.pmBase) {
        this.exportParameters[p.Base].tooltip =
          'Não pode ser maior que profundidade final.';
        isValid = false;
      }
    }
    if (this.exportParameters[p.Interval].tooltip === '') {
      if (parseFloat(this.exportParameters[p.Interval].value) < 1) {
        this.exportParameters[p.Interval].tooltip =
          'O intervalo mínimo é 1 metro.';
        isValid = false;
      }
    }

    return isValid;
  }

  getOptions() {
    if (!this.isValid) {
      this.notybarService.show('Conserte os erros', 'warning');
      return;
    }
    const e: Export = {
      idPoço: this.currCase.id,
      perfis: this.tableData.filter((el) => el.selected).map((el) => el.id),
      pmTopo: parseFloat(this.exportParameters[p.Top].value),
      pmBase: parseFloat(this.exportParameters[p.Base].value),
      intervalo: parseFloat(this.exportParameters[p.Interval].value),
      trajetoria: this.exportTrajectory,
      litologia: this.exportLitology,
      pv: this.includePv,
      cota: this.includeCota,
      arquivo: this.fileSelected.value,
    };

    console.log('Export', e);

    if (e.perfis.length < 1 && !e.trajetoria && !e.litologia) {
      this.notybarService.show(
        'Nada foi selecionado para exportar...',
        'warning'
      );
      return;
    }

    return e;
  }
}
enum p {
  Top,
  Base,
  Interval,
}
