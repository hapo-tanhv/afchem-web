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

### C. Hiển thị thông tin ghi chú trạng thái lỗi trên trang Event (`EventController.cs`, `EventPage.js`, `Event.css`)
Trong phương thức `GetEventLogRealtime` của [EventController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/EventController.cs), chúng tôi đã cập nhật logic sinh các nhãn trạng thái và ghi chú đánh giá chất lượng cho các mẻ bị lỗi hoặc thất bại (`Error` hoặc `Failed`):
1. **Trạng thái chu kỳ (`statusLabel`):**
   * Nếu mẻ bị lỗi, nhãn sẽ hiển thị là `"Chu kỳ bị lỗi"` thay vì nhãn mặc định `"Chu kỳ hoàn tất thành công"`.
2. **Ghi chú chất lượng (`note`):**
   * Nếu mẻ bị lỗi, ghi chú sẽ hiển thị là `"Chu kỳ bị lỗi. Chất lượng sản phẩm: KHÔNG ĐẠT (LỖI)"` với màu chữ đỏ tương ứng của lớp `text-danger`, giúp người vận hành nhận biết nhanh các mẻ chạy không thành công.
3. **Hiển thị Icon lỗi của mẻ trên Event Page (Cập nhật 11/06/2026):**
   * Trong [EventPage.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/Event/EventPage.js), trước đây khi mẻ bị lỗi (`status` = `Error`/`Failed`), hệ thống vẫn hiển thị icon tích xanh (`fas fa-check`) tương tự như mẻ đã hoàn thành bình thường.
   * Đã cập nhật logic kiểm tra trạng thái mẻ trong `renderCycleSummary`. Nếu mẻ có trạng thái `Error` hoặc `Failed` (không phân biệt hoa thường), iconClass sẽ đổi thành `failed` và hiển thị icon dấu X đỏ (`fas fa-times`) bọc trong vòng tròn đỏ.
   * Thêm class CSS `.evt-cycle-icon.failed` trong [Event.css](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Css/Event.css) để tô đỏ cả màu nền, viền và icon của trạng thái lỗi này đồng nhất với bảng cảnh báo.

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
     * Sửa lỗi cộng dồn sai lệch (ví dụ ra 1201.2 KG thay vì ~1000 KG) bằng cách lọc các bản ghi định mức nguyên liệu chỉ tính đơn vị khối lượng (`'kg'`) trong câu truy vấn SUM, tránh cộng dồn các vật tư đóng gói tính bằng đơn vị `chiếc` (như túi nilon, tem, dây thắt).
     * Cập nhật số liệu `totalRuns` và `completedRuns` trên giao diện theo số lượng mẻ hợp lệ (không lỗi), giúp hiển thị tỷ lệ % sản lượng chính xác (100% khi tất cả các mẻ thành công hoàn tất, kể cả khi có mẻ bù).
3. **Định dạng thời gian bắt đầu và dự kiến kết thúc (Tổng quan Batch):**
   * Trong [OverviewController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/OverviewController.cs), trả về thời gian bắt đầu lô dạng đầy đủ `yyyy-MM-dd HH:mm:ss`.
   * Trong [OverviewRealtime.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/RealTime/OverviewRealtime.js), định dạng lại "Thời gian bắt đầu" và "Thời gian dự kiến kết thúc" thành `dd/MM/yyyy HH:mm` bằng hàm trợ giúp `formatDateTimeString`.
4. **Sửa lỗi hiển thị sản lượng hiện tại trên Header:**
   * Loại bỏ cơ chế cộng cứng `500` KG cho mỗi lô hoàn thành ở file [LayoutMain.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/Common/LayoutMain.js). Thay vào đó, backend tự động tính toán chính xác sản lượng đã đạt được của mỗi lô (`producedWeight` tính từ các mẻ con thành công) và truyền sang để JS cộng dồn động lên Header.
5. **Sửa lỗi lệch Thời gian còn lại khi mẻ vừa hoàn thành:**
   * Trong [OverviewRealtime.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/RealTime/OverviewRealtime.js), khi các tag cài đặt thời gian chuẩn từ PLC thay đổi, hàm `updateCalculatedTimeAndStandards` cập nhật thời gian chuẩn trên giao diện nhưng không tính lại thời gian còn lại. Điều này dẫn tới hiện tượng lệch giao diện tạm thời (ví dụ: Chuẩn = 365s, Đã chạy = 405s, nhưng Còn lại vẫn giữ giá trị cũ của 6300s là 5895s).
   * Đã bổ sung logic tính toán lại và cập nhật `#statRemainingTime` ngay lập tức trong `updateCalculatedTimeAndStandards` để đưa giá trị còn lại về `0` (hoặc giá trị đúng theo chuẩn mới) một cách tức thời, không bị trễ theo chu kỳ polling.

### G. Tự động reset ô nhập liệu "Tìm nhanh mẻ" khi chọn Batch hoặc Mẻ (Cập nhật 11/06/2026)
1. **Vấn đề:**
   * Trên các trang **Alarm**, **Event**, và **Report**, có tính năng tìm kiếm nhanh theo mẻ ("Tìm nhanh mẻ" - `#runSearch`).
   * Khi người dùng thay đổi thủ công lựa chọn lô (Batch) hoặc mẻ con (Run) qua dropdown, nội dung đã gõ trong ô "Tìm nhanh mẻ" không được reset, có thể gây hiểu lầm về bộ lọc hiện tại.
2. **Giải pháp:**
   * Bổ sung trình lắng nghe sự kiện thay đổi (`change`) trên các phần tử `#batchId` và `#runId` tại các file giao diện (hoặc file JavaScript điều khiển tương ứng).
   * Khi các dropdown này được thay đổi thủ công trên giao diện, ô nhập liệu `#runSearch` sẽ được gán lại giá trị rỗng (`""`).
   * Việc xóa này không tự động kích hoạt lọc hay tải lại dữ liệu mà chỉ hỗ trợ đồng bộ mặt giao diện; dữ liệu sẽ chỉ được cập nhật lại khi người dùng chủ động nhấn nút **"Tìm kiếm"**.
### H. Sửa đổi hiển thị số thập phân và hướng sắp xếp trang Báo cáo (Cập nhật 11/06/2026)
1. **Lỗi hiển thị số thập phân (Nhiệt độ/Độ ẩm/Áp suất):**
   * **Vấn đề:** Do cấu hình định dạng vùng trên máy chủ Windows là tiếng Việt (sử dụng dấu phẩy `,` làm dấu phân cách thập phân và dấu chấm `.` làm dấu phân cách phần nghìn), hàm `double.TryParse` mặc định trong `TryGetDouble` hiểu sai dấu chấm của chuỗi dữ liệu (ví dụ: `"32.2"`) thành phân cách phần nghìn, dẫn tới hiển thị sai thành `322`.
   * **Giải pháp:** Chuyển đổi phương thức `TryGetDouble` trong [HomeController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/HomeController.cs) sang sử dụng `CultureInfo.InvariantCulture` để luôn phân tích chính xác dấu chấm làm dấu thập phân bất kể cấu hình Windows của máy chủ.
2. **Thay đổi hướng sắp xếp bảng Báo cáo:**
   * **Vấn đề:** Dữ liệu trang Báo cáo trước đây được sắp xếp giảm dần (mới nhất hiển thị trước - `DateTime DESC`). Người dùng muốn chuyển sang sắp xếp tăng dần (cũ nhất hiển thị trước - `DateTime ASC`).
   * **Giải pháp:** Thay đổi mệnh đề `ORDER BY a.DateTime DESC, a.ID DESC` thành `ORDER BY a.DateTime ASC, a.ID ASC` trong các phương thức của `HomeController.cs`: `GetReportData` (trả về JSON cho màn hình), `ExportReportExcel` (xuất file Excel), và `ExportReportCsv` (xuất file CSV).

### I. Cấu hình sắp xếp danh sách mẻ con (GetRuns API) theo thứ tự ID tăng dần (Cập nhật 11/06/2026)
1. **Vấn đề:**
   * API lấy danh sách mẻ con của một lô `/Overview/GetRuns` trước đây được sắp xếp theo `ORDER BY run_number ASC`.
   * Người dùng muốn danh sách mẻ con trả về được sắp xếp tăng dần theo ID của mẻ con (`ORDER BY id ASC`).
2. **Giải pháp:**
   * Thay đổi câu truy vấn SQL trong phương thức `GetRuns` của [OverviewController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/OverviewController.cs) từ `ORDER BY run_number ASC` thành `ORDER BY id ASC`.
   * Thay đổi này sẽ ảnh hưởng đồng bộ tới dropdown chọn mẻ (Run select) trên cả 4 giao diện (Overview, Alarm, Event, Report) vốn dùng chung API này.

### J. Bổ sung số lượng cảnh báo hiển thị trên Sidebar (Cập nhật 11/06/2026)
1. **Vấn đề:**
   * Người dùng muốn xem trực tiếp số lượng cảnh báo đang hoạt động ngay trên Sidebar ở vị trí cạnh phải của mục "Cảnh báo" để dễ theo dõi.
   * Số lượng này cần giống hệt với giá trị hiển thị ở góc trên bên phải Header (bên cạnh icon quả chuông).
2. **Giải pháp:**
   * Cập nhật [_LayoutMain.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Shared/_LayoutMain.cshtml): Thêm một thẻ `<span class="right badge" id="sidebarAlarmCount">` vào phần tử `<p>` của liên kết Menu "Cảnh báo", cấu hình màu nền của badge là `#f59e0b` và màu chữ tối tương phản `#111827`.
   * Cập nhật [LayoutMain.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/Common/LayoutMain.js): Trong hàm `updateHeaderStats`, đồng thời cập nhật cả `#alarmCount` (trên Header) và `#sidebarAlarmCount` (trên Sidebar) với cùng giá trị cảnh báo lấy được từ API để đảm bảo đồng bộ thời gian thực.

---

## 3. Các tệp đã thay đổi (Modified Files)
* [BatchResolver.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Service/BatchResolver.cs)
* [OverviewController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/OverviewController.cs)
* [EventController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/EventController.cs)
* [HomeController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/HomeController.cs)
* [Overview.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/Overview.cshtml)
* [OverviewRealtime.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/RealTime/OverviewRealtime.js)
* [Alarm.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/Alarm.cshtml)
* [EventPage.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/Event/EventPage.js)
* [Event.css](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Css/Event.css)
* [Report.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/Report.cshtml)
* [_LayoutMain.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Shared/_LayoutMain.cshtml)
* [LayoutMain.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/Common/LayoutMain.js)
