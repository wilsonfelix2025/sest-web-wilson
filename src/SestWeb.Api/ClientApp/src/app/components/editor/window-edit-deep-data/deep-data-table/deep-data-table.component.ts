import { Component, OnInit, Input } from '@angular/core';
import { DialogService } from '@services/dialog.service';
import { tipo } from '../window-edit-deep-data.component';
import { InsertDataComponent } from '../insert-data/insert-data.component';
import { Litologia } from 'app/repositories/models/litologia';
import { TableOptions } from '@utils/table-options-default';
import { Perfil } from 'app/repositories/models/perfil';
import { Trajetoria } from 'app/repositories/models/trajetoria';

@Component({
  selector: 'sest-deep-data-table',
  templateUrl: './deep-data-table.component.html',
  styleUrls: ['./deep-data-table.component.scss']
})
export class DeepDataTableComponent implements OnInit {

  /**
   * Tipo do dado de profundidade sendo editado
   */
  @Input() tipo: tipo;

  /**
   * O dado de profundidade atual.
   *
   * Pode ser trajetoria, perfil ou litogia.
   */
  @Input() dadoDeProfundidade: any;

  @Input() calculado: boolean;

  @Input() casoEditavel: boolean;

  tipos = {
    'Perfil': {
      tituloInputSuperior: 'Perfil',
      colHeaders: ['PM (m)', 'PV (m)', 'Valor'],
      columnsTypes: [
        { data: 'pm.valor', readOnly: true },
        { data: 'pv.valor', readOnly: true },
        { data: 'valor', readOnly: true },
      ],
    },
    'Trajetória': {
      tituloInputSuperior: 'Método de Cálculo',
      colHeaders: ['PM (m)', 'PV (m)', 'Inclinação (◦)', 'Azimute (◦)'],
      columnsTypes: [
        { data: 'pm.valor', readOnly: true },
        { data: 'pv.valor', readOnly: true },
        { data: 'inclinação', readOnly: true },
        { data: 'azimute', readOnly: true },
      ],
    },
    'Litologia': {
      tituloInputSuperior: 'Tipo de Litologia',
      colHeaders: ['PM Topo (m)', 'Código', 'Litologia'],
      columnsTypes: [
        { data: 'pm', readOnly: true },
        { data: 'codigo', readOnly: true },
        { data: 'litologia', readOnly: true },
      ],
    },
  };

  dadoInputSuperior = '';

  dataset: any[] = [];

  options: any = TableOptions.createDefault({
    height: 240,
    rowHeaderWidth: 15,
    manualColumnResize: [],
    filters: false,
  });

  constructor(
    public dialog: DialogService,
  ) { }

  ngOnInit() {
    if (this.tipo === 'Trajetória') {
      this.pegarDadosTrajetoria(this.dadoDeProfundidade);
    } else if (this.tipo === 'Litologia') {
      this.pegarDadosLitologia(this.dadoDeProfundidade);
    } else {
      this.pegarDadosPerfil(this.dadoDeProfundidade);
    }
  }

  pegarDadosTrajetoria(trajetoria: Trajetoria) {
    this.dataset = JSON.parse(JSON.stringify(trajetoria.pontos));
    this.dadoInputSuperior = trajetoria.métodoDeCálculoDaTrajetória;
  }

  pegarDadosLitologia(litologia: Litologia) {
    this.dataset = litologia.pontos.map(ponto =>
      ({ 'pm': ponto.pm.valor, 'codigo': ponto.tipoRocha.numero, 'litologia': ponto.tipoRocha.nome }));
    this.dadoInputSuperior = litologia.classificação.nome;
  }

  pegarDadosPerfil(perfil: Perfil) {
    this.dataset = perfil.pontos;
    this.dadoInputSuperior = perfil.nome;

    this.tipos[this.tipo].colHeaders[2] += ` (${perfil.grupoDeUnidades.unidadePadrão.símbolo})`;
  }

  openWindowInsertData() {
    this.dialog.openPageDialog(InsertDataComponent, { minHeight: 450, minWidth: 350 }, { data: this.dadoDeProfundidade, tipo: this.tipo });
  }

}
