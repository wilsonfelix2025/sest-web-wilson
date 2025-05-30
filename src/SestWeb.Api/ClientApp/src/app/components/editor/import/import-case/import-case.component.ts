import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { FileInfoComponent } from '../shared/file-info/file-info.component';
import { StringUtils } from '@utils/string';
import { ExplorerNavigatorService } from '@services/explorer-navigator.service';
import { CaseService } from 'app/repositories/case.service';
import { Case, CaseSlim } from 'app/repositories/models/case';
import { DatasetService } from '@services/dataset/dataset.service';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';
import { environment } from 'environments/environment';


@Component({
  selector: 'app-import-case',
  templateUrl: './import-case.component.html',
  styleUrls: ['./import-case.component.scss']
})
export class ImportCaseComponent implements OnInit {

  @Output() someEvent = new EventEmitter<any>();

  currCase: Case;
  /**
   * O que o usuário digiou na barra de busca.
   */
  userSearch: string = '';
  /**
   * Arquivo que o usuário selecionou.
   */
  selectedCase: CaseSlim;

  /**
   * Lista de arquivos do sestweb.
   */
  caseList: CaseSlim[] = [];
  /**
   * Lista de arquivos filtrados pela barra de busca.
   */
  filteredCaseList: CaseSlim[] = [];

  loading = false;

  constructor(
    public dialogRef: MatDialogRef<ImportCaseComponent>,
    public nav: ExplorerNavigatorService,
    public caseService: CaseService,
    public caseDataset: CaseDatasetService,
    private dataset: DatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);
    this.getFiles();
  }

  /**
   * Seleciona um arquivo da lista de arquivos.
   *
   *
   * @param item arquivo selecionado
   */
  selectSearchResult(item: CaseSlim) {
    this.selectedCase = item;
    // Pegar dado do request
    const url = `${environment.appUrl}/api/`;

  }

  /**
   * Remove o arquivo seleção do arquivo.
   */
  unselect() {
    this.selectedCase = undefined;
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
    this.filteredCaseList = this.caseList.filter(el => {
      return StringUtils.toSlug(el.nome).match(new RegExp(StringUtils.toSlug(this.userSearch), 'g')) !== null;
    });
  }

  /**
   * Pega a lista de arquivos do sestweb.
   */
  getFiles() {
    this.caseService.getAll().then(res => {
      const caseIdList = this.dataset.getCasesIds();
      // console.log('List', res.poços, caseIdList);
      this.caseList = res.poços.filter(el => !caseIdList.includes(el.id));
      // this.caseList = this.caseList.filter(el => el.tipoPoço === 'Retroanalise');
      this.search();
    });

  }

  /**
   * Fechar dialogo.
   */
  onNoClick(): void {
    this.dialogRef.close();
  }

  /**
   * Submit.
   */
  submit() {
    this.loading = true;
    this.caseService.addSupport(this.currCase.id, this.selectedCase.id).then(_ => {
      this.caseService.get(this.selectedCase.id).then(el => {
        // console.log('IMPORTANDO', el);
        this.caseDataset.add(el.poço, el.caminho);

        this.loading = false;
        this.onNoClick();
      }).catch(() => { this.loading = false; });
    });
  }


}
