var EventPage = (function () {

    // Keep a local copy of events to support tab filtering client-side
    var currentEvents = [];

    // ========== RENDER FUNCTIONS ==========

    function renderCycleSummary(d) {
        if (!d) return;
        var iconClass = d.status === 'ACTIVE' ? 'in-progress' : 'completed';
        var iconHtml = d.status === 'ACTIVE' ? '<i class="fas fa-spinner fa-spin"></i>' : '<i class="fas fa-check"></i>';

        var html = '<div class="evt-cycle-status">' +
            '<div class="evt-cycle-icon ' + iconClass + '">' + iconHtml + '</div>' +
            '<div><div class="evt-cycle-label">' + d.status + '</div>' +
            '<div class="evt-cycle-sublabel">' + d.statusLabel + '</div></div></div>' +
            '<div class="evt-cycle-details">' +
            buildDetail("Batch ID", d.batchId) +
            buildDetail("Tên sản phẩm", d.productName) +
            buildDetail("Công thức", d.formula) +
            buildDetail("Khối lượng", d.weight) +
            buildDetail("Thời gian bắt đầu", d.startTime) +
            buildDetail("Thời gian kết thúc", d.endTime) +
            buildDetail("Tổng thời gian", d.totalTime) +
            '</div>';

        document.getElementById('cycleSummaryContainer').innerHTML = html;
    }

    function buildDetail(label, value) {
        return '<div class="evt-detail-item"><span class="evt-detail-label">' + label + '</span><span class="evt-detail-value">' + value + '</span></div>';
    }

    function renderAnomalySummary(events) {
        var alarmCount = 0, warningCount = 0, infoCount = 0;
        events.forEach(function (e) {
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

    function renderPhaseLog(phases) {
        var tbody = document.getElementById('phaseLogBody');
        if (!tbody) return;
        var html = '';

        if (!phases || phases.length === 0) {
            tbody.innerHTML = '<tr><td colspan="7" style="text-align:center;">Không có dữ liệu công đoạn</td></tr>';
            return;
        }

        phases.forEach(function (phase) {
            var alertCount = phase.alerts ? phase.alerts.length : 0;
            var hasAlarm = phase.alerts ? phase.alerts.some(function (a) { return a.type === 'ALARM'; }) : false;

            var anomalyHtml;
            if (alertCount === 0) {
                anomalyHtml = '<span class="evt-anomaly-badge none"><i class="fas fa-check-circle"></i> Không có</span>';
            } else {
                var badgeClass = hasAlarm ? 'has-alert alarm-type' : 'has-alert';
                var color = hasAlarm ? '#ef4444' : '#f59e0b';
                anomalyHtml = '<span class="evt-anomaly-badge ' + badgeClass + '" onclick="EventPage.togglePhaseAlarm(this)" style="color:' + color + '; cursor:pointer;">' +
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
        var filtered = [];
        if (filter === 'all') filtered = currentEvents.slice();
        else if (filter === 'alarm') filtered = currentEvents.filter(function (e) { return e.severity === 'ALARM'; });
        else if (filter === 'warning') filtered = currentEvents.filter(function (e) { return e.severity === 'WARNING'; });
        else if (filter === 'info') filtered = currentEvents.filter(function (e) { return e.severity === 'INFO'; });

        // Sort by time
        filtered.sort(function (a, b) {
            var isAlertA = a.severity === 'ALARM' || a.severity === 'WARNING' ? 0 : 1;
            var isAlertB = b.severity === 'ALARM' || b.severity === 'WARNING' ? 0 : 1;
            if (isAlertA !== isAlertB) return isAlertA - isAlertB;
            return a.time.localeCompare(b.time);
        });

        var tbody = document.getElementById('eventDetailBody');
        if (!tbody) return;
        var html = '';

        if (filtered.length === 0) {
            html = '<tr><td colspan="4" style="text-align:center;">Không có sự kiện nào</td></tr>';
        } else {
            filtered.forEach(function (ev) {
                html += '<tr>' +
                    '<td>' + ev.time + '</td>' +
                    '<td>' + ev.phase + '</td>' +
                    '<td style="text-align:left;">' + ev.event + '</td>' +
                    '<td><span class="evt-severity-badge ' + ev.severity + '">' + ev.severity + '</span></td></tr>';
            });
        }
        tbody.innerHTML = html;

        // Render tabs
        var alarmCount = currentEvents.filter(function (e) { return e.severity === 'ALARM'; }).length;
        var warningCount = currentEvents.filter(function (e) { return e.severity === 'WARNING'; }).length;
        var infoCount = currentEvents.filter(function (e) { return e.severity === 'INFO'; }).length;
        var total = currentEvents.length;

        var tabsHtml = '<div class="evt-tab ' + (filter === 'all' ? 'active' : '') + '" onclick="EventPage.filterEvents(\'all\')">Tất cả (' + total + ')</div>' +
            '<div class="evt-tab ' + (filter === 'alarm' ? 'active' : '') + '" onclick="EventPage.filterEvents(\'alarm\')">Alarm (' + alarmCount + ')</div>' +
            '<div class="evt-tab ' + (filter === 'warning' ? 'active' : '') + '" onclick="EventPage.filterEvents(\'warning\')">Warning (' + warningCount + ')</div>' +
            '<div class="evt-tab ' + (filter === 'info' ? 'active' : '') + '" onclick="EventPage.filterEvents(\'info\')">Info (' + infoCount + ')</div>';
        
        var tabsContainer = document.getElementById('eventFilterTabs');
        if (tabsContainer) tabsContainer.innerHTML = tabsHtml;
    }

    function renderNote(noteText) {
        var el = document.getElementById('eventNote');
        if (el) {
            el.innerHTML = '<i class="fas fa-info-circle"></i> <div><strong>Ghi chú</strong><br>' + noteText + '</div>';
        }
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

    // ========== AJAX LOADING ==========

    function loadBatches(date, callback) {
        var params = {};
        if (date) {
            params.date = date;
        }
        $.ajax({
            url: '/Event/GetBatches',
            type: 'GET',
            data: params,
            dataType: 'json',
            success: function (data) {
                var select = $('#batchId');
                if (select.length) {
                    select.empty();
                    if (data && data.length) {
                        data.forEach(function (batch) {
                            var label = batch.name;
                            if (batch.status === 'Active') {
                                label += ' (Active)';
                            } else if (batch.status === 'Completed') {
                                label += ' (Completed)';
                            }
                            select.append('<option value="' + batch.id + '">' + label + '</option>');
                        });
                    } else {
                        select.append('<option value="">Không có mẻ nào</option>');
                    }
                }
                if (typeof callback === 'function') {
                    callback();
                }
            },
            error: function (xhr, status, error) {
                console.error("Lỗi khi tải danh sách batches:", error);
                if (typeof callback === 'function') {
                    callback();
                }
            }
        });
    }

    function loadEventData(isInitialLoad) {
        var batchId = isInitialLoad ? "" : ($('#batchId').val() || "");
        var formattedDate = "";
        
        if (!isInitialLoad) {
            var dateVal = $('#starttime').val() || "";
            formattedDate = dateVal.replace(/\//g, '-');
        }
        
        // Show loading state
        document.getElementById('cycleSummaryContainer').innerHTML = '<div style="padding: 20px; text-align: center;"><i class="fas fa-spinner fa-spin fa-2x"></i><br>Đang tải dữ liệu...</div>';
        document.getElementById('eventDetailBody').innerHTML = '<tr><td colspan="4" style="text-align:center;"><i class="fas fa-spinner fa-spin"></i> Đang tải dữ liệu...</td></tr>';
        
        $.ajax({
            url: '/Event/GetEventLogRealtime',
            type: 'GET',
            data: { batchId: batchId, date: formattedDate },
            dataType: 'json',
            success: function (res) {
                if (res.error) {
                    alert("Lỗi từ server: " + res.error);
                    return;
                }

                currentEvents = res.events || [];

                // Render all elements
                renderCycleSummary(res.cycleSummary);
                renderAnomalySummary(currentEvents);
                renderPhaseLog(res.phases);
                renderEventDetails('all');
                renderNote(res.note);

                if (isInitialLoad) {
                    if (res.batchDate) {
                        var dateVal = res.batchDate.replace(/-/g, '/');
                        $('#starttime').val(dateVal);
                        
                        var drp = $('#starttime').data('daterangepicker');
                        if (drp) {
                            drp.setStartDate(dateVal);
                            drp.setEndDate(dateVal);
                        }
                        
                        loadBatches(res.batchDate, function() {
                            if (res.batchId && res.batchId > 0) {
                                $('#batchId').val(res.batchId);
                            }
                        });
                    }
                } else {
                    if (res.batchId && res.batchId > 0) {
                        $('#batchId').val(res.batchId);
                    }
                }
            },
            error: function (xhr, status, error) {
                console.error("Lỗi khi tải dữ liệu sự kiện:", error);
                document.getElementById('cycleSummaryContainer').innerHTML = '<div style="padding: 20px; text-align: center; color: #ef4444;"><i class="fas fa-exclamation-triangle"></i> Lỗi kết nối dữ liệu!</div>';
                document.getElementById('eventDetailBody').innerHTML = '<tr><td colspan="4" style="text-align:center; color: #ef4444;">Không thể tải dữ liệu từ máy chủ.</td></tr>';
            }
        });
    }

    // ========== INIT ==========
    function init() {
        // Initial load: fetch the latest active/completed batch from the server
        loadEventData(true);
    }

    return {
        init: init,
        togglePhaseAlarm: togglePhaseAlarm,
        filterEvents: filterEvents,
        loadBatches: loadBatches,
        loadEventData: loadEventData
    };

})();
