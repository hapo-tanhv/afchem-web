# Technical Design Document: Reporting Pages

## 1. Architectural Overview
The redesign focuses on migrating the Alarm, Event, and Report pages to a modernized UI while shifting heavy data processing (filtering, pagination, and exporting) to the Server-Side to optimize performance for large log tables (`realtime_alarms` and `alarmreport`).

## 2. Server-Side Processing (SSP) Implementation
### 2.1 API Endpoints (Controllers)
We will introduce AJAX endpoints in the respective controllers (`AlarmController`, `EventController`, `HomeController`):
- `[HttpPost] public JsonResult GetData(...)`: Handles DataTables' SSP requests containing parameters for start index, length (page size), search value, and sort order.
- Custom Parameters: `startTime`, `endTime`, `batchId`.

### 2.2 Database Querying Strategy
We will implement efficient SQL queries using Dapper or standard ADO.NET (depending on the project's current ORM).
- **Pagination**: Use `ORDER BY [Column] OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY`.
- **Total Records**: Two counts are needed for DataTables SSP: `recordsTotal` (total without filter) and `recordsFiltered` (total after applying filter).

### 2.3 Export Features (Excel & CSV)
- **Excel**: We will use a library like `ClosedXML` or `EPPlus` (if available in the project) to generate `.xlsx` files natively on the server. If a library is not installed, we can fall back to standard HTML table to Excel generation or basic CSV.
- **CSV**: Manually construct a `StringBuilder` looping through the filtered records, appending rows separated by commas and returning a `FileContentResult`.

## 3. UI/UX Design
- **Layout**: Adopt the same CSS grid/flexbox foundations established in the Overview page.
- **Controls Panel**: A dedicated top bar on each page will house:
  - `input type="datetime-local"` for Start Time and End Time.
  - `<select>` or text input for Batch ID filter.
  - Action buttons: "Search", "Export Excel", "Export CSV".
- **DataTables**: Integrate the jQuery DataTables plugin initialized with `serverSide: true` and `ajax: { url: '/Controller/GetData', type: 'POST', data: function(d) { d.startTime = ... } }`.

## 4. Data Mapping
- **Alarm Page Table 1**: `realtime_alarms` -> Maps `OccurrenceTime`, `AlarmType`, `Message`.
- **Alarm Page Table 2**: `alarmreport` -> Maps to calculated columns.
- **Event Page**: Focuses on Batch Info ("Công đoạn", "TC cài đặt"). Fetching from the respective batch tables.
- **Report Page**: Full `alarmreport` table view.
