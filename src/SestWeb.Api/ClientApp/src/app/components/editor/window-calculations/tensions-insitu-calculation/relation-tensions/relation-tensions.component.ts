import { AfterViewInit, Component, DoCheck, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { DatasetService } from '@services/dataset/dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { DialogService } from '@services/dialog.service';
import { NotybarService } from '@services/notybar.service';
import { NumberUtils } from '@utils/number';
import { TableOptions } from '@utils/table-options-default';
import { Case } from 'app/repositories/models/case';
import { CriarPerfilRET, Perfil } from 'app/repositories/models/perfil';
import { ProfileService } from 'app/repositories/profile.service';

@Component({
  selector: 'app-relation-tensions',
  templateUrl: './relation-tensions.component.html',
  styleUrls: ['./relation-tensions.component.scss']
})
export class RelationTensionsComponent implements OnInit, DoCheck, AfterViewInit {

  /**
   * O caso de estudo aberto atualmente
   */
  currCase: Case;


  /**
  * dataset
  */
  tableData: any[] = [];

  /**
  * Tabela
  */
  hotTable: Handsontable;

  /**
   * Titulos das colunas na tabela
   */
  colHeaders = [
    'PM Topo (m)',
    'PM Base (m)',
    'Valor',
  ];

  /**
  * Tipos das colunas na tabela
  */
  columnsTypes: any[] = [
    { data: 'pmTopo' },
    { data: 'pmBase' },
    { data: 'valor' },
  ];

  @ViewChild('table', { static: true }) tableComponent;
  table: Handsontable;

  /**
  * Opções da tabela
  */
  options: any = TableOptions.createDefault({
    height: 280,
    width: 320,
    minSpareRows: 1,
    rowHeaderWidth: 10,
    manualColumnResize: [],
    filters: false,
  });

  nomePerfil = { value: '', tooltip: '' };

  isValid: boolean = true;

  currProfilesNames = [];

  dadosPoco = {
    pmMax: 0,
    pmSup: 0,
  };

  loading: boolean = false;

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { data: { perfis: Perfil[] } },
    public dialog: DialogService,
    public notybarService: NotybarService,

    private profileService: ProfileService,

    private dataset: DatasetService,
    private profileDataset: ProfileDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    this.data.data.perfis.forEach(profile => {
      this.currProfilesNames.push(profile.nome);
    });
    this.dadosPoco = {
      pmMax: this.currCase.trajetória.últimoPonto.pm.valor,
      pmSup: this.currCase.trajetória.primeiroPonto.pm.valor,
    };
    if (this.currCase.dadosGerais.geometria.categoriaPoço === 'OffShore') {
      this.dadosPoco.pmSup += this.currCase.dadosGerais.geometria.mesaRotativa;
      this.dadosPoco.pmSup += this.currCase.dadosGerais.geometria.offShore.laminaDagua;
    } else {
      this.dadosPoco.pmSup = this.currCase.dadosGerais.geometria.mesaRotativa;
      this.dadosPoco.pmSup += this.currCase.dadosGerais.geometria.onShore.alturaDeAntePoço;
    }
  }

  ngAfterViewInit() {
    this.table = this.tableComponent.getHandsontableInstance();
  }

  ngDoCheck() {
    this.isValid = this.canSubmit();
  }

  canSubmit() {
    let isValid = true;

    if (this.nomePerfil.value === undefined || this.nomePerfil.value === null || this.nomePerfil.value === '') {
      this.nomePerfil.tooltip = 'Precisa estar preenchido';
      isValid = false;
    } else if (this.currProfilesNames.includes(this.nomePerfil.value)) {
      this.nomePerfil.tooltip = 'Nome de perfil em uso';
      isValid = false;
    } else {
      this.nomePerfil.tooltip = '';
    }

    return isValid;
  }

  submit() {
    this.loading = true;
    const cols = this.columnsTypes.map(c => c.data);
    const verificador = this.tableData.filter(el => {
      return cols.some(col => el[col] !== undefined && el[col] !== null && el[col] !== '');
    });
    const valores = this.tableData.filter(el => {
      return cols.every(col => el[col] !== undefined && el[col] !== null && el[col] !== '');
    });
    if (verificador.length === 0) {
      this.notybarService.show('Tabela precisa ter valores.', 'warning');
      return;
    }
    if (valores.length !== verificador.length) {
      this.notybarService.show('Tabela precisa estar completa.', 'warning');
      return;
    }
    if (!this.validateTable(valores)) {
      this.notybarService.show('Tabela precisa estar válida.', 'warning');
      return;
    }

    const profile: CriarPerfilRET = {
      nomePerfil: this.nomePerfil.value,
      idPoço: this.currCase.id,
      valores: valores
    }

    this.profileService.criarPerfilRET(profile).then(el => {
      console.log('EL', el);
      this.profileDataset.add(el.perfil, this.dataset.currCaseId);

      this.loading = false;
      this.closeModal();
    }).catch(() => { this.loading = false; });
  }

  /**
   * Fechar diálogo
   */
  closeModal(): void {
    this.dialogRef.close();
  }

  removeError(row, prop) {
    const comment = this.table.getPlugin('comments');
    const col = this.table.propToCol(prop)

    comment.removeCommentAtCell(row, col);
    this.table.setCellMetaObject(row, col, { valid: true });
  }

  setError(row, prop, error) {
    const comment = this.table.getPlugin('comments');
    const col = this.table.propToCol(prop)

    comment.setCommentAtCell(row, col, error);
    comment.updateCommentMeta(row, col, { readOnly: true });
    this.table.setCellMetaObject(row, col, { valid: false });
  }

  validateTable(valores) {
    let valid = true;
    for (let i = 0; i < valores.length; i++) {
      this.removeError(i, 'pmTopo');
      this.removeError(i, 'pmBase');
      let error = '';
      if (valores[i].pmTopo === undefined || valores[i].pmTopo === null || valores[i].pmTopo === '') {
        error = 'Precisa estar preenchido';
        this.setError(i, 'pmTopo', error);
      } else if (!NumberUtils.isNumber(valores[i].pmTopo)) {
        error = 'Precisa ser um número';
        this.setError(i, 'pmTopo', error);
      } else if (valores[i].pmTopo < this.dadosPoco.pmSup) {
        error = 'PM Topo não pode ser menor que PM Sup';
        this.setError(i, 'pmTopo', error);
      }
      if (valores[i].pmBase === undefined || valores[i].pmBase === null || valores[i].pmBase === '') {
        error = 'Precisa estar preenchido';
        this.setError(i, 'pmBase', error);
      } else if (!NumberUtils.isNumber(valores[i].pmBase)) {
        error = 'Precisa ser um número';
        this.setError(i, 'pmBase', error);
      } else if (valores[i].pmBase > this.dadosPoco.pmMax) {
        error = 'PM Base não pode ser maior que PM Max';
        this.setError(i, 'pmBase', error);
      }
      if (error === '' && Number(valores[i].pmTopo) >= Number(valores[i].pmBase)) {
        error = 'PM Topo deve ser menor que PM Base';
        this.setError(i, 'pmTopo', error);
        this.setError(i, 'pmBase', error);
      }

      if (error !== '') {
        valid = false;
      }
    }
    this.table.render();

    return valid;
  }

}
