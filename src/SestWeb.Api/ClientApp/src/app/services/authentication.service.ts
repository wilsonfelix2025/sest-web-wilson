import { Injectable } from '@angular/core';
import { OAuthService, JwksValidationHandler, OAuthEvent } from 'angular-oauth2-oidc';
import { Router } from '@angular/router';
import { authConfig } from '../auth.config';

@Injectable()
export class AuthenticationService {

  constructor(
    private router: Router,
    private oauthService: OAuthService
  ) {}

  bootstrap() {
    // Setup the OAuth service with the provided configuration
    this.oauthService.configure(authConfig);
    // Set the local storage as the storage where token info will be saved
    this.oauthService.setStorage(localStorage);
    // Creates a new validation handler for the token
    this.oauthService.tokenValidationHandler = new JwksValidationHandler();

    // Start subscription to listen to authentication events
    // P.S.: I'm not entirely sure this block of code is necessary
    const subscription = this.oauthService.events.subscribe((event: OAuthEvent) => {
      // If the authentication has ended
      if (event.type === 'token_received') {
        // Finish subscription to free up memory
        subscription.unsubscribe();
        // Go to the entry point of the application
        return this.router.navigate(['pages/start']);
      }
    });

    // I don't know what this does, but it's probably better to leave it here
    this.oauthService.tryLogin().then(() => {
      this.oauthService.setupAutomaticSilentRefresh();
    });
  }
}
