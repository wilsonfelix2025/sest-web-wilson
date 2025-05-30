import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { DialogService } from '@services/dialog.service';
import { EditCorrelationComponent } from '../edit-correlation/edit-correlation.component';
import { CorrelationService } from 'app/repositories/correlation.service';
import { Correlation } from 'app/repositories/models/correlation';
import { TIPOS_PERFIS } from '@utils/perfil/tipo-perfil';
import { GRUPO_PERFIS } from '@utils/perfil/grupo-perfis';
import { DialogDeleteHomeComponent } from '@components/editor/dialog-delete-home/dialog-delete-home.component';
import { OAuthTokenService } from '@services/oauth.service';
import { Case } from 'app/repositories/models/case';
import { DatasetService } from '@services/dataset/dataset.service';

@Component({
  selector: 'sest-select-correlation',
  templateUrl: './select-correlation.component.html',
  styleUrls: ['./select-correlation.component.scss']
})
export class SelectCorrelationComponent implements OnInit {
  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  usuarioLogado: string;
  chaveUsuarioLogado: string;

  profileTypes = TIPOS_PERFIS;

  tabs = [
    {
      name: 'Perfis',
      hasFilter: false,
      data: []
    },
    {
      name: 'Propriedades Mecânicas',
      hasFilter: true,
      data: []
    },
  ];

  displayedColumns: string[] = ['select', 'type', 'name', 'origem'];
  selectedCorrelaction: Correlation;
  groupsPropMec = [
    { mnemônico: 'ANGAT', descrição: 'Ângulo de Atrito' },
    { mnemônico: 'UCS', descrição: 'Resistência à Compressão Simples' },
    { mnemônico: 'COESA', descrição: 'Coesão' },
    { mnemônico: 'RESTR', descrição: 'Resistência à Tração' },
    { mnemônico: '', descrição: 'Propriedades Elásticas' },
  ];
  selectedGroup = this.groupsPropMec[0];

  constructor(
    public dialogRef: MatDialogRef<SelectCorrelationComponent>,
    public dialog: DialogService,
    public correlationService: CorrelationService,
    public auth: OAuthTokenService,

    private dataset: DatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);
    this.getAllData();
    this.getCurrUser();

  }

  getCurrUser() {
    this.chaveUsuarioLogado = this.auth.loggedUser !== null ? this.auth.loggedUser.preferred_username : 'Usuario';
    this.usuarioLogado = this.auth.loggedUser !== null ? this.auth.loggedUser.preferred_username : 'Usuario';
  }

  getAllData() {
    this.correlationService.getAllSlim(this.currCase.id).then(res => {
      this.selectedCorrelaction = undefined;
      this.correlationService.list = res;
      this.tabs[0].data = this.correlationService.list.filter(el =>
        TIPOS_PERFIS[el.perfilSaída].grupoPerfil.nome !== GRUPO_PERFIS.grupoPropMec.nome);
      this.applyFilter(this.selectedGroup, this.tabs[1]);
    });
  }

  onNoClick(): void {
    this.dialogRef.close();
  }

  applyFilter(group, tab) {
    if (group.mnemônico === '') {
      tab.data = this.correlationService.list.filter(el =>
        TIPOS_PERFIS[el.perfilSaída].grupoPerfil.nome === GRUPO_PERFIS.grupoPropMec.nome &&
        !['ANGAT', 'UCS', 'COESA', 'RESTR'].includes(el.perfilSaída));
    } else {
      tab.data = this.correlationService.list.filter(el =>
        TIPOS_PERFIS[el.perfilSaída].grupoPerfil.nome === GRUPO_PERFIS.grupoPropMec.nome &&
        el.perfilSaída === group.mnemônico);
    }
  }

  removeCorrelation(parameters: { correlation: Correlation }) {
    this.correlationService.remove(parameters.correlation).then(res => {
      this.selectedCorrelaction = undefined;
      this.getAllData();
    });
  }

  publishCorrelation() {
    this.correlationService.publish(this.currCase.id, this.selectedCorrelaction).then(res => {
      this.selectedCorrelaction = undefined;
      this.getAllData();
    });
  }

  openDialogRemoveCorrelation() {
    this.dialog.openDialogGeneric(DialogDeleteHomeComponent, this, 'removeCorrelation', { correlation: this.selectedCorrelaction });
  }

  openWindowCreateCorrelaction() {
    this.dialog.openPageDialog(EditCorrelationComponent, { minHeight: 700, minWidth: 1024 }, { case: this.currCase, context: this });
  }

  openWindowEditCorrelaction() {
    this.dialog.openPageDialog(EditCorrelationComponent, { minHeight: 700, minWidth: 1024 },
      { case: this.currCase, correlation: this.selectedCorrelaction, context: this });
  }
}
