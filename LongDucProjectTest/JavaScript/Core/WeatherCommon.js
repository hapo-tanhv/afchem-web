
function Load(ProjectName) {
    SubmitTemp(ProjectName);
    SubmitIrradian(ProjectName);
}

// Get data and draw chart for Temperature
function SubmitTemp(ProjectName) {
    TimeTemp = document.getElementById('selecttimeTemp').value;
    ProjectName = document.getElementById('ProjectNameTemp').value;
    longPoliingTemp(ProjectName);
}
async function longPoliingTemp(ProjectName) {
    var dataLine = await getData(ProjectName);
    updateChartTemp(dataLine);
}

//Get data and draw chart for Irradian
function SubmitIrradian(ProjectName) {
    TimeIrr = document.getElementById('selecttimeIrr').value;
    ProjectName = document.getElementById('ProjectNameIrr').value;
    longPoliingIrr(ProjectName);
}
async function longPoliingIrr(ProjectName) {
    var dataLine = await getData(ProjectName);
    updateChartIrr(dataLine);
}


async function getData(ProjectName) {
    var link = "https://localhost:44359/DataProject/GetProjectWeather?"
    var url = link + new URLSearchParams({
        starttime: TimeTemp + " 00:00:00",
        endtime: TimeTemp + " 23:59:59",
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
            arrfintemp[i] = 0;

            indexnull++;
        }
        tempvalue.push(arrfintemp[i])
        hours[i] = Math.floor(i / 60);
    }
    Highcharts.chart('container_temp', {

        title: {
            text: 'Temperature Chart'
        },
        credits: {
            enabled: false
        },
        yAxis: {
            title: {
                text: '℃'
            }
        },

        xAxis: {
            //type:'datetime',
            tickInterval: 60,
            categories: hours
        },

        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle'
        },

        plotOptions: {
            series: {
                label: {
                    connectorAllowed: true
                },

            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },
        series: [{
            name: 'Installation & Developers',
            data: tempvalue
        }],

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
            arrfinirr[i] = 0;

            indexnull++;
        }
        irrvalue.push(arrfinirr[i])
        hours[i] = Math.floor(i / 60);
    }
    Highcharts.chart('container_temp', {

        title: {
            text: 'Irradiation'
        },
        credits: {
            enabled: false
        },

        yAxis: {
            title: {
                text: 'W/m2'
            }
        },

        xAxis: {
            tickInterval: 60,
            categories: hours
        },

        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'middle'
        },

        plotOptions: {
            series: {
                label: {
                    connectorAllowed: true
                },

            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                '<td style="padding:0"><b>{point.y:.1f}</b></td></tr>',
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },
        series: [{
            name: 'Inrradiation',
            data: irrvalue
        }],

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
async function ExportExcelTemp() {
    TimeLine = document.getElementById('selecttimeTemp').value;
    ProjectName = document.getElementById('ProjectNameTemp').value;
    var link = "https://localhost:44359/DataProject/GetdataExportProjectTemp?"
    var url = link + new URLSearchParams({
        starttime: TimeLine + " 00:00:00",
        endtime: TimeLine + " 23:59:59",
        ProjectName: ProjectName
    });

    var options = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },

    };
    const response = await fetch(url, options);
}

//Export Excel data of  Irradiation
async function ExportExcelIrradian() {
    TimeLine = document.getElementById('selecttimeIrr').value;
    ProjectName = document.getElementById('ProjectNameIrr').value;
    var link = "https://localhost:44359/DataProject/GetdataExportProjectIrradian?"
    var url = link + new URLSearchParams({
        starttime: TimeLine + " 00:00:00",
        endtime: TimeLine + " 23:59:59",
        ProjectName: ProjectName
    });

    var options = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },

    };
    const response = await fetch(url, options);
}