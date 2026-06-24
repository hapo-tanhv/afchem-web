# Nghiên cứu & Quyết định Thiết kế (Research & Design Decisions) - Biểu đồ Nhiệt độ trang Báo cáo

## Summary
- **Feature**: report-temp-chart
- **Discovery Scope**: Extension (Mở rộng hệ thống hiện tại)
- **Key Findings (Phát hiện chính)**:
  - Dự án hiện tại đã nạp thư viện **Highcharts** ở trang Tổng quan qua các link CDN (Highcharts 9+). Chúng ta sẽ tái sử dụng các link CDN này trong trang `Report.cshtml` để vẽ biểu đồ line chart mà không cần cài đặt thêm thư viện mới.
  - Số liệu cho báo cáo được lấy từ bảng `alarmreport`. Cần viết một API mới ở `HomeController.cs` (ví dụ: `GetReportChartData`) nhận các tham số bộ lọc giống hệt bảng DataTable hiện tại để trả về danh sách điểm đo lịch sử nhiệt độ.
  - Để tránh quá tải tài nguyên trình duyệt khi người dùng lọc khoảng thời gian quá rộng (hàng chục nghìn bản ghi), chúng ta sẽ cài đặt thuật toán lấy mẫu (downsampling) ở phía Server (C#) để giới hạn tối đa khoảng 1000 điểm đo hiển thị đều đặn trên biểu đồ.

---

## Nhật ký Nghiên cứu (Research Log)

### Tích hợp Highcharts trong AdminLTE Card
- **Bối cảnh**: Cần đặt biểu đồ phía trên DataTable, hỗ trợ Thu gọn/Mở rộng.
- **Tài liệu tham khảo**: Codebase hiện tại của AdminLTE 3 và các cấu trúc Card Bootstrap.
- **Phát hiện**: 
  - Khung Card của AdminLTE 3 hỗ trợ các nút widget mặc định: `<button type="button" class="btn btn-tool" data-card-widget="collapse"><i class="fas fa-minus"></i></button>` giúp thu gọn/mở rộng card body vô cùng mượt mà bằng CSS/JS tích hợp sẵn của AdminLTE.
  - Thiết lập vùng chứa biểu đồ `<div id="reportChartContainer" style="width:100%; height:400px;"></div>`.
- **Hệ quả**: Card sẽ tự động hỗ trợ tính năng Collapse/Expand mà không cần viết thêm Javascript tùy chỉnh phức tạp cho việc ẩn hiện.

### Lấy mẫu tối ưu hóa hiệu năng (Data Downsampling)
- **Bối cảnh**: Khi tra cứu cả tháng, số lượng bản ghi trong `alarmreport` có thể lên tới 50,000 dòng. Vẽ 50,000 điểm của 5 đường (tổng cộng 250,000 điểm) sẽ làm treo trình duyệt Client.
- **Giải pháp**:
  - Khi API `GetReportChartData` truy vấn danh sách bản ghi đầy đủ (ví dụ: `N` bản ghi).
  - Nếu `N > 1000`, chúng ta tính toán bước nhảy `step = N / 1000` (làm tròn lên).
  - Duyệt qua danh sách và chỉ lấy các bản ghi ở các vị trí `i` chia hết cho `step`.
  - Cách làm này đảm bảo số lượng điểm vẽ lên đồ thị luôn `<= 1000`, giữ cho đồ thị phân bổ đều theo thời gian thực tế và render cực kỳ nhanh (<50ms).
- **Hệ quả**: Đảm bảo hiệu năng hệ thống tối đa ở cả Client và Backend.

---

## Đánh giá Phương án Kiến trúc (Architecture Pattern Evaluation)

| Phương án | Mô tả | Ưu điểm | Nhược điểm / Hạn chế | Ghi chú |
| :--- | :--- | :--- | :--- | :--- |
| **Client-side Downsampling** | Tải toàn bộ dữ liệu thô về JS Client rồi lọc lại trước khi vẽ. | Backend đơn giản, không cần xử lý thuật toán lọc dữ liệu. | Tải file JSON dung lượng lớn qua mạng (gây chậm băng thông mạng và đơ Client). | Không khuyến khích cho các mạng kết nối chậm. |
| **Server-side Downsampling** | Lọc giảm điểm đo trực tiếp trên Backend C# trước khi trả về JSON. | Trả về JSON dung lượng cực kỳ nhỏ (~50KB), Client render siêu nhanh, tải trang tức thì. | Cần thêm code thuật toán lọc trên C# (tuy nhiên độ phức tạp thuật toán cực nhỏ `O(N)`). | **Lựa chọn tối ưu nhất (Selected)** |

---

## Quyết định Thiết kế (Design Decisions)

### Quyết định 1: Tạo Endpoint API chuyên biệt cho Biểu đồ (`GetReportChartData`)
- **Bối cảnh**: DataTable sử dụng cơ chế Server-side Paging (`LIMIT / OFFSET`) để hiển thị dữ liệu phân trang (10 bản ghi một trang). Biểu đồ Line Chart thì cần toàn bộ dữ liệu của mẻ/khoảng thời gian đó chứ không thể vẽ theo từng trang 10 dòng. Do đó không thể dùng chung kết quả JSON của DataTable.
- **Phương án lựa chọn**: Viết thêm phương thức `GetReportChartData` trả về dữ liệu mảng thô (đã lọc lấy mẫu) chứa các cột: `TimeStr` (hoặc `DateTime`), `ApSuat`, `NhietDoMT`, `NhietNapBon`, `NhietGiuaBon`, `NhietDayBon`.
- **Lý do**: Tách biệt rõ ràng luồng dữ liệu phân trang của bảng lưới và luồng dữ liệu biểu đồ đồ thị.

### Quyết định 2: Tải lại biểu đồ dựa trên sự kiện vẽ bảng DataTable (`draw` event)
- **Bối cảnh**: Khi người dùng nhấn nút Tìm kiếm hoặc đổi trang bộ lọc, DataTable sẽ tự động gọi ajax và kích hoạt sự kiện.
- **Phương án lựa chọn**: Lắng nghe sự kiện vẽ bảng hoặc tích hợp lệnh gọi tải dữ liệu biểu đồ ngay trong hàm `initDataTable` khi có phản hồi dữ liệu từ Server. Để đơn giản và tối ưu, chúng ta sẽ gọi hàm `loadChartData()` ngay sau khi khởi tạo hoặc tải lại bộ lọc DataTable thành công.

---

## Rủi ro & Giải pháp Khắc phục (Risks & Mitigations)
- **Rủi ro: Trùng thời gian biểu thị** — Trục X biểu diễn thời gian thực tế của mẻ. Nếu có nhiều bản ghi có cùng giây (trùng timestamp), Highcharts có thể hiển thị các điểm chồng chéo.
  - *Khắc phục:* Trong SQL query, ta sắp xếp `ORDER BY a.DateTime ASC, a.ID ASC`. Trên client, Highcharts sẽ vẽ các điểm tuần tự đúng theo thứ tự thời gian tăng dần.
- **Rủi ro: Không có dữ liệu** — Trình duyệt bị lỗi trống biểu đồ khi không tìm thấy bản ghi nào.
  - *Khắc phục:* Nếu mảng dữ liệu rỗng, Highcharts sẽ hiển thị thông báo "Không có dữ liệu hiển thị" thông qua thuộc tính `lang.noData` hoặc hiển thị thẻ div chứa thông báo trống.
