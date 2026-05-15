# Requirements Document: Reporting Pages (Cảnh báo, Sự kiện, Báo cáo)

## Project Description (Input)
The goal is to re-implement the UI and functionality for three main pages: Alarm (Cảnh báo), Event (Sự kiện), and Report (Báo cáo/Home), moving towards a modernized look consistent with the Overview page. Additionally, robust filtering (by time and batch) and data export (Excel, CSV) features must be implemented using **Server-Side Processing** to handle potentially large datasets efficiently.

## Core Requirements (EARS format)

### 1. Trang Cảnh báo (Alarm Page)
- **[Ubiquitous]** The system shall display two distinct data tables on the Alarm page.
- **[Ubiquitous]** The first table shall fetch and display logs from the `realtime_alarms` table (columns: Time, Type, Message).
- **[Ubiquitous]** The second table shall fetch and display logs from the `alarmreport` table, presenting calculated columns similar to the first table.
- **[State Driven]** When the user interacts with the time filter or batch filter, the system shall filter both tables accordingly using Server-Side processing (pagination, sorting, filtering done in SQL).
- **[Event Driven]** When the user clicks "Export Excel" or "Export CSV", the system shall generate and download the respective files containing the filtered data using Server-Side processing.

### 2. Trang Sự kiện (Event Page)
- **[Ubiquitous]** The system shall display a single statistics table containing batch information (specifically "Công đoạn" and "TC cài đặt", similar to the bottom-left table in the Overview page). No charts are required.
- **[State Driven]** When the user interacts with the time filter or batch filter, the system shall filter the table accordingly using Server-Side processing.
- **[Event Driven]** When the user clicks "Export Excel" or "Export CSV", the system shall generate and download the respective files containing the filtered data using Server-Side processing.

### 3. Trang Báo cáo (Report Page - mapped to Home)
- **[Ubiquitous]** The system shall display a data table fetching all records from the `alarmreport` table.
- **[State Driven]** When the user interacts with the time filter or batch filter, the system shall filter the table accordingly using Server-Side processing.
- **[Event Driven]** When the user clicks "Export Excel" or "Export CSV", the system shall generate and download the respective files containing the filtered data using Server-Side processing.

*(Note: "Trang Cài đặt hệ thống" is explicitly excluded from this specification as per user request).*
