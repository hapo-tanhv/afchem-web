var activepower;
var temperature;
var pressure;
var USE_FAKE_DATA = true; // Set to true to use fake data, false to use real SCADA data

document.addEventListener("DOMContentLoaded", function () {
    // FAKE DATA - Generate random values for testing
    function generateFakeData() {
        if (!USE_FAKE_DATA) return; // Skip if using real data

        try {
            activepower = Math.floor(Math.random() * 4500) + 500; // 500-5000 kW
            temperature = Math.floor(Math.random() * 1500) + 500;    // 500-2000 °C
            pressure = Math.floor(Math.random() * 1200) + 200;    // 200-1400 Pa

            // Separate values for line chart (realistic temperature ranges)
            var ambientTemp = 24 + Math.random() * 8; // 24-32°C
            var machineTemp = ambientTemp + 10 + Math.random() * 10; // machine is 10-20°C higher than ambient

            // Update charts with fake data
            if (window.speedChartInstance && window.speedChartInstance.series && window.speedChartInstance.series[0]) {
                window.speedChartInstance.series[0].update({ data: [activepower] });
                // Update plotBands dynamically based on current value
                updateChartWithDynamicBands(window.speedChartInstance, activepower, 5000, {
                    color1: '#EF4444',
                    color2: 'rgba(239, 68, 68, 0.1)'
                });
            }
            if (window.tempChartInstance && window.tempChartInstance.series && window.tempChartInstance.series[0]) {
                window.tempChartInstance.series[0].update({ data: [temperature] });
                // Update plotBands dynamically based on current value
                updateChartWithDynamicBands(window.tempChartInstance, temperature, 2000, {
                    color1: '#7373f3',
                    color2: 'rgba(239, 68, 68, 0.1)'
                });
            }
            if (window.pressureChartInstance && window.pressureChartInstance.series && window.pressureChartInstance.series[0]) {
                window.pressureChartInstance.series[0].update({ data: [pressure] });
                // Update plotBands dynamically based on current value
                updateChartWithDynamicBands(window.pressureChartInstance, pressure, 1500, {
                    color1: '#a2f7f7',
                    color2: 'rgba(239, 68, 68, 0.1)'
                });
            }
            // Update line chart with temperature and pressure data
            if (window.lineChartInstance) {
                updateLineChart(ambientTemp, machineTemp, pressure);
            }
            console.log('Fake data updated:', { activepower, temperature, pressure, ambientTemp, machineTemp });
        } catch (e) {
            console.error('Error updating fake data:', e);
        }
    }

    // Initialize charts
    try {
        window.speedChartInstance = AcivePowerChart();
        window.tempChartInstance = TemperatureChart();
        window.pressureChartInstance = PressureChart();
        window.lineChartInstance = LineChart(); // Initialize line chart
        console.log('Charts initialized successfully');
    } catch (e) {
        console.error('Error initializing charts:', e);
    }

    // Generate fake data every 2 seconds
    if (USE_FAKE_DATA) {
        setInterval(generateFakeData, 2000);
        setTimeout(generateFakeData, 500);
    }

    var atscadaTask = document.querySelector('atscada-task');
    if (atscadaTask && atscadaTask.dataTask) {
        var dataTask = atscadaTask.dataTask;
        var dataCollection = dataTask.dataCollection;

        // Read scada tag data
        dataCollection.add(`AFChemPLC.NhietDoMay`);
        dataCollection.add(`AFChemPLC.NhietDoMoiTruong`);
        dataCollection.add(`AFChemPLC.ApSuat`);
        dataCollection.add(`AFChemPLC.QuyTrinh`);
        dataCollection.add(`AFChemPLC.CongDoanMay`);
        dataCollection.add(`AFChemPLC.NhietDoGiuaBuongTron`);
        dataCollection.add(`AFChemPLC.DoAmMoiTruong`);

        // Thêm tag nếu cần (các tag mới sẽ được cập nhật ở phase sau nếu kết nối PLC thật)

        if (!USE_FAKE_DATA) {
            updateTag(dataCollection.get(`AFChemPLC.NhietDoMoiTruong`), document.querySelector('#AmbientTemp'));
            updateTag(dataCollection.get(`AFChemPLC.DoAmMoiTruong`), document.querySelector('#Humidity'));
            updateTag(dataCollection.get(`AFChemPLC.NhietDoGiuaBuongTron`), document.querySelector('#MixerMidTemp'));
            updateTag(dataCollection.get(`AFChemPLC.NhietDoGiuaBuongTron`), document.querySelector('#TankDiagramTemp'));
            updateTag(dataCollection.get(`AFChemPLC.ApSuat`), document.querySelector('#TankDiagramPressure'));
        }

        dataTask.start();
    }
});

function updateTag(dataTag, element) {
    if (dataTag && element) {
        dataTag.dispatcher.on('valueChanged', (data) => {
            if (data.e.newValue !== undefined) {
                element.innerHTML = data.e.newValue;
            }
        });
        if (dataTag.Value !== undefined) {
            element.innerHTML = data.e.newValue;
        }
    }
}

var gaugeOptions = {
    chart: {
        type: 'gauge',
        backgroundColor: 'transparent',
        height: '250px' // Cố định chiều cao để không bị vỡ layout
    },

    title: null,

    pane: {
        startAngle: -100,
        endAngle: 100,
        background: [{
            backgroundColor: 'rgba(255, 255, 255, 0.05)', // Tạo vòng cung nền mờ
            borderWidth: 0,
            outerRadius: '105%',
            innerRadius: '60%',
            shape: 'arc'
        }],
        center: ['50%', '55%'] // Căn giữa lại để chừa chỗ cho label bên dưới
    },

    yAxis: {
        minorTickInterval: 'auto',
        minorTickWidth: 1,
        minorTickLength: 5,
        minorTickPosition: 'inside',
        minorTickColor: '#475569',

        tickPixelInterval: 30,
        tickWidth: 2,
        tickPosition: 'inside',
        tickLength: 12,
        tickColor: '#94a3b8',

        labels: {
            distance: -25, // Đẩy con số vào sâu bên trong vòng cung
            style: {
                color: '#a9b7cb',
                fontSize: '10px'
            }
        },
        title: { text: null },
        lineWidth: 0
    },

    plotOptions: {
        gauge: {
            dial: {
                radius: '65%', // Kim ngắn lại để trông sang hơn
                backgroundColor: '#ffffff',
                baseWidth: 4,
                topWidth: 1,
                baseLength: '0%',
                rearLength: '10%' // Thêm một chút đuôi kim
            },
            pivot: {
                radius: 8, // Tâm kim to và rõ như ảnh 2
                backgroundColor: '#ffffff'
            }
        }
    },
    exporting: { enabled: false },
    credits: { enabled: false }
};

function AcivePowerChart() {
    return Highcharts.chart('container-speed', Highcharts.merge(gaugeOptions, {
        yAxis: {
            min: 0,
            max: 5000,
            plotBands: [{
                id: 'green-band',
                from: 0, to: 0, color: '#EF4444', thickness: 15
            }, {
                id: 'red-band',
                from: 0, to: 5000, color: 'rgba(239, 68, 68, 0.1)', thickness: 15
            }],
            labels: {
                step: 2,
                rotation: 'auto',
                style: { color: '#94a3b8' }
            }
        },
        series: [{
            name: 'Nhiệt độ môi trường',
            data: [0],
            dataLabels: {
                format: '<div style="text-align:center">' +
                    '<span style="font-size:22px;color:#fff;font-weight:bold">{y}</span> ' +
                    '<span style="font-size:16px;color:#fff">°C</span><br/>' +
                    '<div style="width:15px; height:2px; background:#EF4444; display:inline-block; margin-bottom:4px"></div> ' +
                    '<span style="font-size:12px;color:#94a3b8;font-weight:normal">Nhiệt độ môi trường</span>' +
                    '</div>',
                borderWidth: 0,
                y: 80,
                useHTML: true
            }
        }]
    }));
}

// Ví dụ cho Temperature - Màu Đỏ (Nhiệt độ môi trường)
function TemperatureChart() {
    return Highcharts.chart('container-temp', Highcharts.merge(gaugeOptions, {
        yAxis: {
            min: 0, max: 2000,
            plotBands: [{
                id: 'green-band',
                from: 0, to: 0,
                color: '#7373f3', // Đỏ rực như ảnh 2
                thickness: 15
            }, {
                id: 'red-band',
                from: 0, to: 2000,
                color: 'rgba(239, 68, 68, 0.1)', // Phần còn lại màu mờ
                thickness: 15
            }]
        },
        series: [{
            name: 'Nhiệt độ máy',
            data: [0],
            dataLabels: {
                format: '<div style="text-align:center; margin-top: 20px">' +
                    '<span style="font-size:24px;color:#fff;font-weight:bold">{y}</span> ' +
                    '<span style="font-size:16px;color:#fff">°C</span><br/>' +
                    '<div style="width:15px; height:2px; background:#7373f3; display:inline-block; margin-bottom:4px"></div> ' +
                    '<span style="font-size:12px;color:#94a3b8;font-weight:normal">Nhiệt độ máy</span>' +
                    '</div>',
                y: 65, // Điều chỉnh lại vị trí text
                useHTML: true,
                borderWidth: 0
            }
        }]
    }));
}

// Line chart with 2 Y-axes for temperature and pressure
function LineChart() {
    // Initial fake data for demonstration
    var now = new Date();
    var dataPoints = 20; // Number of data points to show

    // Generate initial data
    var ambientTempData = [];
    var machineTempData = [];
    var pressureData = [];
    var categories = [];

    for (var i = dataPoints; i > 0; i--) {
        var time = new Date(now.getTime() - i * 60000);
        categories.push(time.getHours() + ':' + (time.getMinutes() < 10 ? '0' : '') + time.getMinutes());

        // Generate fake data with realistic values
        ambientTempData.push(28 + Math.random() * 8 - 4); // 24-32°C
        machineTempData.push(35 + Math.random() * 15 - 7.5); // 27.5-42.5°C
        pressureData.push(980 + Math.random() * 40 - 20); // 960-1000 hPa
    }

    return Highcharts.chart('container_line', {
        chart: {
            type: 'line',
            animation: true,
            backgroundColor: 'transparent'
        },
        title: {
            text: 'Biểu đồ Nhiệt độ và Áp suất',
            style: {
                color: '#ffffff',
                fontSize: '16px'
            }
        },
        xAxis: {
            categories: categories,
            labels: {
                style: { color: '#94a3b8' }
            },
            lineColor: '#475569',
            tickColor: '#475569'
        },
        yAxis: [{
            title: {
                text: 'Nhiệt độ (°C)',
                style: { color: '#e879f9' }
            },
            labels: {
                style: { color: '#e879f9' },
                format: '{value}°C'
            },
            opposite: false,
            gridLineColor: 'rgba(255,255,255,0.1)'
        }, {
            title: {
                text: 'Áp suất (Bar)',
                style: { color: '#22d3ee' }
            },
            labels: {
                style: { color: '#22d3ee' },
                format: '{value}'
            },
            opposite: true,
            gridLineColor: 'rgba(255,255,255,0.1)'
        }],
        tooltip: {
            shared: true,
            backgroundColor: 'rgba(15, 23, 42, 0.9)',
            borderColor: '#475569',
            style: { color: '#ffffff' },
            valueSuffix: ''
        },
        legend: {
            itemStyle: { color: '#94a3b8' },
            itemHoverStyle: { color: '#ffffff' }
        },
        plotOptions: {
            line: {
                marker: {
                    enabled: true,
                    radius: 4
                },
                lineWidth: 2,
                states: {
                    hover: {
                        lineWidth: 3
                    }
                }
            }
        },
        series: [{
            name: 'Nhiệt độ môi trường',
            data: ambientTempData,
            color: '#e879f9',
            marker: {
                symbol: 'diamond'
            },
            yAxis: 0
        }, {
            name: 'Nhiệt độ máy',
            data: machineTempData,
            color: '#3b82f6',
            marker: {
                symbol: 'square'
            },
            yAxis: 0
        }, {
            name: 'Áp suất',
            data: pressureData,
            color: '#22d3ee',
            marker: {
                symbol: 'circle'
            },
            yAxis: 1
        }],
        credits: { enabled: false }
    });
}

// Update line chart with new data point
function updateLineChart(ambientTemp, machineTemp, pressure) {
    if (!window.lineChartInstance || !window.lineChartInstance.series) return;

    var chart = window.lineChartInstance;
    var shift = chart.series[0].data.length > 30; // Keep last 30 points

    // Get current time for x-axis
    var now = new Date();
    var timeStr = now.getHours() + ':' + (now.getMinutes() < 10 ? '0' : '') + now.getMinutes();

    // Add new data points
    chart.series[0].addPoint([timeStr, ambientTemp], true, shift);
    chart.series[1].addPoint([timeStr, machineTemp], true, shift);
    chart.series[2].addPoint([timeStr, pressure], true, shift);

    // Update categories (x-axis)
    var categories = chart.xAxis[0].categories;
    categories.push(timeStr);
    if (categories.length > 30) {
        categories.shift();
    }
    chart.xAxis[0].setCategories(categories, true);
}

function PressureChart() {
    return Highcharts.chart('container-pressure', Highcharts.merge(gaugeOptions, {
        yAxis: {
            min: 0,
            max: 1500,
            plotBands: [{
                id: 'green-band',
                from: 0, to: 0, color: '#a2f7f7', thickness: 15
            }, {
                id: 'red-band',
                from: 0, to: 1500, color: 'rgba(239, 68, 68, 0.1)', thickness: 15
            }],
            labels: {
                step: 2,
                rotation: 'auto',
                style: { color: '#94a3b8' }
            }
        },
        series: [{
            name: 'Áp suất',
            data: [0],
            dataLabels: {
                format: '<div style="text-align:center">' +
                    '<span style="font-size:22px;color:#fff;font-weight:bold">{y}</span> ' +
                    '<span style="font-size:16px;color:#fff">Bar</span><br/>' +
                    '<div style="width:15px; height:2px; background:#a2f7f7; display:inline-block; margin-bottom:4px"></div> ' +
                    '<span style="font-size:12px;color:#94a3b8;font-weight:normal">Áp suất</span>' +
                    '</div>',
                borderWidth: 0,
                y: 80,
                useHTML: true
            }
        }]
    }));
}

// Function to update plotBands dynamically based on current value
function updateChartWithDynamicBands(chartInstance, value, max, config) {
    if (!chartInstance || !chartInstance.yAxis || !chartInstance.yAxis[0]) return;

    var yAxis = chartInstance.yAxis[0];

    // Remove existing plotBands by ID
    yAxis.removePlotBand('green-band');
    yAxis.removePlotBand('red-band');

    // Add new plotBands with updated values
    yAxis.addPlotBand({
        id: 'green-band',
        from: 0,
        to: value,
        color: config.color1 || '#EF4444',
        thickness: 15
    });

    yAxis.addPlotBand({
        id: 'red-band',
        from: value,
        to: max,
        color: 'rgba(239, 68, 68, 0.1)',
        thickness: 15
    });

    // Redraw the entire chart
    chartInstance.redraw();
}