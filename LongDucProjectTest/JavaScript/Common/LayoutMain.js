// Machine Dropdown Functions

function toggleMachineDropdown() {

    const container = document.getElementById('machineSelectContainer');

    const options = document.getElementById('machineOptions');

    const selected = container.querySelector('.selected-machine');

    options.classList.toggle('show');

    selected.classList.toggle('active');

}

function selectMachine(machineName) {

    document.getElementById('selectedMachineName').textContent = machineName;

    document.getElementById('machineOptions').classList.remove('show');

    document.querySelector('.selected-machine').classList.remove('active');

    document.querySelectorAll('.machine-option').forEach(opt => {

        opt.classList.remove('selected');

        if (opt.textContent === machineName) {

            opt.classList.add('selected');

        }

    });

    console.log('Loading data for machine:', machineName);

}

// Open Alarm Page

function openAlarmPage() {

    window.location.href = '/Home/Alarm';

}

// Close dropdown when clicking outside

document.addEventListener('click', function(e) {

    const container = document.getElementById('machineSelectContainer');

    if (container && !container.contains(e.target)) {

        const options = document.getElementById('machineOptions');

        const selected = document.querySelector('.selected-machine');

        if (options) options.classList.remove('show');

        if (selected) selected.classList.remove('active');

    }

});

// Update Clock function & Current Shift

function getCurrentShift() {

    const now = new Date();

    const hours = now.getHours();

    const minutes = now.getMinutes();

    const timeVal = hours + minutes / 60;

    if (timeVal >= 8 && timeVal < 12) {

        return "08:00 - 12:00";

    } else if (timeVal >= 13 && timeVal < 17) {

        return "13:00 - 17:00";

    } else if (timeVal >= 18 && timeVal < 22) {

        return "18:00 - 22:00";

    } else {

        return "Ngoài ca";

    }

}

function updateClock() {

    const now = new Date();

    const timeStr = now.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit', second: '2-digit', hour12: true });

    const dateStr = now.toLocaleDateString('en-GB');

    document.getElementById('txtTime').innerText = timeStr;

    document.getElementById('txtDate').innerText = dateStr;

    const shiftEl = document.getElementById("headerCurrentShift");

    if (shiftEl) {

        shiftEl.innerText = getCurrentShift();

    }

}

setInterval(updateClock, 1000);

updateClock();

// Header running time ticking state
let headerRunningSeconds = 0;
let lastHeaderUpdateLocalTime = 0;
let headerMachineStatus = "COMPLETED";

// Update header stats with data from GetCurrentBatchStats API

function updateHeaderStats(data) {

    if (!data) return;

    if (data.batchInfo) {

        const batchInfo = data.batchInfo;

        // Cache the running seconds and timestamp for real-time ticking
        headerRunningSeconds = Math.floor(Number(batchInfo.batchTotalSeconds)) || 0;
        lastHeaderUpdateLocalTime = Date.now();
        headerMachineStatus = batchInfo.machineStatus || "COMPLETED";

        const batchNumEl = document.getElementById("headerBatchNumber");

        if (batchNumEl) batchNumEl.innerHTML = batchInfo.batchName || "-";

        const stepEl = document.getElementById("step");

        if (stepEl) stepEl.innerHTML = batchInfo.headerStepName || "";

        const statusEl = document.getElementById("headerMachineStatus");

        if (statusEl) {

            statusEl.innerHTML = batchInfo.machineStatus;

            if (batchInfo.machineStatus === "RUNNING") {

                statusEl.style.color = "#22c55e";

            } else {

                statusEl.style.color = "#3b82f6";

            }

        }

        const runningTimeEl = document.getElementById("headerRunningTime");

        if (runningTimeEl) {
            const isOverviewPage = document.getElementById('dailyBatchesBody') !== null;
            if (!isOverviewPage || headerMachineStatus === "COMPLETED") {
                runningTimeEl.innerHTML = batchInfo.headerRunningTime || "0s";
            }
        }

        const alarmCountEl = document.getElementById("alarmCount");

        if (alarmCountEl) alarmCountEl.innerHTML = batchInfo.alarmCount !== undefined ? batchInfo.alarmCount : "0";

    }

    if (data.dailyBatches) {

        let totalOutput = 0;

        let activeStepCode = 0;

        if (data.batchInfo && data.batchInfo.batchStatus === "Active") {

            activeStepCode = data.batchInfo.activeStepCode || 0;

        }

        data.dailyBatches.forEach(function (batch) {

            if (batch.status === "Completed") {

                totalOutput += 500;

            } else if (batch.status === "Active") {

                if (activeStepCode > 0) {

                    totalOutput += Math.round((activeStepCode / 8) * 500);

                }

            }

        });

        const currentOutputEl = document.getElementById("headerCurrentOutput");

        if (currentOutputEl) {

            currentOutputEl.innerHTML = totalOutput;

        }

    }

    const targetOutputEl = document.getElementById("headerTargetOutput");

    if (targetOutputEl) {

        targetOutputEl.innerHTML = "2000";

    }

}

// Fetch stats from server

function fetchHeaderStats() {

    $.ajax({

        url: '/Overview/GetCurrentBatchStats',

        type: 'GET',

        dataType: 'json',

        success: function (data) {

            updateHeaderStats(data);

        },

        error: function (xhr, status, error) {

            console.error('Error fetching header stats:', error);

        }

    });

}

// Global polling initialization

$(document).ready(function() {

    fetchHeaderStats();

    const isOverviewPage = document.getElementById('dailyBatchesBody') !== null;

    if (!isOverviewPage) {

        setInterval(fetchHeaderStats, 20000);

    }

});

// Shared ticker to tick the header running time on pages other than Overview
setInterval(function() {
    const isOverviewPage = document.getElementById('dailyBatchesBody') !== null;
    if (!isOverviewPage && headerMachineStatus === "RUNNING") {
        const runningTimeEl = document.getElementById("headerRunningTime");
        if (runningTimeEl && lastHeaderUpdateLocalTime > 0) {
            const elapsedSinceUpdate = Math.floor((Date.now() - lastHeaderUpdateLocalTime) / 1000);
            const currentRunningSeconds = Math.floor(headerRunningSeconds + elapsedSinceUpdate);
            runningTimeEl.innerHTML = currentRunningSeconds + "s";
        }
    }
}, 1000);