import { Component, OnInit, ViewChild } from '@angular/core';
import { Case } from 'app/repositories/models/case';
import { DialogService } from '@services/dialog.service';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { MatDialogRef } from '@angular/material';
import { NotybarService } from '@services/notybar.service';
import { ExportService } from 'app/repositories/export.service';
import { DatasetService } from '@services/dataset/dataset.service';

@Component({
  selector: 'sest-export',
  templateUrl: './export.component.html',
  styleUrls: ['./export.component.scss'],
})
export class ExportComponent implements OnInit {
  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  selected = 0;

  @ViewChild('profile', { static: true }) profileExport;
  @ViewChild('register', { static: true }) registerExport;
  @ViewChild('report', { static: true }) reportExport;

  loading = false;

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    public dialog: DialogService,
    public notybarService: NotybarService,
    private exportService: ExportService,
    private dataset: DatasetService,
  ) { }

  ngOnInit() {
    // Update the currently opened file
    this.currCase = this.dataset.getById(this.dataset.currCaseId);
  }

  submit() {
    this.loading = true;
    if (this.selected === 0) {
      const options = this.profileExport.getOptions();
      if (options) {
        this.exportService.export(options).then((response) => {
          this.baixarArquivo(response.body, this.currCase.nome, options.arquivo.toLowerCase(), true);
          // this.notybarService.show(
          //   'Exportação concluída - o arquivo foi salvo na sua pasta de downloads.',
          //   'success'
          // );
          this.loading = false;
          // this.closeModal();
        }).catch(() => { this.loading = false; });
      } else {
        this.loading = false;
      }
    } else if (this.selected === 1) {
      const registros = this.registerExport.getRegistros();
      if (registros) {
        this.exportService.exportarRegistros(this.dataset.currCaseId, registros).then((response) => {
          this.baixarArquivo(response.body, this.currCase.nome, 'csv', true);
          // this.notybarService.show(
          //   'Exportação concluída - o arquivo foi salvo na sua pasta de downloads.',
          //   'success'
          // );
          this.loading = false;
          // this.closeModal();
        }).catch(() => { this.loading = false; });
      } else {
        this.loading = false;
      }
    } else {
      this.reportExport.generateReport();

      this.exportService.exportarRelatorio(this.exportService.relatorio).then((response) => {
        this.baixarArquivo(response.body, this.currCase.nome, this.exportService.relatorio.extensao.toLowerCase());
        // this.notybarService.show(
        //   'Exportação de relatório concluída - o arquivo foi salvo na sua pasta de downloads.',
        //   'success'
        // );
        this.loading = false;
        // this.closeModal();
      }).catch(() => { this.loading = false; });
    }
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  visualizar() {
    this.loading = true;
    this.reportExport.generateReport();

    this.exportService.preRelatorio(this.exportService.relatorio).then((res) => {
      this.dialog.openImageDialog({ title: 'Pré-visualização', imagePath: res.caminho })
      this.loading = false;
    }).catch(() => { this.loading = false; });
  }

  baixarArquivo(arquivo, nome: string, extensao: string, binary?: boolean) {
    if (binary) {
      arquivo = new Blob(['\ufeff', arquivo]);
    }
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(arquivo);
    link.download = `${nome}.${extensao}`;
    link.click();
    link.remove();
  }
}
