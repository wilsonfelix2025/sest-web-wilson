import { Component, OnInit, Input, AfterViewInit, ViewChild, OnChanges, Output, EventEmitter } from '@angular/core';
import { DatasetService } from '@services/dataset/dataset.service';
import { Case } from 'app/repositories/models/case';
import { HotTableComponent } from 'ng2-handsontable';
import { TableOptions } from '@utils/table-options-default';
import { MontagemPerfis, Trecho } from 'app/repositories/models/montagem-perfis';
import { NotybarService } from '@services/notybar.service';
import { Perfil } from 'app/repositories/models/perfil';
import { ProfileDatasetService } from '@services/dataset/profile-dataset.service';

@Component({
  selector: 'sest-selecao-perfis',
  templateUrl: './selecao-perfis.component.html',
  styleUrls: ['./selecao-perfis.component.scss']
})
export class SelecaoPerfisComponent implements OnInit, AfterViewInit, OnChanges {

  @Input() trechos;
  @Output() semPerfil = new EventEmitter();

  /**
   * O caso de estudo aberto atualmente.
   */
  currCase: Case;

  tiposDefault = [];

  tiposSelected = this.tiposDefault;

  /**
   * Referência para tabela na tela.
   */
  @ViewChild(HotTableComponent, { static: true }) hotTableComponent;

  /**
   * Tabela.
   */
  table: Handsontable;
  comment;

  lineSelected = [];

  tendenciaPerfis: boolean = false;

  /**
   * Se está marcado para selecionar todos os perfis.
   */
  selectedAll = false;

  /**
   * Titulos das colunas na tabela de selecao de perfis.
   */
  colHeaders = [
    'Caso',
  ];

  /**
   * Tipos das colunas na tabela de selecao de perfis.
   */
  columnsTypes: any[] = [
    { data: 'caso', readOnly: true },
    { data: 'RHOB', type: 'dropdown' },
    { data: 'DTC', type: 'dropdown' },
    { data: 'DTS', type: 'dropdown' },
    { data: 'GRAY', type: 'dropdown' },
    { data: 'RESIST', type: 'dropdown' },
    { data: 'PORO', type: 'dropdown' },
  ];

  casos: any = {};


  /**
   * Dados da tabela de selecao de perfis.
   */
  tableData = [];

  addInput: string = 'e';

  nomesEmUso: string[] = [];

  /**
   * Opções de configuração da tabela de selecao de perfis.
   */
  tableOptions: any = TableOptions.createDefault({
    manualColumnResize: [210, ,],
    height: 300,
    filters: false,
    disableVisualSelection: true,
    allowInvalid: false,
    beforeOnCellMouseDown: (event, coords, TD) => {
      if (coords.row === -1 && event.target.nodeName === 'INPUT') {
        if (event.target.type === 'text') {
          // Pausa o comportamento de seleção da coluna quando o usuário clicar no input do header
          event.stopImmediatePropagation();
          event.target.onchange = changes => {
            this.tiposSelected[coords.col - 1].title = changes.srcElement.value;
            this.colHeaders[coords.col] = this.getColHeaderText(coords.col - 1);
            // VER FORMA DE MARCAR NOMES EM USO
            if (this.nomesEmUso.includes(this.tiposSelected[coords.col - 1].title)) {
            } else {
            }
          };
        } else {
          this.tiposSelected[coords.col - 1].selected = !event.target.checked;
          this.colHeaders[coords.col] = this.getColHeaderText(coords.col - 1);
        }
      } else {
        // this.comment.setCommentAtCell(coords.row, coords.col, 'TESTEE');
        this.atualizarPossibilidades();
      }
    },
  });

  getColHeaderText(col) {
    if (this.tiposSelected[col]) {
      return `<input type='checkbox' ${this.tiposSelected[col].selected ? 'checked' : ''}> <input type="text" style="max-width: 60px !important;" value="${this.tiposSelected[col].title}">`;
    }
    return `<input type='checkbox'> <input type="text" style="max-width: 60px !important;" value="">`;
  }

  constructor(
    public dataset: DatasetService,
    public notybarService: NotybarService,
    private profileDataset: ProfileDatasetService,
  ) { }

  ngOnInit() {
    this.currCase = this.dataset.getById(this.dataset.currCaseId);

    this.reiniciarTela();
    this.tiposSelected = this.tiposDefault;
  }

  ngAfterViewInit() {
    this.table = this.hotTableComponent.getHandsontableInstance();
  }

  ngOnChanges(changes) {
    if (changes.trechos.currentValue !== undefined) {
      this.reiniciarTela();

      this.pegarPerfisDosCasos();

      this.adicionarLinhas();

      this.atualizarPossibilidades();

      this.atualizarHeaderDosTiposSelecionados();
    }
  }

  reiniciarTela() {
    this.tiposDefault = [
      { mnemonico: 'RHOB', title: 'RHOB', selected: true, interface: 'Rhob' },
      { mnemonico: 'DTC', title: 'DTC', selected: true, interface: 'DTC' },
      { mnemonico: 'DTS', title: 'DTS', selected: true, interface: 'DTS' },
      { mnemonico: 'GRAY', title: 'GRAY', selected: true, interface: 'GRay' },
      { mnemonico: 'RESIST', title: 'RESIST', selected: true, interface: 'Resist' },
      { mnemonico: 'PORO', title: 'NPhi', selected: true, interface: 'NPhi' },
    ];

    this.colHeaders = [
      'Caso',
    ];
    // console.log('selecao', this.trechos);
    this.nomesEmUso = this.profileDataset.getAll(this.currCase.id).map(p => p.nome);
    this.tiposDefault.forEach((el, i) => {
      this.colHeaders.push(this.getColHeaderText(i));
      // VER FORMA DE MARCAR NOMES EM USO
      if (this.nomesEmUso.includes(el.title)) {
        // console.log(el, i);
      } else {
      }
    });
  }

  pegarPerfisDosCasos() {
    let temPerfil = false;
    this.trechos.forEach(trecho => {
      const caso: Case = this.dataset.getById(trecho.idCaso);
      const dados: any = { caso: caso.nome, lista: {} };
      this.tiposDefault.forEach(tipo => {
        let perfis: Perfil[] = caso.perfis.filter(perfil => perfil.mnemonico === tipo.mnemonico);
        perfis = perfis.filter(p => p.ultimoPonto.pv.valor >= -trecho.trechoCorrelacao.topo
          && p.primeiroPonto.pv.valor <= -trecho.trechoCorrelacao.base);

        if (perfis.length > 0) {
          temPerfil = true;
        }
        dados.lista[tipo.mnemonico] = perfis.map(el => el.nome);
        dados[tipo.mnemonico] = dados.lista[tipo.mnemonico][0];
      });
      this.casos[caso.id] = dados;
    });
    if (!temPerfil) {
      this.notybarService.show('Não foram encontrados perfis passíveis de montagem nos casos de apoio selecionados.', 'warning');
      this.semPerfil.emit();
    }
  }

  adicionarLinhas() {
    this.tableData = [];
    Object.keys(this.casos).forEach(id => {
      this.tableData.push(this.casos[id]);
    });
    this.table.render();
  }

  atualizarPossibilidades() {
    this.tableData.forEach((el, linha) => {
      this.tiposDefault.forEach(tipo => {
        this.table.setCellMetaObject(linha, this.table.propToCol(tipo.mnemonico), { source: el.lista[tipo.mnemonico] });
      });
    });
    this.table.render();
  }

  atualizarHeaderDosTiposSelecionados() {
    this.tiposDefault.forEach(tipo => {
      if (this.tableData.findIndex(el => el[tipo.mnemonico] !== undefined) < 0) {
        tipo.selected = false;
        const i = this.columnsTypes.findIndex(el => tipo.mnemonico === el.data);
        this.colHeaders.splice(i, 1);
        this.columnsTypes.splice(i, 1);
      }
    });
    this.tiposSelected = this.tiposDefault.filter(el => el.selected);
  }

  pegarMontagemSelecionada() {
    const montagem: MontagemPerfis = {
      listaTrechos: [],
      idPoço: this.currCase.id,
      removerTendência: this.tendenciaPerfis,
    };

    let valido = true;

    let temSelecionado = false;
    this.tiposSelected.forEach(tipo => {
      if (tipo.selected) {
        if (this.nomesEmUso.includes(tipo.title)) {
          valido = false;
          this.notybarService.show(`Nome '${tipo.title}' já está uso`, 'warning');
          return;
        } else {
          montagem[`nome${tipo.interface}`] = tipo.title;
          temSelecionado = true;
        }
      }
    });
    if (valido) {
      if (!temSelecionado) {
        valido = false;
        this.notybarService.show('Selecione ao menos um tipo.', 'warning');
      } else {
        this.trechos.forEach((linha, i) => {
          const caso: Case = this.dataset.getById(linha.idCaso);
          const trecho: Trecho = {
            idPoçoApoio: linha.idCaso,
            pvTopoApoio: linha.trechoCorrelacao.topo,
            pvBaseApoio: linha.trechoCorrelacao.base,
            pvTopo: linha.trechoTrabalho.topo,
            pvBase: linha.trechoTrabalho.base,
          };
          const data = this.tableData.find(el => el.caso === caso.nome);
          this.tiposSelected.forEach((tipo, f) => {
            if (tipo.selected) {
              if (data[tipo.mnemonico]) {
                const perfilSelecionado = caso.perfis.find(el => el.nome === data[tipo.mnemonico]);
                if (perfilSelecionado !== undefined) {
                  trecho[`id${tipo.interface}Apoio`] = perfilSelecionado.id;
                } else {
                  valido = false;
                  this.notybarService.show('Selecione apenas perfis válidos.', 'warning');
                  return;
                }
              }
            }
          });
          if (!valido) { return; }

          // verificar se tem perfil selecionado valido
          montagem.listaTrechos.push(trecho);
        });

      }
    }
    return { montagem: montagem, valido: valido };
  }
}
