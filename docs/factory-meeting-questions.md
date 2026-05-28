# Bộ Câu Hỏi Khảo Sát & Tối Ưu Hóa Hệ Thống SCADA AFCHEM
*Tài liệu chuẩn bị cho buổi họp làm việc trực tiếp với Ban Giám đốc và Đội kỹ thuật Nhà máy*

---

## 1. PHÂN BỔ SẢN LƯỢNG & HIỆU SUẤT MẺ (PRODUCTION YIELD & BATCH ANALYTICS)

*   **Câu hỏi 1: Quy tắc lũy kế sản lượng thực tế từng công đoạn**
    *   *Bối cảnh:* Hiện tại một mẻ hoàn thành tiêu chuẩn có sản lượng là 500kg. 
    *   *Câu hỏi:* Trên giao diện Header, sản lượng hiện tại nên được **cộng dồn tăng dần (lũy kế)** sau khi kết thúc mỗi công đoạn chất lượng đạt hay không? Nếu có, tỷ lệ phân bổ sản lượng cho từng công đoạn như thế nào? 
    *   *Ví dụ đề xuất:* 
        *   Hoàn thành Cấp liệu -> tăng lên 100kg.
        *   Hoàn thành Trộn 1 -> tăng lên 200kg...
        *   Hoàn thành bước cuối "Rung xả hàng" -> đạt đủ 500kg.
        *   *Hay là:* Chỉ khi kết thúc hoàn toàn cả mẻ (bước 8) thì hệ thống mới cộng dồn 500kg vào sản lượng thực tế?
*   **Câu hỏi 2: Quản lý hao hụt nguyên vật liệu (BOM Yield Deviation)**
    *   *Câu hỏi:* Hệ thống có cần tính toán và cảnh báo sự sai lệch giữa **Tổng khối lượng nguyên vật liệu thực tế đã xuất** (từ bảng BOM nguyên liệu đầu vào) so với **Khối lượng sản phẩm thực tế thu được** ở đầu ra hay không? Tỷ lệ hao hụt cho phép tối đa là bao nhiêu % (ví dụ: <1%) trước khi kích hoạt cảnh báo chất lượng?

---

## 2. QUẢN LÝ CA LÀM VIỆC & BÀN GIAO CA (SHIFT MANAGEMENT & HANDOVER)

*   **Câu hỏi 3: Cấu hình ca làm việc linh hoạt**
    *   *Bối cảnh:* Hiện tại hệ thống đang chia cứng làm 3 ca làm việc (Ca 1: 08:00-12:00, Ca 2: 13:00-17:00, Ca 3: 18:00-22:00).
    *   *Câu hỏi:* Khung giờ làm việc thực tế của nhà máy có thay đổi theo mùa hoặc có phát sinh tăng ca (Overtime) hay ca gãy không? Ban quản lý nhà máy có cần một giao diện phân quyền (Admin) để tự cấu hình linh hoạt thời gian bắt đầu/kết thúc các ca thay vì khóa cứng trong code không?
*   **Câu hỏi 4: Xử lý mẻ sản xuất chuyển giao giữa các ca**
    *   *Câu hỏi:* Trong trường hợp một mẻ sản xuất bắt đầu chạy ở cuối Ca 1 nhưng kéo dài sang giờ của Ca 2 mới hoàn thành:
        *   Sản lượng và thời gian chạy của mẻ này sẽ tính cho ca bắt đầu (Ca 1) hay ca kết thúc (Ca 2)?
        *   Hay hệ thống cần tự động chia tỷ lệ (ví dụ mẻ chạy 60 phút, Ca 1 chạy 40 phút tính 67%, Ca 2 chạy 20 phút tính 33%)?

---

## 3. QUY TRÌNH CÔNG ĐOẠN & CÁC TRƯỜNG HỢP NGOẠI LỆ (STEP SEQUENCE & EDGE CASES)

*   **Câu hỏi 5: Vận hành độc lập không qua SCADA Web (Vệ sinh / Bảo trì)**
    *   *Bối cảnh:* Trường hợp máy trộn vật lý hoạt động (chạy thử máy, vệ sinh bồn trộn, bảo trì động cơ hoặc người vận hành bấm chạy tay trực tiếp trên tủ PLC) nhưng hệ thống SCADA Web không kích hoạt nút bắt đầu mẻ.
    *   *Câu hỏi:* Hệ thống SCADA có nên bỏ qua các dữ liệu này không ghi vào lịch sử mẻ lỗi hay không? 
    *   *Đề xuất:* Chúng ta nên phân loại trạng thái máy lúc này là **"MAINTENANCE/CLEANING" (Bảo trì/Vệ sinh)** thay vì ghi nhận một mẻ lỗi bị thiếu bước, để không làm ảnh hưởng đến báo cáo hiệu suất (OEE) chung của nhà máy.
*   **Câu hỏi 6: Xử lý khi người vận hành nhảy cóc/bỏ qua bước (Step Skipping)**
    *   *Câu hỏi:* Có trường hợp nào người vận hành vận hành thủ công (Manual) trên tủ điện PLC quyết định bỏ qua một bước (ví dụ: bỏ qua bước 4 "Rung xả đáy" để chuyển thẳng sang bước 5 "Hút xả đáy") không? 
    *   *Kịch bản xử lý:* Khi đó, giao diện SCADA Web nên hiển thị công đoạn bị bỏ qua này như thế nào? (Đánh dấu màu xám ghi nhận "Bỏ qua", coi như "Hoàn thành" với thời gian chạy = 0s, hay vẫn giữ trạng thái "Chưa chạy"?)
*   **Câu hỏi 7: Lặp lại công đoạn do sự cố (Step Loop / Reprocessing)**
    *   *Câu hỏi:* Nếu một công đoạn trộn chưa đạt chuẩn chất lượng (ví dụ nhiệt độ trộn 1 chưa đủ) và người vận hành phải chạy lại công đoạn đó lần thứ 2. Hệ thống SCADA nên ghi nhận thời gian chạy như thế nào? (Cộng dồn thời gian của cả 2 lần chạy, chỉ lấy lần chạy cuối, hay tách ra thành lịch sử các lần chạy riêng biệt trong báo cáo?)

---

## 4. HỆ THỐNG CẢNH BÁO & AN TOÀN (ALARMS, WARNINGS & SAFETY)

*   **Câu hỏi 8: Hành động phản hồi khi có Cảnh báo mức độ nghiêm trọng (Critical Alarm)**
    *   *Bối cảnh:* Các thông số vượt chuẩn (Nhiệt độ bồn trộn > 35°C, Áp suất > 2.5 bar) đang được chia làm mức `WARNING` (Cảnh báo nhẹ) và `ALARM` (Báo động nghiêm trọng).
    *   *Câu hỏi:* Khi có sự cố mức `ALARM`, nhà máy có cần hệ thống SCADA thực hiện các hành động tự động nào không?
        *   Hú còi cảnh báo qua hệ thống loa của máy tính giám sát?
        *   Gửi tin nhắn thông báo khẩn cấp qua Telegram/Email/SMS đến Đội ngũ Kỹ thuật và Quản lý?
        *   Gửi tín hiệu Interlock dừng khẩn cấp máy trộn (nếu tích hợp sâu)?
*   **Câu hỏi 9: Cảnh báo quá giờ công đoạn (Step Timeout Alarm)**
    *   *Câu hỏi:* Nếu một công đoạn chạy vượt quá đáng kể so với thời gian tiêu chuẩn thiết kế (ví dụ bước Cấp liệu chuẩn là 720s nhưng thực tế chạy đến 1200s vẫn chưa xong do nghẽn ống hoặc bơm yếu). Hệ thống có cần tự động kích hoạt cảnh báo quá giờ công đoạn (`Step Timeout Warning`) để operator xử lý kịp thời hay không?

---

## 5. BÁO CÁO & TRUY XUẤT NGUỒN GỐC (REPORTING & TRACEABILITY)

*   **Câu hỏi 10: Xuất báo cáo dạng PDF lưu trữ chất lượng (Batch Report PDF)**
    *   *Câu hỏi:* Ngoài việc xuất báo cáo Excel/CSV để phân tích số liệu như hiện tại, nhà máy có cần hệ thống tự động xuất ra file **Báo cáo mẻ dạng PDF (Batch Report PDF)** có đầy đủ biểu đồ nhiệt độ/áp suất thực tế và biểu mẫu ký tên bàn giao của Trưởng ca/QC để phục vụ lưu trữ hồ sơ chất lượng sản xuất cứng hay không?
*   **Câu hỏi 11: Truy xuất nguồn gốc LOT nguyên vật liệu thực tế**
    *   *Bối cảnh:* Hiện tại bảng BOM hiển thị các mã LOT NVL cố định.
    *   *Câu hỏi:* Nhà máy có yêu cầu người vận hành quét mã vạch (Barcode/QR) hoặc nhập tay mã LOT nguyên vật liệu thực tế khi bắt đầu nạp liệu cho mẻ đó hay không để lưu trữ dữ liệu phục vụ truy xuất nguồn gốc sản phẩm khi có khiếu nại khách hàng?
