import { Component, OnInit, Output, EventEmitter, Input, ViewChild, AfterViewInit } from '@angular/core';
import { FileData, ImportData, ProfileToImport, LithologyToImport } from '@utils/interfaces';
import { DialogService } from '@services/dialog.service';
import { LITHO_TYPES } from '@utils/litho-types-list';
import { NotybarService } from '@services/notybar.service';
import { editButtonFormatter } from '@utils/edit-button-formatter';
import { iconFormatter, importType, translateImportType } from '@utils/icon-formatter';
import { getRowProperties } from '@utils/get-properties';
import { TableOptions } from '@utils/table-options-default';
import { DatasetService } from '@services/dataset/dataset.service';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';
import { Case } from 'app/repositories/models/case';
import { LithologyDatasetService } from '@services/dataset/lithology-dataset.service';

@Component({
  selector: 'app-file-info',
  templateUrl: './file-info.component.html',
  styleUrls: ['./file-info.component.scss'],
})
export class FileInfoComponent implements OnInit, AfterViewInit {
  /**
   * Os dados recebidos do arquivo selecionado.
   */
  @Input() data: { fileData: FileData; otherData?};
  @Output() someEvent = new EventEmitter<any>();

  buttonFormatter = editButtonFormatter;

  iconFormatter = iconFormatter;

  getRowProperties = getRowProperties;

  /**
   * Referência para tabela na tela de perfis.
   */
  @ViewChild('prof', { static: true }) profilesTableComponent;

  /**
   * Tabela de perfis.
   */
  profilesTable: Handsontable;

  /**
   * Referência para tabela na tela de litologias.
   */
  @ViewChild('litho', { static: true }) lithosTableComponent;

  /**
   * Tabela de litologias.
   */
  lithosTable: Handsontable;

  /**
   * Referência para tabela na tela de litologias.
   */
  @ViewChild('traj', { static: true }) trajectoriesTableComponent;

  trajectoriesTable: Handsontable;

  tableTypes = ['PM'];
  tableType = this.tableTypes[0];
  hasFields = false;
  showTable = false;
  isPocoWeb = false;

  correctionRotaryTable;

  /**
   * Dados do arquivo sest5 aberto.
   */
  fileData: FileData;

  /**
   * Dados do poço que existem no arquivo sest5 e
   * se eles estão selecionados para a importação.
   */
  wellData = [
    { name: 'Dados Gerais', exist: false, selected: false, value: 'DadosGerais' },
    { name: 'Trajetória', exist: false, selected: false, value: 'Trajetória' },
    { name: 'Sapatas', exist: false, selected: false, value: 'Sapatas' },
    { name: 'Objetivos', exist: false, selected: false, value: 'Objetivos' },
    { name: 'Estratigrafia', exist: false, selected: false, value: 'Estratigrafia' },
    { name: 'Registros', exist: false, selected: false, value: 'Registros' },
    { name: 'Eventos de Perfuração', exist: false, selected: false, value: 'Eventos' },
  ];

  /**
   * Se está marcado para selecionar todos os perfis.
   */
  selectedAllProfiles = false;

  /**
   * Dados da tabela de perfis.
   */
  tableProfilesData = [];

  currProfilesNames = [];

  /**
   * Titulos das colunas na tabela de perfis.
   */
  colProfilesHeaders = [
    `<input type='checkbox' ${this.selectedAllProfiles ? 'checked="checked"' : ''}>`,
    'Ação',
    'Nome',
    'Tipo',
    'Unidade',
    'Editar',
  ];

  /**
   * Tipos das colunas na tabela de perfis.
   */
  columnsProfilesTypes: any[] = [
    { type: 'checkbox' },
    { readOnly: true, renderer: 'html' },
    {
      readOnly: true,
      validator: function (value, callback) {
        const comment = this.instance.getPlugin('comments');
        if (this.instance.getDataAtCell(this.row, 6)) {
          comment.setCommentAtCell(this.row, 2, 'Já existe um perfil com esse nome');
          comment.updateCommentMeta(this.row, 2, { readOnly: true });
          callback(false);
        } else {
          comment.removeCommentAtCell(this.row, 2);
          callback(true);
        }
      },
    },
    { readOnly: true },
    { readOnly: true },
    { readOnly: true, renderer: 'html' },
  ];

  /**
   * Opções de configuração da tabela de perfis.
   */
  profilesTableOptions: any = TableOptions.createDefault({
    afterOnCellMouseDown: (event, coords) => {
      // Se clicou no checkbox do título, marca ou desmarca todos os perfis da tabela.
      if (coords.row < 0 && event.target.type === 'checkbox') {
        this.selectedAllProfiles = !event.target.checked;

        this.colProfilesHeaders[0] = `<input type='checkbox' ${this.selectedAllProfiles ? 'checked="checked"' : ''}>`;

        for (let i = 0; i < this.profilesTable.countRows(); i += 1) {
          this.profilesTable.setDataAtCell(i, 0, this.selectedAllProfiles as any);
        }
      } else if (coords.col === 5 && event.target.attributes.id && event.target.classList.contains('button')) {
        const obj: any = this.getRowProperties(this.tableProfilesData, coords.row, 'profile');
        const tableNames = this.profilesTable.getDataAtCol(2);
        tableNames.splice(coords.row, 1);

        obj.usedNamesInTable = tableNames;

        obj.unit = this.tableProfilesData[coords.row][4];

        this.dialog.openDialogColumnProperties(obj);
      }
    },
    disableVisualSelection: true,
    manualColumnResize: [30, 35, , 75, 60, 95],
  });

  /**
   * Se está marcado para selecionar todos as litologias.
   */
  selectedAllLithos = false;

  /**
   * Dados da tabela de litologias.
   */
  tableLithosData = [];

  currLithosNames = [];

  tableTrajectoriesData = [];

  /**
   * Titulos das colunas na tabela de litologias.
   */
  colLithosHeaders = [
    `<input type='checkbox' ${this.selectedAllLithos ? 'checked="checked"' : ''}>`,
    'Ação',
    'Nome',
    'Tipo',
    'Editar',
  ];

  colTrajectoriesHeaders = ['', 'Nome', ''];

  /**
   * Tipos das colunas na tabela de litologias.
   */
  columnsLithosTypes: any[] = [
    { type: 'checkbox' },
    { readOnly: true, renderer: 'html' },
    { readOnly: true },
    { readOnly: true },
    { readOnly: true, renderer: 'html' },
  ];

  columnsTrajectoriesTypes: any[] = [
    { type: 'checkbox' },
    { readOnly: true, renderer: 'html' },
    { readOnly: true, renderer: 'html' },
  ];
  /**
   * Opções de configuração da tabela de litologias.
   */
  lithosTableOptions: any = TableOptions.createDefault({
    afterOnCellMouseDown: (event, coords, TD) => {
      // Se clicou no checkbox do título, marca ou desmarca todos os perfis da tabela.
      if (coords.row < 0 && event.target.type === 'checkbox') {
        this.selectedAllLithos = !event.target.checked;

        this.colLithosHeaders[0] = `<input type='checkbox' ${this.selectedAllLithos ? 'checked="checked"' : ''}>`;

        for (let i = 0; i < this.lithosTable.countRows(); i += 1) {
          this.lithosTable.setDataAtCell(i, 0, this.selectedAllLithos as any);
        }
      } else if (coords.col === 4 && event.target.attributes.id && event.target.classList.contains('button')) {
        const obj: any = this.getRowProperties(this.tableLithosData, coords.row, 'litho');
        const tableNames = this.lithosTable.getDataAtCol(2);
        tableNames.splice(coords.row, 1);

        obj.usedNamesInTable = tableNames;

        this.dialog.openDialogColumnProperties(obj);
      }
    },
    disableVisualSelection: true,
    manualColumnResize: [30, 35, , 120, 120],
  });

  trajectoryTableOptions: any = TableOptions.createDefault({
    afterChange: (changes) => {
      if (!changes || !changes[0]) {
        return;
      }

      const change = changes[0];
      const [row, column, oldValue, newValue] = change;

      if (change && column === 0 && newValue === true) {
        for (let i = 0; i < this.trajectoriesTable.countRows(); i += 1) {
          if (i !== row) {
            this.trajectoriesTable.setDataAtCell(i, 0, false as any);
          }
        }
      }
    },
    disableVisualSelection: true,
    manualColumnResize: [30, , 95],
  });

  currCase: Case;

  constructor(
    public dialog: DialogService,
    private notybarService: NotybarService,

    private dataset: DatasetService,

    private profileDataset: ProfileDatasetService,
    private lithologyDataset: LithologyDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    this.someEvent.emit({ cangoNext: true });
    // Pega os dados recebidos e salva nas respectivas variáveis.
    this.fileData = this.data.fileData;
    if (this.data.otherData !== undefined) {
      this.fileData = this.data.otherData;
    }

    if (this.fileData.extras && this.fileData.extras.poçoWeb.poçoWeb) {
      this.isPocoWeb = true;
    }
    // A seleção de trajetória para arquivos SEST TR1 é feita na tabela
    if (this.fileData.filePath && this.fileData.filePath.endsWith('.xsrt')) {
      this.wellData.forEach((element, i) => {
        if (element.name === 'Trajetória') {
          // Remove o checkbox de Trajetória do menu à esquerda
          this.wellData.splice(i, 1);
        }
      });
    }

    if (!this.fileData.fixedDeepType) {
      this.tableTypes.push('COTA');
    }
    if (this.fileData.hasFields) {
      this.hasFields = true;
      this.wellData.forEach((el) => {
        el.selected = el.exist = this.fileData.fields[el.name];
      });
    }
    this.currProfilesNames = this.profileDataset.getAll(this.currCase.id).map(el => el.name);

    let lithoTypes = LITHO_TYPES[this.currCase.tipoPoço];
    this.lithologyDataset.getAll(this.dataset.currCaseId).forEach((el) => {
      const nomeLito: string = el.classificação.nome;
      this.currLithosNames.push(nomeLito);
      if (el.pontos.length > 0) {
        lithoTypes = lithoTypes.filter((type) => type !== el.classificação.nome);
      }
    });

    if (this.fileData.profiles) {
      this.fileData.profiles.forEach((el) => {
        this.tableProfilesData.push([
          false,
          this.iconFormatter('new'),
          el.name,
          el.type,
          el.unit,
          this.buttonFormatter(el.name),
          this.currProfilesNames.findIndex((name) => name === el.name) > -1,
        ]);
      });
    }

    if (this.fileData.lithologies) {
      this.fileData.lithologies.forEach((el) => {
        this.tableLithosData.push([
          false,
          lithoTypes.length > 0 ? this.iconFormatter('new') : this.iconFormatter('append'),
          el.name,
          LITHO_TYPES[this.currCase.tipoPoço][0],
          this.buttonFormatter(el.name),
        ]);
      });
    }

    if (this.fileData.trajectories) {
      this.fileData.trajectories.forEach((el) => {
        this.tableTrajectoriesData.push([false, el.name]);
      });
    }
    setTimeout(() => {
      this.showTable = true;
      window.dispatchEvent(new Event('resize'));
    }, 1);
  }

  ngAfterViewInit() {
    this.profilesTable = this.profilesTableComponent.getHandsontableInstance();
    this.lithosTable = this.lithosTableComponent.getHandsontableInstance();
    this.trajectoriesTable = this.trajectoriesTableComponent.getHandsontableInstance();

    this.profilesTable.validateCells(() => { });
    this.lithosTable.validateCells(() => { });
  }

  setProperties(data: {
    table: string;
    importType: importType;
    position: number;
    name: string;
    type?: string;
    unit?: string;
    top: number;
    bottom: number;
  }) {
    let table: Handsontable;
    if (data.table === 'litho') {
      table = this.lithosTable;
    } else if (data.table === 'profile') {
      table = this.profilesTable;
    }

    table.setDataAtCell(data.position, 1, this.iconFormatter(data.importType, data.top, data.bottom));
    if (data.name !== undefined && data.table !== 'litho') {
      table.setDataAtCell(data.position, 2, data.name);
    }
    if (data.type !== undefined) {
      table.setDataAtCell(data.position, 3, data.type);
    }
    if (data.unit !== undefined) {
      table.setDataAtCell(data.position, 4, data.unit);
    }
    if (data.type !== undefined || data.unit !== undefined) {
      table.setDataAtCell(data.position, 6, false as any);
    } else {
      table.setDataAtCell(data.position, 5, false as any);
    }
  }

  infoTriggerEnter(trigger) {
    setTimeout(() => {
      trigger.openMenu();
    }, 500);
  }

  exportData(): ImportData {
    const selectedData = [];
    const lithologyList = [];
    const profileList = [];
    let selectedTrajectory = '';

    let hasInvalid = false;

    this.wellData.forEach((el) => {
      if (el.selected) {
        selectedData.push(el.value);
      }
    });

    this.tableLithosData.forEach((el, index) => {
      if (el[0]) {
        const obj = this.getRowProperties(this.tableLithosData, index, '');

        const lithology: LithologyToImport = {
          ação: translateImportType[obj.importType],
          nome: this.fileData.lithologies[index].name,
          tipo: this.tableLithosData[index][3],
        };
        if (obj.top !== undefined && obj.bottom !== undefined) {
          lithology.valorTopo = obj.top;
          lithology.valorBase = obj.bottom;
        }
        if (lithology.nome !== obj.name) {
          lithology.novoNome = obj.name;
        }
        lithologyList.push(lithology);
      }
    });

    this.tableProfilesData.forEach((el, index) => {
      if (el[0]) {
        if (this.tableProfilesData[index][6]) {
          hasInvalid = true;
          return;
        }
        const obj = this.getRowProperties(this.tableProfilesData, index, '');
        const profile: ProfileToImport = {
          ação: translateImportType[obj.importType],
          nome: this.fileData.profiles[index].name,
          tipo: this.tableProfilesData[index][3],
          unidade: this.tableProfilesData[index][4],
        };
        if (obj.top !== undefined && obj.bottom !== undefined) {
          profile.valorTopo = obj.top;
          profile.valorBase = obj.bottom;
        }
        if (profile.nome !== obj.name) {
          profile.novoNome = obj.name;
        }
        profileList.push(profile);
      }
    });
    if (hasInvalid) {
      this.notybarService.show('Edite os nomes inválidos selecionados para poder importar.', 'danger');
      return;
    }

    for (let i = 0; i < this.tableTrajectoriesData.length; i++) {
      const row = this.tableTrajectoriesData[i];
      if (row[0]) {
        selectedTrajectory = row[1];
        break;
      }
    }

    const importData: ImportData = {
      caminhoDoArquivo: this.fileData.filePath,
      poçoId: this.currCase.id,
      tipoProfundidade: this.tableType as 'PM' | 'Cota',
      dadosSelecionados: selectedData,
      listaLitologias: lithologyList,
      listaPerfis: profileList,
    };

    const extras = {};
    if (selectedTrajectory !== '') {
      extras['TrajetóriaSelecionada'] = selectedTrajectory;
    }

    if (this.fileData.extras.selectedWellName) {
      extras['PoçoSelecionado'] = this.fileData.extras.selectedWellName;
    }

    if (this.isPocoWeb) {
      extras['poçoWeb'] = { poçoWeb: true };
    }

    if (Object.keys(extras).length > 0) {
      importData['extras'] = extras;
    }

    if (this.correctionRotaryTable !== undefined) {
      importData.correçãoMesaRotativa = this.correctionRotaryTable;
    }

    return importData;
  }
}
