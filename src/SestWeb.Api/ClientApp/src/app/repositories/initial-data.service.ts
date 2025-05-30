import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpParams } from '@angular/common/http';
import { InsertInitialData } from '@utils/interfaces';
import { Perfil } from 'app/repositories/models/perfil';
import { throwError as observableThrowError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})

export class InitialDataService {

    constructor(private http: HttpClient) { }

    /**
     * @param {string} caseId id do caso.
     * @memberof InitialDataService
     */
    getProfilesForInitialData(caseId: string) {
        return this.http.get(
            `${environment.appUrl}/api/pocos/${caseId}/obter-perfis-trecho`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * @param {InsertInitialData} data o dados a serem inseridos no caso.
     * @memberof InitialDataService
     */
    insertInitialData(data: InsertInitialData) {
        return this.http.post<{ perfil: Perfil }>(
            `${environment.appUrl}/api/inserir-trecho-inicial`, data
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * @param {string} perfilId o dados do arquivo a serem importados.
     * @param {number} pmLimite o dados do arquivo a serem importados.
     * @param {string[]} litologiasSelecionadas o dados do arquivo a serem importados.
     * @param {string} tipoTrecho o dados do arquivo a serem importados.
     * @memberof InitialDataService
     */
    getProfileTotalPoints(perfilId: string, pmLimite: number, litologiasSelecionadas: string[], tipoTrecho: string) {
        const url = `${environment.appUrl}/api/perfis/obter-qtd-ponto`;
        let litos = '';

        if (litologiasSelecionadas !== null) {
            litos = litologiasSelecionadas.join(',');
        }

        const params = new HttpParams().set('perfilId', perfilId).set('pmLimite', pmLimite.toString())
            .set('litologiasSelecionadas', litos).set('tipoTrecho', tipoTrecho);

        return this.http.get(url, { params: params })
            .pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

}