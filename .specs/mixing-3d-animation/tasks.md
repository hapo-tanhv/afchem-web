# Danh sách Tác vụ Triển khai - Hoạt ảnh 3D Máy trộn

## 1. Thiết lập môi trường và cấu trúc DOM
- [ ] 1.1 Tích hợp thư viện Three.js và tạo khung chứa canvas trên trang Overview
  - Tải thư viện Three.js r128 từ cdnjs trong View Overview.cshtml.
  - Thêm thẻ container `<div id="threeJsMixerContainer">` đè lên khu vực bồn trộn trên sơ đồ 2D.
  - Thiết lập CSS tuyệt đối (absolute positioning) để canvas định vị chính xác và có z-index phù hợp không cản trở các luồng SVG khác.
  - _Requirements: 1.1, 1.3_

- [ ] 1.2 (P) Khởi tạo Three.js Renderer, Scene và Camera
  - Khởi tạo WebGLRenderer với nền trong suốt (alpha: true) để nhìn xuyên thấu vào nền sơ đồ bên dưới.
  - Thiết lập camera góc nhìn 2.5D tĩnh (chếch nhẹ) khớp hoàn hảo góc nghiêng của bồn chứa 2D.
  - Lắng nghe sự kiện window resize để cập nhật tỉ lệ canvas và camera đảm bảo hiển thị responsive.
  - _Requirements: 1.2, 1.4, 1.5_

## 2. Dựng hình học trục trộn và khung gá bằng code (Procedural)
- [ ] 2.1 (P) Xây dựng thuật toán vẽ cánh xoắn kép và trục xi lanh
  - Định nghĩa đường cong HelixCurve kế thừa từ THREE.Curve để tính tọa độ đường xoắn ốc toán học.
  - Dựng hình ống cánh xoắn TubeGeometry bao quanh lõi trục CylinderGeometry tạo thành trục trộn đơn.
  - Gom các phần hình học này vào đối tượng Group đại diện cho trục xoắn.
  - _Requirements: 2.1_

- [ ] 2.2 (P) Vẽ khung gá đỡ liên kết và thiết lập vật liệu, ánh sáng
  - Tạo khung đỡ nằm ngang liên kết hai trục trộn lệch tâm với trục xoay chính giữa bồn chứa.
  - Áp dụng vật liệu kim loại MeshStandardMaterial có độ bóng bẩy cao cho toàn bộ mô hình 3D.
  - Thêm nguồn sáng AmbientLight và DirectionalLight để chiếu sáng đổ bóng, làm nổi bật cấu trúc 3D trên nền 2D.
  - _Requirements: 2.2, 2.3_

## 3. Hoạt ảnh điều khiển thời gian thực và tích hợp hệ thống
- [ ] 3.1 Triển khai vòng lặp animation và liên kết trạng thái SCADA
  - Thiết lập vòng lặp kết xuất liên tục dùng requestAnimationFrame.
  - Trong luồng render, đọc trạng thái biến động cơ máy trộn toàn cục từ PLC.
  - Khi động cơ ở trạng thái chạy, xoay liên tục hai trục quanh tâm chính mình (tự xoay) và xoay khung đỡ quanh tâm bồn (quỹ đạo) ở tốc độ không đổi.
  - Khi động cơ ở trạng thái dừng, tạm dừng hoạt ảnh xoay và giữ nguyên tư thế mô hình.
  - _Requirements: 3.1, 3.2, 3.3_

- [ ] 3.2 Giải phóng bộ nhớ và kiểm thử tích hợp
  - Thực hiện dọn dẹp WebGL context và giải phóng bộ nhớ (dispose geometry, material, renderer) khi rời trang hoặc tải lại trang để tránh rò rỉ tài nguyên.
  - Thực hiện chạy thử nghiệm tích hợp trên trình duyệt để kiểm tra tính mượt mà của hoạt ảnh và tính chính xác của phản hồi tín hiệu từ PLC.
  - _Requirements: 1.2, 3.3_
