import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Perfil } from './models/perfil';
import { TipoConversao } from './models/conversao';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ConversaoPerfisService {

    constructor(private http: HttpClient) { }

    /**
     * Converte o perfil escolhido
     *
     * @param {string} idPerfil id do perfil a ser convertido.
     * @param {TipoConversao} tipoConversao tipo da conversao a ser realizada no perfil.
     * @memberof ConversaoPerfisService
     */
    public converter(idPerfil: string, tipoConversao: TipoConversao): Promise<{ perfil: Perfil }> {
        return this.http.post<{ perfil: Perfil }>(
            `${environment.appUrl}/api/converter-grad-tensao`,
            { idPerfil: idPerfil, tipoConversão: tipoConversao }
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
