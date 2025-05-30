
import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { State } from './models/state';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
export class StateApiService {

    constructor(private http: HttpClient) { }

    /**
     * Update the case state
     *
     * @returns {Promise<{ state: State }>}
     * @memberof StateService
     */
    public update(caseId: string, state: State): Promise<{ state: State }> {
        return this.http.put<{ state: State }>(
            `${environment.appUrl}/api/pocos/${caseId}/atualizar-state`, state
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
