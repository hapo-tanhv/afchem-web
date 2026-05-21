# Requirements: Current Batch Statistics (Thống kê mẻ hiện tại)

## Project Description
The goal is to replace the hardcoded fake data in the "Thống kê mẻ hiện tại" table on the Overview page (`Overview.cshtml`) with real-time data fetched directly from the MySQL database.
The backend will query the `alarmreport` table to fetch start/end times and temperatures, and the `realtime_alarms` table to fetch step-specific alarms and warnings.
The frontend will fetch this structured data via a periodic AJAX call every 30 seconds and update the table dynamically.

## Core Requirements (EARS format)

### 1. Data Querying & Batch Tracking
- **[Ubiquitous]** The system shall determine the current active batch ID by finding the maximum `batchId` present in the `alarmreport` table.
- **[Ubiquitous]** The system shall always return and display a complete list of exactly 8 production steps:
  1. Cấp liệu (`CongDoanMay = 1`, Standard: 720s)
  2. Trộn 1 (`CongDoanMay = 2`, Standard: 780s)
  3. Xả đáy (`CongDoanMay = 3`, Standard: 600s)
  4. Rung xả đáy (`CongDoanMay = 4`, Standard: 600s)
  5. Hút xả đáy (`CongDoanMay = 5`, Standard: 720s)
  6. Trộn 2 (`CongDoanMay = 6`, Standard: 1200s)
  7. Xả hàng (`CongDoanMay = 7`, Standard: 1500s)
  8. Rung xả hàng (`CongDoanMay = 8`, Standard: 180s)

### 2. Step Lifecycle and Timing Calculations
- **[State Driven]** If no records exist in `alarmreport` for a given step ID for the active batch, the system shall mark the step as `pending` and display all calculated metrics as `-`.
- **[State Driven]** If records exist for a step, and there exists a subsequent step in `alarmreport` with a higher `CongDoanMay` for the same batch, the system shall mark the step as `completed`.
- **[State Driven]** If records exist for a step, and it is the maximum `CongDoanMay` currently present in `alarmreport` for the active batch, the system shall mark the step as `in-progress`.
- **[Ubiquitous]** The system shall calculate the Start Time of a step as the minimum `DateTime` of its records in `alarmreport`, formatted to show only the time part (`HH:mm:ss`).
- **[State Driven]** For `completed` steps, the system shall calculate the End Time as the maximum `DateTime` of its records, formatted as `HH:mm:ss`, and calculate the Duration as `(EndTime - StartTime)` in seconds, formatted as `"{duration}s"`.
- **[State Driven]** For `in-progress` steps, the system shall display the End Time and Duration as `-`.

### 3. Temperature Analysis (Min-Max Calculation)
- **[Ubiquitous]** The system shall analyze the telemetry data in `alarmreport` for each active/completed step's time range to calculate the minimum and maximum temperatures of the three mixer levels:
  - Bồn trên (`NhietDoBonTronTr`)
  - Bồn giữa (`NhietDoBonTronGi`)
  - Bồn dưới (`NhietDoBonTronDu`)
- **[State Driven]** When there is temperature variation (Min is not equal to Max), the system shall format the display as a range: `"{Min}-{Max}°C"`.
- **[State Driven]** When there is no temperature variation (Min is equal to Max, such as when only one record exists), the system shall format the display as a single value: `"{Min}°C"`.

### 4. Step-Specific Alarm & Warning Association
- **[Ubiquitous]** The system shall query `realtime_alarms` where `batchId` matches the active batch, `CongDoan` matches the step's text representation, and `Severity` is either `ALARM` or `WARNING`.
- **[Ubiquitous]** The system shall map each queried alarm/warning record to the corresponding step using the following schema:
  - `time`: The time part of `DateTime` formatted as `HH:mm:ss`.
  - `type`: The `Severity` value (`ALARM` or `WARNING`).
  - `title`: The `Message` column from the database.
  - `message`: A detailed description string formatted as: `$"Giá trị: {Value} {Unit} (ngưỡng: {Threshold} {Unit})"` where `Unit` is `"°C"` if `TagName` contains `"NhietDo"`, `"bar"` if `TagName` contains `"ApSuat"`, and empty otherwise.

### 5. Periodical Updates
- **[Ubiquitous]** The frontend shall fetch the current batch statistics from the server endpoint every 30 seconds and update the Overview page table without full-page reloads.
