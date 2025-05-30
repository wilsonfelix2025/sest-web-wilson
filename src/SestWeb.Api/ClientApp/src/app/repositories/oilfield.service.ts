
import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Oilfield, OilfieldData } from './models/oilfield';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
/**
 * The repository that mediates connection with the oilfields endpoints.
 */
export class OilfieldService {
    /**
     * PUT, PATCH and GET requests with an ID are performed on the endpoint
     * with name in singular form.
     */
    private singularEndpoint = 'oilfield';
    /**
     * POST requests and GET requests without an ID are performed on the
     * endpoint with name in plural form.
     */
    private pluralEndpoint = 'oilfields';

    constructor(private http: HttpClient) { }

    /**
     * List all oilfields in the system.
     */
    public getAll(): Promise<Oilfield[]> {
        return this.http.get<Oilfield[]>(
            `${environment.pocoWeb}/${this.pluralEndpoint}`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Gets a particular oilfield by ID.
     *
     * @param id the id of the oilfield.
     */
    public getById(id: string | number): Promise<Oilfield> {
        return this.http.get<Oilfield>(
            `${environment.pocoWeb}/${this.singularEndpoint}/${id}/`
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

    public move(oilfieldId: string | number, unitId: string | number) {
        return this.http.post(`${environment.pocoWeb}/${this.pluralEndpoint}/move`, {
            unitId: unitId,
            oilfieldId: oilfieldId
        }).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    public create(oilfieldData: OilfieldData): Promise<Oilfield> {
        return this.http.post<Oilfield>(
            `${environment.pocoWeb}/${this.pluralEndpoint}`,
            oilfieldData
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
