# Kế hoạch Triển khai: Lựa chọn Batch Chạy Linh hoạt (flexible-batch-execution)

Tài liệu này chi tiết hóa các bước thực hiện, kế hoạch kiểm thử và theo dõi tiến độ tích hợp tính năng lựa chọn Batch và Mẻ chạy linh hoạt tại trang Overview.

---

## 1. Trạng thái Nhiệm vụ (Task Status)

- [x] **1. Phát triển API Backend**
  - [x] 1.1 Thêm endpoint `GetStandbyBatchesAndRuns` [HttpGet] vào `OverviewController.cs` để lấy danh sách standby.
  - [x] 1.2 Thêm endpoint `SelectBatchRun` [HttpPost] vào `OverviewController.cs` để kích hoạt mẻ chạy, kiểm tra điều kiện an toàn, và cập nhật `execution_order = MAX(execution_order) + 1` bằng Database Transaction.
- [x] **2. Phát triển Giao diện Frontend UI**
  - [x] 2.1 Thiết kế và chèn HTML panel điều khiển "Lựa chọn Batch" ở phía trên cùng trang `Overview.cshtml`.
  - [x] 2.2 Viết mã CSS và các phong cách inline đồng bộ phong cách SCADA Dark Theme hiện tại.
  - [x] 2.3 Lập trình logic JS trong `OverviewRealtime.js`:
    - Gọi API `/Overview/GetStandbyBatchesAndRuns` khi khởi động trang để nạp dropdowns.
    - Lọc động danh sách Run khi thay đổi dropdown Batch.
    - Khóa/Mở khóa (Disable/Enable) bộ chọn và nút khi có mẻ đang `Active` dựa trên dữ liệu polling từ `GetCurrentBatchStats` kèm opacity và tooltip.
    - Hiển thị modal xác nhận và gửi yêu cầu AJAX POST kích hoạt mẻ chạy mới, sau đó gọi load realtime ngay lập tức.
- [ ] **3. Xác minh và Kiểm thử**
  - [ ] 3.1 Kiểm tra luồng hoạt động chuyển đổi mẻ chạy trên môi trường local.
  - [ ] 3.2 Xác minh tính toàn vẹn của dữ liệu trong bảng `batches` và `runs` trên MySQL.

---

## 2. Thiết kế Kỹ thuật Chi tiết

### Backend API (C#)
1. `/Overview/GetStandbyBatchesAndRuns`
   - Truy vấn các Batch có trạng thái `Active` hoặc `Pending`.
   - Với mỗi Batch, truy vấn các Run có trạng thái `Pending`, `Waiting`, hoặc `Created`.
   - Trả về danh sách lồng kèm theo tổng số lượng Batch và Run chờ.

2. `/Overview/SelectBatchRun`
   - Nhận `batchId` và `runId`.
   - Kiểm tra xem có mẻ nào có `status = 'Active'` trong bảng `runs` không. Nếu có -> Báo lỗi.
   - Nếu không có, thực hiện cập nhật:
     - `UPDATE batches SET status = 'Pending' WHERE status = 'Active'`
     - `UPDATE batches SET status = 'Active' WHERE id = {batchId}`
     - `UPDATE runs SET status = 'Active', start_time = NOW(), execution_order = (SELECT COALESCE(MAX(execution_order), 0) + 1 FROM runs) WHERE id = {runId}` (Sử dụng subquery để tìm Max Order an toàn).

### Frontend UI (HTML/CSS/JS)
- Vị trí Panel: Đặt trên cùng của màn hình Overview (ngay trên thẻ Trạng thái quy trình).
- Trạng thái Khóa: Lắng nghe trạng thái mẻ chạy từ `GetCurrentBatchStats`. Nếu mẻ chạy hoạt động (Active), khóa toàn bộ control.
