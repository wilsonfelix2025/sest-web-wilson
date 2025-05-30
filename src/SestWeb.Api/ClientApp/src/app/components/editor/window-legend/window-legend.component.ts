import { Component, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DatasetService } from '@services/dataset/dataset.service';
import { Perfil } from 'app/repositories/models/perfil';
import { Evento, MARCADORES, Registro } from 'app/repositories/models/registro-evento';
import { DialogImageComponent } from '../dialog-image/dialog-image.component';
@Component({
  selector: 'app-window-legend',
  templateUrl: './window-legend.component.html',
  styleUrls: ['./window-legend.component.scss']
})
export class WindowLegendComponent implements OnInit {

  perfis: Perfil[] = [];
  registrosEventos: (Registro | Evento)[] = [];
  marcadores: any = {};

  constructor(
    public dialogRef: MatDialogRef<DialogImageComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { data: { perfis: Perfil[], registrosEventos: (Registro | Evento)[] } },
  ) { }

  ngOnInit() {
    console.log('legenda', this.data.data);
    this.perfis = this.data.data.perfis;
    this.registrosEventos = this.data.data.registrosEventos;

    this.mapearMarcadores();
  }

  close(): void {
    this.dialogRef.close();
  }

  mapearMarcadores() {
    this.registrosEventos.forEach(registro => {
      this.marcadores[registro.id] = MARCADORES.find(el => registro.estiloVisual.marcador === el.value)
    });
  }

}
