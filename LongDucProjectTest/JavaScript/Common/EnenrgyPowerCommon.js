
var stringStartHour;
var stringEndHour;

var timeselect; //thời gian dùng cho biểu đồ cột
var stringStartTime;
var stringEndTime;
var TimeUnit; // biến để chọn bảng proc
var TimeLine; // thời gian dùng cho biểu đồ đường
var activebtnLine;
var monthstring;
var chartEnergy = null;
var chartPower = null;
async function longPoliing() {

    var data = await getData();
    updateChart(data);

    setTimeout(longPoliing, 150000000);
}

async function getData() {

    var link = "/Overview/GetCommonSolarEnergy?";
    var url = link + new URLSearchParams({
        timeUnit: TimeUnit,
        starttime: stringStartTime,
        endtime: stringEndTime
    });
    var options = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },
    };
    const response = await fetch(url, options);
    const data = response.json();
    return data;
}
//hàm get thời gian hiện tại
function DateTime() {
    const d = new Date();
    let day = String(d.getDate()).padStart(2, '0');
    let month = d.getMonth() + 1;
    if (month < 10) month = '0' + month;
    let year = d.getFullYear();
    return { day, month, year }
}


function Load() {
    TimeUnit = 1; // mặc định sẽ là get data của ngày
    stringStartHour = " 00:00:00";
    stringEndHour = " 23:59:59";
    // mặc định ở ô hiển thị thời gian sẽ là ngày hiện tại


    var x = new DateTime();
    document.getElementById("selectimeEnergy").type = "date";
    document.getElementById("selectimeEnergy").value = `${x.year}-${x.month}-${x.day}`;


    document.getElementById("selecttimePower").type = "date";
    document.getElementById("selecttimePower").value = `${x.year}-${x.month}-${x.day}`;

    document.getElementById("day").style.backgroundColor = "dodgerblue";
    document.getElementById("month").style.backgroundColor = "white";
    document.getElementById("year").style.backgroundColor = "white";
    document.getElementById("day").style.color = "white";
    document.getElementById("month").style.color = "black";
    document.getElementById("year").style.color = "black";

    Submit();
    SubmitLine();
}


//Function các phím chức năng.
//khi chọn đơn vị thời gian là ngày
function day() {
    TimeUnit = 1;
    var x = new DateTime();
    document.getElementById("selectimeEnergy").type = "date";
    document.getElementById("selectimeEnergy").value = `${x.year}-${x.month}-${x.day}`;
    document.getElementById("day").style.backgroundColor = "dodgerblue";
    document.getElementById("month").style.backgroundColor = "white";
    document.getElementById("year").style.backgroundColor = "white";
    document.getElementById("day").style.color = "white";
    document.getElementById("month").style.color = "black";
    document.getElementById("year").style.color = "black";
    Submit();
}

// khi chọn đơn vị thời gian là tháng
function month() {
    TimeUnit = 2;
    var x = new DateTime();
    document.getElementById("selectimeEnergy").type = "month";
    document.getElementById("selectimeEnergy").value = `${x.year}-${x.month}`;
    document.getElementById("day").style.backgroundColor = "white";
    document.getElementById("month").style.backgroundColor = "dodgerblue";
    document.getElementById("year").style.backgroundColor = "white";
    document.getElementById("day").style.color = "black";
    document.getElementById("month").style.color = "white";
    document.getElementById("year").style.color = "black";
    Submit();
}

// khi chọn đơn vị thời gian là năm
function year() {
    TimeUnit = 3;
    var x = new DateTime();
    document.getElementById("selectimeEnergy").type = "number";
    document.getElementById("selectimeEnergy").placeholder = "YYYY";
    document.getElementById("selectimeEnergy").value = `${x.year}`;
    document.getElementById("day").style.backgroundColor = "white";
    document.getElementById("month").style.backgroundColor = "white";
    document.getElementById("year").style.backgroundColor = "dodgerblue";
    document.getElementById("day").style.color = "black";
    document.getElementById("month").style.color = "black";
    document.getElementById("year").style.color = "white";
    Submit();
}
function Submit() {
    timeselect = document.getElementById('selectimeEnergy').value;
    switch (TimeUnit) {
        case 1: // nếu chọn đơn vị thời gian là ngày
            stringStartTime = timeselect + stringStartHour;
            stringEndTime = timeselect + stringEndHour;
            break;

        case 2: // nếu chọn đơn vị thời gian là tháng
            monthstring = timeselect.substring(5, 7);
            var yearstring = timeselect.substring(0, 4);
            if ((monthstring == "01") || (monthstring == "03") || (monthstring == "05") || (monthstring == "07") || (monthstring == "08") || (monthstring == "10") || (monthstring == "12")) {
                var daystring = "31";
            }
            else if (monthstring == "02") { daystring = "28"; }
            else
                daystring = "30";
            stringStartTime = `${yearstring}-${monthstring}-01` + stringStartHour;
            stringEndTime = `${yearstring}-${monthstring}-${daystring}` + stringEndHour;
            break;
        case 3:
            var yearstring = timeselect.substring(0, 4);
            stringStartTime = `${yearstring}-01-01` + stringStartHour;
            stringEndTime = `${yearstring}-12-31` + stringEndHour;
            break;
    }
    longPoliing();

}

// function update dữ liệu vẽ biểu đồ cột

function updateChart(data) {
    var timehour = ["00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"];
    var timeMonth = [];
    var timeYear = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"];
    var chuoiso = []
    var time = []
    var names = []
    var solar = []
    var grid = [];
    var indexnull = 0;
    var arrsolarfin = [];
    var arrgridfin = [];


    if (TimeUnit == 1) {

        for (var i = 0; i < data.length; i++) {
            chuoiso[i] = Number(data[i].DateTime.replace(/[^0-9]/g, ''));
            time[i] = new Date(chuoiso[i]).toString('en-IE').substring(16, 18);
        }
        for (var i = 0; i < 24; i++) {
            if (time.indexOf(timehour[i]) > -1) {
                arrsolarfin[i] = data[i - indexnull].SolarValue;
                arrgridfin[i] = data[i - indexnull].GridValue;
            }
            else {
                arrsolarfin[i] = 0;
                arrgridfin[i] = 0;
                indexnull++;
            }
            names.push(timehour[i]);
            solar.push(arrsolarfin[i]);
            grid.push(arrgridfin[i]);
        }

    }
    else if (TimeUnit == 2) {
        if (monthstring == "02") {
            timeMonth = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28"];
        }
        else if ((monthstring == "01") || (monthstring == "03") || (monthstring == "05") || (monthstring == "07") || (monthstring == "08") || (monthstring == "10") || (monthstring == "12")) {
            timeMonth = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31"];
        }
        else if ((monthstring == "04") || (monthstring == "06") || (monthstring == "09") || (monthstring == "11")) {
            timeMonth = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30"];
        }
        for (var i = 0; i < data.length; i++) {
            chuoiso[i] = Number(data[i].DateTime.replace(/[^0-9]/g, ''));
            time[i] = new Date(chuoiso[i]).toString('en-IE').substring(8, 10);
        }
        for (var i = 0; i < timeMonth.length; i++) {
            if (time.indexOf(timeMonth[i]) > -1) {
                arrsolarfin[i] = data[i - indexnull].SolarValue;
                arrgridfin[i] = data[i - indexnull].GridValue;
            }
            else {
                arrsolarfin[i] = 0;
                arrgridfin[i] = 0;
                indexnull++;
            }
            names.push(timeMonth[i]);
            solar.push(arrsolarfin[i]);
            grid.push(arrgridfin[i]);
        }
        /* chartEnergy.yAxis[0].setTitle({ text: "MWh" });*/
    }
    else if (TimeUnit == 3) {
        for (var i = 0; i < data.length; i++) {
            chuoiso[i] = Number(data[i].DateTime.replace(/[^0-9]/g, ''));
            time[i] = new Date(chuoiso[i]).toLocaleDateString("en-GB").substring(3, 5);
        }
        for (var i = 0; i < 12; i++) {
            if (time.indexOf(timeYear[i]) > -1) {
                arrsolarfin[i] = data[i - indexnull].SolarValue;
                arrgridfin[i] = data[i - indexnull].GridValue;
            }
            else {
                arrsolarfin[i] = 0;
                arrgridfin[i] = 0;
                indexnull++;
            }
            names.push(timeYear[i]);
            solar.push(arrsolarfin[i]);
            grid.push(arrgridfin[i]);
        }
        /*chartEnergy.yAxis[0].setTitle({ text: "MWh" });*/
    }



    chartEnergy = Highcharts.chart('container', {
        chart: {
            type: 'column'
        },
        title: {
            text: 'Total Energy'
        },
        exporting: {
            enabled: false
        },
        credits: {
            enabled: false
        },
        subtitle: {
            text: '<a href="https://www.ssb.no/en/statbank/table/08940/" '
        },
        xAxis: {
            categories: names

        },

        yAxis: [{ // Primary yAxis
            labels: {
                format: '{value}',
                formatter: function () {
                    return Math.round(this.value).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","); // Làm tròn, thêm dấu ',' và 'k' sau giá trị số
                }
            },
            title: {
                useHTML: true,
                text: function () {
                    if (TimeUnit == 1) this.text = 'Energy (kWh)';
                    else this.text = 'Energy (kWh)';
                },
            },
        }],

        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormatter: function () {
                return '<tr><td style="color:' + this.series.color + ';padding:0">' + this.series.name + ': </td>' +
                    '<td style="padding:0"><b>' + Math.round(this.y).toLocaleString() + '</b></td></tr>';
            },
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },

        plotOptions: {
            column: {
                pointPadding: 0.2,
                borderWidth: 0
            }
        },

        series: [{

            name: 'Solar Energy',
            data: solar,
            color: 'blue'
        },
        {
            name: 'Self Consumption',
            data: grid,
            color: '#FF0000'
        }

        ]
    });
    if (TimeUnit == 1) {
        chartEnergy.yAxis[0].setTitle({ text: "Energy (kWh)" });
    }
    else {
        chartEnergy.yAxis[0].setTitle({ text: "Energy (kWh)" });
    }
}
//xuất báo cáo biểu đồ Energy
async function ExportExcelColumn() {
    var link = "/Overview/GetdataExportCommonEnergy?"

    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            timeUnit: TimeUnit,
            starttime: stringStartTime,
            endtime: stringEndTime
        }),
        cache: false,
        success: function (data) {
            window.location = "/Overview/Download";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}

//xuất báo cáo biểu đồ Energy file dạng CSV
async function ExportCSVEnergy() {
    var link = "/Overview/GetdataCSVCommonEnergy?"

    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            timeUnit: TimeUnit,
            starttime: stringStartTime,
            endtime: stringEndTime
        }),
        cache: false,
        success: function (data) {
            window.location = "/Overview/Download";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}


//Function update dữ liệu vẽ biểu đồ đường
async function longPoliingLine() {

    var dataLine = await getDataLine();
    updateChartLine(dataLine);

}
async function getDataLine() {
    var link = "/Overview/GetCommonSolarPower?"
    var url = link + new URLSearchParams({
        datetime: TimeLine
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
//funtion khi thay đổi giá trị thời gian để vẽ biểu đồ
function SubmitLine() {
    TimeLine = document.getElementById('selecttimePower').value;
    longPoliingLine();
}



function updateChartLine(dataLine) {
    var chuoiso = [];
    var time = [];
    var timemin = [];
    var hours = []
    var solarvalue = []
    var gridvalue = []
    var timeindex = 0;
    var indexnull = 0;
    var arrfinsolar = [];
    var arrfingrid = [];
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
            arrfinsolar[i] = dataLine[i - indexnull].SolarValue;
            // arrfingrid[i] = dataLine[i - indexnull].GridValue;
        }
        else {
            arrfinsolar[i] = null;
            // arrfingrid[i] = null;
            indexnull++;
        }
        solarvalue.push(arrfinsolar[i])
        //  gridvalue.push(arrfingrid[i]);
        /* hours[i] = Math.floor(i);*/
        hours[i] = timeXaxis[i];
    }

    chartPower = Highcharts.chart('container_line', {
        chart: {
            zoomType: 'xy'
        },
        title: {
            text: 'Total Power'
        },
        exporting: {
            enabled: false
        },
        credits: {
            enabled: false
        },
        xAxis: [{
            tickInterval: 60,
            categories: hours,

        }],
        yAxis: [{ // Primary yAxis
            labels: {
                format: '{value}',
                formatter: function () {
                    return Math.round(this.value).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","); // Làm tròn, thêm dấu ',' và 'k' sau giá trị số
                }
            },
            title: {
                text: 'Power (kW)',
                //style: {
                //    color: Highcharts.getOptions().colors[0]
                //}
            },
            //min: 0,
            //max: 3000,
            /*tickInterval:250,*/
        }],

        tooltip: {
            formatter: function () {
                // Lấy thời gian từ mảng timeXaxis dựa trên chỉ mục this.x
                var time = timeXaxis[this.x]; // Lấy giá trị thời gian từ timeXaxis theo chỉ mục this.x
                return '<b>' + time + '</b><br/>' + // Hiển thị thời gian trên trục X
                    'Value: ' + this.y.toLocaleString() + ' kW'; // Hiển thị giá trị trên trục Y
            }
        },




        series: [{
            showInLegend: true,
            name: '',
            type: 'spline',
            yAxis: 0,
            data: solarvalue,
            color: 'blue',
            tooltip: {
                valueSuffix: ' kW'
            },
            legendName: 'Active Power'

        }],
        legend: {
            labelFormatter: function () {
                return this.userOptions.legendName || this.name;
            }
        }
    });

}
async function ExportExcelLine() {
    var TimePower = document.getElementById('selecttimePower').value;
    var link = "/Overview/GetdataExportCommonPower?"
    var url = link + new URLSearchParams({
    });

    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            datetime: TimePower
        }),
        cache: false,
        success: function (data) {
            window.location = "/Overview/DownloadPowerOverview";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}

//xuất file báo cáo csv cho power
async function ExportCSVPower() {
    var TimePower = document.getElementById('selecttimePower').value;
    var link = "/Overview/GetdataCSVCommonPower?"

    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            datetime: TimePower
        }),
        cache: false,
        success: function (data) {
            window.location = "/Overview/DownloadPowerOverview";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}
