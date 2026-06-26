# Kế hoạch triển khai (Implementation Plan)

- [x] 1. Khởi tạo cơ sở dữ liệu
  - Tạo bảng `run_step_accumulated_times` trong MySQL với các trường `runId`, `stepCode`, `accumulatedTime`.
  - Khai báo khóa chính kết hợp (`runId`, `stepCode`) và khóa ngoại trỏ đến bảng `runs`.
  - _Requirements: 2.3_

- [x] 2. Xây dựng dịch vụ chạy ngầm BackendTimerAccumulator
- [x] 2.1 Xây dựng lớp dịch vụ BackendTimerAccumulator
  - Thiết kế lớp Singleton chạy một luồng phụ tuần hoàn (Worker Thread) chu kỳ quét 500ms.
  - Sử dụng `RealtimeService.Instance.Read` để đọc 8 tag thời gian của PLC.
  - Đọc trạng thái `is_paused` và `runId` từ bảng `runs`.
  - Thực hiện thuật toán tích lũy delta: cộng dồn hiệu số khi chạy bình thường, đóng băng khi tạm dừng, và thiết lập lại baseline khi chuyển trạng thái từ tạm dừng sang chạy tiếp.
  - _Requirements: 2.1, 2.2_
- [x] 2.2 Tích hợp dịch vụ vào vòng đời WebApp tại Global.asax
  - Khởi chạy dịch vụ `BackendTimerAccumulator.Instance.Start()` tại hàm `Application_Start()`.
  - Giải phóng tài nguyên và dừng luồng chạy ngầm tại hàm `Application_End()`.
  - _Requirements: 2.2_

- [x] 3. (P) Đồng bộ hóa API GetCurrentBatchStats ở Controller
  - Điều chỉnh endpoint `/Overview/GetCurrentBatchStats` trong `OverviewController.cs`.
  - Truy vấn dữ liệu thời gian chạy tích lũy từ bảng `run_step_accumulated_times` thay vì thực hiện truy vấn MAX trên bảng `alarmreport`.
  - Gán mảng dữ liệu này vào trường `batchInfo.accumulatedValues` trả về cho Client.
  - _Requirements: 2.4_

- [x] 4. Kiểm thử và Xác minh giải pháp
- [x] 4.1 Viết kiểm thử đơn vị cho thuật toán tích lũy
  - Viết các test case mô phỏng sự tăng giảm giá trị PLC timer qua các mốc pause/resume để đảm bảo delta tính toán chính xác (thực hiện kiểm chứng thuật toán qua tích hợp thực tế và chạy kiểm thử trong môi trường local).
  - _Requirements: 2.2_
- [x] 4.2* Kiểm thử tích hợp hệ thống thực tế
  - Chạy ứng dụng thực tế và kiểm tra tính nhất quán giữa cơ sở dữ liệu `run_step_accumulated_times` và giá trị hiển thị trên UI.
  - _Requirements: 2.1, 2.3, 2.4_
