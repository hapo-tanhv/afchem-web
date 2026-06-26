# Hướng Dẫn Kỹ Thuật: Hiển Thị Nhiều Công Đoạn Hoạt Động Song Song (Concurrent Steps)

Tài liệu này giải thích chi tiết cơ chế hoạt động và cách triển khai hệ thống xác định trạng thái các công đoạn chạy song song trong **Trạng thái quy trình** (Timeline) dựa trên dữ liệu `alarmlog` thực tế.

---

## 1. Cơ Chế Xác Định Ở Backend (C# Controllers)
Backend truy vấn và xử lý logic tại lớp [OverviewController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/OverviewController.cs) qua hàm `GetCurrentBatchStats`:

- **Quét Trạng Thái Sự Kiện**: Truy vấn dữ liệu từ bảng `alarmlog` dựa theo `runId` hiện tại.
- **Xác Định Nhiều Bước Active**:
  - Hệ thống duy trì một danh sách các mã công đoạn đang chạy (`activeStepCodes`) và tên công đoạn tương ứng (`activeStepNames`).
  - Nếu bất kỳ bản ghi sự kiện công đoạn nào trong `alarmlog` có trạng thái `Status = "Alarm"` (nghĩa là công đoạn đã bắt đầu và chưa có tín hiệu kết thúc), Backend sẽ đưa mã bước đó vào danh sách `activeStepCodes`.
- **Ánh Xạ Trạng Thái Cho Frontend**:
  - Mỗi bước gửi về giao diện sẽ nhận thuộc tính `status` như sau:
    - `"completed"`: Khi bước đó có bản ghi `"Resolved"` trong `alarmlog`.
    - `"in-progress"`: Khi bước đó đang có bản ghi `"Alarm"` trong `alarmlog` hoặc được suy luận dự phòng là công đoạn đang hoạt động.
    - `"pending"`: Quy trình chưa bắt đầu chạy tới bước đó.

---

## 2. Cơ Chế Hiển Thị Ở Frontend (JavaScript UI)
Frontend nhận dữ liệu phản hồi thông qua file [OverviewRealtime.js](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/JavaScript/RealTime/OverviewRealtime.js):

- **Timeline Quy Trình (`updateTimelineUI`)**:
  - Hàm cập nhật giao diện Timeline không còn so sánh tuyến tính dạng `stepNum === activeStepCode` nữa.
  - Thay vào đó, nó kiểm tra trạng thái của từng bước trong mảng dữ liệu thực tế (`window.currentSteps`):
    - Nếu trạng thái của bước là `"completed"` $\rightarrow$ Thêm class `completed` (tích xanh, hiển thị màu xanh lá cây báo hoàn thành).
    - Nếu trạng thái của bước là `"in-progress"` $\rightarrow$ Thêm class `active` (viền nhấp nháy chuyển động). Hỗ trợ nhiều bước cùng hoạt động đồng thời (ví dụ: cả Xả đáy, Rung xả đáy và Hút xả đáy cùng nhấp nháy nhịp nhàng).
- **Thống Kê Bước Hiện Tại (`statCurrentStep`)**:
  - Duyệt qua `window.currentSteps` và tìm tất cả các bước có trạng thái `"in-progress"`.
  - Tự động gộp mã của tất cả các bước đang chạy lại và hiển thị lên khung thông tin (ví dụ: `3,4,5 / 8` thay vì chỉ hiển thị một số nguyên đơn lẻ).
- **Đồng Bộ Dòng Chảy SVG (`updateSvgFlows`)**:
  - Bản thân các dòng chảy động (Feeding, Mixing, Discharge) đã được liên kết với trạng thái `"in-progress"` của các bước tương ứng từ trước, nên khi các bước này cùng chạy song song, các dòng chảy SVG cũng sẽ tự động kích hoạt đồng bộ.
