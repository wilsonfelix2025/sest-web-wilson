import { Component, ViewChild, ElementRef, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileData, ImportData } from '@utils/interfaces';
import { HttpEventType, HttpResponse } from '@angular/common/http';
import { Subject, Observable } from 'rxjs';
import { FileInfoComponent } from '../../shared/file-info/file-info.component';
import { InfoSpreadsheetTrajectoryComponent } from '../info-spreadsheet-trajectory/info-spreadsheet-trajectory.component';
import { InfoSpreadsheetLithologyComponent } from '../info-spreadsheet-lithology/info-spreadsheet-lithology.component';
import { InfoSpreadsheetProfileComponent } from '../info-spreadsheet-profile/info-spreadsheet-profile.component';
import { fileInfoFormatter } from '../../shared/file-info-formatter';
import { Class } from 'highcharts';
import { Case } from 'app/repositories/models/case';
import { DatasetService } from '@services/dataset/dataset.service';
import { WellSelectorComponent } from '../well-selector/well-selector.component';
import { TrajectoryDatasetService } from '@services/dataset/trajectory-dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { LithologyDatasetService } from '@services/dataset/lithology-dataset.service';
import { ImportService } from 'app/repositories/import.service';

@Component({
  selector: 'app-select-type',
  templateUrl: './select-type.component.html',
  styleUrls: ['./select-type.component.scss'],
})
export class SelectTypeComponent implements OnInit {
  /**
   * Os dados recebidos do poço aberto atualmente.
   */
  @Output() someEvent = new EventEmitter<any>();

  /**
   * Tipos de importações.
   *
   * Contém seu nome, o respectivo componente e a extensão do arquivo caso necessário.
   */
  importTypes: Type[] = [
    {
      name: 'Dados via planilha',
      next: [
        { name: 'Trajetória', page: InfoSpreadsheetTrajectoryComponent },
        { name: 'Litologia', page: InfoSpreadsheetLithologyComponent },
        { name: 'Perfis', page: InfoSpreadsheetProfileComponent },
      ],
    },
    {
      name: 'Arquivo Formatado',
      next: [
        {
          name: 'LAS',
          next: [
            { name: 'Trajetória', extension: '.las' },
            { name: 'Litologia / Perfis', page: FileInfoComponent, extension: '.las' },
          ],
        },
        { name: 'SIGEO', page: FileInfoComponent, extension: '.txt' },
        { name: 'Sest 5', page: FileInfoComponent, extension: '.xml' },
        { name: 'Sest TR 1', page: FileInfoComponent, extension: '.xsrt' },
        { name: 'Sest TR 2', page: WellSelectorComponent, extension: '.xsrt2' },
      ],
      fileRequired: true,
    },
  ];

  /**
   * Dados existentes no caso de estudo atual.
   */
  caseDataValid = [
    { name: 'Dados Gerais', exist: false },
    { name: 'Trajetória', exist: false },
    { name: 'Perfis', exist: false },
    { name: 'Objetivos', exist: false },
    { name: 'Sapatas', exist: false },
    { name: 'Estratigrafia', exist: false },
    { name: 'Diâmetro de Broca', exist: false },
    { name: 'Registros', exist: false },
    { name: 'Litologia', exist: false },
  ];

  /**
   * Os dados do arquivo aberto.
   */
  fileData: FileData;

  /**
   * Tipos de importação nos grupos de radio button.
   *
   * Cada elemeto representa um grupo.
   */
  typesRadio: Type[] = [this.importTypes[0], null, null];

  /**
   * Tipo de importação selecionado.
   */
  selectedType: Type = this.importTypes[0];

  /**
   * Se foi preenchido todo o necessário para avançar para o próximo passo.
   */
  canGoNext: boolean = false;

  /**
   * O input do html que recebe o arquivo.
   */
  @ViewChild('fileInput', { static: true }) fileInput: ElementRef<HTMLElement>;

  /**
   * Arquivo aberto.
   */
  file: File;

  /**
   * Nome do arquivo aberto.
   */
  fileName: string = '';

  /**
   * Os dados do poço aberto atualmente.
   */
  currCase: Case;

  /**
   * Observedor de progresso do upload do arquivo.
   */
  uploadProgress: Observable<number>;

  constructor(
    public importService: ImportService,
    private dataset: DatasetService,

    private profileDataset: ProfileDatasetService,
    private trajectoryDataset: TrajectoryDatasetService,
    private lithologyDataset: LithologyDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);
    this.currCaseDataIsValid();
  }

  /**
   * Verifica no caso atual quais dados existem e são válidos.
   */
  currCaseDataIsValid() {
    this.caseDataValid[0].exist = this.currCase.dadosGerais !== undefined;
    this.caseDataValid[1].exist = this.trajectoryDataset.get(this.currCase.id) !== undefined && this.trajectoryDataset.get(this.currCase.id).count > 0;
    this.caseDataValid[2].exist = this.profileDataset.getAll(this.currCase.id).length > 0;
    this.caseDataValid[3].exist = this.currCase.objetivos.length > 0;
    this.caseDataValid[4].exist = this.currCase.sapatas.length > 0;
    this.caseDataValid[5].exist = Object.keys(this.currCase.estratigrafia.Itens).length > 0;
    this.caseDataValid[6].exist = this.profileDataset.getAll(this.currCase.id).findIndex((el) => el.mnemonico === 'DIAM_BROCA') >= 0;
    // this.caseDataValid[7].exist = this.currCase;
    this.caseDataValid[8].exist =
      this.lithologyDataset.getAll(this.currCase.id).length > 0 && this.lithologyDataset.getAll(this.currCase.id).findIndex((el) => el.pontos.length > 0) > 0;
  }

  exportData(): ImportData {
    return {
      caminhoDoArquivo: this.fileData.filePath,
      poçoId: this.currCase.id,
      tipoProfundidade: 'PM',
      dadosSelecionados: ['Trajetória'],
      listaLitologias: [],
      listaPerfis: [],
    };
  }

  /**
   * Altera o tipo de arquivo selecionado e emite um evento com as alterações.
   * @param {Type} selectedType - novo tipo de arquivo selecionado.
   */
  change(selectedType: Type) {
    if (selectedType !== this.selectedType) {
      this.clearFileData();
    }
    this.selectedType = selectedType;
    this.canGoNext =
      this.selectedType.next === undefined && (!this.typesRadio[0].fileRequired || this.fileData !== undefined);
    this.someEvent.emit({ cangoNext: this.canGoNext, type: this.selectedType, data: this.fileData });
  }

  /**
   * Limpa os dados de um arquivo selecionado anteriormente
   */
  clearFileData() {
    this.fileData = undefined;
    this.fileName = '';
    this.uploadProgress = undefined;
    this.fileInput.nativeElement['value'] = null;
  }

  /**
   * Evento para abrir o dialogo de seleção de arquivo.
   */
  uploadFileClicked() {
    this.clearFileData();
    this.fileInput.nativeElement.click();
  }

  /**
   * Sobe o arquivo selecionado para o banco de dados.
   * @param files - Evento do input de arquivo.
   */
  uploadFile(files: FileList) {
    // Se o usuário tiver selecionado um arquivo.
    if (files && files[0]) {
      this.file = files.item(0);
      this.fileName = this.file.name;

      // create a new progress-subject for every file
      const progress = new Subject<number>();

      // send the http-request and subscribe for progress-updates
      this.importService.uploadFile(this.file).subscribe((event) => {
        // this.http.request(req).subscribe(event => {
        if (event.type === HttpEventType.UploadProgress) {
          // calculate the progress percentage
          const percentDone = Math.round((100 * event.loaded) / event.total);
          // pass the percentage into the progress-stream
          progress.next(percentDone);
        } else if (event instanceof HttpResponse) {
          // Close the progress-stream if we get an answer form the API
          // The upload is complete
          progress.complete();

          if (event.body && event.body['retorno']) {
            this.fileData = fileInfoFormatter(event.body['retorno']);

            if (this.selectedType.extension === '.las') {
              this.fileData.hasFields = false;
            }
            this.fileData.filePath = event.body['caminho'];

            this.change(this.selectedType);
          }
        }
      });

      // Save every progress-observable in a map of all observables
      this.uploadProgress = progress.asObservable();
    }
  }
}

interface Type {
  name: string;
  next?: Type[];
  page?: Class<any>;
  extension?: string;
  fileRequired?: boolean;
}
