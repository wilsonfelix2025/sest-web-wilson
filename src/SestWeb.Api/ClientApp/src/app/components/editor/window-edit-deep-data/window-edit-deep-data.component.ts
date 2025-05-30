import { Component, OnInit, NgModule, Inject, ViewChild } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DatasetService } from '@services/dataset/dataset.service';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { EditProfileStyleComponent } from './edit-profile-style/edit-profile-style.component';
import { ProfileService } from 'app/repositories/profile.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { CalculationDatasetService } from '@services/dataset/calculation-dataset.service';
import { TrajectoryDatasetService } from '@services/dataset/trajectory-dataset.service';
import { UNSET } from '@utils/vazio';

@Component({
  selector: 'sest-window-edit-deep-data',
  templateUrl: './window-edit-deep-data.component.html',
  styleUrls: ['./window-edit-deep-data.component.scss']
})
export class WindowEditDeepDataComponent implements OnInit {

  /**
   * Tipo do dado de profundidade sendo editado.
   */
  tipo: tipo = 'Litologia';

  /**
   * O dado de profundidade atual.
   * 
   * Pode ser trajetoria, perfil ou litogia.
   */
  dadoDeProfundidade;

  caseId: string;

  calculado: boolean = false;

  @ViewChild('editStyle', { static: false }) editStyle: EditProfileStyleComponent;

  loading = false;

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    public dataset: DatasetService,
    public profileService: ProfileService,
    private profileDataset: ProfileDatasetService,
    private calculationDataset: CalculationDatasetService,
    private trajectoryDataset: TrajectoryDatasetService,
    @Inject(MAT_DIALOG_DATA) public dialogData: { data: { tipo: tipo, id?: string, caseId: string } }
  ) { }

  ngOnInit() {
    this.tipo = this.dialogData.data.tipo;
    this.caseId = this.dialogData.data.caseId;

    if (this.tipo === 'Trajetória') {
      this.dadoDeProfundidade = this.trajectoryDataset.get(this.caseId);
    } else {
      this.dadoDeProfundidade = JSON.parse(JSON.stringify(this.dataset.getById(this.dialogData.data.id)));

      this.calculado = this.calculationDataset.getAll(this.caseId).findIndex(calculo =>
        calculo.perfisSaída.idPerfis.includes(this.dadoDeProfundidade.id)
      ) >= 0 && this.dadoDeProfundidade.mnemonico !== 'GPPI';
    }
    // console.log('Edit deep', this.tipo, this.dadoDeProfundidade);
  }

  commitVisualStyle() {
    if (this.editStyle !== undefined) {
      this.editStyle.commitVisualStyle();
    }
    // console.log(this.dadoDeProfundidade);
  }

  submit() {
    this.loading = true;
    if (this.tipo === 'Perfil') {
      this.commitVisualStyle();
      this.profileDataset.update(this.dadoDeProfundidade);
      this.profileService.edit(this.dadoDeProfundidade).then(res => {
        // this.profileDataset.update(res.perfil);

        res.perfisAlterados = res.perfisAlterados.filter(el => el.id === res.perfil.id);
        if (!UNSET(res.perfisAlterados)) {
          this.profileDataset.updateList(res.perfisAlterados);
        }
        this.loading = false;
        this.closeModal();
      }).catch(() => { this.loading = false; });
    } else {
      this.loading = false;
      this.closeModal();
    }
  }

  closeModal(): void {
    this.dialogRef.close();
  }

}

export type tipo = 'Perfil' | 'Trajetória' | 'Litologia';