import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest } from '@angular/common/http';
import { ImportData, ImportTrajetoriaData, ImportLitologiaData, ImportPerfilData } from '@utils/interfaces';
import { throwError as observableThrowError } from 'rxjs';
import { Perfil } from 'app/repositories/models/perfil';
import { catchError } from 'rxjs/operators';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class ImportService {

    constructor(private http: HttpClient) { }

    /**
     * Sobe arquivo no back.
     *
     * @param {File} fileToUpload arquivo a ser feito upload.
     * @memberof ImportService
     */
    uploadFile(fileToUpload: File) {
        // create a new multipart-form for every file
        const formData: FormData = new FormData();
        formData.append('Arquivo', fileToUpload);
        const url = `${environment.appUrl}/api/upload`;

        // create a http-post request and pass the form
        // tell it to report the upload progress
        const req = new HttpRequest('POST', url, formData, {
            reportProgress: true
        });
        return this.http.request(req);
    }


    /**
     * Importa arquivo.
     *
     * @param {ImportData} data o dados do arquivo a serem importados.
     * @memberof ImportService
     */
    importFile(data: ImportData) {
        return this.http.post(
            `${environment.appUrl}/api/ImportarArquivo`, data
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Importa litologia.
     *
     * @param {ImportLitologiaData} data o dados da litologia a serem importados.
     * @memberof ImportService
     */
    importLitologia(data: ImportLitologiaData) {
        return this.http.post(
            `${environment.appUrl}/api/importar-dados/litologia`, data
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Importa perfis.
     *
     * @param {ImportPerfilData} data o dados dos perfis a serem importados.
     * @memberof ImportService
     */
    importPerfil(data: ImportPerfilData): Promise<{ perfis: Perfil[] }> {
        return this.http.post<{ perfis: Perfil[] }>(
            `${environment.appUrl}/api/importar-dados/perfil`, data
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Importa trajetoria.
     *
     * @param {ImportTrajetoriaData} data o dados da trajetoria a serem importados.
     * @memberof ImportService
     */
    importTrajetoria(data: ImportTrajetoriaData) {
        return this.http.post(
            `${environment.appUrl}/api/importar-dados/trajetoria`, data
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
