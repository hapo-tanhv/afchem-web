# Codebase Summary

> Auto-generated project overview

## Project Info
| Property | Value |
|----------|-------|
| **Name** | LongDucProjectTest |
| **Version** | 1.0.0 |
| **Type** | ASP.NET MVC 5 Web Application |
| **Language** | C# |

## Statistics
| Metric | Value |
|--------|-------|
| Projects in Solution | 7 |
| Primary Database | MySQL |
| Theme Style | AdminLTE + Premium SCADA Dark Theme |

## Tech Stack
| Layer | Technology |
|-------|------------|
| Framework | ASP.NET MVC 5 (.NET Framework 4.7.2) |
| Language | C# 6.0 |
| Styling | Vanilla CSS + Bootstrap 3.4.1 |
| UI Template | AdminLTE |
| Database | MySQL Server (MySql.Data connector) |
| Utilities | EPPlus (Excel), CsvHelper (CSV), Newtonsoft.Json (JSON) |

## Project Structure
```
c:\Users\tanhv\Project\WebApp_LongDuc_22012025Phase2\WebApp_LongDuc_22012025Phase2
├── LongDucProjectTest.sln            # Visual Studio Solution
├── PROJECT_DOCUMENTATION.md          # Project Quick Reference
├── README.md                         # Project Readme
├── Hino.Getdata.Common               # Common helper libraries (DB auth, models)
├── Hino.Getdata.Project              # Database querying for SCADA projects
├── Hino.Solar.DatabaseConnector      # Database connection executor
├── Connect                           # Desktop Windows Forms app (Windows execution helper)
├── docs/                             # Project documentation directory
│   ├── system-test-checklist.md      # Core test checklist for the 5 pages
│   └── codebase-summary.md           # This summary file
└── LongDucProjectTest/               # Main Web Application directory
    ├── Controllers/                  # MVC Controllers (Home, Alarm, Event, etc.)
    ├── Views/                        # Razor cshtml view templates (Home, Shared)
    ├── App_Start/                    # MVC bootstrap config (RouteConfig)
    ├── Scripts/                      # Front-end JavaScript logic
    ├── Css/                          # Custom stylesheet directories
    ├── Image/                        # Image assets (Diagram overlays)
    └── Web.config                    # IIS web server configuration
```

## Key Directories
| Directory | Purpose |
|-----------|---------|
| `LongDucProjectTest/Controllers` | Hosts the MVC backend controllers managing APIs and web routes |
| `LongDucProjectTest/Views/Home` | Hosts the 5 core pages: Overview, Alarm, Event, Report, UserSetting |
| `LongDucProjectTest/Views/Shared` | Hosts shared layouts, specifically the master `_LayoutMain.cshtml` |
| `Hino.Getdata.Common` | Holds shared models, authorization enums, and database ConnectionStrings |
| `docs` | Holds comprehensive system documentation, architecture guides, and test checklists |
