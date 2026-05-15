# Task List: Reporting Pages Redesign

## Phase 1: Backend Foundations (Server-Side Processing & Export)
- [x] **Task 1.1: Common Export Utilities**
  - Create a utility class for generating CSV and Excel files from generic data lists or DataTables to avoid code duplication across controllers.
- [x] **Task 1.2: Controller APIs for Datatables**
  - Implement `GetAlarmsData` and `GetAlarmReportData` in `AlarmController`.
  - Implement `GetEventData` in `EventController`.
  - Implement `GetReportData` in `HomeController`.
- [x] **Task 1.3: Controller APIs for Export**
  - Implement Excel and CSV export actions in `AlarmController`, `EventController`, and `HomeController` that reuse the filtering logic.

## Phase 2: Frontend Implementation (UI & JS Integration)
- [x] **Task 2.1: Alarm Page UI**
  - Redesign `Alarm.cshtml` layout to include a filter bar (Time, Batch) and export buttons.
  - Render two tables and initialize DataTables with Server-Side configuration pointing to the backend APIs.
- [x] **Task 2.2: Event Page UI**
  - Redesign `Event.cshtml` layout to include the filter bar.
  - Implement the single Batch Statistics table ("Công đoạn", "TC cài đặt") and initialize DataTables.
- [x] **Task 2.3: Report Page UI**
  - Redesign `Home.cshtml` (or the equivalent Report view) with the filter bar.
  - Render the comprehensive `alarmreport` table and initialize DataTables.
- [x] **Task 2.4: CSS & Styling Alignment**
  - Ensure all tables, filter bars, and buttons use consistent CSS classes matching the modernized `Overview.css` aesthetic.
