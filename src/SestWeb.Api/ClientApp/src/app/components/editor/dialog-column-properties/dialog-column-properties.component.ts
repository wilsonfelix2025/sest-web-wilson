import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { LISTA_MNEMÔNICOS, TIPOS_PERFIS } from '@utils/perfil/tipo-perfil';
import { LITHO_TYPES } from '@utils/litho-types-list';
import { importType } from '../import/shared/icon-formatter';
import { UNIDADE_DE_MEDIDA } from '@utils/perfil/unidade-de-medida';
import { DatasetService } from '@services/dataset/dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { LithologyDatasetService } from '@services/dataset/lithology-dataset.service';
import { Case } from 'app/repositories/models/case';

@Component({
  selector: 'app-dialog-column-properties',
  templateUrl: './dialog-column-properties.component.html',
  styleUrls: ['./dialog-column-properties.component.scss']
})
export class DialogColumnPropertiesComponent implements OnInit {

  currCase: Case;

  tableType: 'litho' | 'profile' | 'traj';

  importTypeSelected: importType;
  newDisabled: boolean = false;
  appendDisabled: boolean = false;
  overwriteDisabled: boolean = false;

  mnemonicList = LISTA_MNEMÔNICOS.sort(function (a, b) {
    const keyA = TIPOS_PERFIS[a].descrição, keyB = TIPOS_PERFIS[b].descrição;
    return keyA.localeCompare(keyB);
  });
  profilesTypes = TIPOS_PERFIS;
  units = UNIDADE_DE_MEDIDA;

  lithoTypes = LITHO_TYPES;

  /**
   * Tipos de litologia disponiveis para o tipo de caso atual
   */
  currCaseAvailableLithoTypes: string[];

  /**
   * Lista de perfis do caso atual
   */
  currCaseProfilesList: Perfil[] = [];

  /**
   * Lista de litologias do caso atual
   */
  currCaseLithoList: Litologia[] = [];

  /**
   * Lista de nomes já usados na tabela
   */
  usedNameInTable: string[];

  new: { name: Input, type: Input, unit: Input } = {
    name: { value: undefined, tooltip: '' },
    type: { value: undefined, tooltip: '' },
    unit: { value: undefined, tooltip: '' },
  };

  append: { obj: Input, unit: Input } = {
    obj: { value: undefined, tooltip: '' },
    unit: { value: undefined, tooltip: '' },
  };

  overwrite: { obj: Input, unit: Input, top: Input, bottom: Input, } = {
    obj: { value: undefined, tooltip: '' },
    unit: { value: undefined, tooltip: '' },
    top: { value: undefined, tooltip: '' },
    bottom: { value: undefined, tooltip: '' },
  };

  constructor(
    public dialogRef: MatDialogRef<DialogColumnPropertiesComponent>,
    @Inject(MAT_DIALOG_DATA) public data: {
      context, tableType: 'litho' | 'profile' | 'traj',
      importType: importType,
      top?: number, bottom?: number,
      position?: string
      usedNamesInTable?: string[], name?: string,
      typeDescription?: string, type?: string, unit?: string,
    },

    private dataset: DatasetService,

    private profileDataset: ProfileDatasetService,
    private lithologyDataset: LithologyDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    console.log('DIALOG COLUMN', this.data);

    this.tableType = this.data.tableType;

    this.importTypeSelected = this.data.importType;

    if (this.tableType === 'traj') {
      this.initTraj();
    } else if (this.tableType === 'litho') {
      this.initLitho();
    } else {
      this.initProfile();
    }
  }

  initTraj() {
    if (this.importTypeSelected === 'overwrite') {
      this.overwrite.top.value = this.data.top;
      this.overwrite.bottom.value = this.data.bottom;
    }

  }

  initLitho() {
    this.pegarListaDeLitologias();

    if (this.currCaseLithoList.length === 0) {
      this.appendDisabled = true;
      this.overwriteDisabled = true;
    }
    if (this.currCaseAvailableLithoTypes.length === 0) {
      this.newDisabled = true;
    }

    let currLitho: Litologia;
    if (this.data.type !== undefined) {
      currLitho = this.currCaseLithoList.find(el => el.type === this.data.type);
    } else if (this.data.typeDescription !== undefined) {
      currLitho = this.currCaseLithoList.find(el => el.type === this.data.typeDescription);
    }
    if (this.importTypeSelected === 'append') {
      this.append.obj.value = currLitho;
    } else if (this.importTypeSelected === 'overwrite') {
      this.overwrite.top.value = this.data.top;
      this.overwrite.bottom.value = this.data.bottom;
      this.overwrite.obj.value = currLitho;
    }

    if (this.data.type !== undefined) {
      this.new.type.value = this.data.type;
    } else if (this.data.typeDescription !== undefined) {
      this.new.type.value = this.data.typeDescription;
    } else {
      this.new.type.value = this.currCaseAvailableLithoTypes[0];
    }
  }

  initProfile() {
    this.pegarListaDePerfis();

    if (this.currCaseProfilesList.length === 0) {
      this.appendDisabled = true;
      this.overwriteDisabled = true;
    }

    this.usedNameInTable = this.data.usedNamesInTable;

    this.new.name.value = this.data.name;

    if (this.importTypeSelected !== 'new') {
      const currProfile = this.currCaseProfilesList.find(el => el.name === this.data.name);
      let type;
      if (this.importTypeSelected === 'append') {
        type = this.append;
      } else if (this.importTypeSelected === 'overwrite') {
        type = this.overwrite;

        type.top.value = this.data.top;
        type.bottom.value = this.data.bottom;
      }
      type.obj.value = currProfile;
      type.unit.value = this.profilesTypes[currProfile.mnemonic].grupoUnidade.unidadesDeMedida.find(el => el.símbolo === this.data.unit);
    }

    if (this.data.type !== undefined) {
      this.new.type.value = this.data.type;
    } else if (this.data.typeDescription !== undefined) {
      this.new.type.value = this.mnemonicList.find(el => this.profilesTypes[el].descrição === this.data.typeDescription);
    }

    if (this.new.type.value === '' || this.new.type.value === undefined || this.new.type.value === null) {
      this.new.type.value = 'GENERICO';
      this.new.unit.value = this.profilesTypes[this.new.type.value].unidadePadrão;
    } else {
      this.new.unit.value = this.profilesTypes[this.new.type.value].grupoUnidade.unidadesDeMedida.find(el => el.símbolo === this.data.unit);
    }
  }

  pegarListaDePerfis() {
    this.profileDataset.getAll(this.dataset.currCaseId).forEach(p => {
      const perfil: Perfil = {
        name: p.nome,
        mnemonic: p.mnemonico,
        description: p.descrição,
        unit: p.grupoDeUnidades.unidadePadrão.símbolo,
      };
      if (p.count > 0) {
        perfil.top = p.primeiroPonto.pm.valor;
        perfil.bottom = p.ultimoPonto.pm.valor;
      }
      this.currCaseProfilesList.push(perfil);
    });
  }

  pegarListaDeLitologias() {
    this.currCaseAvailableLithoTypes = LITHO_TYPES[this.currCase.tipoPoço];
    this.lithologyDataset.getAll(this.dataset.currCaseId).forEach(el => {
      if (el.pontos.length > 0) {
        this.currCaseAvailableLithoTypes = this.currCaseAvailableLithoTypes.filter(type => type !== el.classificação.nome);
        const litologia: Litologia = {
          type: el.classificação.nome,
          top: el.primeiroPonto.pm.valor,
          bottom: el.ultimoPonto.pm.valor,
        };
        this.currCaseLithoList.push(litologia);
      }
    });
  }

  changedObject(event, type) {
    console.log('CHANGED OBJECT', event, type);
    if (type.obj !== undefined) {
      type.unit.value = this.profilesTypes[type.obj.value.mnemonic].unidadePadrão;
    } else {
      type.unit.value = this.profilesTypes[type.type.value].unidadePadrão;
    }
  }

  displayType(profile: Perfil): string {
    return profile && profile.description ? profile.description : '';
  }

  displayPerfil(perfil: Perfil): string {
    return perfil && perfil.name ? perfil.name : '';
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  closePanel(mode: importType) {
    if (this.importTypeSelected === mode) {
      this.importTypeSelected = undefined;
    }
  }

  canSubmitProfile(): boolean {
    if (this.new.name.value === null || this.new.name.value === undefined || this.new.name.value === '') {
      this.new.name.tooltip = 'Precisa estar preenchido.';
      return false;
    }
    if (this.currCaseProfilesList.findIndex(el => el.name === this.new.name.value) > -1) {
      this.new.name.tooltip = 'Já existe um perfil com esse nome.';
      return false;
    }
    if (this.usedNameInTable.findIndex(el => el === this.new.name.value) > -1) {
      this.new.name.tooltip = 'Já existe outra perfil na tabela com esse nome.';
      return false;
    }
    if (this.new.type.value === null || this.new.type.value === undefined || this.new.type.value === '') {
      this.new.type.tooltip = 'Precsa estar preenchido';
      return false;
    }
    this.new.name.tooltip = '';
    this.new.type.tooltip = '';
    return true;
  }

  canSubmitLitho(): boolean {
    if (this.new.type.value === null || this.new.type.value === undefined || this.new.type.value === '') {
      this.new.type.tooltip = 'Precsa estar preenchido';
      return false;
    }
    if (this.currCaseLithoList.findIndex(el => el.type === this.new.type.value) > -1) {
      this.new.type.tooltip = 'Já existe essa litologia.';
      return false;
    }
    this.new.name.tooltip = '';
    this.new.type.tooltip = '';

    return true;
  }

  canSubmit(): boolean {
    if (this.importTypeSelected === undefined) {
      return false;
    }
    if (this.importTypeSelected === 'new') {
      if (this.tableType === 'profile') {
        return this.canSubmitProfile();
      } else if (this.tableType === 'litho') {
        return this.canSubmitLitho();
      }
    } else if (this.importTypeSelected === 'overwrite') {
      if (this.overwrite.top.value === NaN || this.overwrite.top.value === null) {
        this.overwrite.top.tooltip = 'Precisa estar preenchido.';
        return false;
      }
      if (this.overwrite.bottom.value === NaN || this.overwrite.bottom.value === null) {
        this.overwrite.bottom.tooltip = 'Precisa estar preenchido.';
        return false;
      }
      if (this.overwrite.bottom.value < this.overwrite.top.value) {
        this.overwrite.bottom.tooltip = this.overwrite.top.tooltip = 'Base deve ser maior que topo.';
        return false;
      }
      if (this.overwrite.obj.value !== null && this.overwrite.obj.value !== undefined) {
        if (this.overwrite.bottom.value > this.overwrite.obj.value.bottom) {
          this.overwrite.bottom.tooltip = 'Base deve ser menor ou igual ao atual.\nBase atual é ' + this.overwrite.obj.value.bottom;
          return false;
        }
        if (this.overwrite.top.value < this.overwrite.obj.value.top) {
          this.overwrite.top.tooltip = 'Topo deve ser maior ou igual ao atual.\nTopo atual é ' + this.overwrite.obj.value.top;
          return false;
        }
      }
      this.overwrite.top.tooltip = '';
      this.overwrite.bottom.tooltip = '';
    }
    return true;
  }

  submit() {
    const data: any = {
      table: this.data.tableType,
      importType: this.importTypeSelected,
      position: this.data.position,
    };

    if (this.tableType !== 'traj') {
      if (this.importTypeSelected === 'new') {
        data.type = this.new.type.value;
        if (this.tableType === 'profile') {
          data.name = this.new.name.value;
          data.typeDescription = this.profilesTypes[this.new.type.value].descrição;
          data.unit = this.new.unit.value.símbolo;
        } else {
          data.typeDescription = this.new.type.value;
        }
      } else if (this.importTypeSelected === 'append') {
        if (this.tableType === 'profile') {
          data.name = this.append.obj.value.name;
          data.type = this.append.obj.value.mnemonic;
          data.typeDescription = this.profilesTypes[this.append.obj.value.mnemonic].descrição;
          data.unit = this.append.unit.value.símbolo;
        } else {
          data.type = this.append.obj.value.type;
          data.typeDescription = this.append.obj.value.type;
        }
      } else if (this.importTypeSelected === 'overwrite') {
        data.top = this.overwrite.top.value;
        data.bottom = this.overwrite.bottom.value;
        if (this.tableType === 'profile') {
          data.name = this.overwrite.obj.value.name;
          data.type = this.overwrite.obj.value.mnemonic;
          data.typeDescription = this.profilesTypes[this.overwrite.obj.value.mnemonic].descrição;
          data.unit = this.overwrite.unit.value.símbolo;
        } else {
          data.type = this.overwrite.obj.value.type;
          data.typeDescription = this.overwrite.obj.value.type;
        }
      }
    } else if (this.importTypeSelected === 'overwrite') {
      data.top = this.overwrite.top.value;
      data.bottom = this.overwrite.bottom.value;
    }

    this.data.context.setProperties(data);
    this.dialogRef.close();
  }
}

interface Input {
  value: any;
  tooltip: string;
}

interface Perfil {
  name: string;
  mnemonic: string;
  description: string;
  unit: string;
  top?: number;
  bottom?: number;
}

interface Litologia {
  type: string;
  top?: number;
  bottom?: number;
}
