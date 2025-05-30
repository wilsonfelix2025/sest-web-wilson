import { Component, DoCheck, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { RegisterEventDatasetService } from '@services/dataset/register-event-dataset.service';
import { NotybarService } from '@services/notybar.service';
import { NumberUtils } from '@utils/number';
import { TableOptions } from '@utils/table-options-default';
import { UNSET } from '@utils/vazio';
import { EstiloUpdate, Evento, TrechoEventoUpdate } from 'app/repositories/models/registro-evento';
import { RegistrosEventosService } from 'app/repositories/registros-eventos.service';
import { HotTableComponent } from 'ng2-handsontable';

@Component({
  selector: 'app-events',
  templateUrl: './events.component.html',
  styleUrls: ['./events.component.scss']
})
export class EventsComponent implements OnInit, DoCheck {
  isValid: boolean = true;

  eventos: Evento[] = [];
  trechos: (TrechoEventoUpdate & { nome: string })[] = [];
  marcadores: (EstiloUpdate & { nome: string })[] = [];

  /**
   * Referência para tabela de Eventos de Perfuração
   */
  @ViewChild(HotTableComponent, { static: false }) hotTableComponent;

  /**
   * Tabela de Registros de Eventos de Perfuração
   */
  table: Handsontable;

  /**
   * Titulos das colunas na tabela de Eventos de Perfuração
   */
  baseColHeaders = [
    'Evento',
    'PM topo (m)',
    `PM base (m) <i type='icon' style='margin: auto; margin-left: 5px; cursor: pointer;' title="Para eventos pontuais: PM topo = PM base" class="far fa-question-circle"></i>`,
    'Comentário',
  ];

  /**
  * Tipos das colunas na tabela de Eventos de Perfuração.
  */
  baseColumnsTypes: any[] = [
    { data: 'nome', type: 'dropdown', source: [] },
    { data: 'pmTopo', },
    { data: 'pmBase', },
    { data: 'comentário', },
  ];

  /**
   * Opções da tabela de Eventos de Perfuração
   */
  baseOptions: any = TableOptions.createDefault({
    height: 350,
    rowHeaderWidth: 10,
    manualColumnResize: [],
    filters: false,
    minSpareRows: 1,
    contextMenu: {
      items: {
        'row_above': { name: 'Inserir linha acima' },
        'row_below': { name: 'Inserir linha abaixo' },
        'remove_row': { name: 'Remover linha' },
        'cut': { name: 'Cortar' },
        'copy': { name: 'Copiar' }
      }
    },
  });

  dadosPoco = {
    pmMax: 0,
    pmSup: 0,
  };

  tabIndex = 0;

  loading = false;

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    private dataset: DatasetService,
    private caseDataset: CaseDatasetService,

    private registrosEventosService: RegistrosEventosService,
    private registerEventDataset: RegisterEventDatasetService,
    private notybarService: NotybarService,
  ) { }

  ngAfterViewInit() {
    this.table = this.hotTableComponent.getHandsontableInstance();
  }

  ngOnInit() {
    this.dadosPoco = this.caseDataset.getLimiteCaso(this.dataset.currCaseId);

    this.eventos = this.registerEventDataset.getAll(this.dataset.currCaseId).filter(el => el.tipo === 'Evento') as Evento[];
    this.eventos.sort((a, b) => a.nome > b.nome ? 1 : -1)
    this.trechos = [];
    this.marcadores = [];

    this.eventos.forEach(e => {
      this.trechos = this.trechos.concat(e.trechos.map(t => {
        return {
          pmTopo: t.topo.pm.valor,
          pmBase: t.base.pm.valor,
          comentário: t.comentário,
          nome: e.nome,
          idRegistroEvento: e.id,
        }
      }));

      this.marcadores.push({
        marcador: e.estiloVisual.marcador,
        corDoMarcador: e.estiloVisual.corDoMarcador,
        contornoDoMarcador: e.estiloVisual.contornoDoMarcador,
        valorPadrão: e.valorPadrão,
        idRegistroEvento: e.id,
        nome: e.nome
      });
    });

    this.baseColumnsTypes[0].source = this.eventos.map(el => el.nome).sort();
  }

  ngDoCheck() {
    this.isValid = this.canSubmit();
  }

  canSubmit() {
    let isValid = true;
    return isValid;
  }

  submit() {
    this.loading = true;
    if (!this.validateTable()) {
      this.loading = false;
      this.notybarService.show('Tabela precisa estar válida.', 'warning');
      return;
    }
    const trechos: TrechoEventoUpdate[] = this.trechos.filter(el => !UNSET(el.nome)).map(el => {
      return {
        idRegistroEvento: this.eventos.find(e => e.nome === el.nome).id,
        comentário: el.comentário,
        pmBase: el.pmBase,
        pmTopo: el.pmTopo
      }
    });
    const estilos: EstiloUpdate[] = this.marcadores;

    this.registrosEventosService.editarRegistrosEventos(this.dataset.currCaseId, 'Evento', trechos, estilos).then(res => {
      console.log('FOI', res);
      this.registerEventDataset.updateList(res.registrosEventos);

      this.loading = false;
      this.closeModal();
    }).catch(() => { this.loading = false; });
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  validateTable() {
    this.trechos.sort((a, b) => UNSET(a.pmBase) ? 1 : UNSET(b.pmBase) ? -1 : Number(a.pmBase) - Number(b.pmBase));
    let valid = true;
    for (let i = 0; i < this.trechos.length; i++) {
      if (!UNSET(this.trechos[i].pmTopo) || !UNSET(this.trechos[i].pmBase) || !UNSET(this.trechos[i].nome)) {
        TableOptions.removeError(this.table, i, 'pmTopo');
        TableOptions.removeError(this.table, i, 'pmBase');
        TableOptions.removeError(this.table, i, 'nome');
        let error = '';

        if (UNSET(this.trechos[i].nome)) {
          error = 'Precisa estar preenchido';
          TableOptions.setError(this.table, i, 'nome', error);
        }
        if (UNSET(this.trechos[i].pmTopo)) {
          error = 'Precisa estar preenchido';
          TableOptions.setError(this.table, i, 'pmTopo', error);
        } else if (!NumberUtils.isNumber(this.trechos[i].pmTopo)) {
          error = 'Precisa ser um número';
          TableOptions.setError(this.table, i, 'pmTopo', error);
        } else if (this.trechos[i].pmTopo < this.dadosPoco.pmSup) {
          error = 'PM Topo não pode ser menor que PM Sup';
          TableOptions.setError(this.table, i, 'pmTopo', error);
        }
        if (UNSET(this.trechos[i].pmBase)) {
          error = 'Precisa estar preenchido';
          TableOptions.setError(this.table, i, 'pmBase', error);
        } else if (!NumberUtils.isNumber(this.trechos[i].pmBase)) {
          error = 'Precisa ser um número';
          TableOptions.setError(this.table, i, 'pmBase', error);
        } else if (this.trechos[i].pmBase > this.dadosPoco.pmMax) {
          error = 'PM Base não pode ser maior que PM Max';
          TableOptions.setError(this.table, i, 'pmBase', error);
        }
        if (error === '' && Number(this.trechos[i].pmTopo) > Number(this.trechos[i].pmBase)) {
          error = 'PM Topo deve ser menor que PM Base';
          TableOptions.setError(this.table, i, 'pmTopo', error);
          TableOptions.setError(this.table, i, 'pmBase', error);
        }
        // if (error === '') {
        //   for (let f = 0; f < this.trechos.length; f++) {
        //     if (Number(this.trechos[i].pmTopo) > Number(this.trechos[f].pmTopo)) {
        //       if (Number(this.trechos[i].pmTopo) < Number(this.trechos[f].pmBase)) {
        //         error = 'Não pode haver sobreposição';
        //         TableOptions.setError(this.table, i, 'pmTopo', error);
        //         TableOptions.setError(this.table, i, 'pmBase', error);
        //       }
        //     } else if (Number(this.trechos[i].pmBase) < Number(this.trechos[f].pmBase)) {
        //       if (Number(this.trechos[i].pmBase) > Number(this.trechos[f].pmTopo)) {
        //         error = 'Não pode haver sobreposição';
        //         TableOptions.setError(this.table, i, 'pmTopo', error);
        //         TableOptions.setError(this.table, i, 'pmBase', error);
        //       }
        //     }
        //   }
        // }
        if (error !== '') {
          valid = false;
        }
      }
      if (this.table) {
        this.table.render();
      }
    }
    return valid;
  }

}
