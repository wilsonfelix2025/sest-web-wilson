import { Component, OnInit, Input } from '@angular/core';
import { NumberUtils } from '@utils/number';
import { Case, Geometria } from 'app/repositories/models/case';
import { environment } from 'environments/environment';

@Component({
  selector: 'sest-geometria',
  templateUrl: './geometria.component.html',
  styleUrls: ['./geometria.component.scss']
})
export class GeometriaComponent implements OnInit {
  @Input() caseData: Case;

  wellType: 'OnShore' | 'OffShore';
  utmX: number = 0;
  utmY: number = 0;

  rotaryTable: number = 0;
  waterBlade: number = 0;
  deepVertical: boolean = false;
  deepVerticalValue: number = 0;

  forefootHeight: number = 0;
  groundwater: number = 0;
  elevation: number = 0;

  offshoreUrl = `${environment.appUrl}/assets/images/offshore.jpg`;
  onshoreUrl = `${environment.appUrl}/assets/images/onshore.jpg`;


  constructor(
  ) { }

  ngOnInit() {
    this.wellType = this.caseData.dadosGerais.geometria.categoriaPoço;
    this.utmX = this.caseData.dadosGerais.geometria.coordenadas.utMx;
    this.utmY = this.caseData.dadosGerais.geometria.coordenadas.utMy;

    this.deepVertical = this.caseData.trajetória.éVertical;
    if (this.deepVertical) {
      this.deepVerticalValue = this.caseData.trajetória.últimoPonto.pm.valor;
    }

    this.rotaryTable = this.caseData.dadosGerais.geometria.mesaRotativa;

    if (this.wellType === 'OffShore') {
      this.waterBlade = this.caseData.dadosGerais.geometria.offShore.laminaDagua;
    } else {
      this.forefootHeight = this.caseData.dadosGerais.geometria.onShore.alturaDeAntePoço;
      this.groundwater = this.caseData.dadosGerais.geometria.onShore.lençolFreático;
      this.elevation = this.caseData.dadosGerais.geometria.onShore.elevação;
    }
  }

  getData() {
    const data: {
      geometria?: Geometria,
      éVertical?: boolean,
      pmFinal?: number,
    } = {};
    const geometria: Geometria = {
      onShore: {
        lençolFreático: this.groundwater,
        elevação: this.elevation,
        alturaDeAntePoço: this.forefootHeight,
      },
      offShore: {
        laminaDagua: this.waterBlade,
      },
      coordenadas: {
        utMx: this.utmX,
        utMy: this.utmY,
      },
      categoriaPoço: this.wellType,
      mesaRotativa: this.rotaryTable,
    };
    const éVertical = this.deepVertical;
    const pmFinal = Number(this.deepVerticalValue);

    let hasAnyChange = false;
    Object.keys(this.caseData.dadosGerais.geometria).forEach(key => {
      const keys = Object.keys(this.caseData.dadosGerais.geometria[key]);
      if (keys.length > 0) {
        keys.forEach(l => {
          if (geometria[key][l] !== this.caseData.dadosGerais.geometria[key][l]) {
            hasAnyChange = true;
            return;
          }
        });
      } else if (geometria[key] !== this.caseData.dadosGerais.geometria[key]) {
        hasAnyChange = true;
        return;
      }
    });
    if (hasAnyChange) {
      data.geometria = geometria;
    }
    if (éVertical !== this.caseData.trajetória.éVertical) {
      hasAnyChange = true;
      data.éVertical = éVertical;
    }
    if (pmFinal !== this.caseData.trajetória.últimoPonto.pm.valor) {
      hasAnyChange = true;
      data.pmFinal = pmFinal;
    }
    if (!hasAnyChange) {
      return {};
    }

    return data;
  }

  onKeyPress(event: KeyboardEvent, value: string | number) {
    let inputChar = event.key;

    if (value !== undefined) {
      inputChar = value + inputChar;
    }

    if (!NumberUtils.isNumber(inputChar)) {
      event.preventDefault();
    }
  }
}
