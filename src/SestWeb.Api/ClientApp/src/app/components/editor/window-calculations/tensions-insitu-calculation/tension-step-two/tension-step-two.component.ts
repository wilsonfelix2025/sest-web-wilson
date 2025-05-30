import { Component, DoCheck, Input, OnChanges, OnInit } from '@angular/core';
import { DialogService } from '@services/dialog.service';
import { RelationTensionsComponent } from '../relation-tensions/relation-tensions.component';
import { TableOptions } from '@utils/table-options-default';
import { NotybarService } from '@services/notybar.service';
import { CalculoTHorMax, RetornoCalculoTensoesInSitu } from 'app/repositories/models/calculo';
import { Case } from 'app/repositories/models/case';
import { TreeDatasetService } from '@services/dataset/state/tree-dataset.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { NumberUtils } from '@utils/number';

@Component({
  selector: 'sest-tension-step-two',
  templateUrl: './tension-step-two.component.html',
  styleUrls: ['./tension-step-two.component.scss']
})
export class TensionStepTwoComponent implements OnInit, DoCheck, OnChanges {

  @Input() perfis;
  @Input() metodologia;
  @Input() deplecao: boolean;
  @Input() calcularSh: boolean;
  @Input() calculo: RetornoCalculoTensoesInSitu;

  /**
   * O caso de estudo aberto atualmente.
   */
  @Input() currCase: Case;

  perfisUsados: any = {};

  /**
   * Tipos metodologia
   */
  tiposMetodologia = {
    RelacaoTensoes: {
      label: 'Relação entre tensões', value: 'RelaçãoEntreTensões',
      backObj: 'relaçãoTensão',
      azthm: { value: 0, tooltip: '', label: 'AZTH menor (°)' },
      tiposRelacao: [
        { label: 'THORm`/THORM`', value: 'THORm`/THORM`', backObj: 'THORmLinhaXTHORMLinha', perfis: ['GPORO', 'TVERT'] },
        { label: 'THORm/THORM', value: 'THORm/THORM', backObj: 'THORmXTHORM', perfis: ['TVERT'] },
        { label: 'THORM`/TVERT`', value: 'THORM`/TVERT`', backObj: 'THORMLinhaXTVertLinha', perfis: ['GPORO', 'TVERT'] },
        { label: 'THORM/TVERT', value: 'THORM/TVERT', backObj: 'THORMXTVert', perfis: ['TVERT'] },
      ],
      tipoRelacaoSelecionado: undefined
    },
    Breakout: {
      label: 'Breakout em trechos verticais', value: 'BreakoutTrechosVerticais',
      backObj: 'breakout',
      azthm: { value: 0, tooltip: '', label: 'AZTH menor (°)' },
      perfis: ['UCS', 'ANGAT', 'GPORO'],
      dataset: [{}, {}, {}],
      colHeaders: [
        'PM (m)',
        'Azim. Breakout (°)',
        'Larg. Breakout (°)',
        'Peso de fluído (lb/gal)',
      ],
      columns: [
        { data: 'pm' },
        { data: 'azimute' },
        { data: 'largura' },
        { data: 'pesoFluido' },
      ]
    },
    Fratura: {
      label: 'Fratura em trechos verticais', value: 'FraturasTrechosVerticais',
      backObj: 'fraturasTrechosVerticais',
      azthm: { value: 0, tooltip: '', label: 'AZTH menor (°)' },
      perfis: ['RESTR', 'GPORO'],
      dataset: [{}, {}, {}],
      colHeaders: [
        'PM (m)',
        'Peso de fluído (lb/gal)',
      ],
      columns: [
        { data: 'pm' },
        { data: 'pesoFluido' },
      ]
    },
  }
  metodologias = Object.keys(this.tiposMetodologia);

  metodologiaSelecionada: any = this.tiposMetodologia.RelacaoTensoes;

  /**
  * Tabela.
  */
  hotTable: Handsontable;

  /**
  * Opções tabela
  */
  options = TableOptions.createDefault({
    width: 470,
    height: 110,
    minSpareRows: 1,
    rowHeaderWidth: 10,
    manualColumnResize: [],
    filters: false,
  });

  nomeCalculo = { value: 'TIS_Calc', tooltip: '' };

  isValid: boolean = true;
  nomesEmUso: string[] = [];

  editando: boolean = false;

  constructor(
    public dialog: DialogService,
    private notybarService: NotybarService,

    private dataset: DatasetService,
    private treeService: TreeDatasetService,
  ) { }

  ngOnInit() {
    this.tiposMetodologia.RelacaoTensoes.tipoRelacaoSelecionado = this.tiposMetodologia.RelacaoTensoes.tiposRelacao[0];

    this.nomesEmUso = this.treeService.getUsedNames(this.dataset.currCaseId, ['Cálculos', 'Tensões']);

    if (this.calculo) {
      this.editando = true;
      this.nomeCalculo.value = this.calculo.nome;

      this.nomesEmUso = this.nomesEmUso.filter(el => el !== this.nomeCalculo.value);
      this.metodologiaSelecionada = this.tiposMetodologia[this.metodologias.find(el => this.tiposMetodologia[el].value === this.calculo.metodologiaTHORmax)];
      if (this.metodologiaSelecionada.value === this.tiposMetodologia.RelacaoTensoes.value) {
        this.metodologiaSelecionada.tipoRelacaoSelecionado = this.metodologiaSelecionada.tiposRelacao.find(el => el.backObj === this.calculo[this.metodologiaSelecionada.backObj].tipoRelação);
        this.metodologiaSelecionada.azthm.value = this.calculo[this.metodologiaSelecionada.backObj].azthMenor;
      } else {
        const titleTrechos = `trechos${this.metodologiaSelecionada.value === this.tiposMetodologia.Fratura.value ? 'Fratura' : 'Breakout'}`;
        this.metodologiaSelecionada.dataset = this.calculo[this.metodologiaSelecionada.backObj][titleTrechos];
        this.metodologiaSelecionada.azthm.value = this.calculo[this.metodologiaSelecionada.backObj].azimuth;
      }
    }

  }

  ngDoCheck() {
    this.isValid = this.canSubmit();
  }

  ngOnChanges(changes) {
    if (changes.perfis !== undefined && changes.perfis.currentValue !== undefined) {
      Object.keys(this.perfisUsados).forEach(key => {
        this.perfisUsados[key] = false;
      });
      if (this.calcularSh) {
        this.metodologia.perfis.forEach(p => {
          this.perfisUsados[p] = true;
        });
      }
      if (this.deplecao) {
        this.perfisUsados.GPORO = true;
        this.perfisUsados.POISS = true;
        this.perfisUsados.BIOT = true;
      }
    }
  }

  canSubmit() {
    let isValid = true;

    if (this.nomeCalculo.value === undefined || this.nomeCalculo.value === null || this.nomeCalculo.value === '') {
      this.nomeCalculo.tooltip = 'Precisa estar preenchido';
      isValid = false;
    } else if (this.nomesEmUso.includes(this.nomeCalculo.value)) {
      this.nomeCalculo.tooltip = 'Nome em uso';
      isValid = false;
    } else {
      this.nomeCalculo.tooltip = '';
    }
    if (this.metodologiaSelecionada.azthm !== undefined) {
      if (this.metodologiaSelecionada.azthm.value === undefined || this.metodologiaSelecionada.azthm.value === null || this.metodologiaSelecionada.azthm.value === '') {
        this.metodologiaSelecionada.azthm.tooltip = 'Precisa estar preenchido';
        isValid = false;
      } else if (!NumberUtils.isNumber(this.metodologiaSelecionada.azthm.value)) {
        this.metodologiaSelecionada.azthm.tooltip = 'Precisa ser um número';
        isValid = false;
      } else {
        this.metodologiaSelecionada.azthm.tooltip = '';
      }
    }
    if (this.metodologiaSelecionada.value === this.tiposMetodologia.RelacaoTensoes.value) {
      if (this.perfis.RET.selecionado === undefined) {
        this.perfis.RET.tooltip = 'Precisa escolher um perfil';
        isValid = false;
      } else {
        this.perfis.RET.tooltip = '';
      }
      this.metodologiaSelecionada.tipoRelacaoSelecionado.perfis.forEach(p => {
        if (this.perfis[p].selecionado === undefined) {
          this.perfis[p].tooltip = 'Precisa escolher um perfil';
          isValid = false;
        } else {
          this.perfis[p].tooltip = '';
        }
      });
    } else {
      this.metodologiaSelecionada.perfis.forEach(p => {
        if (this.perfis[p].selecionado === undefined) {
          this.perfis[p].tooltip = 'Precisa escolher um perfil';
          isValid = false;
        } else {
          this.perfis[p].tooltip = '';
        }
      });
    }

    return isValid;
  }

  openWindowCriarPerfil() {
    this.dialog.openPageDialog(RelationTensionsComponent, { minHeight: 520, minWidth: 50 }, { perfis: this.perfis.RET.lista });
  }

  pegarDados(): { calculo: CalculoTHorMax, backObj: string } | undefined {
    if (!this.isValid) {
      this.notybarService.show('Conserte os erros antes de finalizar.', 'warning');
      return undefined;
    }
    const response: any = {};
    let perfisEscolhidos;
    if (this.metodologiaSelecionada.value === this.tiposMetodologia.RelacaoTensoes.value) {
      response.azthMenor = this.metodologiaSelecionada.azthm.value;
      response.tipoRelação = this.metodologiaSelecionada.tipoRelacaoSelecionado.value;
      response.perfilRelaçãoTensãoId = this.perfis.RET.selecionado.id;
      perfisEscolhidos = this.metodologiaSelecionada.tipoRelacaoSelecionado.perfis.map(el => [`perfil${el}Id`, this.perfis[el].selecionado.id]);
    } else {
      if (this.metodologiaSelecionada.azthm !== undefined) {
        response.azimuth = this.metodologiaSelecionada.azthm.value;
      }
      const titleTrechos = `trechos${this.metodologiaSelecionada.value === this.tiposMetodologia.Fratura.value ? 'Fratura' : 'Breakout'}`;
      const cols = this.metodologiaSelecionada.columns.map(c => c.data);
      const verificador = this.metodologiaSelecionada.dataset.filter(el => {
        return cols.some(col => el[col] !== undefined && el[col] !== null && el[col] !== '');
      });
      response[titleTrechos] = this.metodologiaSelecionada.dataset.filter(el => {
        return cols.every(col => el[col] !== undefined && el[col] !== null && el[col] !== '');
      });
      if (verificador.length === 0) {
        this.notybarService.show('Tabela precisa ter valores.', 'warning');
        return;
      }
      if (response[titleTrechos].length !== verificador.length) {
        this.notybarService.show('Tabela precisa estar completa.', 'warning');
        return;
      }

      perfisEscolhidos = this.metodologiaSelecionada.perfis.map(el => [`perfil${el}Id`, this.perfis[el].selecionado.id]);
    }
    perfisEscolhidos.forEach(perfil => {
      response[perfil[0]] = perfil[1];
    });

    return {
      calculo: {
        tensãoHorizontalMaiorMetodologiaCálculo:
          this.metodologiaSelecionada.value,
        [this.metodologiaSelecionada.backObj]: response,
        nomeCálculo: this.nomeCalculo.value
      }, backObj: this.metodologiaSelecionada.backObj
    };
  }
}
