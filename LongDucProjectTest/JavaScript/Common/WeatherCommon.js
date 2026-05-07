//hàm get thời gian hiện tại
var chartTemp = null;
var chartIrradian = null;
var chartTempCell1 = null;
var chartTempCell2 = null;

function DateTime() {
    const d = new Date();
    let day = String(d.getDate()).padStart(2, '0');
    let month = d.getMonth() + 1;
    if (month < 10) month = '0' + month;
    let year = d.getFullYear();
    return { day, month, year }
}

function Load(ProjectName) {
    var x = new DateTime();

    document.getElementById("selecttimeTemp").value = `${x.year}-${x.month}-${x.day}`;
    document.getElementById("selecttimeIrr").value = `${x.year}-${x.month}-${x.day}`;
    document.getElementById("selecttimeCell1").value = `${x.year}-${x.month}-${x.day}`;
    document.getElementById("selecttimeCell2").value = `${x.year}-${x.month}-${x.day}`;
    SubmitTemp(ProjectName);
    SubmitIrradian(ProjectName);
    SubmitCell1(ProjectName)
    SubmitCell2(ProjectName)
}

// Get data and draw chart for Temperature
function SubmitTemp(ProjectName) {
    Time = document.getElementById('selecttimeTemp').value;
    longPoliingTemp(ProjectName);
}
async function longPoliingTemp(ProjectName) {
    var dataLine = await getData(ProjectName);
    updateChartTemp(dataLine);
}
function ProjectTempChange(ProjectName) {
    SubmitTemp(ProjectName)
}

//Get data and draw chart for Irradian
function SubmitIrradian(ProjectName) {
    Time = document.getElementById('selecttimeIrr').value;
    longPoliingIrr(ProjectName);
}
async function longPoliingIrr(ProjectName) {
    var dataLine = await getData(ProjectName);
    updateChartIrr(dataLine);
}
function ProjectIrrChange(ProjectName) {
    SubmitIrradian(ProjectName)
}

//Get data and draw chart for Cell 1
function SubmitCell1(ProjectName) {
    Time = document.getElementById('selecttimeCell1').value;
    longPoliingCell1(ProjectName);
}
async function longPoliingCell1(ProjectName) {
    var dataLine = await getData(ProjectName);
    updateChartCell1(dataLine);
}
function ProjectCell1Change(ProjectName) {
    SubmitCell1(ProjectName)
}

//Get data and draw chart for Cell 2
function SubmitCell2(ProjectName) {
    Time = document.getElementById('selecttimeCell2').value;
    longPoliingCell2(ProjectName);
}
async function longPoliingCell2(ProjectName) {
    var dataLine = await getData(ProjectName);
    updateChartCell2(dataLine);
}
function ProjectCell2Change(ProjectName) {
    SubmitCell2(ProjectName)
}


//Draw chart for Ambient Temperature
async function getData(ProjectName) {
    /*var link = "/DataProject/GetProjectWeather?"*/
    var link = "/DataProject/GetProjectWeather?"
    var url = link + new URLSearchParams({
        starttime: Time,
        endtime: Time,
        ProjectName: ProjectName
    });

    var options = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },
    };
    const response = await fetch(url, options);
    const dataLine = response.json();
    return dataLine;
}


function updateChartTemp(dataLine) {
    var chuoiso = [];
    var time = [];
    var timemin = [];
    var hours = [];
    var tempvalue = [];
    var timeindex = 0;
    var indexnull = 0;
    var arrfintemp = [];
    var timeXaxis = [];

    var hour = ["00:", "01:", "02:", "03:", "04:", "05:", "06:", "07:", "08:", "09:", "10:", "11:", "12:", "13:", "14:", "15:", "16:", "17:", "18:", "19:", "20:", "21:", "22:", "23:"];
    var min = ["00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10",
        "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
        "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
        "31", "32", "33", "34", "35", "36", "37", "38", "39", "40",
        "41", "42", "43", "44", "45", "46", "47", "48", "49", "50",
        "51", "52", "53", "54", "55", "56", "57", "58", "59",
    ];
    for (var i = 0; i < 24; i++) {
        //for (var j = 0; j < 60; j++) {
        //    time[timeindex] = hour[i] + min[j];
        //    timeindex++;
        //}
        for (var j = 0; j < 60; j++) {
            if (min[j] == "00") {
                timeXaxis[timeindex] = hour[i].substring(0, 2);
            }
            else {
                timeXaxis[timeindex] = hour[i] + min[j];
            }
            time[timeindex] = hour[i] + min[j];
            timeindex++;
        }
    }
    for (var i = 0; i < dataLine.length; i++) {
        chuoiso[i] = Number(dataLine[i].DateTime.replace(/[^0-9]/g, ''));
        timemin[i] = new Date(chuoiso[i]).toLocaleTimeString('en-IE').substring(0, 5);
    }
    for (var i = 0; i < time.length; i++) {
        if (timemin.indexOf(time[i]) > -1) {
            arrfintemp[i] = dataLine[i - indexnull].AmbientTemperature;
        }
        else {
            arrfintemp[i] = null;

            indexnull++;
        }
        tempvalue.push(arrfintemp[i])
        /*hours[i] = Math.floor(i / 60);*/
        hours[i] = timeXaxis[i];
    }
    chartTemp = Highcharts.chart('container_temp', {

        title: {
            text: 'Ambient Temperature'
        },
        exporting: {
            enabled: false
        },
        credits: {
            enabled: false
        },
        yAxis: {
            title: {
                text: '℃'
            },
            min: 0,
            max: 100,
            tickInterval: 20,
        },

        xAxis: {
            //type:'datetime',
            tickInterval: 60,
            categories: hours
        },

        credits: {
            enabled: false
        },
        legend: {
            position: 'bottom',
            display: false
        },

        plotOptions: {
            series: {
                label: {
                    connectorAllowed: true
                },

            }
        },
        //tooltip: {
        //    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
        //    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
        //        '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
        //    footerFormat: '</table>',
        //    shared: true,
        //    useHTML: true
        //},
        series: [{
            name: '',
            data: tempvalue,
            color: '#228b22',
            tooltip: {
                valueSuffix: ' ℃'
            },
            legendName: 'Ambient Temperature'
        }],
        legend: {
            labelFormatter: function () {
                return this.userOptions.legendName || this.name;
            }
        },

        responsive: {
            rules: [{
                condition: {
                    maxWidth: 500
                },
                chartOptions: {
                    legend: {
                        layout: 'horizontal',
                        align: 'center',
                        verticalAlign: 'bottom'
                    }
                }
            }]
        }

    });
}


function updateChartIrr(dataLine) {
    var chuoiso = [];
    var time = [];
    var timemin = [];
    var hours = [];
    var irrvalue = [];
    var timeindex = 0;
    var indexnull = 0;
    var arrfinirr = [];
    var timeXaxis = [];

    var hour = ["00:", "01:", "02:", "03:", "04:", "05:", "06:", "07:", "08:", "09:", "10:", "11:", "12:", "13:", "14:", "15:", "16:", "17:", "18:", "19:", "20:", "21:", "22:", "23:"];
    var min = ["00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10",
        "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
        "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
        "31", "32", "33", "34", "35", "36", "37", "38", "39", "40",
        "41", "42", "43", "44", "45", "46", "47", "48", "49", "50",
        "51", "52", "53", "54", "55", "56", "57", "58", "59",
    ];

    TimeLine = document.getElementById('selecttimeTemp').value;
    for (var i = 0; i < 24; i++) {
        for (var j = 0; j < 60; j++) {
            if (min[j] == "00") {
                timeXaxis[timeindex] = hour[i].substring(0, 2);
            }
            else {
                timeXaxis[timeindex] = hour[i] + min[j];
            }
            time[timeindex] = hour[i] + min[j];
            timeindex++;
        }
    }
    for (var i = 0; i < dataLine.length; i++) {
        chuoiso[i] = Number(dataLine[i].DateTime.replace(/[^0-9]/g, ''));
        timemin[i] = new Date(chuoiso[i]).toLocaleTimeString('en-IE').substring(0, 5);
    }
    for (var i = 0; i < time.length; i++) {
        if (timemin.indexOf(time[i]) > -1) {
            arrfinirr[i] = dataLine[i - indexnull].Irradiation;
        }
        else {
            arrfinirr[i] = null;
            indexnull++;
        }
        irrvalue.push(arrfinirr[i])
      //  hours[i] = Math.floor(i / 60);
        hours[i] = timeXaxis[i];
    }
    chartIrradian = Highcharts.chart('container_irradian', {

        title: {
            text: 'Irradiation'
        },
        exporting: {
            enabled: false
        },

        yAxis: {
            title: {
                text: 'W/m2'
            },
            min: 0,
            max: 1300,
            tickInterval: 100,
        },

        xAxis: {
            tickInterval: 60,
            categories: hours
        },

        credits: {
            enabled: false
        },
        legend: {
            position: 'bottom',
            display: false
        },

        plotOptions: {
            series: {
                label: {
                    connectorAllowed: true
                },

            }
        },
        //tooltip: {
        //    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
        //    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
        //        '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
        //    footerFormat: '</table>',
        //    shared: true,
        //    useHTML: true
        //},
        series: [{
            name: '',
            data: irrvalue,
            color: '#228b22',
            tooltip: {
                valueSuffix: ' W/m2'
            },
            legendName: 'Inrradiation'
        }],
        legend: {
            labelFormatter: function () {
                return this.userOptions.legendName || this.name;
            }
        },
        responsive: {
            rules: [{
                condition: {
                    maxWidth: 500
                },
                chartOptions: {
                    legend: {
                        layout: 'horizontal',
                        align: 'center',
                        verticalAlign: 'bottom'
                    }
                }
            }]
        }

    });
}

//Draw chart for Cell1 Temperature
async function getData(ProjectName) {
    var link = "/DataProject/GetProjectWeather?"
    var url = link + new URLSearchParams({
        starttime: Time,
        endtime: Time,
        ProjectName: ProjectName
    });

    var options = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },
    };
    const response = await fetch(url, options);
    const dataLine = response.json();
    return dataLine;
}


function updateChartCell1(dataLine) {
    var chuoiso = [];
    var time = [];
    var timemin = [];
    var hours = [];
    var tempvalue = [];
    var timeindex = 0;
    var indexnull = 0;
    var arrfintemp = [];
    var timeXaxis = [];

    var hour = ["00:", "01:", "02:", "03:", "04:", "05:", "06:", "07:", "08:", "09:", "10:", "11:", "12:", "13:", "14:", "15:", "16:", "17:", "18:", "19:", "20:", "21:", "22:", "23:"];
    var min = ["00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10",
        "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
        "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
        "31", "32", "33", "34", "35", "36", "37", "38", "39", "40",
        "41", "42", "43", "44", "45", "46", "47", "48", "49", "50",
        "51", "52", "53", "54", "55", "56", "57", "58", "59",
    ];
    for (var i = 0; i < 24; i++) {
        for (var j = 0; j < 60; j++) {
            if (min[j] == "00") {
                timeXaxis[timeindex] = hour[i].substring(0, 2);
            }
            else {
                timeXaxis[timeindex] = hour[i] + min[j];
            }
            time[timeindex] = hour[i] + min[j];
            timeindex++;
        }
    }
    for (var i = 0; i < dataLine.length; i++) {
        chuoiso[i] = Number(dataLine[i].DateTime.replace(/[^0-9]/g, ''));
        timemin[i] = new Date(chuoiso[i]).toLocaleTimeString('en-IE').substring(0, 5);
    }
    for (var i = 0; i < time.length; i++) {
        if (timemin.indexOf(time[i]) > -1) {
            arrfintemp[i] = dataLine[i - indexnull].Cell1Temperature;
        }
        else {
            arrfintemp[i] = null;

            indexnull++;
        }
        tempvalue.push(arrfintemp[i])
       // hours[i] = Math.floor(i / 60);
        hours[i] = timeXaxis[i];
    }
    chartTempCell1 =  Highcharts.chart('container_cell1', {

        title: {
            text: 'PV Module Temperature 1'
        },
        exporting: {
            enabled: false
        },
        yAxis: {
            title: {
                text: '℃'
            },
            min: 0,
            max: 100,
            tickInterval: 20,
        },

        xAxis: {
            //type:'datetime',
            tickInterval: 60,
            categories: hours
        },

        credits: {
            enabled: false
        },
        legend: {
            position: 'bottom',
            display: false
        },

        plotOptions: {
            series: {
                label: {
                    connectorAllowed: true
                },

            }
        },
        //tooltip: {
        //    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
        //    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
        //        '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
        //    footerFormat: '</table>',
        //    shared: true,
        //    useHTML: true
        //},
        series: [{
            name: '',
            data: tempvalue,
            color: '#228b22',
            tooltip: {
                valueSuffix: ' ℃'
            },
            legendName: 'PV Module Temperature 1'
        }],
        legend: {
            labelFormatter: function () {
                return this.userOptions.legendName || this.name;
            }
        },
        responsive: {
            rules: [{
                condition: {
                    maxWidth: 500
                },
                chartOptions: {
                    legend: {
                        layout: 'horizontal',
                        align: 'center',
                        verticalAlign: 'bottom'
                    }
                }
            }]
        }

    });
}

//Draw chart for Cell2 Temperature
async function getData(ProjectName) {
    var link = "/DataProject/GetProjectWeather?"
    var url = link + new URLSearchParams({
        starttime: Time,
        endtime: Time,
        ProjectName: ProjectName
    });


    var options = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },
    };
    const response = await fetch(url, options);
    const dataLine = response.json();
    return dataLine;
}


function updateChartCell2(dataLine) {
    var chuoiso = [];
    var time = [];
    var timemin = [];
    var hours = [];
    var tempvalue = [];
    var timeindex = 0;
    var indexnull = 0;
    var arrfintemp = [];
    var timeXaxis = [];

    var hour = ["00:", "01:", "02:", "03:", "04:", "05:", "06:", "07:", "08:", "09:", "10:", "11:", "12:", "13:", "14:", "15:", "16:", "17:", "18:", "19:", "20:", "21:", "22:", "23:"];
    var min = ["00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10",
        "11", "12", "13", "14", "15", "16", "17", "18", "19", "20",
        "21", "22", "23", "24", "25", "26", "27", "28", "29", "30",
        "31", "32", "33", "34", "35", "36", "37", "38", "39", "40",
        "41", "42", "43", "44", "45", "46", "47", "48", "49", "50",
        "51", "52", "53", "54", "55", "56", "57", "58", "59",
    ];
    for (var i = 0; i < 24; i++) {
        for (var j = 0; j < 60; j++) {
            if (min[j] == "00") {
                timeXaxis[timeindex] = hour[i].substring(0, 2);
            }
            else {
                timeXaxis[timeindex] = hour[i] + min[j];
            }
            time[timeindex] = hour[i] + min[j];
            timeindex++;
        }
    }
    for (var i = 0; i < dataLine.length; i++) {
        chuoiso[i] = Number(dataLine[i].DateTime.replace(/[^0-9]/g, ''));
        timemin[i] = new Date(chuoiso[i]).toLocaleTimeString('en-IE').substring(0, 5);
    }
    for (var i = 0; i < time.length; i++) {
        if (timemin.indexOf(time[i]) > -1) {
            arrfintemp[i] = dataLine[i - indexnull].Cell1Temperature;
        }
        else {
            arrfintemp[i] = null;

            indexnull++;
        }
        tempvalue.push(arrfintemp[i])
       // hours[i] = Math.floor(i / 60);
        hours[i] = timeXaxis[i];
    }
    chartTempCell2 = Highcharts.chart('container_cell2', {

        title: {
            text: 'PV Module Temperature 2'
        },
        exporting: {
            enabled: false
        },
        credits: {
            enabled: false
        },
        yAxis: {
            title: {
                text: '℃'
            },
            min: 0,
            max: 100,
            tickInterval: 20,
        },

        xAxis: {
            //type:'datetime',
            tickInterval: 60,
            categories: hours
        },

        legend: {
            position: 'bottom',
            display: false
        },

        plotOptions: {
            series: {
                label: {
                    connectorAllowed: true
                },

            }
        },
        //tooltip: {
        //    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
        //    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
        //        '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
        //    footerFormat: '</table>',
        //    shared: true,
        //    useHTML: true
        //},
        series: [{
            name: '',
            data: tempvalue,
            color: '#228b22',
            tooltip: {
                valueSuffix: ' ℃'
            },
            legendName: 'PV Module Temperature 2'
        }],
        legend: {
            labelFormatter: function () {
                return this.userOptions.legendName || this.name;
            }
        },
        responsive: {
            rules: [{
                condition: {
                    maxWidth: 500
                },
                chartOptions: {
                    legend: {
                        layout: 'horizontal',
                        align: 'center',
                        verticalAlign: 'bottom'
                    }
                }
            }]
        }

    });
}


//Export Excel data of  Temperature
async function ExportExcelTemp(ProjectName) {
    TimeLine = document.getElementById('selecttimeTemp').value;
    var link = "/DataProject/GetdataExportProjectTemp?"
    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            starttime: TimeLine,
            endtime: TimeLine,
            ProjectName: ProjectName
        }),
        cache: false,
        success: function (data) {
            window.location = "/DataProject/DownloadAmbientTemperature";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}


//Export CSV data of  Temperature
async function ExportCSVTemp(ProjectName) {
    TimeLine = document.getElementById('selecttimeTemp').value;
    var link = "/DataProject/GetdataExportCSVProjectTemp?"
    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            starttime: TimeLine,
            endtime: TimeLine,
            ProjectName: ProjectName
        }),
        cache: false,
        success: function (data) {
            window.location = "/DataProject/DownloadAmbientTemperature";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}

//Export Excel data of  Irradiation
async function ExportExcelIrradian(ProjectName) {
    TimeLine = document.getElementById('selecttimeIrr').value;
    var link = "/DataProject/GetdataExportProjectIrradian?";
    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            starttime: TimeLine,
            endtime: TimeLine,
            ProjectName: ProjectName
        }),
        cache: false,
        success: function (data) {
            window.location = "/DataProject/DownloadIrradiation";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}

//Export CSV data of  Irradiation
async function ExportCSVIrradian(ProjectName) {
    TimeLine = document.getElementById('selecttimeIrr').value;
    var link = "/DataProject/GetdataExportCSVProjectIrradian?";
    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            starttime: TimeLine,
            endtime: TimeLine,
            ProjectName: ProjectName
        }),
        cache: false,
        success: function (data) {
            window.location = "/DataProject/DownloadIrradiation";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}

//Export Excel data of Cell 1 Temperature
async function ExportExcelCell1(ProjectName) {
    TimeLine = document.getElementById('selecttimeCell1').value;
    var link = "/DataProject/GetdataExportProjectCell1?"
    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            starttime: TimeLine,
            endtime: TimeLine,
            ProjectName: ProjectName
        }),
        cache: false,
        success: function (data) {
            window.location = "/DataProject/DownloadCell1Temperature";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}

//Export CSV data of Cell 1 Temperature
async function ExportCSVCell1(ProjectName) {
    TimeLine = document.getElementById('selecttimeCell1').value;
    var link = "/DataProject/GetdataExportCSVProjectCell1?"
    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            starttime: TimeLine,
            endtime: TimeLine,
            ProjectName: ProjectName
        }),
        cache: false,
        success: function (data) {
            window.location = "/DataProject/DownloadCell1Temperature";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}

//Export Excel data of Cell 2 Temperature
async function ExportExcelCell2(ProjectName) {
    TimeLine = document.getElementById('selecttimeCell2').value;
    var link = "/DataProject/GetdataExportProjectCell2?"
    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            starttime: TimeLine,
            endtime: TimeLine,
            ProjectName: ProjectName
        }),
        cache: false,
        success: function (data) {
            window.location = "/DataProject/DownloadCell2Temperature";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}

//Export CSV data of Cell 2 Temperature
async function ExportCSVCell2(ProjectName) {
    TimeLine = document.getElementById('selecttimeCell2').value;
    var link = "/DataProject/GetdataExportCSVProjectCell2?"
    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            starttime: TimeLine,
            endtime: TimeLine,
            ProjectName: ProjectName
        }),
        cache: false,
        success: function (data) {
            window.location = "/DataProject/DownloadCell2Temperature";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}