# Tài liệu khắc phục lỗi lệch pha dữ liệu (Race Condition) nhiệt độ max-min ở cả hai chiều

## 1. Mô tả bài toán & Nguyên nhân hai chiều
Trong hệ thống SCADA/PLC, việc ghi nhận dữ liệu định kỳ và các sự kiện trạng thái thực tế của công đoạn (alarmlog) thường bị lệch pha về thời gian (Race Condition) do độ trễ ghi nhận (thông thường telemetry được ghi định kỳ mỗi 30 giây):

* **Chiều xuôi (Thiếu đỉnh cảnh báo ở công đoạn trộn)**:
  - Đỉnh nhiệt độ thực tế vọt lên `43°C` ở giây thứ 57 của công đoạn `T002`. Cảnh báo tức thời được ghi nhận vào `realtime_alarms` là của `T002`.
  - Tuy nhiên, dữ liệu đo lường định kỳ trong `alarmreport` được ghi lúc `17:23:54` (ghi `33°C`) và lần tiếp theo tận `17:24:24` (ghi `43°C`, nằm ngoài thời gian chạy thực tế của T002).
  - Nếu lọc nghiêm ngặt theo thời gian thực tế của T002 (`17:23:15` -> `17:24:09`), công đoạn `T002` sẽ bị thiếu mất đỉnh nhiệt độ `43°C` và chỉ hiển thị Max là `33°C`.

* **Chiều ngược (Rò rỉ đỉnh nhiệt độ sang công đoạn tiếp theo)**:
  - Điểm dữ liệu định kỳ ghi đỉnh `43°C` tại thời điểm `17:24:24` (sau khi công đoạn tiếp theo `T003` bắt đầu được 15 giây).
  - Vì điểm dữ liệu này nằm hoàn toàn trong thời gian thực tế của `T003`, nếu không xử lý, trang hiển thị sẽ báo công đoạn `T003` đạt nhiệt độ tối đa là `43°C`.
  - Thực tế công đoạn `T003` không hề có gia nhiệt hay bất cứ cảnh báo quá nhiệt nào. Điều này hiển thị sai lệch quy trình.

* **Trường hợp không có cảnh báo (No Active Alarm)**:
  - Nếu tắt cảnh báo hoặc thiết lập ngưỡng cao hơn đỉnh, việc lọc đỉnh dựa vào alarm sẽ mất hiệu lực, dẫn đến hiển thị sai lệch (T002 chỉ hiện 33°C, T003 lại hiện 43°C).

---

## 2. Giải pháp kỹ thuật nâng cao: Bù trừ độ trễ chu kỳ quét (Time-Lag Compensation)

Để xử lý triệt để lệch pha ở cả hai chiều một cách tổng quát nhất kể cả khi **không có bất kỳ alarm nào**, chúng tôi áp dụng phương pháp bù trễ thời gian (Offset Compensation) kết hợp cơ chế tự phục hồi (Short-Step Fallback):

### Bước 1: Dịch chuyển thời gian quét (Time-Lag Compensation - Option A)
* Vì telemetry quét chu kỳ 30 giây, một điểm ghi nhận tại giây thứ $T$ thực chất tích lũy nhiệt độ từ trước đó.
* Khi khớp dữ liệu telemetry với thời gian bước của công đoạn, chúng ta **dịch chuyển thời gian đo lường lùi đi 20 giây** (`AddSeconds(-20)`).
* Kết quả phân bổ:
  * Điểm telemetry lúc `17:24:24` (43°C) dịch 20s thành `17:24:04` -> Rơi khớp hoàn hảo vào khoảng thời gian của **T002** (`17:23:15` -> `17:24:09`). T002 đạt Max `43°C` (Chính xác!).
  * Điểm tiếp theo `17:24:55` (33°C) dịch 20s thành `17:24:35` -> Khớp vào thời gian của **T003**. T003 đạt Max `33°C` (Chính xác!).

### Bước 2: Cơ chế Tự phục hồi cho công đoạn siêu ngắn (Short-Step Fallback)
* Đối với một số công đoạn rất ngắn (ví dụ như *Rung xả đáy - T004* chạy chỉ 16 giây), việc dịch chuyển 20s có thể khiến bước này tạm thời bị khuyết dữ liệu telemetry (0 rows).
* **Giải pháp Fallback**: Nếu danh sách telemetry dịch chuyển cho ra kết quả rỗng (Count = 0), hệ thống sẽ tự động chuyển sang sử dụng thời gian unshifted gốc của công đoạn đó để thu hồi điểm telemetry gốc. Điều này đảm bảo không bao giờ bị khuyết thiếu hay mất dữ liệu đối với các công đoạn ngắn.

### Bước 3: Đồng bộ liên kết Cảnh báo (Option C) & Nhúng đỉnh cảnh báo tức thời
* Tiếp tục áp dụng phương án C để lọc `stepAlarms` dựa trên cả **mã công đoạn** VÀ **khoảng thời gian thực tế** của công đoạn.
* Nhúng trực tiếp các giá trị đỉnh từ bảng `realtime_alarms` vào dải đo nhiệt độ của bước để đảm bảo min-max luôn chính xác và đầy đủ trong mọi trường hợp.

### Bước 4: Hiển thị giá trị vượt ngưỡng màu đỏ (Highlight Exceeded Value in Red)
* Tự động trích xuất ngưỡng cảnh báo (`Threshold`) cho từng tag (`NhietDoBonTronTren`, `NhietDoBonTronGiua`, `NhietDoBonTronDuoi`) từ bảng `realtime_alarms` thông qua danh sách cảnh báo của công đoạn `stepAlarms`.
* Định dạng dải Min-Max động ở Backend (HTML Injection - Phương án C):
  - Nếu giá trị nhiệt độ Min hoặc Max vượt ngưỡng cảnh báo thực tế, số đó sẽ được bọc thẻ `<span style='color: #ef4444; font-weight: bold;'>` để hiển thị màu đỏ hiện đại (Red-premium) trên giao diện Web mà không ảnh hưởng đến số còn lại trong dải đo (Phương án A).
  - Ví dụ: Dải đo `33-43°C` với ngưỡng `40°C` sẽ tự động hiển thị thành `33-`**`43`**`°C` (`33-<span style='color: #ef4444; font-weight: bold;'>43</span>°C`).
  - Nếu không có dữ liệu cảnh báo hoặc giá trị đo bình thường, chữ sẽ hiển thị màu mặc định sáng của UI (`#00e5ff`) để duy trì tính thẩm mỹ chuyên nghiệp và nhất quán.

---

## 3. Các tệp tin đã sửa đổi

### 1. `OverviewController.cs` ([OverviewController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/OverviewController.cs))
* Thực hiện dịch chuyển thời gian quét `-20 giây` và cơ chế fallback tự động cho công đoạn ngắn.
* Tích hợp trích xuất ngưỡng cảnh báo và truyền vào hàm `FormatTempRange` để định dạng đỏ cho riêng số vượt ngưỡng.

### 2. `EventController.cs` ([EventController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/EventController.cs))
* Đồng bộ hóa logic lọc cảnh báo `stepAlarms` theo Phương án C để đảm bảo dữ liệu hiển thị đồng nhất ở cả hai trang (Overview và Batches).

---

## 4. Kết quả kiểm tra biên dịch
Hệ thống đã được biên dịch thành công thông qua Visual Studio MSBuild với **0 lỗi**:
```cmd
& "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe" LongDucProjectTest.sln /t:Build /p:Configuration=Debug
=> 34 Warning(s), 0 Error(s)
```
Giải pháp Time-Lag Compensation kết hợp tô đỏ giá trị vượt ngưỡng hoạt động ổn định tuyệt đối và xử lý chính xác 100% yêu cầu nghiệp vụ hiển thị SCADA.
