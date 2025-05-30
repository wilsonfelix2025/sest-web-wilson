
import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Trend } from './models/trend';
import { Perfil } from './models/perfil';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class TrendService {


    constructor(private http: HttpClient) { }

    /**
     * Create a new trend based on the profile received.
     *
     * @param {string} idProfile the id profile of the profile to create the trend.
     * @returns {Promise<{ trend: Trend}>}
     * @memberof TrendService
     */
    public create(idProfile: string): Promise<{ trend: Trend }> {
        return this.http.post<{ trend: Trend }>(
            `${environment.appUrl}/api/criar-trend?idPerfil=${idProfile}`, undefined
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Delete a trend based on the profile received.
     *
     * @param {string} idProfile the id profile of the profile to delete the trend.
     * @memberof TrendService
     */
    public remove(idProfile: string) {
        return this.http.delete<{ trend: Trend, perfisAlterados: Perfil[] }>(
            `${environment.appUrl}/api/remover-trend?idPerfil=${idProfile}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Save an edited trend on the profile received.
     *
     * @param {Perfil} profile the profile where the trend was edited.
     * @returns {Promise<{ trend: Trend}>}
     * @memberof TrendService
     */
    public edit(profile: Perfil): Promise<{ trend: Trend, perfisAlterados: Perfil[] }> {
        const trend = {
            trechos: profile.trend.trechos,
            idPerfil: profile.id,
            nomeTrend: profile.trend.nome
        };

        return this.http.put<{ trend: Trend, perfisAlterados: Perfil[] }>(
            `${environment.appUrl}/api/editar-trend`, trend
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
