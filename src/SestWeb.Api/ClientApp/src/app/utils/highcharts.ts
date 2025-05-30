import * as Highcharts from 'highcharts';
import Boost from 'highcharts/modules/boost';
import noData from 'highcharts/modules/no-data-to-display';
import Draggable from 'highcharts/modules/draggable-points';
import HC_exporting from 'highcharts/modules/exporting';
import HC_exportData from 'highcharts/modules/export-data';
import offlineExporting from 'highcharts/modules/offline-exporting';
import { environment } from '../../environments/environment';
import { Subject } from 'rxjs';
import { UNSET } from './vazio';
import { testsvg } from './dist/svg';

HC_exporting(Highcharts);
HC_exportData(Highcharts);
offlineExporting(Highcharts);
Boost(Highcharts);
noData(Highcharts);
Draggable(Highcharts);

export function exportChart(chart: Highcharts.Chart) {
  // Get Actual SVG of a chart
  const svgString = chart.getSVG({
    chart: {
      width: 220,
      height: 592,
    },
  });
  // console.log('svg', { svgString });

  // Use DOMParser to parse new svg element from svgString
  const parser = new DOMParser();
  const svgElem = parser.parseFromString(svgString, 'image/svg+xml').documentElement;

  // console.log('base64', { b64 });
  return testsvg.toDataURL(svgElem);
}
/**
 * Prevents the Highcharts.charts array from growing indefinitely.
 *
 * Because of how Highcharts works, whenever a chart is destroyed, the index
 * it was at in the Highcharts.charts array is kept, but set to undefined.
 * As the user navigates through the tabs, potentially hundreds or thousands
 * of charts will be created and destroyed, leaving the array with lots of
 * undefined positions.
 *
 * To prevent this and to further optimize functions and methods that iterate
 * through the array of charts, we must clean the array, removing undefined
 * entries.
 */
export function getNonEmptyCharts() {
  /**
   * We're gonna iterate backwards through the array because we'll use
   * Array.splice. Splicing causes index shifts when iterating forwards,
   * so, to prevent esoteric increments on the counter, we'll iterate
   * from end to start.
   */
  return Highcharts.charts.filter((chart) => chart !== undefined);
}

/**
 * Enables or disables the label of a particular chart series,
 * depending on the size of the label and the height of the series.
 */
export function toggleStratLabels(chart: Highcharts.Chart) {
  // For every point of the series
  return chart.series.forEach((element) => {
    // Get the height of the series' block, in pixels
    const columnSize = element.data[0]['shapeArgs'].height;
    // Get the height of the label inside it, in pixels
    const textSize = element['dataLabelsGroup'].element.childNodes[1].getBoundingClientRect().height;

    // If the size of the label is greater than the size of the column
    if (textSize > columnSize) {
      // Hide the label
      element['dataLabelsGroup'].element.style.visibility = 'hidden';
    } else {
      // Otherwise show it
      element['dataLabelsGroup'].element.style.visibility = 'visible';
    }
  });
}

/**
 * Wraps the toggleStratLabels function so it can be used as a callback on the
 * chart.redraw event on stratigraphyOptions.
 *
 * @param event not used, added for type compatibility only.
 */
function toggleStratLabelsWrapper(event) {
  toggleStratLabels(this);
}

/**
 * Adjusts the stratigraphy chart, showing and hiding labels based on
 * the size of the sections.
 */
export function estratAdjust() {
  // For every chart currently on screen
  Highcharts.charts.forEach((chart) => {
    // Check if the chart is defined and if is a stratigraphy chart
    if (chart && chart.title['textStr'] === 'Estratigrafia') {
      // Update the visibility of the labels in it
      toggleStratLabels(chart);
      // Redraw the chart with duration 0 to prevent animations
      chart.redraw({ duration: 0 });
    }
  });
}

/**
 * Notify sync service that zoom event has occurred
 */
export function syncExtremes(e) {
  // Checks whether the event was called by the sync service to prevent infinite loops
  if (e.trigger !== 'syncExtremes') {
    syncZoomObservable.next({ start: e.min, end: e.max });
  }
}
export const syncZoomObservable: Subject<{ start: number; end: number }> = new Subject();

/**
 * Notify edit trend service that a click has occurred
 */
export function clickChart(e) {
  clickChartObservable.next({ chart: this, event: e });
}
export const clickChartObservable: Subject<{ chart; event }> = new Subject();

/**
 * Notify edit trend service that a drag is started
 */
export function startDragPoint(e) {
  startDragPointObservable.next({ point: this, event: e });
}
export const startDragPointObservable: Subject<{ point; event }> = new Subject();

/**
 * Notify edit trend service that a drag is occurring
 */
export function dragPoint(e) {
  dragPointObservable.next({ point: this, event: e });
}
export const dragPointObservable: Subject<{ point; event }> = new Subject();

/**
 * Notify edit trend service that a drop has occurred
 */
export function dropPoint(e) {
  dropPointObservable.next({ point: this, event: e });
}
export const dropPointObservable: Subject<{ point; event }> = new Subject();

/**
 * These variables must stay as globals so syncLitologia doesn't fall into an
 * infinite loop, as it calls the redraw() function, which will then trigger the
 * render event of the lithology chart, which will then trigger the function once
 * again, endlessly.
 *
 * There must be a better solution for this, but since we don't have much time
 * to do this right now, I'm marking it with a TODO.
 */
let redrawEnabled = true,
  ctr = 0;

/**
 * Inserts the chart pattern into the series' blocks.
 *
 * @param e an object which has the chart being synced.
 */
export function syncLitologia(e) {
  // Fetch the chart from the event object
  const chart = e.target;
  // Define the scale which will be used throughout the code (default is 3)
  const BASE_LITHOLOGY_SCALE = 3;
  // If the chart is defined
  if (chart) {
    // If the global control is allowing redraw
    if (redrawEnabled) {
      // Disable it so the inner calls to redraw() don't create an infinite loop
      redrawEnabled = false;
      // Get the chart renderer
      const renderer = chart.renderer;
      // Iterate through the chart series
      for (let i = 0; i < chart.series.length; i++) {
        // For each one of the points
        chart.series[i].points.forEach(function (p) {
          // p.shapeArgs.height = chart.series[i].dataMax - chart.series[i].dataMin;
          // Get the name of the corresponding lithology from the chart options
          const description = chart.series[i].userOptions.className;
          // Calculate the scale of the image based on the section width and height
          const widthRatio = p.shapeArgs.width / BASE_LITHOLOGY_SCALE / p.shapeArgs.height;
          // Define a unique ID for this pattern
          const id = `pattern-${p.index}-${ctr}`;

          // Create a new pattern object and render it on the chart
          const pattern = renderer.createElement('pattern').add(renderer.defs).attr({
            width: 1,
            height: widthRatio,
            id: id,
            patternContentUnits: 'objectBoundingBox',
          });

          let imageUrl;
          // Create an alias for the image URL
          if (environment.production || environment.staging) {
            imageUrl = `${environment.appUrl}/assets/images/litologias/${description}.png`; // producao
          } else {
            imageUrl = `${environment.appUrl}/sestweb/assets/images/litologias/${description}.png`; // localhost
          }

          // Generate the pattern based on the lithology scale
          for (let j = 0; j < BASE_LITHOLOGY_SCALE; j++) {
            renderer
              .image(imageUrl, (1 / BASE_LITHOLOGY_SCALE) * j, 0, 1 / BASE_LITHOLOGY_SCALE, widthRatio)
              .attr({})
              .add(pattern);
          }

          // Update the series passing the newly created pattern as its 'color'
          p.update({ color: `url(#${id})` }, false);
        });

        // Increment the control variable so IDs are always unique
        ctr++;
      }
      // Redraw the chart
      chart.redraw();
      // Enable redraw again, AFTER redraw has been called, to avoid infinite loops
      redrawEnabled = true;
    }
  }
}

/**
 * Postprocesses the series received, setting their background color and label.
 *
 * @param {*} series an array containing series information.
 */
export function postProcessStratigraphySeries(series: any[]): Highcharts.SeriesColumnrangeOptions[] {
  // The two colors which will be used to alternatingly paint the stratigraphy chart
  const colors = ['#e0e0e0', '#eeeeee'];
  // Initialize the return array
  const processedSeries: Highcharts.SeriesColumnrangeOptions[] = [];

  const entry = function (item, i, deep) {
    return {
      name: item.name,
      color: colors[i % 2], // This makes sure the colors are alternating
      data: deep ? item.pv : item.pm,
      type: 'columnrange',
      dataLabels: {
        enabled: true,
        color: 'auto',
        rotation: -90, // Rotated 90 degrees counterclockwise from the horizontal
        formatter: () => item.name.toUpperCase(), // Make the label uppercase
      },
      deep: deep,
    } as Highcharts.SeriesColumnrangeOptions;
  };

  // For every item in the series
  series.forEach((item, i) => {
    // Add the processed entry to the series
    processedSeries.push(entry(item, i, true));
    processedSeries.push(entry(item, i, false));
  });

  return processedSeries;
}

/**
 * Reflow every chart currently on screen.
 */
export function reflowCharts() {
  // For every chart on screen
  getNonEmptyCharts().forEach((chart) => {
    // Check if it is defined and reflow it
    if (chart) {
      chart.reflow();
    }
  });
}

export const chartMarginX = 0.1;
export const chartMarginY = 0.1;

export const graphicCanvasOptions: Highcharts.Options = {
  legend: {
    enabled: false,
  },
  mapNavigation: {
    enabled: false,
  },
  chart: {
    events: {
      click: clickChart,
    },
    type: 'line',
    className: 'baseArea',
    inverted: true,
    zoomType: 'x',
    marginTop: 30,
    spacingTop: 0,
    marginBottom: 0,
    spacingBottom: 0,
    plotBorderWidth: 1,
    panning: true,
    panKey: 'shift',
    resetZoomButton: {
      theme: {
        display: 'none',
      },
    },
  },
  title: {
    text: '',
    style: { display: 'none' },
  },
  tooltip: {
    enabled: true,
    headerFormat: null,
    useHTML: true,
    formatter: function () {
      if (UNSET(this.point['description'])) {
        return false;
      }
      return `<span style='color:#666;font-size:9px; text-align:center;'> ${this.point['description']} </span>`;
    },
    style: {
      zIndex: '9999',
    },
  },
  yAxis: {
    startOnTick: false,
    endOnTick: false,
    alignTicks: false,
    tickInterval: null,
    minorTickInterval: null,
    opposite: true,
    type: 'linear',
    title: {
      text: '',
    },
    gridLineWidth: 1,
    lineWidth: 0,
    tickWidth: 1,
  },
  xAxis: {
    lineWidth: 0,
    tickWidth: 0,
    reversed: true,
    type: 'linear',
    title: {
      text: null,
    },
    labels: {
      enabled: false,
    },
    events: {
      setExtremes: syncExtremes,
    },
    startOnTick: false,
    endOnTick: false,
    gridLineWidth: 1,
  },
  credits: {
    enabled: false, // get rid of highcharts credit
  },
  exporting: {
    fallbackToExportServer: false,
    enabled: false,
    allowHTML: true,
    chartOptions: {
      plotOptions: {
        series: {
          dataLabels: {
            enabled: true,
          },
          marker: {
            radius: 5,
            lineWidth: 3,
            fillColor: null,
            lineColor: null,
          },
        },
      },
    },
  },
  plotOptions: {
    series: {
      boostThreshold: 0,
      animation: {
        duration: 0,
      },
      states: {
        inactive: {
          opacity: 1,
        },
        hover: {
          enabled: false,
        },
      },
      point: {
        events: {
          dragStart: startDragPoint,
          drag: dragPoint,
          drop: dropPoint,
        },
      },
    },
  },
  boost: {
    seriesThreshold: 200,
  }
};

export const stratigraphyOptions: Highcharts.Options = {
  chart: {
    type: 'columnrange',
    className: 'baseArea',
    spacingTop: 35,
    spacingBottom: 0,
    spacingLeft: 0,
    spacingRight: 0,
    margin: 0,
    marginTop: 30,
    plotBorderWidth: 1,
    zoomType: 'y',
    style: {
      fontSize: '12px',
      fontFamily: `'Lucida' Grande, sans-serif;`,
      fontWeight: 'bold',
    },
    events: {
      redraw: toggleStratLabelsWrapper,
      load: estratAdjust,
    },
    resetZoomButton: {
      theme: {
        display: 'none',
      },
    },
  },
  navigator: {
    enabled: true,
  },
  mapNavigation: {
    enabled: true,
    enableMouseWheelZoom: true,
    mouseWheelSensitivity: 7,
    enableButtons: false,
  },
  rangeSelector: {
    selected: 1,
  },
  tooltip: {
    enabled: true,
    headerFormat: null,
    useHTML: true,
    formatter: function () {
      return `<span style='font-size:11px; text-align:center;'>
        <strong style='color:#666;font-weight:bold;font-size:11px;'>${this.series.name}</br>
        <strong style='color:#666;font-weight:normal;font-size:10px;'>Profund.:</br></strong>
        <strong style='color:#666;font-weight:normal;font-size:09px;'>${this.point['low']}
        </strong></span>`;
    },
    pointFormat: `<tr><td style='color: {series.color}, z-index: '7'>{series.name}: </td>
      <td style='text-align: right', z-index: '7'><b>{point.y} </b></td></tr>`,
    style: {
      zIndex: '9999',
    },
    valueDecimals: 1,
  },
  legend: {
    enabled: false,
  },
  exporting: {
    fallbackToExportServer: false,
    enabled: false,
    allowHTML: true,
    chartOptions: {
      plotOptions: {
        series: {
          dataLabels: {
            enabled: true,
          },
        },
      },
    },
  },
  credits: {
    enabled: false, // get rid of highcharts credit
  },
  title: {
    text: 'Estratigrafia',
    style: { display: 'none' },
  },
  xAxis: {
    type: 'linear',
    tickmarkPlacement: 'on',
    categories: [null],
    labels: {
      enabled: false,
    },
    reversed: true,
    lineWidth: 0,
    minorGridLineWidth: 0,
    lineColor: 'transparente',
    gridLineColor: 'transparente',
    gridLineWidth: 0,
    tickWidth: 0,
    opposite: true,
  },
  yAxis: {
    reversed: true,
    type: 'linear',
    tickInterval: null,
    labels: {
      enabled: false,
    },
    title: {
      text: null,
    },
    crosshair: true,
    events: {
      setExtremes: syncExtremes,
    },
    startOnTick: false,
    endOnTick: false,
  },
  plotOptions: {
    columnrange: {
      dataLabels: {
        enabled: true,
        inside: true,
        allowOverlap: false,
        align: 'center',
        verticalAlign: 'middle',
        format: '{series.name}',
      },
      grouping: false,
      pointPadding: 0,
      borderWidth: 0,
      groupPadding: 0,
      shadow: false,
    },
    series: {
      animation: {
        duration: 0,
      },
    },
  },
};

export const lithologyOptions: Highcharts.Options = {
  legend: {
    enabled: false,
  },
  mapNavigation: {
    enabled: true,
    enableMouseWheelZoom: true,
    mouseWheelSensitivity: 7,
    enableButtons: false,
  },
  chart: {
    type: 'columnrange',
    className: 'baseArea',
    spacingTop: 0,
    margin: 0,
    marginTop: 30,
    plotBorderWidth: 1,
    zoomType: 'y',
    events: {
      render: syncLitologia,
      load: syncExtremes,
    },
    resetZoomButton: {
      theme: {
        display: 'none',
      },
    },
  },
  title: {
    text: 'Litologia',
    style: { display: 'none' },
  },
  tooltip: {
    enabled: true,
    headerFormat: null,
    useHTML: true,
    formatter: function () {
      return `<span style='font-size:11px; text-align:center;'>
        <strong style='color:#666;font-weight:normal;font-size:09px;'>
        ${this.series.name}
        </strong></span>`;
      // + '</br>' + '<strong style='color:#666;font-weight:normal;font-size:10px;'>' + this.y + '</strong></p>';
    },
    pointFormat: `<tr><td style='color: {series.color}, z-index: '7'>{series.name}: </td>
    <td style='text-align: right', z-index: '7'><b>{point.y} </b></td></tr>`,
    style: {
      zIndex: '9999',
    },
    valueDecimals: 1,
  },
  xAxis: {
    tickmarkPlacement: 'on',
    categories: [null],
    labels: {
      enabled: false,
    },
    lineWidth: 0,
    gridLineWidth: 0,
    tickWidth: 0,
    opposite: true,
  },
  yAxis: {
    reversed: true,
    type: 'linear',
    labels: {
      enabled: false,
    },
    title: {
      text: null,
    },
    events: {
      setExtremes: syncExtremes,
    },
    startOnTick: false,
    endOnTick: false,
  },
  credits: {
    enabled: false, // get rid of highcharts credit
  },
  exporting: {
    fallbackToExportServer: false,
    enabled: false,
    allowHTML: true,
    chartOptions: {
      plotOptions: {
        series: {
          dataLabels: {
            enabled: true,
          },
        },
      },
    },
  },
  navigator: {
    enabled: true,
  },
  rangeSelector: {
    selected: 1,
  },
  plotOptions: {
    columnrange: {
      grouping: false,
      pointPadding: 0,
      borderWidth: 0,
      groupPadding: 0,
      cropThreshold: 140,
      shadow: false,
    },
    series: {
      animation: {
        duration: 0,
      },
      events: {
        afterAnimate: function () { },
      },
    },
  },
};

export const trajectoryOptions: Highcharts.Options = {
  series: [
    {
      type: 'line',
      color: '#187BC7',
    },
  ],
  legend: {
    enabled: false,
  },
  mapNavigation: {
    enabled: false,
    buttonOptions: {
      align: 'left',
      verticalAlign: 'bottom',
      x: -65,
    },
  },
  chart: {
    type: 'line',
    className: 'baseArea',
    zoomType: 'x',
    inverted: true,
    marginTop: 30,
    spacingTop: 0,
    marginBottom: 0,
    spacingBottom: 0,
    plotBorderWidth: 1,
    events: {
      load: syncExtremes,
    },
    panning: true,
    panKey: 'ctrl',
    resetZoomButton: {
      theme: {
        display: 'none',
      },
    },
  },
  title: {
    text: 'Trajetoria',
    style: { display: 'none' },
  },
  tooltip: {
    headerFormat: null,
  },
  yAxis: {
    opposite: true,
    type: 'linear',
    title: {
      text: '',
    },
    startOnTick: false,
    endOnTick: false,
    gridLineWidth: 1,
    lineWidth: 0,
    tickWidth: 1,
  },
  xAxis: {
    reversed: true,
    type: 'linear',
    title: {
      text: 'Profundidade(m)',
    },
    events: {
      setExtremes: syncExtremes,
    },
    startOnTick: false,
    endOnTick: false,
    gridLineWidth: 1,
    lineWidth: 0,
    tickWidth: 0,
  },
  credits: {
    enabled: false, // get rid of highcharts credit
  },
  exporting: {
    fallbackToExportServer: false,
    enabled: false,
    allowHTML: true,
    chartOptions: {
      plotOptions: {
        series: {
          dataLabels: {
            enabled: true,
          },
        },
      },
    },
  },
  plotOptions: {
    series: {
      animation: {
        duration: 0,
      },
      keys: ['x', 'y', 'marker', 'dado', 'dataLabels'],
      states: {
        inactive: {
          opacity: 1,
        },
        hover: {
          enabled: false,
        },
      },
    },
  },
};

const tensoes: Highcharts.Options = {
  legend: { enabled: false },
  credits: { enabled: false },
  exporting: { enabled: false },
  chart: {
    type: 'line',
    className: 'chart',
    width: 450,
    marginLeft: 70,
    spacingLeft: 70,
    marginTop: 15,
    spacingTop: 15,
    marginBottom: 40,
    spacingBottom: 40,
    plotBorderWidth: 1,
  },
  title: { style: { display: 'none' } },
  tooltip: { enabled: false },
  xAxis: {
    title: {
      text: 'Soterramento (m)',
    },
  },
  yAxis: {
    title: {
      text: 'LOT-PPORO (psi)',
    },
  },
  series: [],
};

export const ppOptions = JSON.parse(JSON.stringify(tensoes));

export const ldaOptions = JSON.parse(JSON.stringify(tensoes));
ldaOptions.yAxis['title'].text = 'LOT-Pressão LDA (psi)';

export const kOptions = JSON.parse(JSON.stringify(tensoes));
kOptions.xAxis['title'].text = 'σv-PPORO (psi)';

export const kaOptions = JSON.parse(JSON.stringify(tensoes));
kaOptions.chart.width = 260;
kaOptions.chart.inverted = true;
kaOptions.yAxis['title'].text = 'K0';
kaOptions.xAxis['title'].text = 'Profundidade Vertical (m)';

export function selectionChart(e) {
  selectionObservable.next({ point: this, event: e });
  return false;
}
export const selectionObservable: Subject<{ point; event }> = new Subject();

export const compositionCanvasOptions: Highcharts.Options = {
  legend: {
    enabled: false,
  },
  mapNavigation: {
    enabled: false,
  },
  chart: {
    events: {
      selection: selectionChart,
    },
    type: 'line',
    className: 'baseArea',
    inverted: true,
    zoomType: 'x',
    marginTop: 30,
    height: 500,
    spacingTop: 0,
    marginBottom: 5,
    spacingBottom: 0,
    plotBorderWidth: 1,
    resetZoomButton: {
      theme: {
        display: 'none',
      },
    },
  },
  title: {
    text: '',
    style: { display: 'none' },
  },
  tooltip: {
    enabled: false,
  },
  yAxis: {
    startOnTick: false,
    endOnTick: false,
    alignTicks: false,
    tickInterval: null,
    minorTickInterval: null,
    opposite: true,
    type: 'linear',
    title: {
      text: '',
    },
    gridLineWidth: 1,
    lineWidth: 0,
    tickWidth: 1,
  },
  xAxis: {
    lineWidth: 0,
    tickWidth: 1,
    reversed: true,
    type: 'linear',
    title: {
      text: null,
    },
    startOnTick: false,
    endOnTick: false,
    gridLineWidth: 1,
  },
  credits: {
    enabled: false, // get rid of highcharts credit
  },
  exporting: {
    fallbackToExportServer: false,
    enabled: false,
    allowHTML: true,
    chartOptions: {
      plotOptions: {
        series: {
          dataLabels: {
            enabled: true,
          },
        },
      },
    },
  },
  plotOptions: {
    series: {
      boostThreshold: 0,
      animation: {
        duration: 0,
      },
      states: {
        inactive: {
          opacity: 1,
        },
        hover: {
          enabled: false,
        },
      },
    },
  },
};
