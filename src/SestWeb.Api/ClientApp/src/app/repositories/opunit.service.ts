
import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { OpUnit, OpUnitData } from './models/opunit';
import { environment } from 'environments/environment';

@Injectable({
  providedIn: 'root'
})

/**
 * The repository that mediates connection with the opunits endpoints.
 */
export class OpunitService {
  /**
   * PUT, PATCH and GET requests with an ID are performed on the endpoint
   * with name in singular form.
   */
  private singularEndpoint = 'opunit';
  /**
   * POST requests and GET requests without an ID are performed on the
   * endpoint with name in plural form.
   */
  private pluralEndpoint = 'opunits';

  constructor(private http: HttpClient) { }

  /**
   * List all opuinits in the system.
   */
  public getAll(): Promise<OpUnit[]> {
    return this.http.get<OpUnit[]>(
      `${environment.pocoWeb}/${this.pluralEndpoint}`
    ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
  }

  /**
   * Gets a particular opunit by ID.
   *
   * @param id the id of the opunit.
   */
  public getById(id: string | number): Promise<OpUnit> {
    return this.http.get<OpUnit>(
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

  public create(unitData: OpUnitData): Promise<OpUnit> {
    return this.http.post<OpUnit>(
      `${environment.pocoWeb}/${this.pluralEndpoint}`,
      unitData
    ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
  }
}
