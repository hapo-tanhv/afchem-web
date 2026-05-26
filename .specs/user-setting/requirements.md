# Yêu cầu Hệ thống: Cài đặt tài khoản & Phân quyền (UserSetting & Role Authorization)

## 1. Giới thiệu & Mục tiêu (Introduction & Goals)
Mục tiêu là cải tiến toàn diện màn hình **Cài đặt hệ thống (UserSetting)**, chuyển từ giao diện đổi mật khẩu đơn giản hiện tại thành một hệ thống Quản lý Tài khoản (Account Management) chuyên nghiệp hỗ trợ:
1. Xem danh sách tài khoản hiện tại (Account List).
2. Tạo tài khoản mới (Create Account) qua giao diện Modal.
3. Thay đổi mật khẩu tài khoản bất kỳ (Admin Reset Password) qua giao diện Modal.
4. Thay đổi quyền của tài khoản (Change Role) trực tiếp.
5. Áp dụng cơ chế Phân quyền (Authorization) chặt chẽ giữa 2 nhóm tài khoản **Admin** và **Operator** ở cả Frontend và Backend.

---

## 2. Chi tiết Phân quyền (Role Authorization Details)

Hệ thống hỗ trợ 2 nhóm quyền chính:

### 2.1. Admin (Role = 1)
- Có toàn quyền truy cập và thao tác trên mọi màn hình.
- Nhìn thấy và truy cập được menu **Cài đặt hệ thống (UserSetting)**.
- Có thể tạo tài khoản mới, reset mật khẩu của bất kỳ user nào, và chuyển đổi Role của user khác.
- Có quyền thực hiện các thao tác Action nhạy cảm và **Xuất báo cáo (Export Excel, CSV)** trên tất cả các màn hình (Overview, Alarm, Event, Report).

### 2.2. Operator (Role = 2)
- **Hạn chế truy cập:** Không nhìn thấy menu "Cài đặt hệ thống" ở sidebar và bị chặn truy cập trực tiếp bằng URL (nếu cố tình vào sẽ bị Redirect về trang Overview).
- **Hạn chế Action:** Chỉ có quyền xem dữ liệu (Read-only), không thực hiện được các Action thay đổi cấu hình hay dữ liệu.
- **Hạn chế Xuất file:** Không được phép xuất Excel, CSV ở bất kỳ trang nào. Các nút bấm Export trên giao diện sẽ bị ẩn hoặc vô hiệu hóa (disabled), đồng thời API Backend sẽ chặn tải xuống nếu vai trò không phải Admin.

---

## 3. Giao diện người dùng (UI/UX Requirements)

Thiết kế giao diện tuân thủ phong cách thiết kế hiện tại của hệ thống **AFCHEM SCADA** (Premium Dark Theme, hài hòa, hiện đại với tông màu tối và viền màu Cyan đặc trưng).

### 3.1. Trang quản trị UserSetting.cshtml
Thay thế form đổi mật khẩu cũ bằng layout quản trị:
- **Header:** Tiêu đề "HỆ THỐNG CÀI ĐẶT TÀI KHOẢN" (Account Administration) nổi bật.
- **Nút "Tạo tài khoản":** Nút bấm màu xanh dương/Cyan nằm góc trên bên phải bảng dữ liệu, khi click sẽ mở Modal Tạo tài khoản.
- **Bảng dữ liệu tài khoản (Account Table):**
  - Cột: `STT` (Số thứ tự), `Tên đăng nhập (UserName)`, `Vai trò (Role)`, `Hành động (Actions)`.
  - Hỗ trợ ô tìm kiếm (Search) tài khoản nhanh theo UserName và phân trang (Pagination).
  - Cột hành động chứa các icon/button:
    - **Đổi mật khẩu:** Nút mở Modal reset mật khẩu cho tài khoản đó.
    - **Đổi quyền:** Hỗ trợ thay đổi Role của user (phải check điều kiện an toàn).
    - *Lưu ý:* Không hiển thị nút Xóa tài khoản (yêu cầu không cho phép xóa tài khoản).

### 3.2. Modal Tạo tài khoản (Create Account Modal)
Mở ra khi click nút "Tạo tài khoản" với các trường:
- `Tên đăng nhập (UserName)` (Text input, bắt buộc, không dấu, không khoảng trắng, duy nhất).
- `Mật khẩu (Password)` (Password input, bắt buộc).
- `Vai trò (Role)` (Dropdown Select: `Admin` và `Operator`).
- Nút `Lưu` và `Hủy bỏ`.

### 3.3. Modal Reset mật khẩu (Reset Password Modal)
Mở ra khi click nút đổi mật khẩu trên dòng tương ứng:
- `Tên đăng nhập (UserName)` (Text input, bị vô hiệu hóa - readonly, hiển thị tên user được chọn).
- `Mật khẩu mới (NewPassword)` (Password input, bắt buộc).
- `Xác nhận mật khẩu (ConfirmPassword)` (Password input, bắt buộc).
- Nút `Lưu` và `Hủy bỏ`.

---

## 4. Ràng buộc Nghiệp vụ biên (Edge Cases & Safety Rules)
- **Kiểm tra UserName trùng lặp:** Khi tạo tài khoản mới, hệ thống bắt buộc phải kiểm tra trong cơ sở dữ liệu xem UserName đã tồn tại chưa. Nếu đã tồn tại, trả về lỗi hiển thị trên giao diện dạng Toast/Alert.
- **Ràng buộc số lượng Admin tối thiểu:** Khi một Admin thực hiện thay đổi Role của tài khoản khác (ví dụ chuyển một Admin khác thành Operator), hệ thống bắt buộc phải kiểm tra số lượng Admin hiện tại trong DB. **Luôn luôn phải tồn tại ít nhất 1 Admin** trong hệ thống. Hệ thống không cho phép thực hiện hành động nếu nó làm mất đi Admin cuối cùng.
- **Chặn Operator action:** Operator không thể gọi các API chỉnh sửa mật khẩu, đổi role, tạo tài khoản.

---

## 5. Thiết kế Kỹ thuật & API Backend (C# & SQL)

### 5.1. Cập nhật Enum Role (`Authentication.cs`)
Thay thế toàn bộ danh sách vai trò cũ, chỉ giữ lại `None`, `Admin` và `Operator` (với giá trị là 2):
```csharp
public enum Role
{
    None = 0,
    Admin = 1,
    Operator = 2
}
```

### 5.2. Các API cần tạo mới/bổ sung trong `HomeController.cs`

1. **`GetAccounts()` (HttpGet - JsonResult):**
   - Trả về danh sách tất cả tài khoản trong bảng `account` (chỉ lấy ID, UserName, Role).
   - Kiểm tra phân quyền: Chỉ cho phép tài khoản có `Session["Role"] == Role.Admin` truy xuất.

2. **`CreateAccount(AccountParam param)` (HttpPost - JsonResult):**
   - Tạo tài khoản mới.
   - Kiểm tra `Session["Role"] == Role.Admin`.
   - Kiểm tra xem UserName đã tồn tại chưa.
   - Thực hiện insert vào bảng `account`.

3. **`ResetUserPassword(ResetPasswordParam param)` (HttpPost - JsonResult):**
   - Reset mật khẩu cho tài khoản khác (bởi Admin).
   - Kiểm tra `Session["Role"] == Role.Admin`.
   - Cập nhật mật khẩu mới của user mà không cần nhập mật khẩu cũ.

4. **`UpdateUserRole(UpdateRoleParam param)` (HttpPost - JsonResult):**
   - Thay đổi vai trò của user.
   - Kiểm tra `Session["Role"] == Role.Admin`.
   - Nếu thay đổi vai trò làm giảm số lượng Admin (chuyển một Admin thành Operator), phải đếm xem có bao nhiêu tài khoản Admin còn lại (`Role = 1`). Nếu chỉ còn 1 Admin duy nhất, từ chối thực hiện và trả về lỗi: `"Hệ thống phải có ít nhất một tài khoản Admin!"`.

### 5.3. Kiểm soát Trang và Chức năng Xuất Báo cáo
- **Menu Sidebar (`_LayoutMain.cshtml`):**
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
- **Chặn Direct URL (`HomeController.UserSetting()`):**
  ```csharp
  public ActionResult UserSetting()
  {
      if (Session["Role"] is null) return RedirectToAction("Login", "Home");
      
      // Chỉ cho phép Admin vào trang cài đặt
      if ((int)Session["Role"] != (int)Role.Admin)
      {
          return RedirectToAction("Overview", "Home");
      }
      
      ViewBag.ButtonUser = "active";
      ViewBag.DisplayAdmin = "block";
      return View();
  }
  ```
- **Nút Xuất Báo cáo (Frontend - Report, Alarm, Event):**
  Ẩn hoặc disabled các nút `Export Excel` / `Export CSV` nếu `Session["Role"]` của user hiện tại không phải `Role.Admin`.
- **API Xuất Báo cáo (Backend):**
  Trong các Action `ExportReportExcel`, `ExportReportCsv` trong `HomeController` (và các API Export tương tự ở `AlarmController`, `EventController`), bổ sung kiểm tra phân quyền:
  ```csharp
  if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
  {
      return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Bạn không có quyền thực hiện hành động này.");
  }
  ```
