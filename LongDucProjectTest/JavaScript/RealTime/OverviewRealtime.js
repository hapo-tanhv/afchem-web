var activepower;


var temperature;


var pressure;


var USE_FAKE_DATA = false; // Set to true to use fake data, false to use real SCADA data





document.addEventListener("DOMContentLoaded", function () {


    var activeStepTimer = null;


    // FAKE DATA - Generate random values for testing


    function generateFakeData() {


        if (!USE_FAKE_DATA) return; // Skip if using real data





        try {


            activepower = Math.floor(Math.random() * 90) + 10; // 10-100 kW


            temperature = Math.floor(Math.random() * 50) + 25;    // 25-75 °C


            pressure = Math.floor(Math.random() * 10) + 10;    // 10-20 Pa





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


                updateChartWithDynamicBands(window.tempChartInstance, temperature, 5000, {


                    color1: '#7373f3',


                    color2: 'rgba(239, 68, 68, 0.1)'


                });


            }


            if (window.pressureChartInstance && window.pressureChartInstance.series && window.pressureChartInstance.series[0]) {


                window.pressureChartInstance.series[0].update({ data: [pressure] });


                // Update plotBands dynamically based on current value


                updateChartWithDynamicBands(window.pressureChartInstance, pressure, 5000, {


                    color1: '#a2f7f7',


                    color2: 'rgba(239, 68, 68, 0.1)'


                });


            }


            // Update line chart with temperature and pressure data


            if (window.lineChartInstance) {


                updateLineChart(ambientTemp, machineTemp, pressure);


            }





            // Update Tank UI Elements


            function updateElementIfExist(id, value) {


                var el = document.getElementById(id);


                if (el) { el.innerHTML = value; }


            }





            // Randomize tank temperatures based on machine temp


            var tankTopTemp = (machineTemp * 1.1 + Math.random() * 2).toFixed(1);


            var tankMidTemp = (machineTemp * 1.0 + Math.random() * 2).toFixed(1);


            var tankBotTemp = (machineTemp * 0.9 + Math.random() * 2).toFixed(1);


            var tankPressure = (pressure / 1000).toFixed(2); // converting Pa to bar for display





            var envHumidity = (50 + Math.random() * 20).toFixed(1); // 50-70%


            var envPM25 = Math.floor(10 + Math.random() * 20); // 10-30


            var envDust = (0.2 + Math.random() * 0.5).toFixed(2); // 0.2 - 0.7





            updateElementIfExist('TankDiagramTempTop', tankTopTemp);


            updateElementIfExist('TankDiagramTemp', tankMidTemp);


            updateElementIfExist('TankDiagramTempBot', tankBotTemp);


            updateElementIfExist('TankDiagramPressure', tankPressure);





            updateElementIfExist('AmbientTemp', ambientTemp.toFixed(1));


            updateElementIfExist('Humidity', envHumidity);


            updateElementIfExist('PM25', envPM25);


            updateElementIfExist('TotalDust', envDust);





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


        setInterval(generateFakeData, 5000);


        setTimeout(generateFakeData, 500);


    }





    function renderDailyBatches(dailyBatches) {


        var tbody = document.getElementById("dailyBatchesBody");


        if (!tbody) return;


        if (!dailyBatches || dailyBatches.length === 0) {


            tbody.innerHTML = '<tr><td colspan="4" style="color: #94a3b8; padding: 20px;">Không có mẻ nào sản xuất hôm nay</td></tr>';


            return;


        }


        


        var html = '';


        dailyBatches.forEach(function (item) {


            var isActive = item.status === 'Active';


            var rowClass = isActive ? 'row-active' : '';


            var rowStyle = isActive ? 'background: rgba(0, 229, 255, 0.05);' : '';


            var textClass = isActive ? 'text-cyan' : '';


            var textStyle = !isActive ? 'color: #fff;' : '';


            var durationStyle = isActive ? '' : (item.duration === '-' ? 'color: #64748b;' : 'color: #94a3b8;');


            


            var badge = '';


            if (item.status === 'Completed') {


                badge = '<span class="status-badge completed" style="background: rgba(16, 185, 129, 0.15); color: #10b981; border: 1px solid rgba(16, 185, 129, 0.3);">Hoàn thành</span>';


            } else if (isActive) {


                badge = '<span class="status-badge in-progress" style="border: 1px solid #00e5ff; background: transparent; color: #00e5ff;">Đang thực hiện</span>';


            } else {


                badge = '<span class="status-badge pending" style="border: 1px solid #475569; background: transparent; color: #94a3b8;">Chưa thực hiện</span>';


            }


            


            html += '<tr class="' + rowClass + '" style="' + rowStyle + '">' +


                '<td style="text-align: left; padding-left: 20px; font-weight: bold; ' + textStyle + '" class="' + textClass + '">' + item.name + '</td>' +


                '<td style="font-weight: bold; ' + textStyle + '" class="' + textClass + '">' + item.weight + '</td>' +


                '<td class="' + textClass + '" style="' + durationStyle + '">' + item.duration + '</td>' +


                '<td>' + badge + '</td>' +


            '</tr>';


        });


        tbody.innerHTML = html;


    }





    // Polling current batch stats from real database every 30 seconds


    function fetchCurrentBatchStats() {


        $.ajax({


            url: '/Overview/GetCurrentBatchStats',


            type: 'GET',


            dataType: 'json',


            success: function (data) {

                // Update common header stats first
                if (typeof updateHeaderStats === 'function') {
                    updateHeaderStats(data);
                }


                if (data && data.steps) {


                    if (typeof renderBatchTable === 'function') {


                        renderBatchTable(data.steps);


                    }


                }





                if (data && data.dailyBatches) {


                    renderDailyBatches(data.dailyBatches);


                }


                


                if (data && data.batchInfo) {


                    var batchInfo = data.batchInfo;


                    


                    // Update header elements


                    var batchNumEl = document.getElementById("headerBatchNumber");


                    if (batchNumEl) batchNumEl.innerHTML = batchInfo.batchName || "-";





                    var batchNumberInfoEl = document.getElementById("batchNumberInfo");


                    if (batchNumberInfoEl) batchNumberInfoEl.innerHTML = batchInfo.batchName || "-";





                    var batchStartTimeInfoEl = document.getElementById("batchStartTimeInfo");


                    if (batchStartTimeInfoEl) {


                        if (batchInfo.batchStartTime) {


                            var timePart = batchInfo.batchStartTime.split(' ')[1] || batchInfo.batchStartTime;


                            batchStartTimeInfoEl.innerHTML = timePart;


                        } else {


                            batchStartTimeInfoEl.innerHTML = "-";


                        }


                    }


                    


                    var stepEl = document.getElementById("step");


                    if (stepEl) stepEl.innerHTML = batchInfo.headerStepName || "";


                    


                    var statusEl = document.getElementById("headerMachineStatus");


                    if (statusEl) {


                        statusEl.innerHTML = batchInfo.machineStatus;


                        if (batchInfo.machineStatus === "RUNNING") {


                            statusEl.style.color = "#22c55e";


                        } else {


                            statusEl.style.color = "#3b82f6";


                        }


                    }


                    


                    var runningTimeEl = document.getElementById("headerRunningTime");


                    if (runningTimeEl) runningTimeEl.innerHTML = batchInfo.headerRunningTime || "0s";


                    


                    var alarmCountEl = document.getElementById("alarmCount");


                    if (alarmCountEl) alarmCountEl.innerHTML = batchInfo.alarmCount !== undefined ? batchInfo.alarmCount : "0";





                    // Update timeline steps classes


                    updateTimelineUI(batchInfo.activeStepCode, batchInfo.machineStatus);





                    // Set up values for step statistics panel


                    if (activeStepTimer) {


                        clearInterval(activeStepTimer);


                        activeStepTimer = null;


                    }





                    var serverTimeMs = new Date(batchInfo.serverTime.replace(/-/g, '/')).getTime();


                    var localTimeMs = Date.now();


                    var clientServerTimeOffset = localTimeMs - serverTimeMs;


                    


                    var batchStartTime = null;


                    if (batchInfo.batchStartTime) {


                        batchStartTime = new Date(batchInfo.batchStartTime.replace(/-/g, '/')).getTime();


                    }





                    function updateStepStatsUI() {


                        var elapsed = 0;


                        var totalStdTime = 6300; // sum of standard times of all steps: 720+780+600+600+720+1200+1500+180


                        


                        if (batchInfo.machineStatus === "COMPLETED") {


                            var currentStepEl = document.getElementById("statCurrentStep");


                            if (currentStepEl) currentStepEl.innerHTML = "8 / 8";


                            


                            var stdTimeEl = document.getElementById("statStandardTime");


                            if (stdTimeEl) stdTimeEl.innerHTML = totalStdTime;


                            


                            var elapsedVal = Math.floor(batchInfo.batchTotalSeconds || totalStdTime);


                            var elapsedEl = document.getElementById("statElapsedTime");


                            if (elapsedEl) elapsedEl.innerHTML = elapsedVal;


                            


                            var remaining = Math.max(0, totalStdTime - elapsedVal);


                            var remainingEl = document.getElementById("statRemainingTime");


                            if (remainingEl) remainingEl.innerHTML = remaining;


                            


                            var progressPercent = 100;


                            var progressPercentEl = document.getElementById("statProgressPercent");


                            if (progressPercentEl) progressPercentEl.innerHTML = progressPercent + "%";


                            


                            var progressFillEl = document.getElementById("statProgressFill");


                            if (progressFillEl) {


                                progressFillEl.style.width = progressPercent + "%";


                            }


                        } else {


                            var currentStepEl = document.getElementById("statCurrentStep");


                            if (currentStepEl) {


                                currentStepEl.innerHTML = batchInfo.activeStepCode ? (batchInfo.activeStepCode + " / 8") : "";


                            }


                            


                            var stdTimeEl = document.getElementById("statStandardTime");


                            if (stdTimeEl) stdTimeEl.innerHTML = totalStdTime;


                            


                            if (batchStartTime) {


                                var currentAdjustedTime = Date.now() - clientServerTimeOffset;


                                elapsed = Math.floor((currentAdjustedTime - batchStartTime) / 1000);


                                if (elapsed < 0) elapsed = 0;


                            }


                            


                            var elapsedEl = document.getElementById("statElapsedTime");


                            if (elapsedEl) elapsedEl.innerHTML = elapsed;


                            


                            var remaining = Math.max(0, totalStdTime - elapsed);


                            var remainingEl = document.getElementById("statRemainingTime");


                            if (remainingEl) remainingEl.innerHTML = remaining;


                            


                            var activeStepCode = batchInfo.activeStepCode || 0;


                            var progressPercent = Math.round((activeStepCode / 8) * 100);


                            var progressPercentEl = document.getElementById("statProgressPercent");


                            if (progressPercentEl) progressPercentEl.innerHTML = progressPercent + "%";


                            


                            var progressFillEl = document.getElementById("statProgressFill");


                            if (progressFillEl) {


                                progressFillEl.style.width = progressPercent + "%";


                            }


                        }


                    }





                    // Run stats UI update immediately and set 1s interval


                    updateStepStatsUI();


                    activeStepTimer = setInterval(updateStepStatsUI, 1000);


                }


            },


            error: function (xhr, status, error) {


                console.error('Error fetching current batch stats:', error);


            }


        });


    }





    // Polling recent alarms from real database every 2 seconds for high-speed realtime display


    function fetchRecentAlarms() {


        $.ajax({


            url: '/Overview/GetRecentAlarms',


            type: 'GET',


            dataType: 'json',


            success: function (data) {


                if (data && !data.error) {


                    if (typeof renderAlarmList === 'function') {


                        renderAlarmList(data);


                    }


                }


            },


            error: function (xhr, status, error) {


                console.error('Error fetching recent alarms:', error);


            }


        });


    }





    // Call immediately on load


    fetchCurrentBatchStats();


    fetchRecentAlarms();





    // Set polling intervals


    setInterval(fetchCurrentBatchStats, 30000); // 30 seconds for table stats


    setInterval(fetchRecentAlarms, 2000);       // 2 seconds for active alarms





    var latestNhietDoMoiTruong = 0;


    var latestNhietDoBonTronGiua = 0;


    var latestApSuat = 0;





    var atscadaTask = document.querySelector('atscada-task');


    if (atscadaTask && atscadaTask.dataTask) {


        var dataTask = atscadaTask.dataTask;


        var dataCollection = dataTask.dataCollection;





        // Read scada tag data


        dataCollection.add(`AFChemTX01.NhietDoMoiTruong`);


        dataCollection.add(`AFChemTX01.ApSuat`);


        dataCollection.add(`AFChemTX01.DoAmMoiTruong`);


        dataCollection.add(`AFChemTX01.NhietDoBonTronTren`);


        dataCollection.add(`AFChemTX01.NhietDoBonTronGiua`);


        dataCollection.add(`AFChemTX01.NhietDoBonTronDuoi`);


        dataCollection.add(`AFChemTX01.ThoiGianCapLieu`);


        dataCollection.add(`AFChemTX01.ThoiGianTron1`);


        dataCollection.add(`AFChemTX01.ThoiGianXaDay`);


        dataCollection.add(`AFChemTX01.ThoiGianRungXaDay`);


        dataCollection.add(`AFChemTX01.ThoiGianHutXaDay`);


        dataCollection.add(`AFChemTX01.ThoiGianTron2`);


        dataCollection.add(`AFChemTX01.ThoiGianXaHang`);


        dataCollection.add(`AFChemTX01.ThoiGianRungXaHang`);





        // Thêm tag nếu cần (các tag mới sẽ được cập nhật ở phase sau nếu kết nối PLC thật)





        if (!USE_FAKE_DATA) {


            updateTag(dataCollection.get(`AFChemTX01.NhietDoMoiTruong`), document.querySelector('#AmbientTemp'), function(val) {


                latestNhietDoMoiTruong = val;


            });


            updateTag(dataCollection.get(`AFChemTX01.DoAmMoiTruong`), document.querySelector('#Humidity'));


            updateTag(dataCollection.get(`AFChemTX01.NhietDoBonTronTren`), document.querySelector('#TankDiagramTempTop'));


            updateTag(dataCollection.get(`AFChemTX01.NhietDoBonTronGiua`), document.querySelector('#TankDiagramTemp'), function(val) {


                latestNhietDoBonTronGiua = val;


            });


            updateTag(dataCollection.get(`AFChemTX01.NhietDoBonTronDuoi`), document.querySelector('#TankDiagramTempBot'));


            updateTag(dataCollection.get(`AFChemTX01.ApSuat`), document.querySelector('#TankDiagramPressure'), function(val) {


                latestApSuat = val;


            });


            updateTag(dataCollection.get(`AFChemTX01.ThoiGianCapLieu`), document.querySelector('#feedingTime'));


            updateTag(dataCollection.get(`AFChemTX01.ThoiGianTron1`), document.querySelector('#mix1Time'));


            updateTag(dataCollection.get(`AFChemTX01.ThoiGianXaDay`), document.querySelector('#bottomDischargeTime'));


            updateTag(dataCollection.get(`AFChemTX01.ThoiGianRungXaDay`), document.querySelector('#bottomDischargeVibrationTime'));


            updateTag(dataCollection.get(`AFChemTX01.ThoiGianHutXaDay`), document.querySelector('#bottomSuctionDischargeTime'));


            updateTag(dataCollection.get(`AFChemTX01.ThoiGianTron2`), document.querySelector('#mix2Time'));


            updateTag(dataCollection.get(`AFChemTX01.ThoiGianXaHang`), document.querySelector('#clearanceSaleTime'));


            updateTag(dataCollection.get(`AFChemTX01.ThoiGianRungXaHang`), document.querySelector('#vibrationDischargeTime'));





            // Periodically update charts every 5 seconds (with debounce / performance throttling)


            function updateRealCharts() {


                // Update Gauge Charts


                if (window.speedChartInstance && window.speedChartInstance.series && window.speedChartInstance.series[0]) {


                    var val = Number(latestNhietDoMoiTruong) || 0;


                    window.speedChartInstance.series[0].update({ data: [val] });


                    updateChartWithDynamicBands(window.speedChartInstance, val, 100, {


                        color1: '#EF4444',


                        color2: 'rgba(239, 68, 68, 0.1)'


                    });


                }


                


                if (window.tempChartInstance && window.tempChartInstance.series && window.tempChartInstance.series[0]) {


                    var val = Number(latestNhietDoBonTronGiua) || 0;


                    window.tempChartInstance.series[0].update({ data: [val] });


                    updateChartWithDynamicBands(window.tempChartInstance, val, 75, {


                        color1: '#7373f3',


                        color2: 'rgba(239, 68, 68, 0.1)'


                    });


                }


                


                if (window.pressureChartInstance && window.pressureChartInstance.series && window.pressureChartInstance.series[0]) {


                    var val = Number(latestApSuat) || 0;


                    window.pressureChartInstance.series[0].update({ data: [val] });


                    updateChartWithDynamicBands(window.pressureChartInstance, val, 20, {


                        color1: '#a2f7f7',


                        color2: 'rgba(239, 68, 68, 0.1)'


                    });


                }


                


                // Update Line Chart


                if (window.lineChartInstance) {


                    updateLineChart(Number(latestNhietDoMoiTruong) || 0, Number(latestNhietDoBonTronGiua) || 0, Number(latestApSuat) || 0);


                }


            }


            


            // Run immediately after 1 second to ensure tags are resolved and charts are fully loaded


            setTimeout(updateRealCharts, 1000);


            


            // Poll charts updates every 5 seconds


            setInterval(updateRealCharts, 5000);


        }





        dataTask.start();


    }


});





function updateTag(dataTag, element, onValueChangeCallback) {


    if (dataTag && element) {


        dataTag.dispatcher.on('valueChanged', (data) => {


            if (data.e.newValue !== undefined) {


                var val = data.e.newValue;


                element.innerHTML = val;


                if (typeof onValueChangeCallback === 'function') {


                    onValueChangeCallback(val);


                }


            }


        });


        if (dataTag.Value !== undefined) {


            var val = dataTag.Value;


            element.innerHTML = val;


            if (typeof onValueChangeCallback === 'function') {


                onValueChangeCallback(val);


            }


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


            backgroundColor: 'transparent',


            borderWidth: 0,


            outerRadius: '105%',


            innerRadius: '60%',


            shape: 'arc'


        }],


        center: ['50%', '55%'] // Căn giữa lại để chừa chỗ cho label bên dưới


    },





    yAxis: {


        minorTickInterval: null,


        minorTickWidth: 0,


        minorTickLength: 0,


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


            max: 100,


            tickPositions: [0, 100],


            plotBands: [{


                id: 'green-band',


                from: 0, to: 0, color: '#EF4444', thickness: 15


            }, {


                id: 'red-band',


                from: 0, to: 100, color: 'rgba(239, 68, 68, 0.1)', thickness: 15


            }],


            labels: {


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


            min: 0, max: 75,


            tickPositions: [0, 75],


            plotBands: [{


                id: 'green-band',


                from: 0, to: 0,


                color: '#7373f3', // Đỏ rực như ảnh 2


                thickness: 15


            }, {


                id: 'red-band',


                from: 0, to: 75,


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


    var ambientTempData = [];


    var machineTempData = [];


    var pressureData = [];


    var categories = [];





    return Highcharts.chart('container_line', {


        chart: {


            type: 'spline',


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


            spline: {


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


            color: '#EF4444',


            marker: {


                symbol: 'diamond'


            },


            yAxis: 0


        }, {


            name: 'Nhiệt độ máy',


            data: machineTempData,


            color: '#7373f3',


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


    var shift = chart.series[0].data.length >= 30; // Keep last 30 points





    // Get current time for x-axis in HH:mm:ss format


    var now = new Date();


    var timeStr = (now.getHours() < 10 ? '0' : '') + now.getHours() + ':' +


                  (now.getMinutes() < 10 ? '0' : '') + now.getMinutes() + ':' +


                  (now.getSeconds() < 10 ? '0' : '') + now.getSeconds();





    // Add new data points (numeric values only to correctly map with indexed categories)


    chart.series[0].addPoint(Number(ambientTemp) || 0, true, shift);


    chart.series[1].addPoint(Number(machineTemp) || 0, true, shift);


    chart.series[2].addPoint(Number(pressure) || 0, true, shift);





    // Update categories (x-axis)


    var categories = chart.xAxis[0].categories || [];


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


            max: 20,


            tickPositions: [0, 20],


            plotBands: [{


                id: 'green-band',


                from: 0, to: 0, color: '#a2f7f7', thickness: 15


            }, {


                id: 'red-band',


                from: 0, to: 20, color: 'rgba(239, 68, 68, 0.1)', thickness: 15


            }],


            labels: {


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





// Function to update timeline step items styling


function updateTimelineUI(activeStepCode, machineStatus) {


    var timeline = document.getElementById("processTimeline");


    if (!timeline) return;


    


    var steps = timeline.querySelectorAll(".step");


    steps.forEach(function(stepEl) {


        var stepNum = parseInt(stepEl.getAttribute("data-step"));


        var iconEl = stepEl.querySelector(".step-icon");


        


        // Remove all state classes


        stepEl.classList.remove("completed", "active");


        


        if (machineStatus === "COMPLETED") {


            stepEl.classList.add("completed");


            if (iconEl) iconEl.innerHTML = '<i class="fas fa-check"></i>';


        } else {


            if (stepNum < activeStepCode) {


                stepEl.classList.add("completed");


                if (iconEl) iconEl.innerHTML = '<i class="fas fa-check"></i>';


            } else if (stepNum === activeStepCode) {


                stepEl.classList.add("active");


                if (iconEl) iconEl.innerHTML = '';


            } else {


                if (iconEl) iconEl.innerHTML = '';


            }


        }


    });


}