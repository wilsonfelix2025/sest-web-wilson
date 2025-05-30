import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { EstiloUpdate, Evento, Registro, TrechoEventoUpdate, TrechoRegistroUpdate } from './models/registro-evento';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class RegistrosEventosService {

    constructor(private http: HttpClient) { }

    /**
     * Edita os Registros e Eventos de um caso com os dados recebidos
     *
     * @param {string} idCaso id do caso escolhido.
     * @param {(TrechoRegistroUpdate | TrechoEventoUpdate)[]} trechos trechos dos registros e eventos atualizados.
     * @param {EstiloUpdate[]} marcadores estilo visual dos registros e eventos.
     * @memberof RegistrosEventosService
     */
    public editarRegistrosEventos(idCaso: string, tipo: 'Registro' | 'Evento', trechos: (TrechoRegistroUpdate | TrechoEventoUpdate)[], marcadores: EstiloUpdate[]): Promise<{ registrosEventos: (Registro | Evento)[] }> {
        return this.http.put<{ registrosEventos: (Registro | Evento)[] }>(
            `${environment.appUrl}/api/registrosEventos`, {
            idPoço: idCaso,
            tipo: tipo,
            trechos: trechos,
            marcadores: marcadores
        }).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Reinicia um Registro/Evento
     *
     * @param {string} idRegistroEvento id do registro/evento escolhido.
     * @memberof RegistrosEventosService
     */
    public reiniciarRegistroEvento(idRegistroEvento: string): Promise<{}> {
        return this.http.delete(
            `${environment.appUrl}/api/registrosEventos?id=${idRegistroEvento}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
