import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Perfil } from './models/perfil';
import { ComposicaoPerfil } from './models/composicao-perfis';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ComposicaoPerfisService {

    constructor(private http: HttpClient) { }

    /**
     * Compõe perfil com os dados recebidos
     *
     * @param {ComposicaoPerfil} parametros dados para composição do perfil.
     * @memberof ComposicaoPerfisService
     */
    public comporPerfil(parametros: ComposicaoPerfil): Promise<{ perfil: Perfil }> {
        return this.http.post<{ perfil: Perfil }>(
            `${environment.appUrl}/api/composição-perfil`,
            parametros
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
