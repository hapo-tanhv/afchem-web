# Danh Sách Kiểm Thử Hệ Thống (System Testing Checklist)
## Dự án: AFCHEM SCADA System

Tài liệu này tổng hợp toàn bộ các kịch bản kiểm thử (test cases), kịch bản biên và kiểm tra bảo mật phân quyền dành cho 5 trang chức năng cốt lõi của hệ thống **AFCHEM SCADA**: **Tổng quan, Cảnh báo, Batches (Sự kiện), Báo cáo, và Cài đặt hệ thống**.

---

## 💬 Hướng dẫn chung về Môi trường & Tài khoản
Trước khi thực hiện kiểm thử, cần chuẩn bị sẵn 2 tài khoản đại diện cho hai vai trò:
1. **Tài khoản Admin:** `admin` / `101101` (Quyền cao nhất: Xem, cấu hình, quản lý tài khoản, xuất báo cáo).
2. **Tài khoản Operator:** `operator_test` / `123456` (Quyền vận hành: Chỉ xem thông số trực quan, bị ẩn toàn bộ nút xuất dữ liệu và trang Cài đặt hệ thống).

---

## 📊 1. TRANG TỔNG QUAN (OVERVIEW)
*Mục tiêu: Đảm bảo sơ đồ bồn trộn real-time, biểu đồ thống kê và thông tin mẻ hoạt động liên tục, chính xác.*

### 1.1. Kiểm thử Tính năng Real-time & Sơ đồ Thiết bị
- [ ] **Thanh quy trình (Timeline Steps):** Kiểm tra 8 bước quy trình sản xuất (Cấp liệu, Trộn 1, Xả đáy, Rung xả đáy, Hút xả đáy, Trộn 2, Xả hàng, Rung xả hàng).
  - [ ] Bước đang chạy có được highlight màu xanh sáng (`row-active` / `.in-progress`) không?
  - [ ] Các bước đã hoàn thành có dấu tích xanh hoàn thành không?
- [ ] **Thông số thời gian (Step Stats):**
  - [ ] Thời gian đã chạy (Elapsed Time) tăng đều đặn mỗi giây.
  - [ ] Thời gian còn lại (Remaining Time) giảm tương ứng và khớp với tiến độ bước (%).
- [ ] **Thông số bồn trộn (Diagram Parameters):**
  - [ ] Nhiệt độ trên, giữa, dưới của bồn hiển thị đúng số liệu thực tế từ cảm biến.
  - [ ] Áp suất đường ống hiển thị trị số chính xác (đơn vị `bar`).
  - [ ] Thời gian hoạt động của các thiết bị (Bơm, Máy trộn, Van xả) khớp với chu trình thực tế.
- [ ] **Thông số môi trường:** Nhiệt độ, độ ẩm và chỉ số bụi mịn PM2.5 được cập nhật liên tục từ trạm thời tiết.

### 1.2. Kiểm thử Biểu đồ & Widget Cảnh báo
- [ ] **Solid Gauges (Đồng hồ kim):** 3 đồng hồ đo Tốc độ khuấy, Nhiệt độ và Áp suất vẽ bằng Highcharts hoạt động mượt mà, kim quay đúng dải đo.
- [ ] **Line Chart (Biểu đồ đường):** Biểu đồ lịch sử thông số hiển thị đúng chuỗi dữ liệu thời gian, hỗ trợ bật/tắt các series dữ liệu khi nhấp vào legend.
- [ ] **Widget Cảnh báo nhanh:**
  - [ ] Hiển thị danh sách tối đa 5 cảnh báo mới nhất.
  - [ ] Phân biệt rõ cấp độ cảnh báo bằng màu sắc: `ALARM` (Đỏ - nguy hiểm) và `WARNING` (Vàng - cảnh báo).
  - [ ] Nhấp vào nút "Xem tất cả" chuyển hướng chính xác đến trang Cảnh báo (`/Home/Alarm`).

### 1.3. Bảng Thống kê Mẻ & BOM Sản xuất
- [ ] **Thống kê mẻ hiện tại:** Bảng danh sách các công đoạn hiển thị đầy đủ thời gian bắt đầu, kết thúc, nhiệt độ bồn trên/giữa/dưới.
  - [ ] Dòng cảnh báo có thể click để mở rộng xem chi tiết lỗi (thời gian xảy ra, ngưỡng vượt...).
  - [ ] **Định dạng nhiệt độ (chia 10):** Xác nhận các trị số nhiệt độ bồn trên, giữa, dưới, đỉnh nhiệt độ, và các ngưỡng cảnh báo nhiệt độ được chia cho `10.0` để hiển thị đúng dạng số thực (ví dụ: `35.0` °C thay vì `350` °C).
  - [ ] **Thứ tự ưu tiên hiển thị mẻ (BatchResolver):** Khi không chỉ định cụ thể, mẻ được chọn mặc định để hiển thị phải tuân thủ đúng thứ tự ưu tiên của bộ phân giải: `Active` (đang chạy) $\rightarrow$ `Completed` (hoàn thành gần nhất) $\rightarrow$ `Pending` (chờ chạy đầu tiên) $\rightarrow$ `Error` (chỉ hiển thị khi không còn mẻ nào khác).
- [ ] **BOM Sản xuất (Loại trừ mẻ lỗi):** Hiển thị đúng danh sách nguyên vật liệu (NVL), tỉ lệ, tổng xuất thực tế và mã LOT tương ứng của mẻ hiện tại.
  - [ ] Xác nhận các mẻ bị lỗi (`status = 'Error'`) hoàn toàn bị loại bỏ khỏi bảng BOM sản xuất trên giao diện.
  - [ ] Tổng lượng nguyên liệu tiêu hao chỉ được tính toán dựa trên các mẻ hợp lệ (không bao gồm mẻ lỗi).
- [ ] **Tổng số Batch trong ngày & Lô chạy xuyên ngày:**
  - [ ] Bảng "Tổng số batch sản xuất trong ngày" hiển thị đúng các Batch được tạo/bắt đầu hôm nay.
  - [ ] *Kiểm thử lô chạy xuyên ngày:* Lô bắt đầu từ hôm qua nhưng hôm nay mới chạy nốt mẻ còn lại $\rightarrow$ Lô đó vẫn phải hiển thị bình thường trong bảng "Tổng số batch sản xuất trong ngày" hôm nay (lọc theo lô đang `Active` hoặc có mẻ con hoạt động trong ngày).
- [ ] **Banner Cảnh báo mẻ còn thiếu (Pending Run Banner):**
  - [ ] Khi có Lô đang chạy (`Active`) bắt đầu từ hôm trước nhưng vẫn còn mẻ con chưa chạy (`Pending`/`Waiting`/`Created`): Hệ thống phải hiển thị một Banner màu cam cảnh báo nổi bật ở đầu trang Overview dạng: *"Batch đang chạy (tên batch) ngày [ngày bắt đầu], mẻ còn thiếu chưa chạy (tên mẻ)"*.
  - [ ] Khi toàn bộ mẻ con của Lô đó chạy xong hoặc không có Lô xuyên ngày bị thiếu mẻ, Banner này phải tự động ẩn đi (không hiển thị đè hoặc chừa khoảng trắng trống trên UI).

### 1.4. Kiểm thử Mối liên hệ Dữ liệu & Tác động Giao diện (Data-Driven UI Impact Testing)
*Mục tiêu: Đảm bảo khi số liệu đo đạc (Telemetry) vượt ngưỡng an toàn hoặc thay đổi trạng thái, giao diện người dùng (UI) sẽ tự động phản hồi trực quan về màu sắc, thông báo lỗi, trạng thái thiết bị và cập nhật thời gian đồng bộ chính xác theo đúng thiết kế mã nguồn Frontend & Backend.*

- [ ] **Đồng bộ lệch chu kỳ khi xảy ra sự cố vượt ngưỡng (Ví dụ: Cảm biến vượt ngưỡng 40°C):**
  - [ ] **Mô tả hành vi dữ liệu:** Khi nhiệt độ bồn trên đo được vượt quá ngưỡng cảnh báo (Ví dụ: `NhietDoBonTronTren >= 40°C` được ghi nhận vào database ở bảng `realtime_alarms`):
    - [ ] **Pha 1 - Cảnh báo nhanh xuất hiện lập tức (Tối đa 2 giây):**
      - [ ] Trình duyệt chạy ngầm hàm AJAX `fetchRecentAlarms()` gọi lên API `/Overview/GetRecentAlarms` với tần suất **2 giây/lần** (`2000ms`).
      - [ ] Xác nhận: Dòng cảnh báo sự cố mới phải xuất hiện trong widget **Cảnh báo nhanh** ở góc dưới bên phải Header **trong vòng tối đa 2 giây** kể từ khi dữ liệu vượt ngưỡng được ghi nhận.
    - [ ] **Pha 2 - Cập nhật chi tiết bảng thống kê & highlight màu đỏ (Tối đa 30 giây):**
      - [ ] Trình duyệt chạy ngầm hàm AJAX `fetchCurrentBatchStats()` gọi lên API `/Overview/GetCurrentBatchStats` với tần suất **30 giây/lần** (`30000ms`).
      - [ ] Xác nhận: Sau **tối đa 30 giây**, bảng **Thống kê mẻ hiện tại** tự động tải lại và cập nhật:
        - [ ] Số liệu min hoặc max tương ứng trong cột **Nhiệt độ bồn trên** chuyển sang hiển thị **Màu đỏ in đậm** (`<span style='color: #ef4444; font-weight: bold;'>`).
        - [ ] Cột **Cảnh báo** sinh ra icon tam giác màu đỏ nhấp nháy, hiển thị tổng số cảnh báo của công đoạn đó, cho phép nhấp chuột để mở rộng hiển thị chi tiết (Thời gian, tiêu đề, giá trị đo được kèm ngưỡng so sánh).
- [ ] **Mối liên hệ giữa Trạng thái Công đoạn (Step Status) & Phong cách Giao diện (UI Styling):**
  - [ ] **Trạng thái Pending (Chưa thực hiện):**
    - [ ] Các thông số nhiệt độ bồn (Bồn trên, Bồn giữa, Bồn dưới) trong bảng "Thống kê mẻ hiện tại" của công đoạn chưa bắt đầu phải hiển thị **màu xanh lục lam sáng** (`class="td-temp text-cyan"`).
    - [ ] Nhãn trạng thái hiển thị viền xám mờ với chữ xám: `Chưa bắt đầu` (`class="status-badge pending"`).
  - [ ] **Trạng thái In-progress (Đang thực hiện):**
    - [ ] Dòng tương ứng của công đoạn trong bảng sẽ được highlight nền tối có ánh sáng mờ (`class="row-active"`).
    - [ ] Nhãn trạng thái hiển thị dạng viền sáng màu xanh cyan kèm chữ xanh cyan: `Đang thực hiện` (`class="status-badge in-progress"`).
  - [ ] **Trạng thái Completed (Hoàn thành):**
    - [ ] Dòng công đoạn trở lại màu chữ sáng thông thường. Nhãn trạng thái hiển thị nền xanh lá nhạt kèm chữ xanh lá nổi bật: `Hoàn thành` (`class="status-badge completed"`).
- [ ] **Cảnh báo thời gian thực hiện thực tế vượt quá tiêu chuẩn (Step Duration Overtime):**
  - [ ] Khi thời gian chạy thực tế của công đoạn (`step.duration`) lớn hơn thời gian tiêu chuẩn được thiết lập (`step.standard`) (Ví dụ: Cấp liệu thực tế chạy 840 giây trong khi tiêu chuẩn chỉ là 720 giây):
    - [ ] Kiểm tra FE render: Giá trị thời gian thực tế hiển thị trong cột **Thời gian** phải tự động chuyển sang **Màu vàng/cam cảnh báo** (`class="td-duration text-warning"`).
- [ ] **Tác động của Dữ liệu Trạng thái Mẻ đến Bảng Thống kê Mẻ Hôm Nay (Daily Batches):**
  - [ ] **Mẻ đang hoạt động (Active):**
    - [ ] Dòng của mẻ trong bảng được tô nền màu cyan nhạt (`background: rgba(0, 229, 255, 0.05)`), chữ cyan (`text-cyan`), nhãn trạng thái `Đang thực hiện` (viền cyan).
  - [ ] **Mẻ đã hoàn thành (Completed):**
    - [ ] Dòng hiển thị chữ trắng thường (`color: #fff`), thời gian hoạt động hiển thị rõ ràng (Ví dụ: `08:04 - 09:48`), nhãn trạng thái `Hoàn thành` (nền xanh lá nhạt).
- [ ] **Tương tác dữ liệu Sensor thời gian thực (ATScada Real-time Tags) & Biểu đồ:**
  - [ ] **Sơ đồ thiết bị (Tank Diagram parameters):**
    - [ ] Khi không sử dụng FAKE DATA (`USE_FAKE_DATA = false`), các trị số nhiệt độ bồn trên, giữa, dưới và áp suất lắng nghe sự kiện `valueChanged` từ Tag dispatcher của PLC thực tế (Các tag `AFChemTX01.*`).
    - [ ] Xác nhận: Trị số số liệu trên sơ đồ bồn cập nhật **tức thời và liên tục** khi giá trị tag PLC thay đổi.
  - [ ] **Đồng hồ kim Gauge & Biểu đồ lịch sử spline:**
    - [ ] Xác nhận: Định kỳ mỗi **5 giây** (`setInterval(updateRealCharts, 5000)`), các kim chỉ số trên 3 đồng hồ Gauge (Nhiệt độ môi trường, Nhiệt độ máy, Áp suất) và biểu đồ đường Spline lịch sử tự động vẽ lại theo đúng giá trị tag cảm biến mới nhất.
    - [ ] Các plotBands màu sắc (Đỏ, xanh dương, xanh cyan) tự động co dãn theo vị trí kim quay để giữ tính trực quan cao.
- [ ] **Dữ liệu mẻ sản xuất tác động đến Lũy kế sản lượng & Tiến độ Header:**
  - [ ] **Tiến độ mẻ (Step Stats Panel):**
    - [ ] Khi mẻ ở trạng thái `Active`, vòng tròn bước hiện tại trên timeline chuyển sang nhấp nháy xanh dương (`class="step active"`).
    - [ ] Thanh tiến độ lấp đầy và số phần trăm tiến độ cập nhật chính xác theo tỷ lệ công đoạn hiện tại (Ví dụ: Đang ở công đoạn 4/8 $\rightarrow$ Thanh tiến độ lấp đầy `50%`, chữ hiển thị `50%`).
    - [ ] Đồng hồ thời gian đã chạy (`statElapsedTime`) tự động tăng đều đặn mỗi **1 giây** qua timer của Client, tự động bù trừ sai lệch thời gian so với Server (`clientServerTimeOffset`).
  - [ ] **Sản lượng tích lũy thực tế (`#headerCurrentOutput`):**
    - [ ] Mỗi mẻ đã hoàn thành (`Completed`) cộng thêm **500 kg**.
    - [ ] Mẻ đang chạy (`Active`) cộng dồn lũy kế tạm tính theo công đoạn hiện tại: `(activeStepCode / 8) * 500 kg`.
    - [ ] Xác nhận: Khi một mẻ hoàn thành hoặc chuyển tiếp công đoạn, sản lượng tích lũy trên Header phải được cập nhật tương ứng lập tức.
- [ ] **Chu kỳ đồng bộ hóa dữ liệu ngầm giữa các trang (Header Polling Sync):**
  - [ ] Tại trang Overview: Việc cập nhật diễn ra liên tục theo thời gian thực (1s-2s) nhờ các tiến trình AJAX thời gian thực cao tốc.
  - [ ] Tại các trang khác (Báo cáo, Cài đặt...): Xác nhận tiến trình ngầm `fetchHeaderStats` gửi request lên API `/Overview/GetCurrentBatchStats` định kỳ mỗi **20 giây/lần** để đảm bảo các số liệu mẻ, công đoạn, trạng thái máy và sản lượng tích lũy trên Header vẫn được cập nhật đồng bộ chính xác mà không làm nặng tải hệ thống.

---

## 🚨 2. TRANG CẢNH BÁO (ALARM)
*Mục tiêu: Đảm bảo ghi nhận lịch sử lỗi chính xác, hỗ trợ lọc tìm kiếm và bảo mật tính năng xuất báo cáo.*

### 2.1. Kiểm thử Bộ lọc Lịch sử (Filter Bar)
- [ ] **Lọc theo khoảng thời gian:** Chọn Từ ngày (From Date) và Đến ngày (To Date).
  - [ ] Bấm **"Truy vấn"** hệ thống trả về đúng dữ liệu nằm trong khoảng thời gian đã chọn.
  - [ ] *Trường hợp biên (Edge Case):* Chọn Từ ngày lớn hơn Đến ngày $\rightarrow$ Hệ thống phải hiển thị thông báo lỗi/Toast cảnh báo người dùng và từ chối truy vấn.
- [ ] **Lọc theo phân loại:** Lọc theo Cấp độ (Alarm/Warning) hoặc Khu vực sự cố. Kết quả bảng tải lại tức thì và chính xác.

### 2.2. Kiểm thử Bảng dữ liệu (DataTables)
- [ ] **Tìm kiếm nhanh (Search Box):**
  - [ ] Nhập từ khóa bất kỳ (Ví dụ: "nhiệt độ", "áp suất") $\rightarrow$ Bảng lọc tức thì dữ liệu khớp với ký tự nhập vào.
  - [ ] Đảm bảo chữ tiêu đề nhãn *"Tìm kiếm:"* hiển thị rõ ràng, độ tương phản cao, ô nhập liệu nền tối sang trọng, không chói mắt.
- [ ] **Phân trang & Hiển thị số dòng:**
  - [ ] Lựa chọn số lượng dòng (5, 10, 25, 50) hoạt động đúng.
  - [ ] Nút phân trang (Trước, Sau, các số trang) hiển thị đúng định dạng Dark Theme, chuyển trang mượt mà không load lại trang vật lý.

### 2.3. Kiểm thử Bảo mật & Phân quyền (MANDATORY)
- [ ] **Đối với tài khoản Admin:**
  - [ ] Xuất hiện đầy đủ 2 nút **Export Excel** và **Export CSV** màu xanh lá nổi bật trên thanh bộ lọc.
  - [ ] Click nút xuất Excel/CSV $\rightarrow$ File tải xuống thành công, dữ liệu tiếng Việt hiển thị chuẩn không lỗi font chữ.
- [ ] **Đối với tài khoản Operator:**
  - [ ] Hai nút **Export Excel** và **Export CSV** phải **bị ẩn hoàn toàn** khỏi giao diện (không render HTML).
  - [ ] *Kiểm thử bảo mật nâng cao (API Block):* Nếu Operator cố tình gõ trực tiếp URL API xuất báo cáo (Ví dụ: `/Alarm/ExportAlarmsExcel`) trên trình duyệt hoặc gọi bằng Postman $\rightarrow$ Hệ thống phải chặn và trả về mã lỗi **`403 Forbidden`** kèm thông báo cảnh báo dạng Toast màu đỏ trên giao diện.

---

## 📦 3. TRANG BATCHES (SỰ KIỆN / EVENT)
*Mục tiêu: Đảm bảo theo dõi lịch sử vận hành các mẻ sản phẩm chính xác, bảo mật dữ liệu.*

### 3.1. Kiểm thử Tìm kiếm & Hiển thị
- [ ] **Bộ lọc thời gian:** Chọn ngày sản xuất mẻ và bấm truy vấn. Hệ thống hiển thị danh sách các mẻ hoàn thành hoặc đang chạy trong ngày đó.
- [ ] **Tìm kiếm theo tên sản phẩm / số mẻ:** Nhập số Batch hoặc mã sản phẩm $\rightarrow$ Bảng DataTables hiển thị dữ liệu lọc tương ứng.
- [ ] **Độ hiển thị:** Đảm bảo toàn bộ chữ mô tả bảng và các nhãn điều hướng rõ nét, dễ đọc trên giao diện tối.
- [ ] **Hiển thị thông tin mẻ lỗi (Error/Failed Run):**
  - [ ] Chọn một mẻ bị lỗi (`status = 'Error'` hoặc `'Failed'`) từ dropdown/bảng danh sách.
  - [ ] Xác nhận nhãn trạng thái chu kỳ hiển thị là **`"Chu kỳ bị lỗi"`** (không hiển thị nhãn mặc định *"Chu kỳ hoàn tất thành công"*).
  - [ ] Ghi chú chất lượng của mẻ lỗi hiển thị đúng nội dung: **`"Chu kỳ bị lỗi. Chất lượng sản phẩm: KHÔNG ĐẠT (LỖI)"`** và có chữ màu đỏ nổi bật (`class="text-danger"`).
- [ ] **Kiểm thử chu trình Dừng mẻ (Pause) và Chạy tiếp mẻ (Resume):**
  - [ ] *Trường hợp Dừng mẻ:* Khi mẻ đang hoạt động bị tạm dừng chạy (tín hiệu PLC dừng hoặc chuyển trạng thái):
    - [ ] Đồng hồ hiển thị thời gian chạy mẻ (`#headerRunningTime`) phải dừng tăng thời gian và giữ nguyên giá trị hiện tại.
    - [ ] Hệ thống giữ nguyên trạng thái hiển thị của công đoạn hiện hành, không nhảy bước hoặc cập nhật sai chu trình.
  - [ ] *Trường hợp Chạy tiếp mẻ:* Khi mẻ tiếp tục vận hành:
    - [ ] Thời gian chạy mẻ tiếp tục tăng tích lũy bình thường.
    - [ ] Các thông số PLC thực tế cập nhật liên tục trở lại.
    - [ ] Các sự kiện cảnh báo phát sinh trong quá trình dừng/chạy tiếp được ghi nhận đầy đủ với mốc thời gian chính xác.
- [ ] **Đồng bộ hóa dữ liệu (BatchResolver Integration):**
  - [ ] Khi chọn một Batch từ bộ lọc dropdown, mẻ con mặc định được tải lên phải khớp theo đúng độ ưu tiên phân giải của `BatchResolver`.
  - [ ] Dữ liệu kết xuất từ API xuất báo cáo Excel (`/Event/ExportEventExcel`) và CSV (`/Event/ExportEventCsv`) phải trùng khớp 100% với dữ liệu mẻ con đang hiển thị trên giao diện của trang Event.

### 3.2. Kiểm thử Bảo mật & Phân quyền Xuất dữ liệu
- [ ] **Tài khoản Admin:** Xem được đầy đủ nút Export và tải xuống file Excel/CSV báo cáo mẻ thành công.
- [ ] **Tài khoản Operator:**
  - [ ] Không nhìn thấy bất kỳ nút Export nào trên màn hình.
  - [ ] Nếu truy cập trực tiếp URL API xuất mẻ (Ví dụ: `/Event/ExportEventExcel`) $\rightarrow$ Backend lập tức chặn đứng, trả về mã **`403 Forbidden`** và hiển thị Toast báo lỗi.

---

## 📈 4. TRANG BÁO CÁO HOẠT ĐỘNG (REPORT)
*Mục tiêu: Đảm bảo bộ lọc Từ ngày, Đến ngày, Chọn batch hoạt động đồng bộ; bảng báo cáo 19 cột hiển thị dữ liệu chính xác và bảo mật tính năng xuất dữ liệu.*

### 4.1. Kiểm thử Bộ lọc & Ràng buộc Ngày tự động (Filters & Smart Date Constraints)
- [ ] **Ràng buộc ngày Từ ngày - Đến ngày tự động (Smart Auto-Sync):**
  - [ ] Khi chọn **Từ ngày** lớn hơn **Đến ngày** $\rightarrow$ Hệ thống tự động thay đổi Đến ngày bằng Từ ngày (không cho phép khoảng ngày nghịch đảo).
  - [ ] Khi chọn **Đến ngày** nhỏ hơn **Từ ngày** $\rightarrow$ Hệ thống tự động thay đổi Từ ngày bằng Đến ngày.
  - [ ] *Kiểm thử nhập bàn phím:* Nhập ngày thủ công bằng bàn phím $\rightarrow$ Khi mất tiêu điểm (Focusout), hệ thống tự động kiểm tra và đồng bộ lại nếu ngày không hợp lệ.
- [ ] **Đồng bộ Dropdown "Chọn batch" theo ngày (`/Home/GetBatches`):**
  - [ ] Khi thay đổi Từ ngày hoặc Đến ngày $\rightarrow$ Hệ thống tự động kích hoạt AJAX gọi lên `/Home/GetBatches` để tải lại danh sách batch tương ứng với khoảng ngày đó.
  - [ ] Đảm bảo các batch được thêm hậu tố rõ ràng: `(Active)` đối với mẻ đang chạy và `(Completed)` đối với mẻ đã hoàn thành. Mặc định luôn có tùy chọn `-- Tất cả --`.

### 4.2. Kiểm thử Bảng Báo cáo Hoạt động (reportTable)
- [ ] **Hiển thị đúng 19 cột thông số vận hành của SCADA:**
  - [ ] Kiểm tra tiêu đề và dữ liệu 19 cột: Ngày, Giờ, Quy trình, Công đoạn, T/g cấp liệu, T/g trộn 1, T/g xả đáy, T/g rung xả đáy, T/g hút xả đáy, T/g trộn 2, T/g xả hàng, T/g rung xả hàng, Tổng t/g trộn, Áp suất, Nhiệt độ MT, Độ ẩm MT, Nhiệt nắp bồn, Nhiệt giữa bồn, Nhiệt đáy bồn.
- [ ] **Kiểm tra chức năng Tìm kiếm & Phân trang:**
  - [ ] Nhập từ khóa tìm kiếm nhanh trên thanh `.dataTables_filter` hoạt động tốt.
  - [ ] Lựa chọn số lượng bản ghi hiển thị (10, 25, 50, 100) và số đếm tổng số bản ghi hiển thị chính xác (Ví dụ: `Hiển thị 1 đến 10 của 171 bản ghi`).

### 4.3. Kiểm thử Bảo mật & Phân quyền Xuất Báo cáo
- [ ] **Tài khoản Admin:**
  - [ ] Xuất hiện 2 nút **Export Excel** và **Export CSV** màu xanh lá nằm cạnh bộ lọc.
  - [ ] Click xuất file tải xuống thành công, tiếng Việt có dấu hiển thị chuẩn, không bị lỗi font.
- [ ] **Tài khoản Operator:**
  - [ ] Vẫn có toàn quyền lọc ngày, chọn batch và nhấn **Tìm kiếm** để theo dõi bảng dữ liệu báo cáo trực tiếp trên Web.
  - [ ] Hai nút **Export Excel** và **Export CSV** **bị ẩn hoàn toàn** khỏi giao diện (không được render HTML).
  - [ ] Gửi yêu cầu trực tiếp đến URL API xuất báo cáo (Ví dụ: `/Home/ExportReportExcel`) bị Backend chặn đứng với mã lỗi **`403 Forbidden`** và hiển thị cảnh báo đỏ trên giao diện.

---

## ⚙️ 5. TRANG CÀI ĐẶT HỆ THỐNG (USER SETTING)
*Mục tiêu: Quản trị tài khoản an toàn tuyệt đối, thực thi phân quyền nghiêm ngặt và ngăn chặn các hành động phá hoại hệ thống.*

### 5.1. Kiểm thử Phân quyền truy cập Trang (Page-level Authorization)
- [ ] **Tài khoản Admin:** Có quyền truy cập hợp lệ, nhìn thấy liên kết "Cài đặt hệ thống" trên Sidebar và tải trang thành công.
- [ ] **Tài khoản Operator (QUAN TRỌNG):**
  - [ ] Liên kết "Cài đặt hệ thống" ở thanh Sidebar trái **phải ẩn hoàn toàn**.
  - [ ] Nếu Operator cố tình gõ trực tiếp URL `/Home/UserSetting` lên thanh địa chỉ $\rightarrow$ Controller phải **lập tức chặn đứng và chuyển hướng (Redirect) người dùng trở lại trang Tổng quan (`/Home/Overview`)**.

### 5.2. Kiểm thử Tính năng Quản lý Tài khoản (Chỉ dành cho Admin)
- [ ] **Danh sách tài khoản (Account List):** Hiển thị danh sách tài khoản hiện tại dạng bảng DataTables (STT, UserName, Role, Hành động). Chữ hiển thị rõ nét, không bị lỗi font tiếng Việt.
- [ ] **Tạo tài khoản mới (Modal Create User):**
  - [ ] Click nút "Tạo tài khoản mới" $\rightarrow$ Popup Modal xuất hiện mượt mà.
  - [ ] *Kiểm thử bắt lỗi dữ liệu đầu vào:* Để trống UserName hoặc Password $\rightarrow$ Bấm Lưu $\rightarrow$ Hệ thống hiển thị Toast lỗi đỏ báo *"Tên đăng nhập / Mật khẩu không được để trống!"*.
  - [ ] *Kiểm thử trùng lặp tên (Uniqueness Check):* Nhập một UserName đã tồn tại trong bảng $\rightarrow$ Bấm Lưu $\rightarrow$ Hệ thống báo lỗi đỏ *"Tên đăng nhập đã tồn tại!"*.
  - [ ] *Tạo thành công:* Nhập tài khoản hợp lệ $\rightarrow$ Hệ thống báo thành công bằng Toast xanh lá, Modal tự động đóng, và bảng danh sách cập nhật dòng tài khoản mới ngay lập tức.
- [ ] **Reset Mật khẩu (Modal Reset Password):**
  - [ ] Click "Đổi mật khẩu" tại một dòng $\rightarrow$ Modal xuất hiện hiển thị sẵn UserName ở dạng Disabled (không cho sửa).
  - [ ] *Kiểm thử khớp mật khẩu:* Nhập Mật khẩu mới và Xác nhận mật khẩu mới khác nhau $\rightarrow$ Báo lỗi đỏ *"Mật khẩu xác nhận không khớp!"*.
  - [ ] *Reset thành công:* Nhập mật khẩu khớp $\rightarrow$ Lưu thành công, hệ thống cập nhật mật khẩu mới vào cơ sở dữ liệu.
- [ ] **Thay đổi Vai trò (Role Change Dropdown):**
  - [ ] Click vào dropdown chọn vai trò của một tài khoản $\rightarrow$ Hệ thống hiển thị hộp thoại xác nhận (Confirm Dialog): *"Bạn có chắc chắn muốn thay đổi quyền của tài khoản..."*.
  - [ ] Nếu chọn **Cancel** $\rightarrow$ Trạng thái Dropdown giữ nguyên vai trò cũ, không gọi API.
  - [ ] Nếu chọn **OK** $\rightarrow$ Hệ thống thực hiện cập nhật và báo thành công qua Toast xanh lá.
- [ ] **Quy tắc An toàn cốt lõi - Ngăn chặn xóa hoặc hạ cấp Admin cuối cùng (Edge Case):**
  - [ ] *Kịch bản kiểm thử:* Hệ thống chỉ còn 1 tài khoản Admin duy nhất. Admin này thực hiện tự đổi vai trò của mình thành `Operator` qua dropdown $\rightarrow$ Chọn **OK** trên hộp thoại $\rightarrow$ Hệ thống **phải từ chối cập nhật**, trả về mã lỗi và hiển thị cảnh báo Toast đỏ: **"Hệ thống phải có ít nhất một tài khoản Admin!"**. Vai trò của tài khoản này trên giao diện tự động khôi phục về `Admin`.

---

## 👑 6. KIỂM THỬ THÔNG SỐ TRÊN HEADER (HEADER METRICS)
*Mục tiêu: Đảm bảo thanh trạng thái Header hiển thị đầy đủ, chính xác và đồng bộ các chỉ số vận hành thời gian thực của máy trộn.*

### 6.1. Kiểm thử Đồng hồ & Phân ca tự động (Real-time Clock & Auto Shift)
- [ ] **Đồng hồ thời gian thực:** Đồng hồ (`#txtTime`) và Ngày (`#txtDate`) hiển thị chính xác theo giờ hệ thống và tự động cập nhật liên tục mỗi giây qua hàm `updateClock()`.
- [ ] **Phân ca tự động (`#headerCurrentShift`):** Lịch trình ca hiện tại tự động tính toán dựa trên thời gian thực tế thông qua hàm `getCurrentShift()`:
  - [ ] Khung giờ `08:00 - 11:59` $\rightarrow$ Hiển thị ca: `"08:00 - 12:00"`
  - [ ] Khung giờ `13:00 - 16:59` $\rightarrow$ Hiển thị ca: `"13:00 - 17:00"`
  - [ ] Khung giờ `18:00 - 21:59` $\rightarrow$ Hiển thị ca: `"18:00 - 22:00"`
  - [ ] Các khung giờ còn lại $\rightarrow$ Hiển thị trạng thái: `"Ngoài ca"`

### 6.2. Kiểm thử Cập nhật Chỉ số Vận hành (Batch & Process Step Stats)
- [ ] **Số mẻ sản xuất (`#headerBatchNumber`):** Cập nhật đúng mã tên mẻ (`batchInfo.batchName`) từ API `/Overview/GetCurrentBatchStats`.
- [ ] **Công đoạn hiện tại (`#step`):** Hiển thị chính xác tên công đoạn Razor đang chạy của mẻ hiện tại (`batchInfo.headerStepName`).
- [ ] **Trạng thái máy (`#headerMachineStatus`):** Đổi màu sắc trực quan tùy theo trạng thái máy:
  - [ ] Trạng thái `"RUNNING"` $\rightarrow$ Chuyển chữ sang màu **xanh lá** (`#22c55e`).
  - [ ] Các trạng thái khác $\rightarrow$ Chuyển chữ sang màu **xanh dương** (`#3b82f6`).
- [ ] **Thời gian chạy mẻ (`#headerRunningTime`):** Hiển thị đúng thời gian lũy kế mẻ hoạt động (`batchInfo.headerRunningTime`), mặc định là `"0s"` nếu không có dữ liệu.

### 6.3. Kiểm thử Tính toán Sản lượng Tích lũy (Production Yield Calculations)
- [ ] **Sản lượng mục tiêu (`#headerTargetOutput`):** Luôn hiển thị cố định là **`2000`** KG.
- [ ] **Sản lượng thực tế tích lũy (`#headerCurrentOutput`):** Được tính toán tự động dựa trên danh sách mẻ trong ngày (`dailyBatches`):
  - [ ] Mỗi mẻ đã hoàn thành (`Completed`) $\rightarrow$ Cộng thêm **`500`** KG.
  - [ ] Mẻ đang chạy (`Active`) $\rightarrow$ Cộng dồn lũy kế theo tỷ lệ công đoạn hiện tại (`activeStepCode` / 8 * 500 KG).
  - [ ] Xác nhận tổng sản lượng tích lũy hiển thị chính xác theo dữ liệu tính toán.

### 6.4. Kiểm thử Chức năng Tương tác & Chu kỳ Cập nhật (Interactions & Polling)
- [ ] **Dropdown "Chọn máy" (`#machineSelectContainer`):**
  - [ ] Click vào nút "Chọn máy" mở ra menu dropdown, click ra vùng ngoài tự động đóng menu.
  - [ ] Click chọn máy (Ví dụ: `TX01`) hiển thị đúng tên máy, cập nhật class `.selected` cho tùy chọn và ghi nhận log tải thông số.
- [ ] **Đèn chuông báo động (`#alarmCount`):** Hiển thị đúng tổng số lỗi đang xảy ra, click vào chuông chuyển hướng chính xác đến trang Cảnh báo.
- [ ] **Đồng bộ hóa ngầm (Polling Interval):**
  - [ ] Tại trang Tổng quan: Cập nhật real-time chu kỳ 1 giây qua bộ kết nối `OverviewRealtime.js`.
  - [ ] Tại các trang khác (Cài đặt, Báo cáo...): Header tự động kích hoạt tiến trình ngầm `fetchHeaderStats` gửi request lên API `/Overview/GetCurrentBatchStats` định kỳ mỗi **20 giây** một lần để cập nhật số liệu mới nhất.

---
*Tài liệu này là cẩm nang hướng dẫn kiểm thử chính thức cho hệ thống quản trị phân quyền AFCHEM SCADA. Mọi đợt cập nhật mã nguồn trong tương lai đều phải chạy qua bộ checklist này trước khi bàn giao.*
