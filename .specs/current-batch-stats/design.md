# Technical Design: Current Batch Statistics (Thống kê mẻ hiện tại)

## 1. Architectural Overview
This feature integrates database-driven batch status tracking into the SCADA Overview dashboard. It bridges the gap between backend MySQL telemetry tables and the modern dashboard UI by introducing a real-time JSON API in `OverviewController` and periodic AJAX pulling in the client-side scripts.

```
+--------------------+        +-------------------------+        +--------------------+
|  Overview.cshtml   | <====> |   OverviewController    | <====> |    MySQL Server    |
| (AJAX every 30s)   |  JSON  | (GetCurrentBatchStats)  |  SQL   | (scada DB tables)  |
+--------------------+        +-------------------------+        +--------------------+
```

## 2. Database Schema Details

### 2.1 Table: `scada.alarmreport`
This table records system telemetry periodic dumps (every 30s).
- `DateTime` (datetime): Timestamp of the data snapshot.
- `CongDoanMay` (int): Step identifier (1 = Cấp liệu, 2 = Trộn 1, ..., 8 = Rung xả hàng).
- `NhietDoBonTronTr` (double/float): Temperature of mixer upper level.
- `NhietDoBonTronGi` (double/float): Temperature of mixer middle level.
- `NhietDoBonTronDu` (double/float): Temperature of mixer lower level.
- `batchId` (int): Unique identifier of the production batch.

### 2.2 Table: `scada.alarmlog`
This table captures the status of step-specific timer and SCADA logs.
- `OccurrenceTime` (datetime): Time the step was started.
- `RestoreTime` (datetime): Time the step was completed/resolved.
- `Description` (varchar): Event or timer description (e.g. "Timer Cấp Liệu").
- `Status` (varchar): `"Alarm"` for in-progress or `"Resolved"` for completed.
- `TagNo` (varchar): Unique code identifying each of the 8 stages (`T001` to `T008` respectively).
- `batchId` (int): Production batch ID.

### 2.3 Table: `scada.realtime_alarms`
This table captures active and historical alarm occurrences for active batches.
- `DateTime` (datetime): Time of event occurrence.
- `CongDoan` (varchar): Text representation of the step (e.g., "Cấp liệu", "Trộn 1", etc.).
- `batchId` (int): Production batch ID.
- `Severity` (varchar): Event severity level (`ALARM`, `WARNING`, `INFO`).
- `TagName` (varchar): Tag name of the triggering sensor.
- `Value` (double): The actual reading value.
- `Threshold` (double): The warning limit.
- `Message` (varchar): Descriptive message.

## 3. Backend Implementation (C#)

### 3.1 Endpoint
A new action in `OverviewController.cs`:
```csharp
[HttpGet]
public JsonResult GetCurrentBatchStats()
```

### 3.2 Logic Flow in Endpoint
1. Initialize `MySQLConnect` to connect to `Server=localhost;Database=scada;Uid=root;Pwd=101101;`.
2. Query the active batch ID:
   ```sql
   SELECT id FROM batches WHERE status = 'Active' LIMIT 1
   ```
3. Retrieve all step log records for this batch from `alarmlog`:
   ```sql
   SELECT OccurrenceTime, RestoreTime, Description, Status, TagNo FROM alarmlog WHERE batchId = @BatchId
   ```
4. Retrieve all telemetry logs for this batch from `alarmreport` sorted by `DateTime` ascending:
   ```sql
   SELECT DateTime, NhietDoBonTronTren, NhietDoBonTronGiua, NhietDoBonTronDuoi FROM alarmreport WHERE batchId = @BatchId ORDER BY DateTime ASC
   ```
5. Retrieve all alarms/warnings for this batch from `realtime_alarms` where severity is `ALARM` or `WARNING`:
   ```sql
   SELECT DateTime, CongDoan, Severity, TagName, Value, Threshold, Message FROM realtime_alarms WHERE batchId = @BatchId AND Severity IN ('ALARM', 'WARNING') ORDER BY DateTime ASC
   ```
6. Process the steps (1 to 8):
   - Define a static list of the 8 steps mapped to their standard duration and TagNo:
     - 1: Cấp liệu (720s, `T001`)
     - 2: Trộn 1 (780s, `T002`)
     - 3: Xả đáy (600s, `T003`)
     - 4: Rung xả đáy (600s, `T004`)
     - 5: Hút xả đáy (720s, `T005`)
     - 6: Trộn 2 (1200s, `T006`)
     - 7: Xả hàng (1500s, `T007`)
     - 8: Rung xả hàng (180s, `T008`)
   - For each step:
     - Check if a log entry exists in `alarmlog` where `TagNo` matches (falling back to keyword matching on description).
     - **If no record**:
       - `status = "pending"`
       - `statusText = "Chưa bắt đầu"`
       - Start/End/Duration = `"-"`
       - Temperatures = `"-"`
       - Alerts = `[]`
     - **If record exists**:
       - `StartTime` = `OccurrenceTime`.
       - If `Status == "Resolved"`:
         - `status = "completed"`
         - `statusText = "Hoàn thành"`
         - `EndTime` = `RestoreTime`.
         - `Duration` = `(EndTime - StartTime).TotalSeconds` formatted as `"{seconds}s"`.
       - If `Status == "Alarm"` (in-progress):
         - `status = "in-progress"`
         - `statusText = "Đang thực hiện"`
         - `EndTime` = `"-"`
         - `Duration` = `"-"`
       - Calculate temperatures (Min and Max for top, mid, bottom):
         - Filter `alarmreport` telemetry records where `DateTime` is between `StartTime` and `EndTime` (or >= `StartTime` if in-progress).
         - Format as `"{Min}-{Max}°C"` if `Min != Max`, otherwise `"{Min}°C"`. If no telemetry records exist yet, display `"-"`.
       - Filter alarms from `realtime_alarms` where `CongDoan = stepName` or falling within step's time range:
         - Map to an alert model:
           - `time` = `DateTime.ToString("HH:mm:ss")`
           - `type` = `Severity`
           - `title` = `Message`
           - `message` = `$"Giá trị: {Value} {Unit} (ngưỡng: {Threshold} {Unit})"` where `Unit` is calculated based on sensor tag type.

## 4. Frontend Integration (JS & Razor)
1. **`Overview.cshtml`**:
   - Locate the `<script>` tag defining `mockBatchData` and `mockGlobalAlarms`.
   - Remove these hardcoded variables and the static rendering on load.
2. **`OverviewRealtime.js`**:
   - Implement `fetchCurrentBatchStats()` which performs an AJAX `GET` to `/Overview/GetCurrentBatchStats`.
   - On success:
     - Render the batch table using `renderBatchTable(data.steps)`.
     - Extract all alerts from the steps to build a global list and render using `renderAlarmList(globalAlarms)`.
   - Set an interval to run `fetchCurrentBatchStats()` every 30 seconds.
   - Run once on `DOMContentLoaded` (after initializing the charts).
