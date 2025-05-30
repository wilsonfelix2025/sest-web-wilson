import { Component, OnInit, Inject, ViewChild, AfterViewInit, DoCheck } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogService } from '@services/dialog.service';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { CorrelationService } from 'app/repositories/correlation.service';
import { Correlation } from 'app/repositories/models/correlation';
import { Calculo, CalculoPerfis } from 'app/repositories/models/calculo';
import { CalculoService } from 'app/repositories/calculo.service';
import { Case } from 'app/repositories/models/case';
import { DatasetService } from '@services/dataset/dataset.service';
import { ArrayUtils } from '@utils/array';
import { NotybarService } from '@services/notybar.service';
import { TableOptions } from '@utils/table-options-default';
import { TreeDatasetService } from '@services/dataset/state/tree-dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { CalculationDatasetService } from '@services/dataset/calculation-dataset.service';
import { Perfil } from 'app/repositories/models/perfil';
import { Observable } from 'rxjs';
import { LoaderService } from '@services/loader.service';

@Component({
  selector: 'app-profile-calculations',
  templateUrl: './profile-calculations.component.html',
  styleUrls: ['./profile-calculations.component.scss']
})
export class ProfileCalculationsComponent implements OnInit, AfterViewInit, DoCheck {
  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  /**
   * Referência para tabela Parametros das Correlações Base.
   */
  @ViewChild('tableBase', { static: true }) tableBaseComponent;

  /**
   * Tabela Parametros das Correlações Base.
   */
  tableBase: Handsontable;

  /**
   * Titulos das colunas na tabela Parametros das Correlações Base.
   */
  baseColHeaders = [
    'Saída',
    'Correlação',
    'Parâmetro',
    'Valor',
  ];

  /**
  * Tipos das colunas na tabela Parametros das Correlações Base.
  */
  baseColumnsTypes: any[] = [
    { data: 'saida', readOnly: true },
    { data: 'correlação', readOnly: true },
    { data: 'parâmetro', readOnly: true },
    { data: 'valor', type: 'numeric', numericFormat: { pattern: '0[.][00000]' } },
  ];

  /**
  * dataset da tabela Parametros das Correlações Base
  */
  baseDataset: { saida, correlação, parâmetro, valor }[] = [];

  /**
  * Opções da tabela Parametros das Correlações Base
  */
  baseOptions: any = TableOptions.createDefault({
    height: 400,
    rowHeaderWidth: 10,
    manualColumnResize: [],
    filters: false,
    fillHandle: {
      autoInsertRow: false,
    }
  });

  /**
   * Referência para tabela Parametros por Trecho Específico A.
   */
  @ViewChild('tableA', { static: true }) tableAComponent;

  /**
   * Tabela Parametros por Trecho Específico A.
   */
  tableA: Handsontable;

  lineSelected = [];

  selectedAll = false;

  /**
   * Titulos das colunas na tabela Parametros por Trecho Específico A.
   */
  specificAColHeaders = [
    `<input type='checkbox'  ${this.selectedAll ? 'checked="checked"' : ''}'>`,
    'Perfil',
    'Correlação',
    'Topo(PM)',
    'Base(PM)',
  ];

  /**
  * Tipos das colunas na tabela Parametros por Trecho Específico A.
  */
  specificAColumnsTypes: any[] = [
    { data: 'selected', type: 'checkbox' },
    { data: 'tipoPerfil', readOnly: true },
    { data: 'correlação', readOnly: true },
    { data: 'pmTopo', type: 'numeric', readOnly: true },
    { data: 'pmBase', type: 'numeric', readOnly: true },
  ];

  /**
  * dataset da tabela Parametros por Trecho Específico A
  */
  specificADataset: { selected, tipoPerfil, correlação, pmTopo, pmBase }[] = [];

  /**
  * Opções das tabelas Parametros por Trecho Específico A
  */
  specificAOptions: any = TableOptions.createDefault({
    afterOnCellMouseDown: (event, coords) => {
      if (event.target.type === 'checkbox') {
        // Se clicou no checkbox do título, marca ou desmarca todos as linhas da tabela.
        if (coords.row < 0) {
          this.selectedAll = !event.target.checked;

          this.specificAColHeaders[0] = `<input type='checkbox' ${this.selectedAll ? 'checked="checked"' : ''}>`;

          for (let i = 0; i < this.tableA.countRows(); i += 1) {
            this.tableA.setDataAtCell(i, 0, this.selectedAll as any);
          }
        }
      }
    },
    afterChange: () => {
      this.atualizarSelecionados();
    },
    height: 200,
    rowHeaderWidth: 10,
    manualColumnResize: [25, 40, , 60, 60],
    filters: false,
    fillHandle: {
      autoInsertRow: false,
    }
  });

  /**
   * Referência para tabela Parametros por Trecho Específico B.
   */
  @ViewChild('tableB', { static: true }) tableBComponent;

  /**
   * Tabela Parametros por Trecho Específico B.
   */
  tableB: Handsontable;

  /**
   * Titulos das colunas na tabela Parametros por Trecho Específico B.
   */
  specificBColHeaders = [
    'Correlação',
    'Parâmetro',
    'Valor',
  ];

  /**
  * Tipos das colunas na tabela Parametros por Trecho Específico B.
  */
  specificBColumnsTypes: any[] = [
    { data: 'correlação', readOnly: true },
    { data: 'parâmetro', readOnly: true },
    { data: 'valor', type: 'numeric' },
  ];

  /**
  * dataset da tabela Parametros por Trecho Específico B
  */
  specificBDataset: { correlação, parâmetro, valor }[] = [];

  /**
  * Opções das tabelas Parametros por Trecho Específico B
  */
  specificBOptions: any = TableOptions.createDefault({
    height: 200,
    rowHeaderWidth: 10,
    manualColumnResize: [],
    filters: false,
    fillHandle: {
      autoInsertRow: false,
    }
  });

  /**
  * Nome do cálculo
  */
  nomeCalculo: { value: string, tooltip: string } = { value: 'Perfis_Calc', tooltip: '' };

  /**
   * Trecho específico
   */
  trechoEspecifico: boolean = false;

  baseCorrelations = [
    { title: 'DTMS', list: [], value: undefined, tooltip: '' },
    { title: 'VCL', list: [], value: undefined, tooltip: '' },
    { title: 'DTC', list: [], value: undefined, tooltip: '' },
    { title: 'DTS', list: [], value: undefined, tooltip: '' },
    { title: 'PORO', list: [], value: undefined, tooltip: '' },
    { title: 'PERM', list: [], value: undefined, tooltip: '' },
    { title: 'RHOB', list: [], value: undefined, tooltip: '' },
  ];

  inputProfiles = [
    { title: 'DTC', list: [], value: undefined, tooltip: '' },
    { title: 'DTS', list: [], value: undefined, tooltip: '' },
    { title: 'GRAY', list: [], value: undefined, tooltip: '' },
    { title: 'RHOB', list: [], value: undefined, tooltip: '' },
    { title: 'VP', list: [], value: undefined, tooltip: '' },
  ];

  /**
  * TOPO
  */
  topo: { value: string, tooltip: string } = { value: '', tooltip: '' };

  /**
   * BASE
   */
  base: { value: string, tooltip: string } = { value: '', tooltip: '' };

  profileSelected = this.baseCorrelations[0];
  correlationSelected: { value: Correlation, tooltip: string } = { value: undefined, tooltip: '' };

  isValid: boolean = true;
  specificValid: boolean = true;

  editando: boolean = false;

  nomesEmUso: string[] = [];

  _loading: boolean = false;
  get loading(): boolean { return this._loading; }
  set loading(val: boolean) {
    this._loading = val;

    val ? this.loaderService.addLoading() : this.loaderService.removeLoading();
  }

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { data: { calculo?} },
    public dialog: DialogService,
    public notybarService: NotybarService,
    private loaderService: LoaderService,

    public correlationService: CorrelationService,
    public calculoService: CalculoService,

    private dataset: DatasetService,
    private treeService: TreeDatasetService,
    private profileDataset: ProfileDatasetService,
    private calculationDataset: CalculationDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    this.nomesEmUso = this.treeService.getUsedNames(this.dataset.currCaseId, ['Cálculos', 'Perfis']);

    let calc;
    if (this.data.data && this.data.data.calculo !== undefined) {
      calc = this.data.data.calculo;
      console.log(calc);
      this.editando = true;

      this.nomeCalculo.value = calc.nome;

      this.nomesEmUso = this.nomesEmUso.filter(el => el !== this.nomeCalculo.value);
    }

    const perfisCalculo = this.profileDataset.getAll(this.dataset.currCaseId).filter(perfil => perfil.podeSerEntradaCálculoPerfis);

    this.inputProfiles.forEach(el => {
      el.list = perfisCalculo.filter(perfil => perfil.mnemonico === el.title);
      if (this.editando) {
        el.value = el.list.find(perfil => calc.perfisEntrada.idPerfis.includes(perfil.id));
      }
    });

    this.correlationService.getAll(this.currCase.id).then(res => {
      this.correlationService.list = res;

      this.baseCorrelations.forEach(el => {
        el.list = res.filter(correlation => correlation.perfilSaída === el.title);
        if (this.editando) {
          el.value = el.list.find(corr => calc.listaNomesCorrelação.includes(corr.nome));
        } else {
          if (el.title === 'RHOB') {
            el.value = el.list.find(corr => corr.nome === 'RHOB_GARDNER');
          } else if (el.title === 'DTC') {
            el.value = el.list.find(corr => corr.nome === 'DTC_VP');
          } else {
            el.value = el.list.find(corr => corr.origem === 'Fixa' && /\b\w+_MECPRO\b/.test(corr.nome));
          }
        }
        this.mudouBase(el.value, el);
      });

      if (this.editando) {
        calc.variáveis.forEach(v => {
          this.baseDataset.find(el => el.correlação === v.correlação && el.parâmetro === v.parâmetro).valor = v.valor;
        });
        this.tableBase.render();

        if (calc.trechosFront !== undefined && calc.trechosFront.length > 0) {
          this.trechoEspecifico = true;
          calc.trechosFront.forEach(t => {
            this.specificADataset.push({
              selected: false,
              tipoPerfil: t.tipoPerfil,
              pmTopo: t.pmTopo,
              pmBase: t.pmBase,
              correlação: t.correlação
            });
            t.listaParametros.forEach(p => {
              this.specificBDataset.push({
                correlação: p.correlação,
                parâmetro: p.parâmetro,
                valor: p.valor,
              });
            });
          });
          this.tableB.render();
          this.tableA.render();
        }
      }

      this.profileSelected = this.baseCorrelations[0];
      this.correlationSelected.value = this.profileSelected.list[0];
    });
  }

  ngAfterViewInit() {
    this.tableBase = this.tableBaseComponent.getHandsontableInstance();
    this.tableA = this.tableAComponent.getHandsontableInstance();
    this.tableB = this.tableBComponent.getHandsontableInstance();
  }

  ngDoCheck() {
    this.isValid = this.canSubmit();
    if (this.trechoEspecifico) {
      this.specificValid = this.canAddLine();
    }
  }

  canSubmit() {
    let isValid = true;
    if (this.nomeCalculo.value === undefined || this.nomeCalculo.value === null || this.nomeCalculo.value === '') {
      this.nomeCalculo.tooltip = 'Nome precisa estar preenchido';
      isValid = false;
    } else if (this.nomesEmUso.includes(this.nomeCalculo.value)) {
      this.nomeCalculo.tooltip = 'Nome em uso';
      isValid = false;
    } else {
      this.nomeCalculo.tooltip = '';
    }

    const dtc = this.inputProfiles.find(el => el.title === 'DTC'), vp = this.inputProfiles.find(el => el.title === 'VP');
    if (dtc.value === undefined && vp.value === undefined) {
      dtc.tooltip = 'Precisa escolher um perfil de DTC ou VP como entrada';
      vp.tooltip = 'Precisa escolher um perfil de DTC ou VP como entrada';
      isValid = false;
    } else {
      dtc.tooltip = '';
      vp.tooltip = '';
    }

    return isValid;
  }

  submit() {
    this.loading = true;
    setTimeout(() => {
      const calculo: CalculoPerfis = {
        idPoço: this.currCase.id,
        nome: this.nomeCalculo.value,
        listaIdPerfisEntrada: this.inputProfiles.filter(el => el.value !== undefined).map(el => el.value.id),
        correlações: this.baseCorrelations.filter(el => el.value !== undefined).map(el => el.value.nome),
        parâmetros: this.baseDataset.map(i => {
          return { correlação: i.correlação, parâmetro: i.parâmetro, valor: i.valor.toString() };
        }),
      };
      if (this.trechoEspecifico) {
        calculo.trechos = this.specificADataset.map(line => {
          return {
            tipoPerfil: line.tipoPerfil,
            pmTopo: line.pmTopo,
            pmBase: line.pmBase,
            correlação: line.correlação,
            listaParametros: this.specificBDataset.filter(el => el.correlação === line.correlação)
          };
        });
      }
      if (this.editando) {
        const calc = this.data.data.calculo;
        calculo.idCálculo = calc.id;
        let teveAlteracao = false;

        if (calc.nome === calculo.nome) {
          calculo.nome = undefined;
        } else {
          teveAlteracao = true;
        }
        calc.perfisEntrada.idPerfis.sort();
        calculo.listaIdPerfisEntrada.sort();
        if (ArrayUtils.equals(calc.perfisEntrada.idPerfis, calculo.listaIdPerfisEntrada)) {
          calculo.listaIdPerfisEntrada = undefined;
        } else {
          teveAlteracao = true;
        }
        calc.listaNomesCorrelação.sort();
        calculo.correlações.sort();
        if (ArrayUtils.equals(calc.listaNomesCorrelação, calculo.correlações)) {
          calculo.correlações = undefined;
        } else {
          teveAlteracao = true;
        }
        const changedParametros = calculo.parâmetros.filter(p =>
          calc.variáveis.findIndex(el => el.correlação === p.correlação && el.parâmetro === p.parâmetro && el.valor === p.valor) === -1);
        if (changedParametros.length === 0) {
          calculo.parâmetros = undefined;
        } else {
          teveAlteracao = true;
        }

        if (this.trechoEspecifico) {
          let changedTrechos = false;
          calculo.trechos.forEach(novo => {
            const antigo = calc.trechosFront.find(el =>
              el.correlação === novo.correlação && el.pmBase === novo.pmBase && el.pmTopo === novo.pmTopo);
            if (antigo !== undefined) {
              const lista = novo.listaParametros.filter(nP =>
                antigo.listaParametros.findIndex(aP => nP.parâmetro === aP.parâmetro && nP.valor.toString() !== aP.valor.toString()) >= 0);
              if (lista.length > 0) {
                changedTrechos = true;
                return;
              }
            } else {
              changedTrechos = true;
              return;
            }
          });
          if (!changedTrechos) {
            calculo.trechos = undefined;
          } else {
            teveAlteracao = true;
          }
        }

        if (!teveAlteracao) {
          this.loading = false;
          this.notybarService.show('Não foram feitas alterações...', 'warning');
          return;
        }
      }
      const metodo: Promise<{ cálculo: Calculo, perfisAlterados?: Perfil[] }> = this.editando ?
        this.calculoService.editarCalculoPerfis(calculo) :
        this.calculoService.criarCalculoPerfis(calculo);

      // console.log('criar calculo', calculo);
      metodo.then(res => {
        // console.log('FOI', res);
        if (this.editando) {
          this.calculationDataset.update(res.cálculo, calculo.idCálculo, res.perfisAlterados);
        } else {
          this.calculationDataset.add(res.cálculo, this.dataset.currCaseId);
        }

        this.loading = false;
        this.closeModal();
      }).catch(() => { this.loading = false; });
    }, 10);
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  atualizarSelecionados() {
    this.lineSelected = this.specificADataset.filter(el => el.selected);
  }

  mudouBase(correlacao, tipo) {
    this.baseDataset = this.baseDataset.filter(el => el.saida !== tipo.title);
    if (correlacao !== undefined && correlacao.variáveis !== undefined && correlacao.variáveis.length > 0) {
      const variaveis = this.correlationService.matchConstVarRegex(this.correlationService.VAR_REGEX, correlacao.expressãoBruta);
      variaveis.forEach(el => {
        this.baseDataset.push({
          saida: correlacao.perfilSaída,
          correlação: correlacao.nome,
          parâmetro: el.name,
          valor: el.value,
        });
      });
    }
    this.tableBase.render();
  }

  perfilChanged() {
    this.correlationSelected.value = this.profileSelected.list[0];
  }

  canAddLine() {
    let isValid = true;
    if (this.topo.value === undefined || this.topo.value === null || this.topo.value === '') {
      this.topo.tooltip = 'Topo precisa estar preenchido';
      isValid = false;
    } else {
      this.topo.tooltip = '';
    }
    if (this.base.value === undefined || this.base.value === null || this.base.value === '') {
      this.base.tooltip = 'Base precisa estar preenchido';
      isValid = false;
    } else {
      this.base.tooltip = '';
    }
    if (parseFloat(this.topo.value) > parseFloat(this.base.value)) {
      this.topo.tooltip = 'Topo precisa ser menor que base';
      this.base.tooltip = 'Topo precisa ser menor que base';
      isValid = false;
    } else {
      const perfilEmUso = this.specificADataset.filter(el => el.tipoPerfil === this.profileSelected.title);
      if (perfilEmUso.length > 0) {
        console.log(perfilEmUso);
        const tooltip = 'Sobreposição detectada.';
        perfilEmUso.forEach(el => {
          if (parseFloat(this.topo.value) < parseFloat(el.pmTopo)) {
            if (parseFloat(this.base.value) > parseFloat(el.pmTopo)) {
              this.topo.tooltip = tooltip;
              this.base.tooltip = tooltip;
              isValid = false;
              return;
            }
          } else if (parseFloat(this.topo.value) > parseFloat(el.pmTopo)) {
            if (parseFloat(this.topo.value) < parseFloat(el.pmBase)) {
              this.topo.tooltip = tooltip;
              this.base.tooltip = tooltip;
              isValid = false;
              return;
            }
          } else {
            this.topo.tooltip = tooltip;
            this.base.tooltip = tooltip;
            isValid = false;
            return;
          }
        });
      }
    }
    if (this.correlationSelected.value === undefined) {
      this.correlationSelected.tooltip = 'Precisa estar preenchido';
      isValid = false;
    } else {
      this.correlationSelected.tooltip = '';
    }

    return isValid;
  }

  addLine() {
    if (this.correlationSelected.value !== undefined &&
      this.correlationSelected.value.variáveis !== undefined &&
      this.correlationSelected.value.variáveis.length > 0) {
      if (this.specificADataset.findIndex(el => el.correlação === this.correlationSelected.value.nome) < 0) {
        const variaveis = this.correlationService.matchConstVarRegex(this.correlationService.VAR_REGEX,
          this.correlationSelected.value.expressãoBruta);
        variaveis.forEach(el => {
          this.specificBDataset.push({
            correlação: this.correlationSelected.value.nome,
            parâmetro: el.name,
            valor: el.value,
          });
        });
        this.tableB.render();
      }
    }
    this.specificADataset.push({
      selected: false,
      tipoPerfil: this.profileSelected.title,
      pmTopo: this.topo.value,
      pmBase: this.base.value,
      correlação: this.correlationSelected.value.nome
    });
    this.tableA.render();
  }

  removeLine() {
    this.lineSelected.forEach(line => {
      const linhaDuplicada = this.specificADataset.findIndex(el => el.correlação === line.correlação && !el.selected) >= 0;
      if (!linhaDuplicada) {
        this.specificBDataset = this.specificBDataset.filter(el => el.correlação !== line.correlação);
      }
    });
    this.specificADataset = this.specificADataset.filter(el => !el.selected);
  }

}
