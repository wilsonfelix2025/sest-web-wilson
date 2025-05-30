import { Subscription } from 'rxjs';
import { Component, OnInit, OnDestroy, AfterViewInit } from '@angular/core';
import { MediaChange, MediaObserver } from '@angular/flex-layout';
import { SynchronizeChartsService } from '@services/synchronize-charts.service';

@Component({
  selector: 'sest-editor-page',
  templateUrl: './editor.page.html',
  styleUrls: ['./editor.page.scss']
})
export class EditorPageComponent implements OnInit, OnDestroy, AfterViewInit {

  rtoConnect: boolean;

  event: Event;

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
  openFiles: any;

  constructor(
    private media: MediaObserver,
    public sync: SynchronizeChartsService
  ) {}

  onChange(event: Event) {
    this.event = event;
  }

  ngOnInit(): void {
    // Get initial state of the sidenav.
    this.calculateSidenavStatus();

    // Subscribe to changes in screen size to change sidenav behavior.
    this.mediaSubscription = this.media
      .asObservable().subscribe((change: MediaChange[]) => this.calculateSidenavStatus());
  }

  ngAfterViewInit() {
    this.justMargin();
  }

  ngOnDestroy(): void {
    this.mediaSubscription.unsubscribe();
  }

  justMargin() {
    if (this.leftSidenavOpen === !true) {
      const el: any = [document.getElementsByTagName('mat-sidenav-content')];
      el[0][0].style.cssText = 'margin-left:60px !important';
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
