import { Inject, Injectable, InjectionToken } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
} from '@angular/common/http';

import { Observable, throwError } from 'rxjs';
import { map, catchError, timeout } from 'rxjs/operators';
import { NotybarService } from '../services/notybar.service';
import { LoaderService } from '../services/loader.service';


export const DEFAULT_TIMEOUT = new InjectionToken<number>('defaultTimeout');
/**
 * This class is responsible for adding the authorization token for every POCOWEB request
 */
@Injectable()
export class HttpConfigInterceptor implements HttpInterceptor {
  private requests: HttpRequest<any>[] = [];

  public urlWithoutLoading = [
    'atualizar-state',
    'upload',
    'editar-trend',
    'propmec/get-all-possible-correlations',
  ];

  constructor(
    @Inject(DEFAULT_TIMEOUT) protected defaultTimeout: number,
    private notybarService: NotybarService,
    private loaderService: LoaderService
  ) { }

  removeRequest(req: HttpRequest<any>) {
    const i = this.requests.indexOf(req);
    if (i >= 0) {
      this.requests.splice(i, 1);
      this.loaderService.removeLoading();
    }
  }

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {
    const timeoutValue = request.headers.get('timeout') || this.defaultTimeout;
    const timeoutValueNumeric = Number(timeoutValue);
    // Ensures the token is only added for production API URL
    // if (!environment.production || !request.url.startsWith(PROD_BASE_URL)) {
    //   return next.handle(request);
    // }
    const token: string = localStorage.getItem('accessToken');

    if (token) {
      if (!request.headers.get('Authorization')) {
        request = request.clone({
          headers: request.headers.set('Authorization', 'Bearer ' + token),
        });
      }
    }

    if (!request.headers.has('Access-Control-Allow-Credentials')) {
      request = request.clone({
        headers: request.headers.set(
          'Access-Control-Allow-Credentials',
          'true'
        ),
      });
    }

    if (!request.headers.has('Access-Control-Allow-Origin')) {
      request = request.clone({
        headers: request.headers.set('Access-Control-Allow-Origin', '*'),
      });
    }

    if (!request.headers.get('Accept')) {
      request = request.clone({
        headers: request.headers.set('Accept', 'application/json'),
      });
    }

    const lastUrl = request.url.split('/').pop();
    if (this.shouldDisplayLoader(request.url, request.method)) {
      this.requests.push(request);
      this.loaderService.addLoading();
    }

    return next.handle(request).pipe(
      timeout(timeoutValueNumeric),
      map((event: HttpEvent<any>) => {
        if (event['status']) {
          this.removeRequest(request);
        }
        return event;
      }),
      catchError((error: HttpErrorResponse) => {
        console.error(error);
        if (error) {
          if (
            error instanceof HttpErrorResponse &&
            error.error instanceof Blob &&
            error.error.type === 'application/json'
          ) {
            error.error['text']().then((val) => {
              try {
                const errorInfo = JSON.parse(val);
                this.notybarService.show(errorInfo.mensagem, 'danger');
              } catch (e) {
                this.notybarService.show(
                  'Ocorreu um erro e não foi possível processar a resposta do servidor.'
                );
              }
            });
            // const reader = new FileReader();
            // reader.onload = (e: Event) => {
            //   try {
            //     const errmsg = JSON.parse((<any>e.target).result);
            //     console.log(errmsg);
            //   } catch (e) {
            //     console.warn('erro ao parsear o json');
            //   }
            // };
            // reader.onerror = (e) => {
            //   console.warn('erro no leitor');
            // };
            // console.log(reader.readAsText(error.error));
          }
        }

        switch (error.status) {
          case 400:
            if (Object.keys(error.error)[0] === 'mensagem') {
              this.notybarService.show(error.error.mensagem, 'danger');
            } else {
              this.notybarService.show(error.error, 'danger');
            }
            break;
          default:
            this.notybarService.show(error.message, 'danger');
            break
        }

        // console.error('Erro ao interceptar a URL', data);
        this.removeRequest(request);
        return throwError(error);
      })
    );
  }

  shouldDisplayLoader(url: string, method: string) {
    for (const endpoint of this.urlWithoutLoading) {
      if (url.endsWith(endpoint)) {
        return false;
      }
    }

    if (url.indexOf('/perfis/') > -1 && method === 'PUT') {
      return false;
    }

    return true;
  }
}
