import { Injectable } from '@angular/core';
import { Subject, Subscription } from 'rxjs';
import { CursorInfo } from '@utils/interfaces';
import { ArrayUtils } from '@utils/array';
import * as Highcharts from 'highcharts';
import { MathUtils } from '@utils/math';
import * as HighchartsUtils from '@utils/highcharts';

@Injectable({
  providedIn: 'root'
})

export class SynchronizeChartsService {

  globalStart: number;
  globalEnd: number;

  actualZoomStart: number;
  actualZoomEnd: number;

  zoomObservable: Subject<{ start: number, end: number }> = new Subject();

  /**
   * Observable to watch when zoom occurred.
   */
  $syncZoomObservable: Subscription;

  /**
   * Observable to watch when cursor position changes on top of chart track.
   */
  chartPositionObservable: Subject<CursorInfo> = new Subject();

  constructor() {
    this.$syncZoomObservable = HighchartsUtils.syncZoomObservable.subscribe(zoom => {
      if (zoom.start === undefined || zoom.end === undefined) {
        this.resetZoom();
      } else {
        this.zoom(zoom.start, zoom.end);
      }
    });
  }

  /**
   * Sets the default value of maximum and minimum points for all charts.
   *
   * @param {number} start the depth of the shallower point.
   * @param {number} end the depth of the deepest point.
   */
  setsGlobalDepth(start: number, end: number) {
    this.globalStart = start;
    this.globalEnd = end + (end - start) * HighchartsUtils.chartMarginY;

    const s: any = HighchartsUtils.stratigraphyOptions;
    s.yAxis.min = this.globalStart;
    s.yAxis.max = this.globalEnd;

    const t: any = HighchartsUtils.trajectoryOptions;
    // t.series[0].pointStart = this.globalStart;
    t.xAxis.min = this.globalStart;
    t.xAxis.max = this.globalEnd;

    const l: any = HighchartsUtils.lithologyOptions;
    l.yAxis.min = this.globalStart;
    l.yAxis.max = this.globalEnd;

    const g: any = HighchartsUtils.graphicCanvasOptions;
    g.xAxis.min = this.globalStart;
    g.xAxis.max = this.globalEnd;

    this.zoom(this.globalStart, this.globalEnd);
  }

  /**
   * Zooms back to the default value, which shows the plot from minimum to maximum point.
   */
  resetZoom() {
    this.zoom(this.globalStart, this.globalEnd);
  }

  /**
   * Sync the zoom of all charts in the screen.
   */
  syncZoom() {
    this.zoom(this.actualZoomStart, this.actualZoomEnd);
    HighchartsUtils.reflowCharts();
  }

  /**
   * Synchronize the range of all charts on screen when the user zooms in or out.
   *
   * @param {number} min the beginning of the zoom.
   * @param {number} max the end of the zoom.
   */
  zoom(min: number, max: number) {
    // Save the actual zoom
    this.actualZoomStart = min;
    this.actualZoomEnd = max;

    Highcharts.charts.forEach(chart => {
      // Verifica se o grafico esta definido
      if (chart) {
        if (chart.title['textStr'] === 'Estratigrafia' || chart.title['textStr'] === 'Litologia') {
          // Check if the chart has the required function
          if (chart.yAxis[0].setExtremes && (chart.yAxis[0].min !== this.actualZoomStart || chart.yAxis[0].max !== this.actualZoomEnd)) {
            // Update the viewing range of the chart
            chart.yAxis[0].setExtremes(this.actualZoomStart, this.actualZoomEnd, false, false, {
              trigger: 'syncExtremes'
            });
          }
          // If this is a stratigraphy chart
          if (chart.title['textStr'] === 'Estratigrafia') {
            // Update the visibility of the labels in it
            HighchartsUtils.toggleStratLabels(chart);
          }

        } else {
          // Check if the chart has the required function
          if (chart.xAxis[0].setExtremes && (chart.xAxis[0].min !== this.actualZoomStart || chart.xAxis[0].max !== this.actualZoomEnd)) {
            // Update the viewing range of the chart
            chart.xAxis[0].setExtremes(this.actualZoomStart, this.actualZoomEnd, false, false, {
              trigger: 'syncExtremes'
            });
          }
        }
        // Redraw the chart with duration 0 to prevent animations
        chart.redraw({ duration: 0 });
      }
    });

    this.zoomObservable.next({ start: this.actualZoomStart, end: this.actualZoomEnd });
  }

  /**
   * Alias function so that both service functions can be called from a
   * component's template.
   *
   * @param {*} e an object containing event information.
   */
  synchronize(e: any) {
    this.synchronizeCrosslines(e);
    this.showPointerPosition(e);
  }

  /**
   * Draws a red line across all charts at the same Y coordinate.
   *
   * @param {PointerEventObject} e an object containing event information.
   */
  synchronizeCrosslines(e: Highcharts.PointerEventObject) {
    // Get the list of charts
    const charts = Highcharts.charts;
    // Get the chart currently being hovered
    const sourceChart = charts[(Highcharts as any).hoverChartIndex];

    for (const chart of charts) {
      // Check if the chart is defined
      if (chart && sourceChart) {
        // Get the chart coordinates from the event
        const x = e.chartX, y = e.chartY;
        // Assemble the path of the SVG line; type 'any' is required to ensure Highcharts compatibility
        const path = [
          'M', chart.plotLeft, y,
          'L', chart.plotLeft + chart.plotWidth, y,
        ] as any;

        // Checks if the cursor is in the plottable area of the chart, and not on the scales or caption
        const isOnPlottableArea =
          x > sourceChart.plotLeft && x < sourceChart.plotLeft + sourceChart.plotWidth &&
          y > sourceChart.plotTop && y < sourceChart.plotTop + sourceChart.plotHeight;

        // If the 'crossLines' attribute has already been defined
        if (chart['crossLines']) {
          // Check if the cursor is in the plottable area of the chart
          if (isOnPlottableArea) {
            // Draw the path defind above
            chart['crossLines'].attr({
              d: path
            });
          } else {
            // When the user move outside the plottable area, remove the line
            chart['crossLines'].attr('d', '');
          }
        } else {
          // If the attribute does not exist, create it and draw the line
          chart['crossLines'] = chart.renderer.path(path).attr({
            'stroke-width': 1,
            'stroke': 'red',
            'zIndex': 4
          }).add();
        }
      }
    }
  }

  /**
   * Updates the current position of the cursor on the chart.
   *
   * @param {PointerEvent} e the event object with cursor information.
   */
  showPointerPosition(e: PointerEvent) {
    // Get the list of charts
    const charts = Highcharts.charts;
    // Get the chart currently being hovered
    const sourceChart = charts[(Highcharts as any).hoverChartIndex];

    if (sourceChart) {
      // Convert event into Highcharts object
      const event = sourceChart.pointer.normalize(e);
      // Fetch the y position of the pointer
      let y;

      /**
       * Os eixos dos gráficos de linha (que não são litologia nem estratigrafia) são invertidos.
       * Isso foi feito por conta do highcharts pedir que os pontos do eixo X gráfico estejam ordenados,
       * e como os pontos já vem ordenados mas pela profundidade (que corresponde ao eixo Y) e afim de
       * evitar um maior processamento reordenando os pontos, os eixos foram invertidos.
       */
      if (sourceChart.title['textStr'] === 'Estratigrafia' || sourceChart.title['textStr'] === 'Litologia') {
        y = sourceChart.yAxis[0].toValue(event.chartY);
      } else {
        y = sourceChart.xAxis[0].toValue(event.chartY);
      }

      const graphs: {
        x: number,
        title: string,
      }[] = [];
      /**
       * This check is necessary because, in order to ensure proper scales and
       * range on the chart, a blank series must be initialized, so even empty
       * charts have one series.
       *
       * We need to check if there's a second series, to make sure at least one
       * series was added to the chart.
       */
      if (sourceChart.title['textStr'] !== 'Estratigrafia' && sourceChart.title['textStr'] !== 'Litologia') {
        let series = sourceChart.series;
        if (sourceChart.title['textStr'] === 'Trajetoria') {
          series = series.filter(el => el.visible && el.name === 'Trajetória');
        }
        series.forEach(el => {
          // Fetch the coordinates of every point
          const xData = el['yData'];
          const yData = el['xData'];

          // Use binary search to efficiently find which points will be used to interpolate
          let index = ArrayUtils.binarySearch(yData, y);

          // Ensure that the point found is always smaller than the current Y
          if (y < yData[index]) {
            index -= 1;
          }

          let x;
          // If there is a next point
          if (index !== yData.length - 1 && xData[index] !== undefined && yData[index] !== undefined) {
            // Interpolate using the current point and the next
            x = MathUtils.linearInterpolate(xData[index], yData[index], xData[index + 1], yData[index + 1], y);
            x = Math.round(x * 1000) / 1000;
          }
          if (x !== undefined) {
            if (sourceChart.title['textStr'] === 'Trajetoria') {
              if (el.points[index] !== undefined && el.points[index]['dado'] !== undefined) {
                if (index !== el.points.length - 1 && el.points[index] !== undefined && el.points[index] !== undefined) {
                  // Interpolate using the current point and the next
                  x = MathUtils.linearInterpolate(el.points[index]['dado'].ponto, yData[index], el.points[index + 1]['dado'].ponto, yData[index + 1], y);
                  x = Math.round(x);

                  graphs.push({ x: x, title: el['userOptions'].deep === 'PM' ? 'PV' : 'PM' });
                }
                graphs.push({ x: el.points[index]['dado'].azimute, title: 'Azimute' });
                graphs.push({ x: el.points[index]['dado'].inclinacao, title: 'Inclinação' });
              }
              graphs.push({ x: sourceChart.axes[0]['dataMax'], title: `${el['userOptions'].deep} max` });
            } else {
              graphs.push({ x: x, title: el.name });
            }
          }
        });
      }

      // Check if y is non-negative to prevent glitches when the cursor is very near the edge
      if (y >= 0) {
        // Assemble an object and broadcast it
        this.chartPositionObservable.next({
          y: Math.round(y),
          graphs: graphs,
          unidade: sourceChart.options.subtitle.text,
          isBaseChart: sourceChart.options.chart.className !== 'plotArea',
        });
      }
    }
  }

  /**
   * Callback for when the cursor moves out of the chart.
   *
   * Currently, it's only used to update the content of the status bar.
   */
  onMouseOut(e) {
    this.chartPositionObservable.next({
      y: 0,
      graphs: [],
      isBaseChart: false,
    });
  }

  /**
   * Removes the crossline when the cursor moves outside the chart.
   */
  onMouseOutCrossline(e) {
    // Get the list of charts on screen
    const charts = Highcharts.charts;

    for (const chart of charts) {
      // The list of charts gets weird when switching tabs, so the check below is required
      if (chart && chart['crossLines']) {
        // Reset the crosslines of the chart
        chart['crossLines'].attr('d', '');
      }
    }
  }
}
