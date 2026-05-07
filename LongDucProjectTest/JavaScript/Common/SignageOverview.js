
var stringStartHour;
var stringEndHour;

var timeselect, stringStartTime, stringEndTime; //thời gian dùng cho totalchart
//var timeselectDaily, StartTimeDaily, EndTimeDaily; //thời gian dùng cho DailyChart
var timeselectMonthly, StartTimeMonthly, EndTimeMonthly; //thời gian dùng cho MonthlyChart
var timeselectYearly, StartTimeYearly, EndTimeYearly; //thời gian dùng cho YearlyChart
var timeselectTenYear, StartTimeTenYear, EndTimeYearly; //thời gian dùng cho YearlyChart
var TimeUnit; // biến để chọn bảng proc
var selectProc;
var activebtnLine;
var monthstring;
var chartEnergy = null;
var chartPower = null;
let timeMonth = [];
var recentYears = [];
var charttotal;

//hàm get thời gian hiện tại
function DateTime() {
    const d = new Date();
    let day = String(d.getDate()).padStart(2, '0');
    var month = String((d.getMonth() + 1)).padStart(2, '0');
    // if (month < 10) month = '0' + month;
    let year = d.getFullYear();
    return { day, month, year }
}


function LoadProject(ProjectName) {
    TimeUnit = 1; // mặc định sẽ là get data của ngày
    stringStartHour = " 00:00:00";
    stringEndHour = " 23:59:59";
    // mặc định ở ô hiển thị thời gian sẽ là ngày hiện tại
    var x = new DateTime();

    //ô thời gian của biểu đồ total
    document.getElementById("selectimeTotal").type = "text";
    document.getElementById("selectimeTotal").placeholder = "YYYY-MM-DD";
    document.getElementById("selectimeTotal").value = `${x.year}-${x.month}-${x.day}`;

    //ô thời gian của biểu đồ Monthly
    //  document.getElementById("selecttimeMonthly").type = "month";
    document.getElementById("selecttimeMonthly").type = "text";
    document.getElementById("selecttimeMonthly").placeholder = "YYYY-MM-DD";
    document.getElementById("selecttimeMonthly").value = `${x.year}-${x.month}`;

    //ô thời gian của biểu đồ Yearly
    document.getElementById("selecttimeYearly").type = "number";
    document.getElementById("selecttimeYearly").placeholder = "YYYY";
    document.getElementById("selecttimeYearly").value = `${x.year}`;

    SubmitTotalChart(ProjectName);
    SubmitMonthlyChart(ProjectName);
    SubmitYearlyChart(ProjectName);
    SubmitTenYearChart(ProjectName)
    setInterval(() => longPoliingEnergy(ProjectName), 3000000); //sau 30' update 1 lần

    // Cập nhật dữ liệu mỗi phút (600000 milliseconds = 60 phút)
    setInterval(() => {

        var currentDate = new Date();
        var currentDay = currentDate.getDate();
        if (currentDay !== x.day) {  // Nếu ngày đã thay đổi
            x = new DateTime();  // Cập nhật lại thời gian hiện tại
            document.getElementById("selectimeTotal").value = `${x.year}-${x.month}-${x.day}`;
            document.getElementById("selecttimeMonthly").value = `${x.year}-${x.month}`;
            document.getElementById("selecttimeYearly").value = `${x.year}`;
            // Cập nhật lại thời gian cho các biểu đồ
            SubmitTotalChart(ProjectName);
            SubmitMonthlyChart(ProjectName);
            SubmitYearlyChart(ProjectName);
            SubmitTenYearChart(ProjectName);
        }
        longPoliingEnergy(ProjectName);
        SubmitTotalChart(ProjectName);
        SubmitMonthlyChart(ProjectName);
        SubmitYearlyChart(ProjectName);
        SubmitTenYearChart(ProjectName);
    }, 600000); // 60000 milliseconds = 60 phút

}
//Function get data for total chart
function SubmitTotalChart(ProjectName) {
    setTimeout(function () {
        timeselect = document.getElementById('selectimeTotal').value;
        stringStartTime = timeselect + stringStartHour;
        stringEndTime = timeselect + stringEndHour;
        longPoliingEnergy(ProjectName);
    }, 5000);

}


//Function get data for Monthly chart
function SubmitMonthlyChart(ProjectName) {
    timeselectMonthly = document.getElementById('selecttimeMonthly').value;
    var month = timeselectMonthly.substring(5, 7);
    var year = timeselectMonthly.substring(0, 4);
    var day;
    if ((month == "01") || (month == "03") || (month == "05") || (month == "07") || (month == "08") || (month == "10") || (month == "12")) {
        day = "31";
        timeMonth = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17",
            "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30", "31"];
    }
    else if (month == "02") {
        if ((year % 4) == 0) {
            day = 29;
            timeMonth = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15",
                "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29"];
        }
        else {
            day = 28;
            timeMonth = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15",
                "16", "17", "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28"];
        }
    }
    else {
        day = 30;
        timeMonth = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17",
            "18", "19", "20", "21", "22", "23", "24", "25", "26", "27", "28", "29", "30"];
    }

    StartTimeMonthly = `${year}-${month}-01` + stringStartHour;
    EndTimeMonthly = `${year}-${month}-${day}` + stringEndHour;
    longPoliingEnergyMonthly(ProjectName);
}

//Function get data for Yearly chart
function SubmitYearlyChart(ProjectName) {
    timeselectYearly = document.getElementById('selecttimeYearly').value;
    var year = timeselectYearly.substring(0, 4);

    StartTimeYearly = `${year}-01-01` + stringStartHour;
    EndTimeYearly = `${year}-12-31` + stringEndHour;
    longPoliingEnergyYearly(ProjectName);
}

//Function get data for 10 year chart
function SubmitTenYearChart(ProjectName) {
    year = 2023;
    for (i = 0; i < 10; i++) {
        recentYears[i] = (year + i).toString();
    }
    var yearvalue = year - 10;
    StartTimeTenYear = `2023-01-01` + stringStartHour;
    EndTimeTenYear = `2033-12-31` + stringEndHour;
    longPoliingEnergyTenYear(ProjectName);
}


//Hàm update dữ liệu
async function longPoliingEnergy(ProjectName) {
    var data = await getData(ProjectName);
    updateChart(data);
}

//async function longPoliingEnergyDaily(ProjectName) {
//    var data = await getDataDaily(ProjectName);
//    updateChartDaily(data);
//}

async function longPoliingEnergyMonthly(ProjectName) {
    var data = await getDataMonthly(ProjectName);
    updateChartMonthly(data);
}
async function longPoliingEnergyYearly(ProjectName) {
    var data = await getDataYearly(ProjectName);
    updateChartYearly(data);
}
async function longPoliingEnergyTenYear(ProjectName) {
    var data = await getDataTenYear(ProjectName);
    updateChartTenYear(data);
}
//gửi request data đến controller
async function getData(ProjectName) {

    var link = "/DataSignage/GetProjectDataForSignage?";
    var url = link + new URLSearchParams({
        ProjectName: ProjectName,
        timeUnit: 3,
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

//async function getDataDaily(ProjectName) {

//    var link = "/DataSignage/GetProjectDailySelfConsumption?";
//    var url = link + new URLSearchParams({
//        ProjectName: ProjectName,
//        selectProc: 1,
//        starttime: StartTimeDaily,
//        endtime: EndTimeDaily
//    });
//    var options = {
//        method: 'GET',
//        headers: {
//            'Content-Type': 'application/json'
//        },
//    };
//    const response = await fetch(url, options);
//    const data = response.json();
//    return data;
//}

async function getDataMonthly(ProjectName) {

    var link = "/DataSignage/GetProjectMonthlySelfConsumption?";
    var url = link + new URLSearchParams({
        ProjectName: ProjectName,
        selectProc: 2,
        starttime: StartTimeMonthly,
        endtime: EndTimeMonthly
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

async function getDataYearly(ProjectName) {

    var link = "/DataSignage/GetProjectYearlySelfConsumption?";
    var url = link + new URLSearchParams({
        ProjectName: ProjectName,
        selectProc: 3,
        starttime: StartTimeYearly,
        endtime: EndTimeYearly
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

async function getDataTenYear(ProjectName) {

    var link = "/DataSignage/GetProjectTenYearSelfConsumption?";
    var url = link + new URLSearchParams({
        ProjectName: ProjectName,
        selectProc: 4,
        starttime: StartTimeTenYear,
        endtime: EndTimeTenYear
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

// function update dữ liệu vẽ biểu đồ cột


function updateChart(data) {
    var timehour = ["00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12", "13", "14", "15", "16", "17", "18", "19", "20", "21", "22", "23"];
    var chuoiso = []
    var time = []
    var names = []
    var solarpower = []
    var self = [];
/*    var purchased = [];*/
    var indexnull = 0;
    var solarpowervalue = [];
    var selfvalue = [];
/*    var purchasedvalue = [];*/


    for (var i = 0; i < data.length; i++) {
        chuoiso[i] = Number(data[i].DateTime.replace(/[^0-9]/g, ''));
        time[i] = new Date(chuoiso[i]).toString('en-IE').substring(16, 18);
    }
    for (var i = 0; i < 24; i++) {
        if (time.indexOf(timehour[i]) > -1) {
            solarpowervalue[i] = data[i - indexnull].SolarPower;
            selfvalue[i] = data[i - indexnull].SelfConsumption;
/*            purchasedvalue[i] = data[i - indexnull].PurchasedPower;*/
        }
        else {
            solarpowervalue[i] = 0;
            selfvalue[i] = 0;
/*            purchasedvalue[i] = 0;*/
            indexnull++;
        }
        names.push(timehour[i]);
        solarpower.push(solarpowervalue[i]);
        self.push(selfvalue[i]);
/*        purchased.push(purchasedvalue[i]);*/
    }

    if (!charttotal) {


        Highcharts.chart('container', {
            chart: {
                type: 'column'
            },
            title: {
                text: ''
            },
            exporting: {
                enabled: false
            },
            credits: {
                enabled: false
            },
            xAxis: {
                categories: ['00:00', '01:00', '02:00', '03:00', '04:00', '05:00', '06:00', '07:00', '08:00', '09:00', '10:00', '11:00', '12:00',
                    '13:00', '14:00', '15:00', '16:00', '17:00', '18:00', '19:00', '20:00', '21:00', '22:00', '23:00']
            },
            yAxis: [{
                allowDecimals: false,
                min: 0,
                title: {
                    text: 'kWh'
                }
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
                    stacking: null
                }
            },
            series: [{
                name: 'Solar Energy',
                color: 'blue',
                data: solarpower,
                stack: 'solar'
            }, {
                name: 'Self Consumption',
                color: 'red',
                data: self,
                stack: 'grid'
                }]


            //            series: [{
            //    name: 'Solar Energy',
            //    color: 'blue',
            //    data: solarpower,
            //    stack: 'solar'
            //}, {
            //    name: 'Purchased Energy',
            //    color: '#32CD32',
            //    data: purchased,
            //    stack: 'grid'
            //}, {
            //    name: 'Self Consumption',
            //    color: 'red',
            //    data: self,
            //    stack: 'grid'
            //}]

        });
    }
    else {
        for (var i = 0; i < dailyself.length; i++) {
            chart.series[0].data[i].update(dailyself[i], false);
        }
        chart.redraw();
    }
}



function updateChartMonthly(data) {
    var chuoiso = [];
    var time = [];
    var names = [];
    var dailyself = [];
    var indexnull = 0;
    var dailyselfvalue = [];

    for (var i = 0; i < data.length; i++) {
        chuoiso[i] = Number(data[i].DateTime.replace(/[^0-9]/g, ''));
        time[i] = new Date(chuoiso[i]).toString('en-IE').substring(8, 10);
    }
    for (var i = 0; i < timeMonth.length; i++) {
        if (time.indexOf(timeMonth[i]) > -1) {
            dailyselfvalue[i] = data[i - indexnull].SelfConsumption;
        } else {
            dailyselfvalue[i] = 0;
            indexnull++;
        }
        names.push(timeMonth[i]);
        dailyself.push(dailyselfvalue[i]);
    }

    //var max = Math.ceil(Math.max(...dailyself) / 1000) * 1000; // Giá trị tối đa (làm tròn lên đến hàng nghìn)
    //var tickInterval = max / 3; // Khoảng cách giữa các đường kẻ trên trục Y (chia thành 4 phần)

    Highcharts.chart('container_monthlyself', {
        chart: {
            type: 'column',
            plotBorderWidth: 0 // Bỏ viền khung phần chart area
        },
        credits: {
            enabled: false
        },
        exporting: {
            enabled: false
        },
        title: null,
        xAxis: {
            categories: names,
            labels: {
                style: {
                    fontSize: '10px' // Điều chỉnh kích thước font theo ý muốn
                }
            },
            tickInterval: 1
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

        legend: {
            enabled: false // Tắt chú thích
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormatter: function () {
                return '<tr><td style="color:' + this.series.color + ';padding:0">Value:  </td>' +
                    '<td style="padding:0"><b>' + Math.round(this.y).toLocaleString() + '</b></td></tr>';
            },
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },
        plotOptions: {
            column: {
                grouping: false,
                shadow: false,
                borderWidth: 0
            }
        },
        series: [{
            color: 'blue', // Mã màu cam
            data: dailyself,
            pointPadding: 0.3,
        }]
    });
}


function updateChartYearly(data) {
    var timeyear = ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"];
    var chuoiso = [];
    var time = [];
    var names = [];
    var yearlyself = [];
    var indexnull = 0;
    var yearselfvalue = [];

    for (var i = 0; i < data.length; i++) {
        chuoiso[i] = Number(data[i].DateTime.replace(/[^0-9]/g, ''));
        time[i] = new Date(chuoiso[i]).toLocaleDateString("en-GB").substring(3, 5);
    }
    for (var i = 0; i < 12; i++) {
        if (time.indexOf(timeyear[i]) > -1) {
            yearselfvalue[i] = data[i - indexnull].SelfConsumption;
        } else {
            yearselfvalue[i] = 0;
            indexnull++;
        }
        names.push(timeyear[i]);
        yearlyself.push(yearselfvalue[i]);
    }

/*     Tính toán giá trị max và tickInterval dựa trên dữ liệu*/
    //var max = Math.ceil(Math.max(...yearlyself) / 300000) * 300000; // Giá trị tối đa (làm tròn lên đến hàng nghìn)
    //var tickInterval = max / 3; // Khoảng cách giữa các đường kẻ trên trục Y (chia thành 4 phần)

    Highcharts.chart('container_yearlyself', {
        chart: {
            type: 'column',
            plotBorderWidth: 0 // Bỏ viền khung phần chart area
        },
        credits: {
            enabled: false
        },
        exporting: {
            enabled: false
        },
        title: null,
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
        /*    tickInterval: tickInterval,*/
            //min: 0,
            //max: 360000,
            //tickInterval: 120000,
        }],
        legend: {
            enabled: false // Ẩn chú thích
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormatter: function () {
                return '<tr><td style="color:' + this.series.color + ';padding:0">Value:  </td>' +
                    '<td style="padding:0"><b>' + Math.round(this.y).toLocaleString() + '</b></td></tr>';
            },
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },
        plotOptions: {
            column: {
                grouping: false,
                shadow: false,
                borderWidth: 0
            }
        },
        series: [{
            name: null, // hoặc name: '', để không hiển thị tên series
            color: 'blue', // Mã màu cam
            data: yearlyself,
            pointPadding: 0.2,
        }],
        // Đặt marginBottom là 0 để tận dụng hết không gian đáy biểu đồ
        marginBottom: 0
    });
}


function updateChartTenYear(data) {
    var chuoiso = [];
    var time = [];
    var names = [];
    var yearlyself = [];
    var indexnull = 0;
    var yearselfvalue = [];

    for (var i = 0; i < data.length; i++) {
        chuoiso[i] = Number(data[i].DateTime.replace(/[^0-9]/g, ''));
        time[i] = new Date(chuoiso[i]).toLocaleDateString("en-GB").substring(6, 10);
    }
    for (var i = 0; i < 10; i++) {
        if (time.indexOf(recentYears[i]) > -1) {
            yearselfvalue[i] = data[i - indexnull].SelfConsumption;
        } else {
            yearselfvalue[i] = 0;
            indexnull++;
        }
        names.push(recentYears[i]);
        yearlyself.push(yearselfvalue[i]);
    }

    // Tính toán giá trị max và tickInterval dựa trên dữ liệu
    //var max = Math.ceil(Math.max(...yearlyself) / 300000) * 300000; // Giá trị tối đa (làm tròn lên đến hàng nghìn)
    //var tickInterval = max / 3; // Khoảng cách giữa các đường kẻ trên trục Y (chia thành 4 phần)

    Highcharts.chart('container_tenyear', {
        chart: {
            type: 'column',
            plotBorderWidth: 0 // Bỏ viền khung phần chart area
        },
        credits: {
            enabled: false
        },
        exporting: {
            enabled: false
        },
        title: null,
        xAxis: {
            categories: names
        },
        //yAxis: [{
        //    min: 0,
        //    max: max,
        //    tickInterval: tickInterval,
        //    title: {
        //        text: 'kWh'
        //    },
        //    labels: {
        //        format: '{value}',
        //        formatter: function () {
        //            return Math.round(this.value).toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","); // Làm tròn, thêm dấu ',' và 'k' sau giá trị số
        //        }
        //    },
        //}],

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
            //min: 0,
            //max: 900000,
            //tickInterval: 300000,
/*            tickInterval: tickInterval,*/
        }],
        legend: {
            enabled: false // Ẩn chú thích
        },
        tooltip: {
            headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
            pointFormatter: function () {
                return '<tr><td style="color:' + this.series.color + ';padding:0">Value:  </td>' +
                    '<td style="padding:0"><b>' + Math.round(this.y).toLocaleString() + '</b></td></tr>';
            },
            footerFormat: '</table>',
            shared: true,
            useHTML: true
        },
        plotOptions: {
            column: {
                grouping: false,
                shadow: false,
                borderWidth: 0
            }
        },
        series: [{
            name: null, // hoặc name: '', để không hiển thị tên series
            color: 'blue', // Mã màu cam
            data: yearlyself,
            pointPadding: 0.2,
        }],
        // Đặt marginBottom là 0 để tận dụng hết không gian đáy biểu đồ
        marginBottom: 0
    });

}
