
import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { FiltroSimples, FiltroMediaMovel, FiltroLitologia, FiltroLBF, Filtro } from './models/filtro';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class FiltroService {


    constructor(private http: HttpClient) { }

    /**
     * Cria um novo filtro simples com os dados recebidos.
     *
     * @param {FiltroSimples} filtroSimples o filtro para ser criado no banco.
     * @memberof FiltroService
     */
    public criarFiltroSimples(filtroSimples: FiltroSimples) {
        return this.http.post(
            `${environment.appUrl}/api/criar-filtro-simples`, filtroSimples
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Edita um filtro simples com os dados recebidos.
     *
     * @param {FiltroSimples} filtroSimples os dados alterados do novo filtro.
     * @memberof FiltroService
     */
    public editarFiltroSimples(filtroSimples: FiltroSimples) {
        return this.http.put<{ filtro: FiltroSimples }>(
            `${environment.appUrl}/api/editar-filtro-simples`, filtroSimples
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Cria um novo filtro por media móvel com os dados recebidos.
     *
     * @param {FiltroMediaMovel} filtroMediaMovel o filtro para ser criado no banco.
     * @memberof FiltroService
     */
    public criarFiltroMediaMovel(filtroMediaMovel: FiltroMediaMovel) {
        return this.http.post(
            `${environment.appUrl}/api/criar-filtro-media-movel`, filtroMediaMovel
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Edita um filtro por media móvel com os dados recebidos.
     *
     * @param {FiltroMediaMovel} filtroMediaMovel os dados alterados do novo filtro.
     * @memberof FiltroService
     */
    public editarFiltroMediaMovel(filtroMediaMovel: FiltroMediaMovel) {
        return this.http.put<{ filtro: FiltroMediaMovel }>(
            `${environment.appUrl}/api/editar-filtro-media-movel`, filtroMediaMovel
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Cria um novo filtro por litologia com os dados recebidos.
     *
     * @param {FiltroLitologia} filtroLitologia o filtro para ser criado no banco.
     * @memberof FiltroService
     */
    public criarFiltroLitologia(filtroLitologia: FiltroLitologia) {
        return this.http.post(
            `${environment.appUrl}/api/criar-filtro-litologia`, filtroLitologia
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }


    /**
     * Edita um filtro por litologia com os dados recebidos.
     *
     * @param {FiltroLitologia} filtroLitologia os dados alterados do novo filtro.
     * @memberof FiltroService
     */
    public editarFiltroLitologia(filtroLitologia: FiltroLitologia) {
        return this.http.put<{ filtro: FiltroLitologia }>(
            `${environment.appUrl}/api/editar-filtro-litologia`, filtroLitologia
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Cria um novo filtro por LBF com os dados recebidos.
     *
     * @param {FiltroLBF} filtroLBF o filtro para ser criado no banco.
     * @memberof FiltroService
     */
    public criarFiltroLBF(filtroLBF: FiltroLBF) {
        return this.http.post(
            `${environment.appUrl}/api/criar-filtro-lbf`, filtroLBF
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Edita um filtro por LBF com os dados recebidos.
     *
     * @param {FiltroLBF} filtroLBF os dados alterados do novo filtro.
     * @memberof FiltroService
     */
    public editarFiltroLBF(filtroLBF: FiltroLBF) {
        console.log(this);
        return this.http.put<{ filtro: FiltroLBF }>(
            `${environment.appUrl}/api/editar-filtro-lbf`, filtroLBF
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Cria um novo filtro de corte com os dados recebidos.
     *
     * @param {Filtro} corte o filtro para ser criado no banco.
     * @memberof FiltroService
     */
    public criarCorte(corte: Filtro) {
        return this.http.post(
            `${environment.appUrl}/api/criar-filtro-corte`, corte
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Edita um filtro de corte com os dados recebidos.
     *
     * @param {Filtro} corte os dados alterados do novo filtro.
     * @memberof FiltroService
     */
    public editarCorte(corte: Filtro) {
        return this.http.put<{ filtro: Filtro }>(
            `${environment.appUrl}/api/editar-filtro-corte`, corte
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
