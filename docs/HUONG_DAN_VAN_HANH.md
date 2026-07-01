# HƯỚNG DẪN VẬN HÀNH & BÀN GIAO CHI TIẾT THEO MÀN HÌNH
## HỆ THỐNG: AFCHEM SCADA & HINOTOOLS CORE LOGIC
*(Tài liệu chuẩn hóa dưới dạng danh sách chức năng theo từng màn hình phục vụ đào tạo và nghiệm thu)*

---

## 1. TRANG ĐĂNG NHẬP (LOGIN SCREEN)
* **Chức năng chính:**
  * Xác thực người dùng truy cập hệ thống bằng tài khoản cá nhân.
  * Tự động xóa thông tin phiên làm việc cũ (`Session.Clear()`) mỗi khi tải lại trang đăng nhập để bảo mật thông tin.
  * Tự động điều hướng tài khoản sau khi đăng nhập thành công vào màn hình Tổng quan (Overview).
* **Các lỗi thường gặp và cách xử lý:**
  * Sai tài khoản hoặc mật khẩu: Hệ thống hiển thị thông báo lỗi trực tiếp dưới ô nhập liệu.

---

## 2. THANH TRẠNG THÁI CHUNG (SYSTEM HEADER)
* **Chức năng chính:**
  * Giám sát trạng thái hoạt động thực tế của máy trộn TX01 trực quan qua màu sắc:
    * Chữ màu xanh lá `"Active"`: Mẻ sản xuất đang hoạt động/chạy bình thường.
    * Chữ màu vàng `"PAUSED"`: Máy đang tạm dừng trộn (có tín hiệu tạm dừng từ tủ PLC, WebApp tự động khóa bộ đếm thời gian).
    * Chữ màu xanh dương `"Pending"` / `"Standby"`: Mẻ sản xuất đang ở trạng thái chờ hoặc sẵn sàng kích hoạt.
    * Chữ màu xanh dương `"Completed"`: Mẻ/lô sản xuất hiện tại đã hoàn tất chu kỳ trộn.
  * Tự động đếm giờ thực tế hoạt động tích lũy (`headerRunningTime`) của mẻ đang chạy.
  * Đóng băng bộ đếm giờ (ngừng tăng giây) khi máy chuyển sang trạng thái tạm dừng `"PAUSED"` để tránh sai lệch dữ liệu.
  * Tự động hiển thị và phân ca làm việc của Operator dựa trên đồng hồ thời gian thực:
    * **Ca 1:** `08:00 - 12:00`
    * **Ca 2:** `13:00 - 17:00`
    * **Ngoài ca:** Các khung giờ nằm ngoài 2 ca trên.
  * Hiển thị mã mẻ con đang chạy thực tế (Ví dụ: `TX01-20260701-01`).
  * Hiển thị tên công đoạn hiện tại máy đang thực hiện (Cấp liệu, Trộn 1, Xả hàng...).
  * Hiển thị sản lượng thực tế tích lũy và sản lượng mục tiêu của ca:
    * **Sản lượng mục tiêu:** Được tải động từ thông số khối lượng kế hoạch của Lô sản xuất (`batches.target_weight` nhận từ Base), không sử dụng số liệu cố định.
    * **Sản lượng thực tế tích lũy:** Tính toán động từ tổng khối lượng nguyên vật liệu thực tế nạp của các mẻ con đã hoàn tất (cột `quantity` trong bảng `run_info`), tự động điều chỉnh bù trừ theo tỷ lệ hao hụt cho phép (allowable loss) và loại trừ hoàn toàn các mẻ con bị lỗi (`Error` / `Failed`).
  * Hiển thị đồng hồ thời gian thực tự động đồng bộ giờ hệ thống chính xác đến từng giây.
  * Tích hợp nút chuông báo động hiển thị tổng số lỗi chưa xử lý trong ca. Nhấp vào chuông để chuyển hướng nhanh đến trang Cảnh báo.
  * Menu dropdown "Chọn máy" cho phép cấu hình đổi trạm máy trộn (TX01).

---

## 3. MÀN HÌNH TỔNG QUAN (OVERVIEW SCREEN)
* **Sơ đồ bồn trộn trực quan (Mixing Tank Diagram):**
  * Giám sát thời gian thực trạng thái quay của các cánh khuấy bồn trộn.
  * Giám sát trạng thái đóng/mở của các van xả đáy, van xả hàng và trạng thái chạy của bơm cấp liệu.
  * Đo lường liên tục nhiệt độ bồn trộn tại 3 vị trí: Nhiệt độ nắp bồn (trên), Nhiệt độ giữa bồn, Nhiệt độ đáy bồn (dưới).
  * Đo lường áp suất ống dẫn thực tế (đơn vị `bar`).
  * Đo lường nhiệt độ phòng và độ ẩm không khí phòng sản xuất thực tế.
  * Đồng hồ kim (Solid Gauges) hiển thị trực quan tốc độ khuấy, nhiệt độ và áp suất.
  * Biểu đồ đường (Spline Line Chart) cập nhật tự động mỗi 5 giây vẽ lịch sử biến thiên của nhiệt độ và áp suất bồn.
* **Quy trình trộn 8 bước (Timeline Steps):**
  * Hiển thị chu trình 8 bước chuẩn: Cấp liệu, Trộn 1, Xả đáy, Rung xả đáy, Hút xả đáy thêm, Trộn 2, Xả hàng, Rung xả hàng.
  * Tự động highlight màu xanh cyan nhấp nháy tại công đoạn đang chạy thực tế.
  * Hiển thị tích xanh hoàn thành tại các công đoạn đã chạy xong.
  * Hiển thị màu xám mờ tại các công đoạn chờ chưa thực hiện.
  * Đối chiếu thời gian chạy thực tế của công đoạn với thời gian tiêu chuẩn cài đặt từ PLC (cột **TC cài đặt**).
* **Bảng định mức vật tư (BOM Table):**
  * Hiển thị định mức nguyên liệu nạp chi tiết cho mẻ hiện tại (Mã hàng, tên nguyên liệu, tổng xuất định mức, LOT số, đơn vị tính).
  * Đồng bộ tự động dữ liệu BOM kế hoạch nhận từ hệ thống Base của đối tác qua HinoTools Webhook.
  * Loại bỏ hoàn toàn các mẻ con bị lỗi (`status = 'Error'`) ra khỏi hiển thị và không tính toán tiêu hao nguyên liệu của mẻ lỗi.
* **Bảng thống kê mẻ trong ngày (Daily Batches Table):**
  * Liệt kê các Lô mẻ được khởi chạy hoặc tạo mới trong ngày hôm nay.
  * Tự động hiển thị các Lô sản xuất cũ của ngày hôm trước nếu vẫn chưa chạy hết (ở trạng thái đang hoạt động `Active` hoặc có bất kỳ mẻ con nào được vận hành chạy xuyên ngày sang hôm nay).
  * Tải dữ liệu mặc định theo bộ phân giải mẻ thông minh (Batch Resolver): ưu tiên hiển thị mẻ đang chạy -> mẻ hoàn thành gần nhất -> mẻ chờ chạy.
* **Banner cảnh báo mẻ con còn thiếu (Pending Run Banner):**
  * Hiển thị banner màu cam nổi bật ở đầu trang khi phát hiện có Lô chạy xuyên ngày chưa hoàn thành hết số mẻ con kế hoạch.
  * Banner tự động ẩn đi sau khi người vận hành hoàn thành nốt mẻ con còn thiếu đó.
* **Quy trình giám sát và xử lý cảnh báo quá nhiệt tại chỗ:**
  * Khi nhiệt độ bồn trên vượt quá ngưỡng an toàn ($\ge 40^\circ C$), hệ thống ghi nhận lỗi vào cơ sở dữ liệu.
  * **Trong vòng 2 giây:** Đèn chuông báo Header chớp đỏ và widget "Cảnh báo nhanh" góc dưới màn hình xuất hiện thông số lỗi.
  * **Trong vòng 30 giây:** Bảng thống kê mẻ tự làm mới, trị số nhiệt độ lỗi chuyển sang **Màu đỏ in đậm**, đồng thời cột Cảnh báo hiển thị biểu tượng tam giác đỏ nhấp nháy.
  * Operator nhấp chuột vào biểu tượng tam giác đỏ để mở popup xem chi tiết mốc thời gian, giá trị đo được và ngưỡng an toàn cài đặt.

---

## 4. MÀN HÌNH CẢNH BÁO SỰ CỐ (ALARM SCREEN)
* **Bộ lọc và truy vấn dữ liệu:**
  * Lọc tìm kiếm lịch sử sự cố theo khoảng thời gian tùy chọn (Từ ngày - Đến ngày).
  * Ràng buộc ngày thông minh: tự động điều chỉnh Đến ngày bằng Từ ngày nếu chọn ngày nghịch đảo để tránh lỗi truy vấn.
  * Lọc nhanh danh sách lỗi theo Phân loại cấp độ sự cố (ALARM hoặc WARNING).
* **Bảng chi tiết sự cố (Datatables):**
  * Hệ thống hỗ trợ xử lý và lưu trữ 5 cấp độ cảnh báo lỗi trong cơ sở dữ liệu và C# Backend: `ALARM`, `WARNING`, `HIGH`, `AVERAGE`, và `LOW`.
  * Trên giao diện trang Cảnh báo, các mức độ này được gom nhóm trực quan thành 2 nhóm chính:
    * **ALARM / HIGH (Màu đỏ):** Nhóm lỗi nghiêm trọng đe dọa an toàn máy (ví dụ: bồn trộn quá nhiệt, áp suất đường ống vượt ngưỡng).
    * **WARNING / AVERAGE / LOW (Màu vàng):** Nhóm lỗi cảnh báo vận hành nhẹ hoặc nhắc nhở (ví dụ: thời gian chạy bước trộn thực tế vượt quá tiêu chuẩn cài đặt từ PLC).
  * Ô tìm kiếm nhanh cho phép gõ từ khóa (tên cảm biến, mã lỗi) để lọc dòng dữ liệu tức thì.
  * Phân trang tự động giúp người vận hành tra cứu lỗi cũ dễ dàng mà không làm chậm trang web.

---

## 5. MÀN HÌNH NHẬT KÝ LÔ SẢN XUẤT (BATCHES SCREEN)
* **Giám sát lịch sử mẻ chạy:**
  * Chọn ngày và chọn Lô sản xuất (Batch Name) trong dropdown để tải danh sách các mẻ con.
  * Đối với các mẻ bị lỗi (`status = 'Error'`), hệ thống hiển thị nhãn chất lượng màu đỏ nổi bật: **"Chu kỳ bị lỗi. Chất lượng sản phẩm: KHÔNG ĐẠT (LỖI)"**.
  * Hiển thị thời gian chạy thực tế của 8 bước trộn, bảng BOM nạp thực tế của mẻ con được lựa chọn.
* **Chức năng xuất báo cáo Excel Nhật ký mẻ sản xuất (Batch Production Record):**
  * Tải xuống tệp báo cáo chuẩn hóa theo mẫu nghiệm thu QA/QC của nhà máy (`structure_batch_export.xlsx`).
  * Định dạng tệp xuất tự động giữ nguyên các ô merge cell, kẻ viền border và màu sắc của file mẫu gốc.
  * **Tích hợp dữ liệu đa nguồn:** Tự động tìm kiếm bản ghi payload thô trong bảng `webhook_logs` nhận từ Base qua HinoTools để điền các mục kế hoạch (Mã sản phẩm, tên sản phẩm, lệnh sản xuất Work Order), kết hợp với số liệu chạy thực tế của SCADA (khối lượng xả thực nạp bồn của từng mã nguyên liệu, thời gian thực chạy của 8 công đoạn).
  * So sánh đối chiếu trực tiếp trên file Excel: hiển thị song song cột **Định lượng kế hoạch** và **Khối lượng thực xả** của từng nguyên vật liệu đầu vào.
  * Tự động đánh giá chất lượng mẻ (ĐẠT/KHÔNG ĐẠT) dựa trên việc có phát sinh lỗi cảnh báo trong chu kỳ.

---

## 6. MÀN HÌNH BÁO CÁO HOẠT ĐỘNG (REPORT SCREEN)
* **Truy vấn thông số phẳng:**
  * Lọc dữ liệu thông số SCADA của toàn bộ quá trình trộn theo khoảng ngày tùy chọn (Từ ngày - Đến ngày).
  * Dropdown chọn Batch tự động tải lại các mẻ chạy tương ứng nằm trong khoảng ngày lọc.
  * Xuất bảng dữ liệu phẳng bao gồm **19 cột thông số vận hành của SCADA**:
    1. **Ngày:** Ngày diễn ra ca sản xuất thực tế.
    2. **Giờ:** Mốc thời gian ghi nhận bản ghi thông số của hệ thống.
    3. **Quy trình:** Tên Lô sản xuất (Batch Name).
    4. **Công đoạn:** Bước quy trình đang chạy tại thời điểm đó.
    5. **T/g cấp liệu (s):** Thời gian thực hiện thực tế của công đoạn Cấp liệu.
    6. **T/g trộn 1 (s):** Thời gian thực hiện thực tế của công đoạn Trộn 1.
    7. **T/g xả đáy (s):** Thời gian thực hiện thực tế của công đoạn Xả đáy.
    8. **T/g rung xả đáy (s):** Thời gian thực hiện thực tế của công đoạn Rung xả đáy.
    9. **T/g hút xả đáy thêm (s):** Thời gian thực hiện thực tế của công đoạn Hút xả đáy thêm.
    10. **T/g trộn 2 (s):** Thời gian thực hiện thực tế của công đoạn Trộn 2.
    11. **T/g xả hàng (s):** Thời gian thực hiện thực tế của công đoạn Xả hàng.
    12. **T/g rung xả hàng (s):** Thời gian thực hiện thực tế của công đoạn Rung xả hàng.
    13. **Tổng t/g trộn (s):** Tổng thời gian hoạt động của máy trộn trong mẻ đó.
    14. **Áp suất (bar):** Trị số áp suất đường ống thực tế.
    15. **Nhiệt độ MT (°C):** Nhiệt độ môi trường phòng sản xuất.
    16. **Độ ẩm MT (%):** Độ ẩm môi trường phòng sản xuất.
    17. **Nhiệt nắp bồn (°C):** Nhiệt độ bồn trộn ở điểm Trên (Hiển thị nhiệt độ thực tế).
    18. **Nhiệt giữa bồn (°C):** Nhiệt độ bồn trộn ở điểm Giữa (Hiển thị nhiệt độ thực tế).
    19. **Nhiệt đáy bồn (°C):** Nhiệt độ bồn trộn ở điểm Dưới (Hiển thị nhiệt độ thực tế).

---

## 7. CÁC TÌNH HUỐNG SỰ CỐ & KỊCH BẢN KIỂM THỬ ĐẶC BIỆT (UAT TESTING)
* **Kịch bản 1: Mẻ trộn bị dừng đột ngột và chạy tiếp (Pause & Resume)**
  * *Cách test:* Cho mẻ chạy -> bấm Pause trên tủ PLC -> xác nhận WebApp đổi trạng thái máy sang PAUSED và bộ đếm thời gian dừng lại. Bấm Resume -> xác nhận máy chạy tiếp và đếm lũy kế tiếp.
* **Kịch bản 2: Mẻ lỗi nghiêm trọng & Tự động chạy bù (Compensation)**
  * *Cách test:* Đẩy lỗi nghiêm trọng lên PLC -> xác nhận mẻ con đang chạy chuyển sang Error trên giao diện. Kiểm tra trên dropdown và DB xuất hiện mẻ con mới tiếp nối để chạy bù. Xuất file Excel mẻ lỗi, xác nhận chất lượng in đỏ "KHÔNG ĐẠT (LỖI)".
* **Kịch bản 3: Lô chạy xuyên ngày**
  * *Cách test:* Giả lập Lô chạy dở từ hôm qua và chạy tiếp hôm nay -> xác nhận mẻ chạy dở vẫn hiển thị trong danh sách mẻ hôm nay. Xuất Excel mẻ xuyên ngày -> kiểm tra file Excel tổng hợp chính xác dữ liệu của cả hai ngày.
* **Kịch bản 4: Kiểm thử bộ lọc ngày thông minh tại trang Report**
  * *Cách test:* Chọn Từ ngày sau Đến ngày -> xác nhận Đến ngày tự nhảy bằng Từ ngày. Nhập sai định dạng ngày bằng bàn phím -> xác nhận hệ thống tự động reset về ngày hôm nay khi click ra ngoài.

---
*Tài liệu bàn giao này đã được nghiệm thu kỹ thuật và là hướng dẫn chính thức cho việc vận hành hệ thống AFCHEM SCADA & HINOTOOLS.*
