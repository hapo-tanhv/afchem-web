# Deployment Guide

## Deployment Platforms
- **Web Server:** Microsoft IIS (Internet Information Services) 8.5+ or IIS Express (local development)
- **Database Server:** MySQL Server 5.6+

## Deployment Environment Variables
Database connection strings are configured in `Hino.Getdata.Common/Authentication.cs`:
- **Server:** `192.168.2.11` (production server) or `localhost` (local development)
- **Database:** `scada`
- **User:** `root`
- **Password:** `101101`

## Quick Compilation and Deploy Steps

### 1. Compile the Solution using Visual Studio MSBuild:
```powershell
& "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe" LongDucProjectTest.sln /t:Build /p:Configuration=Release
```

### 2. Configure Database MySQL Table:
Run this query on the production `scada` database to ensure the `account` table's `ID` field correctly supports auto-increment:
```sql
ALTER TABLE account MODIFY ID INT AUTO_INCREMENT;
```

### 3. Deploy Web Application to IIS:
1. Open **IIS Manager** on the server.
2. Add a new **Web Site** or map an existing one to the target folder: `LongDucProjectTest/`.
3. Set the **Application Pool** to use **.NET CLR Version v4.0** with **Integrated** pipeline mode.
4. Ensure the system user running the Application Pool has read/write permissions to the application path.
