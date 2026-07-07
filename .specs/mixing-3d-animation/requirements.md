# Requirements Document - Mixing 3D Animation

## Introduction
Tính năng này cải tiến phần hiển thị hoạt ảnh của bồn trộn trên trang Overview. Hiện tại, trang [Overview.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/Overview.cshtml) đang sử dụng một ảnh sơ đồ 2D (`sodobontron2.jpeg`) và một lớp SVG chồng đè để tạo hiệu ứng dòng chảy 2D đơn giản. Chức năng mới sẽ tích hợp một canvas 3D bằng thư viện Three.js, dựng hình động cơ trục trộn xoắn kép thông qua mã nguồn (procedural generation), hiển thị đè đúng vị trí bồn trộn và chạy hoạt ảnh xoay khi máy trộn hoạt động dựa trên biến trạng thái thời gian thực `window.dongCoTron`.

## Requirements

### Requirement 1: Khởi tạo Viewport Three.js trên sơ đồ 2D
**Objective:** As an Operator, I want a 3D overlay representing the mixing area, so that I can visualize the mixer in 3D without obscuring the static 2D feeding and discharging diagrams.

#### Acceptance Criteria
1. When người dùng truy cập trang Overview, the Web Application shall tải thư viện Three.js (phiên bản r128 hoặc tương đương) từ CDN.
2. When thư viện Three.js tải xong, the Web Application shall khởi tạo đối tượng WebGLRenderer và chèn một phần tử canvas vào vùng chứa bồn trộn.
3. The Web Application shall cấu hình Three.js canvas ở chế độ `position: absolute` đè lên khu vực bồn trộn trên sơ đồ `sodobontron2.jpeg`.
4. The Web Application shall thiết lập nền canvas trong suốt (alpha: true) và camera ở góc phối cảnh 2.5D cố định để khớp với góc nhìn của sơ đồ 2D.
5. When kích thước cửa sổ trình duyệt thay đổi, the Web Application shall tự động cập nhật lại tỷ lệ (resize) canvas và camera để duy trì vị trí khớp nối chính xác trên sơ đồ.

### Requirement 2: Dựng hình 3D trục trộn xoắn kép qua mã nguồn
**Objective:** As a Developer, I want to construct the mixer shafts and components programmatically in Three.js, so that I don't need to load external GLTF files.

#### Acceptance Criteria
1. The Web Application shall dựng hai trục dạng xoắn kép (double helical screw shafts) sử dụng các đối tượng hình học nguyên bản của Three.js (ví dụ như `TubeGeometry` theo đường xoắn ốc toán học hoặc `ExtrudeGeometry`).
2. The Web Application shall dựng một khung gá đỡ (supporting bracket) kết nối hai trục trộn với tâm quay của bồn chứa.
3. The Web Application shall áp dụng vật liệu kim loại bán phản xạ (MeshStandardMaterial với độ kim loại - metalness cao) và cấu hình nguồn sáng chiếu vào bồn để hiển thị các chi tiết 3D một cách trực quan, chân thực.

### Requirement 3: Điều khiển hoạt ảnh trộn thời gian thực
**Objective:** As an Operator, I want the mixing shafts to twist and rotate concurrently, so that the simulation matches the actual mechanical mixing process.

#### Acceptance Criteria
1. While biến trạng thái `window.dongCoTron` có giá trị bằng `1`, the Three.js Animation Service shall xoay liên tục hai trục xoắn quanh trục tự thân của chúng với tốc độ không đổi.
2. While biến trạng thái `window.dongCoTron` có giá trị bằng `1`, the Three.js Animation Service shall xoay liên tục khung gá đỡ quanh trục chính giữa bồn chứa với tốc độ không đổi để tạo chuyển động quỹ đạo.
3. When biến trạng thái `window.dongCoTron` có giá trị bằng `0`, the Three.js Animation Service shall dừng toàn bộ hoạt ảnh chuyển động của trục xoắn và khung gá đỡ.
