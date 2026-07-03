# Kế hoạch triển khai: Trang Hướng dẫn sử dụng mới

Tài liệu thiết kế và danh sách các thay đổi để triển khai trang Hướng dẫn sử dụng mới trong WebApp.

## Yêu cầu
* Tạo một trang mới có tên là **Hướng dẫn sử dụng** (User Guide).
* Hiển thị danh sách các tài liệu hướng dẫn vận hành, hiện tại bao gồm liên kết tài liệu Google Doc:
  * [Tài liệu hướng dẫn vận hành Google Doc](https://docs.google.com/document/d/1m4Q-2nUYAadBFtszmIBdBwrnELT6ZE0hsxQZDegPJJA/edit?tab=t.0#heading=h.ddt12oo7j38c)
* Tích hợp link này vào menu thanh bên (Sidebar) ở layout chính để tất cả người dùng (Admin & Operator) sau khi đăng nhập đều có thể truy cập dễ dàng.
* Áp dụng giao diện AdminLTE chuyên nghiệp, sử dụng thiết kế hộp chứa (Card) hiện đại và gọn gàng.

## Các file chỉnh sửa và tạo mới

### [MODIFY] [HomeController.cs](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Controllers/HomeController.cs)
* Thêm action `UserGuide()` trả về View hướng dẫn sử dụng.
* Thiết lập trạng thái active cho nút trên Sidebar: `ViewBag.ButtonUserGuide = "active"`.
* Kiểm tra quyền đăng nhập cơ bản (`Session["Role"]`).

### [MODIFY] [_LayoutMain.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Shared/_LayoutMain.cshtml)
* Thêm mục menu mới `"Hướng dẫn sử dụng"` với icon `<i class="nav-icon fas fa-book"></i>`.
* Đặt nằm ngoài khối `@if` của Admin để cả Operator cũng nhìn thấy.

### [NEW] [UserGuide.cshtml](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/Views/Home/UserGuide.cshtml)
* View hiển thị danh sách tài liệu hướng dẫn.
* Sử dụng AdminLTE Card với tiêu đề `"DANH SÁCH TÀI LIỆU HƯỚNG DẪN"`.
* Trình bày danh sách link liên kết đẹp mắt với các icon trực quan, hỗ trợ mở liên kết trong tab mới (`target="_blank"`).

### [MODIFY] [LongDucProjectTest.csproj](file:///c:/Users/tanhv/Project/WebApp_LongDuc_22012025Phase2/WebApp_LongDuc_22012025Phase2/LongDucProjectTest/LongDucProjectTest.csproj)
* Đăng ký tệp View mới `Views\Home\UserGuide.cshtml` vào dự án để MSBuild nhận diện khi biên dịch.

---

## Kế hoạch kiểm thử (Verification)
* Truy cập trang WebApp ở localhost.
* Đăng nhập với tài khoản Operator hoặc Admin.
* Xác nhận nút "Hướng dẫn sử dụng" xuất hiện ở Sidebar và hiển thị trạng thái hoạt động chính xác khi nhấp chọn.
* Xác nhận trang hiển thị đầy đủ liên kết tài liệu Google Doc, nhấp vào liên kết mở ra tab mới bình thường.
