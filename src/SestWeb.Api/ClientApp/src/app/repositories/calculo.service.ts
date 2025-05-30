import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { Calculo, CalculoPerfis, CalculoSobrecarga, CalculoGPP, CalculoPPH, CalculoPP, CalculoPropriedadesMecanicas, CalculoTensoesInSitu, CalcularGraficoTensoes, TipoGraficoTensoes, CalculoGradiente, CalculoExpD } from './models/calculo';
import { Perfil } from './models/perfil';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class CalculoService {

    constructor(private http: HttpClient) { }

    /**
     * Cria calculo de perfis com os dados recebidos
     *
     * @param {CalculoPerfis} calculoPerfil dados para calculo de perfis.
     * @memberof CalculoService
     */
    public criarCalculoPerfis(calculoPerfil: CalculoPerfis): Promise<{ cálculo: Calculo }> {
        return this.http.post<{ cálculo: Calculo }>(
            `${environment.appUrl}/api/criar-calculo-perfis`,
            calculoPerfil
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Edita calculo de perfis com os dados recebidos
     *
     * @param {CalculoPerfis} calculoPerfil dados para calculo de perfis.
     * @memberof CalculoService
     */
    public editarCalculoPerfis(calculoPerfil: CalculoPerfis): Promise<{ cálculo: Calculo, perfisAlterados: Perfil[] }> {
        return this.http.post<{ cálculo: Calculo, perfisAlterados: Perfil[] }>(
            `${environment.appUrl}/api/editar-calculo-perfis`,
            calculoPerfil
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Cria calculo de sobrecarga com os dados recebidos
     *
     * @param {CalculoSobrecarga} calculoSobrecarga dados para calculo de sobrecarga.
     * @memberof CalculoService
     */
    public criarCalculoSobrecarga(calculoSobrecarga: CalculoSobrecarga): Promise<{ cálculo: Calculo }> {
        return this.http.post<{ cálculo: Calculo }>(
            `${environment.appUrl}/api/criar-calculo-sobrecarga`,
            calculoSobrecarga
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Edita calculo de sobrecarga com os dados recebidos
     *
     * @param {CalculoSobrecarga} calculoSobrecarga dados para calculo de sobrecarga.
     * @memberof CalculoService
     */
    public editarCalculoSobrecarga(calculoSobrecarga: CalculoSobrecarga): Promise<{ cálculo: Calculo, perfisAlterados: Perfil[] }> {
        return this.http.post<{ cálculo: Calculo, perfisAlterados: Perfil[] }>(
            `${environment.appUrl}/api/editar-calculo-sobrecarga`,
            calculoSobrecarga
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Cria calculo de pressão de poros com os dados recebidos
     *
     * @param {CalculoPP | CalculoPPH | CalculoGPP} calculoPressaoPoros dados para calculo de pressão de poros.
     * @memberof CalculoService
     */
    public criarCalculoPressaoPoros(calculoPressaoPoros: CalculoPP | CalculoPPH | CalculoGPP): Promise<{ cálculo: Calculo }> {
        return this.http.post<{ cálculo: Calculo }>(
            `${environment.appUrl}/api/criar-calculo-pressao-poros`,
            calculoPressaoPoros
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Edita calculo de pressão de poros com os dados recebidos
     *
     * @param {CalculoPP | CalculoPPH | CalculoGPP} calculoPressaoPoros dados para calculo de pressão de poros.
     * @memberof CalculoService
     */
    public editarCalculoPressaoPoros(calculoPressaoPoros: CalculoPP | CalculoPPH | CalculoGPP): Promise<{ cálculo: Calculo, perfisAlterados: Perfil[] }> {
        return this.http.post<{ cálculo: Calculo, perfisAlterados: Perfil[] }>(
            `${environment.appUrl}/api/editar-calculo-pressao-poros`,
            calculoPressaoPoros
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Cria calculo de propriedades mecânicas com os dados recebidos
     *
     * @param {CalculoPropriedadesMecanicas} calculoPropriedadesMecanicas dados para calculo de propriedades mecânicas.
     * @memberof CalculoService
     */
    public criarCalculoPropriedadesMecanicas(calculoPropMec: CalculoPropriedadesMecanicas): Promise<{ cálculo: Calculo }> {
        return this.http.post<{ cálculo: Calculo }>(
            `${environment.appUrl}/api/criar-calculo-propriedades-mecanicas`,
            calculoPropMec
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Edita calculo de propriedades mecânicas com os dados recebidos
     *
     * @param {CalculoPropriedadesMecanicas} calculoPropMec dados para calculo de propriedades mecânicas.
     * @memberof CalculoService
     */
    public editarCalculoPropriedadesMecanicas(calculoPropMec: CalculoPropriedadesMecanicas): Promise<{ cálculo: Calculo, perfisAlterados: Perfil[] }> {
        return this.http.post<{ cálculo: Calculo, perfisAlterados: Perfil[] }>(
            `${environment.appUrl}/api/editar-calculo-propriedades-mecanicas`,
            calculoPropMec
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Cria calculo de Tensões in Situ com os dados recebidos
     *
     * @param {CalculoTensoesInSitu} calculoTensoes dados para calculo de tensões in situ.
     * @memberof CalculoService
     */
    public criarCalculoTensoesInSitu(calculoTensoes: CalculoTensoesInSitu): Promise<{ cálculo: Calculo }> {
        return this.http.post<{ cálculo: Calculo }>(
            `${environment.appUrl}/api/criar-calculo-tensoesinsitu`,
            calculoTensoes
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Edita calculo de Tensões in Situ com os dados recebidos
     *
     * @param {CalculoTensoesInSitu} calculoTensoes dados para calculo de tensões in situ.
     * @memberof CalculoService
     */
    public editarCalculoTensoesInSitu(calculoTensoes: CalculoTensoesInSitu): Promise<{ cálculo: Calculo, perfisAlterados: Perfil[] }> {
        return this.http.post<{ cálculo: Calculo, perfisAlterados: Perfil[] }>(
            `${environment.appUrl}/api/editar-calculo-tensoesinsitu`,
            calculoTensoes
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Cria grafico de Tensões in Situ com os dados recebidos
     *
     * @param {CalcularGraficoTensoes} opcoesGrafico dados para gerar grafico tensões in situ.
     * @param {TipoGraficoTensoes} tipoGrafico tipo do grafico a ser gerado.
     * @memberof CalculoService
     */
    public criarGraficoTensoesInSitu(opcoesGrafico: CalcularGraficoTensoes, tipoGrafico: TipoGraficoTensoes): Promise<{ perfil?: Perfil, retorno?: { coeficiente?: number, pontosDTO?: { valorX: number, valorY: number }[] } }> {
        let url = `${environment.appUrl}/api/calcular-`;
        if (tipoGrafico === 'NormalizaçãoPP') {
            url += 'normalizacao-pp';
        } else if (tipoGrafico === 'NormalizaçãoLDA') {
            url += 'normalizacao-lda';
        } else if (tipoGrafico === 'K0') {
            url += 'k0';
        } else if (tipoGrafico === 'K0Acompanhamento') {
            url += 'k0-acompanhamento';
        }
        return this.http.post<{ perfil?: Perfil, retorno?: { coeficiente?: number, pontosDTO?: { valorX: number, valorY: number }[] } }>(
            url, opcoesGrafico
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Cria calculo de Gradiente com os dados recebidos
     *
     * @param {CalculoGradiente} calculoGradiente dados para calculo de gradiente.
     * @memberof CalculoService
     */
    public criarCalculoGradiente(calculoGradiente: CalculoGradiente): Promise<{ cálculo: Calculo }> {
        return this.http.post<{ cálculo: Calculo }>(
            `${environment.appUrl}/api/criar-calculo-gradientes`,
            calculoGradiente
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Edita calculo de Gradiente com os dados recebidos
     *
     * @param {CalculoGradiente} calculoGradiente dados para calculo de gradiente.
     * @memberof CalculoService
     */
    public editarCalculoGradiente(calculoGradiente: CalculoGradiente): Promise<{ cálculo: Calculo, perfisAlterados: Perfil[] }> {
        return this.http.post<{ cálculo: Calculo, perfisAlterados: Perfil[] }>(
            `${environment.appUrl}/api/editar-calculo-gradientes`,
            calculoGradiente
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Recalcula o calculo de Gradiente
     *
     * @param {string} idCaso id do caso do calculo.
     * @param {string} idCalculo id do calculo de gradiente.
     * @memberof CalculoService
     */
    public recalcularCalculoGradiente(idCaso: string, idCalculo: string): Promise<{ cálculo: Calculo, perfisAlterados: Perfil[] }> {
        return this.http.post<{ cálculo: Calculo, perfisAlterados: Perfil[] }>(
            `${environment.appUrl}/api/recalcular-calculo-gradientes`,
            { idPoço: idCaso, idCálculo: idCalculo }
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Cria calculo de Expoente D com os dados recebidos
     *
     * @param {CalculoExpD} calculoExpD dados para calculo de expoente D.
     * @memberof CalculoService
     */
    public criarCalculoExpD(calculoExpD: CalculoExpD): Promise<{ cálculo: Calculo }> {
        return this.http.post<{ cálculo: Calculo }>(
            `${environment.appUrl}/api/criar-calculo-expoented`,
            calculoExpD
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Edita calculo de Expoente D com os dados recebidos
     *
     * @param {CalculoExpD} calculoExpD dados para calculo de expoente D.
     * @memberof CalculoService
     */
    public editarCalculoExpD(calculoExpD: CalculoExpD): Promise<{ cálculo: Calculo, perfisAlterados: Perfil[] }> {
        return this.http.post<{ cálculo: Calculo, perfisAlterados: Perfil[] }>(
            `${environment.appUrl}/api/editar-calculo-expoented`,
            calculoExpD
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Remove o calculo com o id recebido
     *
     * @param {string} idPoco o id do poço atual.
     * @param {string} idCalculo o id do calculo a ser removido.
     * @memberof CalculoService
     */
    public obter(idPoco: string, idCalculo: string): Promise<{ cálculo: Calculo }> {
        return this.http.get<{ cálculo: Calculo }>(
            `${environment.appUrl}/api/obter-calculo?idCálculo=${idCalculo}&idPoço=${idPoco}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Remove o calculo com o id recebido
     *
     * @param {string} idPoco o id do poço atual.
     * @param {string} idCalculo o id do calculo a ser removido.
     * @memberof CalculoService
     */
    public remover(idPoco: string, idCalculo: string) {
        return this.http.delete(
            `${environment.appUrl}/api/remover-calculo?idPoço=${idPoco}&idCálculo=${idCalculo}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
