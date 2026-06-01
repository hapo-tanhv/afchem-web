# Requirements: Batch-Runs 1-N Upgrade

## Project Description
The SCADA web portal is being upgraded from a **1 Batch = 1 Run** model to a **1 Batch = N Runs (Mẻ con)** model. 
A Batch represents the overall production order (e.g., `TX01-20260601-01`), while a Run represents an actual execution cycle of 8 PLC process stages (e.g., `TX01-20260601-01-Run01`, `TX01-20260601-01-Run02`).
This upgrade requires significant changes to the Overview page to allow selecting and reviewing individual runs, as well as database-driven cascading filters and autocomplete search bars on the Alarm, Event, and Report pages.

---

## Core Requirements (EARS Format)

### 1. Active Run & Batch Status Resolution
- **[Ubiquitous]** The system shall determine the active run by checking for a record in the `runs` table where `status = 'Active'`.
- **[Ubiquitous]** At any given moment, each device (Device Name) shall have at most one active run in the `runs` table.
- **[Ubiquitous]** The system shall mark a Batch as `Active` if it is currently in production (meaning it has at least one active run, or some runs are completed but not all expected runs `total_runs` are finished).
- **[Ubiquitous]** The system shall mark a Batch as `Completed` only when all of its defined runs (up to `total_runs`) are marked as `Completed` in the `runs` table.

### 2. Overview Page: Run Tab Selector (UI/UX)
- **[Ubiquitous]** The Overview page shall feature a premium horizontal **Tab Selector** representing all runs under the selected Batch.
- **[Ubiquitous]** Each Tab in the selector shall clearly display the run number, run name, and its status (e.g., `[Run 01 - Hoàn thành]`, `[Run 02 - Đang chạy]`, `[Run 03 - Chưa thực hiện]`) using vibrant HSL colors and sleek glassmorphism effects.
- **[State Driven]** When the Overview page is loaded, the interface shall default to showing the **Active Run** (if one exists) or the **Latest Run** of the active/selected Batch.
- **[Event Driven]** When the operator clicks on a completed run tab, the page shall pause live SCADA tag polling, display a prominent "HISTORIC VIEW" badge on the Mixer Diagram, and load the static stage telemetry (`alarmreport`) and alarms (`realtime_alarms`) corresponding to that specific `runId` via AJAX.
- **[Event Driven]** When the operator clicks back to the active run tab, the page shall dismiss the "HISTORIC VIEW" badge, resume 1-second/30-second live SCADA tag polling, and display live data in real-time.

### 3. Granular Stage & Telemetry Tracking
- **[Ubiquitous]** The system shall record and associate all 8 production stages and temperature/pressure logs with both `batchId` and `runId` in the `alarmreport`, `alarmlog`, and `realtime_alarms` tables.
- **[Ubiquitous]** When rendering the 8-stage progress table, the system shall query telemetry data using the selected run's `runId` (`SELECT * FROM alarmreport WHERE runId = @selectedRunId`) to guarantee separation of data between morning and afternoon runs in the same Batch.

### 4. Advanced Cascading Filters & Autocomplete Search (Alarm, Event, Report Pages)
- **[Ubiquitous]** The Alarm, Event, and Report pages shall replace the single "Batch" filter with a **Cascading Filter**:
  1. Dropdown 1: Select Batch (queries `GET /api/batches`).
  2. Dropdown 2: Select Run (initially disabled; once a Batch is selected, queries `GET /api/runs?batch_id=xxx` to populate the runs list).
- **[Ubiquitous]** The system shall also provide an **Autocomplete Search Bar** allowing the operator to type or paste a unique run name (e.g., `TX01-20260601-01-Run01`).
- **[Event Driven]** When a valid run name is selected via the autocomplete search, the system shall automatically pre-populate the cascading dropdowns and load the corresponding reports, events, and alarms for that specific run.
