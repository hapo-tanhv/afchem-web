# Implementation Tasks: User Settings & Authorization

- [x] **Phase 1: Backend Cleanup & Authentication Updates**
  - [x] Modify `Hino.Getdata.Common/Authentication.cs` to update the `Role` enum to consist only of `None = 0`, `Admin = 1`, and `Operator = 2`.
  - [x] Clean up redundant `Role` checks and switch cases in `HomeController.cs` (Actions: `Login` POST, `Home`, `Overview`, `OverviewSignage`, `Alarm`, `Event`).
  - [x] Lock down `HomeController.UserSetting()` to strictly check for `Role.Admin`. If not, redirect to `/Home/Overview`.
  - [x] Add backend export constraints to return 403 Forbidden for non-Admin users in:
    - [x] `HomeController.cs` (actions `ExportReportExcel`, `ExportReportCsv`)
    - [x] `AlarmController.cs` (actions `ExportAlarmsExcel`, `ExportAlarmsCsv`, `ExportAlarmReportExcel`, `ExportAlarmReportCsv`)
    - [x] `EventController.cs` (actions `ExportEventExcel`, `ExportEventCsv`)

- [x] **Phase 2: Account Management APIs**
  - [x] Implement data models (`AccountDto`, `CreateAccountParam`, `ResetUserPasswordParam`, `UpdateRoleParam`) in `HomeController.cs`.
  - [x] Write `GetAccounts()` API endpoint in `HomeController.cs` to query all rows from the `account` table.
  - [x] Write `CreateAccount()` API endpoint with username uniqueness validation.
  - [x] Write `ResetUserPassword()` API endpoint to update the password of a user.
  - [x] Write `UpdateUserRole()` API endpoint with edge-case validation to ensure at least one Admin remains.

- [x] **Phase 3: Sidebar & Frontend Security Constraints**
  - [x] Wrap the `Cài đặt hệ thống` menu item in `_LayoutMain.cshtml` with a check `Session["Role"] != null && (int)Session["Role"] == (int)Role.Admin` to hide it from Operators.
  - [x] In `Report.cshtml`, `Alarm.cshtml`, and `Event.cshtml`, retrieve the role of the logged-in user and disable/hide the Excel/CSV Export buttons for Operators.

- [x] **Phase 4: User Settings Page UI Makeover**
  - [x] Redesign `Views/Home/UserSetting.cshtml` using a modern SCADA Dark Theme layout.
  - [x] Implement the account list table with search, pagination, and actions.
  - [x] Implement "Create Account" Modal in HTML, CSS, and JS.
  - [x] Implement "Reset Password" Modal in HTML, CSS, and JS.
  - [x] Implement "Change Role" dropdown/trigger with confirmation prompts in JS.
  - [x] Write JS AJAX controllers to interact with `GetAccounts`, `CreateAccount`, `ResetUserPassword`, and `UpdateUserRole`.

- [x] **Phase 5: Verification & Testing**
  - [x] Verify compilation of the .NET Web Application (Successfully built using Visual Studio 2019 MSBuild with 0 errors).
  - [ ] Manually verify Admin capabilities (Create account, Reset password, Change role, Export reports).
  - [ ] Manually verify Operator restrictions (URL blocking, hidden menu, hidden/disabled export buttons, API blocking).
