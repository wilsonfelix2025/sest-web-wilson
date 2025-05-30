import { Injectable } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material';
import { DialogDeleteComponent } from '@components/explorer/dialog-delete/dialog-delete.component';
import { DialogDuplicateComponent } from '@components/explorer/dialog-duplicate/dialog-duplicate.component';
import { DialogCreateComponent } from '@components/explorer/dialog-create/dialog-create.component';
import { DialogCreateTabComponent } from '@components/editor/dialog-create-tab/dialog-create-tab.component';
import { DialogRenameComponent } from '@components/explorer/dialog-rename/dialog-rename.component';
import { DialogMoveComponent } from '@components/explorer/dialog-move/dialog-move.component';
import { DialogColumnPropertiesComponent } from '@components/editor/dialog-column-properties/dialog-column-properties.component';
import { E_TYPES } from '@utils/explorerTypesEnum';
import { DialogImageComponent } from '@components/editor/dialog-image/dialog-image.component';
import { WindowLegendComponent } from '@components/editor/window-legend/window-legend.component';

@Injectable({
  providedIn: 'root'
})

export class DialogService {
  readonly itemTypeMap = [
    {
      name: 'unit',
      label: 'unidade operacional'
    },
    {
      name: 'oilfield',
      label: 'campo',
      parent: 'unidade operacional'
    },
    {
      name: 'well',
      label: 'poço',
      parent: 'campo',
    },
    {
      name: 'file',
      label: 'caso',
      parent: 'poço'
    }
  ];

  readonly fileTypeMap = [
    {
      name: 'project',
      label: 'projeto',
    },
    {
      name: 'monitoring',
      label: 'monitoramento'
    },
    {
      name: 'retroanalysis',
      label: 'correlação',
    }
  ];

  constructor(public dialog: MatDialog) { }

  openDialog(popupComponent, data?: any) {
    const dialogConfig = new MatDialogConfig();
    if (data !== undefined) {
      if (data.minWidth !== undefined) {
        dialogConfig.minWidth = data.minWidth;
      }
      if (data.minHeight !== undefined) {
        dialogConfig.minHeight = data.minHeight;
      }
      dialogConfig.maxHeight = '100vh';
      if (data.hasBackdrop !== undefined) {
        dialogConfig.hasBackdrop = data.hasBackdrop;
      }
    }
    dialogConfig.disableClose = true;
    dialogConfig.autoFocus = true;
    dialogConfig.data = data || {};
    // dialogConfig.width = '300px'
    this.dialog.open(popupComponent, dialogConfig);
  }

  openDialogColumnProperties(column) {
    this.openDialog(DialogColumnPropertiesComponent, column);
  }

  openDialogCreateTabs() {
    this.openDialog(DialogCreateTabComponent);
  }

  // =============== PAGE DIALOGS =============== //

  openPageDialog(component, dialogSize: { minHeight?: number, minWidth?: number, maxHeight?: number, maxWidth?: number }, data?: any) {
    this.openDialog(component, {
      minHeight: dialogSize.minHeight ? dialogSize.minHeight + 'px' : undefined,
      minWidth: dialogSize.minWidth ? dialogSize.minWidth + 'px' : undefined,
      maxHeight: dialogSize.maxHeight ? dialogSize.maxHeight + 'px' : undefined,
      maxWidth: dialogSize.maxWidth ? dialogSize.maxWidth + 'px' : undefined,
      data: data
    });
  }

  // ============================================ //

  openDialogGeneric(componente, classe, operation, parameters) {
    this.openDialog(componente, { classe: classe, parameters: parameters, operation: operation });
  }

  openImageDialog(parameters: { title, imagePath }) {
    this.openDialog(DialogImageComponent, { hasBackdrop: false, parameters: parameters });
  }

  openLegendDialog(parameters) {
    this.openDialog(WindowLegendComponent, { hasBackdrop: false, data: parameters });
  }

  openDialogDuplicate(itemType: E_TYPES, item) {
    const data = this.assembleDialogBundle(itemType, item);
    this.openDialog(DialogDuplicateComponent, data);
  }

  openDialogMove(itemType: E_TYPES, item) {
    const data = this.assembleDialogBundle(itemType, item);
    this.openDialog(DialogMoveComponent, data);
  }

  openDialogRename(itemType: E_TYPES, item) {
    const data = this.assembleDialogBundle(itemType, item);
    this.openDialog(DialogRenameComponent, data);
  }

  openDialogDelete(itemType: E_TYPES, item) {
    const data = this.assembleDialogBundle(itemType, item);
    this.openDialog(DialogDeleteComponent, data);
  }

  openDialogCreate(itemType: E_TYPES, parentId: number, fileType?: number) {
    const data = this.assembleDialogBundle(
      itemType,
      null,
      fileType === undefined ? null : fileType,
      { parent: parentId }
    );
    this.openDialog(DialogCreateComponent, data);
  }

  assembleDialogBundle(itemType, item?, fileType?, additionalData?) {
    const data = {};
    data['itemType'] = itemType;
    data['itemTypeInfo'] = this.itemTypeMap[itemType];

    if (item) {
      data['item'] = item;
    }

    if (data['itemTypeInfo']['name'] === 'file') {
      data['fileType'] = fileType;
      data['fileTypeInfo'] = this.fileTypeMap[fileType];
    }

    for (const key in additionalData) {
      if (additionalData.hasOwnProperty(key)) {
        data[key] = additionalData[key];
      }
    }

    return data;
  }
}
