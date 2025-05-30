import { Component, Input, OnChanges, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material';
import { EstiloUpdate, Marcador, MARCADORES } from 'app/repositories/models/registro-evento';
import { ColorPickerService } from 'ngx-color-picker';

@Component({
  selector: 'sest-markers',
  templateUrl: './markers.component.html',
  styleUrls: ['./markers.component.scss']
})
export class MarkersComponent implements OnInit, OnChanges {

  isValid: boolean = true;

  position: { value: string, tooltip: string } = { value: '', tooltip: '' };

  Marcadores: Marcador[] = MARCADORES;

  @Input() marcadores: (EstiloUpdate & { nome: string })[] = [];
  @Input() tipo: 'Evento' | 'Registro' = 'Registro';

  displayedColumns = ['events', 'marker', 'markerColor', 'outlineColor'];
  dataSource = new MatTableDataSource(this.marcadores);

  constructor(
    private cpService: ColorPickerService,
  ) { }

  ngOnInit() {
    if (this.tipo === 'Evento') {
      this.displayedColumns.push('position');
    }
  }

  ngOnChanges() {
    if (this.marcadores) {
      this.dataSource = new MatTableDataSource(this.marcadores);
    }
  }

  public onChangeColorHex8(color: string): string {
    const hsva = this.cpService.stringToHsva(color, true);

    if (hsva) {
      return this.cpService.outputFormat(hsva, 'rgba', null);
    }

    return '';
  }
}
