# Yêu cầu kỹ thuật (Requirements Document) - Responsive Pages & Sidebar

## Introduction
Dự án **AFCHEM SCADA System** hiện tại đã có cơ chế responsive cơ bản cho phần Header và một số widget của trang Tổng quan. Tuy nhiên, thanh điều hướng Sidebar và các trang chức năng cốt lõi (Cảnh báo, Batches, Báo cáo, Cài đặt hệ thống) vẫn gặp tình trạng vỡ layout hoặc hiển thị kém tối ưu trên các thiết bị di động, máy tính bảng và các dòng laptop MacBook có màn hình thu nhỏ (dưới 1280px). Tài liệu này xác định các yêu cầu kỹ thuật chi tiết nhằm tối ưu hóa hiển thị đáp ứng (responsive) trên toàn bộ hệ thống.

---

## Requirements

### 1. Sidebar và Khung Giao Diện Chính (Layout & Sidebar Responsiveness)
**Objective:** Là một người vận hành hệ thống, tôi muốn thanh menu Sidebar có thể tự động thu gọn hoặc ẩn đi trên màn hình nhỏ và cho phép đóng/mở thủ công, để tối đa hóa không gian hiển thị biểu đồ và bảng số liệu SCADA.

#### Acceptance Criteria
1. `The system shall` hiển thị một nút Hamburger Menu (Toggle Sidebar) ở phía bên trái Header, ngay trước Logo thương hiệu.
2. `When` người dùng nhấn vào nút Hamburger Menu, `the system shall` kích hoạt đóng/mở thanh Sidebar (collapsible sidebar) thông qua cơ chế toggle lớp `sidebar-collapse` của AdminLTE.
3. `While` chiều rộng màn hình nhỏ hơn `992px` (máy tính bảng và thiết bị di động), `the sidebar shall` tự động ẩn hoàn toàn (collapsed) để tránh che khuất nội dung trang.
4. `While` thanh Sidebar đang ở trạng thái ẩn, `the content wrapper shall` tự động co dãn rộng ra sát mép trái màn hình (`margin-left: 0`).
5. `The sidebar shall` sử dụng thanh cuộn ẩn (overlay scrollbar) hoặc tự động cuộn độc lập khi số lượng menu vượt quá chiều cao màn hình.

---

### 2. Bộ Lọc Tìm Kiếm trên các Trang (Responsive Filter Bars)
**Objective:** Là một người dùng truy vấn báo cáo, tôi muốn bộ lọc tìm kiếm trên các màn hình Cảnh báo, Batches, Báo cáo và Cài đặt tự động dàn hàng hoặc chuyển thành dạng cột khi màn hình thu nhỏ, để giao diện không bị tràn viền và dễ thao tác trên màn hình cảm ứng.

#### Acceptance Criteria
1. `While` chiều rộng màn hình lớn hơn `1200px`, `the filter bar shall` xếp các ô nhập liệu (ngày tháng, dropdown chọn mẻ, chọn máy) thẳng hàng ngang.
2. `While` chiều rộng màn hình nằm trong khoảng `768px` đến `1200px`, `the filter bar shall` cho phép các ô nhập liệu tự động quấn hàng (wrap) thành 2 dòng và căn chỉnh đều khoảng cách.
3. `While` chiều rộng màn hình nhỏ hơn `768px` (thiết bị di động), `the filter bar shall` chuyển đổi tất cả các ô nhập liệu thành dạng cột dọc (`flex-direction: column`) với chiều rộng `100%`.
4. `While` chiều rộng màn hình nhỏ hơn `768px`, `the filter actions` (nút Tìm kiếm, Xuất Excel, Xuất CSV) `shall` hiển thị dàn đều hoặc căn giữa ở dòng dưới cùng.

---

### 3. Bảng Dữ Liệu và Biểu Đồ (Responsive DataTables & Charts)
**Objective:** Là một người phân tích dữ liệu, tôi muốn các bảng biểu hiển thị nhiều cột (đặc biệt là bảng Báo cáo 19 cột) và biểu đồ Highcharts tự động co giãn và hiển thị thanh cuộn ngang khi cần thiết, để không làm hỏng bố cục chung của trang web.

#### Acceptance Criteria
1. `When` bảng dữ liệu (như bảng Báo cáo hoạt động 19 cột hoặc bảng Cảnh báo) vượt quá chiều rộng màn hình hiển thị, `the system shall` bao bọc bảng trong một thẻ chứa có thanh cuộn ngang tự động (`overflow-x: auto`).
2. `The data tables shall` thu nhỏ khoảng cách padding của các ô (`td`, `th`) và kích thước font chữ tiêu đề cột xuống tối thiểu `11px` trên các màn hình có chiều rộng dưới `1200px`.
3. `While` màn hình hiển thị nhỏ hơn `768px`, `the charts` (biểu đồ đường Spline lịch sử, đồng hồ Gauge đo áp suất/nhiệt độ) `shall` tự động co dãn chiều rộng về `100%` theo thẻ chứa cha thay vì giữ kích thước cố định.

---

### 4. Sơ Đồ Bồn Trộn và Quy Trình Trang Tổng Quan (Responsive Diagram & Timeline)
**Objective:** Là một người vận hành, tôi muốn sơ đồ bồn trộn và thanh tiến trình công đoạn (Timeline) trên trang Tổng quan tự động thay đổi cách sắp xếp (layout) khi màn hình nhỏ lại, để các thông số cảm biến không bị che khuất hoặc đè lên nhau.

#### Acceptance Criteria
1. `While` chiều rộng màn hình nhỏ hơn `1200px`, `the process timeline shall` hiển thị dưới dạng danh sách các bước xếp dọc (vertical timeline) thay vì dàn hàng ngang (horizontal timeline).
2. `While` chiều rộng màn hình nhỏ hơn `1500px`, `the tank diagram shall` xếp chồng các cột thông số thiết bị phụ trợ (bơm, van) bên cạnh bồn trộn chính theo hàng dọc để tiết kiệm chiều ngang.
3. `While` màn hình hiển thị nhỏ hơn `768px`, `the tank diagram shall` thu nhỏ hình ảnh bồn trộn chính (`scale`) để vừa vặn với chiều ngang của màn hình điện thoại mà không bị vỡ bố cục hiển thị số liệu cảm biến trên bồn.
