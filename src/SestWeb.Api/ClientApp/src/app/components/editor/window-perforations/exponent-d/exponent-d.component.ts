import { Component, OnInit, Inject, DoCheck } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { CalculationDatasetService } from '@services/dataset/calculation-dataset.service';
import { DatasetService } from '@services/dataset/dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { TreeDatasetService } from '@services/dataset/state/tree-dataset.service';
import { CalculoService } from 'app/repositories/calculo.service';
import { Calculo, CalculoExpD } from 'app/repositories/models/calculo';
import { Case } from 'app/repositories/models/case';
import { Perfil } from 'app/repositories/models/perfil';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-exponent-d',
  templateUrl: './exponent-d.component.html',
  styleUrls: ['./exponent-d.component.scss']
})
export class ExponentDComponent implements OnInit, DoCheck {

  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  tipoCalculo = [
    { title: 'Expoente D', back: 'CalculadorExpoenteD', name: { value: 'ExpoenteD', tooltip: '' } },
    { title: 'Expoente D Corrigido', back: 'CalculadorExpoenteDCorrigido', name: { value: 'ExpD_corrigido', tooltip: '' }, ecd: true },
  ];

  calculoSelecionado = this.tipoCalculo[0];

  /**
   * Lista de perfil no caso de estudo aberto
   */
  perfisDoPoco: Perfil[];

  perfisEntrada = [
    { title: 'ROP/IROP', mnemonic: 'ROP', list: [], selected: undefined, tooltip: '' },
    { title: 'RPM', mnemonic: 'RPM', list: [], selected: undefined, tooltip: '' },
    { title: 'WOB', mnemonic: 'WOB', list: [], selected: undefined, tooltip: '' },
    { title: 'Diâmetro de Broca', mnemonic: 'DIAM_BROCA', list: [], selected: undefined, tooltip: '' },
    { title: 'ECD', mnemonic: 'GECD', list: [], selected: undefined, tooltip: '' },
  ];

  isValid: boolean = true;

  editando: boolean = false;

  nomesEmUso: string[] = [];

  loading = false;

  constructor(
    public dialogRef: MatDialogRef<ExponentDComponent>,
    @Inject(MAT_DIALOG_DATA) public data: { data: { calculo?} },

    public calculoService: CalculoService,

    private dataset: DatasetService,
    private treeService: TreeDatasetService,
    private profileDataset: ProfileDatasetService,
    private calculationDataset: CalculationDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    this.nomesEmUso = this.treeService.getUsedNames(this.dataset.currCaseId, ['Perfuração', 'ExpoenteD']);

    this.perfisDoPoco = this.profileDataset.getAll(this.dataset.currCaseId).sort((a, b) => a.nome.localeCompare(b.nome));

    let calc;
    if (this.data.data && this.data.data.calculo !== undefined) {
      console.log(this.data.data.calculo);
      calc = this.data.data.calculo;

      this.calculoSelecionado.name.value = calc.nome;
      this.nomesEmUso = this.nomesEmUso.filter(el => el !== calc.nome);

      this.editando = true;
    }

    this.perfisEntrada.forEach(p => {
      p.list = this.perfisDoPoco.filter(el => el.mnemonico === p.mnemonic);
      if (p.mnemonic === 'ROP') {
        p.list = p.list.concat(this.perfisDoPoco.filter(el => el.mnemonico === 'IROP'));
      }
      if (this.editando) {
        p.selected = p.list.find(el => calc.perfisEntrada.idPerfis.includes(el.id));
      }
      if (p.selected === undefined) {
        p.selected = p.list[0];
      }
    });
  }

  ngDoCheck() {
    this.isValid = this.canSubmit();
  }

  canSubmit() {
    let isValid = true;
    if (this.calculoSelecionado.name.value === undefined || this.calculoSelecionado.name.value === null || this.calculoSelecionado.name.value === '') {
      this.calculoSelecionado.name.tooltip = 'Nome precisa estar preenchido';
      isValid = false;
    } else if (this.nomesEmUso.includes(this.calculoSelecionado.name.value)) {
      this.calculoSelecionado.name.tooltip = 'Nome em uso';
      isValid = false;
    } else {
      this.calculoSelecionado.name.tooltip = '';
    }

    this.perfisEntrada.forEach(p => {
      if ((p.selected === undefined && p.mnemonic !== 'GECD') || (p.selected === undefined && this.calculoSelecionado.ecd)) {
        p.tooltip = 'Precisa escolher um perfil';
        isValid = false;
      } else {
        p.tooltip = '';
      }
    });

    return isValid;
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  submit() {
    this.loading = true;
    const calculo: CalculoExpD = {
      nome: this.calculoSelecionado.name.value,
      idPoço: this.currCase.id,

      correlação: this.calculoSelecionado.back,

      perfilDIAM_BROCA: this.perfisEntrada.find(el => el.mnemonic === 'DIAM_BROCA').selected.id,
      perfilROPId: this.perfisEntrada.find(el => el.mnemonic === 'ROP').selected.id,
      perfilWOBId: this.perfisEntrada.find(el => el.mnemonic === 'WOB').selected.id,
      perfilRPMId: this.perfisEntrada.find(el => el.mnemonic === 'RPM').selected.id,
      perfilECDId: this.calculoSelecionado.ecd ? this.perfisEntrada.find(el => el.mnemonic === 'GECD').selected.id : undefined,
    }

    if (this.editando) {
      calculo.idCálculo = this.data.data.calculo.id;
    }

    const metodo: Promise<{ cálculo: Calculo, perfisAlterados?: Perfil[] }> = this.editando ?
      this.calculoService.editarCalculoExpD(calculo) :
      this.calculoService.criarCalculoExpD(calculo);

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
  }
}
