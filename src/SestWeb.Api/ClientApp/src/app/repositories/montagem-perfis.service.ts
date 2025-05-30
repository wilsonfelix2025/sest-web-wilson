import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { MontagemPerfis } from './models/montagem-perfis';
import { Perfil } from './models/perfil';
import { Litologia } from './models/litologia';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class MontagemPerfisService {

    constructor(private http: HttpClient) { }

    /**
     * Monta perfis com os dados recebidos
     *
     * @param {MontagemPerfis} parametros dados para montagem de perfis.
     * @memberof MontagemPerfisService
     */
    public montarPerfis(parametros: MontagemPerfis): Promise<{ listaPerfis: Perfil[], litologia: Litologia }> {
        return this.http.post<{ listaPerfis: Perfil[], litologia: Litologia }>(
            `${environment.appUrl}/api/montar-perfis`,
            parametros
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
