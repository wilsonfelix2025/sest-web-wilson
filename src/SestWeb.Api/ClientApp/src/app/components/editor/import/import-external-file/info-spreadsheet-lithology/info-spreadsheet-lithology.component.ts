import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { ImportLitologiaData } from '@utils/interfaces';
import { DialogService } from '@services/dialog.service';
import { LITHO_TYPES } from '@utils/litho-types-list';
import { NotybarService } from '@services/notybar.service';
import { editButtonFormatter } from '../../shared/edit-button-formatter';
import { iconFormatter, importType, translateImportType } from '../../shared/icon-formatter';
import { getProperties } from '../../shared/get-properties';
import { Case } from 'app/repositories/models/case';
import { DatasetService } from '@services/dataset/dataset.service';
import { LithologyDatasetService } from '@services/dataset/lithology-dataset.service';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';
import { ImportService } from 'app/repositories/import.service';

@Component({
  selector: 'app-info-spreadsheet-lithology',
  templateUrl: './info-spreadsheet-lithology.component.html',
  styleUrls: ['./info-spreadsheet-lithology.component.scss']
})
export class InfoSpreadsheetLithologyComponent implements OnInit {

  @Output() someEvent = new EventEmitter<any>();
  /**
   * Os dados do poço aberto atualmente.
   */
  currCase: Case;

  fixedRowsTop = 2;

  rotaryTable: number = null;

  iconFormatter = iconFormatter;

  buttonFormatter = editButtonFormatter;

  tableData = [
    ['PM', '-'],
    ['m', '-'],
  ];
  defaultCellTypes = [
    { row: 0, col: 0, readOnly: true, type: 'text', },
    { row: 0, col: 1, readOnly: true, renderer: 'html', },
    { row: 1, col: 0, readOnly: true, type: 'text', },
    { row: 1, col: 1, readOnly: true, type: 'text', },
  ];

  colHeaders = ['Profundidade', this.columnHeaders('Litologia')];
  columns: any[] = [
    {
      type: 'numeric',
      numericFormat: { pattern: '0[.]0*' },
      // validator: function (value, callback) {
      //   if (this.row === this.instance.countRows() - 1) {
      //     callback(true);
      //     return;
      //   }
      //   const comment = this.instance.getPlugin('comments');
      //   let error = '';
      //   if (value !== null) {
      //     if (NumberUtils.isNumber(value)) {
      //       if (!(this.instance.getDataAtCol(this.col).filter(el => el === value).length > 1)) {
      //         comment.removeCommentAtCell(this.row, this.col);
      //         callback(true);
      //         return;
      //       } else {
      //         error = 'Profundidade Repetida';
      //       }
      //     } else {
      //       error = 'Precisa ser um número.';
      //     }
      //   } else {
      //     error = 'Não pode estar vazia.';
      //   }
      //   comment.setCommentAtCell(this.row, this.col, error);
      //   comment.updateCommentMeta(this.row, this.col, { readOnly: true });
      //   callback(false);
      // },
    },
    {},
  ];

  isValid = true;

  getLitologiaProperties(nameWithIcon, typeWithIcon, unit) {
    return getProperties(nameWithIcon, typeWithIcon, unit, 'litho');
  }

  constructor(
    public dialog: DialogService,
    public importService: ImportService,
    private notybarService: NotybarService,

    private dataset: DatasetService,
    private lithologyDataset: LithologyDatasetService,
    private caseDataset: CaseDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);
    let lithoTypes = LITHO_TYPES[this.currCase.tipoPoço];
    this.lithologyDataset.getAll(this.currCase.id).forEach(el => {
      if (el.pontos.length > 0) {
        lithoTypes = lithoTypes.filter(type => type !== el.classificação.nome);
      }
    });
    if (lithoTypes.length > 0) {
      this.tableData[0][1] = lithoTypes[0] + ' ' + this.iconFormatter('new');
    } else {
      this.tableData[0][1] = LITHO_TYPES[this.currCase.tipoPoço][0] + ' ' + this.iconFormatter('append');
    }
    this.someEvent.emit({ cangoNext: true });
    console.log(lithoTypes);
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

  onButtonChangeLitologiaClicked(obj: { position: number, name: string, context, table: Handsontable }) {
    const litologiaProperties = this.getLitologiaProperties(
      obj.table.getColHeader()[obj.position],
      obj.table.getDataAtCell(0, obj.position),
      obj.table.getDataAtCell(1, obj.position)
    );

    const data = {
      context: obj.context,
      tableType: litologiaProperties.table,
      importType: litologiaProperties.importType,
      top: litologiaProperties.top,
      bottom: litologiaProperties.bottom,
      position: obj.position,
      usedNamesInTable: [],
      // name: litologiaProperties.name,
      typeDescription: litologiaProperties.typeDescription,
      // unit: litologiaProperties.unit,
    };

    this.dialog.openDialogColumnProperties(data);
  }

  changeLitologia(data: {
    table: Handsontable,
    importType: importType,
    position: number,
    name: string, type: string, unit: string, top: number, bottom: number
  }) {
    // this.colHeaders[data.position] = this.columnHeaders(data.name);
    data.table.setDataAtCell(0, data.position, data.type + ' ' + this.iconFormatter(data.importType, data.top, data.bottom));
  }

  importData() {
    if (!this.isValid) {
      this.notybarService.show('Conserte os erros na tabela', 'danger');
      return;
    }
    const limites = this.caseDataset.getLimiteCaso(this.dataset.currCaseId);
    let foraLimite = false;

    const prop = this.getLitologiaProperties(this.colHeaders[1], this.tableData[0][1], this.tableData[1][1]);

    const litologiaData: ImportLitologiaData = {
      poçoId: this.currCase.id,
      litologia: {
        nome: prop.name,
        tipo: prop.typeDescription,
        ação: translateImportType[prop.importType],
        pontosLitologia: [],
        tipoProfundidade: this.tableData[0][0] as 'PM' | 'Cota',
        correçãoMesaRotativa: this.rotaryTable,
        valorBase: prop.bottom,
        valorTopo: prop.top,
      },
    };
    for (let i = 2; i < this.tableData.length - 1; i++) {
      if (this.tableData[i][0] !== null && this.tableData[i][1] !== null) {
        const ponto = {
          pm: Math.abs(parseFloat(this.tableData[i][0])),
          tipoRocha: this.tableData[i][1],
        }
        if (litologiaData.litologia.tipoProfundidade === 'PM') {
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
        litologiaData.litologia.pontosLitologia.push(ponto);
      }
    }
    // console.log('Importando', litologiaData, this.tableData);
    this.importService.importLitologia(litologiaData).then(response => {
      if (foraLimite) {
        this.notybarService.show('Foram detectadas profundidades fora do trecho de sedimentos', 'warning');
      }
      location.reload();
    });
  }
}
