
import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Correlation, CorrelationSlim, ObjetoIdentificado } from './models/correlation';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class CorrelationService {

    /**
     * All correlations endpoints starts with this commonEndpoind
     *
     * @private
     * @memberof CorrelationService
     */
    private commonEndpoint = 'api/correlacoes';

    public list: Correlation[] = [];

    constructor(private http: HttpClient) { }

    /**
     * Get all correlations from the system with just small weight.
     *
     * @returns {Promise<Correlation[]>}
     * @memberof CorrelationService
     */
    public getAllSlim(idWell: string): Promise<CorrelationSlim[]> {
        return this.http.get<Correlation[]>(
            `${environment.appUrl}/${this.commonEndpoint}/get-all-slim/${idWell}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Get all correlations from the system.
     *
     * @returns {Promise<Correlation[]>}
     * @memberof CorrelationService
     */
    public getAll(idWell: string): Promise<Correlation[]> {
        return this.http.get<Correlation[]>(
            `${environment.appUrl}/${this.commonEndpoint}/get-all/${idWell}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Get a correlation by name, with more details.
     *
     * @param {string} name name of the correlation.
     * @returns {Promise<Correlation>}
     * @memberof CorrelationService
     */
    public getByName(idWell: string, name: string): Promise<Correlation> {
        return this.http.get<Correlation>(
            `${environment.appUrl}/${this.commonEndpoint}/get-by-name/${idWell}/${name}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Create a correlation from a given correlation data.
     *
     * @param {Correlation} correlation information about the correlation.
     * @returns {Promise<{ correlação: Correlation }>}
     * @memberof CorrelationService
     */
    public createGlobal(correlation: Correlation): Promise<{ correlação: Correlation }> {
        return this.http.post<{ correlação: Correlation }>(
            `${environment.appUrl}/${this.commonEndpoint}/create`,
            correlation
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Create a correlation from a given correlation data.
     *
     * @param {Correlation} correlation information about the correlation.
     * @returns {Promise<{ correlação: Correlation }>}
     * @memberof CorrelationService
     */
    public createLocal(correlation: Correlation): Promise<{ correlação: Correlation }> {
        return this.http.post<{ correlação: Correlation }>(
            `${environment.appUrl}/${this.commonEndpoint}/create-well-corr`,
            correlation
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Publish globally a correlation.
     *
     * @param {Correlation} correlation The correlation to publish.
     * @memberof CorrelationService
     */
    public publish(idWell: string, correlation: Correlation) {
        return this.http.put<{ mensagem: string }>(
            `${environment.appUrl}/${this.commonEndpoint}/${idWell}/${correlation.nome}/publish-corr`,
            undefined
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Delete a correlation.
     *
     * @param {Correlation} correlation The correlation to delete.
     * @memberof CorrelationService
     */
    public remove(correlation: Correlation) {
        return this.http.delete(
            `${environment.appUrl}/${this.commonEndpoint}/${correlation.nome}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Update a local correlation from a given correlation data.
     *
     * @param {Correlation} correlation the new information about the correlation.
     * @returns {Promise<{ mensagem: string }>}
     * @memberof CorrelationService
     */
    public updateLocal(idWell: string, correlation: Correlation): Promise<{ mensagem: string }> {
        return this.http.put<{ mensagem: string }>(
            `${environment.appUrl}/${this.commonEndpoint}/${idWell}/${correlation.nome}/update-well-corr`,
            correlation
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    public CONST_REGEX: RegExp = this.generateConstVarRegex('const');
    public VAR_REGEX: RegExp = this.generateConstVarRegex('var');

    private generateConstVarRegex(type: string): RegExp {
        return new RegExp(
            /\b/g.source +
            type +
            /\b\s+\b(\w+)\b\s*=\s*\b([\w\.]+)\b/g.source, 'g'
        );
    }

    public matchConstVarRegex(regex: RegExp, equation: string): ObjetoIdentificado[] {
        let match: ObjetoIdentificado[] = [];
        let m = regex.exec(equation);
        if (m === null) {
            return [];
        }
        while (m) {
            match.push({ name: m[1], value: m[2], valid: true });
            m = regex.exec(equation);
        }
        return match;
    }
}
