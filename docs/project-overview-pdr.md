# Project Overview (PDR)

## Identity
- **Name:** AFCHEM SCADA System
- **Type:** ASP.NET MVC 5 Web Portal
- **Status:** Active (Phase 2 Upgrade Complete)

## Description
AFCHEM SCADA System is a high-performance web portal designed for solar scada monitoring and mixing tank process control. The system provides real-time SCADA parameter monitoring, historical alarm querying, batch event records tracking, performance reports analysis, and robust user role management.

## Features
- **Real-Time Sơ đồ bồn trộn (Mixing Tank Diagram):** Uses high-speed canvas and dynamic sensory overlays to display temps, pressure, and timeline steps real-time.
- **High-contrast Analytics Charts:** Highcharts gauges and line plots tracking temperature/pressure metrics.
- **Alarm Log & Severity Filter:** Datatable-based alarm database query with From/To Date filter, area sorting, and severity search.
- **Event Log (Batches records):** Displays historical mixing batches with detailed elapsed time and warning logs.
- **Role-Based Authorization:** Strict security separation:
  - **Admin:** Full rights, access to Cài đặt hệ thống (UserSetting), and full permissions to export Excel/CSV reports.
  - **Operator:** View-only rights. Settings link hidden, direct URL entry blocked, exports hidden, and Backend APIs locked with 403 Forbidden.
- **User Settings Management:** Creation of unique accounts, password resetting, and dynamic safeguarding (denying changes if only 1 Admin remains).

## Roadmap
- [x] Redesign UserSetting page using a premium SCADA Dark Theme layout.
- [x] Secure both frontend UI views and backend controller actions for non-Admins.
- [x] Restructure database account table ID field to support AUTO_INCREMENT.
- [x] Fix Vietnamese encoding issue across all razor views by converting them to UTF-8 with BOM.
- [ ] User acceptance testing (UAT) and deployment to staging.
