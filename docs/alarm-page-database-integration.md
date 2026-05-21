# Hướng dẫn Tích hợp Cơ sở Dữ liệu và Phân trang phía Server cho Trang Cảnh báo

Tài liệu này ghi lại chi tiết các thay đổi đã được thực hiện để chuyển đổi dữ liệu giả (mock data) trên trang Cảnh báo (Alarm Board) sang sử dụng dữ liệu thực tế từ cơ sở dữ liệu MySQL (`realtime_alarms` & `batches`) kết hợp với cơ chế **Phân trang phía Server (Server-Side Pagination)** cho hiệu suất tối ưu.

---

## 1. Yêu cầu & Tiêu chí Thiết kế

1. **Hiển thị Tên Batch**: Sử dụng liên kết bảng (`INNER JOIN batches ON realtime_alarms.batchId = batches.id`) để lấy `batches.name` hiển thị lên bảng thay vì hiển thị ID số nguyên đơn thuần.
2. **Loại bỏ các record không có Batch**: Nếu `batchId` bằng `NULL` hoặc không tồn tại khớp trong bảng `batches`, dòng đó sẽ không được hiển thị (sử dụng `INNER JOIN`).
3. **Lọc bỏ Cảnh báo có độ nghiêm trọng là INFO**: Chỉ tải dữ liệu có `Severity IN ('ALARM', 'WARNING')`.
4. **Phân trang phía Server**: Chuyển đổi toàn bộ bảng DataTables sang sử dụng chế độ `"serverSide": true` và gọi API thông qua HTTP POST.
5. **Bộ lọc nâng cao**: Tìm kiếm và lọc chính xác theo khoảng thời gian (`starttime` - `endtime`) và lọc theo Batch (`batchId`).

---

## 2. Các thành phần đã được thay đổi

### A. Backend: [AlarmController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/AlarmController.cs)

* **`GetBatches` [GET]**: Trả về danh sách tất cả các batch từ bảng `batches` (được sắp xếp theo ID giảm dần) dưới dạng JSON để hiển thị động lên thẻ `<select>` ở giao diện.
* **`GetAlarmsData` [POST]**:
  * Nhận các tham số tìm kiếm: `starttime`, `endtime`, `batchId`, `draw`, `start`, `length`.
  * Thực hiện câu lệnh SQL đếm tổng bản ghi khớp với điều kiện lọc để trả về `recordsFiltered` và `recordsTotal`.
  * Chạy truy vấn lấy dữ liệu có phân trang: `LIMIT {length} OFFSET {start}` kết hợp `ORDER BY a.DateTime DESC, a.id DESC`.
  * Tính toán trạng thái cảnh báo dựa trên giá trị cột `restore_time`:
    * Nếu `restore_time IS NULL` -> Trạng thái: `"Cảnh báo"`.
    * Nếu `restore_time IS NOT NULL` -> Trạng thái: `"Đã khôi phục"`.
* **`ExportAlarmsExcel` & `ExportAlarmsCsv` [GET]**:
  * Được nâng cấp để truy vấn dữ liệu trực tiếp từ cơ sở dữ liệu thật dựa trên các tham số bộ lọc hiện tại.
  * Định nghĩa Class `AlarmExportDto` giúp kiểm soát chính xác tên các cột và định dạng dữ liệu khi xuất ra file CSV và Excel.

### B. Giao diện: [Alarm.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/Alarm.cshtml)

* **Danh sách Batch động**: Khi tải trang, AJAX sẽ gọi tới `/Alarm/GetBatches` để nạp danh sách các batch thực tế từ DB vào thẻ `<select id="batchId">`.
* **Cấu hình DataTables AJAX**:
  * Cài đặt `"processing": true`, `"serverSide": true`.
  * Truyền động các tham số bộ lọc (`starttime`, `endtime`, `batchId`) thông qua hàm `data` của cấu hình AJAX.
* **Hiển thị STT Toàn cục (Global Index)**:
  * Thay vì hiển thị số thứ tự cục bộ từ `1` đến `10` trên mỗi trang, hàm render STT được cập nhật thành `meta.settings._iDisplayStart + meta.row + 1` để đảm bảo STT tăng tiến liên tục giữa các trang (ví dụ: trang 2 hiển thị `11` - `20`).
* **Badge Trạng thái & Màu sắc Dòng**:
  * Giữ nguyên giao diện cao cấp (Premium Aesthetics) với các dòng màu cam cho `"Cảnh báo"` và màu xanh lá cây cho `"Đã khôi phục"`.

---

## 3. Xác nhận & Kiểm tra (Verification)

* **Build**: Dự án đã được build/compile lại hoàn tất bằng MSBuild với **0 lỗi (0 Errors)**.
* **Cơ sở dữ liệu**: Đã cấu hình và kết nối chính xác tới MySQL (`scada` DB tại `localhost` với mật khẩu `101101`).
