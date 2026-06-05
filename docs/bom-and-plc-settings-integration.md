# Tài liệu Tích hợp BOM sản xuất và Cài đặt thời gian từ PLC (Overview)

Tài liệu này chi tiết hóa giải pháp kỹ thuật đã triển khai để hiển thị dữ liệu BOM sản xuất từ bảng `run_info`, tích hợp dữ liệu Tổng quan lô sản xuất từ bảng `batches`, và tính toán thời gian chuẩn động từ các thanh ghi PLC.

---

## 1. Hiển thị BOM sản xuất & Tổng quan lô sản xuất

### 1.1. Luồng dữ liệu (BOM)
- **Bảng `run_info`**: Lưu trữ chi tiết định mức nguyên vật liệu của từng Mẻ con (`runs`).
- **Truy vấn C#**: Trong API `GetCurrentBatchStats` của `OverviewController.cs`, hệ thống thực hiện truy vấn động danh sách BOM tương ứng với mẻ con được chỉ định (`resolvedRunId`):
  ```sql
  SELECT code, material_code, quantity, value, unit, batch_no 
  FROM run_info 
  WHERE run_id = [resolvedRunId] 
  ORDER BY id ASC;
  ```
- **Hiển thị giao diện**:
  - Giao diện `Overview.cshtml` được cấu trúc lại với thẻ `<tbody id="bomTableBody">` trống.
  - JavaScript `OverviewRealtime.js` nhận danh sách `bom` từ API Ajax và gọi hàm `renderBOMTable(bom)` để sinh động bảng nguyên vật liệu gồm: Mã nguyên vật liệu (`material_code`), Định lượng tỉ lệ (`quantity`), Giá trị tổng xuất (`value`), Đơn vị tính (`unit`), và LOT nguyên vật liệu (`batch_no`).

### 1.2. Tổng quan Lô (Batch Overview)
- Các thông số trong khung **TỔNG QUAN BATCH** trên giao diện được đồng bộ từ dữ liệu thực tế của bảng `batches` (trước đây là tĩnh):
  - **Sản phẩm**: Cột `product_name` từ bảng `batches`.
  - **Sản lượng kế hoạch**: Cột `target_weight` từ bảng `batches`.
  - **Sản lượng đã sản xuất**: Được tính toán động dựa trên số mẻ con đã hoàn thành (`status = 'Completed'`) nhân với định mức khối lượng từng mẻ (`target_weight / total_runs`).
  - **Thời gian bắt đầu mẻ**: Lấy chính xác thời gian thực tế mẻ bắt đầu từ cột `start_time` trong bảng `batches` (định dạng `HH:mm:ss`). Nếu chưa bắt đầu chạy (giá trị Null/Empty), giao diện hiển thị `-`.
  - **Thời gian còn lại & Dự kiến kết thúc**: Tính toán tự động dựa trên số mẻ con còn lại và Thời gian chuẩn thực tế lấy từ PLC.

---

## 2. Tính toán Thời gian chuẩn động từ các thanh ghi PLC

### 2.1. Cột "TC cài đặt" trong Thống kê mẻ hiện tại
- Trước đây, cột **TC cài đặt** (thời gian chuẩn của từng công đoạn) trong bảng thống kê được hiển thị từ dữ liệu tĩnh cứng (ví dụ: `720s`, `780s`, v.v.).
- **Cải tiến mới**: Khi hệ thống chạy ở chế độ Realtime (không xem dữ liệu lịch sử), cột **TC cài đặt** được cập nhật trực tiếp theo thời gian thực từ các thanh ghi PLC tương ứng:
  1. `AFChemTX01.ThoiGianCaiDatCapLieu` (Cấp liệu)
  2. `AFChemTX01.ThoiGianCaiDatTron1` (Trộn 1)
  3. `AFChemTX01.ThoiGianCaiDatXaDay` (Xả đáy)
  4. `AFChemTX01.ThoiGianCaiDatRungXaDay` (Rung xả đáy)
  5. `AFChemTX01.ThoiGianCaiDatHutXaDayThem` (Hút xả đáy)
  6. `AFChemTX01.ThoiGianCaiDatTron2` (Trộn 2)
  7. `AFChemTX01.ThoiGianCaiDatXaHang` (Xả hàng)
  8. `AFChemTX01.ThoiGianCaiDatRungXaHang` (Rung xả hàng)
- Các giá trị này được lưu trữ trong một đối tượng cache phía frontend JavaScript (`window.plcStandardTimes`).

### 2.2. Thời gian chuẩn trong Trạng thái quy trình
- Ô **Thời gian chuẩn** trong bảng "Trạng thái quy trình" (trước đây là hằng số `1800` hoặc `6300` tĩnh) giờ đây được tính toán động bằng tổng giá trị của 8 thanh ghi cài đặt trên:
  $$\text{Thời Gian Chuẩn} = \sum (\text{Các thanh ghi cài đặt thời gian từ PLC})$$
- Khi bất kỳ thanh ghi nào thay đổi giá trị trên PLC, frontend sẽ bắt sự kiện thay đổi (`valueChanged`), cập nhật cache `window.plcStandardTimes`, tính lại tổng thời gian chuẩn, và tự động vẽ lại cột **TC cài đặt** của bảng thống kê mà không cần tải lại trang.

---

## 3. Tổng kết mã nguồn đã chỉnh sửa

1. **[OverviewController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/OverviewController.cs)**:
   - Bổ sung truy vấn chi tiết Batch (`product_name`, `target_weight`, `total_runs`, `start_time`).
   - Tính toán động khối lượng thực tế sản xuất của Batch (`producedWeight` & `percent`).
   - Query danh sách BOM từ bảng `run_info` theo `run_id` được chọn.
   - Thêm cột `target_weight` cho API danh sách mẻ chạy trong ngày `dailyBatches`.
2. **[Overview.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/Overview.cshtml)**:
   - Bổ sung các ID động (`batchProductName`, `batchTargetWeight`, `batchActualWeight`, `batchEstimatedEndTime`, `batchRemainingTime`) để frontend điền dữ liệu.
   - Tái cấu trúc bảng BOM thành tbody động (`bomTableBody`).
   - Cập nhật hàm `renderBatchTable` hỗ trợ hiển thị giá trị tiêu chuẩn từ cache PLC realtime.
   - Thêm hàm `renderBOMTable` để kết xuất danh sách BOM động.
3. **[OverviewRealtime.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/RealTime/OverviewRealtime.js)**:
   - Khai báo cache toàn cục `window.plcStandardTimes` cho các thanh ghi cài đặt PLC.
   - Gắn hàm callback cập nhật cache PLC và vẽ lại giao diện khi các thanh ghi cài đặt thay đổi.
   - Tính toán động Thời gian chuẩn và Thời gian còn lại/Dự kiến kết thúc của Lô dựa trên dữ liệu PLC.
   - Tích hợp gọi Ajax cập nhật BOM và Batch Overview tự động mỗi chu kỳ hoặc khi đổi mẻ con.
   - Cập nhật logic phân giải thứ tự ưu tiên (Priority Resolution):
     1. Ưu tiên 1: Batch đang chạy (`Active`) và mẻ chạy con đang thực thi (`runs.status = 'Active'`).
     2. Ưu tiên 2: Nếu không có mẻ nào chạy, chọn Batch có mẻ con vừa hoàn thành gần nhất (`runs.status = 'Completed'` sắp xếp theo `end_time DESC, id DESC`).
     3. Ưu tiên 3: Nếu không có mẻ nào Done, chọn Batch đang ở trạng thái chờ (`Pending`) trong ngày. Đối với Batch đang Pending, hiển thị mẻ con đầu tiên (cũ nhất, sắp xếp theo `id ASC`).
     4. Fallback cuối cùng: Chọn Batch mới nhất trong hệ thống và hiển thị mẻ con đầu tiên.
   - Cập nhật định dạng dữ liệu trả về của BOM sản xuất: gồm các cột Mã hàng (`code`), Tên nguyên vật liệu (`material_code`), Tổng xuất (`quantity`), Đơn vị tính (`unit`), và Lô/Số (`batch_no`).


---

## 4. Sửa lỗi Giao diện (Layout Grid Nesting) bị thu hẹp bên phải

### 4.1. Hiện tượng lỗi
- Toàn bộ phần thân bên dưới (từ biểu đồ Thống kê số liệu, Thống kê mẻ hiện tại, đến các bảng BOM và Batches ngày) bị dồn về bên trái và thu hẹp chiều rộng, để lại khoảng trống màu đen lớn (khoảng 350px) ở bên phải màn hình.
- Đồng hồ gauge và biểu đồ đường cũng bị co cụm lại với độ rộng nhỏ (600px).

### 4.2. Nguyên nhân
- Trong `Overview.cshtml` tại thẻ hiển thị **Độ ẩm môi trường** (dòng 405-412), thiếu thẻ đóng `</div>` cho thẻ chứa `.param-card-info` và `.param-card-side`.
- Sự thiếu hụt này làm hỏng cấu trúc phân cấp DOM: Trình duyệt hiểu nhầm toàn bộ phần giao diện phía sau (gồm cả biểu đồ, bảng biểu) là phần tử con nằm trong cột bên phải `.right-panels-col` (vốn có độ rộng cố định là 350px).

### 4.3. Giải pháp khắc phục
1. **[Overview.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/Overview.cshtml)**:
   - Bổ sung 2 thẻ `</div>` đóng hoàn chỉnh cho thẻ đo Độ ẩm môi trường.
   - Loại bỏ 1 thẻ đóng `</div>` thừa ở cuối tệp (dòng 573) để khôi phục cân bằng thẻ DOM cho layout chính.
2. **[OverviewRealtime.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/RealTime/OverviewRealtime.js)**:
   - Gọi phương thức `.reflow()` trên các biểu đồ sau khi khởi tạo (300ms) và sau khi tải dữ liệu động từ Ajax (100ms) để các biểu đồ tự động mở rộng theo kích thước 100% của cột Grid mới được giải phóng.
