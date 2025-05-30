import { FlatTreeControl } from '@angular/cdk/tree';
import { Component, OnInit, ViewChild, ChangeDetectorRef, Input } from '@angular/core';
import {
  MatTreeFlatDataSource,
  MatTreeFlattener,
} from '@angular/material/tree';
import { MatMenuTrigger } from '@angular/material';
import { DialogService } from '@services/dialog.service';
import { NewDialogDeleteComponent } from '../new-dialog-delete/new-dialog-delete.component';
import { DatasetService } from '@services/dataset/dataset.service';
import { TrendService } from 'app/repositories/trend.service';
import { WindowEditTrendComponent } from '../window-edit-trend/window-edit-trend.component';
import {
  Calculo,
  isExpD,
  isGradiente,
  isPerfis,
  isPressaoPoros,
  isPropriedadesMecanicas,
  isSobrecarga,
  isTensoesInSitu,
} from 'app/repositories/models/calculo';
import { isFiltro } from 'app/repositories/models/filtro';
import { WindowFilterComponent } from '../window-filter/window-filter.component';
import { CalculoService } from 'app/repositories/calculo.service';
import { WindowEditDeepDataComponent } from '../window-edit-deep-data/window-edit-deep-data.component';
import { OverloadCalculationComponent } from '../window-calculations/overload-calculation/overload-calculation.component';
import { ProfileCalculationsComponent } from '../window-calculations/profile-calculations/profile-calculations.component';
import { ProfileService } from 'app/repositories/profile.service';
import { PorePressureCalculationComponent } from '../window-calculations/pore-pressure-calculation/pore-pressure-calculation.component';
import { MechanicalCalculationsComponent } from '../window-calculations/mechanical-calculations/mechanical-calculations.component';
import { TensionsInsituCalculationComponent } from '../window-calculations/tensions-insitu-calculation/tensions-insitu-calculation.component';
import { GradientCalculationComponent } from '../window-calculations/gradient-calculation/gradient-calculation.component';
import { NodeTypes } from 'app/repositories/models/state';
import { TreeDatasetService } from '@services/dataset/state/tree-dataset.service';
import { TrajectoryDatasetService } from '@services/dataset/trajectory-dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { LithologyDatasetService } from '@services/dataset/lithology-dataset.service';
import { CalculationDatasetService } from '@services/dataset/calculation-dataset.service';
import { NotybarService } from '@services/notybar.service';
import { ExponentDComponent } from '../window-perforations/exponent-d/exponent-d.component';
import { RecordsComponent } from '../window-perforations/records/records.component';
import { EventsComponent } from '../window-perforations/events/events.component';
import { ConversaoPerfisService } from 'app/repositories/conversao-perfis.service';
import { TipoConversao } from 'app/repositories/models/conversao';
import { RegisterEventDatasetService } from '@services/dataset/register-event-dataset.service';
import { RegistrosEventosService } from 'app/repositories/registros-eventos.service';
import { Evento, Registro } from 'app/repositories/models/registro-evento';
import { NoConvergenceComponent } from '../window-calculations/gradient-calculation/no-convergence/no-convergence.component';
import { CaseService } from 'app/repositories/case.service';

@Component({
  selector: 'sest-node-tree',
  templateUrl: './node-tree.component.html',
  styleUrls: ['./node-tree.component.scss'],
})
export class NodeTreeComponent implements OnInit {
  @Input() caseId: string = '0';

  treeControl: FlatTreeControl<FileNode>;
  treeFlattener: MatTreeFlattener<any, FileNode>;
  dataSource: MatTreeFlatDataSource<any, FileNode>;
  droppedData: Object = '';

  content: any;
  @ViewChild(MatMenuTrigger, { static: false })
  contextMenu: MatMenuTrigger;
  contextMenuPosition = { x: '0px', y: '0px' };

  constructor(
    public dialog: DialogService,
    private changeDetector: ChangeDetectorRef,
    private notybarService: NotybarService,

    private trendService: TrendService,
    private caseService: CaseService,
    public calculoService: CalculoService,
    private profileService: ProfileService,
    private conversaoPerfisService: ConversaoPerfisService,
    private registrosEventosService: RegistrosEventosService,

    private dataset: DatasetService,
    private treeDataset: TreeDatasetService,
    private registerEventDataset: RegisterEventDatasetService,
    private trajectoryDataset: TrajectoryDatasetService,
    private profileDataset: ProfileDatasetService,
    private lithologyDataset: LithologyDatasetService,
    private calculationDataset: CalculationDatasetService,
  ) {
    this.content = this.treeDataset.get(this.caseId);

    this.treeDataset.$treeChanged.subscribe((tree) => {
      this.content = this.treeDataset.get(this.caseId);
      this.ordenaFilhos(this.content);
      this.dataSource.data = this.content;
      // console.log('treeChanged', this.dataSource.data);
      this.changeDetector.detectChanges();
    });

    const transformer = (node: any, level: number) => {
      if (node.tipo === NodeTypes.Trajetória) {
        node.data = this.trajectoryDataset.get(this.caseId);
        node.data.nome = 'Trajetória';
      } else if (node.tipo !== NodeTypes.Folder) {
        if (this.dataset.hasById(node.id)) {
          node.data = this.dataset.getById(node.id);
        } else {
          node.data = undefined;
          console.warn("Node without data", node, level);
        }
      } else if (node.tipo === NodeTypes.Folder && node.id !== undefined) {
        node.data.forEach((filho) => {
          filho.perfilCalculado = true;
        });
      }

      node.level = level;
      node.expandable = node.tipo === NodeTypes.Folder;
      return node;
    };
    this.treeControl = new FlatTreeControl<any>(
      (node) => node.level,
      (node) => node.expandable
    );
    this.treeFlattener = new MatTreeFlattener(
      transformer,
      (node) => node.level,
      (node) => node.expandable,
      (node) => node.data
    );
    this.dataSource = new MatTreeFlatDataSource(
      this.treeControl,
      this.treeFlattener
    );
  }

  ngOnInit() {
    this.content = this.treeDataset.get(this.caseId);
    this.ordenaFilhos(this.content);
    this.dataSource.data = this.content;
    // console.log('teste', this.content);
  }

  ordenaFilhos(no) {
    no.forEach((element) => {
      if (element.tipo === NodeTypes.Folder) {
        this.ordenaNo(element.data);
      }
    });
  }

  ordenaNo(no) {
    no.sort((a, b) => (a.name > b.name ? 1 : b.name > a.name ? -1 : 0));
    no.forEach((element) => {
      if (element.tipo === NodeTypes.Folder) {
        this.ordenaNo(element.data);
      }
    });
  }

  hasChild = (_: number, node: FileNode) => node.expandable;

  onContextMenu(event: MouseEvent, node: FileNode) {
    event.preventDefault();
    this.contextMenuPosition.x = event.clientX + 'px';
    this.contextMenuPosition.y = event.clientY + 'px';
    this.contextMenu.menuData = { node: node };
    // console.log(node)
    // console.log(node.data)
    this.contextMenu.menu.focusFirstItem('mouse');
    this.contextMenu.openMenu();
  }

  delete(item) {
    let callback = () => { };
    let postCallback = (resp) => { };
    const wellId = this.dataset.currCaseId;
    if (item.tipo === NodeTypes.RegistroEvento) {
      callback = () => {
        return this.registrosEventosService.reiniciarRegistroEvento(item.id);
      };

      postCallback = (res) => {
        const re: (Registro | Evento) = item.data;
        re.trechos = [];
        this.registerEventDataset.update(re);
      };

    } else if (item.trend !== undefined) {
      callback = () => {
        return this.trendService.remove(item.trend.data.id);
      };

      postCallback = (resp) => {
        this.profileDataset.removeTrend(item.trend.data.id);
      };
    } else if (item.tipo === NodeTypes.Litologia) {
      const litho = this.lithologyDataset.getAll(wellId).find((lito) => lito.classificação.nome === item.name);
      const lithoID = litho.id;

      callback = () => {
        return this.caseService.removeLithology(wellId, lithoID);
      };

      postCallback = (resp) => {
        this.lithologyDataset.remove(lithoID, wellId);
      };
    } else if (
      item.data.idCálculo !== undefined ||
      (item.tipo === NodeTypes.Folder && item.id)
    ) {
      let idCálculo;
      if (item.tipo === NodeTypes.Folder) {
        idCálculo = item.id;
      } else {
        idCálculo = item.data.idCálculo;
      }
      callback = () => {
        return this.calculoService.remover(wellId, idCálculo);
      };

      postCallback = (resp) => {
        this.calculationDataset.remove(idCálculo, wellId);
      };
    } else {
      callback = () => {
        return this.profileService.remove(item.data.id, wellId);
      };

      postCallback = (resp) => {
        this.profileDataset.remove(item.data.id, wellId);
      };
    }

    this.dialog.openDialog(NewDialogDeleteComponent, {
      submitCallback: callback,
      postSubmitCallback: postCallback,
    });
  }

  openWindowEdition(item) {
    if (item.trend !== undefined) {
      this.openDialogEditTrend(item);
    } else if (item.tipo === NodeTypes.Trajetória) {
      this.openWindowEditTrajectory(item);
    } else if (item.tipo === NodeTypes.Litologia) {
      this.openWindowEditLitology(item);
    } else if (item.tipo === NodeTypes.Perfil) {
      this.openWindowEditProfile(item);
    } else if (item.tipo === NodeTypes.Folder && item.id) {
      this.editarCalculo(item.id);
    } else if (item.tipo === NodeTypes.RegistroEvento) {
      if (item.data.tipo === 'Registro') {
        this.openWindowRegistry();
      } else {
        this.openWindowEvents();
      }
    }
  }

  addTrend(item) {
    // console.log("TREND", item);
    this.trendService.create(item.id).then((res) => {
      // console.log("created", res.trend);
      item.expandable = true;

      item.data.trend = res.trend;
    });
  }
  converterManometrica(item) {
    this.converterPerfil(item.id, TipoConversao.PressãoManométrica);
  }
  converterAbsoluta(item) {
    this.converterPerfil(item.id, TipoConversao.PressãoAbsoluta);
  }
  converterGradiente(item) {
    this.converterPerfil(item.id, TipoConversao.Gradiente);
  }

  converterPerfil(id: string, tipo: TipoConversao) {
    this.conversaoPerfisService.converter(id, tipo).then((res) => {
      this.profileDataset.add(res.perfil, this.dataset.currCaseId);
    });
  }

  editarCalculo(idCálculo) {
    if (this.dataset.hasById(idCálculo)) {
      this.openWindowEditCalculation(this.dataset.getById(idCálculo));
    } else {
      console.error('Erro ao encontrar cálculo com id', idCálculo);
    }
  }

  isGradiente(item) {
    if (item.tipo === NodeTypes.Folder && item.id !== undefined && this.dataset.hasById(item.id) && isGradiente(this.dataset.getById(item.id).grupoCálculo)) {
      return true;
    }
    return false;
  }

  recalcularGradiente(idCálculo) {
    this.calculoService.recalcularCalculoGradiente(this.dataset.currCaseId, idCálculo).then(res => {
      this.calculationDataset.update(res.cálculo, idCálculo, res.perfisAlterados);
      if (res.cálculo['informações'] && res.cálculo['informações'].length > 0) {
        this.dialog.openPageDialog(
          NoConvergenceComponent,
          { minHeight: 0, minWidth: 900 },
          { info: res.cálculo['informações'] }
        );
      }
    })
  }

  openWindowEditCalculation(calculo: Calculo) {
    if (this.caseId !== this.dataset.currCaseId) {
      this.notybarService.show('Não é possível editar Cálculo de caso de apoio.', 'warning');
      return;
    }
    if (isFiltro(calculo.grupoCálculo)) {
      this.dialog.openPageDialog(
        WindowFilterComponent,
        { minHeight: 450, minWidth: 450 },
        { calculo: calculo }
      );
    } else if (isSobrecarga(calculo.grupoCálculo)) {
      this.dialog.openPageDialog(
        OverloadCalculationComponent,
        { minHeight: 300, minWidth: 450 },
        { calculo: calculo }
      );
    } else if (isPerfis(calculo.grupoCálculo)) {
      this.dialog.openPageDialog(
        ProfileCalculationsComponent,
        { minHeight: 520, minWidth: 1024 },
        { calculo: calculo }
      );
    } else if (isPressaoPoros(calculo.grupoCálculo)) {
      this.dialog.openPageDialog(
        PorePressureCalculationComponent,
        { minHeight: 400, minWidth: 450 },
        { calculo: calculo }
      );
    } else if (isPropriedadesMecanicas(calculo.grupoCálculo)) {
      this.dialog.openPageDialog(
        MechanicalCalculationsComponent,
        { minHeight: 450, minWidth: 1024 },
        { calculo: calculo }
      );
    } else if (isTensoesInSitu(calculo.grupoCálculo)) {
      this.dialog.openPageDialog(
        TensionsInsituCalculationComponent,
        { minHeight: 100, minWidth: 450 },
        { calculo: calculo }
      );
    } else if (isGradiente(calculo.grupoCálculo)) {
      this.dialog.openPageDialog(
        GradientCalculationComponent,
        { minHeight: 400, minWidth: 450 },
        { calculo: calculo }
      );
    } else if (isExpD(calculo.grupoCálculo)) {
      this.dialog.openPageDialog(
        ExponentDComponent,
        { minHeight: 300, minWidth: 450 },
        { calculo: calculo }
      );
    } else {
      console.error(`Grupo de Cálculo não mapeado: ${calculo.grupoCálculo}`);
    }
  }

  openWindowEditProfile(item) {
    this.dialog.openPageDialog(
      WindowEditDeepDataComponent,
      { minHeight: 450, minWidth: 400 },
      { tipo: 'Perfil', id: item.data.id, caseId: this.caseId }
    );
  }

  openWindowEditLitology(item) {
    this.dialog.openPageDialog(
      WindowEditDeepDataComponent,
      { minHeight: 450, minWidth: 400 },
      { tipo: 'Litologia', id: item.data.id, caseId: this.caseId }
    );
  }

  openWindowEditTrajectory(item) {
    this.dialog.openPageDialog(
      WindowEditDeepDataComponent,
      { minHeight: 450, minWidth: 400 },
      { tipo: 'Trajetória', caseId: this.caseId }
    );
  }

  openWindowRegistry() {
    this.dialog.openPageDialog(
      RecordsComponent,
      { minHeight: 300, minWidth: 1024 },
    );
  }

  openWindowEvents() {
    this.dialog.openPageDialog(
      EventsComponent,
      { minHeight: 300, minWidth: 1024 },
    );
  }

  openDialogEditTrend(item) {
    this.dialog.openPageDialog(
      WindowEditTrendComponent,
      { minHeight: 450, minWidth: 450 },
      { idPerfil: item.trend.data.id, caseId: this.caseId }
    );
  }

  openNode(node) {
    node.isExpanded = !node.isExpanded;
  }
}

class FileNode {
  expandable: boolean;
  name: string;
  level: number;
  data?: any;
}
