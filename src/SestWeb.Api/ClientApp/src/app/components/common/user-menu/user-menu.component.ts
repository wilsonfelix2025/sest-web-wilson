import { Component, OnInit } from "@angular/core";
import { OAuthTokenService } from "@services/oauth.service";
import { UserInfo } from "../../../repositories/models/user-info";

@Component({
  selector: "sest-user-menu",
  templateUrl: "./user-menu.component.html",
  styleUrls: ["./user-menu.component.scss"]
})
export class UserMenuComponent implements OnInit {
  public userInfo: UserInfo = {} as UserInfo;
  public userLabel: string;

  constructor(private oauth: OAuthTokenService) {}

  ngOnInit(): void {
    this.userInfo = this.oauth.loggedUser && !!Object.keys(this.oauth.loggedUser).length
      ? (this.oauth.loggedUser as UserInfo)
      : ({ preferred_username: "" } as UserInfo);
    this.userLabel = this.userInfo.given_name ? `${this.userInfo.given_name} ${this.userInfo.family_name}` : this.userInfo.preferred_username;
  }

  /**
   * Invokes the OAuthTokenService logoff function.
   */
  logoff() {
    this.oauth.logoff();
  }
}
