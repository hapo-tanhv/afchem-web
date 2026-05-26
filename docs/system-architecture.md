# System Architecture

## Overview
The AFCHEM SCADA System is built as a classic 3-Tier Layered Monolith Architecture, optimized for real-time process monitoring and quick transactional reporting.

## Components
| Component | Tech | Purpose |
|-----------|------|---------|
| **Presentation (UI)** | HTML5, CSHTML, Javascript, jQuery, DataTables | Renders real-time dashboards, modals, tables, and manages client-side authentication guards |
| **Business Logic (BE)** | ASP.NET MVC 5 Controllers (`HomeController`, `AlarmController`, etc.) | Validates session roles, locks down reports APIs, and manages transaction endpoints |
| **Data Access Layer** | `MySql.Data.MySqlClient` via `DatabaseConnector` | Connects, reads, and writes to the local MySQL SCADA database |
| **Data Persistence** | MySQL Server 5.6 | Persists system data in tables like `account`, `alarm`, `event`, `report` |

## Data Flow
1. **SCADA Realtime Data:**
   Sensors / PLCs $\rightarrow$ Database `scada` $\rightarrow$ Web App via `atscada-task` dynamic elements $\rightarrow$ Presentation dashboard rendering.
2. **User Actions (Create / Reset / Shift):**
   Browser Form Action $\rightarrow$ AJAX POST Request $\rightarrow$ Session Authorization Check in C# Controller $\rightarrow$ Executing SQL against MySQL $\rightarrow$ JSON Response back to Front-end $\rightarrow$ Toast alert notification.
3. **Data Export Action:**
   User clicks export $\rightarrow$ Controller validates `Role.Admin` $\rightarrow$ Formats records into Excel (via EPPlus) or CSV (via CsvHelper) $\rightarrow$ Stream response payload to browser download. Non-Admins receive a `403 Forbidden` response.

## Database Schema
The primary table modified in this release is `account` inside `scada` database:
- **Table Name:** `account`
  - `ID` (INT, Primary Key, AUTO_INCREMENT)
  - `UserName` (VARCHAR(100), NOT NULL, UNIQUE)
  - `Password` (VARCHAR(45), NOT NULL)
  - `Role` (VARCHAR(45) / INT, NOT NULL) - strictly limited to Admin (`1`) and Operator (`2`).
