# Tổng quan dự án LongDucProjectTest

## Thông tin cơ bản

| Thông tin | Chi tiết |
|-----------|----------|
| **Tên solution** | LongDucProjectTest.sln |
| **Framework** | ASP.NET MVC 5 (.NET Framework 4.7.2) |
| **Ngôn ngữ** | C# |
| **Template UI** | AdminLTE (Bootstrap 3.4.1) |
| **Database** | MySQL |

## Cấu trúc dự án

### Projects trong Solution

1. **LongDucProjectTest** - Main Web Application
2. **Hino.Solar.DatabaseConnector** - Kết nối database
3. **Hino.Getdata.Common** - Thư viện chung
4. **Hino.Getdata.Project** - Xử lý data projects
5. **Hino.Getdata.Project1, Project2** - Các module project
6. **Hino.Parameter.Common** - Tham số chung
7. **Connect** - Ứng dụng Desktop (Windows Forms)

### Thư mục chính (LongDucProjectTest)

```
LongDucProjectTest/
├── Controllers/          # Các MVC Controllers
├── App_Start/            # Cấu hình MVC (Bundle, Filter, Route)
├── Connected Services/   # Web Service references
├── AdminLTE-master/      # Template giao diện
├── Content/              # CSS, Images
├── Scripts/              # JavaScript files
└── Global.asax.cs        # Application entry point
```

## Controllers

| Controller | Mô tả |
|------------|-------|
| HomeController | Trang chủ, đăng nhập |
| AlarmController | Quản lý báo động |
| DataRealTimeController | Dữ liệu real-time |
| DataInverterController | Dữ liệu inverter |
| DataProjectController | Dữ liệu project |
| DataSignageController | Hiển thị data |
| EventController | Sự kiện |
| OverviewController | Tổng quan |
| LDIPController | Chức năng LDIP |
| Project1Controller | Project 1 |
| Project2Controller | Project 2 |
| Project3Controller | Project 3 |
| Project4Controller | Project 4 |
| Project5Controller | Project 5 |
| Project6Controller | Project 6 |
| Project7Controller | Project 7 |
| BaseController | Controller cơ sở |
| BaseProject1Controller | Controller cơ sở cho Project1 |

## NuGet Packages sử dụng

| Package | Version | Mục đích |
|---------|---------|----------|
| Microsoft.AspNet.Mvc | 5.2.7 | ASP.NET MVC Framework |
| bootstrap | 3.4.1 | CSS Framework |
| jQuery | 3.4.1 | JavaScript Library |
| CsvHelper | 30.0.1 | Xử lý file CSV |
| EPPlus | 6.1.0 | Excel file handling |
| Newtonsoft.Json | 12.0.2 | JSON serialization |
| jQuery.Validation | 1.17.0 | Client-side validation |

## Database

- **MySQL** - Sử dụng MySql.Data.dll để kết nối
- Các class library xử lý database:
  - Hino.Solar.DatabaseConnector
  - Hino.Getdata.Project

### Cấu hình Database Connection

Database connection string được cấu hình **hard-coded** trong file `Hino.Getdata.Common/Authentication.cs`:

```csharp
// Dòng 36 trong Authentication.cs
ConnectionString = $"Server=192.168.2.11;Database=scada;Uid=root;Pwd=101101;"
```

| Thông số | Giá trị |
|----------|---------|
| Server | 192.168.2.11 |
| Database | scada |
| Username | root |
| Password | 101101 |
| Port | 3306 (default MySQL) |

### Hướng dẫn cài đặt database local

1. **Cài đặt MySQL Server** (khuyến nghị phiên bản 5.7+)
2. **Tạo database** có tên `scada`
3. **Import schema和数据** (nếu có file .sql đi kèm)
4. **Cập nhật connection string** trong `Authentication.cs` nếu dùng database local khác

> **Lưu ý**: Connection string hiện tại đang trỏ đến server 192.168.2.11. Nếu muốn chạy local, cần sửa IP server thành `localhost` hoặc `127.0.0.1` và đảm bảo MySQL đã được cài đặt và chạy.

## Công nghệ sử dụng

- **Backend**: ASP.NET MVC 5, C#
- **Frontend**: HTML5, JavaScript, jQuery, Bootstrap 3
- **Template**: AdminLTE
- **Data**: MySQL, JSON
- **Excel**: EPPlus (import/export Excel)
- **CSV**: CsvHelper

## File cấu hình

- `LongDucProjectTest.sln` - Solution file
- `LongDucProjectTest/Web.config` - Web configuration
- `LongDucProjectTest/packages.config` - NuGet packages
- `LongDucProjectTest/Connect/App.config` - Desktop app config