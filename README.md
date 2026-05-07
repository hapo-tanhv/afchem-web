# SOL Solar SCADA

A solar power monitoring system built with ASP.NET MVC 5 and AdminLTE 2 (Bootstrap 3).

## Technology Stack

- **Framework**: ASP.NET MVC 5 (.NET Framework 4.6+)
- **Frontend**: AdminLTE 2, Bootstrap 3, Font Awesome
- **Authentication**: Session-based login
- **Database**: SQL Server (via ADO.NET)
- **Real-time**: Web API for data monitoring

## Project Structure

```
LongDucProjectTest/
├── App_Start/
│   ├── BundleConfig.cs      # CSS/JS bundling
│   ├── FilterConfig.cs      # MVC filters
│   └── RouteConfig.cs       # URL routing
├── Controllers/
│   ├── HomeController.cs    # Login, authentication
│   ├── AlarmController.cs   # Alarm management
│   ├── DataRealTimeController.cs  # Real-time data
│   └── ...                  # Other controllers
├── Views/
│   ├── Shared/
│   │   └── _LayoutMain.cshtml  # Main layout (dark theme)
│   ├── Home/
│   │   ├── Login.cshtml
│   │   ├── Home.cshtml
│   │   └── ...
│   └── ...                  # Other views
├── Models/                  # Data models
├── Content/                 # CSS, images
└── Scripts/                 # JavaScript files
```

## Features

- User authentication (session-based)
- Real-time solar power monitoring
- Alarm management and display
- Event logging
- Multiple project layouts (Project1-7, LDIP, Signage1-7)
- Dark theme UI with modern styling
- Responsive design
- Notification system with dropdown

## Getting Started

### Prerequisites

- Visual Studio 2019 or later
- .NET Framework 4.6+
- SQL Server (for database)

### Configuration

1. Update `Web.config` with your database connection string
2. Configure your SMTP settings for email notifications (if needed)
3. Update any API endpoints for inverter communication

### Build & Run

1. Open `LongDucProjectTest.sln` in Visual Studio
2. Restore NuGet packages
3. Build the solution (Ctrl+Shift+B)
4. Run the project (F5)

### Default Login

- Login page: `/Home/Login`
- Use credentials configured in the database

## User Roles

- **JGC**: Junior Gird Controller
- **Hino**: Hino monitoring
- **LDIP**: Long Duc Industrial Park
- **Project1-7**: Project-specific roles
- **Signage1-7**: Signage-specific roles

## Layouts

The application uses different layouts based on user role:
- `_LayoutMain.cshtml`: Main layout with dark theme
- `_LayoutLDIP.cshtml`: LDIP specific
- `_LayoutProject1.cshtml` - `_LayoutProject7.cshtml`: Project specific
- `_LayoutSignage1.cshtml` - `_LayoutSignage7.cshtml`: Signage specific

## API Endpoints

- `/DataRealTime/GetInverterData`: Real-time inverter data
- `/Alarm/GetAlarms`: Get alarm list
- `/Home/Login`: User authentication

## License

Copyright © 2024 SOL Solar. All rights reserved.