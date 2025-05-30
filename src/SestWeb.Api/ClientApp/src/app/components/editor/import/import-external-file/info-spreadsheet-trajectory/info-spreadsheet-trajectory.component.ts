import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { NotybarService } from '@services/notybar.service';
import { editButtonFormatter } from '@utils/edit-button-formatter';
import { DialogService } from '@services/dialog.service';
import { getProperties } from '@utils/get-properties';
import { importType, iconFormatter, translateImportType } from '../../shared/icon-formatter';
import { Case } from 'app/repositories/models/case';
import { DatasetService } from '@services/dataset/dataset.service';
import { ImportService } from 'app/repositories/import.service';

@Component({
  selector: 'app-info-spreadsheet-trajectory',
  templateUrl: './info-spreadsheet-trajectory.component.html',
  styleUrls: ['./info-spreadsheet-trajectory.component.scss'],
})
export class InfoSpreadsheetTrajectoryComponent implements OnInit {
  @Input() data;
  @Output() someEvent = new EventEmitter<any>();

  /**
   * Os dados do poço aberto atualmente.
   */
  currCase: Case;

  iconFormatter = iconFormatter;

  fixedRowsTop = 1;

  currImportType: importType = 'new';
  overwriteBottom: number;
  overwriteTop: number;
  tableData = [['m', 'graus', 'graus']];
  colHeaders = [`Profundidade`, `Inclinação`, 'Azimute'];
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
      //       if (Number(value) >= 0) {
      //         comment.removeCommentAtCell(this.row, this.col);
      //         callback(true);
      //         return;
      //       } else {
      //         error = 'Inclinação não pode ser negativa.';
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
      //       if (Number(value) >= 0 && Number(value) <= 360) {
      //         comment.removeCommentAtCell(this.row, this.col);
      //         callback(true);
      //         return;
      //       } else {
      //         error = 'Azimute tem que estar entre 0 e 360º.';
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
  ];

  isValid = true;

  constructor(
    public dialog: DialogService,
    public importService: ImportService,
    private notybarService: NotybarService,

    private dataset: DatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);
    this.someEvent.emit({ cangoNext: true });
  }

  rowHeaders(index) {
    if (index === 0) {
      return 'Unidade';
    }
    return index;
  }

  importData() {
    if (!this.isValid) {
      this.notybarService.show('Conserte os erros na tabela', 'danger');
      return;
    }
    const trajectoryData: any = {
      poçoId: this.currCase.id,
      pontosTrajetória: [],
      ação: translateImportType[this.currImportType],
    };
    if (this.currImportType === 'overwrite') {
      trajectoryData.valorTopo = this.overwriteTop;
      trajectoryData.valorBase = this.overwriteBottom;
    }
    for (let i = 1; i < this.tableData.length - 1; i++) {
      const element = this.tableData[i];
      if (element[0] !== null || element[1] !== null || element[2] !== null) {
        trajectoryData.pontosTrajetória.push({
          pm: Number(element[0]),
          inclinacao: Number(element[1]),
          azimute: Number(element[2]),
        });
      }
    }
    this.importService.importTrajetoria(trajectoryData).then((response) => {
      location.reload();
    });
  }

  changeTrajetoria(data: {
    table: Handsontable;
    importType: importType;
    position: number;
    name: string;
    type: string;
    unit: string;
    top: number;
    bottom: number;
  }) {
    this.currImportType = data.importType;
    this.overwriteTop = data.top;
    this.overwriteBottom = data.bottom;
    if (this.currImportType === 'new') {
      this.colHeaders[0] = `Profundidade ${editButtonFormatter('Profundidade')} ${this.iconFormatter(
        'new',
        undefined,
        undefined,
        'Criar novo / Substituir'
      )}`;
    } else {
      this.colHeaders[0] = `Profundidade ${editButtonFormatter('Profundidade')} ${this.iconFormatter(
        this.currImportType
      )}`;
    }
    data.table.setDataAtCell(0, 0, 'm');
  }

  getTrajetoriaProperties(nameWithIcon, typeWithIcon, unit) {
    return getProperties(nameWithIcon, typeWithIcon, unit, 'traj');
  }

  onButtonChangeClicked(obj: { position: number; name: string; context; table: Handsontable }) {
    const data = {
      context: obj.context,
      tableType: 'traj',
      importType: this.currImportType,
      top: this.overwriteTop,
      bottom: this.overwriteBottom,
    };
    this.dialog.openDialogColumnProperties(data);
  }
}
