# Research & Design Decisions - Responsive Pages & Sidebar

## Summary
- **Feature**: responsive-pages
- **Discovery Scope**: Extension
- **Key Findings**:
  - CSS Dependency: Tất cả 5 trang chức năng chính đều nhập tệp `Overview.css`. Do đó, `Overview.css` là nơi lý tưởng để định nghĩa các thuộc tính responsive dùng chung cho bảng biểu, thẻ card và bộ lọc `.filter-bar`.
  - AdminLTE Sidebar: AdminLTE 3.x đi kèm với cơ chế đóng/mở sidebar được tích hợp sẵn thông qua thuộc tính `data-widget="pushmenu"` và CSS class `sidebar-collapse`. Dự án hiện tại đang thiếu nút toggle này trên Header, dẫn đến việc sidebar cố định 250px gây vỡ layout trên màn hình hẹp.
  - Bảng dữ liệu rộng: Bảng Báo cáo có tới 19 cột. Việc sử dụng lớp `.table-responsive` của Bootstrap/AdminLTE là bắt buộc để bảng có thể cuộn ngang độc lập, tránh bóp méo layout tổng thể của trang web.

---

## Research Log

### Tích hợp Sidebar Toggle của AdminLTE
- **Context**: Làm thế nào để đóng/mở sidebar trên di động và màn hình nhỏ một cách tự động và thủ công mà không cần viết thêm JS custom phức tạp?
- **Sources Consulted**: Tài liệu chính thức của AdminLTE 3.x về Sidebar và PushMenu plugin.
- **Findings**:
  - Một phần tử HTML (thẻ `a` hoặc `button`) có thuộc tính `data-widget="pushmenu"` sẽ được file JS của AdminLTE (`adminlte.js`) tự động bắt sự kiện click và toggle class `sidebar-collapse` trên thẻ `body`.
  - Trên màn hình máy tính để bàn (>992px), class `sidebar-collapse` thu gọn sidebar thành dạng mini (chỉ hiển thị icon).
  - Trên màn hình máy tính bảng/di động (<992px), class này ẩn hoàn toàn sidebar và hiển thị lại bằng hiệu ứng trượt ra (slide-out) khi người dùng bấm lại nút toggle.
- **Implications**: Chúng ta chỉ cần thêm nút nhấn có thuộc tính `data-widget="pushmenu"` vào Header trong `_LayoutMain.cshtml` và tinh chỉnh CSS của Header để chứa nút này mà không bị xô lệch vị trí.

---

## Architecture Pattern Evaluation

| Option | Description | Strengths | Risks / Limitations | Notes |
|--------|-------------|-----------|---------------------|-------|
| CSS media-queries | Sử dụng `@media` queries trực tiếp trong CSS hiện có | Trực quan, hiệu năng cao, dễ tích hợp | CSS có thể phình to nếu không kiểm soát | Lựa chọn chính thức |
| Flexbox & Grid | Dùng CSS Grid/Flexbox tự co giãn (`flex-wrap`, `grid-template-columns: repeat(auto-fit, ...)`) | Tự động thích ứng tốt mà ít cần viết media queries | Khó kiểm soát chính xác điểm nhảy hàng trên các thiết bị cụ thể | Kết hợp cùng với media-queries |

---

## Design Decisions

### Decision: Cấu trúc CSS Responsive
- **Context**: Nên viết mã CSS responsive ở một tệp CSS mới hay tích hợp vào các tệp hiện có?
- **Alternatives Considered**:
  1. Tạo tệp `Responsive.css` mới và nhập vào `_LayoutMain.cshtml`.
  2. Tích hợp trực tiếp vào `_LayoutMain.cshtml` (cho phần khung) và `Overview.css` (cho phần thân).
- **Selected Approach**: Lựa chọn 2.
- **Rationale**: Do các trang đều đã kế thừa `Overview.css` và layout tổng thể nằm trong `_LayoutMain.cshtml`, việc cập nhật trực tiếp vào các file này giúp tránh tạo thêm các request HTTP phụ để tải file CSS mới, tối ưu hóa tốc độ tải trang và giữ cấu trúc thư mục gọn gàng.
- **Trade-offs**: Cần cẩn thận khi chỉnh sửa `Overview.css` để không ảnh hưởng đến các thành phần layout khác.
- **Follow-up**: Đảm bảo thực hiện build và kiểm tra giao diện trên cả 5 trang.

---

## Risks & Mitigations
- **Retina Displays & Safari compatibility**: Màn hình Retina của máy Mac có thể hiểu sai một số tỉ lệ phần trăm pixel.
  - *Biện pháp giảm thiểu*: Sử dụng các đơn vị tương đối như `rem`, `em`, và đặt các mốc `@media` rộng rãi để bao phủ cả độ rộng vật lý lẫn độ rộng logic.

---

## References
- [AdminLTE 3.x PushMenu Documentation](https://adminlte.io/docs/3.2/javascript/pushmenu.html) — Hướng dẫn cấu hình đóng mở menu.
