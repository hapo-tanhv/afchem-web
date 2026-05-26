# Code Standards

## Stack
- **Language:** C# 6.0 (supported by MSBuild 2019)
- **Framework:** ASP.NET MVC 5 (.NET Framework 4.7.2)
- **Database:** MySQL Server 5.6+
- **Front-end:** Vanilla HTML, CSS, JavaScript, jQuery 3.4.1, Bootstrap 3.4.1

## Conventions
- **Controller Action Protection:** Every action that modifies system settings or exports data must verify authentication:
  ```csharp
  if (Session["Role"] is null || (int)Session["Role"] != (int)Role.Admin)
  {
      // Throw 403 HttpException or return JSON Status = false
  }
  ```
- **Encoding Rule (CRITICAL):** All `.cshtml` view files and resource scripts containing Vietnamese characters must be saved in **UTF-8 with BOM** (`utf-8-sig`) encoding. Failure to include BOM will result in character rendering errors.
- **SQL Queries:** DB access should be executed through the unified `MySQLConnect` utility with proper parameter sanitization.
- **UI Elements:** Use modern typography (Google Fonts Outfit/Inter) and standard SCADA Dark Theme palette (`#1e293b` for card panels, `#0f172a` for background and inputs, and `#3b82f6` for accents).

## File Organization
- `Controllers/*.cs` - Hosts clean controller actions.
- `Views/Home/*.cshtml` - Hosts clean layouts. Avoid inline CSS.
- `docs/*.md` - Hosts detailed system documentation and checklists.
