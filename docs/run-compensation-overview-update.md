# Tài liệu cập nhật hiển thị mẻ bù trừ (Run Compensation) và định dạng nhiệt độ (chia 10) trên trang Overview và trang Event

## 1. Vấn đề hiện tại
Khi một Batch có cơ chế bù trừ mẻ chạy lỗi:
- Mẻ 1 chạy lỗi (`status = 'Error'`).
- Hệ thống tạo mẻ 3 để chạy bù cho mẻ 1. Mẻ 2 (chưa chạy) được đẩy lùi xuống sau mẻ 3.
- Khi mẻ 3 chạy xong (`status = 'Completed'`), mẻ 2 vẫn chưa chạy (không có mẻ nào đang `Active`).
- **Lỗi hiển thị mẻ trên Overview:** Trang Overview hiển thị thông tin của mẻ 1 (bị lỗi `Error`) thay vì hiển thị mẻ vừa hoàn thành gần nhất (mẻ 3).
- **Lỗi hiển thị BOM:** Dữ liệu bảng BOM sản xuất và tổng lượng nguyên liệu vẫn lấy cả thông tin định mức của mẻ bị lỗi (`Error`).
- **Lỗi hiển thị Ghi chú trên Event:** Khi chọn xem mẻ lỗi (`Error`), trang Event hiển thị nhầm nhãn trạng thái và ghi chú chất lượng là "Chu kỳ hoàn tất thành công" và "Chất lượng sản phẩm: ĐẠT".
- **Lỗi hiển thị nhiệt độ (chưa chia 10):** Trong bảng "Thống kê mẻ hiện tại" và các mô tả cảnh báo của từng bước, nhiệt độ thực tế lấy từ database hiển thị dạng số nguyên thô (ví dụ: `350` °C thay vì `35.0` °C) do chưa được chia cho 10.
- **Lỗi hiển thị Lô chạy xuyên ngày:** Khi một Lô được tạo/bắt đầu hôm qua nhưng hôm nay mới chạy nốt mẻ còn lại, Lô đó biến mất khỏi bảng "Tổng số batch sản xuất trong ngày" hôm nay (vì chỉ lọc theo ngày bắt đầu Lô).

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

### C. Hiển thị thông tin ghi chú trạng thái lỗi trên trang Event (`EventController.cs`)
Trong phương thức `GetEventLogRealtime` của [EventController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/EventController.cs), chúng tôi đã cập nhật logic sinh các nhãn trạng thái và ghi chú đánh giá chất lượng cho các mẻ bị lỗi hoặc thất bại (`Error` hoặc `Failed`):
1. **Trạng thái chu kỳ (`statusLabel`):**
   * Nếu mẻ bị lỗi, nhãn sẽ hiển thị là `"Chu kỳ bị lỗi"` thay vì nhãn mặc định `"Chu kỳ hoàn tất thành công"`.
2. **Ghi chú chất lượng (`note`):**
   * Nếu mẻ bị lỗi, ghi chú sẽ hiển thị là `"Chu kỳ bị lỗi. Chất lượng sản phẩm: KHÔNG ĐẠT (LỖI)"` với màu chữ đỏ tương ứng của lớp `text-danger`, giúp người vận hành nhận biết nhanh các mẻ chạy không thành công.

### D. Định dạng trị số nhiệt độ trong Thống kê mẻ hiện tại và Cảnh báo
Để sửa lỗi các giá trị nhiệt độ hiển thị dạng số nguyên thô (ví dụ: `350` thay vì `35.0` °C):
1. **Trong `OverviewController.GetCurrentBatchStats`:**
   * Khi lấy dữ liệu nhiệt độ từ bảng lịch sử `alarmreport`, các giá trị `NhietDoBonTronTren`, `NhietDoBonTronGiua`, `NhietDoBonTronDuoi` được chia cho `10.0` để định dạng về kiểu số thực có 1 chữ số phần thập phân.
   * Các đỉnh nhiệt độ từ `realtime_alarms` để ghép vào dải nhiệt độ từng bước cũng được chia cho `10.0`.
   * Các ngưỡng cảnh báo nhiệt độ (`topThreshold`, `midThreshold`, `botThreshold`) được chia `10.0` để đảm bảo logic tô đỏ hoạt động đúng.
2. **Trong thông báo chi tiết của Cảnh báo (`GetCurrentBatchStats`, `GetRecentAlarms`, `GetEventLogRealtime`):**
   * Đối với các tag đo lường nhiệt độ (`NhietDo`), giá trị đo được (`val`) và giá trị ngưỡng (`threshold`) hiển thị trong chuỗi `detailMessage` được tự động chia cho `10.0` trước khi kết xuất thông báo.

### E. Mở rộng bộ lọc Lô trong ngày và Cảnh báo mẻ còn thiếu chạy xuyên ngày
1. **Hiển thị Lô chạy xuyên ngày:**
   * Cập nhật lại câu lệnh SQL lấy Lô sản xuất trong ngày tại trang Overview. Ngoài việc lọc theo ngày tạo/ngày bắt đầu lô là hôm nay, hệ thống lọc thêm:
     * Các Lô đang ở trạng thái `Active`.
     * Hoặc các Lô có bất kỳ mẻ con nào chạy (bắt đầu hoặc kết thúc) trong ngày hôm nay.
   * Nhờ đó, Lô chạy xuyên ngày sẽ luôn hiển thị trong bảng "Tổng số batch sản xuất trong ngày" của cả hai ngày hoạt động.
2. **Banner Cảnh báo mẻ còn thiếu:**
   * Trong phương thức `GetCurrentBatchStats`, hệ thống tự động quét cơ sở dữ liệu để kiểm tra xem có Lô đang chạy (`batches.status = 'Active'`) nào được bắt đầu từ ngày hôm trước (`DATE(start_time) < CURDATE()`) nhưng vẫn còn các mẻ con chưa chạy xong (trạng thái `Pending`/`Waiting`/`Created`).
   * Nếu phát hiện, hệ thống tự động sinh ghi chú dạng: `Batch đang chạy (tên batch) ngày [ngày bắt đầu], mẻ còn thiếu chưa chạy (tên mẻ)`.
   * Trên giao diện [Overview.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/Overview.cshtml) và [OverviewRealtime.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/RealTime/OverviewRealtime.js), chúng tôi bổ sung một Banner cảnh báo màu cam nổi bật đặt ngay trên phần **"Trạng thái quy trình"**. Banner này sẽ tự động hiện lên khi có mẻ chưa chạy xuyên ngày và ẩn đi khi tất cả các mẻ của Lô trước đó đã hoàn tất.
   * *Cập nhật sửa lỗi (10/06/2026):* Khắc phục hiện tượng banner trống hiển thị đè lên giao diện khi chưa có dữ liệu bằng cách loại bỏ thuộc tính `display: flex;` trùng lặp trong thuộc tính `style` nội tuyến của phần tử `#pendingRunBanner`, chỉ giữ lại `display: none;` lúc khởi tạo. Đồng thời, thay đổi query parameter cache-buster cho file script `OverviewRealtime.js` từ chuỗi tĩnh `?v=now()` thành `@DateTime.Now.Ticks` để buộc trình duyệt nạp lại mã script mới nhất chứa logic xử lý ẩn/hiện động.

### F. Cải tiến hiển thị thời gian và sản lượng trên Overview (Cập nhật 10/06/2026)
1. **Thời gian sản xuất (bảng tổng số lô):**
   * Định dạng trường `durationStr` trong [OverviewController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/OverviewController.cs) được chuyển sang định dạng đầy đủ ngày giờ: `dd/MM/yyyy HH:mm - dd/MM/yyyy HH:mm` (sử dụng `CultureInfo.InvariantCulture`).
2. **Hiển thị sản lượng cộng dồn (Tổng quan Batch):**
   * Thay vì chia trung bình cơ học thô sơ (`target_weight / total_runs` nhân với số mẻ completed), hệ thống truy vấn tổng định mức thực tế từ `run_info` cho từng mẻ con:
     * Loại bỏ hoàn toàn các mẻ lỗi (`status = 'Error'` hoặc `'Failed'`).
     * Cộng dồn chính xác khối lượng của các mẻ đã hoàn thành (`status = 'Completed'`) dựa trên định mức BOM chi tiết của mẻ đó.
     * Cập nhật số liệu `totalRuns` và `completedRuns` trên giao diện theo số lượng mẻ hợp lệ (không lỗi), giúp hiển thị tỷ lệ % sản lượng chính xác (100% khi tất cả các mẻ thành công hoàn tất, kể cả khi có mẻ bù).
3. **Định dạng thời gian bắt đầu và dự kiến kết thúc (Tổng quan Batch):**
   * Trong [OverviewController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/OverviewController.cs), trả về thời gian bắt đầu lô dạng đầy đủ `yyyy-MM-dd HH:mm:ss`.
   * Trong [OverviewRealtime.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/RealTime/OverviewRealtime.js), định dạng lại "Thời gian bắt đầu" và "Thời gian dự kiến kết thúc" thành `dd/MM/yyyy HH:mm` bằng hàm trợ giúp `formatDateTimeString`.

---

## 3. Các tệp đã thay đổi (Modified Files)
* [BatchResolver.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Service/BatchResolver.cs)
* [OverviewController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/OverviewController.cs)
* [EventController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/EventController.cs)
* [Overview.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/Overview.cshtml)
* [OverviewRealtime.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/RealTime/OverviewRealtime.js)
