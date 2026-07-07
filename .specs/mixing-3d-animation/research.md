# Nghiên cứu & Quyết định Thiết kế - Hoạt ảnh 3D Máy trộn

---
**Mục đích**: Ghi lại các phát hiện nghiên cứu, phân tích kỹ thuật và lý do đằng sau các quyết định kiến trúc cho tính năng hoạt ảnh 3D máy trộn xoắn kép.
---

## Summary
- **Feature**: `mixing-3d-animation`
- **Discovery Scope**: Extension (Tích hợp Thư viện và Chuyển động 3D phía Client)
- **Key Findings**:
  - Tải thư viện Three.js qua CDN (cdnjs) và tích hợp vào View của ASP.NET MVC qua khối `@section Scripts` là giải pháp nhẹ nhàng nhất, tránh làm nặng dung lượng build của Project.
  - Sử dụng phương pháp dựng hình bằng code (Procedural Generation) thông qua việc định nghĩa đường cong toán học `THREE.Curve` và dựng hình ống `THREE.TubeGeometry` quấn quanh trục xi lanh `THREE.CylinderGeometry` là hoàn toàn khả thi để vẽ trục xoắn kép, không cần tải mô hình GLTF bên ngoài.
  - Định vị canvas dạng phần tử tuyệt đối (`position: absolute`) và thu nhỏ khu vực render (Localized Canvas) thay vì phủ toàn bộ màn hình sẽ tối ưu hóa hiệu năng render đáng kể.

## Research Log

### 1. Dựng hình xoắn kép (Helical Screw Shaft) bằng Three.js
- **Context**: Làm thế nào để vẽ trục xoắn cánh xoắn kép (screw conveyor) mà không dùng file GLTF?
- **Sources Consulted**: Tài liệu Three.js về `THREE.TubeGeometry` và `THREE.Curve`.
- **Findings**:
  - Có thể tạo một lớp kế thừa từ `THREE.Curve` để tính toán tọa độ đường xoắn ốc theo công thức:
    $$x = R \cdot \cos(\theta)$$
    $$z = R \cdot \sin(\theta)$$
    $$y = h \cdot t$$
    Trong đó $R$ là bán kính xoắn, $h$ là chiều cao trục, $t$ chạy từ 0 đến 1, và góc $\theta = t \cdot \text{vòng xoắn} \cdot 2\pi$.
  - Truyền đường cong này vào `THREE.TubeGeometry` với độ dày (radius) thích hợp để tạo cánh xoắn.
  - Trục trung tâm được tạo bằng `THREE.CylinderGeometry`. Ghép cả hai phần này vào một `THREE.Group` đại diện cho một trục xoắn.
- **Implications**: Trục trộn 3D được tạo hoàn toàn bằng Javascript chạy phía client, giảm tải băng thông tải file GLTF (~500KB - 2MB) xuống 0KB.

### 2. Điều khiển chuyển động xoắn kép (Double Shaft Mixing Animation)
- **Context**: Trục trộn xoắn kép hoạt động cơ học bằng cách vừa tự xoay vừa quay quỹ đạo. Làm thế nào để biểu diễn trong Three.js?
- **Sources Consulted**: Nguyên lý động học máy trộn xoắn hình nón (Conical Twin Screw Mixer).
- **Findings**:
  - Cần tạo một cấu trúc phân cấp (Hierarchical Tree) trong Three.js:
    - `MixerSystemGroup` (Tâm đặt tại tâm bồn trộn): Quay quanh trục Y để tạo chuyển động quỹ đạo (orbital rotation).
      - `ShaftHolderArm` (Khung đỡ nằm ngang kết nối hai trục): Quay cùng `MixerSystemGroup`.
        - `LeftShaftGroup` (Đặt lệch tâm trái): Tự xoay quanh trục Y cục bộ của nó.
        - `RightShaftGroup` (Đặt lệch tâm phải): Tự xoay quanh trục Y cục bộ của nó.
  - Trong hàm `requestAnimationFrame`:
    - Nếu `window.dongCoTron === 1`:
      - `MixerSystemGroup.rotation.y += orbitalSpeed` (quỹ đạo)
      - `LeftShaftGroup.rotation.y += selfSpeed` (tự xoay)
      - `RightShaftGroup.rotation.y += selfSpeed` (tự xoay)
- **Implications**: Chuyển động cơ học được mô phỏng chính xác tuyệt đối và mượt mà.

### 3. Tối ưu hóa hiệu năng và vị trí Canvas đè
- **Context**: Đặt canvas Three.js đè lên sơ đồ bồn trộn thế nào để không che các luồng nạp/xả và không gây lag?
- **Sources Consulted**: DOM CSS absolute positioning, Three.js transparent renderer.
- **Findings**:
  - **Phương án A (Full-size Canvas):** Tạo canvas phủ toàn bộ khung chứa 1000x1000px của sơ đồ. Cần tính toán camera phức tạp để khớp với tọa độ bồn trộn. Gây thừa vùng render và tốn GPU.
  - **Phương án B (Localized Canvas - Chọn):** Tạo một thẻ `<div id="threejs-mixer-container">` đặt bên trong `.tank-image-wrapper`, định vị bằng CSS absolute chính xác đè lên thân bồn trộn (ví dụ: `top: 35%; left: 36%; width: 28%; height: 42%;` tùy chỉnh tỉ lệ). Three.js chỉ render trong phạm vi nhỏ này.
- **Implications**: Tiết kiệm đáng kể tài nguyên GPU, không ảnh hưởng đến các luồng SVG cấp liệu (feeding) ở trên và xả đáy (discharge) ở dưới.

## Architecture Pattern Evaluation

| Option | Description | Strengths | Risks / Limitations | Notes |
|--------|-------------|-----------|---------------------|-------|
| **Localized Canvas + Procedural 3D** | Canvas nhỏ đè lên vùng bồn, dựng hình bằng code trong trang Overview. | Tải nhanh, hiệu năng cực tốt, không phụ thuộc file tĩnh ngoài. | Đòi hỏi viết code dựng hình toán học phức tạp hơn. | Phù hợp nhất với yêu cầu của dự án. |
| **Full-size Canvas + GLTF Model** | Tải file GLTF bồn trộn 3D đầy đủ và render toàn bộ bồn đè lên ảnh. | Visual rất đẹp và thực tế, ánh sáng đổ bóng chân thực. | Tải file nặng, khó căn chỉnh khớp với sơ đồ 2D xung quanh, tốn GPU. | Bị loại bỏ do yêu cầu giữ nguyên luồng cấp/xả 2D hiện tại. |

## Design Decisions

### Decision: Dựng hình xoắn kép bằng code (Procedural Tube Helix)
- **Context**: Người dùng không có sẵn mô hình 3D.
- **Selected Approach**: Tạo đường cong toán học kế thừa từ `THREE.Curve` và sinh `THREE.TubeGeometry`.
- **Rationale**: Cho phép tùy biến số vòng xoắn, bán kính xoắn, độ dày cánh xoắn trực tiếp bằng code và render tức thì khi tải trang.
- **Trade-offs**: Cần kiểm thử hình học để đảm bảo cánh xoắn trông cân đối và không bị lỗi dựng mặt (polygon clipping).

### Decision: Localized Canvas Overlay với Nền Trong Suốt
- **Context**: Tích hợp Three.js đè lên sơ đồ bồn trộn 2D hiện có.
- **Selected Approach**: Tạo thẻ div chứa canvas có `position: absolute` căn giữa bồn trộn, cấu hình renderer với `alpha: true` và `clearColor: 0x000000, 0`.
- **Rationale**: Đảm bảo bồn trộn 3D hiển thị hài hòa với sơ đồ 2D xung quanh, nhìn thấy trục trộn 3D quay trên nền chất lỏng hoặc khung bồn cũ.

## Risks & Mitigations
- **Risk 1 (Rò rỉ bộ nhớ khi chuyển trang):** WebGL context có thể không giải phóng nếu người dùng chuyển qua lại giữa các trang trong SCADA.
  - *Mitigation:* Bắt sự kiện hủy view hoặc hủy đối tượng Three.js (`renderer.dispose()`, giải phóng các hình học `geometry.dispose()` và vật liệu `material.dispose()`) khi rời trang Overview hoặc khi tải lại dữ liệu.
- **Risk 2 (Mất đồng bộ vị trí khi Responsive):** Khi trình duyệt resize, ảnh sơ đồ co giãn nhưng canvas 3D bị lệch.
  - *Mitigation:* Lắng nghe sự kiện `resize` trên window và cập nhật lại kích thước canvas của renderer và camera aspect ratio khớp theo kích thước của thẻ container cha.

## References
- [Three.js Documentation - TubeGeometry](https://threejs.org/docs/#api/en/geometries/TubeGeometry) - Hướng dẫn tạo hình ống theo đường cong.
- [Three.js transparent background](https://threejs.org/docs/#api/en/renderers/WebGLRenderer) - Hướng dẫn cấu hình renderer với tham số `alpha: true` để tạo nền trong suốt.
