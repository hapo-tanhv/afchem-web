# Tài liệu sửa lỗi: Bộ lọc ngày (Date Filter) trên trang Batches (Event)

Tài liệu này ghi lại các thay đổi kỹ thuật để khắc phục sự cố bộ lọc chọn ngày không hoạt động trên trang Batches (Event).

---

## 1. Nguyên nhân lỗi (Root Cause)

Trước khi sửa đổi:
- **Frontend (`EventPage.js` & `Event.cshtml`):**
  - Hàm `loadBatches()` gọi đến endpoint `/Event/GetBatches` nhưng không truyền bất cứ tham số ngày nào, dẫn đến việc dropdown hiển thị tất cả các mẻ từ trước đến nay mà không lọc theo ngày đã chọn.
  - Khi người dùng thay đổi ngày trên datepicker, hệ thống không kích hoạt bất kỳ sự kiện nào để tải lại danh sách mẻ và dữ liệu sự kiện tương ứng.
  - Hàm `loadEventData()` chỉ truyền tham số `batchId` cho API `/Event/GetEventLogRealtime` mà không gửi kèm ngày được lọc.
  - Các hàm xuất báo cáo (`ExportExcel` & `ExportCSV`) chỉ gửi `batchId` mà không gửi ngày được chọn.
- **Backend (`EventController.cs`):**
  - Endpoint `GetBatches()` không chấp nhận tham số date và luôn truy vấn toàn bộ bảng `batches`.
  - Endpoint `GetEventLogRealtime()` và các phương thức xuất Excel/CSV khi không tìm thấy `batchId` hợp lệ thì mặc định lấy mẻ mới nhất toàn hệ thống thay vì mẻ mới nhất của ngày đang lọc.

---

## 2. Các thay đổi đã thực hiện (Implementation)

### A. Cập nhật Backend (`EventController.cs`)

1. **Hỗ trợ lọc mẻ theo ngày trong `GetBatches(string date)`**:
   ```csharp
   [HttpGet]
   public JsonResult GetBatches(string date)
   {
       ...
       string query = "SELECT id, name, status FROM batches ORDER BY id DESC";
       if (!string.IsNullOrEmpty(date))
       {
           if (DateTime.TryParseExact(date, new[] { "yyyy-MM-dd", "yyyy/MM/dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
           {
               string formattedDate = parsedDate.ToString("yyyy-MM-dd");
               query = $"SELECT id, name, status FROM batches WHERE DATE(start_time) = '{formattedDate}' ORDER BY id DESC";
           }
       }
       ...
   }
   ```

2. **Hỗ trợ phân giải mẻ mặc định theo ngày trong `GetEventLogRealtime(string batchId, string date)`**:
   Nếu người dùng chưa chọn mẻ (hoặc không có mẻ nào trong ngày), hệ thống tự động tìm và tải mẻ mới nhất của ngày đã lọc:
   ```csharp
   [HttpGet]
   public JsonResult GetEventLogRealtime(string batchId, string date)
   {
       ...
       if (selectedBatchId <= 0)
       {
           string dateFilter = "";
           if (!string.IsNullOrEmpty(date) && DateTime.TryParseExact(date, new[] { "yyyy-MM-dd", "yyyy/MM/dd" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
           {
               dateFilter = $"WHERE DATE(start_time) = '{parsedDate.ToString("yyyy-MM-dd")}'";
           }

           if (!string.IsNullOrEmpty(dateFilter))
           {
               var dtLatestDate = connector.ExecuteQuery($"SELECT id FROM batches {dateFilter} ORDER BY id DESC LIMIT 1");
               if (dtLatestDate != null && dtLatestDate.Rows.Count > 0)
               {
                   selectedBatchId = Convert.ToInt32(dtLatestDate.Rows[0]["id"]);
               }
           }
           ...
       }
       ...
   }
   ```

3. **Cập nhật tương tự cho `ExportEventExcel` và `ExportEventCsv`** để đảm bảo tệp xuất dữ liệu khớp hoàn toàn với ngày đang lọc trên giao diện.

---

### B. Cập nhật Frontend

1. **JavaScript (`EventPage.js`)**:
   - Cập nhật hàm `loadBatches(date, callback)` để nhận thêm tham số `date` và truyền tham số này qua Ajax:
     ```javascript
     function loadBatches(date, callback) {
         var params = {};
         if (date) { params.date = date; }
         $.ajax({
             url: '/Event/GetBatches',
             type: 'GET',
             data: params,
             ...
         });
     }
     ```
   - Cập nhật `loadEventData()` để tự động đọc ngày từ `#starttime`, định dạng thành `YYYY-MM-DD` và truyền cho API `/Event/GetEventLogRealtime`.
   - Cập nhật hàm `init()` đọc ngày hiện tại để khởi tạo chính xác các mẻ thuộc ngày hôm nay.

2. **Giao diện Razor View (`Event.cshtml`)**:
   - Gắn sự kiện `apply.daterangepicker` vào datepicker để tải lại danh sách mẻ và dữ liệu sự kiện ngay khi người dùng đổi ngày:
     ```javascript
     $('.datepicker').daterangepicker({
         ...
     }).on('apply.daterangepicker', function (ev, picker) {
         var formattedDate = picker.startDate.format('YYYY-MM-DD');
         EventPage.loadBatches(formattedDate, function () {
             EventPage.loadEventData();
         });
     });
     ```
   - Sửa hàm `ExportExcel()` và `ExportCSV()` để gửi kèm ngày đang chọn qua tham số `starttime`.

---

## 3. Kết quả đạt được

- **Đồng bộ hóa 100%:** Khi chọn một ngày bất kỳ trên datepicker, danh sách chọn mẻ (Batch select) sẽ ngay lập tức được tải lại chỉ hiển thị các mẻ của ngày đó. Giao diện cũng tự động tải dữ liệu của mẻ đầu tiên trong ngày đó.
- **Trải nghiệm mượt mà:** Người dùng không cần phải click "Tìm kiếm" thủ công sau khi đổi ngày, dữ liệu sẽ tự động làm mới ngay lập tức.
- **Báo cáo chính xác:** Việc xuất Excel/CSV giờ đây đồng bộ hoàn toàn với ngày đang lọc, không còn bị lỗi xuất lệch mẻ hoặc sai dữ liệu.

---

## 4. Khắc phục lỗi hiển thị Tiếng Việt (Lỗi Font)

Để xử lý triệt để tình trạng lỗi font tiếng Việt hiển thị trên giao diện hoặc lỗi giải mã ký tự có dấu khi lấy từ cơ sở dữ liệu:

1. **Bổ sung `CharSet=utf8;` vào các ConnectionString (`EventController.cs`):**
   Tất cả các kết nối MySQL trong Controller đã được thêm thuộc tính mã hóa ký tự UTF-8, giúp dữ liệu chữ tiếng Việt có dấu (như tên mẻ, nội dung log sự kiện, cảnh báo) từ database MySQL được hiển thị chính xác hoàn toàn:
   ```csharp
   ConnectionString = "Server=localhost;Database=scada;Uid=root;Pwd=101101;CharSet=utf8;"
   ```

2. **Chống Cache trình duyệt với Cache Buster (`Event.cshtml`):**
   Thêm tham số phiên bản động `?v=@DateTime.Now.Ticks` vào thẻ gọi tệp JavaScript `EventPage.js` để bắt buộc trình duyệt của khách hàng luôn tải tệp tin JavaScript mới nhất, tránh tình trạng lưu trữ cache phiên bản lỗi font cũ:
   ```html
   <script src="~/JavaScript/Event/EventPage.js?v=@DateTime.Now.Ticks"></script>
   ```

3. **Mã hóa file nguồn:**
   Đảm bảo tất cả 3 file chỉnh sửa (`EventController.cs`, `Event.cshtml`, `EventPage.js`) được lưu ở định dạng chuẩn **UTF-8 with BOM** (mã hóa tiếng Việt tối ưu cho môi trường ASP.NET MVC/IIS trên Windows).

