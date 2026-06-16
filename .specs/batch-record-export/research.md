# Research & Design Decisions: Báo cáo Lô sản xuất (Batch Production Record) Export

---
**Purpose**: Ghi nhận kết quả nghiên cứu cấu trúc dữ liệu, khảo sát file mẫu Excel và đưa ra các quyết định thiết kế kỹ thuật cho tính năng xuất báo cáo Lô sản xuất.

---

## Summary
- **Feature**: `batch-record-export`
- **Discovery Scope**: Complex Integration
- **Key Findings**:
  - Dự án hiện đang sử dụng thư viện **EPPlus** phiên bản 5+ (được cấu hình `LicenseContext = LicenseContext.NonCommercial` trong `ExportUtility.cs`). EPPlus hỗ trợ đầy đủ việc đọc file mẫu, chèn dòng (`InsertRow`), sao chép style, merge cell và lưu thành file mới.
  - File mẫu [structure_batch_export.xlsx](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/docs/structure_batch_export.xlsx) chứa 3 sheet, trong đó sheet chính cần xuất dữ liệu là **`Sheet1`** (kích thước A1:M67) có đầy đủ các thông số SCADA của bồn trộn. Sheet `Nhat ky san xuat` có định dạng checklist thủ công đơn giản hơn, không phù hợp cho tự động hóa SCADA. Sheet `Huong dan` chứa hướng dẫn vận hành.
  - Dữ liệu webhook lưu trong bảng `webhook_logs` (cột `payload`) dạng chuỗi URL-encoded. Khi giải mã thu được các tham số BOM nguyên vật liệu (`custom_thong_tin_bom_san_xuat_a`, `_b`...) dạng Base64 chứa mảng JSON của vật tư đầu vào.
  - Thời gian tạo lô `batches.created_at` trùng khớp hoàn toàn với `webhook_logs.received_at`, là khóa liên kết duy nhất để tìm webhook payload tương ứng với lô sản xuất.

---

## Research Log

### 1. Khảo sát Cấu trúc File Mẫu Excel
- **Context**: Cần xác định chính xác sheet mẫu và các vị trí ô cần điền dữ liệu.
- **Sources Consulted**: Đọc cấu trúc trực tiếp từ file [structure_batch_export.xlsx](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/docs/structure_batch_export.xlsx) bằng Python openpyxl.
- **Findings**:
  - `Sheet1` chứa layout chuẩn BPR (Batch Production Record).
  - Vùng thông tin chung (Section 1) nằm ở Row 5 - Row 9, sử dụng merge cell 2 cột hoặc 3 cột (ví dụ: `B5:C5` cho Tên sản phẩm, `E5:F5` cho Mã hàng FG).
  - Vùng nguyên vật liệu đầu vào (Section 2) nằm ở Row 13 - Row 17 (mặc định 5 dòng trống).
  - Vùng thông số quá trình (Section 3) được thiết kế sẵn cho 2 mẻ con: Mẻ 1 ở Row 21-28, Mẻ 2 ở Row 31-38. Mỗi mẻ có 8 dòng công đoạn.
- **Implications**: 
  - Ta sẽ dùng `Sheet1` làm template gốc.
  - Điền dữ liệu vào đúng các tọa độ ô được xác định.
  - Cần viết thuật toán dịch chuyển dòng động khi số lượng nguyên vật liệu (BOM) hoặc số lượng mẻ con (runs) vượt quá thiết kế tĩnh của template.

### 2. Giải mã Payload Webhook trong C#
- **Context**: Bảng `webhook_logs` chứa payload thô cần được parse trong ASP.NET Controller.
- **Sources Consulted**: File `docs/data_example.txt` và `docs/api-integration-guide.md`.
- **Findings**:
  - Payload dạng URL-encoded: `custom_lotno=JEL-101-260609TX01&custom_ten_hang_hoa=...`
  - BOM được mã hóa Base64: `custom_thong_tin_bom_san_xuat_a=W1siS0VPLTAx...`
  - Chuỗi Base64 giải mã ra mảng JSON: `[["KEO-014","AF-01","382.96","382.96","kg","2026033111F"], ...]`
- **Implications**:
  - Viết helper method trong C# để parse URL-encoded thành `Dictionary<string, string>`.
  - Dùng thư viện `Newtonsoft.Json` (đã có sẵn trong ASP.NET MVC) để parse chuỗi JSON của BOM sau khi giải mã Base64 bằng `Convert.FromBase64String`.

---

## Architecture Pattern Evaluation

| Hướng tiếp cận | Mô tả | Ưu điểm | Nhược điểm | Đánh giá |
|:---|:---|:---|:---|:---|
| **Code-driven Styling** | Tạo file Excel mới hoàn toàn, dùng code C# vẽ từng cell, border, set font và màu nền. | Không phụ thuộc vào file mẫu bên ngoài, kiểm soát 100% bằng code. | Code cực kỳ dài dòng, tốn hiệu năng, khó bảo trì khi layout thay đổi. | **Không chọn** |
| **Template-based Fill** | Load file `structure_batch_export.xlsx`, điền dữ liệu vào các ô tương ứng, lưu lại thành file mới. | Giữ nguyên 100% format gốc, code ngắn gọn, dễ dàng thay đổi mẫu Excel mà không cần sửa code. | Cần xử lý dịch chuyển dòng và nhân bản cell khi dữ liệu vượt quá kích thước mẫu. | **Chọn (Tối ưu nhất)** |

---

## Design Decisions

### Quyết định: Phương án xử lý dịch chuyển dòng động (Dynamic Row Shifting)
- **Context**:
  - Section 2 (BOM) chỉ có sẵn 5 dòng (Row 13-17). Nếu lô có 10 nguyên vật liệu, ta cần chèn thêm 5 dòng.
  - Khi chèn dòng tại Row 18, EPPlus sẽ tự động đẩy toàn bộ Section 3 (Thông số quá trình) và các section phía dưới xuống dưới (dòng 22 trở đi).
- **Giải pháp lựa chọn**:
  - Dùng `worksheet.InsertRow(startRow, count, copyStylesFromRow)` để chèn dòng động.
  - Viết hàm helper sao chép định dạng ô (border, font, background) từ dòng mẫu (Row 13) sang các dòng mới chèn, vì EPPlus đôi khi không tự động kế thừa tất cả style phức tạp (như border nét đứt/nét liền).
  - Cập nhật công thức và tham chiếu nếu có.

### Quyết định: Nhân bản bảng thông số quá trình cho nhiều mẻ con (Run Duplication)
- **Context**: Lô sản xuất có số mẻ con động (`total_runs` từ 1 đến N). Template chỉ vẽ sẵn 2 mẻ con.
- **Giải pháp lựa chọn**:
  - Nếu số mẻ con $N = 1$: Xóa bảng mẻ 2 trong template (xóa các dòng từ 30 đến 38).
  - Nếu số mẻ con $N = 2$: Giữ nguyên và điền dữ liệu.
  - Nếu số mẻ con $N > 2$: Nhân bản khối 9 dòng của mẻ 2 (Row 30 đến Row 38) cho các mẻ tiếp theo ($i = 3...N$). Sử dụng `worksheet.InsertRow` tại vị trí cuối bảng mẻ trước, sau đó sao chép giá trị và định dạng từ mẻ 2 sang khối mới chèn, rồi điền dữ liệu của mẻ con tương ứng.

---

## Risks & Mitigations
- **Rủi ro 1**: Lỗi định dạng khi chèn dòng trong EPPlus (mất border hoặc lỗi merge cell).
  - *Giảm thiểu*: Thiết lập quy trình clone style chi tiết cho từng ô trong dòng được chèn mới. Sử dụng thuộc tính `StyleID` của EPPlus để tối ưu hóa hiệu năng và độ chính xác của định dạng.
- **Rủi ro 2**: Không tìm thấy webhook log tương ứng với lô sản xuất.
  - *Giảm thiểu*: Fallback lấy dữ liệu định mức trực tiếp từ bảng `run_info` (nếu webhook đã được parse lưu vào DB từ trước) hoặc để trống các ô không tìm thấy dữ liệu và ghi log cảnh báo.
- **Rủi ro 3**: Trùng lặp/trôi dữ liệu do cache file Excel mẫu.
  - *Giảm thiểu*: Mỗi yêu cầu export sẽ đọc file mẫu từ ổ đĩa dưới dạng stream chỉ đọc (`FileShare.Read`), khởi tạo một đối tượng `ExcelPackage` mới trong khối `using` để đảm bảo giải phóng tài nguyên.
