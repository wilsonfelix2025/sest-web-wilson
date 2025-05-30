import { Component, OnInit, Inject, DoCheck } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogService } from '@services/dialog.service';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { Case } from 'app/repositories/models/case';
import { NotybarService } from '@services/notybar.service';
import { FormControl } from '@angular/forms';
import { DatasetService } from '@services/dataset/dataset.service';
import { ArrayUtils } from '@utils/array';
import { TipoRocha } from 'app/repositories/models/litologia';
import { NumberUtils } from '@utils/number';
import { ParametrosPoroElastico } from 'app/repositories/models/calculo';
@Component({
  selector: 'app-poroelastic-parameters',
  templateUrl: './poroelastic-parameters.component.html',
  styleUrls: ['./poroelastic-parameters.component.scss']
})
export class PoroelasticParametersComponent implements OnInit, DoCheck {

  /**
  * O caso de estudo aberto atualmente.
  */
  currCase: Case;

  /**
  * Nome do cálculo
  */
  nomeCalculo: { value: string, tooltip: string } = { value: 'Perfis_Calc', tooltip: '' };

  propFluido = [
    { title: 'Kf', unit: 'psi', back: 'kf', value: '2500', tooltip: '' },
    { title: 'Viscosidade', unit: 'cP', back: 'viscosidade', value: '0.3', tooltip: '' },
  ];

  temp = '60';

  propFisicoQuim = [
    { title: 'Coeficiente de reflexão', unit: '', back: 'coeficienteReflexão', value: '0.4', tooltip: '' },
    { title: 'Coeficiente de inchamento', unit: 'psi', back: 'coeficienteInchamento', value: '10.5', tooltip: '' },
    { title: 'Coeficiente de difusão do soluto', unit: 'm²/s', back: 'coeficienteDifusãoSoluto', value: '1.6e-10', tooltip: '' },
    { title: 'Densidade do fluído de formação', unit: 'Kg/m³', back: 'densidadeFluidoFormação', value: '1000', tooltip: '' },
    { title: 'Temperatura da formação', unit: '°C', back: 'temperaturaFormaçãoFisicoQuimica', value: '', tooltip: '', temp: true },
    { title: 'Concentração Sol do fluido de perfuração', unit: 'Kg/m³', back: 'concentraçãoSolFluidoPerfuração', value: '200', tooltip: '' },
    { title: 'Concentração do soluto na rocha', unit: 'Kg/m³', back: 'concentraçãoSolutoRocha', value: '25.44', tooltip: '' },
  ];

  tiposSal = {
    list: [
      { title: 'NaCl', value: 'NaCl', coefSoluto: 1.8464, massaSoluto: 0.00585 },
      { title: 'CaCl2', value: 'CaCl2', coefSoluto: 2.601, massaSoluto: 0.1110 },
      { title: 'KCl', value: 'KCl', coefSoluto: 1.854, massaSoluto: 0.0745 },
    ],
    solutos: [
      { title: 'Coeficiente de dissociação do soluto', unit: '', back: 'coeficienteDissociaçãoSoluto', value: 'coefSoluto' },
      { title: 'Massa molar do soluto', unit: 'Kg/mol', back: 'massaMolarSoluto', value: 'massaSoluto' },
    ],
    selected: undefined,
    tooltip: ''
  };

  propTermic = [
    { title: 'Expansão térmica do volume do fluído de poros', unit: '1/°C', back: 'expansãoTérmicaVolumeFluidoPoros', value: '0.000303', tooltip: '' },
    { title: 'Temperatura do poço', unit: '°C', back: 'temperaturaPoço', value: '60', tooltip: '' },
    { title: 'Temperatura da formação', unit: '°C', back: 'temperaturaFormação', value: '', tooltip: '', temp: true },
    { title: 'Difusividade térmica', unit: 'm²/s', back: 'difusidadeTérmica', value: '5.5e-07', tooltip: '' },
    { title: 'Expansão térmica na rocha', unit: '1/°C', back: 'expansãoTérmicaRocha', value: '8.87e-06', tooltip: '' },
  ];

  /**
   * Lista de litologias no caso de estudo aberto
   *
   * Se caso é projeto, pegar da litologia adaptada
   * Se caso é correlação, pegar da litologia interpretada
   * Se caso é acompanhamento, pegar da litologia interpretada
   */
  litosDoPoco: TipoRocha[] = [];

  litosSelecionadas: { value: FormControl, tooltip: string } = { value: new FormControl(), tooltip: '' };

  isValid: boolean = true;

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    public dialog: DialogService,
    public notybarService: NotybarService,
    @Inject(MAT_DIALOG_DATA) public data: { data: { context, caseId, parametros: ParametrosPoroElastico } },

    private dataset: DatasetService,
  ) {
    this.tiposSal.selected = this.tiposSal.list[0];
  }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.data.data.caseId);
    this.tiposSal.selected = this.tiposSal.list[0];
    this.litosDoPoco = this.getTiposRochaDoPoco();

    this.litosSelecionadas.value.setValue(this.litosDoPoco);
    console.log(this.litosDoPoco);

    if (this.data.data.parametros) {
      const p = this.data.data.parametros;

      this.propFluido.forEach(input => {
        input.value = p[input.back];
      });

      this.temp = String(p.temperaturaFormação);

      this.propFisicoQuim.forEach(input => {
        input.value = p[input.back];
      });

      this.tiposSal.selected = this.tiposSal.list.find(el => el.value === p.tipoSal);

      this.propTermic.forEach(input => {
        input.value = p[input.back];
      });

      const litos = [];
      p.litologias.forEach(el => {
        litos.push(this.litosDoPoco.find(l => l.mnemonico === el));
      });
      this.litosSelecionadas.value.setValue(litos);
    }
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  ngDoCheck() {
    this.isValid = this.canSubmit();
  }

  canSubmit() {
    let isValid = true;

    this.propFisicoQuim.find(el => el.back === 'temperaturaFormaçãoFisicoQuimica').value = this.temp;
    this.propTermic.find(el => el.back === 'temperaturaFormação').value = this.temp;

    this.propFluido.forEach(input => {
      if (input.value === undefined || input.value === null || input.value === '') {
        input.tooltip = 'Precisa estar preenchido';
        isValid = false;
      } else if (!NumberUtils.isNumber(input.value)) {
        input.tooltip = 'Precisa ser um número';
        isValid = false;
      } else if (Number(input.value) < 0) {
        input.tooltip = 'Precisa ser >= 0';
        isValid = false;
      } else {
        input.tooltip = '';
      }
    });

    this.propFisicoQuim.forEach(input => {
      if (input.value === undefined || input.value === null || input.value === '') {
        input.tooltip = 'Precisa estar preenchido';
        isValid = false;
      } else if (!NumberUtils.isNumber(input.value)) {
        input.tooltip = 'Precisa ser um número';
        isValid = false;
      } else if (Number(input.value) < 0) {
        input.tooltip = 'Precisa ser >= 0';
        isValid = false;
      } else {
        input.tooltip = '';
      }
    });

    this.propTermic.forEach(input => {
      if (input.value === undefined || input.value === null || input.value === '') {
        input.tooltip = 'Precisa estar preenchido';
        isValid = false;
      } else if (!NumberUtils.isNumber(input.value)) {
        input.tooltip = 'Precisa ser um número';
        isValid = false;
      } else if (Number(input.value) < 0) {
        input.tooltip = 'Precisa ser >= 0';
        isValid = false;
      } else {
        input.tooltip = '';
      }
    });

    return isValid
  }

  submit() {
    const p: ParametrosPoroElastico = this.data.data.parametros;

    this.propFluido.forEach(input => {
      p[input.back] = input.value;
    });

    this.propFisicoQuim.forEach(input => {
      p[input.back] = input.value;
    });

    p.tipoSal = this.tiposSal.selected.value;
    p.coeficienteDissociaçãoSoluto = this.tiposSal.selected.coefSoluto;
    p.massaMolarSoluto = this.tiposSal.selected.massaSoluto;

    this.propTermic.forEach(input => {
      p[input.back] = input.value;
    });
    p.litologias = this.litosSelecionadas.value.value.map(el => el.mnemonico);

    this.data.data.context.salvarParametros(p);
    this.closeModal();
  }

  onLithoRemoved(litho: string) {
    const lithos = this.litosSelecionadas.value.value as string[];
    ArrayUtils.removeFirst(lithos, litho);
    this.litosSelecionadas.value.setValue(lithos); // To trigger change detection
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
}
