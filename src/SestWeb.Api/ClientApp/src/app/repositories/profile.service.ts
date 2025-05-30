import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { CriarPerfilRET, Perfil } from './models/perfil';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ProfileService {

    constructor(private http: HttpClient) { }

    /**
     * Save the profile received.
     *
     * @param {Perfil} profile the profile that was edited.
     * @returns {Promise<{ profile: Perfil }>}
     * @memberof ProfileService
     */
    public edit(profile: Perfil, deepView: 'PM' | 'PV' = 'PM'): Promise<{ perfil: Perfil, perfisAlterados: Perfil[] }> {
        const reqBody = {
            idPerfil: profile.id,
            nome: profile.nome,
            descrição: profile.descrição,
            estiloVisual: profile.estiloVisual,
            emPv: deepView === 'PV',
            pontosDePerfil: profile.pontos.map(el => {
                return {
                    'pm': el.pm.valor,
                    'pv': el.pv.valor,
                    'valor': el.valor,

                    'origemPonto': el.origem,
                };
            })
        };
        return this.http.put<{ perfil: Perfil, perfisAlterados: Perfil[] }>(
            `${environment.appUrl}/api/perfis/${profile.id}`, reqBody
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Delete the profile received.
     *
     * @param {string} idProfile the id profile of the profile to delete.
     * @memberof ProfileService
     */
    public remove(idProfile: string, idPoço: string) {
        return this.http.delete(
            `${environment.appUrl}/api/perfis?idPerfil=${idProfile}&idPoço=${idPoço}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Cria perfil de relação entre as tensões com os dados recebidos. 
     *
     * @param {CriarPerfilRET} perfilRET dados para criação do perfil.
     * @memberof ProfileService
     */
    public criarPerfilRET(perfilRET: CriarPerfilRET): Promise<{ perfil: Perfil }> {
        return this.http.post<{ perfil: Perfil }>(
            `${environment.appUrl}/api/criar-perfil-relacao-tensoes`,
            perfilRET
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
