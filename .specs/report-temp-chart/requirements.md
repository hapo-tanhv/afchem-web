# Tài liệu Yêu cầu (Requirements Document) - Biểu đồ Nhiệt độ trang Báo cáo

## Giới thiệu (Introduction)
Dự án yêu cầu bổ sung một biểu đồ đường (Line Chart) trực quan hóa các dải nhiệt độ lịch sử trên trang Báo cáo (Report). Biểu đồ này sẽ nằm ngay phía trên bảng dữ liệu báo cáo hiện tại, lấy dữ liệu từ bảng cơ sở dữ liệu `alarmreport`. Mục đích chính là cung cấp một góc nhìn tổng quan trực quan giúp người vận hành dễ dàng so sánh dải nhiệt độ thực tế của bồn trộn (trên, giữa, dưới) và môi trường với tiêu chuẩn thiết lập.

---

## Yêu cầu (Requirements)

### Requirement 1: Hiển thị Biểu đồ Nhiệt độ (Temperature Line Chart)
**Mục tiêu (Objective):** Là Người vận hành, tôi muốn nhìn thấy một biểu đồ đường biểu diễn các thông số nhiệt độ phía trên bảng báo cáo, để tôi có thể nhanh chóng so sánh và đánh giá các dải nhiệt độ.

#### Tiêu chí Nghiệm thu (Acceptance Criteria)
1. Hệ thống sẽ hiển thị một biểu đồ đường sử dụng thư viện **Highcharts** (đã dùng ở trang Tổng quan) đặt ở phía trên bảng dữ liệu báo cáo.
2. Khi báo cáo có dữ liệu, biểu đồ sẽ vẽ **5 đường (Series)**:
   - **Đường 1: Tiêu chuẩn nhiệt độ** (đường nằm ngang cố định ở giá trị `40.0 °C`).
   - **Đường 2: Nhiệt độ môi trường** (`NhietDoMoiTruong`).
   - **Đường 3: Nhiệt độ bồn trộn trên** (`NhietDoBonTronTren`).
   - **Đường 4: Nhiệt độ bồn trộn giữa** (`NhietDoBonTronGiua`).
   - **Đường 5: Nhiệt độ bồn trộn dưới** (`NhietDoBonTronDuoi`).
3. Khi người dùng di chuột (hover) qua bất kỳ điểm nào trên biểu đồ, hệ thống sẽ hiển thị một tooltip chung (shared tooltip) kèm theo đường gióng đứng (crosshair) chỉ rõ giá trị nhiệt độ của cả 5 đường tại thời điểm đó để dễ dàng đối chiếu.
4. Hệ thống sẽ sử dụng mốc thời gian thực tế (`DateTime` hoặc `HH:mm:ss`) từ bản ghi làm trục hoành (X-Axis) cho biểu đồ.

### Requirement 2: Đồng bộ Bộ lọc và Tối ưu hóa Dữ liệu (Filter Sync & Downsampling)
**Mục tiêu (Objective):** Là Người vận hành, tôi muốn biểu đồ tự động cập nhật theo bộ lọc tìm kiếm và hoạt động mượt mà, để đảm bảo số liệu trên biểu đồ luôn khớp với bảng và không gây đơ/lag trình duyệt.

#### Tiêu chí Nghiệm thu (Acceptance Criteria)
1. Khi người dùng thay đổi các bộ lọc tìm kiếm (Thời gian bắt đầu/kết thúc, Lô hàng - Batch, Lần chạy - Run, hoặc Ô tìm kiếm nhanh) và bấm nút "Tìm kiếm", hệ thống sẽ truy vấn dữ liệu từ bảng `alarmreport` và tải lại (reload) dữ liệu biểu đồ đồng thời.
2. Nếu tập dữ liệu truy vấn được quá lớn (ví dụ: trên 1000 bản ghi), hệ thống sẽ thực hiện lấy mẫu (downsample) tối đa **1000 điểm đo** phân bổ đều theo thời gian để vẽ lên biểu đồ nhằm tối ưu hóa hiệu năng render của trình duyệt.
3. Nếu kết quả truy vấn không có dữ liệu, biểu đồ sẽ hiển thị thông báo "Không có dữ liệu hiển thị".

### Requirement 3: Hộp Card Biểu đồ Thu gọn / Mở rộng (Collapsible Chart Card)
**Mục tiêu (Objective):** Là Người vận hành, tôi muốn có khả năng thu gọn hoặc mở rộng khung biểu đồ, để tôi có thể tối ưu hóa diện tích hiển thị màn hình khi chỉ cần tập trung xem bảng dữ liệu.

#### Tiêu chí Nghiệm thu (Acceptance Criteria)
1. Hệ thống sẽ đặt biểu đồ nằm bên trong một khung hộp dạng **Card** của AdminLTE/Bootstrap có nút toggle Thu gọn/Mở rộng (Collapse/Expand).
2. Khi người dùng bấm thu gọn, hệ thống sẽ ẩn biểu đồ đi và tự động đẩy bảng dữ liệu DataTable lên phía trên để tối ưu hóa không gian.
