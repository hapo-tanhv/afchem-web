# Thiết kế kỹ thuật: Cài đặt tài khoản & Phân quyền (UserSetting & Role Authorization)

## 1. Tổng quan Kiến trúc (Architectural Overview)

Mục tiêu thiết kế là thay đổi cấu trúc vai trò trong hệ thống chỉ còn 2 Role chính là `Admin` và `Operator`, đồng thời phát triển các API phía Backend để hỗ trợ xem danh sách tài khoản, tạo tài khoản mới, reset mật khẩu và cập nhật quyền, song song với việc thắt chặt bảo mật ở cả Frontend và Backend.

```
+--------------------------+        +--------------------------+        +--------------------+
|   UserSetting.cshtml     | <====> |    HomeController.cs     | <====> |    MySQL Server    |
| (AJAX List/Create/Reset) |  JSON  |  (GetAccounts / Create)  |  SQL   | (scada DB account) |
+--------------------------+        +--------------------------+        +--------------------+
```

---

## 2. Thay đổi Cơ sở dữ liệu & Cấu trúc mã nguồn (Database & Schema Changes)

### 2.1. Cập nhật Enum Role (`Hino.Getdata.Common/Authentication.cs`)
Xóa bỏ tất cả các role cũ không sử dụng, định nghĩa lại `Role` chỉ gồm:
- `None = 0`
- `Admin = 1`
- `Operator = 2`

### 2.2. Bảng dữ liệu CSDL `scada.account`
Bảng `account` trong database MySQL `scada` được truy xuất trực tiếp để lưu trữ thông tin tài khoản:
- `ID` (int, Primary Key, Auto Increment)
- `UserName` (varchar, Unique)
- `Password` (varchar)
- `Role` (int) - Lưu giá trị 1 (Admin) hoặc 2 (Operator).

---

## 3. Thiết kế Backend (C# Cải tiến & API mới)

Bổ sung các API quản lý tài khoản và thực hiện kiểm tra quyền Admin trong `HomeController.cs`, `AlarmController.cs`, `EventController.cs`.

### 3.1. Dọn dẹp HomeController.cs
- **Đăng nhập (`Login` POST):** Chuyển hướng cả Admin và Operator (Role = 1 và Role = 2) đến trang `/Home/Overview`.
- **Dọn dẹp kiểm tra Role cũ:** Thay đổi các ViewBag.DisplayAdmin và kiểm tra role trong các action `Home`, `Overview`, `OverviewSignage`, `Alarm`, `Event`, `UserSetting` để chỉ kiểm tra `Session["Role"] == Role.Admin`.
- **Action `UserSetting`:** Chỉ cho phép truy cập nếu `Session["Role"] != null` và `(int)Session["Role"] == (int)Role.Admin`. Nếu là Operator, redirect sang `/Home/Overview`.

### 3.2. Viết thêm các API quản lý tài khoản trong `HomeController.cs`

1. **`GetAccounts` (HttpGet):**
   - Trả về danh sách tài khoản trong hệ thống.
   - SQL: `SELECT ID, UserName, Role FROM account`
   - Chỉ cho phép `Admin` truy cập.

2. **`CreateAccount` (HttpPost):**
   - Tạo tài khoản mới.
   - Kiểm tra xem username đã tồn tại chưa: `SELECT COUNT(*) FROM account WHERE UserName = '{param.UserName}'`
   - SQL: `INSERT INTO account (UserName, Password, Role) VALUES ('{param.UserName}', '{param.Password}', {param.Role})`
   - Chỉ cho phép `Admin` truy cập.

3. **`ResetUserPassword` (HttpPost):**
   - Reset mật khẩu cho user (bởi Admin).
   - SQL: `UPDATE account SET Password = '{param.NewPassword}' WHERE UserName = '{param.UserName}'`
   - Chỉ cho phép `Admin` truy cập.

4. **`UpdateUserRole` (HttpPost):**
   - Đổi vai trò tài khoản.
   - Thao tác an toàn (Edge case check): Nếu chuyển từ Admin sang Operator, bắt buộc phải kiểm tra hệ thống còn ít nhất 1 Admin khác hay không.
   - SQL đếm Admin: `SELECT COUNT(*) FROM account WHERE Role = 1`
   - SQL cập nhật: `UPDATE account SET Role = {param.NewRole} WHERE UserName = '{param.UserName}'`
   - Chỉ cho phép `Admin` truy cập.

### 3.3. Bảo mật API Xuất Báo cáo (Excel / CSV)
Trong các Action Export ở các Controller, kiểm tra `Session["Role"]`. Nếu vai trò không phải Admin, chặn thao tác và ném lỗi 403 Forbidden.
- **`HomeController.cs`:**
  - `ExportReportExcel`
  - `ExportReportCsv`
- **`AlarmController.cs`:**
  - `ExportAlarmsExcel`
  - `ExportAlarmsCsv`
  - `ExportAlarmReportExcel`
  - `ExportAlarmReportCsv`
- **`EventController.cs`:**
  - `ExportEventExcel`
  - `ExportEventCsv`

---

## 4. Thiết kế Giao diện (Frontend - HTML & JS Cải tiến)

### 4.1. Ẩn menu Sidebar (`_LayoutMain.cshtml`)
Ẩn mục cài đặt hệ thống đối với Operator:
```html
@if (Session["Role"] != null && (int)Session["Role"] == (int)Hino.Getdata.Common.Role.Admin)
{
    <li class="nav-item">
        <a href="~/Home/UserSetting" class="nav-link @(ViewBag.ButtonUser)">
            <i class="nav-icon fas fa-cog"></i>
            <p>Cài đặt hệ thống</p>
        </a>
    </li>
}
```

### 4.2. Cập nhật giao diện `UserSetting.cshtml`
- Thay thế form đổi mật khẩu cũ bằng:
  - Một nút "Tạo tài khoản mới" (Create Account) thiết kế hiện đại.
  - Một bảng Bootstrap tinh gọn với DataTables để hiển thị danh sách tài khoản gồm cột: STT, Tên đăng nhập, Vai trò, Hành động.
  - Modal "Tạo tài khoản mới" hỗ trợ nhập UserName, Password và chọn Role.
  - Modal "Reset mật khẩu" hỗ trợ nhập mật khẩu mới và xác nhận mật khẩu cho tài khoản tương ứng.
  - Hỗ trợ đổi Role trực tiếp qua dropdown/nút trên dòng tương ứng (kèm theo xác nhận confirm trước khi đổi).

### 4.3. Ẩn/Vô hiệu hóa nút Export trên UI (Report, Alarm, Event)
Tại các trang `Report.cshtml`, `Alarm.cshtml`, `Event.cshtml`:
- Lấy thông tin Role của user đăng nhập từ `Session["Role"]`.
- Nếu role không phải là `Admin` (tức là Operator), ẩn hoàn toàn các nút có class `btn-export` (hoặc disabled chúng) để Operator không thể nhấp vào.
