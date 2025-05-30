import { Component, OnInit, Inject, ViewChild, AfterViewInit, DoCheck } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogService } from '@services/dialog.service';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { CorrelationService } from 'app/repositories/correlation.service';
import { Case } from 'app/repositories/models/case';
import { TableOptions } from '@utils/table-options-default';
import { Calculo, CalculoPropriedadesMecanicas } from 'app/repositories/models/calculo';
import { Observable } from 'rxjs';
import { CalculoService } from 'app/repositories/calculo.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { PropriedadesMecanicasService } from 'app/repositories/propriedades-mecanicas.service';
import { ArrayUtils } from '@utils/array';
import { LISTA_MNEMÔNICOS } from '@utils/perfil/tipo-perfil';
import { Perfil } from 'app/repositories/models/perfil';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { CalculationDatasetService } from '@services/dataset/calculation-dataset.service';
import { TreeDatasetService } from '@services/dataset/state/tree-dataset.service';
import { LoaderService } from '@services/loader.service';

@Component({
  selector: 'app-mechanical-calculations',
  templateUrl: './mechanical-calculations.component.html',
  styleUrls: ['./mechanical-calculations.component.scss']
})
export class MechanicalCalculationsComponent implements OnInit, AfterViewInit, DoCheck {

  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  lineSelected = [];

  /**
   * Tabela Parametros das Correlações Base.
   */
  tableBase: Handsontable;

  /**
  * Referência para tabela Parametros das Correlações Base.
  */
  @ViewChild('tableBase', { static: true }) tableBaseComponent;

  /**
   * Titulos das colunas na tabela de PARAMETROS BASE.
   */
  colHeaders = [
    'Grupo Litológico',
    'UCS (psia)',
    'Coesão (psia)',
    'Âng. Atrito (°)',
    'RESTR (psia)',
  ];

  /**
  * Tipos das colunas na tabela de PARAMETROS BASE.
  */
  columnsTypes: any[] = [
    { data: 'grupoLitológico', readOnly: true },
    { data: 'ucs', type: 'dropdown' },
    { data: 'coesa', type: 'dropdown' },
    { data: 'angat', type: 'dropdown' },
    { data: 'restr', type: 'dropdown' },
  ];

  /**
  * Opções dos PARAMETROS BASE
  */
  optionsParameters: any = TableOptions.createDefault({
    afterChange: (changes) => {
      if (changes) {
        const nRow = changes[0][0], nCol = changes[0][1], oldValue = changes[0][2], newValue = changes[0][3];
        this.atualizarPossibilidadesNaTabela(nRow, nCol, oldValue, newValue);
      }
    },
    width: 650,
    height: 200,
    rowHeaderWidth: 50,
    manualColumnResize: [],
    autoColumnSize: true,
    filters: false,
    allowInvalid: false,
  });

  /**
  * dataset dos PARAMETROS BASE
  */
  datasetBase: any[] = [];

  /**
  * Referência para tabela PARAMETROS TRECHO ESPECÍFICO.
  */
  @ViewChild('tableSpecific', { static: true }) tableSpecificComponent;

  /**
   * Tabela PARAMETROS TRECHO ESPECÍFICO.
   */
  tableSpecific: Handsontable;

  /**
   * Titulos das colunas na tabela de PARAMETROS TRECHO ESPECÍFICO.
   */
  colHeadersSpecificStretch = [
    `<input type='checkbox'' + (this.selectedAll ? 'checked='checked'' : '') + '>`,
    'PM Topo (m)',
    'PM Base (m)',
    'Litologia',
    'UCS (psia)',
    'Coesão (psia)',
    'Âng. Atrito (°)',
    'RESTR (psia)',
    'Biot (psia)',
  ];

  selectedAll = false;

  /**
  * Opções dos PARAMETROS TRECHO ESPECÍFICO.
  */
  optionsSpecificStretchParameters: any = TableOptions.createDefault({
    afterOnCellMouseDown: (event, coords) => {
      if (event.target.type === 'checkbox') {
        // Se clicou no checkbox do título, marca ou desmarca todos as linhas da tabela.
        if (coords.row < 0) {
          this.selectedAll = !event.target.checked;

          this.colHeadersSpecificStretch[0] = `<input type='checkbox' ${this.selectedAll ? 'checked="checked"' : ''}>`;

          for (let i = 0; i < this.tableSpecific.countRows(); i += 1) {
            this.tableSpecific.setDataAtCell(i, 0, this.selectedAll as any);
          }
        }
      }
    },
    afterChange: () => {
      this.atualizarSelecionados();
    },
    height: 430,
    rowHeaderWidth: 10,
    manualColumnResize: [25,],
    autoColumnSize: true,
    filters: false,
  });

  /**
  * Tipos das colunas na tabela de PARAMETROS TRECHO ESPECÍFICO.
  */
  columnsTypesSpecificStretch: any[] = [
    { data: 'selected', type: 'checkbox' },
    { data: 'pmTopo', readOnly: true },
    { data: 'pmBase', readOnly: true },
    { data: 'grupoLitológico', readOnly: true },
    { data: 'ucs', readOnly: true },
    { data: 'coesa', readOnly: true },
    { data: 'angat', readOnly: true },
    { data: 'restr', readOnly: true },
    { data: 'biot', readOnly: true },
  ];

  /**
  * dataset dos PARAMETROS TRECHO ESPECÍFICO
  */
  datasetSpecific: any[] = [];

  /**
  * Nome do cálculo
  */
  nomeCalculo: { value: string, tooltip: string } = { value: 'Prop_Mec_Calc', tooltip: '' };

  /**
   * Trecho específico
   */
  trechoEspecifico: boolean = false;

  baseCorrelations = [
    { title: 'POISS', list: [], value: undefined, tooltip: '' },
    { title: 'YOUNG', list: [], value: undefined, tooltip: '' },
    { title: 'KS', list: [], value: undefined, tooltip: '' },
    { title: 'BIOT', list: [], value: undefined, tooltip: '' },
  ];

  listaMnemonicos = LISTA_MNEMÔNICOS;
  calculados = ['UCS', 'COESA', 'ANGAT', 'RESTR', 'POISS', 'YOUNG', 'KS', 'BIOT'];
  inputProfiles: { title: string, list: Perfil[], value: Perfil, tooltip: string, required: boolean }[] = [];
  requiredInputProfiles: { title: string, list: Perfil[], value: Perfil, tooltip: string, required: boolean }[] = [];

  grupo = { title: 'Grupo Litologico', list: [], value: undefined, tooltip: '' };

  topo: { value: string, tooltip: string } = { value: '0', tooltip: '' };

  base: { value: string, tooltip: string } = { value: '0', tooltip: '' };

  trechoCorrelations = [
    { title: 'UCS', unit: 'psia', list: [], value: undefined, tooltip: '' },
    { title: 'COESA', unit: 'psia', list: [], value: undefined, tooltip: '' },
    { title: 'ANGAT', unit: '°', list: [], value: undefined, tooltip: '' },
    { title: 'RESTR', unit: 'psia', list: [], value: undefined, tooltip: '' },
    { title: 'BIOT', unit: 'psia', list: [], value: undefined, tooltip: '' },
  ];

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
    private loaderService: LoaderService,

    public correlationService: CorrelationService,
    private propmecService: PropriedadesMecanicasService,
    public calculoService: CalculoService,

    private dataset: DatasetService,
    private treeService: TreeDatasetService,
    private profileDataset: ProfileDatasetService,
    private calculationDataset: CalculationDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    let calc;
    if (this.data.data && this.data.data.calculo !== undefined) {
      calc = this.data.data.calculo;
      console.log('teste calc', calc);
      this.editando = true;
    }

    this.nomesEmUso = this.treeService.getUsedNames(this.dataset.currCaseId, ['Cálculos', 'PropriedadesMecânicas']);

    const perfisCalculo = this.profileDataset.getAll(this.dataset.currCaseId);

    this.listaMnemonicos.forEach(mnemonico => {
      this.inputProfiles.push({ title: mnemonico, list: [], value: undefined, tooltip: '', required: false });
    });
    this.inputProfiles.forEach(el => {
      el.list = perfisCalculo.filter(perfil => perfil.mnemonico === el.title);
      if (this.editando) {
        el.value = el.list.find(perfil => calc.perfisEntrada.idPerfis.includes(perfil.id));
      }
    });

    if (this.editando) {
      this.nomeCalculo.value = calc.nome;

      this.nomesEmUso = this.nomesEmUso.filter(el => el !== this.nomeCalculo.value);

      calc.regiõesFront.forEach((el, i) => {
        const linhaDataset = {
          grupoLitológico: el.grupoLitológico,
          ucs: el.ucsCorrelação,
          coesa: el.coesãoCorrelação,
          angat: el.angatCorrelação,
          restr: el.restrCorrelação,
        };

        this.grupo.list.push(el.grupoLitológico);
        this.datasetBase.push(linhaDataset);
        this.atualizarPossibilidadesNaTabela(i, undefined, undefined, true);
      });

      if (calc.trechos) {
        this.trechoEspecifico = true;
        this.datasetSpecific = calc.trechos.map(t => {
          return {
            selected: false,
            pmTopo: t.topo,
            pmBase: t.base,
            grupoLitológico: t.grupoLitológico.nome,
            ucs: t.ucs.nome,
            coesa: t.coesa.nome,
            angat: t.angat.nome,
            restr: t.restr.nome,
            biot: t.biot.nome,
          }
        });
      }
    } else {
      this.propmecService.obterCorrelacoesPorGruposLito(this.currCase.id).then(res => {
        res.correlaçõesPossíveis.possibleCorrs.forEach(el => {
          const listaUcs = ArrayUtils.removeDups(el.ucsPossibleCorrelations.map(corr => corr.nome));
          const listaCoesa = ArrayUtils.removeDups(el.coesaPossibleCorrelations.map(corr => corr.nome));
          const listaAngat = ArrayUtils.removeDups(el.angatPossibleCorrelations.map(corr => corr.nome));
          const listaRestr = ArrayUtils.removeDups(el.restrPossibleCorrelations.map(corr => corr.nome));

          const linhaDataset = {
            grupoLitológico: el.grupoLitologico.nome,
            ucs: listaUcs.length === 1 ? listaUcs[0] : undefined,
            coesa: listaCoesa.length === 1 ? listaCoesa[0] : undefined,
            angat: listaAngat.length === 1 ? listaAngat[0] : undefined,
            restr: listaRestr.length === 1 ? listaRestr[0] : undefined,
          };

          this.grupo.list.push(el.grupoLitologico.nome);

          this.datasetBase.push(linhaDataset);
          this.tableBase.setCellMetaObject(this.datasetBase.length - 1, this.tableBase.propToCol('ucs'), { source: listaUcs });
          this.tableBase.setCellMetaObject(this.datasetBase.length - 1, this.tableBase.propToCol('coesa'), { source: listaCoesa });
          this.tableBase.setCellMetaObject(this.datasetBase.length - 1, this.tableBase.propToCol('angat'), { source: listaAngat });
          this.tableBase.setCellMetaObject(this.datasetBase.length - 1, this.tableBase.propToCol('restr'), { source: listaRestr });
        });
        this.tableBase.render();

        this.atualizarTiposPerfisEntrada();
      });
    }

    this.correlationService.getAll(this.currCase.id).then(res => {
      this.correlationService.list = res;

      this.baseCorrelations.forEach(el => {
        el.list = res.filter(correlation => correlation.perfilSaída === el.title);
        if (this.editando) {
          el.value = el.list.find(corr => calc.listaNomesCorrelação.includes(corr.nome));
        } else {
          if (el.title === 'BIOT') {
            el.list = el.list.filter(corr => corr.nome !== 'BIOT_PRASAD_ET_AL_1' && corr.nome !== 'BIOT_PRASAD_ET_AL_2');
            el.value = el.list.find(corr => corr.nome === 'BIOT_REGIME_ELÁSTICO');
          } else {
            el.value = el.list.find(corr => corr.origem === 'Fixa' && /\b\w+_MECPRO\b/.test(corr.nome));
          }
        }
      });

      this.atualizarTiposPerfisEntrada();
    });

  }

  ngAfterViewInit() {
    this.tableBase = this.tableBaseComponent.getHandsontableInstance();
    this.tableSpecific = this.tableSpecificComponent.getHandsontableInstance();
  }

  ngDoCheck() {
    this.isValid = this.canSubmit();
    if (this.trechoEspecifico) {
      this.specificValid = this.canAddLine();
    }
  }

  submit() {
    this.loading = true;
    setTimeout(() => {
      const calculo: CalculoPropriedadesMecanicas = {
        nome: this.nomeCalculo.value,
        idPoço: this.currCase.id,
        correlações: this.baseCorrelations.map(el => el.value.nome),
        listaIdPerfisEntrada: this.requiredInputProfiles.map(el => el.value.id),
        regiões: this.datasetBase
      };

      if (this.trechoEspecifico) {
        calculo.trechos = this.datasetSpecific;
      }

      console.log('PropMEC', calculo);

      let metodo: Promise<{ cálculo: Calculo, perfisAlterados?: Perfil[] }>;

      if (this.editando) {
        calculo.idCálculo = this.data.data.calculo.id;
        metodo = this.calculoService.editarCalculoPropriedadesMecanicas(calculo);
      } else {
        metodo = this.calculoService.criarCalculoPropriedadesMecanicas(calculo);
      }

      metodo.then(res => {
        // console.log('res', res);
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
    this.baseCorrelations.forEach(el => {
      if (el.value === undefined || el.value === null) {
        el.tooltip = 'Precisa escolher uma correlação';
        isValid = false;
      } else {
        el.tooltip = '';
      }
    });
    this.requiredInputProfiles.forEach(el => {
      if (el.value === undefined || el.value === null) {
        el.tooltip = 'Precisa escolher um perfil';
        isValid = false;
      } else if (el.value.pontos.length === 0) {
        el.tooltip = `Faltam dados\nO perfil selecionado deve possuir dados.`;
        isValid = false;
      } else {
        el.tooltip = '';
      }
    });

    return isValid;
  }

  canAddLine() {
    let isValid = true;
    if (this.grupo.value === undefined || this.grupo.value === null) {
      this.grupo.tooltip = 'Precisa escolher um grupo litológico.';
      isValid = false;
    } else {
      this.grupo.tooltip = '';
    }
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
    this.trechoCorrelations.forEach(el => {
      if (el.value === undefined || el.value === null) {
        el.tooltip = 'Precisa escolher uma correlação';
        isValid = false;
      } else {
        el.tooltip = '';
      }
    });
    let sedimentos;
    if (this.currCase.dadosGerais.geometria.categoriaPoço === 'OnShore') {
      sedimentos = this.currCase.dadosGerais.geometria.mesaRotativa + this.currCase.dadosGerais.geometria.onShore.alturaDeAntePoço;
    } else {
      sedimentos = this.currCase.dadosGerais.geometria.mesaRotativa + this.currCase.dadosGerais.geometria.offShore.laminaDagua;
    }
    if (parseFloat(this.topo.value) > parseFloat(this.base.value)) {
      this.topo.tooltip = 'Topo precisa ser menor que base';
      this.base.tooltip = 'Topo precisa ser menor que base';
      isValid = false;
    } else if (parseFloat(this.topo.value) < sedimentos || parseFloat(this.base.value) > this.currCase.trajetória.últimoPonto.pm.valor) {
      this.topo.tooltip = `Topo não pode ser menor que superfície de sedimentos (${sedimentos})`;
      this.base.tooltip = `Base não pode ser maior que ultimo ponto da trajetória (${this.currCase.trajetória.últimoPonto.pm.valor})`;
      isValid = false;
    } else {
      const litoEmUso = this.datasetSpecific.filter(el => el.grupoLitológico === this.grupo.value);
      if (litoEmUso.length > 0) {
        // console.log(litoEmUso, this.topo, this.base);
        const tooltip = 'Sobreposição detectada.';
        litoEmUso.forEach(el => {
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

    return isValid;
  }

  addLine() {
    const newLine = {
      selected: false,
      pmTopo: this.topo.value,
      pmBase: this.base.value,
      grupoLitológico: this.grupo.value,
    };

    this.trechoCorrelations.forEach(el => {
      if (el.value) {
        newLine[el.title.toLowerCase()] = el.value.nome;
      }
    });

    this.datasetSpecific.push(newLine);
    this.tableSpecific.render();
    this.atualizarTiposPerfisEntrada();
  }

  atualizarSelecionados() {
    this.lineSelected = this.datasetSpecific.filter(el => el.selected);
  }

  removeLine() {
    this.lineSelected.forEach(line => {
      const linhaDuplicada = this.datasetSpecific.findIndex(el => el.correlação === line.correlação && !el.selected) >= 0;
      if (!linhaDuplicada) {
        this.datasetSpecific = this.datasetSpecific.filter(el => el.correlação !== line.correlação);
      }
    });
    this.datasetSpecific = this.datasetSpecific.filter(el => !el.selected);
    this.atualizarTiposPerfisEntrada();
  }

  atualizarTrechoEspecificoCorrs(grupo: boolean, valor?) {
    const ucs = this.trechoCorrelations.find(el => el.title === 'UCS'),
      coesa = this.trechoCorrelations.find(el => el.title === 'COESA'),
      angat = this.trechoCorrelations.find(el => el.title === 'ANGAT'),
      restr = this.trechoCorrelations.find(el => el.title === 'RESTR'),
      biot = this.trechoCorrelations.find(el => el.title === 'BIOT');

    let metodo: Promise<any>;
    if (grupo || valor === undefined) {
      metodo = this.propmecService.obterCorrelacoesPossiveis(this.currCase.id, this.grupo.value);
    } else {
      const ucsNome = ucs.value ? ucs.value.nome : undefined,
        coesaNome = coesa.value ? coesa.value.nome : undefined,
        angatNome = angat.value ? angat.value.nome : undefined,
        restrNome = restr.value ? restr.value.nome : undefined,
        biotNome = biot.value ? biot.value.nome : undefined;

      metodo = this.propmecService.obterCorrelacoesPossiveis(
        this.currCase.id, this.grupo.value, ucsNome, coesaNome, angatNome, restrNome, biotNome);
    }

    metodo.then(res => {
      const corrs = res.correlaçõesPossíveis;

      ucs.list = ArrayUtils.removeDups(corrs.ucsPossibleCorrelations);
      coesa.list = ArrayUtils.removeDups(corrs.coesaPossibleCorrelations);
      angat.list = ArrayUtils.removeDups(corrs.angatPossibleCorrelations);
      restr.list = ArrayUtils.removeDups(corrs.restrPossibleCorrelations);
      if (corrs.biotPossibleCorrelations) {
        biot.list = ArrayUtils.removeDups(corrs.biotPossibleCorrelations);
      }

      this.trechoCorrelations.forEach(el => {
        if (el.list.length === 1) {
          el.value = el.list[0];
        } else if (grupo) {
          el.value = undefined;
        } else if (el.value !== undefined) {
          el.value = el.list.find(corr => corr.nome === el.value.nome);
        }
      });
    });
  }

  atualizarTiposPerfisEntrada() {
    const pegarPerfisEntradaLinha = (linha) => {
      let ucs: any = this.correlationService.list.find(corr => corr.nome === linha.ucs),
        coesa: any = this.correlationService.list.find(corr => corr.nome === linha.coesa),
        angat: any = this.correlationService.list.find(corr => corr.nome === linha.angat),
        restr: any = this.correlationService.list.find(corr => corr.nome === linha.restr);

      ucs = ucs ? ucs.perfisEntrada : undefined;
      coesa = coesa ? coesa.perfisEntrada : undefined;
      angat = angat ? angat.perfisEntrada : undefined;
      restr = restr ? restr.perfisEntrada : undefined;

      return ArrayUtils.mergeWithoutDups(ucs, coesa, angat, restr);
    }

    let tiposEntrada: string[] = [];
    this.datasetBase.forEach(linha => {
      tiposEntrada = ArrayUtils.mergeWithoutDups(tiposEntrada, pegarPerfisEntradaLinha(linha));
    });

    if (this.trechoEspecifico) {
      this.datasetSpecific.forEach(linha => {
        tiposEntrada = ArrayUtils.mergeWithoutDups(tiposEntrada, pegarPerfisEntradaLinha(linha));
      });
    }

    this.baseCorrelations.forEach(input => {
      if (input.value) {
        tiposEntrada = ArrayUtils.mergeWithoutDups(tiposEntrada, input.value.perfisEntrada);
      }
    });

    this.inputProfiles.forEach(el => {
      el.required = false;
      if (tiposEntrada.includes(el.title) && !this.calculados.includes(el.title)) {
        el.required = true;
      }
    });
    // console.log(this.datasetBase, tiposEntrada);
    this.requiredInputProfiles = this.inputProfiles.filter(el => el.required);

    if (!this.editando) {
      this.requiredInputProfiles.forEach(el => {
        if (el.list.length === 1) {
          el.value = el.list[0];
        }
      });
    }
  }

  atualizarPossibilidadesNaTabela(nRow, nCol, oldValue, newValue, repeating?) {
    // console.log(nRow, nCol, newValue);
    const l = this.datasetBase[nRow];
    if (!repeating && oldValue !== undefined && l.ucs && l.coesa && l.angat && l.restr) {
      l.ucs = undefined;
      l.coesa = undefined;
      l.angat = undefined;
      l.restr = undefined;

      l[nCol] = newValue;
    }
    this.propmecService.obterCorrelacoesPossiveis(
      this.currCase.id, l.grupoLitológico, l.ucs, l.coesa, l.angat, l.restr).then(res => {
        const corrs = res.correlaçõesPossíveis;

        const listaUcs = ArrayUtils.removeDups(corrs.ucsPossibleCorrelations.map(corr => corr.nome));
        const listaCoesa = ArrayUtils.removeDups(corrs.coesaPossibleCorrelations.map(corr => corr.nome));
        const listaAngat = ArrayUtils.removeDups(corrs.angatPossibleCorrelations.map(corr => corr.nome));
        const listaRestr = ArrayUtils.removeDups(corrs.restrPossibleCorrelations.map(corr => corr.nome));

        if (!l.ucs || !l.coesa || !l.angat || !l.restr) {
          l.grupoLitológico = corrs.grupoLitologico.nome;
          l.ucs = listaUcs.length === 1 ? listaUcs[0] : undefined;
          l.coesa = listaCoesa.length === 1 ? listaCoesa[0] : undefined;
          l.angat = listaAngat.length === 1 ? listaAngat[0] : undefined;
          l.restr = listaRestr.length === 1 ? listaRestr[0] : undefined;

          /* Para não preencher automaticamente uma celula que o usuário apagou */
          if (!newValue) {
            l[nCol] = newValue;
          }

          if (listaUcs.length === 1 && listaCoesa.length === 1 && listaAngat.length === 1 && listaRestr.length === 1) {
            // repetir requisição para pegar todas as opções
            this.atualizarPossibilidadesNaTabela(nRow, nCol, newValue, newValue, true);
            return;
          }
        }

        this.tableBase.setCellMetaObject(nRow, this.tableBase.propToCol('ucs'), { source: listaUcs });
        this.tableBase.setCellMetaObject(nRow, this.tableBase.propToCol('coesa'), { source: listaCoesa });
        this.tableBase.setCellMetaObject(nRow, this.tableBase.propToCol('angat'), { source: listaAngat });
        this.tableBase.setCellMetaObject(nRow, this.tableBase.propToCol('restr'), { source: listaRestr });

        this.tableBase.render();

        this.atualizarTiposPerfisEntrada();
      });
  }

}
