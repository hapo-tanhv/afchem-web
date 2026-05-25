# Thiết kế tích hợp Bộ lọc & API trang Report (Báo cáo hoạt động)

Tài liệu này chi tiết phương án kỹ thuật để tích hợp API thực tế cho trang Report, thay thế dữ liệu giả lập (mock data) hiện tại bằng dữ liệu thực và hỗ trợ các bộ lọc: Từ ngày (`start`), Đến ngày (`end`), và Chọn mẻ (`batchId`).

---

## 1. Yêu cầu & Hành vi mong muốn (Requirements)
1. **Dữ liệu thực tế:** Loại bỏ dữ liệu mock trong DataTable của trang `Report.cshtml`, thay bằng gọi AJAX thực tế đến `/Home/GetReportData` sử dụng Server-side Processing (vì lượng dữ liệu báo cáo rất lớn).
2. **Hành vi mặc định khi tải trang (Initial Load):** Tự động tìm và hiển thị dữ liệu của mẻ (Batch) trạng thái `Completed` gần nhất (nếu không có thì lấy mẻ `Active` gần nhất). Đồng thời, tự động cập nhật lại ô nhập ngày bắt đầu/kết thúc trên giao diện khớp với ngày thực tế của mẻ đó.
3. **Liên kết bộ lọc:**
   - Khi thay đổi khoảng ngày (Từ ngày/Đến ngày), danh sách chọn mẻ (`batchId`) sẽ tự động được tải lại qua API chỉ chứa các mẻ nằm trong khoảng thời gian đó.
   - Khi chọn mẻ hoặc nhấn nút "Tìm kiếm", DataTable sẽ tự động reload theo bộ lọc hiện tại.

---

## 2. Các thay đổi đề xuất (Proposed Changes)

### A. Phía Backend (`HomeController.cs`)

1. **Thêm API `GetBatches(string starttime, string endtime)`**:
   Tải danh sách mẻ động nằm trong khoảng thời gian được chọn:
   ```csharp
   [HttpGet]
   public JsonResult GetBatches(string starttime, string endtime)
   {
       // Phân tích cú pháp ngày starttime, endtime
       // Truy vấn SQL: WHERE start_time >= start AND start_time <= end
       // Trả về danh sách { id, name, status } dạng JSON
   }
   ```

2. **Cập nhật `GetReportData`**:
   Bổ sung tham số `isInitialLoad` (bool?). Nếu `isInitialLoad == true`:
   - Tìm mẻ `Completed` gần nhất trong DB.
   - Lọc dữ liệu của mẻ đó.
   - Trả về kèm các trường thông tin: `resolvedBatchId`, `batchDateStart`, `batchDateEnd`.

---

### B. Phía Giao diện (`Report.cshtml`)

1. **Cập nhật Dropdown mẻ (`#batchId`)**:
   Thay đổi các option cứng hiện tại thành thẻ rỗng `<option value="0">-- Tất cả --</option>`, sau đó tải dữ liệu động từ API.

2. **Cấu hình Server-side DataTable**:
   Cấu hình DataTable gọi AJAX thực tế đến `/Home/GetReportData`, truyền các tham số bộ lọc (`starttime`, `endtime`, `batchId`, `isInitialLoad`).

3. **Gắn sự kiện Datepicker**:
   Khi người dùng thay đổi "Từ ngày" hoặc "Đến ngày", gọi hàm tải lại dropdown mẻ (`loadBatchesRange`).

4. **Xử lý phản hồi Tải trang đầu tiên**:
   Trong sự kiện `xhr` hoặc `drawCallback` của DataTable, nếu là lần tải đầu tiên (`isInitialLoad == true`), đọc các giá trị `resolvedBatchId`, `batchDateStart`, `batchDateEnd` trả về từ server để thiết lập lại các trường nhập liệu trên giao diện và đặt `isInitialLoad = false`.

---

## 3. Kế hoạch xác thực (Verification Plan)
- **Kiểm tra biên dịch:** Biên dịch lại Solution trong Visual Studio để đảm bảo không có lỗi C#.
- **Kiểm tra giao diện:** Mở trang Report, xác nhận trang hiển thị mẻ Completed gần nhất, danh sách mẻ thay đổi động khi đổi khoảng ngày, và nút "Tìm kiếm" / "Xuất Excel" / "Xuất CSV" hoạt động chuẩn xác với các bộ lọc được chọn.
