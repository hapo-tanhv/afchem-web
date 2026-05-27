# Kế Hoạch Dọn Dẹp Code Dư Thừa (Code Cleanup Plan)

Tài liệu này chi tiết danh sách các file mã nguồn, giao diện (views), và tài nguyên tĩnh (assets) không còn sử dụng thuộc hệ thống AFCHEM SCADA sẽ được dọn dẹp (xóa bỏ) để tối ưu hóa dự án, đảm bảo giữ lại đúng và đủ 6 trang hoạt động chính: **Overview**, **Alarm**, **Batches (Event)**, **Report**, **UserSetting**, **Login**.

---

## 1. Danh Sách Các File Backend C# (Controllers) Sẽ Xóa

Các Controller dưới đây hoàn toàn không được kế thừa, không được tham chiếu trong bất kỳ Route hay View hoạt động nào của hệ thống:

| File Controller | Đường dẫn vật lý | Trạng thái / Lý do |
|---|---|---|
| `BaseController.cs` | [BaseController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/BaseController.cs) | Không sử dụng. Tất cả controller chính đều kế thừa trực tiếp từ `System.Web.Mvc.Controller`. |
| `BaseProject1Controller.cs` | [BaseProject1Controller.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/BaseProject1Controller.cs) | Legacy code từ dự án/máy cũ, không dùng. |
| `DataInverterController.cs` | [DataInverterController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/DataInverterController.cs) | Legacy code quản lý Inverter mặt trời cũ, không dùng. |
| `DataProjectController.cs` | [DataProjectController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/DataProjectController.cs) | Code cũ truy xuất dữ liệu solar, không dùng. |
| `DataRealTimeController.cs` | [DataRealTimeController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/DataRealTimeController.cs) | Code real-time cũ, không dùng. |
| `DataSignageController.cs` | [DataSignageController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/DataSignageController.cs) | Legacy code của màn hình Signage quảng cáo, không dùng. |
| `LDIPController.cs` | [LDIPController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/LDIPController.cs) | Controller cũ của dự án phụ LDIP, không dùng. |

---

## 2. Danh Sách Giao Diện cshtml (Views) Sẽ Xóa

| File View | Đường dẫn vật lý | Trạng thái / Lý do |
|---|---|---|
| `Home.cshtml` | [Home.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/Home.cshtml) | Giao diện trang chủ mặc định cũ. Hiện tại trang chủ được điều hướng thẳng tới `Overview.cshtml`. |
| `OverviewSignage.cshtml` | [OverviewSignage.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/OverviewSignage.cshtml) | Màn hình SCADA dạng Signage cũ, không nằm trong danh sách 6 trang hoạt động. |

---

## 3. Danh Sách File CSS Sẽ Xóa

| File CSS | Đường dẫn vật lý | Trạng thái / Lý do |
|---|---|---|
| `project.css` | [project.css](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Css/project.css) | Chỉ được import trong `OverviewSignage.cshtml` (trang đã bị xóa). |

---

## 4. Danh Sách File & Thư Mục JavaScript Sẽ Xóa

### A. Trong thư mục `JavaScript/RealTime/`
Các file này chứa logic realtime của các dự án máy phụ (Project 1 - 7) hoặc màn hình Signage cũ, không thuộc phạm vi SCADA hiện tại:

- `JavaScript/RealTime/HomeRealtime.js`
- `JavaScript/RealTime/Project1Realtime.js`
- `JavaScript/RealTime/Project2Realtime.js`
- `JavaScript/RealTime/Project3Realtime.js`
- `JavaScript/RealTime/Project4Realtime.js`
- `JavaScript/RealTime/Project5Realtime.js`
- `JavaScript/RealTime/Project6Realtime.js`
- `JavaScript/RealTime/Project7Realtime.js`
- Các thư mục con (chứa assets/config cũ): `Project1/`, `Project2/`, `Project3/`, `Project4/`, `Project5/`, `Project6/`, `Project7/`, `Signage/`

### B. Trong thư mục `JavaScript/Common/`
Các file này là thư viện JS dùng cho các máy/màn hình signage cũ:

- `JavaScript/Common/EnenrgyPowerCommon.js`
- `JavaScript/Common/Home.js`
- `JavaScript/Common/LayoutSignage.js`
- `JavaScript/Common/Project.js`
- `JavaScript/Common/Sigange.js`
- `JavaScript/Common/Signage1.js`
- `JavaScript/Common/SignageOverview.js`
- `JavaScript/Common/WeatherCommon.js`
- `JavaScript/Common/alarmProject.js`
- `JavaScript/Common/eventProject.js`
- `JavaScript/Common/inverter.js`

---

## 5. Các Thay Đổi Trong Code Của File Hoạt Động (Surgical Cleanups)

Để đảm bảo dự án biên dịch thành công và sạch sẽ hoàn toàn:

### A. Trong [HomeController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/HomeController.cs)
Chúng ta sẽ loại bỏ hoàn toàn các Action Method trỏ tới các trang không dùng:
1. Xóa `public ActionResult Home()`
2. Xóa `public ActionResult OverviewSignage()`

### B. Trong Cấu Hình Project File [LongDucProjectTest.csproj](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/LongDucProjectTest.csproj)
Loại bỏ hoàn toàn các thẻ `<Compile Include="..." />` và `<Content Include="..." />` liên quan đến tất cả các file đã nêu ở mục 1, 2, 3, 4 ở trên để tránh lỗi biên dịch (Compile Error / Missing Files) trên MSBuild/Visual Studio.

---

## 6. Kế Hoạch Xác Minh (Verification Plan)

1. **Biên dịch dự án (MSBuild/Visual Studio Build):**
   - Chạy lệnh build hoặc mở Visual Studio để biên dịch thử dự án nhằm đảm bảo không có lỗi thiếu tham chiếu file (Compile Error).
2. **Kiểm tra hoạt động thực tế:**
   - Đảm bảo 6 trang hoạt động (`Overview`, `Alarm`, `Batches/Event`, `Report`, `UserSetting`, `Login`) vẫn load đầy đủ giao diện, CSS và các file JS tương ứng chạy trơn tru.
