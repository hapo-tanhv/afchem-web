# Project Roadmap

## Current Sprint (Phase 2 Upgrade Completed)
- [x] Complete security authorization controls to divide Admin and Operator rights.
- [x] Implement complete UserSetting remake with premium high-contrast dark theme.
- [x] Correct Vietnamese font encoding issues across all views by switching them to UTF-8 with BOM.
- [x] Solve `Field 'ID' doesn't have a default value` MySQL error by enabling auto-increment.
- [x] Initialize comprehensive system documentation and test checklists.

## Short Term Plan (30 Days)
- [ ] Run full user acceptance testing (UAT) based on the test checklist in `docs/system-test-checklist.md`.
- [ ] Create automated integration tests for authentication controllers.
- [ ] Implement database connection string extraction from `Authentication.cs` into `Web.config` `appSettings` for better deployment manageability.

## Medium Term Plan (90 Days)
- [ ] Upgrade Frontend elements from Bootstrap 3.4.1 to Bootstrap 5 or newer for modern responsiveness.
- [ ] Implement full SSL encryption for local intranet communication.
- [ ] Optimize database indexing on `alarm` and `event` tables to boost large range queries speed.

## Technical Debt Items
- **Hard-coded MySQL Credentials:** Database connection string is hard-coded in `Authentication.cs` and `HomeController.cs` instead of being read from standard encrypted config files.
- **SQL String Concatenation:** Query execution utilizes string interpolation, posing potential risk if input strings contain special characters.
