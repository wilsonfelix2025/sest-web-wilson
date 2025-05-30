import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { CorrelaçõesPossíveis } from './models/propriedades-mecanicas';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class PropriedadesMecanicasService {

    constructor(private http: HttpClient) { }

    /**
     * Obtém todas correlações ucs, coesa, angat e restr que estão em relacionamentos, agrupadas por seus grupos Litológicos.
     *
     * @param {string} idCaso id do caso atual.
     * @memberof PropriedadesMecanicasService
     */
    public obterCorrelacoesPorGruposLito(idCaso: string): Promise<{ correlaçõesPossíveis: { possibleCorrs: CorrelaçõesPossíveis[] } }> {
        return this.http.get<{ correlaçõesPossíveis: { possibleCorrs: CorrelaçõesPossíveis[] } }>(
            `${environment.appUrl}/api/propmec/get-all-correlations/${idCaso}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Obtém todas correlações ucs, coesa, angat e restr que estão em relacionamentos, filtradas pelas seleções feitas.
     *
     * @param {string} idCaso id do caso atual.
     * @param {string} lito nome do grupo litologico selecionado.
     * @param {string} ucs nome da correlação ucs selecionada.
     * @param {string} coesa nome da correlação coesa selecionada.
     * @param {string} angat nome da correlação angat selecionada.
     * @param {string} restr nome da correlação restr selecionada.
     * @memberof PropriedadesMecanicasService
     */
    public obterCorrelacoesPossiveis(idCaso: string, lito: string, ucs?: string, coesa?: string, angat?: string, restr?: string, biot?: string)
        : Promise<{ correlaçõesPossíveis: CorrelaçõesPossíveis }> {
        return this.http.post<{ correlaçõesPossíveis: CorrelaçõesPossíveis }>(
            `${environment.appUrl}/api/propmec/get-all-possible-correlations`, {
            idPoço: idCaso, grupoLitológicoSelecionado: lito,
            corrUcsSelecionada: ucs,
            corrCoesaSelecionada: coesa,
            corrAngatSelecionada: angat,
            corrRestrSelecionada: restr,
            corrBiotSelecionada: biot,
        }).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
