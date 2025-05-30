import { AuthConfig } from 'angular-oauth2-oidc';
import { environment } from 'environments/environment';

export const authConfig: AuthConfig = {
  clientId: environment.clientId,
  scope: 'openid',
  responseType: 'code',
  oidc: false,
  loginUrl: `${environment.pocoWeb}/o/authorize`
};
