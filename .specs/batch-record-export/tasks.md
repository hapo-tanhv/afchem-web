# Quy trình triển khai: Báo cáo Lô sản xuất (Batch Production Record) Export

## Danh sách công việc

- [x] 1. Tích hợp nút bấm và giao diện trên Frontend
- [x] 1.1 (P) Thiết kế và tích hợp nút bấm xuất báo cáo trên trang Event
  - Bổ sung nút bấm "Export BPR" bên cạnh nút "Export Excel" hiện tại trong file giao diện trang Event
  - Lồng cấu trúc điều kiện kiểm tra phân quyền để chỉ hiển thị nút bấm đối với người dùng có quyền Admin
  - _Requirements: 6.1_

- [x] 1.2 (P) Lập trình Javascript kích hoạt sự kiện tải báo cáo
  - Viết hàm xử lý sự kiện click nút bấm "Export BPR" thu thập các bộ lọc hiện tại (mã lô, mã mẻ con, ngày sản xuất)
  - Điều hướng trình duyệt thực hiện request tải file từ endpoint API `/Event/ExportBatchRecordExcel` kèm các tham số lọc tương ứng
  - _Requirements: 6.2_

- [x] 2. Xây dựng Controller Endpoint và Phân quyền
- [x] 2.1 (P) Tạo endpoint API tiếp nhận yêu cầu xuất báo cáo trong Controller
  - Thêm phương thức hành động `ExportBatchRecordExcel` tiếp nhận tham số mã lô `batchId` dạng GET
  - Tích hợp kiểm tra quyền truy cập Admin của phiên làm việc hiện tại, trả về lỗi cấm truy cập nếu không đủ quyền
  - Thực hiện validate tham số đầu vào và trả về FileResult chứa file Excel kết quả
  - _Requirements: 1.3, 6.2_

- [x] 3. Phát triển Dịch vụ nghiệp vụ Xuất báo cáo (EPPlus Service)
- [x] 3.1 Thiết lập cấu trúc cơ bản cho Service xuất báo cáo Excel
  - Tạo mới interface và class dịch vụ xuất báo cáo nhận kết nối cơ sở dữ liệu và đường dẫn file Excel mẫu làm đầu vào
  - Viết logic mở file template Excel dưới dạng luồng dữ liệu chỉ đọc thông qua EPPlus
  - _Requirements: 1.1_

- [x] 3.2 (P) Xây dựng hàm helper giải mã payload webhook
  - Viết hàm tách chuỗi URL-encoded từ cơ sở dữ liệu thành từ điển khóa-giá trị
  - Viết hàm giải mã chuỗi Base64 BOM và chuyển đổi thành danh sách đối tượng vật tư dạng JSON
  - _Requirements: 2.1, 3.1_

- [x] 3.3 Điền thông tin chung và xử lý danh sách nguyên vật liệu (BOM)
  - Thực hiện truy vấn thông tin lô sản xuất từ cơ sở dữ liệu và webhook logs, điền các giá trị vào các ô tọa độ cố định của mục 1
  - Viết thuật toán chèn dòng động tại mục 2 (BOM) khi số lượng vật tư vượt quá 5 dòng, bao gồm việc copy định dạng ô từ dòng mẫu sang dòng mới chèn
  - Giải mã và điền thông tin chi tiết vật tư vào bảng BOM nguyên vật liệu ở mục 2
  - _Requirements: 2.2, 3.2, 3.3_

- [x] 3.4 Điền thông số quá trình và xử lý số lượng mẻ con động
  - Truy vấn danh sách mẻ con thực tế của lô sản xuất từ cơ sở dữ liệu
  - Viết thuật toán nhân bản hoặc xóa khối bảng thông số 8 công đoạn của mục 3 dựa trên số mẻ con thực tế (N mẻ con)
  - Lọc dữ liệu nhật ký sự kiện và đo lường SCADA theo thời gian chạy từng công đoạn để tính toán dải nhiệt độ bồn trộn và điền vào bảng
  - _Requirements: 4.1, 4.2, 4.3, 4.4_

- [x] 3.5 Điền QC, xử lý sự cố phát sinh và dọn dẹp bảng biểu
  - Điền các chỉ tiêu QC lô ở mục 5 ở dạng trống hoặc trạng thái mặc định
  - Truy vấn các lỗi cảnh báo nghiêm trọng từ cơ sở dữ liệu thuộc các mẻ con để điền vào bảng sự cố mục 6
  - Thiết lập dọn dẹp tệp Excel: xóa bỏ các worksheet hướng dẫn/nháp và đổi tên sheet chính thành tên chuẩn trước khi xuất
  - _Requirements: 5.1, 5.2, 5.3_

- [x] 4. Kiểm thử và Xác minh chất lượng
- [x] 4.1 (P) Viết unit tests kiểm tra các hàm xử lý dữ liệu logic
  - Thực hiện viết kiểm thử đơn lẻ cho hàm giải mã payload URL-encoded và hàm parse JSON BOM từ chuỗi Base64
  - Thực hiện viết kiểm thử đơn lẻ kiểm tra tính đúng đắn của hàm clone row style trong EPPlus
  - _Requirements: 1.2, 3.3_

- [x] 4.2 Viết integration tests kiểm tra định dạng và cấu trúc file xuất ra
  - Viết kiểm thử tích hợp thực hiện xuất lô với số lượng mẻ con khác nhau (1 mẻ, 2 mẻ, 3 mẻ) và kiểm tra cấu trúc dòng trong Excel
  - Xác minh độ nguyên vẹn của định dạng (border, background color, font chữ) sau khi thực hiện chèn dòng và nhân bản mẻ con
  - _Requirements: 1.2, 3.3, 4.3_

- [x] 4.3* Viết tests kiểm tra luồng giao diện và tải file báo cáo
  - Viết kiểm thử tích hợp giao diện giả lập thao tác click nút bấm xuất báo cáo của Admin
  - Kiểm tra tính đúng đắn của định dạng tên file tải về và mã HTTP trả về từ Controller
  - _Requirements: 6.2_
