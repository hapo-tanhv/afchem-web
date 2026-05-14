# Phân tích cải tổ giao diện trang Overview

> So sánh code hiện tại (`Overview.cshtml`) với thiết kế mục tiêu (ảnh đính kèm)

---

## 1. TỔNG QUAN SỰ KHÁC BIỆT

### Giao diện HIỆN TẠI (code)
- Layout dọc, cuộn dài (scroll-heavy)
- 3 section riêng biệt xếp chồng: Cards môi trường → Cards thời gian → Bảng thống kê → Thông tin sản phẩm / BOM
- Không có sơ đồ thiết bị bồn trộn
- Không có thanh tiến trình quy trình (process steps)
- Không có phần "Điều kiện chuyển sang bước tiếp theo"
- Không có phần "Tổng quan mẻ" sidebar

### Giao diện MỤC TIÊU (ảnh)
- Layout dashboard đa panel, chia vùng rõ ràng trong 1 màn hình
- Header bar: Máy TX01, Mẻ hiện tại, Sản phẩm, Công đoạn, Trạng thái, Thời gian → (ĐÃ CÓ trong `_LayoutMain.cshtml`)
- Content area chia thành nhiều panel song song

---

## 2. PHÂN TÍCH CHI TIẾT TỪNG SECTION

### 2.1. TRẠNG THÁI QUY TRÌNH (Process Steps Timeline) — **MỚI HOÀN TOÀN**

**Hiện tại:** KHÔNG CÓ

**Mục tiêu:** Thanh timeline ngang hiển thị 9 bước quy trình:
1. Cấp liệu → 2. Hút xả đáy → 3. Trộn 1 → 4. Xả đáy → 5. Rung xả đáy → 6. **Trộn 2** (đang active) → 7. Xả hàng → 8. Rung xả hàng → 9. Hoàn thành

**Thông tin hiển thị kèm theo:**
| Trường | Giá trị mẫu |
|--------|-------------|
| Bước hiện tại | 6 / 9 |
| Thời gian chuẩn | 1,200 s |
| Thời gian đã chạy | 780 s |
| Thời gian còn lại | 420 s |
| Tiến độ bước | 65% |

**Công việc:**
- [ ] Tạo HTML mới cho timeline ngang (process-steps)
- [ ] CSS cho timeline nodes, connecting lines, active/completed/pending states
- [ ] JS cập nhật realtime active step

---

### 2.2. THÔNG TIN BƯỚC HIỆN TẠI — **MỚI HOÀN TOÀN**

**Hiện tại:** KHÔNG CÓ (thông tin nằm rải rác trong các card)

**Mục tiêu:** Panel bên trái hiển thị chi tiết bước đang chạy:
| Trường | Giá trị |
|--------|---------|
| Tên bước | Trộn 2 |
| Trạng thái | RUNNING (xanh) |
| Thời gian chuẩn | 1,200 s |
| Thời gian đã chạy | 780 s |
| Thời gian còn lại | 420 s |
| Tiến độ bước | 65% (progress bar) |

**Công việc:**
- [ ] Tạo HTML cho panel "Thông tin bước hiện tại"
- [ ] Progress bar hiển thị % tiến độ
- [ ] CSS styling dark theme

---

### 2.3. SƠ ĐỒ THIẾT BỊ - BỒN TRỘN — **MỚI HOÀN TOÀN**

**Hiện tại:** KHÔNG CÓ

**Mục tiêu:** Panel trung tâm lớn hiển thị hình ảnh minh họa bồn trộn (mixer tank) với các sensor:
- Cảm biến nhiệt: 23.9 °C
- Áp suất trong bồn: 0.60 bar
- Cảm biến mực: 2.0 m (67%)

**Công việc:**
- [ ] Tạo/tích hợp hình ảnh SVG hoặc PNG bồn trộn
- [ ] Overlay các giá trị sensor lên hình ảnh (absolute positioning)
- [ ] CSS cho các label/value trên sơ đồ
- [ ] JS realtime update giá trị sensor

---

### 2.4. ĐIỀU KIỆN CHUYỂN SANG BƯỚC TIẾP THEO — **MỚI HOÀN TOÀN**

**Hiện tại:** KHÔNG CÓ

**Mục tiêu:** Không cần do thực tế không có điều kiện chuyển sang bước tiếp theo

### 2.5. THÔNG SỐ QUY TRÌNH HIỆN TẠI — **THAY ĐỔI BỐ CỤC**

**Hiện tại:** 6 card lớn (cards row) cho nhiệt độ bồn trộn trên/giữa/dưới + 3 card môi trường

**Mục tiêu:** Panel dạng danh sách dọc (key-value list) bên phải:
| Thông số | Giá trị |
|----------|---------|
| Nhiệt độ bồn trên | 23.9 °C |
| Nhiệt độ bồn giữa | 23.8 °C |
| Nhiệt độ bồn dưới | 24.1 °C |

**Công việc:**
- [ ] Xóa bỏ 3 card nhiệt độ bồn trộn (trên/giữa/dưới) khỏi row 1
- [ ] Tạo panel "Thông số quy trình hiện tại" dạng list dọc
- [ ] Thêm icon cho mỗi thông số

---

### 2.6. THÔNG SỐ MÔI TRƯỜNG — **THAY ĐỔI BỐ CỤC**

**Hiện tại:** 3 card (Nhiệt độ MT, Độ ẩm, Bụi) + các card nhiệt độ bồn trộn → 6 card/row, mỗi card full width

**Mục tiêu:** Panel nhỏ gọn hơn, 4 ô ngang:
| Nhiệt độ phòng | Độ ẩm | Bụi PM2.5 | Bụi tổng |
|---|---|---|---|
| 25.5 °C | 60.2 %RH | 18 µg/m³ | 0.38 mg/m³ |
| Tiêu chuẩn: ≤ 30°C | Tiêu chuẩn: 30-70% | Tiêu chuẩn: ≤ 50 | Tiêu chuẩn: ≤ 1.0 |

**Sự khác biệt cụ thể:**
- Hiện tại: Gộp nhiệt độ bồn trộn chung row với môi trường → Mục tiêu: Tách riêng ra panel "Thông số QT hiện tại"
- Hiện tại: 3 card → Mục tiêu: 4 card (thêm "Bụi tổng" tách riêng Bụi PM2.5)
- Bỏ phần "Min-Max", thay bằng "Tiêu chuẩn" format gọn hơn

**Công việc:**
- [ ] Tái cấu trúc section "Tổng quan môi trường" thành 4 card nhỏ gọn
- [ ] Tách card "Chỉ số bụi" thành 2 card: PM2.5 + Bụi tổng
- [ ] Bỏ row Min-Max, giữ Tiêu chuẩn
- [ ] Di chuyển vị trí section xuống góc phải dưới (bên cạnh "Tổng quan mẻ")

---

### 2.7. LỊCH SỬ CÁC BƯỚC CỦA MẺ HIỆN TẠI — **THAY ĐỔI CỘT**

**Hiện tại:** Bảng "Thống kê mẻ hiện tại" với cột:
`Công đoạn | TC cài đặt | T.g bắt đầu | T.g kết thúc | Thời gian | NĐ bồn trên | NĐ bồn giữa | NĐ bồn dưới | Trạng thái | Cảnh báo`

**Mục tiêu:** Bảng "Lịch sử các bước của mẻ hiện tại" với cột:
`Bước | Tên bước | Thời gian chuẩn (s) | Bắt đầu | Kết thúc | Thời gian thực tế (s) | Chênh lệch (s) | Trạng thái | Người vận hành | Cảnh báo`

**Sự khác biệt cụ thể:**
- Thêm cột: "Bước" (số thứ tự), "Tên bước", "Chênh lệch (s)", "Người vận hành"
- Bỏ 3 cột nhiệt độ bồn (trên/giữa/dưới) — chuyển sang panel riêng
- Cột "Chênh lệch" highlight màu đỏ nếu vượt, xanh nếu sớm

**Công việc:**
- [ ] Cập nhật cấu trúc bảng `batch-table` thead
- [ ] Thêm cột Bước, Tên bước, Chênh lệch, Người vận hành
- [ ] Bỏ 3 cột nhiệt độ bồn
- [ ] CSS cho giá trị chênh lệch âm/dương (xanh/đỏ)
- [ ] Thêm icon radio button cho mỗi row (ảnh mục tiêu có ◉ ở cột đầu)

---

### 2.8. TỔNG QUAN MẺ — **THAY ĐỔI BỐ CỤC**

**Hiện tại:** 2 phần tách riêng:
1. Bảng "Tổng số mẻ sản xuất trong ngày" (bảng dọc)
2. Panel "Thông tin sản phẩm" + "BOM sản xuất"

**Mục tiêu:** Panel "Tổng quan mẻ" gọn ở góc phải dưới:
| Trường | Giá trị |
|--------|---------|
| Mẻ số | 2 |
| Sản phẩm | Jelly Powder JP-01 |
| Sản lượng kế hoạch | 2,000 KG |
| Sản lượng đã SX | 1,000 KG (50%) |
| Thời gian bắt đầu mẻ | 14:02:10 |
| Thời gian dự kiến kết thúc | 15:33:00 |
| Thời gian còn lại mẻ | 1h 20m 35s |
| **Nút KẾT THÚC MẺ** | Nút đỏ lớn |

**Công việc:**
- [ ] Thay thế bảng "Tổng số mẻ sản xuất trong ngày" bằng panel "Tổng quan mẻ"
- [ ] Gom thông tin sản phẩm vào panel này
- [ ] Thêm nút "KẾT THÚC MẺ" (đỏ, to, góc phải dưới)
- [ ] Bỏ/ẩn panel "BOM sản xuất" (hoặc chuyển sang tab khác)

---

### 2.9. SECTION "THỜI GIAN DỮ LIỆU" (8 small cards) — **XÓA BỎ**

**Hiện tại:** 8 card nhỏ hiển thị thời gian các bước (Cấp liệu, Trộn 1, Xả đáy, Rung xả đáy, Hút xả đáy, Trộn 2, Xả hàng, Rung xả hàng)

**Mục tiêu:** KHÔNG CÒN SECTION NÀY — Thông tin thời gian đã được tích hợp vào:
- Thanh timeline quy trình (section 2.1)
- Bảng lịch sử bước (section 2.7)
- Panel thông tin bước hiện tại (section 2.2)

**Công việc:**
- [ ] Xóa hoặc ẩn toàn bộ section "Thời gian dữ liệu" (8 small cards)

---

## 3. LAYOUT TỔNG THỂ MỚI

```
┌─────────────────────────────────────────────────────────────────┐
│ HEADER BAR (đã có)                                              │
├─────────────────────────────────────────────────────────────────┤
│ TRẠNG THÁI QUY TRÌNH (Timeline ngang 9 steps)     | Thống kê  │
├──────────────┬──────────────────────┬───────────────────────────┤
│ THÔNG TIN    │ SƠ ĐỒ THIẾT BỊ     │ THÔNG SỐ QUY TRÌNH       │
│ BƯỚC HIỆN TẠI│ BỒN TRỘN           │ HIỆN TẠI (list dọc)       │
│              │ (hình minh họa)      │                           │
├──────────────┤                      ├───────────────────────────┤
│ ĐIỀU KIỆN    │                      │ THÔNG SỐ MÔI TRƯỜNG      │
│ CHUYỂN BƯỚC  │                      │ (4 card ngang)            │
│ (checklist)  │                      │                           │
├──────────────┴──────────────────────┼───────────────────────────┤
│ LỊCH SỬ CÁC BƯỚC CỦA MẺ HIỆN TẠI │ TỔNG QUAN MẺ              │
│ (bảng chi tiết)                     │ + Nút KẾT THÚC MẺ        │
└─────────────────────────────────────┴───────────────────────────┘
```

**Grid CSS đề xuất:**
```css
.overview-dashboard {
    display: grid;
    grid-template-columns: 1fr;
    gap: 16px;
    padding: 16px;
}

.overview-main-content {
    display: grid;
    grid-template-columns: 250px 1fr 350px;
    grid-template-rows: auto auto;
    gap: 16px;
}

.overview-bottom {
    display: grid;
    grid-template-columns: 1fr 350px;
    gap: 16px;
}
```

---

## 4. CHECKLIST TỔNG HỢP VIỆC CẦN LÀM

### Phase 1: Restructure Layout
- [ ] Refactor `Overview.cshtml` layout từ scroll-vertical sang grid dashboard
- [ ] Cập nhật `Overview.css` cho grid layout mới

### Phase 2: Tạo các section MỚI
- [ ] **[MỚI]** Timeline trạng thái quy trình (9 steps)
- [ ] **[MỚI]** Panel thông tin bước hiện tại + progress bar
- [ ] **[MỚI]** Sơ đồ thiết bị bồn trộn (SVG/image + sensor overlays)
- [ ] **[MỚI]** Checklist điều kiện chuyển bước

### Phase 3: Thay đổi section HIỆN CÓ
- [ ] **[SỬA]** Cards môi trường: 6 → 4 card, tách PM2.5 vs Bụi tổng
- [ ] **[SỬA]** Thông số quy trình: Chuyển từ cards sang list dọc, thêm thông số mới
- [ ] **[SỬA]** Bảng lịch sử bước: Thêm/bỏ cột theo thiết kế mới
- [ ] **[SỬA]** Tổng quan mẻ: Gom thông tin + thêm nút Kết thúc mẻ

### Phase 4: Xóa section KHÔNG CẦN
- [ ] **[XÓA]** Section "Thời gian dữ liệu" (8 small cards)
- [ ] **[XÓA/ẨN]** Panel BOM sản xuất (hoặc chuyển tab)
- [ ] **[XÓA]** Panel "Tổng số mẻ sản xuất trong ngày" (thay bằng Tổng quan mẻ)

### Phase 5: JavaScript & Realtime
- [ ] Cập nhật `OverviewRealtime.js` cho các ID mới
- [ ] Thêm binding cho sensor data trên sơ đồ bồn trộn
- [ ] Thêm logic cập nhật timeline, checklist, progress bar
- [ ] Xử lý nút "KẾT THÚC MẺ"

### Phase 6: Polish
- [ ] Responsive breakpoints
- [ ] Micro-animations cho status changes
- [ ] Test trên các kích thước màn hình

---

## 5. FILE CẦN THAY ĐỔI

| File | Hành động | Mức độ |
|------|-----------|--------|
| `Views/Home/Overview.cshtml` | Restructure toàn bộ HTML | **Lớn** |
| `Css/Overview.css` | Rewrite CSS cho layout mới | **Lớn** |
| `JavaScript/RealTime/OverviewRealtime.js` | Thêm bindings, xóa gauge charts | **Trung bình** |
| `Views/Shared/_LayoutMain.cshtml` | Có thể cần thêm field header | **Nhỏ** |
| Asset mới: SVG bồn trộn | Tạo mới | **Trung bình** |

---
