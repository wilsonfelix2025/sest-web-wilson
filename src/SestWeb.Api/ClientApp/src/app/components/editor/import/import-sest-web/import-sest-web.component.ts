import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { FileInfoComponent } from '../shared/file-info/file-info.component';
import { HttpClient } from '@angular/common/http';
import { fileInfoFormatter } from '../shared/file-info-formatter';
import { StringUtils } from '@utils/string';
import { Case } from 'app/repositories/models/case';
import { DatasetService } from '@services/dataset/dataset.service';
import { environment } from 'environments/environment';

@Component({
  selector: 'app-import-sest-web',
  templateUrl: './import-sest-web.component.html',
  styleUrls: ['./import-sest-web.component.scss']
})
export class ImportSestWebComponent implements OnInit {

  @Output() someEvent = new EventEmitter<any>();

  currCase: Case;
  /**
   * O que o usuário digiou na barra de busca.
   */
  userSearch: string = '';
  /**
   * Arquivo que o usuário selecionou.
   */
  selectedFile: Arquivo;

  /**
   * Lista de arquivos do poçoweb.
   */
  filesList: Arquivo[] = [];
  /**
   * Lista de arquivos filtrados pela barra de busca.
   */
  filtredFilesList: Arquivo[] = [];

  constructor(
    private http: HttpClient,
    private dataset: DatasetService
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);
    this.getFiles();
  }

  /**
   * Seleciona um arquivo da lista de arquivos.
   *
   * Pega as informações do back e retorna para processo de importação.
   *
   * @param item arquivo selecionado
   */
  selectSearchResult(item: Arquivo) {
    this.selectedFile = item;
    // Pegar dado do request
    const url = `${environment.appUrl}/api/LerArquivoPoçoWeb`;

    this.http.get(url, { params: { urlArquivo: item.url } }).subscribe(response => {
      const fileData = fileInfoFormatter(response['retorno']);
      fileData.filePath = item.url;
      fileData.extras.poçoWeb.poçoWeb = true;
      this.someEvent.emit({ cangoNext: true, type: { page: FileInfoComponent }, data: fileData });
    });
  }

  /**
   * Remove o arquivo seleção do arquivo.
   */
  unselect() {
    this.selectedFile = undefined;
    this.someEvent.emit({ cangoNext: false, type: { page: FileInfoComponent } });
  }

  /**
   * Limpa a barra de busca.
   */
  clearSearch() {
    this.userSearch = '';
    this.search();
  }

  /**
   * Filtra a lista de arquivos com base no que o usuário digitou na barra de busca.
   */
  search() {
    this.filtredFilesList = this.filesList.filter(el => {
      return StringUtils.toSlug(el.name).match(new RegExp(StringUtils.toSlug(this.userSearch), 'g')) !== null;
    });
  }

  /**
   * Pega a lista de arquivos do poçoweb.
   */
  getFiles() {
    const url = `${environment.appUrl}/api/pocoweb/files/get-files`;

    let tipoArquivo = 'drilling';
    if (this.currCase.tipoPoço === 'Retroanalise') {
      tipoArquivo = 'asbuilt';
    }

    this.http.get(url, { params: { tipoArquivo: tipoArquivo } }).subscribe(response => {
      this.filesList = response['listaArquivos'];
      this.search();
    });
  }

}

interface Arquivo {
  id: string;
  rev: string;
  name: string;
  wellId: string;
  fileType: string;
  url: string;
}
