import { Component, Inject, OnInit, ViewChild, ViewChildren } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { InsertInitialData } from '@utils/interfaces';
import { Case } from 'app/repositories/models/case';
import { NotybarService } from '@services/notybar.service';
import { MontagemPerfisComponent } from './montagem-perfis/montagem-perfis.component';
import { Perfil } from 'app/repositories/models/perfil';
import { DatasetService } from '@services/dataset/dataset.service';
import { SelecaoPerfisComponent } from './selecao-perfis/selecao-perfis.component';
import { MontagemPerfisService } from 'app/repositories/montagem-perfis.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { LithologyDatasetService } from '@services/dataset/lithology-dataset.service';
import { InitialDataService } from 'app/repositories/initial-data.service';

@Component({
  selector: 'app-window-initial-data',
  templateUrl: './window-initial-data.component.html',
  styleUrls: ['./window-initial-data.component.scss']
})
export class WindowInitialDataComponent implements OnInit {

  /**
   * O caso de estudo aberto atualmente
   */
  currCase: Case;

  currTab: number = 0;

  stepPerfis: number = 0;

  tipoPoco: string;

  casosSelecionados;

  @ViewChildren('tab') tabs;

  @ViewChild('montagem', { static: false }) montagem: MontagemPerfisComponent;

  @ViewChild('selecao', { static: false }) selecao: SelecaoPerfisComponent;

  loading = false;

  constructor(
    public dialogRef: MatDialogRef<WindowInitialDataComponent>,
    public notybarService: NotybarService,

    private dataset: DatasetService,
    private profileDataset: ProfileDatasetService,
    private lithologyDataset: LithologyDatasetService,

    public initialDataService: InitialDataService,
    private montagemService: MontagemPerfisService,

    @Inject(MAT_DIALOG_DATA) public dialogData: { data: { caseId: string } }
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dialogData.data.caseId);
    this.tipoPoco = this.currCase.tipoPoço;
    // console.log(this.tipoPoco);
  }

  close(): void {
    this.dialogRef.close();
  }

  changeStep(newStep: string): void {
    if (newStep === 'increment' && this.stepPerfis < 1) {
      this.montagem.pegarCasos().then(casos => {
        if (!casos.cancelado) {
          if (casos.valido) {
            this.casosSelecionados = casos.trechos;
            this.stepPerfis = this.stepPerfis + 1;
          } else {
            this.notybarService.show('Conserte os erros na tabela.', 'warning');
          }
        }
      });
    }
    else if (newStep === 'decrement' && this.stepPerfis > 0) {
      this.stepPerfis = this.stepPerfis - 1;
    }
  }

  submit() {
    this.loading = true;
    const data: any = <InsertInitialData>{};

    this.tabs._results.forEach(tab => {
      if (tab && tab.getData) {
        Object.assign(data, tab.getData());
      }
    });
    if (Object.keys(data).length > 0) {
      this.initialDataService.insertInitialData(data).then(res => {
        this.loading = false;
        const perfil: Perfil = res.perfil;
        this.profileDataset.add(perfil, this.currCase.id);
        this.close();
      }).catch(() => { this.loading = false; });
    } else {
      this.loading = false;
    }
  }

  finalizar() {
    this.loading = true;
    const montagem = this.selecao.pegarMontagemSelecionada();
    console.log(montagem);

    if (montagem.valido) {
      this.montagemService.montarPerfis(montagem.montagem).then(res => {
        this.loading = false;
        console.log('MONTOU', res);
        res.listaPerfis.forEach(perfil => {
          this.profileDataset.add(perfil, this.currCase.id);
        });
        this.lithologyDataset.add(res.litologia, this.currCase.id);
        this.close();
      }).catch(() => { this.loading = false; });
    } else {
      this.loading = false;
    }
  }

}
