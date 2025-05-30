import { AuthGuard } from './auth.guard';
import { OAuthHandlerComponent } from './oauth-handler/oauth-handler.component';
import { NgModule } from '@angular/core';

@NgModule({
  declarations: [
      OAuthHandlerComponent
  ],
  providers: [
      AuthGuard
  ]
})
export class AuthModule { }
