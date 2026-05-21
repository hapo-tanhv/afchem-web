Feature: Current Batch Statistics
  As a SCADA System Operator
  I want to view real-time batch step statuses, timing, temperatures, and alarms
  So that I can monitor the production process accurately without manual estimation

  Scenario: A step is pending (no telemetry data yet)
    Given the current batch ID is 1
    And there are no records for step 3 "Xả đáy" (CongDoanMay = 3) in the alarmreport table
    When the system fetches current batch statistics
    Then the step "Xả đáy" status should be "pending"
    And the display status text should be "Chưa thực hiện"
    And all time, duration, and temperature metrics for this step should be "-"

  Scenario: A step is in progress
    Given the current batch ID is 1
    And the latest records in alarmreport have CongDoanMay = 2
    When the system fetches current batch statistics
    Then the step "Trộn 1" (CongDoanMay = 2) status should be "in-progress"
    And the display status text should be "Đang thực hiện"
    And the Start Time should be formatted as HH:mm:ss based on the earliest record for CongDoanMay = 2
    And the End Time and Duration should be "-"

  Scenario: A step is completed
    Given the current batch ID is 1
    And there are records for CongDoanMay = 1
    And there are also records for a later step CongDoanMay = 2
    When the system fetches current batch statistics
    Then the step "Cấp liệu" (CongDoanMay = 1) status should be "completed"
    And the display status text should be "Hoàn thành"
    And the Start Time and End Time should be formatted as HH:mm:ss
    And the Duration should be calculated as the difference in seconds between End Time and Start Time

  Scenario: Displaying temperature min-max ranges
    Given a completed step has multiple telemetry records
    And the mixer middle temperatures are between 32°C and 35°C
    When the system fetches current batch statistics
    Then the "NĐ bồn giữa" for this step should display "32-35°C"

  Scenario: Displaying single temperature value when no variation
    Given a newly started step has exactly one telemetry record
    And the mixer top temperature is 30°C
    When the system fetches current batch statistics
    Then the "NĐ bồn trên" for this step should display "30°C"

  Scenario: Step-specific alarms are retrieved
    Given the current batch ID is 1
    And there is an alarm in realtime_alarms with Severity "ALARM", batchId 1, and CongDoan "Cấp liệu"
    When the system fetches current batch statistics
    Then the step "Cấp liệu" should have 1 alert in its list
    And the alert title should match the Message column
    And the alert type should be "ALARM"
