import { Injectable } from '@angular/core';
import {
  Router,
  CanActivate,
  ActivatedRouteSnapshot,
  RouterStateSnapshot
} from '@angular/router';
import { environment } from '../../environments/environment';
import { OAuthTokenService } from '../services/oauth.service';
import { OAuthService } from 'angular-oauth2-oidc';
​
@Injectable()
export class AuthGuard implements CanActivate {
  constructor(
    private oauthTokenService: OAuthTokenService,
    private oauthService: OAuthService,
    private router: Router) {
  }
​
  /**
   * Determines whether or not the routes of the app are accessible.
   *
   * @param {ActivatedRouteSnapshot} route the route being accessed (currently unused).
   * @param {RouterStateSnapshot} state the state of the application (currently unused).
   * @returns {boolean} whether or not the route is accessible.
   * @memberof AuthGuard
   */
  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): boolean {
    // Bypass authentication in local development
    //if (!environment.production && !environment.staging) {
      return true;
    //}
​
    // If the OAuth service identifies valid credentials proceed
    if (this.oauthTokenService.hasValidToken()) {
      return true;
    } else if (this.oauthTokenService.refreshToken) {
      this.oauthTokenService.updateToken();
    }
​
    // Otherwise go to the login page
    this.oauthService.initImplicitFlow();
​
    // Authentication failed
    return false;
  }
}