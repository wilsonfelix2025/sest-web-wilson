import { Component, OnInit, Input } from '@angular/core';
import { DialogService } from '@services/dialog.service';
import { ExplorerNavigatorService } from '@services/explorer-navigator.service';
import { E_TYPES } from '@utils/explorerTypesEnum';


@Component({
  selector: 'create-menu',
  templateUrl: './create-menu.component.html',
  styleUrls: ['./create-menu.component.scss']
})
export class CreateMenuComponent implements OnInit {

  @Input() depth: number;

  constructor(public dialog: DialogService, public nav: ExplorerNavigatorService) { }

  ngOnInit() {
  }

  openDialogCreateWell(parentId) {
    this.dialog.openDialogCreate(E_TYPES.Well, parentId);
  }

  openDialogCreateCase(type: number, wellId: number) {
    this.dialog.openDialogCreate(E_TYPES.Case, wellId, type);
  }

}
