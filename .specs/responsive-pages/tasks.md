# Implementation Plan - Responsive Pages & Sidebar

## Major + Sub-task structure

- [x] 1. Tối ưu hóa Sidebar và Khung giao diện chính (Layout & Sidebar Responsiveness)
- [x] 1.1 (P) Thêm nút Hamburger Toggle và kích hoạt tính năng đóng mở Sidebar
  - Bổ sung nút nhấn Hamburger với thuộc tính `data-widget="pushmenu"` vào góc trái Header của layout chính.
  - Viết các rule CSS để đảm bảo khi thu gọn Sidebar, Header vẫn giữ nguyên vị trí căn lề mà không bị lệch bố cục.
  - _Requirements: 1.1, 1.2_

- [x] 1.2 (P) Cấu hình Sidebar tự động ẩn và căn lề nội dung chính trên màn hình nhỏ
  - Cấu hình CSS để tự động ẩn hoàn toàn thanh điều hướng khi chiều rộng màn hình dưới 992px.
  - Đảm bảo vùng nội dung chính co giãn sát mép trái màn hình khi Sidebar ẩn đi.
  - Kích hoạt chế độ cuộn dọc độc lập cho menu Sidebar khi danh sách menu dài hơn chiều cao màn hình.
  - _Requirements: 1.3, 1.4, 1.5_

- [x] 2. Điều chỉnh Responsive cho các bộ lọc và các bảng biểu (Filters, Tables & Charts)
- [x] 2.1 (P) Tối ưu hóa bộ lọc tìm kiếm thành dạng quấn hàng hoặc cột dọc
  - Cấu hình bộ lọc co giãn linh hoạt hàng ngang trên màn hình rộng (>1200px) và tự động xuống dòng (wrap) trên màn hình trung bình (768px-1200px).
  - Tự động chuyển đổi các hộp chọn và nút bấm sang dạng cột dọc chiếm 100% chiều ngang trên màn hình di động (<768px).
  - Căn giữa các nút bấm Tìm kiếm, Xuất Excel/CSV khi ở chế độ hiển thị trên di động.
  - _Requirements: 2.1, 2.2, 2.3, 2.4_

- [x] 2.2 (P) Tối ưu hóa bảng DataTables hiển thị đa cột và co giãn biểu đồ
  - Đảm bảo tất cả bảng dữ liệu nhiều cột được bao quanh bởi thẻ div có lớp cuộn ngang độc lập để tránh xô lệch layout.
  - Tự động giảm kích thước chữ và khoảng đệm (padding) của tiêu đề và các ô dữ liệu trên màn hình hẹp dưới 1200px.
  - Cấu hình biểu đồ tròn và biểu đồ lịch sử tự động co dãn theo 100% chiều rộng của thẻ chứa cha.
  - _Requirements: 3.1, 3.2, 3.3_

- [x] 3. Tối ưu hóa hiển thị trang Tổng quan (Overview Page Responsiveness)
- [x] 3.1 (P) Thay đổi sơ đồ bồn trộn chính sang dạng xếp chồng dọc
  - Cấu hình CSS để xếp các khối thông tin thiết bị phụ trợ (bơm, van) bên cạnh bồn trộn chính theo hàng dọc thay vì hàng ngang khi dưới 1500px.
  - Tự động thu nhỏ tỉ lệ (scale) hình ảnh bồn trộn chính và các thẻ giá trị đo đạc để vừa khít màn hình di động (<768px).
  - _Requirements: 4.2, 4.3_

- [x] 3.2 (P) Thiết kế lại thanh tiến trình Timeline sang dạng danh sách dọc
  - Cấu hình CSS để chuyển đổi Timeline 8 bước từ dạng ngang sang dạng danh sách bước xếp dọc khi màn hình dưới 1200px.
  - _Requirements: 4.1_

- [x] 4. Kiểm thử và Xác minh toàn diện (Testing & Verification)
- [x] 4.1 Kiểm thử tích hợp hiển thị giao diện trên các môi trường di động và máy tính bảng giả lập
  - Chạy ứng dụng web trên môi trường local.
  - Sử dụng công cụ nhà phát triển trên trình duyệt để kiểm tra trực quan giao diện ở các kích thước màn hình 1440px, 1366px, 1024px, 768px và 375px.
  - Xác nhận không còn tình trạng tràn viền, chồng chéo chữ hoặc vỡ bố cục Header.
  - _Requirements: 1.1, 1.3, 2.3, 3.1, 4.3_

- [x] 4.2* Thực hiện kiểm thử tự động render bố cục của các trang ở chế độ không đầu (headless) mô phỏng các breakpoint
  - Thực hiện kiểm thử render giao diện để ghi nhận tính ổn định của layout mà không bị lệch vị trí.
  - _Requirements: 1.1, 2.1, 3.1, 4.1_
