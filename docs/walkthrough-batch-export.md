# Tài liệu tổng kết: Chức năng xuất Excel "Batch Production Record" (BPR)

Tài liệu này tổng hợp toàn bộ các thay đổi kiến trúc, kết quả lập trình và quy trình xác minh chất lượng cho chức năng xuất file Excel báo cáo Nhật ký sản xuất ("Batch Production Record" - BPR) từ trang Batches/Event.

---

## 1. Yêu cầu nghiệp vụ & Kỹ thuật

### 1.1. Mục tiêu
- Xuất file Excel (.xlsx) báo cáo Nhật ký sản xuất cho một lô cụ thể (`batchId`), giữ nguyên 100% định dạng (font chữ, cỡ chữ, căn lề, border, merge cell, màu nền, độ rộng cột, chiều cao dòng) của file mẫu `docs/structure_batch_export.xlsx`.
- Tên file tải về tuân theo định dạng: `batch_record_{batch_id}_{yyyyMMdd}.xlsx`.

### 1.2. Nguồn dữ liệu
- **Bảng `batches`**: Lấy thông tin chung của lô sản xuất (Tên sản phẩm, Mã sản phẩm, Mã lô, Lệnh sản xuất, Thiết bị chạy, Trạng thái, Giờ bắt đầu/kết thúc).
- **Bảng `runs`**: Lọc danh sách các mẻ con thuộc lô sản xuất.
- **Bảng `run_info`**: Lọc thông tin nguyên vật liệu sử dụng thực tế (BOM) của từng mẻ con (Mã hàng, ĐVT, số lượng kế hoạch, số lượng thực tế, số lô).
- **Bảng `webhook_logs`**: Lấy dữ liệu Administrative và BOM kế hoạch gửi từ webhook (lấy theo trường `received_at` khớp với `batches.created_at`). Nếu không có webhook, hệ thống sẽ tự động fallback sang cơ sở dữ liệu `run_info` và hiển thị thông báo.
- **Bảng `alarmlog` & `alarmreport`**: Lọc thời gian bắt đầu/kết thúc của 8 công đoạn sản xuất (Cấp liệu, Trộn 1, Xả đáy, Rung xả đáy, Hút xả đáy, Trộn 2, Xả hàng, Rung xả hàng) dựa trên Tag số hiệu từ `T001` đến `T008` và tính toán nhiệt độ bồn trộn (Lớn nhất, Nhỏ nhất, Trung bình).
- **Bảng `realtime_alarms`**: Lấy cảnh báo nghiêm trọng trong mẻ chạy điền vào mục Sự cố phát sinh.

### 1.3. Cơ chế xử lý bố cục động
- **Mục 2 (BOM)**: Chèn dòng động nếu số lượng vật tư vượt quá 5 dòng của biểu mẫu. Toàn bộ định dạng của dòng mẫu được sao chép chính xác sang các dòng chèn mới.
- **Mục 3 (Runs)**:
  - Nếu lô chỉ có 1 mẻ con: Tự động xóa block mẻ con thứ 2 trong template và kéo toàn bộ các phần bên dưới lên.
  - Nếu lô có 2 mẻ con: Giữ nguyên định dạng template.
  - Nếu lô có nhiều hơn 2 mẻ con: Nhân bản chính xác khối mẻ con 2 cho các mẻ con 3, 4, 5... bao gồm sao chép chiều cao dòng, giá trị mặc định, font chữ và màu sắc.
- **Các phần dưới (QC, Sự cố, Ký tên)**: Dịch chuyển tọa độ dòng động (`qcSectionStart`) tương ứng với số lượng dòng BOM được chèn thêm và số lượng mẻ con chạy trong lô.

---

## 2. Các tệp tin được chỉnh sửa và tạo mới

### 2.1. Frontend & View
- **[LongDucProjectTest/Views/Home/Event.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/Event.cshtml)**:
  - Tích hợp nút bấm **Export BPR** (chỉ hiển thị với Admin).
  - Viết Javascript thu thập mã lô `batchId` hiện tại và điều hướng tải file qua endpoint: `/Event/ExportBatchRecordExcel?batchId=xxx`.

### 2.2. Controller
- **[LongDucProjectTest/Controllers/EventController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/EventController.cs)**:
  - Thêm Action Method `ExportBatchRecordExcel(string batchId)` phân quyền Admin.
  - Thực hiện kiểm tra quyền truy cập hợp lệ, khởi tạo `BatchRecordExportService` và trả về tệp tải về định dạng `FileResult`.

### 2.3. Service (EPPlus Service Layer)
- **[LongDucProjectTest/Service/IBatchRecordExportService.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Service/IBatchRecordExportService.cs)**: Định nghĩa interface nghiệp vụ.
- **[LongDucProjectTest/Service/BatchRecordExportService.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Service/BatchRecordExportService.cs)**:
  - Hiện thực logic đọc file template chỉ đọc.
  - Xử lý giải mã Base64 BOM nguyên vật liệu và phân tích URL-encoded webhook logs.
  - Viết thuật toán chèn dòng và copy style thủ công (`CopyStyle`, `CopyBorderItem`, `CopyExcelColor`) do EPPlus không hỗ trợ gán trực tiếp thuộc tính `Style.ID` chỉ đọc.
  - Khắc phục các lỗi về việc gán màu nền/border khi `PatternType` hoặc `BorderStyle` chưa được thiết lập.
  - Đồng bộ hóa độ dịch chuyển dòng động (`currentShift`) khi xóa khối mẻ con trống (1 mẻ con) hoặc thêm mẻ con mới (>2 mẻ con) để bảo toàn tuyệt đối bố cục các phần QC, Sự cố và Chữ ký ở cuối trang.

---

## 3. Quy trình Kiểm thử và Đánh giá Chất lượng (Testing & Verification)

Để đảm bảo chương trình biên dịch thành công và xuất dữ liệu chính xác tuyệt đối, chúng tôi đã xây dựng chương trình kiểm thử tích hợp tự động:

### 3.1. Biên dịch dự án
- Sử dụng MSBuild thông qua script `scratch/find_and_build.py`. Biên dịch thành công dự án chính không có bất kỳ lỗi nào (`Build finished with code: 0`).

### 3.2. Chương trình kiểm thử tự động
- Tạo mới tệp kiểm thử độc lập **[scratch/TestExport.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/scratch/TestExport.cs)** liên kết trực tiếp với các DLL đã biên dịch và cơ sở dữ liệu thực tế tại `localhost:3306` (database `scada`).
- Chương trình kiểm thử thực hiện chạy xuất báo cáo cho 3 lô sản xuất đại diện cho các kịch bản thực tế:
  1. **Lô 3 (1 mẻ con)**: Xác minh đã xóa khối mẻ con 2 thành công, kéo Section 4 (Kết quả đầu ra) lên đúng dòng 30.
  2. **Lô 1 (4 mẻ con)**: Xác minh nhân bản thành công 4 mẻ con. Kiểm tra đánh số thứ tự từ 1 đến 4 ở cột A mẻ con động, kiểm tra sao chép định dạng in đậm (Bold) của tiêu đề mẻ con 3.
  3. **Lô 2 (5 mẻ con)**: Đảm bảo xuất thành công không lỗi.
- **Kết quả chạy thử nghiệm**:
  ```text
  === STARTING BATCH RECORD EXPORT TESTS ===

  [TEST 1] Exporting Batch 3 (1 run)...
  Saved to: c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\scratch\batch_record_3_20260612.xlsx
  Asserting Sheet Name: OK
  Found BOM shift: 0
  Row 30 Col A text: ''
  Assertion: Run 2 deleted successfully - OK

  [TEST 2] Exporting Batch 1 (4 runs)...
  Saved to: c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\scratch\batch_record_1_20260609.xlsx
  Found BOM shift: 15
  Run 1 Col A (Row 36): '1' (Expected: '1')
  Run 2 Col A (Row 46): '2' (Expected: '2')
  Run 3 Col A (Row 55): '3' (Expected: '3')
  Run 4 Col A (Row 64): '4' (Expected: '4')
  Assertion: Run duplication and numbering - OK
  Run 3 header bold style on row 54: True (Expected: True)
  Assertion: Style copying - OK

  [TEST 3] Exporting Batch 2 (5 runs)...
  Saved to: c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2\scratch\batch_record_2_20260612.xlsx
  Assertion: Batch 2 export - OK

  === ALL TESTS PASSED SUCCESSFULLY! ===
  ```

---

## 4. Hướng dẫn sử dụng và kiểm tra thủ công

1. Khởi động Web App Long Đức trên máy chủ IIS Express / Localhost.
2. Đăng nhập tài khoản có quyền **Admin**.
3. Truy cập trang **Batches** (URL: `/Home/Event`).
4. Tìm đến lô sản xuất cần báo cáo, nhấn nút **Export BPR**.
5. Kiểm tra file Excel tải về trong thư mục Downloads của bạn. Tên file sẽ ở dạng `batch_record_{id}_{date}.xlsx`. Mở file bằng Microsoft Excel để kiểm tra giao diện hiển thị.
