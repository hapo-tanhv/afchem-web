# Walkthrough Kết quả & Hướng dẫn Xác thực (Verification Walkthrough)

Tính năng quản lý tài khoản, thay đổi mật khẩu và phân quyền (Admin / Operator) cho dự án **AFCHEM SCADA** đã được thực thi và hoàn thiện 100%.

---

## 1. Các file đã thay đổi (Files Changed)

### 1.1. Backend C#
1. **`Hino.Getdata.Common/Authentication.cs`**
   - Rút gọn enum `Role` chỉ còn: `None = 0`, `Admin = 1` và `Operator = 2`.
2. **`LongDucProjectTest/Controllers/HomeController.cs`**
   - Bổ sung 4 API quản lý tài khoản: `GetAccounts`, `CreateAccount`, `ResetUserPassword`, `UpdateUserRole` (đều kiểm tra quyền Admin).
   - Thêm nghiệp vụ biên chặn việc hạ cấp Admin cuối cùng trong `UpdateUserRole`.
   - Cập nhật logic điều hướng `Login` và các ViewBag kiểm tra quyền `Admin` duy nhất.
   - Thêm chặn 403 ở các API xuất Excel/CSV báo cáo: `ExportReportExcel`, `ExportReportCsv`.
3. **`LongDucProjectTest/Controllers/AlarmController.cs`**
   - Thêm chặn 403 ở 6 API xuất báo cáo cảnh báo dành cho Operator.
4. **`LongDucProjectTest/Controllers/EventController.cs`**
   - Thêm chặn 403 ở 2 API xuất báo cáo sự kiện mẻ dành cho Operator.

### 1.2. Frontend Views (HTML/JS)
1. **`Views/Shared/_LayoutMain.cshtml`**
   - Ẩn menu "Cài đặt hệ thống" đối với Operator.
   - Hiển thị động tên đăng nhập của User hiện tại trên Profile Toggle ở Header thay vì hardcode "Admin".
2. **`Views/Home/UserSetting.cshtml`**
   - Thiết kế lại 100% giao diện sang phong cách Premium SCADA Dark Theme.
   - Hỗ trợ xem danh sách, tìm kiếm tài khoản, phân trang bằng DataTables.
   - Tích hợp 2 Modal: Tạo tài khoản mới, Reset mật khẩu Admin.
   - Hỗ trợ đổi Role trực tiếp qua Dropdown dòng dữ liệu (kèm confirm trước khi đổi).
3. **`Views/Home/Report.cshtml`, `Alarm.cshtml`, `Event.cshtml`**
   - Ẩn container chứa nút `Export Excel` và `Export CSV` đối với Operator.

---

## 2. Hướng dẫn Xác thực kiểm thử (Manual Verification Guide)

Bạn có thể chạy thử hệ thống và xác thực tính năng theo các bước sau:

### 2.1. Đăng nhập tài khoản Admin
1. Mở trang Đăng nhập và đăng nhập với tài khoản **Admin** (ví dụ: `admin` mật khẩu `101101`).
2. Trên góc phải Header, tên người dùng sẽ hiển thị chính xác là **admin**.
3. Menu **Cài đặt hệ thống** sẽ xuất hiện ở Sidebar bên trái. Các nút **Export Excel**, **Export CSV** xuất hiện đầy đủ ở các trang Report, Alarm, Event.
4. Nhấp vào **Cài đặt hệ thống**:
   - Bạn sẽ nhìn thấy danh sách tất cả tài khoản trong hệ thống.
   - Click nút **Tạo tài khoản mới**, nhập UserName `operator1`, mật khẩu `123456`, chọn Role `Operator`, click **Lưu**. Hệ thống sẽ báo "Tạo tài khoản thành công" qua Toast thông báo góc phải màn hình, và bảng sẽ tự động tải lại có user `operator1`.
   - Click nút **Đổi mật khẩu** tại dòng `operator1`, nhập mật khẩu mới, click **Lưu**.
   - Tại dòng của bạn (`admin`), hãy thử chọn đổi vai trò sang `Operator`. Hệ thống sẽ hiển thị cảnh báo và từ chối nếu đây là Admin duy nhất: **"Hệ thống phải có ít nhất một tài khoản Admin!"**.

### 2.2. Đăng nhập tài khoản Operator
1. Thực hiện Đăng xuất và Đăng nhập với tài khoản **operator1** vừa tạo.
2. Trên góc phải Header, tên người dùng hiển thị là **operator1**.
3. Menu **Cài đặt hệ thống** sẽ **biến mất hoàn toàn** khỏi Sidebar.
4. Nếu cố tình gõ trực tiếp URL `/Home/UserSetting` trên thanh địa chỉ trình duyệt, hệ thống sẽ tự động chặn và **Redirect ngược trở lại trang Overview**.
5. Truy cập các trang **Báo cáo**, **Cảnh báo**, **Batches (Event)**: Các nút **Export Excel / Export CSV** ở góc phải thanh bộ lọc sẽ **bị ẩn hoàn toàn** khỏi màn hình.
6. Nếu cố tình gửi request trực tiếp bằng URL của API Export (ví dụ: `/Home/ExportReportExcel`), trình duyệt sẽ nhận được mã phản hồi **403 Forbidden** và không tải xuống được file.
