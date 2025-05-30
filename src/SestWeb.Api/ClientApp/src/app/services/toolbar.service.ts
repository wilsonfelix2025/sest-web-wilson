import { Injectable } from '@angular/core';
import { FileService } from '../repositories/file.service';
import { WellService } from '../repositories/well.service';
import { OilfieldService } from '../repositories/oilfield.service';
import { OpunitService } from '../repositories/opunit.service';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ToolbarService {

  readonly itemTypeMap = [
    {
      name: 'unit',
      label: 'unidade operacional'
    },
    {
      name: 'oilfield',
      label: 'campo',
      parent: 'unidade operacional',
      parentField: 'opunit'
    },
    {
      name: 'well',
      label: 'poço',
      parent: 'campo',
      parentField: 'oilfield'
    },
    {
      name: 'file',
      label: 'caso',
      parent: 'poço',
      parentField: 'well'
    }
  ];

  constructor(
    private units: OpunitService,
    private oilfields: OilfieldService,
    private wells: WellService,
    private files: FileService
  ) { }

  fetchIdFromUrl(url: string) {
    const urlParts = url.split('/');
    return urlParts[urlParts.length - 2];
  }

  getProviderByItemType(itemType) {
    switch (itemType) {
      case 0: return this.units;
      case 1: return this.oilfields;
      case 2: return this.wells;
      case 3: return this.files;
      default: return undefined;
    }
  }

  rename(item, itemType, newName) {
    const id = this.fetchIdFromUrl(item.url);
    const provider = this.getProviderByItemType(itemType);

    return provider.rename(id, newName);
  }

  delete(item, itemType) {
    const id = this.fetchIdFromUrl(item.url);
    const provider = this.getProviderByItemType(itemType);

    return provider.delete(id);
  }

  move(item, itemType, targetId) {
    const id = this.fetchIdFromUrl(item.url);
    let provider = null;

    switch (itemType) {
      case 1: provider = this.oilfields; break;
      case 2: provider = this.wells; break;
      case 3: provider = this.files; break;
    }

    return provider.move(id, targetId);
  }

  loadParentList(itemType) {
    let provider = null;

    switch (itemType) {
      case 1: provider = this.units; break;
      case 2: provider = this.oilfields; break;
      case 3: provider = this.wells; break;
    }

    return new Observable((subscriber) => {
      provider.getAll().subscribe(val => {
        if (itemType === 1) {
          const data = val.map(option => ({ id: this.fetchIdFromUrl(option.url), label: option.name }));
          subscriber.next(data);
        } else {
          const parentTypeInfo = this.itemTypeMap[itemType - 1];
          const parentTypeSentenceCase = `${parentTypeInfo.parent.substr(0, 1).toUpperCase()}${parentTypeInfo.parent.substring(1)}`;
          const data = val.map(option => ({
            id: this.fetchIdFromUrl(option.url),
            label: option.name,
            description: `${parentTypeSentenceCase}: ${option[parentTypeInfo.parentField]['name']}`
          }));
          subscriber.next(data);
        }
      });
    });
  }

  create(itemType) {
    const provider = this.getProviderByItemType(itemType);
    // provider.create('name')
  }

  duplicate(item, itemType) {
    const id = this.fetchIdFromUrl(item.url);

    if (itemType === 3) {
      return this.files.duplicate(id);
    }
  }
}
