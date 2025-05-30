import { Subscription } from 'rxjs';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { MediaChange, MediaObserver } from '@angular/flex-layout';

@Component({
  selector: 'sest-explorer-page',
  templateUrl: './explorer.page.html',
  styleUrls: ['./explorer.page.scss']
})
export class ExplorerPageComponent implements OnInit, OnDestroy {
  /**
   * Stores if left sidenav is open.
   */
  leftSidenavOpen: boolean;

  /**
   * Stores left sidenav display mode.
   */
  leftSidenavMode: string;

  /**
   * Stores media observable subscription.
   */
  mediaSubscription: Subscription;

  constructor(
    private media: MediaObserver
  ) {}

  ngOnInit(): void {
    // Get initial state of the sidenav.
    this.calculateSidenavStatus();

    // Subscribe to changes in screen size to change sidenav behavior.
    this.mediaSubscription = this.media
      .asObservable().subscribe((change: MediaChange[]) => this.calculateSidenavStatus());
  }

  ngOnDestroy(): void {
    if (this.mediaSubscription) {
      this.mediaSubscription.unsubscribe();
    }
  }

  calculateSidenavStatus(): void {
    const isMobile = this.media.isActive('lt-md');
    // Close sidenav on mobile.
    this.leftSidenavOpen = !isMobile;
    // Make sidenav open over content on mobile.
    this.leftSidenavMode = isMobile ? 'over' : 'side';
  }
}
