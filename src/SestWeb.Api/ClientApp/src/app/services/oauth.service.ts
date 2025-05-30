import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Subject, BehaviorSubject } from 'rxjs';
import { RequestUtils } from '@utils/requests';
import { UserInfo } from '../repositories/models/user-info';
import { Router } from '@angular/router';

export interface AuthStatusResponse {
  status: number;
  data: Object;
}

@Injectable({
  providedIn: 'root'
})
export class OAuthTokenService {

  /**
   * Object containing information about the user currently
   * logged in.
   */
  private _loggedUser: UserInfo = {} as UserInfo;
  get loggedUser(): UserInfo {
    if (Object.keys(this._loggedUser).length) {
      return this._loggedUser;
    } else {
      return JSON.parse(localStorage.getItem('userInfo')) as UserInfo;
    }
  }
  set loggedUser(newUserInfo: UserInfo) {
    this._loggedUser = newUserInfo;
    if (Object.keys(newUserInfo).length) {
      localStorage.setItem('userInfo', JSON.stringify(newUserInfo));
    } else {
      localStorage.removeItem('userInfo');
    }
  }

  private _accessToken: string;
  get accessToken(): string { return this._accessToken; }
  set accessToken(newToken: string) {
    this._accessToken = newToken;
    this.updateOnLocalStorage('accessToken', newToken);
  }

  private _expiresAt: number;
  get expiresAt(): number { return this._expiresAt; }
  set expiresAt(newExpiresAt: number) {
    this._expiresAt = newExpiresAt;
    this.updateOnLocalStorage('expiresAt', newExpiresAt);
  }

  private _refreshToken: string;
  get refreshToken(): string { return this._refreshToken; }
  set refreshToken(newRefreshToken: string) {
    this._refreshToken = newRefreshToken;
    this.updateOnLocalStorage('refreshToken', newRefreshToken);
  }

  /**
   * The URI the user must be redirected to after a successful authentication.
   */
  readonly redirectUri: string = `${environment.appUrl}/oidc/complete${environment.production ? '/' : ''}`;

  $userAuthenticated = new BehaviorSubject(false);

  /**
   * An observable that changes whenever the status of the authentication changes.
   */
  authStatus: Subject<AuthStatusResponse> = new Subject();

  constructor(private http: HttpClient, private router: Router) {
    this.accessToken = localStorage.getItem('accessToken');
    this.expiresAt = parseInt(localStorage.getItem('expiresAt'));
    this.refreshToken = localStorage.getItem('refreshToken');

    if (this.hasValidToken()) {
      this.$userAuthenticated.next(true);
    }
  }

  updateOnLocalStorage(attributeName: string, value: any) {
    if (value) {
      localStorage.setItem(attributeName, value.toString());
    } else {
      localStorage.removeItem(attributeName);
    }
  }

  hasValidToken() {
    const nowToMilis = new Date().getTime();
    return this.accessToken && this.expiresAt > nowToMilis;
  }

  async updateToken() {
    this.authStatus.next({ status: OAuthStatus.InProgress, data: {} });

    const oauthPayload = {
      grant_type: 'refreshToken',
      client_id: environment.clientId,
      client_secret: environment.clientSecret,
      refreshToken: this.refreshToken
    };
    const token: string = btoa(`${environment.clientId}:${environment.clientSecret}`);

    const oauthHeaders = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded',
      Authorization: `Basic ${token}`
    });

    const oauthOptions = {
      headers: oauthHeaders
    };

    this.http.post(`${environment.appUrl}/o/token`, RequestUtils.toURI(oauthPayload), oauthOptions)
      .subscribe(async response => {
        // Create a new date to calculate the expiration date of the token
        const expirationDate = new Date();
        // Gets the expiration time and deducts one minue to account for request delays
        const expiresIn = response['expires_in'] - 60;
        // Discount 2 minutes to account for request delays
        expirationDate.setSeconds(expirationDate.getSeconds() + expiresIn);
        // Set items on localStorage
        this.accessToken = response['access_token'];
        this.expiresAt = expirationDate.getTime();
        this.refreshToken = response['refresh_token'];
        this.$userAuthenticated.next(true);
      });
  }

  /**
   * Performs the request to fetch the user token.
   *
   * @param code the code produced by OAuth's /authorize endpoint.
   */
  async getToken(code: string) {
    // Changes the current state of the authentication procedure to in progress
    this.authStatus.next({ status: OAuthStatus.InProgress, data: {} });

    // The x-www-form content
    const oauthPayload = {
      grant_type: 'authorization_code',
      code: code,
      redirect_uri: this.redirectUri
    };
    const token: string = btoa(`${environment.clientId}:${environment.clientSecret}`);

    // The request headers
    const oauthHeaders = new HttpHeaders({
      'Content-Type': 'application/x-www-form-urlencoded',
      Authorization: `Basic ${token}`
    });

    // The object containing request options to reduce clutter on function call
    const oauthOptions = {
      headers: oauthHeaders
    };

    // Convert the payload to an URI-encoded string and performs the request
    await this.http
      .post(`${environment.appUrl}/o/token`, RequestUtils.toURI(oauthPayload), oauthOptions)
      .subscribe(async response => {
        // Create a new date to calculate the expiration date of the token
        const expirationDate = new Date();
        // Gets the expiration time and deducts one minue to account for request delays
        const expiresIn = response['expires_in'] - 60;
        // Discount 2 minutes to account for request delays
        expirationDate.setSeconds(expirationDate.getSeconds() + expiresIn);

        // Set items on localStorage
        this.accessToken = response['access_token'];
        this.expiresAt = expirationDate.getTime();
        this.refreshToken = response['refresh_token'];
        this.$userAuthenticated.next(true);

        this.getUserInfo();
      });
  }

  /**
   * Finishes the current user session.
   */
  logoff() {
    // Remove the local storage entries related to user session
    this.accessToken = null;
    this.expiresAt = null;
    this.refreshToken = null;
    this.loggedUser = {} as UserInfo;

    // Redirect the user to the logout page
    document.location.href = `${environment.pocoWeb}/accounts/login/`;
  }

  async getUserInfo() {
    const token = this.accessToken ? this.accessToken : 'FAKE_TOKEN';

    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    });

    const options = {
      headers: headers
    };

    await this.http
      .get(`${environment.appUrl}/o/userinfo`, options)
      .subscribe(async userInfo => {
        this.loggedUser = userInfo as UserInfo;

        this.authStatus.next({
          status: OAuthStatus.Success,
          data: this.loggedUser
        });
      });
  }
}

/**
 * Enum which defines the current status of the authentication.
 */
export enum OAuthStatus {
  Failed = -1,
  InProgress = 0,
  Success = 1
}
