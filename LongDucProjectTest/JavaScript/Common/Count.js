
setInterval(myTimer, 1000);

function myTimer() {
    if (window.Counter == undefined) {
        clearInterval;
        var x = localStorage.getItem("local");
        document.getElementById("alarmcount").innerHTML = x
        return;
    }
    else setInterval(myTimer, 1000);
    var x = localStorage.getItem("local");
    document.getElementById("alarmcount").innerHTML = x;
}
async function logout() {
    var link = "/Login/Login";
    var url = link;
    var options = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json'
        },
    };
    const response = await fetch(url, options);
   
}