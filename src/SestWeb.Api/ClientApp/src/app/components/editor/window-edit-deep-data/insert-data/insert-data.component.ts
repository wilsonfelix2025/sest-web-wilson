import { Component, OnInit, Inject } from '@angular/core';
import { DialogService } from '@services/dialog.service';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { DialogBaseComponent } from '@components/common/dialog-base/dialog-base.component';
import { tipo, WindowEditDeepDataComponent } from '../window-edit-deep-data.component';
import { DatasetService } from '@services/dataset/dataset.service';
import { TableOptions } from '@utils/table-options-default';
import { ProfileService } from 'app/repositories/profile.service';
import { Litologia } from 'app/repositories/models/litologia';
import { Perfil } from 'app/repositories/models/perfil';
import { Trajetoria } from 'app/repositories/models/trajetoria';
import { TrajectoryDatasetService } from '@services/dataset/trajectory-dataset.service';
import { Case } from 'app/repositories/models/case';
import { LithologyDatasetService } from '@services/dataset/lithology-dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { NumberUtils } from '@utils/number';
import { UNSET } from '@utils/vazio';
import { CaseService } from 'app/repositories/case.service';
import { CaseDatasetService } from '@services/dataset/case-dataset.service';

@Component({
    selector: 'sest-insert-data',
    templateUrl: './insert-data.component.html',
    styleUrls: ['./insert-data.component.scss']
})
export class InsertDataComponent implements OnInit {

    /**
     * O caso de estudo aberto atualmente.
     */
    currCase: Case;

    tipo: tipo;

    /**
     * Tabela.
     */
    hotTable: Handsontable;

    tipos = {
        'Perfil': {
            colHeaders: ['PM (m)', /*'PV (m)',*/ 'Valor'],
            columnsTypes: [
                { data: 'pm.valor' },
                // { data: 'pv.valor' },
                { data: 'valor' },
            ],
        },
        'Trajetória': {
            colHeaders: ['PM (m)', /*'PV (m)',*/ 'Inclinação (◦)', 'Azimute (◦)'],
            columnsTypes: [
                { data: 'pm.valor', type: 'numeric', numericFormat: { pattern: '0[.][00000]' } },
                // { data: 'pv' },
                { data: 'inclinação', type: 'numeric', numericFormat: { pattern: '0[.][00000]' } },
                { data: 'azimute', type: 'numeric', numericFormat: { pattern: '0[.][00000]' } },
            ],
        },
        'Litologia': {
            colHeaders: ['PM Topo (m)', 'Código'/*, 'Litologia'*/],
            columnsTypes: [
                { data: 'pm' },
                { data: 'codigo' },
                // { data: 'litologia' },
            ],
        },
    };

    options: any = TableOptions.createDefault({
        height: 300,
        rowHeaderWidth: 15,
        manualColumnResize: [],
        filters: false,
        minSpareRows: 1,
        contextMenu: {
            items: {
                'row_above': {
                    name: 'Inserir linha acima'
                },
                'row_below': {
                    name: 'Inserir linha abaixo'
                },
                'remove_row': {
                    name: 'Remover linha'
                },
            }
        },
    });

    tableData: any[] = [];

    nome = '';

    loading = false;

    dadosPoco = {
        pmMax: 0,
        pmSup: 0,
        cotaFinal: 0,
        cotaSup: 0,
        cotaMax: 0,
    };

    constructor(
        public dialogRef: MatDialogRef<DialogBaseComponent>,
        @Inject(MAT_DIALOG_DATA) public dialogData: { data: { tipo: tipo, data } },
        public dialog: DialogService,

        private caseService: CaseService,
        private profileService: ProfileService,

        private dataset: DatasetService,
        private caseDataset: CaseDatasetService,
        private trajectoryService: TrajectoryDatasetService,
        private profileDataset: ProfileDatasetService,
        private lithologyDataset: LithologyDatasetService,
    ) { }

    ngOnInit() {
        this.currCase = this.dataset.getById(this.dataset.currCaseId);

        this.tipo = this.dialogData.data.tipo;

        this.dadosPoco = this.caseDataset.getLimiteCaso(this.dataset.currCaseId);

        if (this.tipo === 'Trajetória') {
            this.pegarDadosTrajetoria(this.dialogData.data.data);
        } else if (this.tipo === 'Litologia') {
            this.pegarDadosLitologia(this.dialogData.data.data);
        } else {
            this.pegarDadosPerfil(this.dialogData.data.data);
        }
    }

    pegarDadosTrajetoria(trajetoria: Trajetoria) {
        this.tableData = JSON.parse(JSON.stringify(trajetoria.pontos));
    }

    pegarDadosLitologia(litologia: Litologia) {
        this.tableData = litologia.pontos.map(ponto =>
            ({ 'pm': ponto.pm.valor, 'codigo': ponto.tipoRocha.numero, 'litologia': ponto.tipoRocha.nome }));
    }

    pegarDadosPerfil(perfil: Perfil) {
        this.tableData = JSON.parse(JSON.stringify(perfil.pontos));
        this.options.afterCreateRow = (index) => {
            this.tableData[index].origem = 'Importado';
            this.tableData[index].pv.valor = 0;
        };
        this.nome = this.dialogData.data.data.nome;
        this.tipos[this.tipo].colHeaders[1] += ` (${perfil.grupoDeUnidades.unidadePadrão.símbolo})`;
    }

    closeEditModal(goBackToEdit = true): void {
        if (!goBackToEdit || this.tipo !== 'Perfil') {
            this.dialogRef.close();
        } else {
            this.dialog.openPageDialog(WindowEditDeepDataComponent, { minHeight: 450, minWidth: 400 },
                { tipo: this.tipo, id: this.dialogData.data.data.id });
            this.dialogRef.close();
        }
    }

    submit() {
        this.loading = true;
        if (this.tipo === 'Trajetória') {
            this.salvarTrajetoria();
        } else if (this.tipo === 'Litologia') {
            this.salvarLitologia();
        } else {
            this.salvarPerfil();
        }
    }

    salvarTrajetoria() {
        const finalPontos = [];
        for (const ponto of this.tableData) {
            if (NumberUtils.isNumber(ponto.pm.valor) && NumberUtils.isNumber(ponto.pm.valor) && NumberUtils.isNumber(ponto.pm.valor)) {
                delete ponto['pv'];
                finalPontos.push(ponto);
            }
        }

        this.caseService.updateTrajectory(this.currCase.id, finalPontos).then(resp => {
            this.trajectoryService.update(resp['trajetória']);

            if (!UNSET(resp['perfisAlterados'])) {
                this.profileDataset.updateList(resp['perfisAlterados']);
            }

            if (!UNSET(resp['litologias'])) {
                resp['litologias'].forEach(el => {
                    this.lithologyDataset.update(el, this.currCase.id);
                });
            }
            this.loading = false;
            this.closeEditModal(false);
        }).catch(() => { this.loading = false; });
    }

    salvarLitologia() {
        const newPontos = this.tableData.filter(ponto => NumberUtils.isNumber(ponto.pm) && NumberUtils.isNumber(ponto.codigo));

        this.caseService.updateLithology(
            this.currCase.id,
            this.dialogData.data.data.classificação.nome,
            newPontos
        ).then(resp => {
            this.lithologyDataset.update(resp['litologia'], this.currCase.id);

            if (!UNSET(resp['perfisAlterados'])) {
                this.profileDataset.updateList(resp['perfisAlterados']);
            }
            this.loading = false;
            this.closeEditModal(false);
        }).catch(() => { this.loading = false; });
    }

    salvarPerfil() {
        const _perfil: Perfil = this.dialogData.data.data;
        _perfil.pontos = this.tableData.filter(el => NumberUtils.isNumber(el.pm.valor) && NumberUtils.isNumber(el.valor));
        this.profileDataset.update(_perfil);
        this.profileService.edit(_perfil).then(res => {
            this.profileDataset.update(res.perfil);

            if (!UNSET(res.perfisAlterados)) {
                this.profileDataset.updateList(res.perfisAlterados);
            }
            this.loading = false;
        }).catch(() => { this.loading = false; });
        this.closeEditModal(false);
    }
}
