# TÀI LIỆU BÀN GIAO KỸ THUẬT & HƯỚNG DẪN ĐÀO TẠO VẬN HÀNH
## HỆ THỐNG: AFCHEM SCADA & HINOTOOLS CORE LOGIC
*(Tài liệu chuẩn hóa phục vụ công tác bàn giao dự án, đào tạo kỹ thuật viên và nhân viên vận hành nhà máy)*

---

# PHẦN I: KẾ HOẠCH ĐÀO TẠO & BÀN GIAO HỆ THỐNG (TRAINING AGENDA)

Khi bàn giao dự án cho ban quản lý nhà máy và tổ vận hành, kỹ sư bàn giao thực hiện chương trình đào tạo theo 3 nội dung chính sau:

## 1. Đào tạo Lý thuyết & Tổng quan dòng chảy dữ liệu (30 phút)
* Hướng dẫn mô hình liên kết dữ liệu tự động giữa **Base** (Kế hoạch sản xuất) $\rightarrow$ **HinoTools Webhook** (Xử lý trung tâm) $\rightarrow$ **Database MySQL / PLC** $\rightarrow$ **WebApp SCADA** (Giám sát trực quan).
* Giải thích vai trò của các trang chức năng trên giao diện WebApp (Overview, Alarms, Batches, Report).

## 2. Thực hành Vận hành Tiêu chuẩn (45 phút)
* Thực hành quy trình đăng nhập, đối chiếu kiểm tra thông số Lô và bảng định mức nguyên vật liệu (BOM) đầu ca sản xuất.
* Thực hành theo dõi Timeline 8 bước trộn, giám sát nhiệt độ và áp suất thực tế.
* Thực hành xuất nhật ký mẻ sản xuất (Batch Production Record) ra file Excel và tra cứu bảng Báo cáo 19 cột cuối ca.

## 3. Thực hành các Kịch bản Vận hành & Kiểm thử Đặc biệt (45 phút) - Trọng tâm đào tạo
* **Kịch bản 1 (Tạm dừng & Chạy tiếp):** Giả lập dừng máy đột ngột khi đang chạy mẻ và tiếp tục vận hành.
* **Kịch bản 2 (Xử lý mẻ lỗi & Chạy bù):** Giả lập mẻ bị lỗi nghiêm trọng, hệ thống tự hủy mẻ lỗi, ghi nhận chất lượng không đạt, và tự động tạo mẻ mới trong DB để chạy bù sản lượng.
* **Kịch bản 3 (Bộ phân giải mẻ hiển thị mặc định):** Cách hệ thống tự động xác định mẻ nào được ưu tiên hiển thị trên màn hình Overview khi mở trang web.
* **Kịch bản 4 (Banner cảnh báo mẻ xuyên ngày còn thiếu):** Nhận diện cảnh báo khi lô sản xuất hôm trước chưa chạy hết các mẻ con kế hoạch.

---

# PHẦN II: CÁC KỊCH BẢN VẬN HÀNH VÀ KIỂM THỬ ĐẶC BIỆT (SPECIAL CASES)

Để bàn giao dự án thành công và giúp khách hàng làm chủ hệ thống, người vận hành và kỹ sư nghiệm thu cần hiểu rõ 4 kịch bản nghiệp vụ đặc biệt sau:

## Kịch bản 1: Mẻ trộn bị dừng đột ngột và chạy tiếp (Pause & Resume)
* **Mô tả nghiệp vụ:** Trong lúc máy đang thực hiện 1 trong 8 bước trộn (Ví dụ: đang ở bước *Trộn 1*), máy bị dừng đột ngột do nút nhấn cơ học hoặc sự cố thiết bị dẫn đến PLC báo trạng thái dừng.
* **Hành vi của WebApp SCADA:**
  * Trạng thái máy trộn trên thanh Header chuyển từ chữ màu xanh lá `"RUNNING"` sang chữ màu xanh dương `"STOPPED/PAUSE"`.
  * Bộ đếm thời gian chạy mẻ (`#headerRunningTime`) và thời gian chạy công đoạn hiện tại trên màn hình Tổng quan lập tức **đóng băng dữ liệu (ngừng tăng giây)** để bảo toàn đúng số giây máy thực tế vận hành.
* **Cách kiểm thử nghiệm thu:**
  1. Cho mẻ chạy đến bước bất kỳ, theo dõi thời gian đã chạy đang tăng đều mỗi giây.
  2. Bấm nút dừng máy trên tủ PLC. Xác nhận chữ trạng thái trên Header đổi sang xanh dương `"STOPPED/PAUSE"` và bộ đếm thời gian dừng lại.
  3. Bấm chạy tiếp máy trên tủ PLC. Xác nhận trạng thái đổi lại xanh lá `"RUNNING"` và bộ đếm thời gian tiếp tục chạy lũy kế từ mốc tạm dừng trước đó.

## Kịch bản 2: Mẻ lỗi nghiêm trọng & Cơ chế tự động tạo mẻ chạy bù (Failed Run & Auto Compensation)
* **Mô tả nghiệp vụ:** Khi một mẻ con đang chạy gặp sự cố nghiêm trọng không thể khắc phục (ví dụ: quá áp kéo dài, lỗi mất kết nối PLC quá thời gian cho phép), hệ thống ghi nhận trạng thái mẻ đó là **`Error` / `Failed` (Mẻ lỗi)**. Để đảm bảo tổng sản lượng của Lô trong ngày không bị thiếu hụt, hệ thống tích hợp cơ chế tự động chạy bù.
* **Hành vi của HinoTools & WebApp:**
  * **HinoTools** tự động tạo thêm một mẻ con mới (Run) trong Database MySQL với công thức BOM tương tự để người vận hành chạy bù cho mẻ lỗi trước đó.
  * WebApp SCADA đánh dấu mẻ cũ là **`Error`** và loại bỏ hoàn toàn mẻ lỗi này ra khỏi bảng BOM sản xuất trên giao diện. Tổng lượng nguyên liệu tiêu hao của ngày chỉ tính toán dựa trên các mẻ hợp lệ.
  * Khi xuất file Excel Nhật ký mẻ sản xuất (Batch Production Record) cho mẻ lỗi, cột trạng thái chất lượng sẽ tự động in đỏ đậm dòng chữ: **"Chu kỳ bị lỗi. Chất lượng sản phẩm: KHÔNG ĐẠT (LỖI)"**.
* **Cách kiểm thử nghiệm thu:**
  1. Giả lập tạo lỗi nghiêm trọng trong lúc mẻ con đang chạy (Ví dụ: đẩy cờ báo lỗi PLC lên mức 1).
  2. Xác nhận mẻ con đó chuyển sang trạng thái `Error` trên giao diện.
  3. Kiểm tra trong cơ sở dữ liệu và trên dropdown mẻ: Xác nhận hệ thống tự động sinh thêm 1 mẻ con mới tiếp nối (Ví dụ: Batch có 2 mẻ con là Me01, Me02. Me02 bị lỗi $\rightarrow$ Hệ thống tự sinh Me03 để chạy bù cho Me02).
  4. Xuất file Excel của mẻ Me02, kiểm tra dòng đánh giá chất lượng sản phẩm hiển thị màu đỏ báo *"KHÔNG ĐẠT (LỖI)"*.

## Kịch bản 3: Cơ chế ưu tiên chọn mẻ hiển thị mặc định (Batch Resolver Priority)
* **Mô tả nghiệp vụ:** Khi người vận hành mở WebApp hoặc tải lại trang Tổng quan (Overview) mà không lựa chọn cụ thể một mẻ nào từ bộ lọc, hệ thống cần tự động phân giải để hiển thị thông tin của mẻ quan trọng nhất đang diễn ra.
* **Hành vi của WebApp SCADA:**
  * Hệ thống tự động xác định mẻ hiển thị mặc định theo **4 cấp độ ưu tiên** sau:
    * **Ưu tiên 1 (Mẻ đang chạy):** Lọc tìm Batch đang hoạt động (`batches.status = 'Active'`) và mẻ chạy con đang thực thi (`runs.status = 'Active'`).
    * **Ưu tiên 2 (Mẻ hoàn thành gần nhất):** Nếu không có mẻ nào đang chạy, chọn Batch có mẻ con vừa hoàn thành gần nhất (`runs.status = 'Completed'` sắp xếp theo thời gian kết thúc giảm dần `end_time DESC, id DESC`).
    * **Ưu tiên 3 (Mẻ chờ chạy đầu tiên):** Nếu không có mẻ nào hoàn thành, chọn Batch đang ở trạng thái chờ (`batches.status = 'Pending'`) và hiển thị mẻ con đầu tiên (sắp xếp theo `id ASC`).
    * **Ưu tiên 4 (Fallback cuối cùng):** Nếu không khớp 3 trường hợp trên, chọn Batch mới nhất trong hệ thống và hiển thị mẻ con đầu tiên.
* **Cách kiểm thử nghiệm thu:**
  1. Giả lập trong DB có đồng thời 1 mẻ đang chạy (`Active`) và 2 mẻ đã xong (`Completed`). Tải lại trang WebApp $\rightarrow$ Xác nhận màn hình hiển thị thông số của mẻ đang chạy (`Active`).
  2. Dừng mẻ đang chạy, tải lại trang $\rightarrow$ Xác nhận màn hình hiển thị thông số của mẻ hoàn thành gần đây nhất.

## Kịch bản 4: Cảnh báo Lô chạy xuyên ngày bị thiếu mẻ (Pending Run Banner)
* **Mô tả nghiệp vụ:** Có Lô sản xuất được tạo từ ngày hôm trước nhưng đến ca hôm nay vẫn còn mẻ con ở trạng thái chờ (`Pending`/`Waiting`) chưa được kích hoạt chạy. Hệ thống cần nhắc nhở Operator xử lý nốt.
* **Hành vi của WebApp SCADA:**
  * Hiển thị một **Banner màu cam nổi bật** ở đầu trang Overview với nội dung: *"Batch đang chạy (tên batch) ngày [ngày bắt đầu], mẻ còn thiếu chưa chạy (tên mẻ)"*.
  * Banner này sẽ tự động ẩn đi khi toàn bộ mẻ con của Lô đó chạy xong hoặc không có Lô xuyên ngày bị thiếu mẻ.
* **Cách kiểm thử nghiệm thu:**
  1. Tạo trong Database một Batch có ngày từ hôm qua ở trạng thái `Active`, trong đó có 2 mẻ con: Me01 đã `Completed` hôm qua, Me02 vẫn `Pending`.
  2. Mở WebApp hôm nay $\rightarrow$ Xác nhận Banner màu cam xuất hiện ở trên cùng trang chủ báo mẻ Me02 còn thiếu chưa chạy.
  3. Bật máy chạy hoàn thành nốt mẻ Me02 $\rightarrow$ Xác nhận Banner tự động biến mất hoàn toàn khỏi giao diện.

---

# PHẦN III: QUY TRÌNH VẬN HÀNH TIÊU CHUẨN (STANDARD OPERATING PROCEDURE)

## 1. Quy trình Đầu Ca (Chuẩn bị và Đồng bộ)
1. Người vận hành đăng nhập vào WebApp SCADA bằng tài khoản vận hành được cấp.
2. Truy cập màn hình **Tổng quan (Overview)**, kiểm tra banner **TỔNG QUAN BATCH** ở đầu trang:
   * Xác nhận đúng `Sản phẩm`, `Mã hàng` và `Sản lượng kế hoạch` của ca chạy hôm nay (HinoTools tự động đồng bộ từ Base qua Webhook).
   * Kiểm tra bảng **BOM Sản xuất**: Đối chiếu danh sách các loại nguyên liệu (Mã nguyên liệu, số LOT, khối lượng định mức) hiển thị trên bảng trùng khớp với các bao nguyên liệu thực tế chuẩn bị nạp.

## 2. Quy trình Trong Ca (Giám sát chu kỳ trộn 8 bước)
Khi máy bắt đầu vận hành, người vận hành theo dõi sát các chỉ số sau trên màn hình Tổng quan:
* **Quy trình trộn 8 bước (Timeline Steps):** 
  * Xác nhận công đoạn máy đang chạy thực tế có màu xanh cyan sáng và nhấp nháy. Các công đoạn đã hoàn thành hiển thị tích xanh.
  * Bộ đếm thời gian chạy thực tế của công đoạn phải tăng đều theo giây và nằm trong giới hạn thời gian tiêu chuẩn cài đặt từ PLC (cột **TC cài đặt**).
* **Sơ đồ bồn trộn trực quan (Tank Diagram):**
  * Giám sát trạng thái hoạt động của cánh khuấy, van xả đáy, van xả hàng và bơm cấp liệu.
  * Theo dõi giá trị nhiệt độ tại 3 điểm (nắp bồn, giữa bồn, đáy bồn) và Áp suất đường ống (bar) dao động trong giới hạn an toàn.

## 3. Quy trình Cuối Ca (Nghiệm thu & Kết xuất báo cáo)
* **Xuất Báo cáo Lô sản xuất (Batch Production Record):** Truy cập trang **Nhật Ký Lô (Batches/Events)** $\rightarrow$ Chọn ngày sản xuất và chọn đúng mã Lô (Batch Name) trong dropdown $\rightarrow$ Bấm nút **Export Excel** để tải báo cáo. Tệp tin Excel kết xuất sẽ tự động tổng hợp đầy đủ định mức BOM từ Webhook Base và khối lượng thực tế SCADA chạy của ca.
* **Tra cứu Báo cáo Hoạt động 19 cột thông số:** Truy cập trang **Báo Cáo (Report)** $\rightarrow$ Chọn khoảng ngày truy vấn (Từ ngày - Đến ngày) $\rightarrow$ Bấm **Tìm kiếm** để xem bảng tổng hợp 19 cột thông số SCADA chi tiết phục vụ phân tích.

---

# PHẦN IV: HƯỚNG DẪN XỬ LÝ SỰ CỐ VẬN HÀNH (TROUBLESHOOTING GUIDE)

## Tình huống 1: Nhiệt độ bồn trộn vượt ngưỡng an toàn ($\ge 40^\circ C$)
* **Hiện tượng:** Chuông báo Header chớp đỏ (sau 2 giây), Widget cảnh báo nhanh hiện lỗi ở góc màn hình. Sau 30 giây, trị số nhiệt độ bị vượt ngưỡng chuyển sang **Chữ màu Đỏ in đậm**, tại cột Cảnh báo nhấp nháy tam giác đỏ.
* **Xử lý:** Operator nhấp chuột vào tam giác đỏ để xem chi tiết ngưỡng vượt. Bấm nút dừng khẩn cấp trên tủ PLC thực tế. Kiểm tra hệ thống nước làm mát. Khi nhiệt độ bồn hạ về ngưỡng an toàn, hệ thống tự động tắt cảnh báo.

## Tình huống 2: Mất kết nối tín hiệu giữa WebApp SCADA với PLC hoặc Cơ sở dữ liệu
* **Hiện tượng:** Toàn bộ thông số trên sơ đồ bồn trộn bị đóng băng (không cập nhật dữ liệu). Trạng thái máy trộn báo `"OFFLINE"`.
* **Xử lý:** Kiểm tra cáp mạng kết nối từ máy tính vận hành đến switch PLC. Kiểm tra dịch vụ HinoTools trên máy chủ có bị dừng hoạt động không, khởi động lại dịch vụ nếu cần.

## Tình huống 3: HinoTools không nhận được kế hoạch sản xuất/BOM từ Base
* **Hiện tượng:** Đầu ca, mục Sản phẩm/Mã hàng báo trống (-), bảng BOM không hiển thị danh sách nguyên vật liệu.
* **Xử lý:** Liên hệ bộ phận kế hoạch kiểm tra xem đã bấm "Gửi kế hoạch" trên Base chưa. Kiểm tra file log của HinoTools để xem có lỗi phân tích payload Webhook hoặc lỗi mạng đường truyền.

---

# PHẦN V: CÁC CÂU HỎI NGHIỆM THU DỮ LIỆU SẢN XUẤT (PRODUCTION DATA FAQ)

Đây là các câu hỏi trọng tâm đối chiếu chất lượng và độ chính xác của dữ liệu (Data Integrity) giữa WebApp, Database, HinoTools và biểu mẫu kết xuất:

### Câu hỏi 1: Làm thế nào để kiểm tra tính chính xác của dữ liệu định mức BOM kế hoạch (từ Base) đối chiếu với dữ liệu thực tế nạp liệu?
* **Trả lời:** 
  1. Trong lúc vận hành, đối chiếu bảng **BOM Sản xuất** hiển thị tại trang Tổng quan. Dữ liệu cột "Định lượng tiêu chuẩn" chính là dữ liệu thô nhận từ Base Webhook thông qua HinoTools.
  2. Cuối ca, khi tải tệp Excel Nhật ký mẻ sản xuất (Batch Production Record), hệ thống thực hiện truy vấn bảng `webhook_logs` và đối chiếu song song: Cột **Định lượng theo kế hoạch** (đọc từ Base webhook) và Cột **Giá trị thực tế** (đọc từ các thanh ghi xả của PLC lưu vào MySQL bảng `run_info`). Mọi sai lệch về khối lượng xả thực tế sẽ hiển thị trực quan để bộ phận QA/QC đánh giá chênh lệch.

### Câu hỏi 2: Sự khác biệt chính giữa Báo cáo Lô ở trang Batches và Báo cáo Hoạt động ở trang Report về mặt cấu trúc dữ liệu là gì?
* **Trả lời:** 
  * Dữ liệu ở trang **Batches** (Nhật ký mẻ) là sự kết hợp (Merge) giữa dữ liệu kế hoạch gốc lưu tại bảng `webhook_logs` (thông tin Lệnh sản xuất Work Order, tên hàng hóa) và dữ liệu thực tế chạy của mẻ. Tệp kết xuất là định dạng `.xlsx` chuẩn hóa theo phom mẫu mẫu có sẵn.
  * Dữ liệu ở trang **Report** (Báo cáo) là dữ liệu thô thời gian thực của SCADA ghi nhận từ PLC (truy vấn trực tiếp từ bảng `realtime_parameters`), định dạng bảng phẳng 19 cột phục vụ mục đích truy xuất biểu đồ và kiểm toán lịch sử thông số thiết bị.

### Câu hỏi 3: Dữ liệu của các mẻ bị lỗi (Error/Failed Run) được hệ thống xử lý như thế nào để không làm sai lệch báo cáo sản lượng?
* **Trả lời:** 
  * Mọi dữ liệu BOM nạp của mẻ bị lỗi (`status = 'Error'`) sẽ hoàn toàn bị loại bỏ khỏi tính toán tổng lượng nguyên liệu tiêu hao của Lô sản xuất trên giao diện.
  * Khối lượng thực tế của mẻ lỗi sẽ không được tính cộng dồn (500 KG) vào sản lượng thực tế tích lũy trên Header máy trộn.
  * Chỉ khi mẻ con chạy bù (được tự động sinh ra) hoàn thành thành công (`status = 'Completed'`), sản lượng tích lũy mới được cộng dồn, đảm bảo số liệu sản lượng cuối ca khớp chính xác với lượng hàng hóa thành phẩm thực tế sản xuất ra.

---
*Tài liệu bàn giao này đã được nghiệm thu kỹ thuật và là hướng dẫn chính thức cho việc vận hành hệ thống AFCHEM SCADA & HINOTOOLS.*
