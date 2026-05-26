Feature: User Setting and Role Authorization
  As a SCADA System Administrator
  I want to manage user accounts and control screen/action permissions
  So that the system remains secure and Operators cannot perform administrative or modification tasks

  Background:
    Given the database contains the following accounts:
      | UserName | Password | Role  |
      | admin1   | 101101   | Admin |
      | oper1    | 123456   | Operator |

  Scenario: Admin accesses the System Settings page successfully
    Given the current user is logged in as "admin1" with role "Admin"
    When the user navigates to the "/Home/UserSetting" URL
    Then the system should display the "HỆ THỐNG CÀI ĐẶT TÀI KHOẢN" page
    And the account management table should be visible

  Scenario: Operator is blocked from accessing the System Settings page
    Given the current user is logged in as "oper1" with role "Operator"
    When the user navigates to the "/Home/UserSetting" URL
    Then the system should redirect the user to "/Home/Overview"
    And the "Cài đặt hệ thống" menu item should not be visible in the sidebar

  Scenario: Admin creates a new account successfully
    Given the current user is logged in as "admin1" with role "Admin"
    And a user named "oper2" does not exist in the database
    When the Admin opens the "Tạo tài khoản" modal
    And fills in UserName "oper2", Password "pass123", and Role "Operator"
    And clicks the "Lưu" button
    Then the system should save the new account in the database
    And display a success toast "Tạo tài khoản thành công"
    And the account management table should show "oper2"

  Scenario: Admin tries to create a duplicate account and is blocked
    Given the current user is logged in as "admin1" with role "Admin"
    And a user named "oper1" already exists in the database
    When the Admin opens the "Tạo tài khoản" modal
    And fills in UserName "oper1", Password "pass123", and Role "Operator"
    And clicks the "Lưu" button
    Then the system should not save any new account
    And display an error alert "Tên đăng nhập đã tồn tại!"

  Scenario: Admin resets another user's password successfully
    Given the current user is logged in as "admin1" with role "Admin"
    When the Admin clicks the "Reset mật khẩu" button on the row for "oper1"
    And fills in NewPassword "newsecret" and ConfirmPassword "newsecret"
    And clicks the "Lưu" button
    Then the system should update the password of "oper1" in the database to "newsecret"
    And display a success toast "Thay đổi mật khẩu thành công"

  Scenario: Admin updates a user's role successfully
    Given the current user is logged in as "admin1" with role "Admin"
    When the Admin changes the role of "oper1" to "Admin"
    Then the system should update the role of "oper1" to "Admin" in the database
    And display a success message "Cập nhật quyền thành công"

  Scenario: Admin is blocked from downgrading the last Admin in the system
    Given the current user is logged in as "admin1" with role "Admin"
    And "admin1" is the only account in the database with role "Admin"
    When the Admin tries to change the role of "admin1" to "Operator"
    Then the system should reject the update
    And display an error alert "Hệ thống phải có ít nhất một tài khoản Admin!"

  Scenario: Operator is blocked from exporting Excel or CSV files
    Given the current user is logged in as "oper1" with role "Operator"
    When the Operator navigates to the "/Home/Report" page
    Then the "Export Excel" and "Export CSV" buttons should be disabled on the UI
    And if the Operator makes a direct request to "/Home/ExportReportExcel", the backend API should return a Forbidden status
