"use strict";
exports.__esModule = true;
exports.trajectoryOptions = exports.lithologyOptions = exports.stratigraphyOptions = exports.graphicCanvasOptions = exports.chartMarginY = exports.chartMarginX = exports.reflowCharts = exports.postProcessStratigraphySeries = exports.syncLitologia = exports.dropPointObservable = exports.dropPoint = exports.dragPointObservable = exports.dragPoint = exports.startDragPointObservable = exports.startDragPoint = exports.clickChartObservable = exports.clickChart = exports.syncZoomObservable = exports.syncExtremes = exports.estratAdjust = exports.toggleStratLabels = exports.getNonEmptyCharts = void 0;
var Highcharts = require("highcharts");
var boost_1 = require("highcharts/modules/boost");
var no_data_to_display_1 = require("highcharts/modules/no-data-to-display");
var draggable_points_1 = require("highcharts/modules/draggable-points");
var exporting_1 = require("highcharts/modules/exporting");
var export_data_1 = require("highcharts/modules/export-data");
var environment_1 = require("../../environments/environment");
var rxjs_1 = require("rxjs");
var app_constants_1 = require("app/app.constants");
exporting_1["default"](Highcharts);
export_data_1["default"](Highcharts);
boost_1["default"](Highcharts);
no_data_to_display_1["default"](Highcharts);
draggable_points_1["default"](Highcharts);
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
function getNonEmptyCharts() {
    /**
     * We're gonna iterate backwards through the array because we'll use
     * Array.splice. Splicing causes index shifts when iterating forwards,
     * so, to prevent esoteric increments on the counter, we'll iterate
     * from end to start.
     */
    return Highcharts.charts.filter(function (chart) { return chart !== undefined; });
}
exports.getNonEmptyCharts = getNonEmptyCharts;
/**
 * Enables or disables the label of a particular chart series,
 * depending on the size of the label and the height of the series.
 */
function toggleStratLabels(chart) {
    // For every point of the series
    return chart.series.forEach(function (element) {
        // Get the height of the series' block, in pixels
        var columnSize = element.data[0]['shapeArgs'].height;
        // Get the height of the label inside it, in pixels
        var textSize = element['dataLabelsGroup'].element.childNodes[1].getBoundingClientRect().height;
        // If the size of the label is greater than the size of the column
        if (textSize > columnSize) {
            // Hide the label
            element['dataLabelsGroup'].element.style.visibility = 'hidden';
        }
        else {
            // Otherwise show it
            element['dataLabelsGroup'].element.style.visibility = 'visible';
        }
    });
}
exports.toggleStratLabels = toggleStratLabels;
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
function estratAdjust() {
    // For every chart currently on screen
    Highcharts.charts.forEach(function (chart) {
        // Check if the chart is defined and if is a stratigraphy chart
        if (chart && chart.title['textStr'] === 'Estratigrafia') {
            // Update the visibility of the labels in it
            toggleStratLabels(chart);
            // Redraw the chart with duration 0 to prevent animations
            chart.redraw({ duration: 0 });
        }
    });
}
exports.estratAdjust = estratAdjust;
/**
 * Notify sync service that zoom event has occurred
 */
function syncExtremes(e) {
    // Checks whether the event was called by the sync service to prevent infinite loops
    if (e.trigger !== 'syncExtremes') {
        exports.syncZoomObservable.next({ start: e.min, end: e.max });
    }
}
exports.syncExtremes = syncExtremes;
exports.syncZoomObservable = new rxjs_1.Subject();
/**
 * Notify edit trend service that a click has occurred
 */
function clickChart(e) {
    exports.clickChartObservable.next({ chart: this, event: e });
}
exports.clickChart = clickChart;
exports.clickChartObservable = new rxjs_1.Subject();
/**
 * Notify edit trend service that a drag is started
 */
function startDragPoint(e) {
    exports.startDragPointObservable.next({ point: this, event: e });
}
exports.startDragPoint = startDragPoint;
exports.startDragPointObservable = new rxjs_1.Subject();
/**
 * Notify edit trend service that a drag is occurring
 */
function dragPoint(e) {
    exports.dragPointObservable.next({ point: this, event: e });
}
exports.dragPoint = dragPoint;
exports.dragPointObservable = new rxjs_1.Subject();
/**
 * Notify edit trend service that a drop has occurred
 */
function dropPoint(e) {
    exports.dropPointObservable.next({ point: this, event: e });
}
exports.dropPoint = dropPoint;
exports.dropPointObservable = new rxjs_1.Subject();
/**
 * These variables must stay as globals so syncLitologia doesn't fall into an
 * infinite loop, as it calls the redraw() function, which will then trigger the
 * render event of the lithology chart, which will then trigger the function once
 * again, endlessly.
 *
 * There must be a better solution for this, but since we don't have much time
 * to do this right now, I'm marking it with a TODO.
 */
var redrawEnabled = true, ctr = 0;
/**
 * Inserts the chart pattern into the series' blocks.
 *
 * @param e an object which has the chart being synced.
 */
function syncLitologia(e) {
    // Fetch the chart from the event object
    var chart = e.target;
    // Define the scale which will be used throughout the code (default is 3)
    var BASE_LITHOLOGY_SCALE = 3;
    // If the chart is defined
    if (chart) {
        // If the global control is allowing redraw
        if (redrawEnabled) {
            // Disable it so the inner calls to redraw() don't create an infinite loop
            redrawEnabled = false;
            // Get the chart renderer
            var renderer_1 = chart.renderer;
            var _loop_1 = function (i) {
                // For each one of the points
                chart.series[i].points.forEach(function (p) {
                    // p.shapeArgs.height = chart.series[i].dataMax - chart.series[i].dataMin;
                    // Get the name of the corresponding lithology from the chart options
                    var description = chart.series[i].userOptions.className;
                    // Calculate the scale of the image based on the section width and height
                    var widthRatio = (p.shapeArgs.width / BASE_LITHOLOGY_SCALE) / p.shapeArgs.height;
                    // Define a unique ID for this pattern
                    var id = "pattern-" + p.index + "-" + ctr;
                    // Create a new pattern object and render it on the chart
                    var pattern = renderer_1.createElement('pattern').add(renderer_1.defs).attr({
                        width: 1,
                        height: widthRatio,
                        id: id,
                        patternContentUnits: 'objectBoundingBox'
                    });
                    var imageUrl;
                    // Create an alias for the image URL
                    if (environment_1.environment.production || environment_1.environment.staging) {
                        imageUrl = app_constants_1.BASE_URL + "/assets/images/litologias/" + description + ".png"; // producao
                    }
                    else {
                        imageUrl = app_constants_1.BASE_URL + "/sestweb/assets/images/litologias/" + description + ".png"; // localhost
                    }
                    // Generate the pattern based on the lithology scale
                    for (var j = 0; j < BASE_LITHOLOGY_SCALE; j++) {
                        renderer_1.image(imageUrl, (1 / BASE_LITHOLOGY_SCALE) * j, 0, (1 / BASE_LITHOLOGY_SCALE), widthRatio).attr({}).add(pattern);
                    }
                    // Update the series passing the newly created pattern as its 'color'
                    p.update({ color: "url(#" + id + ")" }, false);
                });
                // Increment the control variable so IDs are always unique
                ctr++;
            };
            // Iterate through the chart series
            for (var i = 0; i < chart.series.length; i++) {
                _loop_1(i);
            }
            // Redraw the chart
            chart.redraw();
            // Enable redraw again, AFTER redraw has been called, to avoid infinite loops
            redrawEnabled = true;
        }
    }
}
exports.syncLitologia = syncLitologia;
/**
 * Postprocesses the series received, setting their background color and label.
 *
 * @param {*} series an array containing series information.
 */
function postProcessStratigraphySeries(series) {
    // The two colors which will be used to alternatingly paint the stratigraphy chart
    var colors = ['#e0e0e0', '#eeeeee'];
    // Initialize the return array
    var processedSeries = [];
    // For every item in the series
    series.forEach(function (item, i) {
        // Create a new entry with a label
        var entry = {
            name: item.name,
            color: colors[i % 2],
            data: item.data,
            type: 'columnrange',
            dataLabels: {
                enabled: true,
                color: 'auto',
                rotation: -90,
                formatter: function () { return item.name.toUpperCase(); } // Make the label uppercase
            }
        };
        // Add the processed entry to the series
        processedSeries.push(entry);
    });
    return processedSeries;
}
exports.postProcessStratigraphySeries = postProcessStratigraphySeries;
/**
 * Reflow every chart currently on screen.
 */
function reflowCharts() {
    // For every chart on screen
    getNonEmptyCharts().forEach(function (chart) {
        // Check if it is defined and reflow it
        if (chart) {
            chart.reflow();
        }
    });
}
exports.reflowCharts = reflowCharts;
exports.chartMarginX = 0.1;
exports.chartMarginY = 0.1;
exports.graphicCanvasOptions = {
    legend: {
        enabled: false
    },
    mapNavigation: {
        enabled: false
    },
    chart: {
        events: {
            click: clickChart
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
                display: 'none'
            }
        }
    },
    title: {
        text: '',
        style: { 'display': 'none' }
    },
    tooltip: {
        enabled: false
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
            text: ''
        },
        gridLineWidth: 1,
        lineWidth: 0,
        tickWidth: 1
    },
    xAxis: {
        lineWidth: 0,
        tickWidth: 0,
        reversed: true,
        type: 'linear',
        minorTickInterval: 0.1,
        title: {
            text: null
        },
        labels: {
            enabled: false
        },
        events: {
            setExtremes: syncExtremes
        },
        startOnTick: false,
        endOnTick: false,
        gridLineWidth: 1
    },
    credits: {
        enabled: false // get rid of highcharts credit
    },
    exporting: {
        enabled: false,
        allowHTML: true,
        chartOptions: {
            plotOptions: {
                series: {
                    dataLabels: {
                        enabled: true
                    }
                }
            }
        }
    },
    plotOptions: {
        series: {
            boostThreshold: 0,
            animation: {
                duration: 0
            },
            states: {
                inactive: {
                    opacity: 1
                },
                hover: {
                    enabled: false
                }
            },
            point: {
                events: {
                    dragStart: startDragPoint,
                    drag: dragPoint,
                    drop: dropPoint
                }
            }
        }
    }
};
exports.stratigraphyOptions = {
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
            fontFamily: "'Lucida' Grande, sans-serif;",
            fontWeight: 'bold'
        },
        events: {
            redraw: toggleStratLabelsWrapper,
            load: estratAdjust
        },
        resetZoomButton: {
            theme: {
                display: 'none'
            }
        }
    },
    navigator: {
        enabled: true
    },
    mapNavigation: {
        enabled: true,
        enableMouseWheelZoom: true,
        mouseWheelSensitivity: 7,
        enableButtons: false
    },
    rangeSelector: {
        selected: 1
    },
    tooltip: {
        enabled: true,
        headerFormat: null,
        useHTML: true,
        formatter: function () {
            return ("<span style='font-size:11px; text-align:center;'>\n        <strong style='color:#666;font-weight:bold;font-size:11px;'>" + this.series.name + "</br>\n        <strong style='color:#666;font-weight:normal;font-size:10px;'>Profund.:</br></strong>\n        <strong style='color:#666;font-weight:normal;font-size:09px;'>" + this.point['low'] + "\n        </strong></span>");
        },
        pointFormat: "<tr><td style='color: {series.color}, z-index: '7'>{series.name}: </td>\n      <td style='text-align: right', z-index: '7'><b>{point.y} </b></td></tr>",
        style: {
            zIndex: '9999'
        },
        valueDecimals: 1
    },
    legend: {
        enabled: false
    },
    exporting: {
        enabled: false,
        allowHTML: true,
        chartOptions: {
            plotOptions: {
                series: {
                    dataLabels: {
                        enabled: true
                    }
                }
            }
        }
    },
    credits: {
        enabled: false // get rid of highcharts credit
    },
    title: {
        text: 'Estratigrafia',
        style: { display: 'none' }
    },
    xAxis: {
        type: 'linear',
        tickmarkPlacement: 'on',
        categories: [null],
        labels: {
            enabled: false
        },
        reversed: true,
        lineWidth: 0,
        minorGridLineWidth: 0,
        lineColor: 'transparente',
        gridLineColor: 'transparente',
        gridLineWidth: 0,
        tickWidth: 0,
        opposite: true
    },
    yAxis: {
        reversed: true,
        type: 'linear',
        tickInterval: null,
        labels: {
            enabled: false
        },
        title: {
            text: null
        },
        crosshair: true,
        events: {
            setExtremes: syncExtremes
        },
        startOnTick: false,
        endOnTick: false
    },
    plotOptions: {
        columnrange: {
            dataLabels: {
                enabled: true,
                inside: true,
                allowOverlap: false,
                align: 'center',
                verticalAlign: 'middle',
                format: '{series.name}'
            },
            grouping: false,
            pointPadding: 0,
            borderWidth: 0,
            groupPadding: 0,
            shadow: false
        },
        series: {
            animation: {
                duration: 0
            }
        }
    }
};
exports.lithologyOptions = {
    legend: {
        enabled: false
    },
    mapNavigation: {
        enabled: true,
        enableMouseWheelZoom: true,
        mouseWheelSensitivity: 7,
        enableButtons: false
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
            load: syncExtremes
        },
        resetZoomButton: {
            theme: {
                display: 'none'
            }
        }
    },
    title: {
        text: 'Litologia',
        style: { display: 'none' }
    },
    tooltip: {
        enabled: true,
        headerFormat: null,
        useHTML: true,
        formatter: function () {
            return ("<span style='font-size:11px; text-align:center;'>\n        <strong style='color:#666;font-weight:normal;font-size:09px;'>\n        " + this.series.name + "\n        </strong></span>");
            // + '</br>' + '<strong style='color:#666;font-weight:normal;font-size:10px;'>' + this.y + '</strong></p>';
        },
        pointFormat: "<tr><td style='color: {series.color}, z-index: '7'>{series.name}: </td>\n    <td style='text-align: right', z-index: '7'><b>{point.y} </b></td></tr>",
        style: {
            zIndex: '9999'
        },
        valueDecimals: 1
    },
    xAxis: {
        tickmarkPlacement: 'on',
        categories: [null],
        labels: {
            enabled: false
        },
        lineWidth: 0,
        gridLineWidth: 0,
        tickWidth: 0,
        opposite: true
    },
    yAxis: {
        reversed: true,
        type: 'linear',
        labels: {
            enabled: false
        },
        title: {
            text: null
        },
        events: {
            setExtremes: syncExtremes
        },
        startOnTick: false,
        endOnTick: false
    },
    credits: {
        enabled: false // get rid of highcharts credit
    },
    exporting: {
        enabled: false,
        allowHTML: true,
        chartOptions: {
            plotOptions: {
                series: {
                    dataLabels: {
                        enabled: true
                    }
                }
            }
        }
    },
    navigator: {
        enabled: true
    },
    rangeSelector: {
        selected: 1
    },
    plotOptions: {
        columnrange: {
            grouping: false,
            pointPadding: 0,
            borderWidth: 0,
            groupPadding: 0,
            cropThreshold: 140,
            shadow: false
        },
        series: {
            animation: {
                duration: 0
            },
            events: {
                afterAnimate: function () {
                }
            }
        }
    }
};
exports.trajectoryOptions = {
    series: [{
            type: 'line',
            color: '#187BC7'
        }],
    legend: {
        enabled: false
    },
    mapNavigation: {
        enabled: true,
        buttonOptions: {
            align: 'left',
            verticalAlign: 'bottom',
            x: -65
        }
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
            load: syncExtremes
        },
        panning: true,
        panKey: 'meta',
        resetZoomButton: {
            theme: {
                display: 'none'
            }
        }
    },
    title: {
        text: 'Trajetória',
        style: { display: 'none' }
    },
    tooltip: {
        headerFormat: null
    },
    yAxis: {
        opposite: true,
        type: 'linear',
        title: {
            text: ''
        },
        startOnTick: false,
        endOnTick: false,
        gridLineWidth: 1,
        lineWidth: 0,
        tickWidth: 1
    },
    xAxis: {
        reversed: true,
        type: 'linear',
        title: {
            text: 'Profundidade(m)'
        },
        events: {
            setExtremes: syncExtremes
        },
        startOnTick: false,
        endOnTick: false,
        gridLineWidth: 1,
        lineWidth: 0,
        tickWidth: 0
    },
    credits: {
        enabled: false // get rid of highcharts credit
    },
    exporting: {
        enabled: false,
        allowHTML: true,
        chartOptions: {
            plotOptions: {
                series: {
                    dataLabels: {
                        enabled: true
                    }
                }
            }
        }
    },
    plotOptions: {
        series: {
            animation: {
                duration: 0
            },
            keys: ['x', 'y', 'marker', 'dado', 'dataLabels'],
            states: {
                inactive: {
                    opacity: 1
                },
                hover: {
                    enabled: false
                }
            }
        }
    }
};
