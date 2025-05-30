import { AfterViewInit, Component, DoCheck, ElementRef, Inject, OnInit, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { DatasetService } from '@services/dataset/dataset.service';
import { Observable, Subscription } from 'rxjs';
import { PoroelasticParametersComponent } from './poroelastic-parameters/poroelastic-parameters.component';
import { TrechoEspecificoComponent } from './trecho-especifico/trecho-especifico.component';
import { DialogService } from '@services/dialog.service';
import { Case } from 'app/repositories/models/case';
import { Perfil } from 'app/repositories/models/perfil';
import { CalculoService } from 'app/repositories/calculo.service';
import { Calculo, CalculoGradiente, ParametrosPoroElastico } from 'app/repositories/models/calculo';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { CalculationDatasetService } from '@services/dataset/calculation-dataset.service';
import { TreeDatasetService } from '@services/dataset/state/tree-dataset.service';
import { NumberUtils } from '@utils/number';
import { TipoRocha } from 'app/repositories/models/litologia';
import { NoConvergenceComponent } from './no-convergence/no-convergence.component';
import { LoaderService } from '@services/loader.service';

@Component({
  selector: 'app-gradient-calculation',
  templateUrl: './gradient-calculation.component.html',
  styleUrls: ['./gradient-calculation.component.scss']
})
export class GradientCalculationComponent implements OnInit, AfterViewInit, DoCheck {

  /**
   * A lista de casos de estudo aberta atualmente
   */
  openedCases: Case[];

  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  isValid: boolean = true;

  editando: boolean = false;

  /**
   * Subscription for listening to responses when opening a file.
   */
  private $fileResponseLoaded: Subscription;

  /**
  * Nome do cálculo
  */
  nomeCalculo: { value: string, tooltip: string } = { value: 'JO_Calc', tooltip: '' };

  malha = {
    rextRint: { value: '4', tooltip: '', min: 1.0, max: 20.0 },
    numAng: { value: '20', tooltip: '', min: 1, max: 100, int: true },
    numRad: { value: '20', tooltip: '', min: 1, max: 100, int: true },
    rmaxRmin: { value: '100', tooltip: '', min: 1.0, max: 100.0 },
  };
  @ViewChild('canvas', { static: true }) canvas: ElementRef<HTMLCanvasElement>;
  ctx: CanvasRenderingContext2D;

  /**
   * Lista de perfil no caso de estudo aberto
   */
  perfisDoPoco: Perfil[];

  /**
  * Perfis de entrada
  */
  perfisEntrada = [
    { title: 'ANGAT', list: [], value: undefined, tooltip: '' },
    { title: 'AZTHmin', list: [], value: undefined, tooltip: '' },
    { title: 'BIOT', list: [], value: undefined, tooltip: '' },
    { title: 'COESA', list: [], value: undefined, tooltip: '' },
    { title: 'DIAM_BROCA', list: [], value: undefined, tooltip: '' },
    { title: 'GPORO', list: [], value: undefined, tooltip: '' },
    { title: 'GSOBR', list: [], value: undefined, tooltip: '' },
    { title: 'KS', list: [], value: undefined, tooltip: '' },
    { title: 'PERM', list: [], value: undefined, tooltip: '' },
    { title: 'POISS', list: [], value: undefined, tooltip: '' },
    { title: 'PORO', list: [], value: undefined, tooltip: '' },
    { title: 'RESTR', list: [], value: undefined, tooltip: '' },
    { title: 'THORmax', list: [], value: undefined, tooltip: '' },
    { title: 'THORmin', list: [], value: undefined, tooltip: '' },
    { title: 'UCS', list: [], value: undefined, tooltip: '' },
    { title: 'YOUNG', list: [], value: undefined, tooltip: '' },

  ];

  /**
   * Tipos de modelos
   */
  tipoModelos = [
    { label: 'Elástico', name: 'Elastico' },
    { label: 'Poroelástico', name: 'Poroelastico' },
  ];
  modeloSelecionado: { value, tooltip: string } = { value: this.tipoModelos[0], tooltip: '' };

  /**
  * Area Plastificada
  */
  areaPlastificada: { value: string, tooltip: string } = { value: '0', tooltip: '' };

  /**
   * Tipos de ruptura
   */
  tipoCriterio = [
    { label: 'Mohr-Coulomb', back: 'MohrCoulomb' },
    { label: 'Lade-Ewy', back: 'Lade' },
    { label: 'Drucker - Pragger Interno', back: 'DruckerPraggerInternal' },
    { label: 'Drucker - Pragger Central', back: 'DruckerPraggerMiddle' },
    { label: 'Drucker - Pragger Externo', back: 'DruckerPraggerExternal' },
  ];
  criterioSelecionado: { value, tooltip: string } = { value: this.tipoCriterio[0], tooltip: '' };

  /**
   * Tempo min
   */
  tempoMin: { value: string, tooltip: string } = { value: '60', tooltip: '' };

  checkboxes = [
    { label: 'Fluido Penetrante', back: 'fluidoPenetrante', value: false },
    { label: 'Habilitar filtro automático', back: 'habilitarFiltroAutomatico', value: false },
    { label: 'Incluir efeitos Físico-Químicos', back: 'incluirEfeitosFísicosQuímicos', value: false, soPoroelastico: true },
    { label: 'Incluir efeitos Térmicos', back: 'incluirEfeitosTérmicos', value: false, soPoroelastico: true },
    { label: 'Calcular fraturas e colapsos separadamente', back: 'calcularFraturasColapsosSeparadamente', value: false },
  ];

  parametros: ParametrosPoroElastico = {
    kf: 2500,
    viscosidade: 0.3,
    coeficienteReflexão: 0.4,
    coeficienteInchamento: 10.5,
    coeficienteDifusãoSoluto: Number('1.6e-10'),
    densidadeFluidoFormação: 1000,
    temperaturaFormação: 60,
    temperaturaFormaçãoFisicoQuimica: 60,
    concentraçãoSolFluidoPerfuração: 200,
    concentraçãoSolutoRocha: 25.44,
    tipoSal: 'NaCl',
    coeficienteDissociaçãoSoluto: 1.8464,
    massaMolarSoluto: 0.00585,
    expansãoTérmicaVolumeFluidoPoros: 0.000303,
    temperaturaPoço: 60,
    propriedadeTérmicaTemperaturaFormação: 60,
    difusidadeTérmica: Number('5.5e-07'),
    expansãoTérmicaRocha: Number('8.87e-06'),
    litologias: [],
  };

  nomesEmUso: string[] = [];

  _loading: boolean = false;
  get loading(): boolean { return this._loading; }
  set loading(val: boolean) {
    this._loading = val;
    val ? this.loaderService.addLoading() : this.loaderService.removeLoading();
  }

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { data: { calculo?: CalculoGradiente } },
    public dialog: DialogService,
    private loaderService: LoaderService,

    public calculoService: CalculoService,

    private dataset: DatasetService,
    private treeService: TreeDatasetService,
    private profileDataset: ProfileDatasetService,
    private calculationDataset: CalculationDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    this.perfisDoPoco = this.profileDataset.getAll(this.dataset.currCaseId).sort((a, b) => a.nome.localeCompare(b.nome));

    this.parametros.litologias = this.getTiposRochaDoPoco().map(el => el.mnemonico);

    this.nomesEmUso = this.treeService.getUsedNames(this.dataset.currCaseId, ['Cálculos', 'Gradientes']);

    let calc;
    if (this.data.data && this.data.data.calculo !== undefined) {
      console.log(this.data.data.calculo);
      calc = this.data.data.calculo;
      this.editando = true;

      this.nomeCalculo.value = calc.nome;

      this.nomesEmUso = this.nomesEmUso.filter(el => el !== this.nomeCalculo.value);

      this.malha.numAng.value = String(calc.dadosmalha.anguloDivisao / 2);
      this.malha.numRad.value = calc.dadosmalha.raioDivisao;
      this.malha.rmaxRmin.value = calc.dadosmalha.anguloMaxMinIncremento;
      this.malha.rextRint.value = calc.dadosmalha.anguloInternoPorExterno;

      this.areaPlastificada.value = calc.entradaColapsos.areaPlastificada;
      this.criterioSelecionado.value = this.tipoCriterio.find(el => el.back === calc.entradaColapsos.tipoCritérioRuptura);
      this.tempoMin.value = calc.entradaColapsos.tempo;

      this.checkboxes.forEach(el => {
        el.value = calc.entradaColapsos[el.back];
      });

      if (calc.entradaColapsos.poroElastico) {
        this.parametros = calc.entradaColapsos.poroElastico;
      }
      if (calc.entradaColapsos.tempo) {
        this.modeloSelecionado.value = this.tipoModelos[1];
      }
    }

    this.perfisEntrada.forEach(p => {
      p.list = this.perfisDoPoco.filter(el => el.mnemonico === p.title);
      if (p.title === 'GPORO') {
        p.list = p.list.concat(this.perfisDoPoco.filter(el => el.mnemonico === 'GPPI'));
      }
      if (this.editando) {
        p.value = p.list.find(el => calc.perfisEntrada.idPerfis.includes(el.id));
      }
      if (p.value === undefined) {
        p.value = p.list[0];
      }
    });
  }

  ngAfterViewInit(): void {
    let ctx = this.canvas.nativeElement.getContext('2d');
    if (ctx !== null) {
      this.ctx = ctx;
      this.ctx.transform(1, 0, 0, -1, 0, this.canvas.nativeElement.height)
    }
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  ngDoCheck() {
    this.isValid = this.podeAtualizarMalha();
    if (this.isValid && this.ctx !== undefined) {
      this.atualizarMalha();
    }
    this.isValid = this.canSubmit() && this.isValid;

  }

  podeAtualizarMalha() {
    let isValid = true;

    Object.keys(this.malha).forEach(campo => {
      if (this.malha[campo].value === undefined || this.malha[campo].value === null || this.malha[campo].value === '') {
        this.malha[campo].tooltip = 'Precisa estar preenchido';
        isValid = false;
      } else if (!NumberUtils.isNumber(this.malha[campo].value)) {
        this.malha[campo].tooltip = 'Precisa ser um número';
        isValid = false;
      } else if (this.malha[campo].int && !Number.isInteger(Number(this.malha[campo].value))) {
        this.malha[campo].tooltip = 'Precisa ser um inteiro';
        isValid = false;
      } else if (Number(this.malha[campo].value) <= Number(this.malha[campo].min) || Number(this.malha[campo].value) > Number(this.malha[campo].max)) {
        this.malha[campo].tooltip = `Precisa ser > ${this.malha[campo].min} e <= ${this.malha[campo].max}`;
        isValid = false;
      } else {
        this.malha[campo].tooltip = '';
      }
    });

    return isValid;
  }

  atualizarMalha() {
    let ctx = this.ctx;

    var w = this.canvas.nativeElement.width;
    var h = this.canvas.nativeElement.height;

    ctx.clearRect(0, 0, w, h);
    // ctx.fillStyle = 'rgba(200, 200, 200, 1.0)';
    // ctx.fillRect(0, 0, w, h);
    ctx.fillStyle = 'rgba(0, 0, 0, 1.0)';

    let pontos = this.gerarPontos(w);
    // console.log(pontos);
    for (let i = 0; i < pontos.length; i++) {
      ctx.beginPath();
      ctx.moveTo(pontos[i][0][0], pontos[i][0][1]);
      for (let j = 0; j < pontos[i].length; j++) {
        ctx.lineTo(pontos[i][j][0], pontos[i][j][1]);
      }
      ctx.stroke();
    }
    for (let j = 0; j < pontos[0].length; j++) {
      ctx.beginPath();
      ctx.moveTo(pontos[0][j][0], pontos[0][j][1]);
      for (let i = 0; i < pontos.length; i++) {
        ctx.lineTo(pontos[i][j][0], pontos[i][j][1]);
      }
      ctx.stroke();
    }
    for (let z = 1; z < pontos[0].length; z++) {
      ctx.beginPath();
      ctx.moveTo(pontos[pontos.length - 1][z][0], pontos[pontos.length - 1][z][1]);
      for (let i = pontos.length - 1, j = z; i >= 0 && j >= 0; i--, j--) {
        ctx.lineTo(pontos[i][j][0], pontos[i][j][1]);
      }
      ctx.stroke();
    }
    for (let z = pontos.length - 2; z > 0; z--) {
      ctx.beginPath();
      ctx.moveTo(pontos[z][pontos[0].length - 1][0], pontos[z][pontos[0].length - 1][1]);
      for (let i = z, j = pontos[0].length - 1; i >= 0 && j >= 0; i--, j--) {
        ctx.lineTo(pontos[i][j][0], pontos[i][j][1]);
      }
      ctx.stroke();
    }
  }

  gerarPontos(raio: number) {
    let raioExterno: number = raio;
    let raioInterno: number = Math.pow(Number(this.malha.rextRint.value), -1) * raioExterno;

    let numeroDeAngulos: number = Number(this.malha.numAng.value) + 1;
    let numeroDeRaios: number = Number(this.malha.numRad.value) + 1;

    let incrementoMinimoDeRaio: number = 2 / (1 + Number(this.malha.rmaxRmin.value)) * (raioExterno - raioInterno) / Number(this.malha.numRad.value);

    let incrementoMaximoDeRaio: number = incrementoMinimoDeRaio * Number(this.malha.rmaxRmin.value);
    let d: number = (incrementoMaximoDeRaio - incrementoMinimoDeRaio) / (Number(this.malha.numRad.value) - 1);
    let iTheta: number = 90 / Number(this.malha.numAng.value);

    let builder: any[] = [];

    for (let i = 0; i < numeroDeAngulos; i++) {
      //angulo em radianos
      let angulo = i * iTheta * Math.PI / 180.0;

      let raio = raioInterno;
      let incrementoRaio = incrementoMinimoDeRaio;

      builder.push([]);

      for (let j = 0; j < numeroDeRaios; j++) {
        let x = raio * Math.cos(angulo);
        let y = raio * Math.sin(angulo);

        builder[i].push([x, y, angulo, raio]);

        raio += incrementoRaio;
        incrementoRaio += d;
      }
    }
    return builder;
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

    this.perfisEntrada.forEach(p => {
      if (p.value === undefined) {
        p.tooltip = 'Precisa escolher um perfil';
        isValid = false;
      } else {
        p.tooltip = '';
      }
    });

    if (this.areaPlastificada.value === undefined || this.areaPlastificada.value === null || this.areaPlastificada.value === '') {
      this.areaPlastificada.tooltip = 'Precisa estar preenchido';
      isValid = false;
    } else if (!NumberUtils.isNumber(this.areaPlastificada.value)) {
      this.areaPlastificada.tooltip = 'Precisa ser um número';
      isValid = false;
    } else if (Number(this.areaPlastificada.value) < 0 || Number(this.areaPlastificada.value) > 100) {
      this.areaPlastificada.tooltip = 'Precisa ser >= 0 e <= 100';
      isValid = false;
    } else {
      this.areaPlastificada.tooltip = '';
    }

    if (this.modeloSelecionado.value.name === 'Poroelastico') {
      if (this.tempoMin.value === undefined || this.tempoMin.value === null || this.tempoMin.value === '') {
        this.tempoMin.tooltip = 'Precisa estar preenchido';
        isValid = false;
      } else if (!NumberUtils.isNumber(this.tempoMin.value)) {
        this.tempoMin.tooltip = 'Precisa ser um número';
        isValid = false;
      } else if (Number(this.tempoMin.value) < 0) {
        this.tempoMin.tooltip = 'Precisa ser >= 0';
        isValid = false;
      } else {
        this.tempoMin.tooltip = '';
      }
    } else {
      this.tempoMin.tooltip = '';
    }

    return isValid
  }

  submit() {
    this.loading = true;
    setTimeout(() => {
      const checks: boolean = this.modeloSelecionado.value.name === 'Poroelastico' && this.checkboxes[0].value;

      const calculo: CalculoGradiente = {
        idCálculo: this.editando ? this.data.data.calculo.id : undefined,
        nomeCálculo: this.nomeCalculo.value,
        idPoço: this.currCase.id,
        tipoModelo: this.modeloSelecionado.value.label,
        areaPlastificada: Number(this.areaPlastificada.value),
        tipoCritérioRuptura: this.criterioSelecionado.value.label,
        tempo: this.modeloSelecionado.value.name === 'Poroelastico' ? Number(this.tempoMin.value) : undefined,

        fluidoPenetrante: this.checkboxes[0].value,
        habilitarFiltroAutomatico: this.checkboxes[1].value,
        incluirEfeitosFísicosQuímicos: checks ? this.checkboxes[2].value : undefined,
        incluirEfeitosTérmicos: checks ? this.checkboxes[3].value : undefined,
        calcularFraturasColapsosSeparadamente: this.checkboxes[4].value,

        malhaRextRint: Number(this.malha.rextRint.value),
        malhaRMaxRMin: Number(this.malha.rmaxRmin.value),
        malhaNunDivAng: Number(this.malha.numAng.value),
        malhaNunDivRad: Number(this.malha.numRad.value),

        parâmetrosPoroElástico: this.parametros,
      };

      this.perfisEntrada.forEach(el => {
        calculo[`perfil${el.title}Id`] = el.value.id
      });

      const metodo: Promise<{ cálculo: Calculo, perfisAlterados?: Perfil[] }> = this.editando ?
        this.calculoService.editarCalculoGradiente(calculo) :
        this.calculoService.criarCalculoGradiente(calculo);

      // console.log('criar calculo', calculo);
      metodo.then(res => {
        // console.log('FOI', res);
        if (this.editando) {
          this.calculationDataset.update(res.cálculo, calculo.idCálculo, res.perfisAlterados);
        } else {
          this.calculationDataset.add(res.cálculo, this.dataset.currCaseId);
        }
        if (res.cálculo['informações'] && res.cálculo['informações'].length > 0) {
          this.dialog.openPageDialog(
            NoConvergenceComponent,
            { minHeight: 0, minWidth: 900 },
            { info: res.cálculo['informações'] }
          );
        }

        this.loading = false;
        this.closeModal();
      }).catch(() => { this.loading = false; });
    }, 10);
  }

  salvarParametros(parametros: ParametrosPoroElastico) {
    this.parametros = parametros;
  }

  getTiposRochaDoPoco() {
    const tipoLitologia: string = this.currCase.tipoPoço === 'Projeto' ? 'Adaptada' : 'Interpretada';
    const lito = this.currCase.litologias.find(el => el.classificação.nome === tipoLitologia);
    const tipoRochaAdicionado: any = {};
    const tiposRocha: TipoRocha[] = [];

    if (lito && lito.pontos) {
      lito.pontos.forEach(el => {
        if (el.tipoRocha.grupo.nome === 'Argilosas' && !tipoRochaAdicionado[el.tipoRocha.mnemonico]) {
          tipoRochaAdicionado[el.tipoRocha.mnemonico] = true;
          tiposRocha.push(el.tipoRocha);
        }
      });
    }

    return tiposRocha;
  }

  openWindowParametrosPoroelasticos() {
    this.dialog.openPageDialog(
      PoroelasticParametersComponent,
      { minHeight: 300, minWidth: 300 },
      { context: this, caseId: this.currCase.id, parametros: this.parametros }
    );
  }

  openWindowTrechoEspecifico() {
    this.dialog.openPageDialog(
      TrechoEspecificoComponent,
      { minHeight: 300, minWidth: 450 },
      { case: this.currCase }
    );
  }

}
