import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OAuthStatus, OAuthTokenService } from '@services/oauth.service';
import { OAuthService } from 'angular-oauth2-oidc';
​
@Component({
  selector: 'app-oauth-handler',
  templateUrl: './oauth-handler.component.html',
  styleUrls: ['./oauth-handler.component.scss']
})
export class OAuthHandlerComponent {
  /**
   * Component responsible for processing OIDC completion.
   */
  constructor(
    private route: ActivatedRoute,
    private oauthToken: OAuthTokenService,
    private oauth: OAuthService,
    private router: Router
  ) {
    // Fetch params from the route URL
    this.route.queryParams.subscribe(params => {
      // Subscribe to the authentication status observable
      this.oauthToken.authStatus.subscribe(value => {
        // When authentication succeeds
        if (value.status === OAuthStatus.Success) {
          // Redirect user to the root route
          this.router.navigate(['explorer']);
        }
      });
​
      // If the URL has param 'code'
      if (params['code']) {
        // Fetch the token with the code received via the URL
        this.oauthToken.getToken(params['code']);
      } else {
        if (this.oauthToken.refreshToken) {
          this.oauthToken.updateToken();
        } else {
          this.oauth.initImplicitFlow();
        }
      }
    });
  }
}