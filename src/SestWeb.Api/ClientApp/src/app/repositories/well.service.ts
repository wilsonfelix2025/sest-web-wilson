
import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Well, WellData } from './models/well';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
/**
 * The repository that mediates connection with the wells endpoints.
 */
export class WellService {
    /**
     * PUT, PATCH and GET requests with an ID are performed on the endpoint
     * with name in singular form.
     */
    private singularEndpoint = 'well';
    /**
     * POST requests and GET requests without an ID are performed on the
     * endpoint with name in plural form.
     */
    private pluralEndpoint = 'wells';

    constructor(private http: HttpClient) { }

    /**
     * List all wells in the system.
     */
    public getAll(): Promise<Well[]> {
        return this.http.get<Well[]>(
            `${environment.pocoWeb}/${this.pluralEndpoint}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Gets a particular well by ID.
     *
     * @param id the id of the well.
     */
    public getById(id: string | number): Promise<Well> {
        return this.http.get<Well>(
            `${environment.pocoWeb}/${this.singularEndpoint}/${id}/`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Create a well from a given well data.
     *
     * @param wellData information about the well.
     */
    public create(wellData: WellData): Promise<Well> {
        return this.http.post<Well>(
            `${environment.appUrl}/api/pocoweb/${this.pluralEndpoint}/`,
            wellData
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Edit a well with a given id, replacing its data with the given info.
     *
     * @param id the id of the well to edit.
     * @param wellData the new information of the well.
     */
    public edit(id: string | number, wellData: WellData): Promise<Well> {
        return this.http.put<Well>(
            `${environment.pocoWeb}/${this.singularEndpoint}/${id}`,
            wellData
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    public rename(id: string | number, newName: string) {
        return this.http.post(`${environment.pocoWeb}/${this.pluralEndpoint}/rename`, {
            id: id
        }).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    public delete(id: string | number) {
        return this.http.delete(`${environment.pocoWeb}/${this.singularEndpoint}/${id}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    public move(wellId: string | number, oilfieldId: string | number) {
        return this.http.post(`${environment.pocoWeb}/${this.pluralEndpoint}/move`, {
            oilfieldId: oilfieldId,
            wellId: wellId
        }).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
