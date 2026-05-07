
function DateTime() {
    const d = new Date();
    let day = String(d.getDate()).padStart(2, '0');
    let month = d.getMonth() + 1;
    if (month < 10) month = '0' + month;
    let year = d.getFullYear();
    return { day, month, year }
}

async function Load() {
    var x = new DateTime();
    document.getElementsByName("starttime")[0].value = `${x.year}-${x.month}-${x.day}`;
    document.getElementsByName("endtime")[0].value = `${x.year}-${x.month}-${x.day}`;
    var startTime = `${x.year}-${x.month}-${x.day}` + " 00:00:00";
    var endTime = `${x.year}-${x.month}-${x.day}` + " 23:59:59";
    

    // Xoa tat ca caca hang trong table
    var table = document.getElementById("alarmTable");
    for (var i = 1; i < table.rows.length;) {
        table.deleteRow(i);
    };

    // Lay event Log
    var eventList = await getData(startTime, endTime);


    //Add row
    for (const event of eventList) {
        // Tao moi mot hang
        const row = document.createElement('tr');
        // Tao cot
        for (const val of Object.values(event)) {
            const col = document.createElement('td');
            col.textContent = val;
            col.style.textAlign = "center";
            row.appendChild(col);
            row.style.backgroundColor = "white";
            row.style.color = "black";
            row.style.fontweight = "bold";
        }
        table.appendChild(row);
    }
}

async function Submit() {
    var startTime = document.getElementsByName("starttime")[0].value + " 00:00:00";
    var endTime = document.getElementsByName("endtime")[0].value + " 23:59:59";

    // Xoa tat ca caca hang trong table
    var table = document.getElementById("alarmTable");
    for (var i = 1; i < table.rows.length;) {
        table.deleteRow(i);
    };

    // Lay event Log
    var eventList = await getData(startTime, endTime);


    //Add row
    for (const event of eventList) {
        // Tao moi mot hang
        const row = document.createElement('tr');
        // Tao cot
        for (const val of Object.values(event)) {
            const col = document.createElement('td');
            col.textContent = val;
            col.style.textAlign = "center";
            row.appendChild(col);
            row.style.backgroundColor = "white";
            row.style.color = "black";
            row.style.fontweight = "bold";
        }
        table.appendChild(row);
    }
}

async function getData(startTime, endTime) {
    var link = "/Event/GetEventLog?"
    var url = link + new URLSearchParams({
        starttime: startTime,
        endtime: endTime 
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
async function ExportExcelEvent() {
    var startTime = document.getElementsByName("starttime")[0].value + " 00:00:00";
    var endTime = document.getElementsByName("endtime")[0].value + " 23:59:59";
    var link = "/Event/GetdataExportEvent?"


    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            starttime: startTime,
            endtime: endTime 
        }),
        cache: false,
        success: function (data) {
            window.location = "/Event/Download";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}

//Xuất dữ liệu alarm ra file CSV
async function ExportCSVEvent() {
    var startTime = document.getElementsByName("starttime")[0].value + " 00:00:00";
    var endTime = document.getElementsByName("endtime")[0].value + " 23:59:59";
    var link = "/Event/GetdataCSVExportEvent?"


    $.ajax({
        type: "POST",
        url: link + new URLSearchParams({
            starttime: startTime,
            endtime: endTime
        }),
        cache: false,
        success: function (data) {
            window.location = "/Event/Download";
        },
        error: function (data) {
            Materialize.toast("Something went wrong.", 3000, 'rounded');
        }
    });
}
