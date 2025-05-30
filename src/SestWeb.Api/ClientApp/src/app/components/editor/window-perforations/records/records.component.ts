import { Component, DoCheck, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { RegisterEventDatasetService } from '@services/dataset/register-event-dataset.service';
import { NotybarService } from '@services/notybar.service';
import { NumberUtils } from '@utils/number';
import { TIPOS_PERFIS, TipoPerfil } from '@utils/perfil/tipo-perfil';
import { UNSET } from '@utils/vazio';
import { EstiloUpdate, Registro, TrechoRegistroUpdate } from 'app/repositories/models/registro-evento';
import { RegistrosEventosService } from 'app/repositories/registros-eventos.service';
import { RecordsTableComponent } from './records-table/records-table.component';

@Component({
  selector: 'sest-records',
  templateUrl: './records.component.html',
  styleUrls: ['./records.component.scss']
})
export class RecordsComponent implements OnInit, DoCheck {

  selected = 0;

  isValid: boolean = true;
  currTab: number = 0;
  registros: Registro[] = [];

  marcadores: (EstiloUpdate & { nome: string })[] = [];

  dadosPoco = {
    pmMax: 0,
    pmSup: 0,
    pvMax: 0,
    pvSup: 0,
  };

  @ViewChild('table', { static: false }) table: RecordsTableComponent;

  abas: {
    nome: string,
    trechos: (TrechoRegistroUpdate & { nome: string })[],
    unidadeGeral?: string,
    tipos: { tipo?, nome?: string, id: string, unidade?: string }[],

    valid: boolean,
    colHeaders: string[],
    colTypes: any[],
    pv: boolean,
    unidade?: boolean,
  }[] = [
      {
        nome: 'Pressão de Poros',
        trechos: [],
        unidadeGeral: TIPOS_PERFIS.PPORO.unidadePadrão.símbolo,
        tipos: [
          { tipo: TIPOS_PERFIS.PPORO, id: '' }
        ],

        valid: false,
        colHeaders: ['PM (m)', 'PV (m)', 'Valor'],
        colTypes: [{ data: 'pm' }, { data: 'pv' }, { data: 'valor' }],
        pv: false,
      },
      {
        nome: 'Ensaios',
        trechos: [],
        tipos: [
          { tipo: TIPOS_PERFIS.ANGAT, unidade: TIPOS_PERFIS.ANGAT.unidadePadrão.símbolo, id: '' },
          { tipo: TIPOS_PERFIS.BIOT, unidade: TIPOS_PERFIS.BIOT.unidadePadrão.símbolo, id: '' },
          { tipo: TIPOS_PERFIS.PERM, unidade: TIPOS_PERFIS.PERM.unidadePadrão.símbolo, id: '' },
          { tipo: TIPOS_PERFIS.KS, unidade: TIPOS_PERFIS.KS.unidadePadrão.símbolo, id: '' },
          { tipo: TIPOS_PERFIS.UCS, unidade: TIPOS_PERFIS.UCS.unidadePadrão.símbolo, id: '' },
          { tipo: TIPOS_PERFIS.RESTR, unidade: TIPOS_PERFIS.RESTR.unidadePadrão.símbolo, id: '' },
          { tipo: TIPOS_PERFIS.COESA, unidade: TIPOS_PERFIS.COESA.unidadePadrão.símbolo, id: '' },
          { tipo: TIPOS_PERFIS.YOUNG, unidade: TIPOS_PERFIS.YOUNG.unidadePadrão.símbolo, id: '' },
        ],

        valid: false,
        colHeaders: ['Propriedade', 'PM (m)', 'PV (m)', 'Valor', 'Unidade'],
        colTypes: [
          { data: 'nome', type: 'dropdown', source: [] },
          { data: 'pm' },
          { data: 'pv' },
          { data: 'valor' },
          { data: 'unidade', type: 'dropdown', source: [] }
        ],
        pv: false,
        unidade: true,
      },
      {
        nome: 'Testes de absorção',
        trechos: [],
        unidadeGeral: TIPOS_PERFIS.PPORO.unidadePadrão.símbolo,
        tipos: [
          { nome: 'FIT', id: '' },
          { nome: 'LOT', id: '' },
          { nome: 'XLOT', id: '' },
          { nome: 'Minifrac', id: '' },
          { nome: 'Microfrac', id: '' },
          { nome: 'Micro TI', id: '' },
          { nome: 'Step rate test', id: '' },
        ],

        valid: false,
        colHeaders: ['Tipo', 'PM (m)', 'PV (m)', 'Valor'],
        colTypes: [
          { data: 'nome', type: 'dropdown', source: [] },
          { data: 'pm' },
          { data: 'pv' },
          { data: 'valor' }
        ],
        pv: false,
      },
      {
        nome: 'Outros',
        trechos: [],
        tipos: [
          { nome: 'Pump in/ flowback', id: '' },
          { nome: 'Perfilagem', id: '' },
          { nome: 'Cascalho angular', id: '' },
          { nome: 'Cascalho lascado', id: '' },
          { nome: 'Cascalho tabular', id: '' },
          { nome: 'Cascalho arredondado', id: '' },
          { nome: 'Cascalho outros', id: '' },
          { nome: 'Testemunhagem', id: '' },
          { nome: 'Falha operacional', id: '' },
          { nome: 'Pescaria', id: '' },
        ],

        valid: false,
        colHeaders: ['Registro', 'PM (m)', 'PV (m)', 'Posição no track', 'Comentário'],
        colTypes: [
          { data: 'nome', type: 'dropdown', source: [] },
          { data: 'pm' },
          { data: 'pv' },
          { data: 'valor' },
          { data: 'comentário' }
        ],
        pv: false,
      },
    ];

  loading = false;

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    private dataset: DatasetService,
    private caseDataset: CaseDatasetService,

    private registrosEventosService: RegistrosEventosService,
    private registerEventDataset: RegisterEventDatasetService,
    private notybarService: NotybarService,
  ) { }

  ngOnInit() {
    this.dadosPoco = this.caseDataset.getLimiteCaso(this.dataset.currCaseId);
    this.updateLists();
  }

  updateLists() {
    this.registros = this.registerEventDataset.getAll(this.dataset.currCaseId).filter(el => el.tipo === 'Registro') as Registro[];

    this.marcadores = [];
    this.abas.forEach(aba => {
      aba.trechos = [];
    });

    this.registros.forEach(registro => {
      this.abas.forEach(aba => {
        const tipo = aba.tipos.findIndex(el => {
          if (!UNSET(el.tipo)) {
            return el.tipo.descrição === registro.nome;
          }
          return el.nome === registro.nome;
        });
        if (tipo >= 0) {
          if (!UNSET(aba.unidadeGeral) && !UNSET(registro.unidade)) {
            aba.unidadeGeral = registro.unidade;
          } else if (!UNSET(aba.tipos[tipo].unidade)) {
            aba.tipos[tipo].unidade = registro.unidade;
          }
          aba.tipos[tipo].id = registro.id;
          aba.trechos = aba.trechos.concat(registro.trechos.map(t => {
            return {
              idRegistroEvento: registro.id,
              nome: registro.nome,
              pm: t.ponto.pm.valor,
              pv: t.ponto.pv.valor,
              valor: t.valor,
              comentário: t.comentário,
              unidade: !UNSET(aba.tipos[tipo].unidade) ? aba.tipos[tipo].unidade : undefined
            }
          }));
          if (aba.colTypes[0].data === 'nome') {
            aba.colTypes[0].source = aba.tipos.map(el => {
              if (!UNSET(el.tipo)) {
                return el.tipo.descrição;
              }
              return el.nome;
            });
          }
        }
      });

      this.marcadores.push({
        marcador: registro.estiloVisual.marcador,
        corDoMarcador: registro.estiloVisual.corDoMarcador,
        contornoDoMarcador: registro.estiloVisual.contornoDoMarcador,
        unidade: registro.unidade,
        idRegistroEvento: registro.id,
        nome: registro.nome
      });
    });

  }

  ngDoCheck() {
    this.isValid = this.canSubmit();
  }

  canSubmit() {
    let isValid = true;
    return isValid;
  }

  submit() {
    this.apply(true);
  }

  apply(close = false) {
    this.loading = true;
    if (this.table) {
      this.table.validateTable();
    }
    this.abas.forEach(aba => this.validarAba(aba));
    const telasInvalidas = this.abas.filter(el => !el.valid).map(el => el.nome);
    if (telasInvalidas.length > 0) {
      if (telasInvalidas.length === 1) {
        this.notybarService.show(`A tabela ${telasInvalidas[0]} está inválida.`, 'warning');
      } else {
        this.notybarService.show(`As tabelas ${telasInvalidas.toString()} estão inválidas.`, 'warning');
      }
      this.loading = false;
      return;
    }

    const estilos: EstiloUpdate[] = this.marcadores;

    let trechos: TrechoRegistroUpdate[] = [];
    this.abas.forEach(aba => {
      if (!UNSET(aba.unidadeGeral)) {
        const i = estilos.findIndex(estilo => aba.tipos.some(el => el.id === estilo.idRegistroEvento));
        if (i >= 0) {
          estilos[i].unidade = aba.unidadeGeral;
        }
      } else {
        aba.tipos.forEach(tipo => {
          const i = estilos.findIndex(el => el.idRegistroEvento === tipo.id);
          if (i >= 0) {
            estilos[i].unidade = tipo.unidade;
          }
        });
      }

      const _t: TrechoRegistroUpdate[] = aba.trechos.filter(trecho => !UNSET(trecho.valor)).map(trecho => {
        let id = '';
        if (!UNSET(trecho.idRegistroEvento)) {
          id = trecho.idRegistroEvento;
        } else if (aba.nome === 'Pressão de Poros') {
          id = this.registros.find(e => e.nome === aba.nome).id;
        } else if (this.registros.some(e => e.nome === trecho.nome)) {
          id = this.registros.find(e => e.nome === trecho.nome).id;
        } else {
          console.warn('ERROR ao mapear trecho', trecho);
        }
        return {
          idRegistroEvento: id,
          comentário: trecho.comentário,
          pm: aba.pv ? undefined : trecho.pm,
          pv: aba.pv ? trecho.pv : undefined,
          valor: trecho.valor,
        }
      });

      trechos = trechos.concat(_t);
    });

    this.registrosEventosService.editarRegistrosEventos(this.dataset.currCaseId, 'Registro', trechos, estilos).then(res => {
      console.log('FOI', res);
      this.registerEventDataset.updateList(res.registrosEventos);
      if (close) {
        this.closeModal();
      } else {
        this.updateLists();
      }
      this.loading = false;
    }).catch(() => { this.loading = false; });
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  validarAba(aba) {
    aba.trechos.sort((a, b) => UNSET(a.pm) ? 1 : UNSET(b.pm) ? -1 : Number(a.pm) - Number(b.pm));
    let valid = true;
    for (let i = 0; i < aba.trechos.length; i++) {
      if (!UNSET(aba.trechos[i].nome) || !UNSET(aba.trechos[i].pm) || !UNSET(aba.trechos[i].pv) || !UNSET(aba.trechos[i].valor)) {
        let error = '';
        if (aba.pv) {
          if (UNSET(aba.trechos[i].pv)) {
            error = 'Precisa estar preenchido';
          } else if (!NumberUtils.isNumber(aba.trechos[i].pv)) {
            error = 'Precisa ser um número';
          } else if (aba.trechos[i].pv < this.dadosPoco.pvSup || aba.trechos[i].pv > this.dadosPoco.pvMax) {
            error = 'PV deve estar dentro dos limites de sedimentos';
          }
        } else {
          if (UNSET(aba.trechos[i].pm)) {
            error = 'Precisa estar preenchido';
          } else if (!NumberUtils.isNumber(aba.trechos[i].pm)) {
            error = 'Precisa ser um número';
          } else if (aba.trechos[i].pm < this.dadosPoco.pmSup || aba.trechos[i].pm > this.dadosPoco.pmMax) {
            error = 'PM deve estar dentro dos limites de sedimentos';
          }
        }
        if (aba.colTypes[0].data === 'nome') {
          if (UNSET(aba.trechos[i].nome)) {
            error = 'Precisa estar preenchido';
          } else if (!aba.colTypes[0].source.includes(aba.trechos[i].nome)) {
            error = 'Precisa ser um valor válido';
          }

          else if (aba.unidade) {
            const tipo: TipoPerfil = aba.tipos.find(el => {
              return el.tipo.descrição === aba.trechos[i].nome;
            }).tipo;
            const unidades = tipo.grupoUnidade.unidadesDeMedida.map(el => el.símbolo);
            if (UNSET(aba.trechos[i].unidade)) {
              error = 'Precisa estar preenchido';
            } else if (!unidades.includes(aba.trechos[i].unidade)) {
              error = 'Precisa ser uma unidade válida';
            } else {
              const t = aba.tipos.findIndex(el => el.tipo.descrição === aba.trechos[i].nome);
              if (t >= 0) {
                aba.tipos[t].unidade = aba.trechos[i].unidade;
              }
            }
          }
        }

        if (UNSET(aba.trechos[i].valor)) {
          error = 'Precisa estar preenchido';
        } else if (!NumberUtils.isNumber(aba.trechos[i].valor)) {
          error = 'Precisa ser um número';
        }
        if (error !== '') {
          valid = false;
        }
      }
    }

    aba.valid = valid;
    return valid;
  }


}
