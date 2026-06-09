# Tài liệu cập nhật hiển thị mẻ bù trừ (Run Compensation) trên trang Overview

## 1. Vấn đề hiện tại
Khi một Batch có cơ chế bù trừ mẻ chạy lỗi:
- Mẻ 1 chạy lỗi (`status = 'Error'`).
- Hệ thống tạo mẻ 3 để chạy bù cho mẻ 1. Mẻ 2 (chưa chạy) được đẩy lùi xuống sau mẻ 3.
- Khi mẻ 3 chạy xong (`status = 'Completed'`), mẻ 2 vẫn chưa chạy (không có mẻ nào đang `Active`).
- **Lỗi hiển thị mẻ trên Overview:** Trang Overview hiển thị thông tin của mẻ 1 (bị lỗi `Error`) thay vì hiển thị mẻ vừa hoàn thành gần nhất (mẻ 3).
- **Lỗi hiển thị BOM:** Dữ liệu bảng BOM sản xuất và tổng lượng nguyên liệu vẫn lấy cả thông tin định mức của mẻ bị lỗi (`Error`).

---

## 2. Giải pháp kỹ thuật đã triển khai

### A. Tối ưu hóa bộ phân giải mẻ/run mặc định (`BatchResolver.cs`)
Chúng tôi đã tái cấu trúc lại helper phân giải mẻ [BatchResolver.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Service/BatchResolver.cs):
1. **Ủy quyền phân giải thống nhất:** Các logic fallback trong Step 3 (khi không có tham số `runId`) giờ đây chỉ tìm kiếm ID của Batch (Batch ID). Việc phân giải Run ID được giao hoàn toàn cho Step 4 để đảm bảo tính nhất quán và áp dụng đúng thứ tự ưu tiên.
2. **Quy tắc ưu tiên (Priority Rules):** Trong Step 4, khi tự động tìm kiếm Run ID thích hợp cho một Batch, chúng tôi truy vấn tất cả các mẻ con của Batch đó và sắp xếp theo độ ưu tiên:
   - **Ưu tiên 1 (Active):** Mẻ đang chạy (`status = 'Active'`). Sắp xếp theo ID giảm dần (lấy mẻ active mới nhất).
   - **Ưu tiên 2 (Completed):** Mẻ đã hoàn thành (`status = 'Completed'`). Sắp xếp theo thời gian kết thúc thực tế (`end_time DESC`) kết hợp ID giảm dần (`id DESC`) để lấy mẻ hoàn thành gần nhất.
   - **Ưu tiên 3 (Pending):** Mẻ chưa chạy/chờ chạy (`status = 'Pending'/'Waiting'/'Created'`). Sắp xếp theo ID tăng dần (`id ASC`) để lấy mẻ tiếp theo trong hàng đợi.
   - **Ưu tiên 4 (Error):** Mẻ chạy lỗi (`status = 'Error'/'Failed'`). Chỉ hiển thị khi Batch không có bất kỳ mẻ Active, Completed hay Pending nào.
   - **Fallback cuối cùng:** Chọn mẻ đầu tiên của lô.

### B. Loại bỏ dữ liệu của mẻ lỗi khỏi BOM sản xuất (`OverviewController.cs`)
Trong phương thức `GetCurrentBatchStats` của [OverviewController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/OverviewController.cs), truy vấn SQL lấy dữ liệu định mức BOM sản xuất đã được cập nhật:
- Bổ sung điều kiện loại bỏ các mẻ có trạng thái `Error`: `AND r.status != 'Error'`
- Câu lệnh SQL mới:
  ```sql
  SELECT ri.code, ri.material_code, ri.quantity, ri.value, ri.unit, ri.batch_no, r.run_number, ri.run_id 
  FROM run_info ri 
  JOIN runs r ON ri.run_id = r.id 
  WHERE r.batch_id = {resolvedBatchId} AND r.status != 'Error' 
  ORDER BY r.run_number ASC, ri.id ASC
  ```
- **Kết quả:** Các mẻ bị lỗi sẽ không xuất hiện trong bảng BOM sản xuất trên giao diện, và tổng lượng nguyên liệu tiêu hao sẽ chỉ tính toán dựa trên các mẻ hợp lệ (không lỗi).

---

## 3. Các tệp đã thay đổi (Modified Files)
* [BatchResolver.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Service/BatchResolver.cs)
* [OverviewController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/OverviewController.cs)
