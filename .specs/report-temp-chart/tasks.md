# Danh sách Nhiệm vụ (Task List) - Biểu đồ Nhiệt độ trang Báo cáo

Danh sách các bước triển khai chi tiết cho tính năng bổ sung biểu đồ đường nhiệt độ trên trang Báo cáo.

---

## Danh sách Task

### 1. ⚙️ Thiết lập & Phát triển Backend
- [ ] Định nghĩa action method `GetReportChartData` trong `HomeController.cs`.
- [ ] Thêm các tham số lọc: `starttime`, `endtime`, `batchId`, `runId`, `isInitialLoad`, `searchValue`.
- [ ] Sao chép logic thiết lập lô hàng (batch) và mẻ (run) mặc định từ `GetReportData` khi tải lần đầu (`isInitialLoad == true`).
- [ ] Xây dựng truy vấn SQL lấy dữ liệu nhiệt độ từ bảng `alarmreport` sắp xếp theo thời gian tăng dần (`ORDER BY DateTime ASC, ID ASC`).
- [ ] Thiết lập thuật toán lấy mẫu (downsampling) ở Backend giới hạn tối đa 1000 điểm đo.
- [ ] Chuẩn hóa giá trị nhiệt độ bằng hàm helper `TryGetTemp` đồng bộ hoàn toàn với bảng báo cáo.

### 2. 🖥️ Phát triển Frontend
- [ ] Thêm các script CDN Highcharts vào đầu `@section Scripts` ở `Report.cshtml`.
- [ ] Thêm HTML cấu trúc Card AdminLTE chứa container biểu đồ (`#reportChartContainer`) ngay trên bảng dữ liệu.
- [ ] Viết mã Javascript `loadChartData()` để gọi API gửi các tham số lọc hiện tại.
- [ ] Viết mã Javascript `renderChart(data)` cấu hình spline chart, shared tooltip, crosshairs, màu sắc riêng biệt và zoom-x.
- [ ] Tích hợp gọi `loadChartData()` ở cuối handler sự kiện `xhr.dt` trong `initDataTable()`.

### 3. 🧪 Biên dịch & Kiểm tra
- [ ] Chạy lệnh build MSBuild để xác minh dự án biên dịch không có lỗi.
- [ ] Chạy script python chuyển đổi file mã nguồn sang chuẩn định dạng UTF-8 with BOM để tránh lỗi hiển thị tiếng Việt.
- [ ] Kiểm tra giao diện biểu đồ: đóng/mở card, bộ lọc tìm kiếm, tooltip khi di chuột.
