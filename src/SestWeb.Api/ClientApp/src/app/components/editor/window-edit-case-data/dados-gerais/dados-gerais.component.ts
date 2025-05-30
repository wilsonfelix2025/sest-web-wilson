import { Component, OnInit, Input } from '@angular/core';
import { DialogService } from '@services/dialog.service';
import { ImportSestWebComponent } from '../../import/import-sest-web/import-sest-web.component';
import { ImportComponent } from '@components/editor/import/import.component';
import { HttpClient } from '@angular/common/http';
import { Case, DadosGerais } from 'app/repositories/models/case';
import { FileInfoComponent } from '@components/editor/import/shared/file-info/file-info.component';
import { NumberUtils } from '@utils/number';
import { environment } from 'environments/environment';
import { fileInfoFormatter } from '@components/editor/import/shared/file-info-formatter';

@Component({
  selector: 'sest-dados-gerais',
  templateUrl: './dados-gerais.component.html',
  styleUrls: ['./dados-gerais.component.scss'],
})
export class DadosGeraisComponent implements OnInit {
  @Input() caseData: Case;

  importedCaseName: string = '';
  importedCaseUrl: string = '';
  importedCaseUrlRevision: string = '';
  publicationDate: string = '';
  isUpdated: boolean = true;

  currCaseName: string = '';
  currPocoName: string = '';
  currPocoWebName: string = '';
  currCasePurpose: string = '';
  currCaseAnalyst: string = '';
  currCaseType: string = '';

  classifications: string[] = [
    'Desenvolvimento',
    'Extensão',
    'Pioneiro',
    'Especial',
  ];
  classificationSelected: string;

  protectionLevels: string[] = ['NP-1', 'NP-2', 'NP-3', 'NP-4'];
  protectionLevelSelected: string;

  completionTypes: string[] = [
    'Frac-Pack',
    'Simples Poço Revestido',
    'Completação Inteligente (2 zonas)',
    'Completação Inteligente (3 zonas)',
    'Gravel Pack',
    'Stand Alone',
    'PACS - Simples Poço Aberto',
    'Abandono Temporário',
    'Terra',
    'Marítimo',
    'Subsee',
    'PACI - Poço Aberto Completação Inteligente (2 zonas)',
    'PACI 2 + 1 - Poço Aberto Completação Inteligente (3 zonas)',
  ];
  completionTypeSelected: string;

  wellComplexity: number;
  expectedUsefulLife: number;
  criticalityInTheWell: boolean;
  workoverIntervention: boolean;

  waterDensity: number;
  surfaceDensity: number;
  surfaceSonic: number;
  surfaceDTS: number;

  constructor(public dialog: DialogService, private http: HttpClient) { }

  ngOnInit() {
    this.currCaseName = this.caseData.nome;
    this.currPocoName = this.caseData.dadosGerais.identificação.nomePoço;
    this.currPocoWebName = this.caseData.dadosGerais.identificação.nomePoçoWeb;
    this.currCaseType = this.caseData.tipoPoço;

    this.currCasePurpose = this.caseData.dadosGerais.identificação.finalidade;
    this.currCaseAnalyst = this.caseData.dadosGerais.identificação.analista;
    this.protectionLevelSelected = this.caseData.dadosGerais.identificação.nívelProteção;
    this.classificationSelected = this.caseData.dadosGerais.identificação.classificaçãoPoço;
    this.completionTypeSelected = this.caseData.dadosGerais.identificação.tipoCompletação;
    this.wellComplexity = this.caseData.dadosGerais.identificação.complexidadePoço;
    this.expectedUsefulLife = this.caseData.dadosGerais.identificação.vidaÚtilPrevista;
    this.criticalityInTheWell = this.caseData.dadosGerais.identificação.criticidadePoço;
    this.workoverIntervention = this.caseData.dadosGerais.identificação.intervençãoWorkover;

    this.importedCaseName = this.caseData.nome;
    this.importedCaseUrl = this.caseData.dadosGerais.identificação.poçoWebUrl;
    this.publicationDate = this.caseData.dadosGerais.identificação.poçoWebDtÚltimaAtualização;
    this.isUpdated = this.caseData.dadosGerais.identificação.poçoWebAtualizado;
    this.importedCaseUrlRevision = this.caseData.dadosGerais.identificação.poçoWebRevisãoUrl;

    this.waterDensity = this.caseData.dadosGerais.area.densidadeAguaMar;
    this.surfaceDensity = this.caseData.dadosGerais.area.densidadeSuperficie;
    this.surfaceSonic = this.caseData.dadosGerais.area.sonicoSuperficie;
    this.surfaceDTS = this.caseData.dadosGerais.area.dtsSuperficie;
  }

  getData() {
    const data: DadosGerais = <DadosGerais>{};

    data.identificação = {
      nome: this.currCaseName,
      nomePoço: this.currPocoName,
      nomePoçoWeb: this.caseData.dadosGerais.identificação.nomePoçoWeb,

      bacia: this.caseData.dadosGerais.identificação.bacia,
      campo: this.caseData.dadosGerais.identificação.campo,
      companhia: this.caseData.dadosGerais.identificação.companhia,
      sonda: this.caseData.dadosGerais.identificação.sonda,

      finalidade: this.currCasePurpose,
      analista: this.currCaseAnalyst,
      nívelProteção: this.protectionLevelSelected,
      classificaçãoPoço: this.classificationSelected,
      tipoCompletação: this.completionTypeSelected,
      complexidadePoço: this.wellComplexity,
      vidaÚtilPrevista: this.expectedUsefulLife,
      criticidadePoço: this.criticalityInTheWell,
      intervençãoWorkover: this.workoverIntervention,
    };
    data.area = {
      densidadeAguaMar: this.waterDensity,
      densidadeSuperficie: this.surfaceDensity,
      sonicoSuperficie: this.surfaceSonic,
      dtsSuperficie: this.surfaceDTS,
    };

    let hasAnyChange = false;
    Object.keys(data).forEach((el) => {
      Object.keys(data[el]).forEach((key) => {
        if (data[el][key] !== this.caseData.dadosGerais[el][key]) {
          hasAnyChange = true;
          return;
        }
      });
    });

    if (!hasAnyChange) {
      return {};
    }
    return {
      identificação: data.identificação,
      area: data.area,
    };
  }

  openDialogImportSestWeb() {
    this.dialog.openPageDialog(
      ImportComponent,
      { minHeight: 520, minWidth: 600 },
      { case: this.caseData, initialComponent: ImportSestWebComponent }
    );
  }

  openDialogUpdate() {
    // Pegar dado do request
    const url = `${environment.appUrl}/api/LerArquivoPoçoWeb`;

    this.http.get(url, { params: { urlArquivo: this.importedCaseUrlRevision } }).subscribe(response => {
      const fileData = fileInfoFormatter(response['retorno']);
      fileData.filePath = this.importedCaseUrlRevision;
      fileData.extras.poçoWeb.poçoWeb = true;

      this.dialog.openPageDialog(
        ImportComponent,
        { minHeight: 520, minWidth: 600 },
        {
          case: this.caseData,
          initialComponent: FileInfoComponent,
          initialData: fileData,
        }
      );

    });
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
}
