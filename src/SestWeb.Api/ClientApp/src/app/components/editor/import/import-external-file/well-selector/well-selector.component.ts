import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FileData } from '@utils/interfaces';
import { FileInfoComponent } from '../../shared/file-info/file-info.component';

@Component({
  selector: 'app-well-selector',
  templateUrl: './well-selector.component.html',
  styleUrls: ['./well-selector.component.scss'],
})
export class WellSelectorComponent implements OnInit {
  constructor() {}

  @Output() someEvent = new EventEmitter<any>();

  @Input() poços = [];

  @Input() data: { fileData: FileData; otherData? };

  forwardData = {};

  wells = [
    { internalType: 'Projeto', type: 'Projeto', entries: [] },
    { internalType: 'Acompanhamento', type: 'Monitoramento', entries: [] },
    { internalType: 'Retroanalise', type: 'Retroanalise', entries: [] },
  ];

  ngOnInit() {
    console.log(this.data);
    this.someEvent.emit({ cangoNext: false, type: { page: FileInfoComponent }, data: null });
    const importedWells = this.data.fileData.extras.wells;
    Object.assign(this.forwardData, this.data.fileData);
    for (const wellName in importedWells) {
      if (importedWells.hasOwnProperty(wellName)) {
        importedWells[wellName].nome = wellName;
        for (const typeFolder of this.wells) {
          if (typeFolder.internalType === importedWells[wellName].tipo) {
            typeFolder.entries.push(importedWells[wellName]);
          }
        }
      }
    }
  }

  change(well) {
    this.forwardData['profiles'] = well.perfis.map((el) => ({ name: el.nome, type: el.tipo, unit: el.unidade }));
    this.forwardData['lithologies'] = well.litologias.map((el) => ({ name: el }));
    this.forwardData['extras'].selectedWellName = well.nome;
    this.someEvent.emit({ cangoNext: true, type: { page: FileInfoComponent }, data: this.forwardData });
  }
}
