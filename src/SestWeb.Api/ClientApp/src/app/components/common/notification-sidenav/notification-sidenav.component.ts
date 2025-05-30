import { Component, OnInit, Input } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { MatSidenav } from '@angular/material/sidenav';

import { switchMap } from 'rxjs/operators';

import { Observable } from 'rxjs';

@Component({
  selector: 'sest-notification-sidenav',
  templateUrl: './notification-sidenav.component.html',
  styleUrls: ['./notification-sidenav.component.scss'],
})
export class NotificationSidenavComponent implements OnInit {
  /**
   * Import material sidenav so we can access open close functions.
   */
  @Input() sidenav: MatSidenav;

  /**
   * Stores todays date for top title.
   */
  todaysDate: Date = new Date();

  constructor(private http: HttpClientModule) { }

  ngOnInit(): void {
  }

}
