import { Component, OnInit, Inject, DoCheck } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { FormControl } from '@angular/forms';
import { ArrayUtils } from '@utils/array';
import { DialogService } from '@services/dialog.service';
import { TiposCorte, Filtro } from 'app/repositories/models/filtro';
import { FiltroService } from 'app/repositories/filtro.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { Calculo } from 'app/repositories/models/calculo';
import { Case } from 'app/repositories/models/case';
import { Perfil } from 'app/repositories/models/perfil';
import { TipoRocha } from 'app/repositories/models/litologia';
import { CalculationDatasetService } from '@services/dataset/calculation-dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { NumberUtils } from '@utils/number';
import { LithologyDatasetService } from '@services/dataset/lithology-dataset.service';

@Component({
  selector: 'app-window-filter',
  templateUrl: './window-filter.component.html',
  styleUrls: ['./window-filter.component.scss']
})
export class WindowFilterComponent implements OnInit, DoCheck {
  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;
  /**
   * Calculo sendo editado.
   */
  calculo: Calculo;
  editando: boolean = false;

  /**
   * Lista de perfil no caso de estudo aberto
   */
  perfisDoPoco: Perfil[];

  /**
   * Perfil escolhido para ser filtrado.
   */
  perfilSelecionado: { value: Perfil, tooltip: string } = { value: undefined, tooltip: '' };

  /**
   * Nome do perfil que será gerado
   */
  nomePerfilFiltrado: { value: string, tooltip: string, placeholder: string } = { value: '', tooltip: '', placeholder: '' };

  foiAlterado: boolean = false;

  /**
   * Se vai aplicar corte ou não
   */
  aplicarCorte: boolean = false;

  /**
   * Limite superior do corte
   */
  limiteSup: { value: string, tooltip: string } = { value: '0', tooltip: '' };

  /**
   * Limite inferior do corte
   */
  limiteInf: { value: string, tooltip: string } = { value: '0', tooltip: '' };

  /**
   * Tipo de corte escolhido
   */
  tipoCorteSelecionado: TiposCorte = 'Truncar';

  /**
   * Se vai aplicar filtro ou não
   */
  aplicarFiltro: boolean = true;

  /**
   * Tipos de filtros
   */
  tipoFiltros = [
    { label: 'Filtro simples', value: 'FiltroSimples', placeholder: 'Simples' },
    { label: 'Filtro por litologia', value: 'FiltroLitologia', placeholder: 'Litologia' },
    { label: 'Filtro por média móvel', value: 'FiltroMédiaMóvel', placeholder: 'MediaMovel' },
    { label: 'Filtro por linha base folhelho', value: 'FiltroLinhaBaseFolhelho', placeholder: 'LBF' },
  ];

  /**
   * Tipos de filtros selecionado
   */
  filtroSelecionado = this.tipoFiltros[0];
  editandoFiltroCorte = false;

  /**
   * Desvio máximo para filtro simples
   *
   * Só permite valores positivos
   */
  desvioMaximo: { value: string, tooltip: string } = { value: '2', tooltip: '' };

  /**
   * Lista de litologias no caso de estudo aberto
   *
   * Se caso é projeto, pegar da litologia adaptada
   * Se caso é correlação, pegar da litologia interpretada
   * Se caso é acompanhamento, pegar da litologia interpretada
   */
  litosDoPoco: TipoRocha[] = [];

  /**
   * Controle do Multiple Select para filtro por litologia
   */
  litosSelecionadas: { value: FormControl, tooltip: string } = { value: new FormControl(), tooltip: '' };

  /**
   * Numero de pontos para filtro por média móvel
   *
   * Só permite valores inteiros, impares e positivos, com valor mínimo = 3
   */
  pontosCalculo: { value: string, tooltip: string } = { value: '3', tooltip: '' };

  /**
   * Lista de perfis no caso de estudo aberto que contem LBF
   */
  tiposGRay: Perfil[];

  /**
   * Perfil selecionado para filtro por LBF
   */
  gRaySelecionado: { value: Perfil, tooltip: string } = { value: undefined, tooltip: '' };

  isValid: boolean = true;

  loading = false;

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { data: { calculo?} },
    public dialog: DialogService,

    private filtroService: FiltroService,

    private dataset: DatasetService,
    private profileDataset: ProfileDatasetService,
    private lithologyDataset: LithologyDatasetService,
    private calculationDataset: CalculationDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    this.perfisDoPoco = this.profileDataset.getAll(this.dataset.currCaseId).sort((a, b) => a.nome.localeCompare(b.nome));
    this.litosDoPoco = this.getTiposRochaDoPoco();
    this.tiposGRay = this.perfisDoPoco.filter(el => el.trend && el.trend.tipoTrend === 'LBF');

    this.gRaySelecionado.value = this.tiposGRay[0];

    if (this.data.data && this.data.data.calculo !== undefined) {
      this.editando = true;
      this.calculo = this.data.data.calculo;
      // console.log("editarFiltro", this.calculo);

      this.filtroSelecionado = this.tipoFiltros.find(el => el.value === this.calculo.grupoCálculo);
      if (this.filtroSelecionado === undefined) {
        this.aplicarFiltro = false;
        this.filtroSelecionado = this.tipoFiltros[0];
        this.editandoFiltroCorte = true;
      }

      this.perfilSelecionado.value = this.dataset.getById(this.calculo.perfisEntrada.idPerfis[0]);
      this.nomePerfilFiltrado.value = `${this.calculo.nome}`;
      this.foiAlterado = true;
      if (this.calculo['desvioMáximo'] !== undefined) {
        this.desvioMaximo.value = this.calculo['desvioMáximo'];
      }
      if (this.calculo['númeroPontos'] !== undefined) {
        this.pontosCalculo.value = this.calculo['númeroPontos'];
      }
      if (this.calculo['litologias'] !== undefined) {
        this.litosSelecionadas.value.setValue(this.litosDoPoco.filter(el => this.calculo['litologias'].includes(el.mnemonico)));
      }
      if (this.calculo['idPerfilLBF'] !== undefined) {
        this.gRaySelecionado.value = this.tiposGRay.find(el => el.id === this.calculo['idPerfilLBF']);
      }
      if (this.calculo['tipoCorte'] !== undefined) {
        this.aplicarCorte = true;
        this.tipoCorteSelecionado = this.calculo['tipoCorte'];

        this.limiteInf.value = this.calculo['limiteInferior'];
        this.limiteSup.value = this.calculo['limiteSuperior'];
      }
    } else {
      this.editando = false;
      this.perfilSelecionado.value = this.perfisDoPoco[0];

      this.perfilChanged();
    }

    this.isValid = this.canSubmit();
  }

  ngDoCheck() {
    if (!this.foiAlterado && this.nomePerfilFiltrado.value !== this.nomePerfilFiltrado.placeholder) {
      this.foiAlterado = true;
    }
    this.isValid = this.canSubmit();
  }

  toggleChanged(filtro) {
    if (!this.aplicarCorte && !this.aplicarFiltro) {
      if (filtro) {
        this.aplicarCorte = true;
      } else {
        this.aplicarFiltro = true;
      }
    }
  }

  perfilChanged() {
    if (this.perfilSelecionado.value !== undefined) {
      const pontos = this.perfilSelecionado.value.pontos;
      this.limiteInf.value = pontos.reduce((min, p) => p.valor < min ? p.valor : min, pontos[0].valor).toString();
      this.limiteSup.value = pontos.reduce((min, p) => p.valor > min ? p.valor : min, pontos[0].valor).toString();
    }
  }

  atualizarNome() {
    if (this.aplicarFiltro) {
      this.nomePerfilFiltrado.placeholder = `${this.perfilSelecionado.value.nome}_F${this.filtroSelecionado.placeholder}`;
    } else {
      this.nomePerfilFiltrado.placeholder = `${this.perfilSelecionado.value.nome}_Corte`;
    }
    if (!this.foiAlterado) {
      this.nomePerfilFiltrado.value = `${this.nomePerfilFiltrado.placeholder}`;
    }
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  pegarFiltroGeral(): Filtro {
    const filtro: Filtro = {
      idPerfil: this.perfilSelecionado.value.id,
      nome: this.nomePerfilFiltrado.value,
    };
    return filtro;
  }

  submit() {
    this.loading = true;
    const filtro: any = {
      idPerfil: this.perfilSelecionado.value.id,
      nome: this.nomePerfilFiltrado.value,
    };

    if (this.editando) {
      filtro.idPerfil = this.calculo.perfisSaída.idPerfis[0];
    }

    if (this.nomePerfilFiltrado.value === undefined || this.nomePerfilFiltrado.value === null || this.nomePerfilFiltrado.value === '') {
      filtro.nome = this.nomePerfilFiltrado.placeholder;
    }

    if (this.aplicarCorte) {
      filtro.tipoCorte = this.tipoCorteSelecionado;
      filtro.limiteInferior = parseFloat(this.limiteInf.value);
      filtro.limiteSuperior = parseFloat(this.limiteSup.value);
    }

    let metodo: Promise<any>;
    if (this.aplicarFiltro) {
      switch (this.filtroSelecionado.value) {
        case this.tipoFiltros[0].value:
          filtro.desvioMáximo = parseFloat(this.desvioMaximo.value);
          metodo = this.editando ? this.filtroService.editarFiltroSimples(filtro) : this.filtroService.criarFiltroSimples(filtro);
          break;
        case this.tipoFiltros[1].value:
          filtro.litologiasSelecionadas = this.litosSelecionadas.value.value.map(i => i.mnemonico);
          metodo = this.editando ? this.filtroService.editarFiltroLitologia(filtro) : this.filtroService.criarFiltroLitologia(filtro);
          break;
        case this.tipoFiltros[2].value:
          filtro.númeroPontos = parseInt(this.pontosCalculo.value);
          metodo = this.editando ? this.filtroService.editarFiltroMediaMovel(filtro) : this.filtroService.criarFiltroMediaMovel(filtro);
          break;
        case this.tipoFiltros[3].value:
          filtro.idLBF = this.gRaySelecionado.value.id;
          metodo = this.editando ? this.filtroService.editarFiltroLBF(filtro) : this.filtroService.criarFiltroLBF(filtro);
          break;
      }
    } else {
      metodo = this.editando ? this.filtroService.editarCorte(filtro) : this.filtroService.criarCorte(filtro);
    }

    console.log('filtroSelecionado', filtro);
    metodo.then(res => {
      console.log('FOI', res);
      if (this.editando) {
        this.calculationDataset.updateFilter(res.cálculo, this.dataset.currCaseId);
      } else {
        this.calculationDataset.addFilter(res.cálculo, this.dataset.currCaseId);
      }

      this.loading = false;
      this.closeModal();
    }).catch(() => { this.loading = false; });
  }

  canSubmit() {
    let isValid = true;
    if ((this.nomePerfilFiltrado.value === '' || this.nomePerfilFiltrado.value === null || this.nomePerfilFiltrado.value === undefined)
      && this.perfisDoPoco.findIndex(el => el.nome === this.nomePerfilFiltrado.placeholder) >= 0) {
      this.nomePerfilFiltrado.tooltip = 'Nome padrão já está em uso.';
      isValid = false;
    } else if (this.perfisDoPoco.findIndex(el => el.nome === this.nomePerfilFiltrado.value) >= 0
      && (this.calculo === undefined || this.calculo.nome !== this.nomePerfilFiltrado.value)) {
      this.nomePerfilFiltrado.tooltip = 'Nome já está em uso.';
      isValid = false;
    } else {
      this.nomePerfilFiltrado.tooltip = '';
    }
    if (this.perfilSelecionado.value === null || this.perfilSelecionado.value === undefined) {
      this.perfilSelecionado.tooltip = 'Precisa escolher um perfil.';
      isValid = false;
    } else {
      this.perfilSelecionado.tooltip = '';
      this.atualizarNome();
    }
    if (this.aplicarCorte) {
      if (this.limiteInf.value === null || this.limiteInf.value === undefined) {
        this.limiteInf.tooltip = 'Precisa estar preenchido.';
        isValid = false;
      } else if (!NumberUtils.isNumber(this.limiteInf.value)) {
        this.limiteInf.tooltip = 'Precisa ser número';
        isValid = false;
      } else {
        this.limiteInf.tooltip = '';
      }
      if (this.limiteSup.value === null || this.limiteSup.value === undefined) {
        this.limiteSup.tooltip = 'Precisa estar preenchido.';
        isValid = false;
      } else if (!NumberUtils.isNumber(this.limiteSup.value)) {
        this.limiteSup.tooltip = 'Precisa ser número';
        isValid = false;
      } else {
        this.limiteSup.tooltip = '';
      }
      if (parseFloat(this.limiteInf.value) > parseFloat(this.limiteSup.value)) {
        this.limiteInf.tooltip = this.limiteSup.tooltip = 'Limite inferior precisa ser menor que limite superior.';
        isValid = false;
      }
    }
    if (this.aplicarFiltro) {
      switch (this.filtroSelecionado.value) {
        case this.tipoFiltros[0].value:
          if (this.desvioMaximo.value === null || this.desvioMaximo.value === undefined) {
            this.desvioMaximo.tooltip = 'Precisa estar preenchido.';
            isValid = false;
          } else if (!NumberUtils.isNumber(this.desvioMaximo.value)) {
            this.desvioMaximo.tooltip = 'Precisa ser número';
            isValid = false;
          } else if (parseFloat(this.desvioMaximo.value) <= 0) {
            this.desvioMaximo.tooltip = 'Desvio máximo precisa ser positivo.';
            isValid = false;
          } else {
            this.desvioMaximo.tooltip = '';
          }
          break;
        case this.tipoFiltros[1].value:
          if (this.litosSelecionadas.value.value === null || this.litosSelecionadas.value.value === undefined ||
            (this.litosSelecionadas.value.value as string[]).length < 1) {
            this.litosSelecionadas.tooltip = 'Precisa ter pelo menos uma litologia selecionada.';
            isValid = false;
          } else {
            this.litosSelecionadas.tooltip = '';
          }
          break;
        case this.tipoFiltros[2].value:
          if (this.pontosCalculo.value === null || this.pontosCalculo.value === undefined) {
            this.pontosCalculo.tooltip = 'Precisa estar preenchido.';
            isValid = false;
          } else if (!NumberUtils.isNumber(this.pontosCalculo.value)) {
            this.desvioMaximo.tooltip = 'Precisa ser número';
            isValid = false;
          } else if (parseFloat(this.pontosCalculo.value) < 3 || parseFloat(this.pontosCalculo.value) % 2 === 0 ||
            !Number.isInteger(parseFloat(this.pontosCalculo.value))) {
            this.pontosCalculo.tooltip = 'Precisa ser inteiro, ímpar e maior que 3.';
            isValid = false;
          } else {
            this.pontosCalculo.tooltip = '';
          }
          break;
        case this.tipoFiltros[3].value:
          if (this.gRaySelecionado.value === null || this.gRaySelecionado.value === undefined) {
            this.gRaySelecionado.tooltip = 'Precisa estar preenchido.';
            isValid = false;
          } else {
            this.gRaySelecionado.tooltip = '';
          }
          break;
        default:
          break;
      }
    }

    return isValid;
  }

  onLithoRemoved(litho: string) {
    const lithos = this.litosSelecionadas.value.value as string[];
    ArrayUtils.removeFirst(lithos, litho);
    this.litosSelecionadas.value.setValue(lithos); // To trigger change detection
  }

  getTiposRochaDoPoco() {
    const tipoLitologia: string = this.currCase.tipoPoço === 'Projeto' ? 'Adaptada' : 'Interpretada';

    const litos = this.lithologyDataset.getAll(this.currCase.id);
    const lito = litos.find(el => el.classificação.nome === tipoLitologia);

    const tipoRochaAdicionado: any = {};
    const tiposRocha: TipoRocha[] = [];

    if (lito && lito.pontos) {
      lito.pontos.forEach(el => {
        if (!tipoRochaAdicionado[el.tipoRocha.mnemonico]) {
          tipoRochaAdicionado[el.tipoRocha.mnemonico] = true;
          tiposRocha.push(el.tipoRocha);
        }
      });
    }

    return tiposRocha;
  }

}
