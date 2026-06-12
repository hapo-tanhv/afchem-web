# Nhật ký Nghiên cứu & Quyết định Thiết kế

---

## Tóm tắt
- **Feature**: `flexible-batch-execution`
- **Discovery Scope**: Extension (Mở rộng tính năng điều khiển batch/run hiện có)
- **Key Findings**:
  - Giao diện Overview hiện tại sử dụng polling (`OverviewRealtime.js` gọi `GetCurrentBatchStats` mỗi 30 giây) để lấy thông tin mẻ chạy hiện tại. Do đó, sau khi cập nhật trạng thái mẻ chạy dưới cơ sở dữ liệu qua API mới, giao diện sẽ tự động cập nhật mà không cần tải lại trang.
  - Lớp `MySQLConnect` cung cấp phương thức `ExecuteNonQuery` để thực hiện cập nhật cơ sở dữ liệu và `ExecuteQuery` để truy vấn. Ta có thể sử dụng các hàm này trong controller action mới.
  - Bảng `runs` trong database có trường `execution_order` để kiểm soát thứ tự chạy. Khi một mẻ chạy được lựa chọn để thực hiện ngoài luồng, ta cần truy vấn giá trị `MAX(execution_order)` hiện tại và cập nhật mẻ chạy được chọn thành `MAX(execution_order) + 1` để đưa nó lên đầu hàng đợi chạy tiếp theo.
  - Cần vô hiệu hóa (disable) bộ chọn mẻ chạy trên UI Overview khi có bất kỳ mẻ nào đang ở trạng thái `Active` (tránh xung đột chạy đồng thời).

---

## Research Log

### Giao thức tương tác database và helper BatchResolver
- **Context**: Làm thế nào hệ thống nhận diện mẻ nào đang chạy và cách thay đổi nó mà không phá vỡ logic cũ?
- **Sources Consulted**: `BatchResolver.cs`, `OverviewController.cs`.
- **Findings**:
  - `BatchResolver.Resolve` giải quyết mẻ chạy dựa trên các tham số truyền vào, hoặc nếu không có tham số, nó sẽ lấy mẻ có trạng thái `Active` trước.
  - Trạng thái các mẻ chạy và batch được lưu trữ trực tiếp trong MySQL bảng `runs` và `batches`.
- **Implications**:
  - Để đổi mẻ chạy, ta chỉ cần thay đổi trạng thái của mẻ mong muốn thành `Active` (và các mẻ khác thành `Pending`), đồng thời đặt trạng thái batch chứa mẻ đó thành `Active` (và các batch khác thành `Pending`). `BatchResolver.Resolve` sẽ tự động nhận diện mẻ chạy mới được chọn này là mẻ hoạt động.

### Quản lý thứ tự chạy với trường execution_order
- **Context**: Làm thế nào để mẻ chạy được chọn chạy trước các mẻ khác trong hàng đợi của PLC/SCADA?
- **Sources Consulted**: Quy tắc nghiệp vụ từ khách hàng.
- **Findings**:
  - Trường `execution_order` quyết định thứ tự xử lý của các mẻ chạy.
- **Implications**:
  - Khi người dùng chọn bắt đầu một mẻ chạy, ta cập nhật `execution_order = (SELECT MAX(execution_order) FROM runs) + 1` (hoặc tính toán giá trị này trong C# để tránh lỗi cú pháp MySQL khi tự tham chiếu trong câu lệnh UPDATE). Việc này đảm bảo mẻ chạy được chọn sẽ có số thứ tự thực thi lớn nhất và được chạy ngay lập tức.

---

## Kiến trúc & Quyết định Thiết kế

### Quyết định: Thiết lập API đổi mẻ chạy tại OverviewController
- **Context**: Cần có một endpoint để UI Overview gửi yêu cầu đổi mẻ chạy.
- **Selected Approach**: Tạo phương thức `[HttpPost] public JsonResult SelectBatchRun(int batchId, int runId)` trong `OverviewController.cs`.
- **Rationale**: Trang Overview là nơi hiển thị chính của SCADA và người dùng thao tác chọn mẻ chạy tại đây, đặt API này ở `OverviewController` giúp gom cụm logic hiển thị và điều khiển.
- **Trade-offs**: Cần thêm quyền kiểm tra session đăng nhập để đảm bảo an toàn.

---

## Rủi ro & Giải pháp giảm thiểu
- **Xung đột chạy đồng thời (Concurrency):** Hai người vận hành mở hai trình duyệt khác nhau và cùng nhấn "Bắt đầu mẻ" cùng lúc.
  - *Giải pháp:* Trước khi thực hiện bất kỳ lệnh cập nhật nào ở Backend, hệ thống sẽ thực hiện truy vấn kiểm tra xem có mẻ nào đang `Active` hay không. Nếu có, lập tức trả về lỗi và từ chối cập nhật.
- **Sai lệch thứ tự chạy:** Cập nhật `execution_order` không chính xác dẫn đến PLC đọc sai mẻ.
  - *Giải pháp:* Truy vấn giá trị `MAX(execution_order)` lớn nhất một cách tường minh từ C#, cộng thêm 1 rồi mới thực hiện update.
