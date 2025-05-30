import { Component, Input, OnDestroy } from '@angular/core';
import { SynchronizeChartsService } from '@services/synchronize-charts.service';
import { Subscription } from 'rxjs';
import { CursorInfo } from '@utils/interfaces';
import { StateService } from '@services/dataset/state/state.service';

@Component({
  selector: 'sest-statusbar',
  templateUrl: './statusbar.component.html',
  styleUrls: ['./statusbar.component.scss'],
})
export class StatusbarComponent implements OnDestroy {

  /**
   * Função que recebe um evento de STATUS da conexão RTO
   */
  @Input()
  set event(event: Event) {
    if (event) {
      this.toggle();
    }
  }
  /**
   * conectado ou desconectado.
   */
  hidden = false;

  /**
   * Object containing updated information about the cursor position.
   */
  cursorInfo: CursorInfo;
  /**
   * Subscription which watches changes in cursor position when
   * it's on top of a chart.
   */
  $chartPositionObservable: Subscription;

  $zoomObservable: Subscription;

  actualMin: number;
  actualMax: number;

  constructor(
    public sync: SynchronizeChartsService,
    public stateService: StateService
  ) {
    // Whenever the cursor position changes on top of the chart
    this.$chartPositionObservable = this.sync.chartPositionObservable.subscribe(
      (cursorInfo) => {
        // Update the component's attributes with information about cursor position
        this.cursorInfo = cursorInfo;
      }
    );
    this.$zoomObservable = this.sync.zoomObservable.subscribe((zoom) => {
      // Update the component's attributes with information about zoom info
      this.actualMin = zoom.start;
      this.actualMax = zoom.end;
    });
  }

  resetZoom() {
    this.sync.resetZoom();
  }

  setZoom() {
    this.sync.zoom(this.actualMin, this.actualMax);
  }

  ngOnDestroy() {
    // Destroy subscriptions upon component destruction
    this.$chartPositionObservable.unsubscribe();
    this.$zoomObservable.unsubscribe();
  }

  /**
   * Função toggle para exibir o status da conexão rto online/offline.
   */
  toggle() {
    this.hidden = !this.hidden;
  }
}
