import { Component, OnInit, Input } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { ObjetoIdentificado } from 'app/repositories/models/correlation';

const ELEMENT_DATA: ObjetoIdentificado[] = [
  { name: 'Teste', value: 0 },
  { name: 'Teste', value: 0 }
];

@Component({
  selector: 'app-data-summary',
  templateUrl: './data-summary.component.html',
  styleUrls: ['./data-summary.component.scss']
})
export class DataSummaryComponent implements OnInit {

  @Input() profiles: ObjetoIdentificado[] = [];
  @Input() consts: ObjetoIdentificado[] = [];
  @Input() vars: ObjetoIdentificado[] = [];

  oneColumn: string[] = ['name'];
  twoColumns: string[] = ['name', 'value'];
  dataSource = new MatTableDataSource(ELEMENT_DATA);

  constructor() { }

  ngOnInit() {
  }

}
