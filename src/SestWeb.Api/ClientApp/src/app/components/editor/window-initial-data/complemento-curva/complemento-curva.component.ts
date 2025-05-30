import { Component, OnInit, Input } from '@angular/core';
import { ObjInitialData, ProfileInitialData, InsertInitialData } from '@utils/interfaces';
import { FormControl } from '@angular/forms';
import { NotybarService } from '@services/notybar.service';
import { Case } from 'app/repositories/models/case';
import { ArrayUtils } from '@utils/array';
import { DatasetService } from '@services/dataset/dataset.service';
import { LithologyDatasetService } from '@services/dataset/lithology-dataset.service';
import { NumberUtils } from '@utils/number';
import { InitialDataService } from 'app/repositories/initial-data.service';

@Component({
  selector: 'sest-complemento-curva',
  templateUrl: './complemento-curva.component.html',
  styleUrls: ['./complemento-curva.component.scss']
})
export class ComplementoCurvaComponent implements OnInit {

  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  litologias = new FormControl();
  initialDataModel: ObjInitialData = <ObjInitialData>{};
  profileModel: ProfileInitialData;

  perfilTypes: string = '';
  trechoTypes: string[] = [
    'Inicial',
    'Final'
  ];
  tipoTypes: string[] = [
    'Linear',
    'Quadrático',
    'Potência',
    'Exponencial'
  ];
  litoTypes = [];
  nomeTypes: string = '';

  perfilTypeSelected: ProfileInitialData = <ProfileInitialData>{};
  trechoTypeSelected: string;
  tipoTypeSelected: string;
  nomeTypeSelected: string;
  litoTypeSelected: string;
  pmLimiteTypeSelected: number;

  numeroPontos: number;
  pmTopo: number;
  valorTopo: number;
  unidadeValorTopo: string;
  pmPrimeiro: number;
  pmUltimo: number;
  pmBase: number;

  inserirTrechoData: InsertInitialData = <InsertInitialData>{};

  constructor(
    private initialDataService: InitialDataService,
    private notybarService: NotybarService,

    private dataset: DatasetService,
    private lithologyDataset: LithologyDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    this.getProfiles();
    this.litoTypes = this.getLithoInfo();
  }

  getLithoInfo() {
    const litos = this.lithologyDataset.getAll(this.currCase.id);
    const litoInfo = [];
    const addedLito: any = {};
    let tipoLitologia: string;
    let lito = null;

    if (this.currCase.tipoPoço === 'Projeto') {
      tipoLitologia = 'Adaptada';
    } else {
      tipoLitologia = 'Interpretada';
    }

    for (let i = 0; i < litos.length; i++) {
      if (litos[i].classificação.nome === tipoLitologia) {
        lito = litos[i];
        break;
      }
    }

    for (const litoSection of lito.canvas) {
      if (litoSection.name !== 'Água' && !addedLito[litoSection.code]) {
        addedLito[litoSection.code] = true;
        litoInfo.push({
          label: litoSection.name,
          value: litoSection.code
        });
      }
    }

    return litoInfo;
  }


  // buscar os perfis que podem ser utilizados na funcionalidade de complemento de trecho inicial/final
  getProfiles() {
    return this.initialDataService.getProfilesForInitialData(this.currCase.id).then(response => {
      this.initialDataModel.finalWellPoint = response['dados']['pmBase'];
      this.initialDataModel.initialWellPoint = response['dados']['pmTopo'];
      this.initialDataModel.profiles = [];
      response['dados']['listaPerfis'].forEach(element => {
        this.profileModel = <ProfileInitialData>{};
        this.profileModel.initialPoint = element.pmInicialPerfil;
        this.profileModel.finalPoint = element.pmFinalPerfil;
        this.profileModel.initialValue = element.valorTopoPerfil;
        this.profileModel.profileId = element.id;
        this.profileModel.mnemonic = element.mnemônico;
        this.profileModel.name = element.nome;
        this.profileModel.unit = element.unidade;
        this.initialDataModel.profiles.push(this.profileModel);
      });

      this.pmTopo = this.initialDataModel.initialWellPoint;
      this.pmBase = this.initialDataModel.finalWellPoint;
    });
  }

  // método para pegar os dados da tab ao clicar em salvar
  getData() {
    if (this.nomeTypeSelected == null || this.perfilTypeSelected.profileId == null ||
      this.pmLimiteTypeSelected == null || this.trechoTypeSelected == null ||
      this.tipoTypeSelected == null) {
      this.notybarService.show('Preencha tudo antes de adicionar.', 'danger');
    } else {
      this.inserirTrechoData.litologiasSelecionadas = this.litologias.value === null ? [] : this.litologias.value.map(i => i.value);
      this.inserirTrechoData.nomeNovoPerfil = this.nomeTypeSelected;
      this.inserirTrechoData.perfilId = this.perfilTypeSelected.profileId;
      this.inserirTrechoData.pmLimite = this.pmLimiteTypeSelected;
      this.inserirTrechoData.tipoDeTrecho = this.trechoTypeSelected;
      this.inserirTrechoData.tipoTratamento = this.tipoTypeSelected;
      return this.inserirTrechoData;
    }
  }

  updateScreenInfo(profilseSelected: ProfileInitialData) {
    this.valorTopo = profilseSelected.initialValue;
    this.pmPrimeiro = profilseSelected.initialPoint;
    this.pmUltimo = profilseSelected.finalPoint;
    this.unidadeValorTopo = profilseSelected.unit;
  }

  onKeyPress(event: KeyboardEvent, value: string | number) {
    let inputChar = event.key;

    if (value !== undefined) {
      inputChar = value + inputChar;
    }

    if (!NumberUtils.isNumber(inputChar)) {
      event.preventDefault();
    }
  }

  onBlurPmLimite() {
    this.getQtdPontos();
  }

  onCloseLitologias() {
    this.getQtdPontos();
  }

  onLithoRemoved(litho: string) {
    const lithos = this.litologias.value as string[];
    ArrayUtils.removeFirst(lithos, litho);
    this.litologias.setValue(lithos); // To trigger change detection

    this.getQtdPontos();
  }

  getQtdPontos() {
    if (this.perfilTypeSelected.profileId !== null && this.perfilTypeSelected.profileId !== undefined &&
      this.pmLimiteTypeSelected !== null && this.pmLimiteTypeSelected !== undefined &&
      this.trechoTypeSelected !== null && this.trechoTypeSelected !== undefined) {
      let litos = null;
      if (this.litologias.value !== null) {
        litos = this.litologias.value.map(i => i.value);
      }
      this.initialDataService.getProfileTotalPoints(this.perfilTypeSelected.profileId, this.pmLimiteTypeSelected
        , litos, this.trechoTypeSelected).then(response => {
          this.numeroPontos = response['qtd'];
        });
    }
  }

}
