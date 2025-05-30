import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { DialogService } from '@services/dialog.service';
import { ImportPerfilData } from '@utils/interfaces';
import { NotybarService } from '@services/notybar.service';
import { editButtonFormatter } from '../../shared/edit-button-formatter';
import { iconFormatter, importType, translateImportType } from '../../shared/icon-formatter';
import { getProperties } from '../../shared/get-properties';
import { TIPOS_PERFIS, LISTA_MNEMÔNICOS } from '@utils/perfil/tipo-perfil';
import { Case } from 'app/repositories/models/case';
import { NumberUtils } from '@utils/number';
import { DatasetService } from '@services/dataset/dataset.service';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';
import { UNSET } from '@utils/vazio';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { ImportService } from 'app/repositories/import.service';

@Component({
  selector: 'app-info-spreadsheet-profile',
  templateUrl: './info-spreadsheet-profile.component.html',
  styleUrls: ['./info-spreadsheet-profile.component.scss']
})
export class InfoSpreadsheetProfileComponent implements OnInit {

  @Input() data;
  @Output() someEvent = new EventEmitter<any>();

  profilesTypesList = TIPOS_PERFIS;
  mnemonicList = LISTA_MNEMÔNICOS;
  /**
   * Os dados do poço aberto atualmente.
   */
  currCase: Case;

  fixedColumnsLeft = 1;
  fixedRowsTop = 2;

  rotaryTable: number = null;

  iconFormatter = iconFormatter;

  buttonFormatter = editButtonFormatter;
  contextMenuOptions = {
    'col_left': {
      disabled: function () {
        return this.getSelectedLast()[1] < 1;
      }
    },
    'col_right': {},
    'remove_col': {
      disabled: function () {
        return this.getSelectedLast()[1] < 1;
      }
    },
    'sep2': { name: '---------' },
  };

  tableData = [
    ['PM', `- ${this.iconFormatter('new')}`],
    ['m', '-'],
  ];
  defaultCellTypes = [
    { row: 0, col: 0, readOnly: true, type: 'text', },
    { row: 0, col: 1, readOnly: true, type: 'text', renderer: 'html', },
    { row: 1, col: 0, readOnly: true, type: 'text', },
    { row: 1, col: 1, readOnly: true, type: 'text', },
  ];
  colHeaders = ['Profundidade', this.columnHeaders('Perfil 1')];
  tableType = 'numeric';
  numericFormat = { pattern: '0[.]0*' };

  isValid = true;

  tableValidator = function (value, callback) {
    if (this.row === this.instance.countRows() - 1 || this.row < 2 || this.col === this.instance.countCols() - 1) {
      callback(true);
      return;
    }
    const comment = this.instance.getPlugin('comments');
    let error = '';
    if (value !== null) {
      if (NumberUtils.isNumber(value)) {
        if (this.col !== 0 || !(this.instance.getDataAtCol(this.col).filter(el => el === value).length > 1)) {
          comment.removeCommentAtCell(this.row, this.col);
          callback(true);
          return;
        } else {
          error = 'Profundidade Repetida';
        }
      } else {
        error = 'Precisa ser um número.';
      }
    } else {
      error = 'Não pode estar vazia.';
    }
    comment.setCommentAtCell(this.row, this.col, error);
    comment.updateCommentMeta(this.row, this.col, { readOnly: true });
    callback(false);
  };

  getProfileProperties(nameWithIcon, typeWithIcon, unit) {
    return getProperties(nameWithIcon, typeWithIcon, unit, 'profile');
  }

  constructor(
    public dialog: DialogService,
    public importService: ImportService,
    private notybarService: NotybarService,

    private dataset: DatasetService,
    private caseDataset: CaseDatasetService,
    private profileDataset: ProfileDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);
    this.someEvent.emit({ cangoNext: true });
  }

  afterCreateCol(obj: { change, table: Handsontable }) {
    this.colHeaders[obj.change[0]] = this.columnHeaders(`Perfil ${obj.change[0]}`);
    obj.table.setCellMetaObject(0, obj.change[0], { type: 'text', renderer: 'html', readOnly: true });
    obj.table.setCellMetaObject(1, obj.change[0], { type: 'text', readOnly: true });
    obj.table.setDataAtCell(0, obj.change[0], `- ${this.iconFormatter('new')}`);
    obj.table.setDataAtCell(1, obj.change[0], '-');
  }

  afterChangeTable(obj: { change, table: Handsontable }) {
    if (obj.change !== undefined && obj.change[0] !== null) {
      const mostDeepChange = obj.change[0][obj.change[0].length - 1];
      if (obj.change && obj.change[0] && mostDeepChange[0] >= 2 && mostDeepChange[1] >= this.tableData[0].length - 1) {
        this.afterCreateCol({ change: [this.tableData[0].length], table: obj.table });
      }
    }
  }

  getNewProfilesNames(table: Handsontable) {
    const namesWithIcons = table.getColHeader() as string[];
    const profilesNames = namesWithIcons.map(function (nameWithIcon) {
      const idRegex = /id='(.+?)'/;
      const name = nameWithIcon.match(idRegex);
      if (name !== null && name[1] !== undefined) {
        return name[1];
      }
      return nameWithIcon;
    });

    return profilesNames;
  }

  onButtonChangeProfileClicked(obj: { position: number, name: string, context, table: Handsontable }) {
    const profileProperties = this.getProfileProperties(
      obj.table.getColHeader()[obj.position],
      obj.table.getDataAtCell(0, obj.position),
      obj.table.getDataAtCell(1, obj.position)
    );
    const profilesNames = this.getNewProfilesNames(obj.table);

    const data = {
      context: obj.context,
      tableType: profileProperties.table,
      importType: profileProperties.importType,
      top: profileProperties.top,
      bottom: profileProperties.bottom,
      position: obj.position,
      usedNamesInTable: profilesNames.filter(el => el !== profileProperties.name),
      name: profileProperties.name,
      typeDescription: profileProperties.typeDescription,
      unit: profileProperties.unit,
    };

    this.dialog.openDialogColumnProperties(data);
  }

  changeProfile(data: {
    table: Handsontable,
    importType: importType,
    position: number,
    name: string, typeDescription: string, unit: string, top: number, bottom: number
  }) {
    this.colHeaders[data.position] = this.columnHeaders(data.name);
    data.table.setDataAtCell(0, data.position, data.typeDescription + ' ' + this.iconFormatter(data.importType, data.top, data.bottom));
    data.table.setDataAtCell(1, data.position, data.unit);
  }

  columnHeaders(text) {
    return `${text} ${this.buttonFormatter(text)}`;
  }

  rowHeaders(index) {
    if (index === 0) {
      return 'Tipo';
    }
    if (index === 1) {
      return 'Unidade';
    }
    return index - 1;
  }

  importData() {
    if (!this.isValid) {
      this.notybarService.show('Conserte os erros na tabela', 'danger');
      return;
    }
    const limites = this.caseDataset.getLimiteCaso(this.dataset.currCaseId);
    let foraLimite = false;

    const perfilData: ImportPerfilData = {
      poçoId: this.currCase.id,
      perfis: []
    };

    for (let i = 1; i < this.colHeaders.length - 1; i++) {
      const prop = this.getProfileProperties(this.colHeaders[i], this.tableData[0][i], this.tableData[1][i]);
      const tipo = this.mnemonicList.find(el => this.profilesTypesList[el].descrição === prop.typeDescription);

      const profile: any = {
        nome: prop.name,
        ação: translateImportType[prop.importType],
        pontosPerfil: [],
        tipoProfundidade: this.tableData[0][0] as 'PM' | 'Cota',
        tipo: tipo !== undefined ? tipo : 'GENERICO',
        unidade: prop.unit,
        correçãoMesaRotativa: this.rotaryTable,
        valorBase: prop.bottom,
        valorTopo: prop.top,
      };
      for (let f = 2; f < this.tableData.length - 1; f++) {
        if (this.tableData[f][0] !== null && this.tableData[f][i] !== null) {
          const ponto = {
            pm: Math.abs(parseFloat(this.tableData[f][0])),
            valor: this.tableData[f][i]
          }
          if (profile.tipoProfundidade === 'PM') {
            if (ponto.pm < limites.pmSup || ponto.pm > limites.pmMax) {
              foraLimite = true;
              // this.notybarService.show('Foram detectadas profundidades fora do trecho de sedimentos', 'warning');
            }
          } else {
            if (ponto.pm < -limites.cotaSup || ponto.pm > -limites.cotaMax) {
              foraLimite = true;
              // this.notybarService.show('Foram detectadas profundidades fora do trecho de sedimentos', 'warning');
            }
            ponto.pm = -ponto.pm;
          }
          profile.pontosPerfil.push(ponto);
        }
      }
      perfilData.perfis.push(profile);
    }
    // console.log('Importando', perfilData, this.tableData);
    this.importService.importPerfil(perfilData).then(res => {
      if (foraLimite) {
        this.notybarService.show('Foram detectadas profundidades fora do trecho de sedimentos', 'warning');
      }

      if (!UNSET(res.perfis)) {
        res.perfis.forEach(perfil => {
          this.profileDataset.add(perfil, this.dataset.currCaseId);
        });
      }
    });
  }
}
