import { throwError as observableThrowError, Subject } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Export, Relatorio, RelatorioRequest } from './models/export';
import { environment } from 'environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ExportService {

  public $relatorioRequestedChart = new Subject<RelatorioRequest>();
  public relatorio: Relatorio;

  constructor(private http: HttpClient) { }

  /**
   * Export data from the options provided.
   *
   * @param {Export} options
   * @returns {Promise<any>}
   * @memberof ExportService
   */
  public export(options: Export): Promise<any> {
    const requestOptions = {
      observe: 'response' as const,
      responseType: 'blob' as 'json',
    };
    return this.http
      .post<any>(`${environment.appUrl}/api/exportar-arquivo`, options, requestOptions
      ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
  }

  /**
   * Exportar registros e eventos.
   *
   * @param {string} idPoço
   * @param {string[]} registros
   * @returns {Promise<any>}
   * @memberof ExportService
   */
  public exportarRegistros(idPoço: string, registros: string[]): Promise<any> {
    const requestOptions = {
      observe: 'response' as const,
      responseType: 'blob' as 'json',
    };
    return this.http.post<any>(`${environment.appUrl}/api/exportar-registros`, {
      idPoço: idPoço,
      registros: registros
    }, requestOptions
    ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
  }

  /**
   * Gera um preview do relatorio com as opcoes selecionadas
   *
   * @param {Relatorio} relatorio opcoes e graficos a serem usados pre relatorio.
   * @returns {Promise<any>}
   * @memberof ExportService
   */
  public preRelatorio(relatorio: Relatorio): Promise<any> {
    return this.http.post(
      `${environment.appUrl}/api/report/preview`, relatorio
    ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
  }

  /**
   * Baixa o relatorio com as opcoes selecionadas
   *
   * @param {Relatorio} relatorio opcoes e graficos a serem usados relatorio.
   * @returns {Promise<any>}
   * @memberof ExportService
   */
  public exportarRelatorio(relatorio: Relatorio): Promise<any> {
    const requestOptions = {
      observe: 'response' as const,
      responseType: 'blob' as 'json',
    };
    return this.http.post<any>(`${environment.appUrl}/api/report/download`,
      relatorio,
      requestOptions
    ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
  }
}
