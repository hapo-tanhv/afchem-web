window.Counter = 0;

let count = 0;
var begin; //biển để xác nhận số lượng alarm để hiển thị lúc load trang
function DateTime() {
    const d = new Date();
    let day = String(d.getDate()).padStart(2, '0');
    let month = d.getMonth() + 1;
    if (month < 10) month = '0' + month;
    let year = d.getFullYear();
    return { day, month, year }
}

async function Load(ProjectName) {
    var x = new DateTime();
    document.getElementsByName("starttime")[0].value = `${x.year}-${x.month}-01`;
    document.getElementsByName("endtime")[0].value = `${x.year}-${x.month}-${x.day}`;
    var startTime = `${x.year}-${x.month}-01`; + " 00:00:00";
    var endTime = `${x.year}-${x.month}-${x.day}` + " 23:59:59";
    var Projectname = ProjectName;
    var InverterName = document.getElementById("namefilter").value;
    // Xoa tat ca caca hang trong table
    var table = document.getElementById("alarmTable");
    for (var i = 1; i < table.rows.length;) {
        table.deleteRow(i);
    };

    // Lay alarm Log
    var alarmList = await getData(startTime, endTime, Projectname, InverterName);


    //Add row 
    ////Lúc Load trang thì hiển thị 20 Alarm gần nhất
    var index;

        index = alarmList.length;
    for (var i = 0; i < index; i++) {
        // Tao moi mot hang
        const row = document.createElement('tr');
        // Tao cot
        for (const val of Object.values(alarmList[i])) {
            const col = document.createElement('td');
            col.textContent = val;
            col.style.textAlign = "center";
            row.appendChild(col);
        }
        row.style.color = "black";
        if (alarmList[i]['Status'] == 'Alarm') {
            //row.style.backgroundColor = "red";            
            count = count + 1;
            row.style.backgroundColor = "yellow";
        } else {
            // row.style.backgroundColor = "limegreen";
            row.style.backgroundColor = "white";
        }

        table.appendChild(row);
        row.style.height = "5"
    }
    if (count != "undefined") {
        window.Counter = count;
        localStorage.setItem("local", window.Counter);
    }

}

async function Submit(ProjectName) {
    var count = 0;
    var index = 0;
    var startTime = document.getElementsByName("starttime")[0].value + " 00:00:00";
    var endTime = document.getElementsByName("endtime")[0].value + " 23:59:59";
    var Projectname = ProjectName;
    var InverterName = document.getElementById("namefilter").value;
    // Xoa tat ca caca hang trong table
    var table = document.getElementById("alarmTable");
    for (var i = 1; i < table.rows.length;) {
        table.deleteRow(i);
    };

    // Lay alarm Log
    var alarmList = await getData(startTime, endTime, Projectname, InverterName);




    //Add row

    for (const alarm of alarmList) {
        // Tao moi mot hang
        const row = document.createElement('tr');
        // Tao cot
        for (const val of Object.values(alarm)) {
            index++;
            const col = document.createElement('td');
            col.innerHTML = index;
            col.textContent = val;
            row.appendChild(col);
            row.style.fontweight = "bold";
            /* rows[index].insertBefore(index, rows[i].firstChild);*/
        }
        row.style.color = "black";
        if (alarm['Status'] == 'Alarm') {
            row.style.backgroundColor = "yellow";
            count = count + 1;
        } else {
            row.style.backgroundColor = "white";
        }

        table.appendChild(row);
    }
    if (count != "undefined") {
        window.Counter = count;
        localStorage.setItem("local", window.Counter);
    }
}
async function getData(startTime, endTime, Projectname, InverterName) {
    var link = "/DataProject/GetAlarmProject?"
    var url = link + new URLSearchParams({
        starttime: startTime,
        endtime: endTime,
        projectname: Projectname,
        invertername: InverterName
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
//Xuất dữ liệu alarm ra file Excel
async function ExportExcelAlarm(ProjectName) {
    var startTime = document.getElementsByName("starttime")[0].value + " 00:00:00";
    var endTime = document.getElementsByName("endtime")[0].value + " 23:59:59";
    var Projectname = ProjectName;
    var InverterName = document.getElementById("namefilter").value;
    var link = "/DataProject/GetdataExportAlarm?"


    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            starttime: startTime ,
            endtime: endTime ,
            projectname: Projectname,
            invertername: InverterName
        }),
        cache: false,
        success: function (data) {
            window.location = "/DataProject/Download";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}


//Xuất dữ liệu alarm ra file CSV
async function ExportCSVAlarm(ProjectName) {
    var startTime = document.getElementsByName("starttime")[0].value + " 00:00:00";
    var endTime = document.getElementsByName("endtime")[0].value + " 23:59:59";
    var Projectname = ProjectName;
    var InverterName = document.getElementById("namefilter").value;
    var link = "/DataProject/GetdataCSVExportAlarm?"


    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            starttime: startTime,
            endtime: endTime,
            projectname: Projectname,
            invertername: InverterName
        }),
        cache: false,
        success: function (data) {
            window.location = "/DataProject/Download";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}


//Help
async function Help() {

    window.open("https://manuals.sma.de/STPxxx60/en-US/index.html", "_blank");

}
