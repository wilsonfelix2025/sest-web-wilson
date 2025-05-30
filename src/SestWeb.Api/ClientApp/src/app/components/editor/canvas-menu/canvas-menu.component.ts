import { Component, Input, Output, EventEmitter } from '@angular/core';
import { Perfil } from 'app/repositories/models/perfil';
import { Evento, Registro } from 'app/repositories/models/registro-evento';

@Component({
  selector: 'sest-canvas-menu',
  templateUrl: './canvas-menu.component.html',
  styleUrls: ['./canvas-menu.component.scss']
})
export class CanvasMenuComponent {

  @Input() hasCurve: boolean;
  @Input() defaultXScale: boolean;
  @Input() logScale: boolean;
  @Input() csv: any;
  @Input() title: string;

  @Output() editChartName = new EventEmitter();

  @Output() changeScale = new EventEmitter();
  @Output() changeWidth = new EventEmitter();

  @Output() changeXScale = new EventEmitter();
  @Output() setDefaultXScale = new EventEmitter();

  @Output() removeChart = new EventEmitter();

  @Output() exportChart = new EventEmitter<string>();
}
