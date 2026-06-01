# Implementation Tasks: Batch-Runs 1-N Upgrade

- [x] Task 1: Backend Database Integration & Runs API
  - Create standard API endpoint `GET /api/runs?batch_id=xxx` returning list of runs belonging to a batch.
  - Implement direct run search autocomplete endpoint `/api/search_runs?query=xxx` that returns matching runs and their parent batch details.

- [x] Task 2: Update `OverviewController.cs` for Granular Runs Support
  - Modify `GetCurrentBatchStats(int? runId = null)` to support resolving active run automatically if `runId` is null, or query specific run if `runId` has value.
  - Modify database queries inside `GetCurrentBatchStats` to query `alarmlog`, `alarmreport`, and `realtime_alarms` filtered by `runId` instead of `batchId`.
  - Add run list metadata to the JSON return payload for the selected Batch.
  - Update `GetRecentAlarms()` to fetch alarms specifically for the active run instead of the active batch.

- [x] Task 3: Update `Overview.cshtml` UI Components
  - Add HTML containers and premium Glassmorphism styles for the horizontal Run Tab Selector.
  - Implement dynamic rendering of Run tabs with distinct color badges representing run status.
  - Add visual indicators (e.g., "HISTORIC VIEW" overlay/badge) when a historic completed run is being viewed on the Mixer Diagram.

- [x] Task 4: Integrate Run-Switching Logic in `OverviewRealtime.js`
  - Introduce global states: `selectedRunId`, `activeRunId`, `isHistoricView`.
  - Render Run Tab Selector dynamically from the API JSON response.
  - Implement `switchRun(runId, isActive)` click event to swap view modes.
  - Prevent live SCADA dispatcher updates from modifying the UI when `isHistoricView` is active.
  - Update the step statistics timers to reflect selected run duration instead of total batch duration.

- [x] Task 5: Upgrade Alarm/Event/Report Filters
  - Modify UI filters on the Alarm, Event, and Report pages to use Cascading Dropdowns: Batch -> Run.
  - Integrate an Autocomplete Search Bar to search for unique run names directly.
  - Update backend endpoints `GetAlarmsData`, `GetAlarmReportData`, and `GetEventLogRealtime` to filter records strictly by `runId`.

- [x] Task 6: Testing & End-to-End Verification
  - Compile the ASP.NET MVC project to verify clean compilation.
  - Perform visual verification of the glassmorphism Run Tab Selector.
  - Verify switching to historic runs correctly displays static telemetry/alarms and pauses live updates.
  - Verify cascading filters and autocomplete search on report pages load the correct data.
