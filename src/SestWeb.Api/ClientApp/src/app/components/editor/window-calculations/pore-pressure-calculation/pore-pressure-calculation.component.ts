import { Component, DoCheck, Inject, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { DialogService } from '@services/dialog.service';
import { PpReservatorioComponent } from './pp-reservatorio/pp-reservatorio.component';
import { DatasetService } from '@services/dataset/dataset.service';
import { Case } from 'app/repositories/models/case';
import { Perfil } from 'app/repositories/models/perfil';
import { isFiltro } from 'app/repositories/models/filtro';
import { Calculo, CalculoGPP, CalculoPP, CalculoPPH, Reservatorio } from 'app/repositories/models/calculo';
import { CalculoService } from 'app/repositories/calculo.service';
import { NotybarService } from '@services/notybar.service';
import { TreeDatasetService } from '@services/dataset/state/tree-dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { CalculationDatasetService } from '@services/dataset/calculation-dataset.service';
import { Observable } from 'rxjs';
import { LoaderService } from '@services/loader.service';

@Component({
  selector: 'app-pore-pressure-calculation',
  templateUrl: './pore-pressure-calculation.component.html',
  styleUrls: ['./pore-pressure-calculation.component.scss']
})
export class PorePressureCalculationComponent implements OnInit, DoCheck {

  /**
  * O caso de estudo aberto atualmente.
  */
  currCase: Case;

  isValid: boolean = true;

  /**
   * Tipos de Calculos de pressão de poros
   */
  calculoTypes = [
    { label: 'Calcular Pressão de Poros', value: 'PP', gn: 8.5 },
    { label: 'Criar Pressão de Poros Hidrostática', value: 'PPh', gn: 8.5 },
    { label: 'Criar Gradiente de Pressão de Poros Interpretada (GPOROS)', value: 'GPP', gn: 8.5 },
  ];

  calculoSelected = this.calculoTypes[0];

  /**
   * Tipos de Metodos
   */
  metodoTypes = [
    { label: 'Eaton DTC', value: 'eatonDtc', exp: 3, mnemonico: 'DTC', back: 'EatonDTC' },
    { label: 'Eaton Expoente D', value: 'eatonExp', exp: 1.2, mnemonico: 'ExpoenteD', back: 'EatonExpoenteD' },
    { label: 'Eaton Resistividade', value: 'eatonRes', exp: 1.2, mnemonico: 'RESIST', back: 'EatonResistividade' },
  ];

  metodoSelected = this.metodoTypes[0];

  /**
   * Nome do cálculo que será gerado
   */
  nomeCalculo: { value: string, tooltip: string, placeholder: string } = { value: '', tooltip: '', placeholder: '' };
  foiAlterado: boolean = false;
  idCalculo: string;

  calculoReservatorio: boolean = false;
  gpphXpph: boolean = false;

  /**
   * Lista de perfil no caso de estudo aberto
   */
  perfisDoPoco: Perfil[];

  perfis = {
    DTC: {
      lista: [],
      selecionado: undefined,
      tooltip: ''
    },
    ExpoenteD: {
      lista: [],
      selecionado: undefined,
      tooltip: ''
    },
    RESIST: {
      lista: [],
      selecionado: undefined,
      tooltip: ''
    }
  };

  /**
   * Lista de Perfis GSOBR
  */
  perfisGSOBR: Perfil[];

  /**
   * Perfil GSOBR selecionado
   */
  gsobrSelecionado: { value: Perfil, tooltip: string } = { value: undefined, tooltip: '' };

  /**
   * Lista de Perfis PPoro
  */
  perfisPPoro: Perfil[];

  /**
   * Perfil PPoro selecionado
   */
  pporoSelecionado: { value: Perfil, tooltip: string } = { value: undefined, tooltip: '' };

  nomesEmUso: string[] = [];

  editando: boolean = false;

  reservatorio: Reservatorio;

  _loading: boolean = false;
  get loading(): boolean { return this._loading; }
  set loading(val: boolean) {
    this._loading = val;
    val ? this.loaderService.addLoading() : this.loaderService.removeLoading();
  }

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { data: { calculo?: CalculoPP | CalculoPPH | CalculoGPP } },
    public dialog: DialogService,
    private notybarService: NotybarService,
    private loaderService: LoaderService,

    public calculoService: CalculoService,

    private dataset: DatasetService,
    private treeService: TreeDatasetService,
    private profileDataset: ProfileDatasetService,
    private calculationDataset: CalculationDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    this.nomesEmUso = this.treeService.getUsedNames(this.dataset.currCaseId, ['Cálculos', 'PressãoPoros']);

    this.perfisDoPoco = this.profileDataset.getAll(this.dataset.currCaseId).sort((a, b) => a.nome.localeCompare(b.nome));

    const perfisFiltradosComTrend = this.perfisDoPoco.filter(el => el.trend !== undefined && el.idCálculo !== undefined
      && isFiltro(this.dataset.getById(el.idCálculo).grupoCálculo));


    Object.keys(this.perfis).forEach(key => {
      this.perfis[key].lista = perfisFiltradosComTrend.filter(el => el.mnemonico === key);
      this.perfis[key].selecionado = this.perfis[key].lista[0];
    });

    this.perfisGSOBR = this.perfisDoPoco.filter(el => el.mnemonico === 'GSOBR');
    this.gsobrSelecionado.value = this.perfisGSOBR[0];

    this.perfisPPoro = this.perfisDoPoco.filter(el => el.mnemonico === 'PPORO');
    this.pporoSelecionado.value = this.perfisPPoro[0];

    // this.calculoTypes[2].disabled = this.perfisDoPoco.findIndex(el => el.mnemonico === 'GPORO') < 0;

    if (this.data.data && this.data.data.calculo !== undefined) {
      const calc: any = this.data.data.calculo;

      console.log(calc);
      this.idCalculo = calc.id;
      this.editando = true;

      this.nomeCalculo.value = `${calc.nome}`;
      this.foiAlterado = true;

      this.nomesEmUso = this.nomesEmUso.filter(el => el !== this.nomeCalculo.value);

      const lista: Perfil[] = calc.perfisEntrada.idPerfis.map(el => this.dataset.getById(el));
      console.log(lista);

      if (calc.métodoCálculo === 'EatonDTC' || calc.métodoCálculo === 'EatonResistividade' || calc.métodoCálculo === 'EatonExpoenteD') {
        this.calculoSelected = this.calculoTypes[0];
        this.calculoSelected.gn = calc.gn;
        this.metodoSelected = this.metodoTypes.find(el => el.back === calc.métodoCálculo);
        this.metodoSelected.exp = calc.exp;
        this.calculoReservatorio = calc.comDepleção;
        this.reservatorio = calc.dadosReservatório;
        this.gsobrSelecionado.value = lista.splice(lista.findIndex(el => el.mnemonico === 'GSOBR'), 1)[0];
        this.perfis[lista[0].mnemonico].selecionado = lista[0];
      } else if (calc.métodoCálculo === 'Hidrostática') {
        this.calculoSelected = this.calculoTypes[1];
        this.calculoSelected.gn = calc.gn;
      } else if (calc.métodoCálculo === 'Gradiente') {
        this.calculoSelected = this.calculoTypes[2];
        if (lista.length > 0) {
          this.gpphXpph = true;
          this.pporoSelecionado.value = lista[0];
        } else {
          this.gpphXpph = false;
          this.calculoSelected.gn = calc.gn;
        }

      }
    }
  }

  ngDoCheck() {
    if (!this.foiAlterado && this.nomeCalculo.value !== this.nomeCalculo.placeholder) {
      this.foiAlterado = true;
    }
    this.isValid = this.canSubmit();
  }

  canSubmit() {
    let isValid = true;
    this.atualizarPlaceholder();
    if ((this.nomeCalculo.value === undefined || this.nomeCalculo.value === null || this.nomeCalculo.value === '')
      && this.nomesEmUso.includes(this.nomeCalculo.placeholder)) {
      this.nomeCalculo.tooltip = 'Nome padrão já está em uso';
      isValid = false;
    } else if (this.nomesEmUso.includes(this.nomeCalculo.value)) {
      this.nomeCalculo.tooltip = 'Nome em uso';
      isValid = false;
    } else {
      this.nomeCalculo.tooltip = '';
    }

    if (this.calculoSelected.value === 'PP') {
      if (this.perfis[this.metodoSelected.mnemonico].selecionado === undefined
        || this.perfis[this.metodoSelected.mnemonico].selecionado === null) {
        this.perfis[this.metodoSelected.mnemonico].tooltip = 'Precisa escolher um perfil';
        isValid = false;
      } else {
        this.perfis[this.metodoSelected.mnemonico].tooltip = '';
      }

      if (this.gsobrSelecionado.value === undefined || this.gsobrSelecionado.value === null) {
        this.gsobrSelecionado.tooltip = 'Precisa escolher um perfil';
        isValid = false;
      } else {
        this.gsobrSelecionado.tooltip = '';
      }
    } else if (this.calculoSelected.value === 'GPP') {
      if (this.gpphXpph) {
        if (this.pporoSelecionado.value === undefined || this.pporoSelecionado.value === null) {
          this.pporoSelecionado.tooltip = 'Precisa escolher um perfil';
          isValid = false;
        } else {
          this.pporoSelecionado.tooltip = '';
        }
      }
    }

    return isValid;
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  salvarReservatórios(res: Reservatorio) {
    console.log(res);
    this.reservatorio = res;
  }

  atualizarPlaceholder() {
    const newPlaceholder = `${this.calculoSelected.value}_${this.calculoSelected.value === 'GPP' ? 'interp' : 'calc'}`;
    if (this.nomeCalculo.placeholder !== newPlaceholder) {
      this.nomeCalculo.placeholder = newPlaceholder;
      if (!this.foiAlterado) {
        this.nomeCalculo.value = `${this.nomeCalculo.placeholder}`;
      }
    }
  }

  submit() {
    this.loading = true;
    setTimeout(() => {

      let calculo: CalculoPP | CalculoPPH | CalculoGPP;
      if (this.calculoSelected.value === 'PP') {
        calculo = {
          nome: this.nomeCalculo.value || this.nomeCalculo.placeholder,
          idPoço: this.currCase.id,
          tipo: this.calculoSelected.value,
          gn: this.calculoSelected.gn,
          exp: this.metodoSelected.exp,
          idPerfilFiltrado: this.perfis[this.metodoSelected.mnemonico].selecionado.id,
          idGradienteSobrecarga: this.gsobrSelecionado.value.id,
          calculoReservatorio: this.calculoReservatorio
        };
        if (this.calculoReservatorio) {
          if (this.reservatorio === undefined) {
            this.loading = false;
            this.notybarService.show('Necessário editar informações de reservatório.', 'warning');
            return;
          }
          calculo.reservatorio = this.reservatorio;
        }
      } else if (this.calculoSelected.value === 'PPh') {
        calculo = {
          nome: this.nomeCalculo.value || this.nomeCalculo.placeholder,
          idPoço: this.currCase.id,
          tipo: this.calculoSelected.value,
          gn: this.calculoSelected.gn,
        };
      } else if (this.calculoSelected.value === 'GPP') {
        calculo = {
          nome: this.nomeCalculo.value || this.nomeCalculo.placeholder,
          idPoço: this.currCase.id,
          tipo: this.calculoSelected.value,
        };
        if (this.gpphXpph) {
          calculo.idPph = this.pporoSelecionado.value.id;
        } else {
          calculo.gpph = this.calculoSelected.gn;
        }
      }

      if (this.idCalculo && this.editando) {
        calculo.id = this.idCalculo;
      }

      const metodo: Promise<{ cálculo: Calculo, perfisAlterados?: Perfil[] }> = this.editando ?
        this.calculoService.editarCalculoPressaoPoros(calculo) :
        this.calculoService.criarCalculoPressaoPoros(calculo);

      // console.log('criar calculo', calculo);
      metodo.then(res => {
        // console.log('FOI', res);
        if (this.editando) {
          this.calculationDataset.update(res.cálculo, calculo.id, res.perfisAlterados);
        } else {
          this.calculationDataset.add(res.cálculo, this.dataset.currCaseId);
        }
        this.loading = false;
        this.closeModal();
      }).catch(() => { this.loading = false; });
    }, 10);
  }

  openWindowReservatorio() {
    this.dialog.openPageDialog(PpReservatorioComponent, { minHeight: 150, minWidth: 1100 },
      { context: this, reservatorio: this.reservatorio });
  }


}
