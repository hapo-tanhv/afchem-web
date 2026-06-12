# Nhật ký Triển khai & Kết quả Kiểm thử (Walkthrough & Validation Results)

---

## 1. Các thay đổi đã thực hiện (Changes Made)

Chúng tôi đã hoàn thành triển khai tính năng Lựa chọn Batch Chạy Linh hoạt (flexible-batch-execution) trên cả Frontend và Backend:

### Backend (ASP.NET MVC Controller)
- **File sửa đổi:** [OverviewController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/OverviewController.cs)
- **API `GetStandbyBatchesAndRuns`:** Truy vấn tất cả các Batch đang ở trạng thái `Active`/`Pending` có chứa các mẻ chạy chưa thực hiện (trạng thái `Pending`, `Waiting`, `Created`).
- **API `SelectBatchRun`:**
  - Kiểm tra xem có mẻ nào đang ở trạng thái `Active` hay không. Nếu có, từ chối thay đổi để đảm bảo an toàn.
  - Sử dụng Database Transaction để cập nhật: chuyển batch cũ hoạt động về `Pending`, chuyển batch mới chọn thành `Active`, kích hoạt mẻ chạy được chọn thành `Active` (ghi nhận `start_time` là thời gian hiện tại) và gán số thứ tự chạy `execution_order` tăng dần bằng `MAX(execution_order) + 1`.

### Frontend (Giao diện & Tương tác)
- **File sửa đổi:** [Overview.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/Overview.cshtml)
  - Thêm card điều khiển "Lựa chọn Batch" ở trên cùng trang Overview.
  - Hiển thị thống kê số lượng batch chờ và mẻ chờ bên trái, bộ chọn (dropdown) và nút bấm kích hoạt bên phải.
- **File sửa đổi:** [OverviewRealtime.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/RealTime/OverviewRealtime.js)
  - Tải dữ liệu các mẻ chờ chạy và đổ vào dropdown, hỗ trợ lọc động danh sách mẻ con khi chọn lô hàng.
  - Gọi hàm `checkActiveStatusAndLockControls` khi có polling dữ liệu realtime (mỗi 30 giây): tự động khóa bộ chọn và hiển thị nhãn cảnh báo nếu phát hiện có mẻ đang chạy (`Active`), mở khóa khi mẻ kết thúc.
  - Gửi yêu cầu kích hoạt mẻ chạy qua AJAX POST và hiển thị cảnh báo xác nhận/thành công.

---

## 2. Kết quả kiểm tra biên dịch (Compilation Results)
Chúng tôi đã thực hiện build project bằng công cụ MSBuild và xác minh:
- **Trạng thái biên dịch:** Thành công hoàn toàn (`Build succeeded`).
- **Số lượng lỗi:** `0 Error(s)`.

---

## 3. Kịch bản kiểm thử đề xuất (UAT Manual Verification)

Vui lòng kiểm thử thủ công trên trình duyệt theo các bước sau để xác nhận tính năng hoạt động:

1. **Khởi tạo dữ liệu:** Đảm bảo trong database có ít nhất 2 Batch chờ chạy (trạng thái `Pending`/`Active`) có các mẻ con chưa chạy.
2. **Kiểm tra trạng thái bộ chọn:**
   - Nếu đang có mẻ chạy hoạt động (`Active`), khối Lựa chọn Batch ở đầu trang phải chuyển sang trạng thái bị khóa (`disabled`) và hiển thị biểu tượng ổ khóa kèm dòng chữ cảnh báo màu vàng ở góc phải.
   - Nếu không có mẻ chạy nào hoạt động, bộ chọn phải được mở khóa bình thường.
3. **Thao tác chọn mẻ chạy:**
   - Chọn một Batch bất kỳ từ hộp chọn "Chọn Batch".
   - Kiểm tra hộp chọn "Chọn Mẻ" tự động cập nhật hiển thị đúng danh sách các mẻ con tương ứng của Batch vừa chọn.
4. **Kích hoạt mẻ chạy mới:**
   - Chọn Batch và Mẻ mong muốn chạy kế tiếp, click "Bắt đầu mẻ".
   - Nhấn **OK** trên hộp thoại xác nhận.
   - Kiểm tra thông báo Toast/Alert thành công.
   - Toàn bộ giao diện Overview phải tự động chuyển sang hiển thị và thống kê dữ liệu của mẻ chạy vừa kích hoạt mà không cần reload trang.
   - Kiểm tra trong database: Batch mới chọn và Mẻ mới chọn chuyển thành `status = 'Active'`, mẻ mới có `execution_order` lớn nhất.
