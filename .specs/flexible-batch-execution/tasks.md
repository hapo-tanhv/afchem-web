# Danh sách Task Triển khai (Implementation Tasks)

## 1. Phát triển API Backend
- [ ] 1. Phát triển các API xử lý dữ liệu và nghiệp vụ cho việc thay đổi mẻ chạy tại Backend

- [ ] 1.1 (P) Xây dựng API lấy danh sách các Batch và Mẻ chạy chờ (Standby)
  - Thực hiện truy vấn danh sách các batch có trạng thái chờ (Pending hoặc Active) và còn chứa các mẻ chạy chưa thực hiện (trạng thái Pending, Waiting hoặc Created)
  - Trả về danh sách các batch kèm theo danh sách mẻ tương ứng dưới dạng JSON
  - Tính toán tổng số lượng batch chờ và mẻ chờ để hiển thị thông tin thống kê
  - _Requirements: 1.2, 1.6_

- [ ] 1.2 (P) Xây dựng API kích hoạt mẻ chạy được chọn
  - Thiết lập endpoint POST nhận vào mã định danh của batch và mẻ chạy được chọn
  - Thực hiện kiểm tra an toàn xem cơ sở dữ liệu có mẻ chạy nào đang ở trạng thái hoạt động (Active) hay không
  - Nếu đang có mẻ chạy hoạt động, từ chối và trả về lỗi thông báo cho người dùng
  - Nếu không có mẻ hoạt động, bắt đầu một database transaction:
    - Chuyển trạng thái của tất cả các batch đang ở trạng thái hoạt động (Active) về trạng thái chờ (Pending)
    - Cập nhật batch được chọn thành trạng thái hoạt động (Active)
    - Cập nhật mẻ được chọn thành trạng thái hoạt động (Active), đặt thời gian bắt đầu (start_time) là thời gian hiện tại
    - Truy vấn giá trị thứ tự thực hiện lớn nhất (execution_order) trong bảng mẻ chạy, tăng thêm 1 và gán cho mẻ chạy vừa chọn
  - Trả về kết quả JSON báo thành công hoặc thất bại
  - _Requirements: 2.1, 2.2, 2.3, 2.4_

## 2. Phát triển Giao diện Frontend UI
- [ ] 2. Thiết kế giao diện và lập trình tương tác cho khối điều khiển Lựa chọn Batch tại Overview

- [ ] 2.1 (P) Thiết kế layout HTML/CSS cho khối Lựa chọn Batch
  - Thêm một panel giao diện cao cấp (card layout) phía trên cùng của màn hình Overview
  - Chia panel làm hai phần: bên trái hiển thị thông tin thống kê số lượng batch/mẻ chờ, bên phải chứa hai bộ chọn (select) cho Batch, Mẻ chạy và nút bấm "Bắt đầu mẻ"
  - Thiết kế kiểu dáng Dark Theme SCADA đồng bộ với giao diện hiện tại của trang Overview
  - _Requirements: 1.1, 1.2, 1.3_

- [ ] 2.2 Tải dữ liệu và xử lý thay đổi trên Dropdown Select
  - Thực hiện gọi API lấy danh sách standby khi tải trang để đổ dữ liệu vào bộ chọn Batch
  - Lập trình sự kiện thay đổi lựa chọn Batch: khi người dùng chọn một batch, tự động lọc và cập nhật danh sách mẻ chạy chờ tương ứng của batch đó vào bộ chọn Mẻ chạy
  - _Requirements: 1.6_

- [ ] 2.3 Khóa/Mở khóa điều khiển dựa trên trạng thái hoạt động
  - Đọc thông tin trạng thái mẻ chạy từ dữ liệu polling realtime định kỳ
  - Nếu phát hiện có mẻ đang chạy (trạng thái Active), thực hiện khóa (disable) cả 2 dropdown chọn và nút "Bắt đầu mẻ"
  - Nếu không có mẻ nào chạy, thực hiện mở khóa (enable) các điều khiển để cho phép lựa chọn
  - _Requirements: 1.4, 1.5_

- [ ] 2.4 Gửi yêu cầu kích hoạt mẻ chạy và cập nhật giao diện
  - Thiết lập sự kiện click cho nút "Bắt đầu mẻ": hiển thị modal xác nhận thông tin mẻ chạy đã chọn
  - Gửi yêu cầu POST AJAX lên API backend sau khi người dùng xác nhận
  - Xử lý phản hồi từ server: hiển thị toastr thông báo thành công hoặc báo lỗi cụ thể
  - Nếu kích hoạt thành công, kích hoạt hàm lấy lại thông tin mẻ chạy realtime ngay lập tức để đồng bộ toàn bộ số liệu trên màn hình
  - _Requirements: 1.7_

## 3. Kiểm thử & Tích hợp
- [ ] 3. Thực hiện viết mã kiểm thử tự động và xác minh luồng nghiệp vụ trên hệ thống

- [ ] 3.1 Viết kiểm thử tích hợp (Integration Tests) cho API Backend
  - Thiết lập các test case kiểm tra API thay đổi mẻ chạy với dữ liệu giả lập
  - Kiểm thử trường hợp chặn cập nhật khi có mẻ đang Active
  - Kiểm thử việc cập nhật chính xác trạng thái và thứ tự thực thi execution_order khi mẻ chạy hợp lệ được kích hoạt
  - _Requirements: 2.1, 2.2, 2.3, 2.4_

- [ ] 3.2 Kiểm thử E2E thủ công các luồng vận hành trên giao diện
  - Thực hiện xác minh hiển thị và trạng thái đóng/mở khóa của bộ chọn mẻ trên trình duyệt
  - Vận hành luồng đổi mẻ chạy từ giao diện Overview và xác nhận dữ liệu hiển thị cập nhật chính xác
  - Xác minh dữ liệu trong MySQL database sau khi đổi mẻ chạy khớp với thiết kế
  - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7_
