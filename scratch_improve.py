# -*- coding: utf-8 -*-
path_cs = r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\Controllers\EventController.cs"
path_js = r"c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\LongDucProjectTest\JavaScript\Event\EventPage.js"

# 1. Update EventController.cs
with open(path_cs, "r", encoding="utf-8-sig") as f:
    content_cs = f.read()

content_cs = content_cs.replace("\r\n", "\n")

target_cs1 = """                // If no batchId is selected, resolve the default batch
                if (selectedBatchId <= 0)
                {
                    string dateFilter = "";
                    if (!string.IsNullOrEmpty(date) && DateTime.TryParseExact(date, new[] { "yyyy-MM-dd", "yyyy/MM/dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    {
                        dateFilter = $"WHERE DATE(start_time) = '{parsedDate.ToString("yyyy-MM-dd")}'";
                    }

                    if (!string.IsNullOrEmpty(dateFilter))
                    {
                        // Get latest batch of that date
                        var dtLatestDate = connector.ExecuteQuery($"SELECT id FROM batches {dateFilter} ORDER BY id DESC LIMIT 1");
                        if (dtLatestDate != null && dtLatestDate.Rows.Count > 0)
                        {
                            selectedBatchId = Convert.ToInt32(dtLatestDate.Rows[0]["id"]);
                        }
                    }

                    if (selectedBatchId <= 0)
                    {
                        // 1. Get active batch
                        var dtActive = connector.ExecuteQuery("SELECT id FROM batches WHERE status = 'Active' LIMIT 1");
                        if (dtActive != null && dtActive.Rows.Count > 0)
                        {
                            selectedBatchId = Convert.ToInt32(dtActive.Rows[0]["id"]);
                        }
                        else
                        {
                            // 2. Get latest completed batch
                            var dtCompleted = connector.ExecuteQuery("SELECT id FROM batches WHERE status = 'Completed' ORDER BY id DESC LIMIT 1");
                            if (dtCompleted != null && dtCompleted.Rows.Count > 0)
                            {
                                selectedBatchId = Convert.ToInt32(dtCompleted.Rows[0]["id"]);
                            }
                            else
                            {
                                // 3. Get latest overall
                                var dtLatest = connector.ExecuteQuery("SELECT id FROM batches ORDER BY id DESC LIMIT 1");
                                if (dtLatest != null && dtLatest.Rows.Count > 0)
                                {
                                    selectedBatchId = Convert.ToInt32(dtLatest.Rows[0]["id"]);
                                }
                            }
                        }
                    }
                }"""

replace_cs1 = """                // If no batchId is selected, resolve the default batch
                if (selectedBatchId <= 0)
                {
                    string dateFilter = "";
                    if (!string.IsNullOrEmpty(date) && DateTime.TryParseExact(date, new[] { "yyyy-MM-dd", "yyyy/MM/dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                    {
                        dateFilter = $"WHERE DATE(start_time) = '{parsedDate.ToString("yyyy-MM-dd")}'";
                    }

                    if (!string.IsNullOrEmpty(dateFilter))
                    {
                        // Get latest batch of that date
                        var dtLatestDate = connector.ExecuteQuery($"SELECT id FROM batches {dateFilter} ORDER BY id DESC LIMIT 1");
                        if (dtLatestDate != null && dtLatestDate.Rows.Count > 0)
                        {
                            selectedBatchId = Convert.ToInt32(dtLatestDate.Rows[0]["id"]);
                        }
                        else
                        {
                            selectedBatchId = -2; // Mark as date filtered but no batch found, do not fall back
                        }
                    }

                    if (selectedBatchId == -1) // No date filter was applied (initial load), so find default overall batch
                    {
                        // 1. Get active batch
                        var dtActive = connector.ExecuteQuery("SELECT id FROM batches WHERE status = 'Active' LIMIT 1");
                        if (dtActive != null && dtActive.Rows.Count > 0)
                        {
                            selectedBatchId = Convert.ToInt32(dtActive.Rows[0]["id"]);
                        }
                        else
                        {
                            // 2. Get latest completed batch
                            var dtCompleted = connector.ExecuteQuery("SELECT id FROM batches WHERE status = 'Completed' ORDER BY id DESC LIMIT 1");
                            if (dtCompleted != null && dtCompleted.Rows.Count > 0)
                            {
                                selectedBatchId = Convert.ToInt32(dtCompleted.Rows[0]["id"]);
                            }
                            else
                            {
                                // 3. Get latest overall
                                var dtLatest = connector.ExecuteQuery("SELECT id FROM batches ORDER BY id DESC LIMIT 1");
                                if (dtLatest != null && dtLatest.Rows.Count > 0)
                                {
                                    selectedBatchId = Convert.ToInt32(dtLatest.Rows[0]["id"]);
                                }
                            }
                        }
                    }
                }"""

target_cs2 = """                if (dtBatch != null && dtBatch.Rows.Count > 0)
                {
                    var rowBatch = dtBatch.Rows[0];
                    batchName = rowBatch["name"].ToString();
                    batchStatus = rowBatch["status"].ToString();
                    deviceName = rowBatch["device_name"] != DBNull.Value ? rowBatch["device_name"].ToString() : "TX01 A";

                    if (rowBatch["start_time"] != DBNull.Value)
                    {
                        DateTime startTime = Convert.ToDateTime(rowBatch["start_time"]);
                        startStr = startTime.ToString("HH:mm:ss");"""

replace_cs2 = """                string batchDateStr = "";
                if (dtBatch != null && dtBatch.Rows.Count > 0)
                {
                    var rowBatch = dtBatch.Rows[0];
                    batchName = rowBatch["name"].ToString();
                    batchStatus = rowBatch["status"].ToString();
                    deviceName = rowBatch["device_name"] != DBNull.Value ? rowBatch["device_name"].ToString() : "TX01 A";

                    if (rowBatch["start_time"] != DBNull.Value)
                    {
                        DateTime startTime = Convert.ToDateTime(rowBatch["start_time"]);
                        batchDateStr = startTime.ToString("yyyy-MM-dd");
                        startStr = startTime.ToString("HH:mm:ss");"""

target_cs3 = """                return Json(new {
                    batchId = selectedBatchId,
                    batchName = batchName,
                    cycleSummary = cycleSummary,
                    phases = stepsList,
                    events = eventsList,
                    note = note
                }, JsonRequestBehavior.AllowGet);"""

replace_cs3 = """                return Json(new {
                    batchId = selectedBatchId,
                    batchName = batchName,
                    batchDate = batchDateStr,
                    cycleSummary = cycleSummary,
                    phases = stepsList,
                    events = eventsList,
                    note = note
                }, JsonRequestBehavior.AllowGet);"""

target_cs1_lf = target_cs1.replace("\r\n", "\n")
replace_cs1_lf = replace_cs1.replace("\r\n", "\n")
target_cs2_lf = target_cs2.replace("\r\n", "\n")
replace_cs2_lf = replace_cs2.replace("\r\n", "\n")
target_cs3_lf = target_cs3.replace("\r\n", "\n")
replace_cs3_lf = replace_cs3.replace("\r\n", "\n")

if target_cs1_lf in content_cs:
    content_cs = content_cs.replace(target_cs1_lf, replace_cs1_lf)
    print("EventController Target 1 replaced")
else:
    print("EventController Target 1 NOT found")

if target_cs2_lf in content_cs:
    content_cs = content_cs.replace(target_cs2_lf, replace_cs2_lf)
    print("EventController Target 2 replaced")
else:
    print("EventController Target 2 NOT found")

if target_cs3_lf in content_cs:
    content_cs = content_cs.replace(target_cs3_lf, replace_cs3_lf)
    print("EventController Target 3 replaced")
else:
    print("EventController Target 3 NOT found")

content_cs = content_cs.replace("\n", "\r\n")
with open(path_cs, "w", encoding="utf-8-sig") as f:
    f.write(content_cs)


# 2. Update EventPage.js
with open(path_js, "r", encoding="utf-8-sig") as f:
    content_js = f.read()

content_js = content_js.replace("\r\n", "\n")

target_js1 = """    function loadEventData() {
        var batchId = $('#batchId').val() || "";
        var dateVal = $('#starttime').val() || "";
        var formattedDate = dateVal.replace(/\//g, '-');
        
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

                // If a batch ID was resolved by server, select it in the dropdown
                if (res.batchId && res.batchId > 0) {
                    $('#batchId').val(res.batchId);
                }

                currentEvents = res.events || [];

                // Render all elements
                renderCycleSummary(res.cycleSummary);
                renderAnomalySummary(currentEvents);
                renderPhaseLog(res.phases);
                renderEventDetails('all');
                renderNote(res.note);
            },"""

replace_js1 = """    function loadEventData(isInitialLoad) {
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
            },"""

target_js2 = """    // ========== INIT ==========
    function init() {
        var dateVal = $('#starttime').val() || "";
        var formattedDate = dateVal.replace(/\//g, '-');
        // First load batches, then load the data
        loadBatches(formattedDate, function () {
            loadEventData();
        });
    }"""

replace_js2 = """    // ========== INIT ==========
    function init() {
        // Initial load: fetch the latest active/completed batch from the server
        loadEventData(true);
    }"""

target_js1_lf = target_js1.replace("\r\n", "\n")
replace_js1_lf = replace_js1.replace("\r\n", "\n")
target_js2_lf = target_js2.replace("\r\n", "\n")
replace_js2_lf = replace_js2.replace("\r\n", "\n")

if target_js1_lf in content_js:
    content_js = content_js.replace(target_js1_lf, replace_js1_lf)
    print("EventPage.js Target 1 replaced")
else:
    print("EventPage.js Target 1 NOT found")

if target_js2_lf in content_js:
    content_js = content_js.replace(target_js2_lf, replace_js2_lf)
    print("EventPage.js Target 2 replaced")
else:
    print("EventPage.js Target 2 NOT found")

content_js = content_js.replace("\n", "\r\n")
with open(path_js, "w", encoding="utf-8-sig") as f:
    f.write(content_js)

print("Done!")
