var EventPage = (function () {

    // ========== MOCK DATA ==========
    var mockCycleSummary = {
        status: "COMPLETED",
        statusLabel: "Chu kỳ hoàn tất thành công",
        batchId: "TX01-20260421-01",
        endTime: "10:20:15",
        formula: "TX01 - Formula A",
        totalTime: "2h 15m 59s",
        weight: "500 kg",
        startTime: "08:04:15"
    };

    var mockPhases = [
        {
            num: 1, name: "Cấp liệu",
            startTime: "08:04:15", endTime: "08:18:15",
            status: "completed", statusText: "Hoàn thành",
            duration: "840s",
            alerts: [
                { time: "08:16:33", type: "WARNING", title: "Vượt quá thời gian cài đặt", message: "Giá trị: 840s (ngưỡng: 720s)" }
            ]
        },
        {
            num: 2, name: "Trộn 1",
            startTime: "08:18:15", endTime: "08:31:15",
            status: "completed", statusText: "Hoàn thành",
            duration: "780s", alerts: []
        },
        {
            num: 3, name: "Xả đáy",
            startTime: "08:31:15", endTime: "08:41:15",
            status: "completed", statusText: "Hoàn thành",
            duration: "600s", alerts: []
        },
        {
            num: 4, name: "Rung xả đáy",
            startTime: "08:41:15", endTime: "08:51:15",
            status: "completed", statusText: "Hoàn thành",
            duration: "600s",
            alerts: [
                { time: "08:45:11", type: "ALARM", title: "Nhiệt độ bồn trộn trên vượt ngưỡng cảnh báo", message: "Giá trị: 85.2 °C (ngưỡng: 84 °C)" },
                { time: "08:47:28", type: "ALARM", title: "Nhiệt độ bồn trộn giữa vượt ngưỡng cảnh báo", message: "Giá trị: 86.2 °C (ngưỡng: 85 °C)" }
            ]
        },
        {
            num: 5, name: "Hút xả đáy",
            startTime: "08:51:15", endTime: "09:03:15",
            status: "completed", statusText: "Hoàn thành",
            duration: "720s", alerts: []
        },
        {
            num: 6, name: "Trộn 2",
            startTime: "09:03:15", endTime: "09:23:15",
            status: "completed", statusText: "Hoàn thành",
            duration: "1200s", alerts: []
        },
        {
            num: 7, name: "Xả hàng",
            startTime: "09:23:15", endTime: "09:48:15",
            status: "completed", statusText: "Hoàn thành",
            duration: "1500s",
            alerts: [
                { time: "09:35:12", type: "WARNING", title: "Áp suất đường ống cao hơn ngưỡng cảnh báo", message: "Giá trị: 0.28 bar (ngưỡng: 0.25 bar)" },
                { time: "09:42:45", type: "WARNING", title: "Áp suất đường ống cao hơn ngưỡng cảnh báo", message: "Giá trị: 0.29 bar (ngưỡng: 0.25 bar)" }
            ]
        },
        {
            num: 8, name: "Rung xả hàng",
            startTime: "09:48:15", endTime: "09:51:15",
            status: "completed", statusText: "Hoàn thành",
            duration: "180s", alerts: []
        }
    ];

    var mockEvents = [
        { time: "08:16:33", phase: "Cấp liệu", event: "Vượt quá thời gian cài đặt", severity: "WARNING" },
        { time: "08:45:11", phase: "Rung xả đáy", event: "Nhiệt độ bồn trộn trên vượt ngưỡng cảnh báo", severity: "ALARM" },
        { time: "08:47:28", phase: "Rung xả đáy", event: "Nhiệt độ bồn trộn giữa vượt ngưỡng cảnh báo", severity: "ALARM" },
        { time: "09:35:12", phase: "Xả hàng", event: "Áp suất đường ống cao hơn ngưỡng cảnh báo", severity: "WARNING" },
        { time: "09:42:45", phase: "Xả hàng", event: "Áp suất đường ống cao hơn ngưỡng cảnh báo", severity: "WARNING" },
        { time: "08:04:15", phase: "Cấp liệu", event: "Bắt đầu cấp liệu", severity: "INFO" },
        { time: "08:18:15", phase: "Trộn 1", event: "Bắt đầu trộn lần 1", severity: "INFO" },
        { time: "08:31:15", phase: "Xả đáy", event: "Bắt đầu xả đáy", severity: "INFO" },
        { time: "09:03:15", phase: "Trộn 2", event: "Bắt đầu trộn lần 2", severity: "INFO" },
        { time: "09:51:15", phase: "Rung xả hàng", event: "Xả liệu hoàn tất", severity: "INFO" }
    ];

    var mockNote = "Chu kỳ hoàn tất. Chất lượng sản phẩm: <strong class='text-success'>ĐẠT</strong>";

    // ========== RENDER FUNCTIONS ==========

    function renderCycleSummary() {
        var d = mockCycleSummary;
        var iconClass = d.status === 'COMPLETED' ? 'completed' : 'in-progress';
        var iconHtml = d.status === 'COMPLETED' ? '<i class="fas fa-check"></i>' : '<i class="fas fa-spinner fa-spin"></i>';

        var html = '<div class="evt-cycle-status">' +
            '<div class="evt-cycle-icon ' + iconClass + '">' + iconHtml + '</div>' +
            '<div><div class="evt-cycle-label">' + d.status + '</div>' +
            '<div class="evt-cycle-sublabel">' + d.statusLabel + '</div></div></div>' +
            '<div class="evt-cycle-details">' +
            buildDetail("Batch ID", d.batchId) +
            buildDetail("Thời gian kết thúc", d.endTime) +
            buildDetail("Công thức", d.formula) +
            buildDetail("Tổng thời gian", d.totalTime) +
            buildDetail("Khối lượng", d.weight) +
            buildDetail("Thời gian bắt đầu", d.startTime) +
            '</div>';

        document.getElementById('cycleSummaryContainer').innerHTML = html;
    }

    function buildDetail(label, value) {
        return '<div class="evt-detail-item"><span class="evt-detail-label">' + label + '</span><span class="evt-detail-value">' + value + '</span></div>';
    }

    function renderAnomalySummary() {
        var alarmCount = 0, warningCount = 0, infoCount = 0;
        mockEvents.forEach(function (e) {
            if (e.severity === 'ALARM') alarmCount++;
            else if (e.severity === 'WARNING') warningCount++;
            else infoCount++;
        });

        var html = '<div class="evt-anomaly-card alarm">' +
            '<div class="evt-anomaly-header"><span class="evt-anomaly-title">ALARM</span><span class="evt-anomaly-icon"><i class="fas fa-exclamation-triangle"></i></span></div>' +
            '<div class="evt-anomaly-count">' + alarmCount + '</div></div>' +
            '<div class="evt-anomaly-card warning">' +
            '<div class="evt-anomaly-header"><span class="evt-anomaly-title">WARNING</span><span class="evt-anomaly-icon"><i class="fas fa-exclamation-triangle"></i></span></div>' +
            '<div class="evt-anomaly-count">' + warningCount + '</div></div>' +
            '<div class="evt-anomaly-card info">' +
            '<div class="evt-anomaly-header"><span class="evt-anomaly-title">INFO</span><span class="evt-anomaly-icon"><i class="fas fa-info-circle"></i></span></div>' +
            '<div class="evt-anomaly-count">' + infoCount + '</div></div>';

        document.getElementById('anomalySummaryContainer').innerHTML = html;
    }

    function renderPhaseLog() {
        var tbody = document.getElementById('phaseLogBody');
        var html = '';

        mockPhases.forEach(function (phase) {
            var alertCount = phase.alerts.length;
            var hasAlarm = phase.alerts.some(function (a) { return a.type === 'ALARM'; });

            var anomalyHtml;
            if (alertCount === 0) {
                anomalyHtml = '<span class="evt-anomaly-badge none"><i class="fas fa-check-circle"></i> Không có</span>';
            } else {
                var badgeClass = hasAlarm ? 'has-alert alarm-type' : 'has-alert';
                var color = hasAlarm ? '#ef4444' : '#f59e0b';
                anomalyHtml = '<span class="evt-anomaly-badge ' + badgeClass + '" onclick="EventPage.togglePhaseAlarm(this)" style="color:' + color + '">' +
                    '<i class="fas fa-exclamation-triangle"></i> ' + alertCount +
                    ' <i class="fas fa-chevron-down toggle-icon" style="font-size:10px;margin-left:2px;"></i></span>';
            }

            html += '<tr>' +
                '<td><div class="evt-phase-num">' + phase.num + '</div></td>' +
                '<td style="text-align:left;"><div class="evt-phase-name">' + phase.name + '</div></td>' +
                '<td><div class="evt-phase-time">' + phase.startTime + '</div></td>' +
                '<td><div class="evt-phase-time">' + phase.endTime + '</div></td>' +
                '<td><span class="status-badge ' + phase.status + '">' + phase.statusText + '</span></td>' +
                '<td>' + phase.duration + '</td>' +
                '<td>' + anomalyHtml + '</td></tr>';

            if (alertCount > 0) {
                var bgTint = hasAlarm ? 'rgba(239,68,68,0.05)' : 'rgba(245,158,11,0.05)';
                html += '<tr class="evt-alarm-detail-row" style="display:none;"><td colspan="7" style="padding:0;">' +
                    '<div class="evt-alarm-detail-content" style="background:' + bgTint + ';">';

                phase.alerts.forEach(function (alert) {
                    var color = alert.type === 'ALARM' ? '#ef4444' : '#f59e0b';
                    var badgeType = alert.type === 'ALARM' ? 'alarm' : 'warning';
                    html += '<div class="evt-alarm-item">' +
                        '<div class="evt-alarm-item-left">' +
                        '<div class="evt-alarm-dot" style="color:' + color + '"><i class="fas fa-circle"></i></div>' +
                        '<div class="evt-alarm-time" style="color:' + color + '">' + alert.time + '</div>' +
                        '<div class="evt-alarm-content"><span class="evt-alarm-title" style="color:' + color + '">' + alert.title + '</span>' +
                        '<span class="evt-alarm-msg">' + alert.message + '</span></div></div>' +
                        '<div class="evt-alarm-badge ' + badgeType + '">' + alert.type + '</div></div>';
                });

                html += '</div></td></tr>';
            }
        });

        tbody.innerHTML = html;
    }

    function renderEventDetails(filter) {
        filter = filter || 'all';
        var filtered = mockEvents;
        if (filter === 'alarm') filtered = mockEvents.filter(function (e) { return e.severity === 'ALARM'; });
        else if (filter === 'warning') filtered = mockEvents.filter(function (e) { return e.severity === 'WARNING'; });
        else if (filter === 'info') filtered = mockEvents.filter(function (e) { return e.severity === 'INFO'; });

        // Sort by time
        filtered.sort(function (a, b) {
            var isAlertA = a.severity === 'ALARM' || a.severity === 'WARNING' ? 0 : 1;
            var isAlertB = b.severity === 'ALARM' || b.severity === 'WARNING' ? 0 : 1;
            if (isAlertA !== isAlertB) return isAlertA - isAlertB;
            return a.time.localeCompare(b.time);
        });

        var tbody = document.getElementById('eventDetailBody');
        var html = '';
        filtered.forEach(function (ev) {
            var sevColor = ev.severity === 'ALARM' ? '#ef4444' : ev.severity === 'WARNING' ? '#f59e0b' : '#94a3b8';
            html += '<tr>' +
                '<td>' + ev.time + '</td>' +
                '<td>' + ev.phase + '</td>' +
                '<td style="text-align:left;">' + ev.event + '</td>' +
                '<td><span class="evt-severity-badge ' + ev.severity + '">' + ev.severity + '</span></td></tr>';
        });
        tbody.innerHTML = html;

        // Render tabs
        var alarmCount = mockEvents.filter(function (e) { return e.severity === 'ALARM'; }).length;
        var warningCount = mockEvents.filter(function (e) { return e.severity === 'WARNING'; }).length;
        var infoCount = mockEvents.filter(function (e) { return e.severity === 'INFO'; }).length;
        var total = mockEvents.length;

        var tabsHtml = '<div class="evt-tab ' + (filter === 'all' ? 'active' : '') + '" onclick="EventPage.filterEvents(\'all\')">Tất cả (' + total + ')</div>' +
            '<div class="evt-tab ' + (filter === 'alarm' ? 'active' : '') + '" onclick="EventPage.filterEvents(\'alarm\')">Alarm (' + alarmCount + ')</div>' +
            '<div class="evt-tab ' + (filter === 'warning' ? 'active' : '') + '" onclick="EventPage.filterEvents(\'warning\')">Warning (' + warningCount + ')</div>' +
            '<div class="evt-tab ' + (filter === 'info' ? 'active' : '') + '" onclick="EventPage.filterEvents(\'info\')">Info (' + infoCount + ')</div>';
        document.getElementById('eventFilterTabs').innerHTML = tabsHtml;
    }

    function renderNote() {
        document.getElementById('eventNote').innerHTML = '<i class="fas fa-info-circle"></i> <div><strong>Ghi chú</strong><br>' + mockNote + '</div>';
    }

    // ========== INTERACTIONS ==========
    function togglePhaseAlarm(el) {
        var tr = el.closest('tr');
        var detailRow = tr.nextElementSibling;
        var icon = el.querySelector('.toggle-icon');
        if (detailRow && detailRow.classList.contains('evt-alarm-detail-row')) {
            if (detailRow.style.display === 'none') {
                detailRow.style.display = 'table-row';
                icon.classList.remove('fa-chevron-down');
                icon.classList.add('fa-chevron-up');
            } else {
                detailRow.style.display = 'none';
                icon.classList.remove('fa-chevron-up');
                icon.classList.add('fa-chevron-down');
            }
        }
    }

    function filterEvents(type) {
        renderEventDetails(type);
    }

    // ========== INIT ==========
    function init() {
        renderCycleSummary();
        renderAnomalySummary();
        renderPhaseLog();
        renderEventDetails('all');
        renderNote();
    }

    return {
        init: init,
        togglePhaseAlarm: togglePhaseAlarm,
        filterEvents: filterEvents
    };

})();
