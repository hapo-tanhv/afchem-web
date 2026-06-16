# Requirements: Nhật ký sản xuất (Batch Production Record) Export

## Introduction
Tính năng này xây dựng công cụ xuất báo cáo Excel "Nhật ký sản xuất / Batch Production Record" chuẩn hóa cho AFCHEM SCADA từ trang Batches (Event Page). File Excel xuất ra phải kế thừa chính xác định dạng, merge cell, border, font, và độ rộng từ file Excel mẫu [structure_batch_export.xlsx](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/docs/structure_batch_export.xlsx) và điền động dữ liệu tích hợp từ các bảng cơ sở dữ liệu (`batches`, `runs`, `run_info`, `webhook_logs`).

---

## Requirements

### Requirement 1: Sử dụng Template Excel làm gốc
**Objective:** Là một Người vận hành / Quản lý, tôi muốn file Excel xuất ra giữ nguyên định dạng của file mẫu để đảm bảo tính chuyên nghiệp và thẩm mỹ theo tiêu chuẩn nhà máy.
- **1.1 [Ubiquitous]** Hệ thống phải nạp file mẫu [structure_batch_export.xlsx](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/docs/structure_batch_export.xlsx) từ thư mục `docs/` làm template để điền dữ liệu.
- **1.2 [Ubiquitous]** Hệ thống phải giữ nguyên định dạng (Merge Cells, Borders, Background Colors, Row Heights, Column Widths, Fonts Carlito/Calibri) của template.
- **1.3 [Ubiquitous]** Tên file xuất ra phải tuân theo định dạng: `batch_record_{batch_id}_{yyyyMMdd}.xlsx` (ví dụ: `batch_record_1_20260609.xlsx`).

### Requirement 2: Điền thông tin chung lô sản xuất (Section 1)
**Objective:** Là một Người giám sát, tôi muốn xem đầy đủ thông tin chung của lô sản xuất để kiểm tra nguồn gốc lô hàng.
- **2.1 [Ubiquitous]** Hệ thống phải truy vấn bảng `webhook_logs` theo thời gian `received_at` trùng khớp với `batches.created_at` của lô được chọn để trích xuất payload JSON.
- **2.2 [Ubiquitous]** Hệ thống phải điền các thông tin từ payload giải mã được vào các ô tương ứng của Section 1:
  - **B5:C5** (Merge): Tên sản phẩm (`custom_ten_hang_hoa`)
  - **E5:F5** (Merge): Mã hàng FG (`custom_ma_dinh_danh`)
  - **H5:I5** (Merge): Mã lô FG (`custom_lotno`)
  - **B6:C6** (Merge): Lệnh sản xuất / Kế hoạch (`custom_ke_hoach_san_xuat`)
  - **E6:F6** (Merge): Ngày sản xuất (`custom_ngay_san_xuat`)
  - **H6:I6** (Merge): Ca sản xuất (Truy vấn ca làm việc từ webhook hoặc để trống cho ghi nhận thủ công)
  - **B7:C7** (Merge): Thiết bị sử dụng / Máy (`custom_thiet_bi_su_dung`)
  - **E7:F7** (Merge): Quy cách đóng gói (`custom_quy_cach`)
  - **H7:I7** (Merge): Đơn vị tính (`custom_don_vi_tinh`)
  - **B8:C8** (Merge): Sản lượng kế hoạch (`custom_khoi_luong_muc_tieu`)
  - **E8:F8** (Merge): Sản lượng thực tế (Tính tổng khối lượng của các runs đã hoàn thành)
  - **H8:I8** (Merge): Trạng thái lô (`batches.status`)
  - **B9:C9** (Merge): Giờ bắt đầu thực tế (`batches.start_time` định dạng `HH:mm:ss`)
  - **E9:F9** (Merge): Giờ kết thúc thực tế (`batches.end_time` định dạng `HH:mm:ss`)
  - **H9:I9** (Merge): Mã mẫu lưu (Lấy từ `custom_lotno` làm mã mẫu lưu mặc định)

### Requirement 3: Liệt kê nguyên vật liệu đầu vào (Section 2 - BOM)
**Objective:** Là một Nhân viên QC, tôi muốn xem chi tiết định mức và lô nguyên vật liệu thực xuất cho lô hàng này.
- **3.1 [Ubiquitous]** Hệ thống phải lấy dữ liệu nguyên vật liệu từ payload webhook (giải mã trường Base64 `custom_thong_tin_bom_san_xuat_a`, `custom_thong_tin_bom_san_xuat_b`, v.v.) hoặc từ bảng `run_info` liên kết qua runs.
- **3.2 [Ubiquitous]** Hệ thống phải điền các cột: STT, Mã hàng, Tên hàng, ĐVT, Số lượng kế hoạch, Số lượng thực xuất, Mã lô nguyên vật liệu đầu vào, Ghi chú.
- **3.3 [Ubiquitous]** Nếu số lượng nguyên vật liệu lớn hơn 5 dòng (vượt quá số dòng có sẵn dòng 13-17 trong template), hệ thống phải tự động chèn thêm dòng mới (sử dụng lệnh `InsertRow` của EPPlus), sao chép chính xác style của dòng trước đó và dịch chuyển các Section phía dưới xuống một cách chính xác.

### Requirement 4: Ghi nhận thông số quá trình của các mẻ con (Section 3 - Telemetry)
**Objective:** Là một QC, tôi muốn xem chi tiết thông số vận hành (thời gian, nhiệt độ, áp suất) của từng mẻ con (runs) để đánh giá chất lượng công đoạn.
- **4.1 [Ubiquitous]** Hệ thống phải tự động xác định số lượng mẻ con thực tế (`runs`) của lô từ bảng `runs`.
- **4.2 [Ubiquitous]** Với mỗi mẻ con (mẻ 1, mẻ 2...), hệ thống phải hiển thị một bảng thông số riêng biệt gồm 8 công đoạn tiêu chuẩn (Cấp liệu, Trộn 1, Xả đáy, Rung xả đáy, Hút xả đáy, Trộn 2, Xả hàng, Rung xả hàng).
- **4.3 [Ubiquitous]** Nếu số mẻ con lớn hơn 2 (template chỉ có sẵn mẻ 1 ở dòng 21-28 và mẻ 2 ở dòng 31-38), hệ thống phải nhân bản khối bảng biểu của mẻ 2 (bao gồm cả dòng header mẻ) để tạo bảng cho mẻ 3, mẻ 4... và chèn vào Excel.
- **4.4 [Ubiquitous]** Dữ liệu của từng công đoạn phải được điền động bao gồm:
  - Thời gian bắt đầu, Kết thúc, Chạy thực tế: Lấy từ bảng `alarmlog` dựa trên TagNo (`T001` - `T008`) tương ứng của mẻ con đó.
  - Thông số cài đặt: Lấy thời gian tiêu chuẩn tương ứng (ví dụ: `720s`, `1200s`).
  - Nhiệt độ nắp bồn, bồn giữa, bồn dưới: Tính toán dải Min-Max (hoặc giá trị trung bình) từ bảng `alarmreport` (cột `NhietDoBonTronTren`, `NhietDoBonTronGiua`, `NhietDoBonTronDuoi`) thu thập trong khoảng thời gian chạy của công đoạn đó.
  - Áp suất và cảnh báo: Trích xuất thông tin áp suất và ghi nhận cảnh báo nếu có sự cố.

### Requirement 5: QC lô thành phẩm và Xử lý sự cố (Section 4, 5, 6, 7)
**Objective:** Là một Quản lý sản xuất, tôi muốn xem kết quả kiểm định chất lượng và các sự cố phát sinh của lô hàng.
- **5.1 [Ubiquitous]** Hệ thống phải điền các chỉ tiêu QC ở Section 5 (Cảm quan, Khối lượng, Bao bì, Mã in...) dưới dạng ô trống hoặc điền mặc định "Đạt" để người kiểm định ký duyệt trực tiếp.
- **5.2 [Ubiquitous]** Hệ thống phải tự động quét bảng `realtime_alarms` của tất cả các mẻ con thuộc lô, lấy các cảnh báo nghiêm trọng (Severity = 'ALARM') để điền vào Section 6 (Sự cố phát sinh và xử lý) bao gồm: Thời điểm xảy ra, Mô tả cảnh báo, Trạng thái xử lý.
- **5.3 [Ubiquitous]** Section 7 (Xác nhận) phải giữ nguyên cấu trúc ký tên của các vai trò: Người vận hành, Tổ trưởng, QC, Quản lý sản xuất.

### Requirement 6: Tích hợp nút xuất báo cáo trên trang giao diện (UI/UX)
**Objective:** Là một Admin, tôi muốn thao tác xuất báo cáo Nhật ký sản xuất trực tiếp từ giao diện trang Batches (Event) một cách tiện lợi.
- **6.1 [State Driven]** Nút "Xuất Nhật ký sản xuất" (Export BPR) chỉ hiển thị khi người dùng đã đăng nhập với vai trò Admin (`Session["Role"] == Admin`).
- **6.2 [Event Driven]** Khi Admin nhấn nút "Export BPR", giao diện sẽ gửi request gọi Controller Action tải file Excel về máy.
