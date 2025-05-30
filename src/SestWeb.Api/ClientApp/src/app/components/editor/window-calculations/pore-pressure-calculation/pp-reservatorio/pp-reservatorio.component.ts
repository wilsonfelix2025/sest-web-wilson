import {
  AfterViewInit,
  Component,
  Inject,
  OnInit,
  ViewChild,
} from "@angular/core";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material";
import { DialogBaseComponent } from "@components/common/dialog-base/dialog-base.component";
import { DialogService } from "@services/dialog.service";
import { TableOptions } from "@utils/table-options-default";
import * as Handsontable from "ng2-handsontable/node_modules/handsontable";
import { NotybarService } from "@services/notybar.service";
import { Reservatorio } from "app/repositories/models/calculo";
import { NumberUtils } from "@utils/number";

@Component({
  selector: "app-pp-reservatorio",
  templateUrl: "./pp-reservatorio.component.html",
  styleUrls: ["./pp-reservatorio.component.scss"],
})
export class PpReservatorioComponent implements OnInit, AfterViewInit {
  /**
   * Referência para tabela na tela de litologias.
   */
  @ViewChild("table", { static: true }) tableComponent;

  /**
   * Valor inicial corresponde ao header dinâmico
   */
  dataset: any[] = [
    [
      "Reservatório",
      "Dados de Referência",
      "",
      "",
      "",
      "",
      "",
      "",
      "",
      "Reservatório no poço de interesse (cota - m)",
      "",
    ],
    [
      "",
      "Poço",
      "Datum/Cota (m)",
      "PP (kgf/cm²)",
      "Gradientes (kgf/cm²/m)",
      "",
      "",
      "Contatos (cota - m)",
      "",
      "",
      "",
      "",
    ],
    [
      "",
      "",
      "",
      "",
      "Gás",
      "Óleo",
      "Água",
      "Gás/Óleo",
      "Óleo/Água",
      "Topo",
      "Base",
    ],
    ["", "", 0, 0, 0, 0, 0, 0, 0, 0, 0],
  ];

  /**
   * Tabela.
   */
  table: Handsontable;

  /**
   * Opções dos RESERVATÓRIOS
   */
  optionsParameters: any = TableOptions.createDefault(
    {
      // minSpareRows: 1,
      disableVisualSelection: true,
      height: 100,
      width: 1200,
      mergeCells: [
        { row: 0, col: 0, rowspan: 3, colspan: 1 },
        { row: 0, col: 1, rowspan: 1, colspan: 8 },
        { row: 0, col: 9, rowspan: 2, colspan: 2 },
        { row: 1, col: 1, rowspan: 2, colspan: 1 },
        { row: 1, col: 2, rowspan: 2, colspan: 1 },
        { row: 1, col: 3, rowspan: 2, colspan: 1 },
        { row: 1, col: 4, rowspan: 1, colspan: 3 },
        { row: 1, col: 7, rowspan: 1, colspan: 2 },
      ],
      cells: function (row, col) {
        const cellPrp: any = {};
        if (row <= 2) {
          cellPrp.readOnly = true;
          cellPrp.renderer = function (instance, td) {
            Handsontable.renderers.TextRenderer.apply(this, arguments);
            td.style.color = "#222";
            td.style.background = "#edeef0";
          };
        }
        return cellPrp;
      },
    },
    true
  );

  constructor(
    public dialogRef: MatDialogRef<DialogBaseComponent>,
    public dialog: DialogService,
    private notybarService: NotybarService,
    @Inject(MAT_DIALOG_DATA)
    public data: { data: { context; reservatorio: Reservatorio } }
  ) { }

  ngOnInit() {
    if (this.data.data.reservatorio) {
      const res: Reservatorio = this.data.data.reservatorio;
      this.dataset[3][0] = res.nome;
      this.dataset[3][1] = res.referencia.poco;
      this.dataset[3][2] = res.referencia.cota;
      this.dataset[3][3] = res.referencia.pp;
      this.dataset[3][4] = res.referencia.gradiente.gas;
      this.dataset[3][5] = res.referencia.gradiente.oleo;
      this.dataset[3][6] = res.referencia.gradiente.agua;
      this.dataset[3][7] = res.referencia.contatos.gasOleo;
      this.dataset[3][8] = res.referencia.contatos.oleoAgua;
      this.dataset[3][9] = res.interesse.topo;
      this.dataset[3][10] = res.interesse.base;
    }
  }

  closeModal(): void {
    this.dialogRef.close();
  }

  invalidCel(comment, row, col, message) {
    comment.setCommentAtCell(row, col, message);
    comment.updateCommentMeta(row, col, { readOnly: true });
    this.table.setCellMetaObject(row, col, { valid: false });
  }

  canSubmit(): boolean {
    const comment = this.table.getPlugin("comments");
    let valido = true;
    console.log(this.dataset[3]);
    if (
      this.dataset[3][0] === "" ||
      this.dataset[3][0] === null ||
      this.dataset[3][0] === undefined
    ) {
      this.invalidCel(comment, 3, 0, "Não pode ficar vazio.");
      valido = false;
    } else {
      comment.removeCommentAtCell(3, 0);
    }
    if (
      this.dataset[3][1] === "" ||
      this.dataset[3][1] === null ||
      this.dataset[3][1] === undefined
    ) {
      this.invalidCel(comment, 3, 1, "Não pode ficar vazio.");
      valido = false;
    } else {
      comment.removeCommentAtCell(3, 1);
    }
    for (let i = 2; i < this.dataset[3].length; i++) {
      if (!NumberUtils.isNumber(this.dataset[3][i])) {
        this.invalidCel(comment, 3, i, "Precisa ser um número.");
        valido = false;
      } else {
        this.dataset[3][i] = parseFloat(this.dataset[3][i]);
        comment.removeCommentAtCell(3, i);
      }
    }
    if (valido) {
      if (this.dataset[3][9] < this.dataset[3][10]) {
        this.invalidCel(
          comment, 3, 9,
          "Por ser em cota, o topo precisa ser maior que a base."
        );
        this.invalidCel(
          comment, 3, 10,
          "Por ser em cota, o topo precisa ser maior que a base."
        );
        valido = false;
      } else {
        comment.removeCommentAtCell(3, 9);
        comment.removeCommentAtCell(3, 10);
      }
    }
    this.table.render();

    return valido;
  }

  submit() {
    if (!this.canSubmit()) {
      this.notybarService.show("Conserte os erros na tabela.", "warning");
      return;
    }
    const res: Reservatorio = {
      nome: this.dataset[3][0],
      referencia: {
        poco: this.dataset[3][1],
        cota: Number(this.dataset[3][2]),
        pp: Number(this.dataset[3][3]),
        gradiente: {
          gas: Number(this.dataset[3][4]),
          oleo: Number(this.dataset[3][5]),
          agua: Number(this.dataset[3][6]),
        },
        contatos: {
          gasOleo: Number(this.dataset[3][7]),
          oleoAgua: Number(this.dataset[3][8]),
        },
      },
      interesse: {
        topo: Number(this.dataset[3][9]),
        base: Number(this.dataset[3][10]),
      },
    };

    this.data.data.context.salvarReservatórios(res);
    this.closeModal();
  }

  ngAfterViewInit() {
    this.table = this.tableComponent.getHandsontableInstance();
  }
}
