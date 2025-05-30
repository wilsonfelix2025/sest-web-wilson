import { Component, OnInit, Inject, DoCheck } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { CalculationDatasetService } from '@services/dataset/calculation-dataset.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { TreeDatasetService } from '@services/dataset/state/tree-dataset.service';
import { LoaderService } from '@services/loader.service';
import { CalculoService } from 'app/repositories/calculo.service';
import { Calculo, CalculoSobrecarga } from 'app/repositories/models/calculo';
import { Case } from 'app/repositories/models/case';
import { Perfil } from 'app/repositories/models/perfil';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-overload-calculation',
  templateUrl: './overload-calculation.component.html',
  styleUrls: ['./overload-calculation.component.scss']
})
export class OverloadCalculationComponent implements OnInit, DoCheck {

  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  /**
  * Nome do cálculo
  */
  nomeCalculo: { value: string, tooltip: string } = { value: 'Sobrecarga_Calc', tooltip: '' };

  /**
   * RHOB
   */
  rhob = [];

  /**
  * Perfil selecionado
  */
  rhobSelected: { value: Perfil, tooltip: string } = { value: undefined, tooltip: '' };

  isValid: boolean = true;
  sedimentos = 0;

  editando: boolean = false;

  nomesEmUso: string[] = [];

  _loading: boolean = false;
  get loading(): boolean { return this._loading; }
  set loading(val: boolean) {
    this._loading = val;
    val ? this.loaderService.addLoading() : this.loaderService.removeLoading();
  }

  constructor(
    public dialogRef: MatDialogRef<OverloadCalculationComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { data: { calculo?} },
    private loaderService: LoaderService,

    public calculoService: CalculoService,

    private dataset: DatasetService,
    private treeService: TreeDatasetService,
    private profileDataset: ProfileDatasetService,
    private calculationDataset: CalculationDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    if (this.currCase.dadosGerais.geometria.categoriaPoço === 'OnShore') {
      this.sedimentos = this.currCase.dadosGerais.geometria.mesaRotativa + this.currCase.dadosGerais.geometria.onShore.alturaDeAntePoço;
    } else {
      this.sedimentos = this.currCase.dadosGerais.geometria.mesaRotativa + this.currCase.dadosGerais.geometria.offShore.laminaDagua;
    }

    this.nomesEmUso = this.treeService.getUsedNames(this.dataset.currCaseId, ['Cálculos', 'Sobrecarga']);

    const perfisCalculo = this.profileDataset.getAll(this.dataset.currCaseId);

    this.rhob = perfisCalculo.filter(perfil => perfil.mnemonico === 'RHOB');

    if (this.data.data && this.data.data.calculo !== undefined) {
      this.editando = true;

      const calc = this.data.data.calculo;
      this.nomeCalculo.value = calc.nome;
      this.rhobSelected.value = this.rhob.find(el => el.id === calc.perfisEntrada.idPerfis[0]);

      this.nomesEmUso = this.nomesEmUso.filter(el => el !== this.nomeCalculo.value);
    } else {
      this.rhobSelected.value = this.rhob[0];
    }
  }

  ngDoCheck() {
    this.isValid = this.canSubmit();
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
    if (this.rhobSelected.value === undefined || this.rhobSelected.value === null) {
      this.rhobSelected.tooltip = 'Precisa escolher um perfil';
      isValid = false;
    } else if (this.rhobSelected.value.pontos.length === 0) {
      this.rhobSelected.tooltip = `Faltam dados\nO perfil selecionado deve possuir dados.`;
      isValid = false;
    } else if (Number(this.rhobSelected.value.pontos[0].pm.valor) > this.sedimentos) {
      this.rhobSelected.tooltip = `Faltam dados\nO perfil selecionado deve possuir dados desde a profundidade inicial de sedimentos.\nA profundidade inicial de sedimento é ${this.sedimentos}, enquanto o primeiro ponto do perfil é ${this.rhobSelected.value.pontos[0].pm.valor}.`;
      isValid = false;
    } else {
      this.rhobSelected.tooltip = '';
    }

    return isValid;
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  submit() {
    this.loading = true;
    setTimeout(() => {
      const calculo: CalculoSobrecarga = {
        nome: this.nomeCalculo.value,
        idRhob: this.rhobSelected.value.id,
        idPoço: this.currCase.id
      };

      let metodo: Promise<{ cálculo: Calculo, perfisAlterados?: Perfil[] }>;

      if (this.editando) {
        calculo.idCálculoAntigo = this.data.data.calculo.id;
        metodo = this.calculoService.editarCalculoSobrecarga(calculo);
      } else {
        metodo = this.calculoService.criarCalculoSobrecarga(calculo);
      }

      metodo.then(res => {
        // console.log('res', res);
        if (this.editando) {
          this.calculationDataset.update(res.cálculo, calculo.idCálculoAntigo, res.perfisAlterados);
        } else {
          this.calculationDataset.add(res.cálculo, this.dataset.currCaseId);
        }

        this.loading = false;
        this.closeModal();
      }).catch(() => { this.loading = false; });
    }, 10);
  }
}
