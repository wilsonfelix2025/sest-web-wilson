
import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Case, CaseSlim } from './models/case';
import { Caminho } from './models/caminho';
import { UpdateDadosGerais } from '@utils/interfaces';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class CaseService {

    constructor(private http: HttpClient) { }

    /**
     * Get all cases from the system.
     *
     * @returns {Promise<{ poços: CaseSlim[] }>}
     * @memberof CaseService
     */
    public getAll(): Promise<{ poços: CaseSlim[] }> {
        return this.http.get<{ poços: CaseSlim[] }>(
            `${environment.appUrl}/api/pocos`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Get a case from the system by an ID.
     *
     * @param {string} id id of the case.
     *
     * @returns {Promise<{ poços: CaseSlim }>}
     * @memberof CaseService
     */
    public get(id: string): Promise<{ poço: Case, caminho: Caminho }> {
        return this.http.get<{ poço: Case, caminho: Caminho }>(
            `${environment.appUrl}/api/pocos/${id}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Add a support case to the current case.
     *
     * @param {string} id id of the current case.
     * @param {string} idSupport id of the support case.
     *
     * @memberof CaseService
     */
    public addSupport(id: string, idSupport: string) {
        return this.http.put<{ mensagem: string }>(
            `${environment.appUrl}/api/pocos/${id}/add-apoio?idPoçoApoio=${idSupport}`, undefined
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Remove a support case from the current case.
     *
     * @param {string} id id of the current case.
     * @param {string} idSupport id of the support case.
     *
     * @memberof CaseService
     */
    public removeSupport(id: string, idSupport: string) {
        return this.http.put<{ mensagem: string }>(
            `${environment.appUrl}/api/pocos/${id}/remove-apoio?idPoçoApoio=${idSupport}`, undefined
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Update the general data from the current case.
     *
     * @param {string} id id of the current case.
     * @param {UpdateDadosGerais} generalData general data of the current case.
     *
     * @memberof CaseService
     */
    public updateGeneralData(id: string, generalData: UpdateDadosGerais) {
        return this.http.put(
            `${environment.appUrl}/api/pocos/${id}/atualizar-dados`, generalData
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Update the trajectory from the current case.
     *
     * @param {string} id id of the current case.
     * @param trajectoryPoints trajectory data of the current case.
     *
     * @memberof CaseService
     */
    updateTrajectory(id: string, trajectoryPoints) {
        return this.http.post(`${environment.appUrl}/api/pocos/${id}/editar-trajetoria`, {
            pontos: trajectoryPoints.map((el) => {
                el.pm = el.pm.valor;
                return el;
            }),
        }).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Update the lithology from the current case.
     *
     * @param {string} id id of the current case.
     * @param {string} lithoType type of the lithology.
     * @param lithoPoints lithology data of the current case.
     *
     * @memberof CaseService
     */
    updateLithology(id: string, lithoType: string, lithoPoints) {
        return this.http.post(`${environment.appUrl}/api/pocos/${id}/editar-litologia`, {
            tipoLitologia: lithoType,
            pontos: lithoPoints,
        }).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Remove the lithology from the current case.
     *
     * @param {string} id id of the current case.
     * @param {string} lithoId id of the lithology to remove.
     *
     * @memberof CaseService
     */
    removeLithology(id: string, lithoId: string) {
        return this.http.post(`${environment.appUrl}/api/pocos/${id}/remover-litologia/${lithoId}`, {}
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
