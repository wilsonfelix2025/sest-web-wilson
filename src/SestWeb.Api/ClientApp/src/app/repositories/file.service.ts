
import { throwError as observableThrowError } from 'rxjs';

import { catchError } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

import { RequestUtils } from '@utils/requests';
import { File, FileData, FileListParams, FileRevision, FileRevisionData } from './models/file';
import { environment } from 'environments/environment';

@Injectable({
    providedIn: 'root'
})
/**
 * The repository that mediates connection with the files endpoints.
 */
export class FileService {

    /**
     * PUT, PATCH and GET requests with an ID are performed on the endpoint
     * with name in singular form.
     *
     * @private
     * @memberof FileService
     */
    private singularEndpoint = 'file';

    /**
     * POST requests and GET requests without an ID are performed on the
     * endpoint with name in plural form.
     *
     * @private
     * @memberof FileService
     */
    private pluralEndpoint = 'files';

    constructor(private http: HttpClient) { }

    /**
     * List all files in the system.
     *
     * @param {FileListParams} [params]
     * @returns {Promise<File[]>}
     * @memberof FileService
     */
    public getAll(params?: FileListParams): Promise<File[]> {
        let requestParams = {};
        let requestParamsAsString = '';
        let url = '';

        if (params) {
            requestParams = params;
            requestParamsAsString = `?${RequestUtils.toURI(requestParams)}`;
        }

        if (params) {
            url = `${environment.appUrl}/api/pocoweb/tree`;
        } else {
            url = `${environment.pocoWeb}/${this.pluralEndpoint}/${requestParamsAsString}`;
        }

        return this.http.get<File[]>(url).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Gets a particular file by ID.
     *
     * @param {(string | number)} id the id of the file.
     * @returns {Promise<File>}
     * @memberof FileService
     */
    public getById(id: string | number): Promise<File> {
        return this.http.get<File>(
            `${environment.pocoWeb}/${this.singularEndpoint}/${id}/`
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * List all files filtered by type
     *
     * @param {string} file_type the type of file
     * @returns {Promise<File[]>}
     * @memberof FileService
     */
    public getAllByType(file_type: string): Promise<File[]> {
        return this.getAll({ 'file_type': file_type });
    }

    /**
     * List all files filtered by field updated_after
     *
     * @param {string} updated_after the date of
     * @returns {Promise<File[]>}
     * @memberof FileService
     */
    public getAllUpdatedAfter(updated_after: string): Promise<File[]> {
        return this.getAll({ 'updated_after': updated_after });
    }

    /**
     * List all files filtered by field updated_before
     *
     * @param {string} updated_before the timestamp of last
     * @returns {Promise<File[]>}
     * @memberof FileService
     */
    public getAllUpdatedBefore(updated_before: string): Promise<File[]> {
        return this.getAll({ 'updated_before': updated_before });
    }

    /**
     * Create a file from a given file data.
     *
     * @param {FileData} fileData information about the file.
     * @returns {Promise<File>}
     * @memberof FileService
     */
    public create(fileData: FileData): Promise<File> {
        return this.http.post<File>(
            `${environment.appUrl}/api/pocoweb/${this.pluralEndpoint}/`,
            fileData
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Get the revisions of the file with the given ID.
     *
     * @param {(string | number)} id the ID of the file to fetch revisions.
     * @returns {Promise<FileRevision[]>}
     * @memberof FileService
     */
    public getRevisions(id: string | number): Promise<FileRevision[]> {
        return this.http.get<FileRevision[]>(
            `${environment.pocoWeb}/${this.singularEndpoint}/${id}/revisions/`,
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Create a new revision to the file with the given ID.
     *
     * @param {(string | number)} id the ID of the file to which the new revision should be addeed.
     * @param {FileRevisionData} revisionData the content of the revision.
     * @returns {Promise<FileRevision[]>}
     * @memberof FileService
     */
    public addRevision(id: string | number, revisionData: FileRevisionData): Promise<FileRevision[]> {
        let payload = {};
        payload = revisionData;

        return this.http.post<FileRevision[]>(
            `${environment.pocoWeb}/${this.singularEndpoint}/${id}/revisions/`,
            payload
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Deletes a particular file (actually marks it as archived).
     *
     * @param {(string | number)} id the ID of the file to delete.
     * @returns
     * @memberof FileService
     */
    public delete(id: string | number) {
        return this.http.delete(
            `${environment.appUrl}/api/pocoweb/${this.pluralEndpoint}/${id}/`,
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }


    /**
     * Moves a particular file to another well.
     *
     * @param {(string | number)} fileId the ID of the file to rename.
     * @param {string} wellId the new file name.
     * @returns
     * @memberof FileService
     */
    public move(fileId: string | number, wellId: string | number) {
        return this.http.post(
            `${environment.appUrl}/api/pocoweb/${this.pluralEndpoint}/move`,
            { wellId: wellId, fileId: fileId }
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Duplicate a particular file.
     *
     * @param {(string | number)} id the ID of the file to duplicate.
     * @returns
     * @memberof FileService
     */
    public duplicate(id: string | number): Promise<File> {
        return this.http.post<File>(
            `${environment.appUrl}/api/pocoweb/${this.pluralEndpoint}/duplicate`,
            { id: id }
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }

    /**
     * Rename a particular file.
     *
     * @param {(string | number)} id the ID of the file to rename.
     * @param {string} newName the new file name.
     * @returns
     * @memberof FileService
     */
    public rename(id: string | number, newName: string) {
        return this.http.post(
            `${environment.appUrl}/api/pocoweb/${this.pluralEndpoint}/rename`,
            { id: id, newName: newName }
        ).pipe(catchError(err => observableThrowError(err.message))).toPromise();
    }
}
