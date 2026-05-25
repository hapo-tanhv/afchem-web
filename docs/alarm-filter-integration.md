# Thiết kế tích hợp Bộ lọc & API trang Alarm (Cảnh báo)

Tài liệu này chi tiết phương án kỹ thuật để tích hợp bộ lọc mẻ mặc định khi tải trang Alarm, đảm bảo đồng bộ hóa trải nghiệm với trang Batches và Report.

---

## 1. Yêu cầu & Hành vi mong muốn (Requirements)
1. **Dữ liệu thực tế và Phân giải mẻ mặc định:**
   - Khi mới tải trang Alarm (`isInitialLoad == true`), hệ thống tự động kiểm tra xem có mẻ nào đang `Active` hay không.
   - Nếu không có mẻ `Active` nào, hệ thống tự động tìm và tải dữ liệu mẻ `Completed` gần đây nhất (nếu vẫn không có thì lấy mẻ mới nhất toàn hệ thống).
   - Tự động cập nhật ngày trên thanh bộ lọc (datepicker) khớp với ngày thực tế của mẻ đó trên giao diện.
2. **Lọc mẻ theo khoảng ngày:**
   - Thay đổi ô chọn mẻ từ tải tĩnh toàn bộ mẻ thành tải động dựa theo khoảng ngày `starttime` và `endtime` mà người dùng chọn.
   - Lắng nghe sự kiện đổi ngày để tự động làm mới danh sách mẻ dropdown.
3. **Báo cáo chính xác (Export):**
   - Hỗ trợ xuất dữ liệu Excel/CSV cảnh báo đồng bộ theo mẻ mặc định hoặc mẻ được lọc, đồng thời khắc phục triệt để lỗi hiển thị font tiếng Việt nhờ thuộc tính `CharSet=utf8;` trong chuỗi kết nối MySQL.

---

## 2. Các thay đổi đã thực hiện (Implementation)

### A. Phía Backend (`AlarmController.cs`)
1. **Nâng cấp `GetBatches(string starttime, string endtime)`**:
   Hỗ trợ lọc danh sách mẻ nằm trong khoảng thời gian được truyền vào thay vì lấy toàn bộ mẻ.
2. **Nâng cấp `GetAlarmsData`**:
   Bổ sung tham số `isInitialLoad` và logic tự động xác định mẻ đang `Active` hoặc mẻ `Completed` gần nhất. Trả về thông tin: `resolvedBatchId`, `batchDateStart`, `batchDateEnd` cùng với danh sách dữ liệu cảnh báo.
3. **Bổ sung `CharSet=utf8;`**:
   Áp dụng thuộc tính mã hóa Unicode UTF-8 cho tất cả các chuỗi kết nối trong các hàm lấy dữ liệu cảnh báo và xuất báo cáo (`ExportAlarmsExcel`, `ExportAlarmsCsv`).

### B. Phía Giao diện (`Alarm.cshtml`)
1. **Liên kết Datepicker & Dropdown**:
   Khi người dùng thay đổi ngày trên datepicker, hàm `loadBatchesRange()` sẽ được kích hoạt để làm mới dropdown mẻ.
2. **DataTable Listener (`xhr.dt`)**:
   Khi nhận phản hồi đầu tiên từ server với `isInitialLoad == true`, tự động đọc thông tin mẻ mặc định được phân giải để thiết lập lại khoảng ngày và mẻ tương ứng trên giao diện một cách đồng bộ.
