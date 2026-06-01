Feature: Batch-Runs 1-N Upgrade
  As a SCADA System Operator
  I want to view, track, and filter individual production runs (mẻ con) within a batch
  So that I can monitor active runs in real-time and review historic runs accurately.

  Background:
    Given the database is upgraded to support the 1-N Batch-Runs model
    And the batch "TX01-20260601-01" has been created with 3 runs:
      | Run Name                | Run Number | Status    |
      | TX01-20260601-01-Run01  | 1          | Completed |
      | TX01-20260601-01-Run02  | 2          | Active    |
      | TX01-20260601-01-Run03  | 3          | Pending   |

  Scenario: Initial load of the Overview page defaults to the active run
    When the operator navigates to the "Overview" page
    Then the system should fetch the active batch "TX01-20260601-01"
    And detect that "TX01-20260601-01-Run02" is the active run
    And render 3 tabs in the Run Selector:
      | Tab Index | Label                                | CSS Class          |
      | 1         | Run 01 - Hoàn thành                  | completed          |
      | 2         | Run 02 - Đang chạy (Active)          | active-running     |
      | 3         | Run 03 - Chưa thực hiện              | pending            |
    And default-select Tab 2 (Run 02)
    And enable real-time SCADA tag polling for temperature and pressure
    And display live data on the Mixer Diagram.

  Scenario: Operator switches to a completed run to review historic data
    Given the operator is viewing the active run "Run 02" on the "Overview" page
    When the operator clicks on the "Run 01 - Hoàn thành" tab
    Then the system should pause the live SCADA tag polling
    And display a prominent "HISTORIC VIEW" badge on the Mixer Diagram
    And send an AJAX request to "/Overview/GetCurrentBatchStats" with parameters:
      | parameter | value                   |
      | runId     | <ID of Run 01>          |
    And update the 8-stage progress table with static data from Run 01
    And update the temperature/pressure telemetry charts with Run 01 historical records.

  Scenario: Operator switches back to the active run to resume live monitoring
    Given the operator is viewing the completed run "Run 01" in historic view
    When the operator clicks on the "Run 02 - Đang chạy" tab
    Then the system should dismiss the "HISTORIC VIEW" badge from the Mixer Diagram
    And resume the live SCADA tag polling
    And update the 8-stage progress table and telemetry charts with real-time live data.

  Scenario: Operator filters alarms using the cascading dropdowns
    When the operator navigates to the "Alarm" page
    Then Dropdown 1 should show the list of all Batches
    And Dropdown 2 (Run) should be disabled
    When the operator selects Batch "TX01-20260601-01" in Dropdown 1
    Then Dropdown 2 should be enabled
    And the system should call "GET /api/runs?batch_id=<ID of Batch>" via AJAX
    And populate Dropdown 2 with the runs of the selected batch:
      | Option Value | Option Text              |
      | <ID of R01>  | TX01-20260601-01-Run01   |
      | <ID of R02>  | TX01-20260601-01-Run02   |
      | <ID of R03>  | TX01-20260601-01-Run03   |
    When the operator selects "TX01-20260601-01-Run01" in Dropdown 2
    And clicks the "Tìm kiếm" button
    Then the system should display alarm logs filtered strictly by "runId = <ID of Run 01>".

  Scenario: Operator searches a run directly using Autocomplete search
    Given the operator is on the "Report" page
    When the operator types "Run01" in the Autocomplete search bar
    And selects "TX01-20260601-01-Run01" from the suggestions list
    Then the system should set Dropdown 1 to Batch "TX01-20260601-01"
    And enable and set Dropdown 2 to Run "TX01-20260601-01-Run01"
    And automatically load and display the telemetry report for Run 01.
