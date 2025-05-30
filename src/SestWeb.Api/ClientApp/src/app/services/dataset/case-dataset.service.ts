import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import { SynchronizeChartsService } from '@services/synchronize-charts.service';

import { DatasetService } from '@services/dataset/dataset.service';
import { TrajectoryDatasetService } from './trajectory-dataset.service';
import { LithologyDatasetService } from './lithology-dataset.service';
import { ProfileDatasetService } from './profile-dataset.service';
import { CalculationDatasetService } from './calculation-dataset.service';
import { StratigraphyDatasetService } from './stratigraphy-dataset.service';
import { RegisterEventDatasetService } from './register-event-dataset.service';

import { TabsDatasetService } from './state/tabs-dataset.service';
import { TreeDatasetService } from './state/tree-dataset.service';

import { Case } from 'app/repositories/models/case';
import { DatasetCase } from './interfaces';
import { CaseService } from 'app/repositories/case.service';
import { Caminho } from 'app/repositories/models/caminho';
import { StateService } from './state/state.service';
import { NumberUtils } from '@utils/number';
import { NotybarService } from '@services/notybar.service';

@Injectable({
    providedIn: 'root'
})
export class CaseDatasetService {

    $currCaseLoaded = new Subject<string>();

    $caseAdded = new Subject<Case>();
    $caseRemoved = new Subject<string>();

    constructor(
        private caseService: CaseService,
        private dataset: DatasetService,
        private syncChartsService: SynchronizeChartsService,
        private notybarService: NotybarService,

        private trajectoryDataset: TrajectoryDatasetService,
        private lithologyDataset: LithologyDatasetService,
        private profileDataset: ProfileDatasetService,
        private calculationDataset: CalculationDatasetService,
        private registerEventDatasetService: RegisterEventDatasetService,
        private stratigraphyDatasetService: StratigraphyDatasetService,

        private treeDataset: TreeDatasetService,
        private stateService: StateService,
        private tabsDatasetService: TabsDatasetService,
    ) { }

    loadCurrCase(id: string) {
        this.dataset.currCaseId = id;
        this.caseService.get(this.dataset.currCaseId).then(res => {
            this._add(res.poço, res.caminho);
            this.syncChartsService.setsGlobalDepth(res.poço.dadosGerais.minPm, res.poço.dadosGerais.maxPm);

            this.stateService.setCurrState(this.dataset.currCaseId);

            this.$currCaseLoaded.next(this.dataset.currCaseId);
            res.poço.idsPoçosApoio.forEach(id => {
                this.caseService.get(id).then(el => {
                    this.add(el.poço, el.caminho);
                }).catch(error => {
                    this.notybarService.show('Erro ao carregar caso de apoio', 'danger');
                });
            });
        }).catch(error => {
            this.notybarService.show('Erro ao carregar caso', 'danger');
        });
    }

    add(newCase: Case, path: Caminho) {
        this._add(newCase, path);
        this.$caseAdded.next(newCase);
    }

    getAll() {
        return this.dataset.getCasesIds().map(el => this.dataset.getById(el));
    }

    getLimiteCaso(id: string) {
        if (this.dataset.hasById(id)) {
            const _case = this.dataset.getById(id);
            const trajetoria = this.trajectoryDataset.get(id);
            const limitesCaso: {
                pmMax: number, pmSup: number,
                cotaMax: number, cotaSup: number, cotaFinal: number,
                pvMax: number, pvSup: number,
            } = {
                pmMax: trajetoria.últimoPonto.pm.valor,
                pmSup: trajetoria.primeiroPonto.pm.valor,
                cotaMax: -Math.max(...trajetoria.pontos.map(el => el.pv.valor)),
                cotaFinal: -trajetoria.últimoPonto.pv.valor,
                cotaSup: -trajetoria.primeiroPonto.pv.valor,
                pvMax: trajetoria.últimoPonto.pv.valor,
                pvSup: trajetoria.primeiroPonto.pv.valor,
            };
            if (_case.dadosGerais.geometria.categoriaPoço === 'OffShore') {
                limitesCaso.cotaMax += _case.dadosGerais.geometria.mesaRotativa;
                limitesCaso.cotaFinal += _case.dadosGerais.geometria.mesaRotativa;

                limitesCaso.cotaSup = -_case.dadosGerais.geometria.offShore.laminaDagua;

                limitesCaso.pmSup += _case.dadosGerais.geometria.mesaRotativa;
                limitesCaso.pmSup += _case.dadosGerais.geometria.offShore.laminaDagua;
            } else {
                limitesCaso.cotaMax += _case.dadosGerais.geometria.onShore.elevação;
                limitesCaso.cotaMax += _case.dadosGerais.geometria.mesaRotativa;
                limitesCaso.cotaFinal += _case.dadosGerais.geometria.onShore.elevação;
                limitesCaso.cotaFinal += _case.dadosGerais.geometria.mesaRotativa;

                limitesCaso.cotaSup = _case.dadosGerais.geometria.onShore.elevação;
                limitesCaso.cotaSup -= _case.dadosGerais.geometria.onShore.alturaDeAntePoço;

                limitesCaso.pmSup = _case.dadosGerais.geometria.mesaRotativa;
                limitesCaso.pmSup += _case.dadosGerais.geometria.onShore.alturaDeAntePoço;
            }
            return limitesCaso;
        }
        return undefined;
    }

    remove(id: string) {
        this.dataset.remove(id);
        this.dataset.removeDatasetCase(id);
        this.$caseRemoved.next(id);
    }

    getCasePath(id: string): Caminho {
        if (this.dataset.hasById(id)) {
            return this.dataset.getDatasetCase(id).path;
        }
        return undefined;
    }

    private _add(newCase: Case, path: Caminho) {
        const datasetCase: DatasetCase = {
            caseId: newCase.id,

            trajectoryId: TrajectoryDatasetService.getTrajectoryId(newCase.id),
            lithologiesIds: [],
            stratigraphiesIds: [],
            calculationsIds: [],
            profilesIds: [],
            registerEventsIds: [],
            tabsIds: [],

            state: newCase.state,
            path: path,
        };

        this.updateRange(newCase);

        // Store the case
        this.dataset.add(newCase.id, newCase);
        this.dataset.addDatasetCase(datasetCase);

        // Get the tree, trajectory, lithologies, stratigraphies, profiles, register/events and calculations info
        const tree = newCase.state.tree;
        const tabs = newCase.state.tabs;
        const trajectory = newCase.trajetória;
        const lithologies = newCase.litologias;
        const stratigraphies = newCase.estratigrafia.Itens;
        const profiles = newCase.perfis;
        const calculations = newCase.cálculos;
        const registersEvents = newCase.registrosEventos;

        // Store the case's tree
        this.treeDataset.add(tree, datasetCase.caseId);

        // Store the case's trajectory
        this.trajectoryDataset.add(trajectory, datasetCase.caseId);

        // Store the case's lithologies
        for (const lithology of lithologies) {
            this.lithologyDataset.addJustDataset(lithology, datasetCase.caseId);
        }

        // Store the case's stratigraphies
        Object.keys(stratigraphies).forEach(key => {
            this.stratigraphyDatasetService.add(stratigraphies[key], key, datasetCase.caseId);
        });
        newCase['estratigrafias'] = this.stratigraphyDatasetService.getAllIds(datasetCase.caseId);

        // Store the case's profiles
        for (const profile of profiles) {
            this.profileDataset.addJustDataset(profile, datasetCase.caseId);
        }

        // Store the case's calculations
        for (const calculation of calculations) {
            this.calculationDataset.addJustDataset(calculation, datasetCase.caseId);
        }

        // Store the case's registers and events
        for (const registerEvent of registersEvents) {
            this.registerEventDatasetService.addJustDataset(registerEvent, datasetCase.caseId);
        }

        // Store the case's tabs
        for (const tab of tabs) {
            this.tabsDatasetService.addJustDataset(tab, datasetCase.caseId);
        }
    }

    private updateRange(_case: Case) {
        let firstPoint;
        let lastPoint;
        if (_case.trajetória && _case.trajetória.pontos.length > 0) {
            firstPoint = _case.trajetória.pontos[0];
            lastPoint = _case.trajetória.pontos[_case.trajetória.pontos.length - 1];
        } else {
            firstPoint = { pm: { valor: 0 }, pv: { valor: 0 } };
            lastPoint = { pm: { valor: 0 }, pv: { valor: 0 } };
        }
        _case.dadosGerais.minPm = firstPoint.pm.valor;
        _case.dadosGerais.maxPm = lastPoint.pm.valor;
        _case.dadosGerais.minPv = firstPoint.pv.valor;
        _case.dadosGerais.maxPv = lastPoint.pv.valor;
    }
}
