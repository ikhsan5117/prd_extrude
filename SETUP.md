# Setup Instructions - Velasto Production System

## Quick Start Guide

### 1. Install Required Software

#### Windows
1. **Install .NET 8.0 SDK**
   - Download dari: https://dotnet.microsoft.com/download/dotnet/8.0
   - Run installer dan ikuti instruksi
   - Verify installation:
     ```powershell
     dotnet --version
     ```

2. **Install SQL Server**
   - Option A: SQL Server Express (Free)
     - Download: https://www.microsoft.com/sql-server/sql-server-downloads
     - Pilih "Express" edition
   - Option B: Use LocalDB (sudah termasuk di Visual Studio)
     ```powershell
     sqllocaldb create MSSQLLocalDB
     sqllocaldb start MSSQLLocalDB
     ```

3. **Install Visual Studio 2022 (Optional but Recommended)**
   - Download Community Edition (Free)
   - Pilih workload: "ASP.NET and web development"

### 2. Setup Database

#### Option A: Using Entity Framework Migrations (Recommended)

1. Open terminal di project folder:
   ```powershell
   cd F:\VelastoProductionSystem
   ```

2. Install EF Core tools (jika belum):
   ```powershell
   dotnet tool install --global dotnet-ef
   ```

3. Create migration:
   ```powershell
   dotnet ef migrations add InitialCreate
   ```

4. Update database:
   ```powershell
   dotnet ef database update
   ```

#### Option B: Using SQL Script (Manual)

Jika migration error, run SQL script berikut di SQL Server Management Studio:

```sql
-- Create Database
CREATE DATABASE prd_extrude_hose;
GO

USE prd_extrude_hose;
GO

-- Create Tables (script akan dibuat otomatis dari migration)
```

### 3. Configure Connection String

Edit file `appsettings.json`:

#### For LocalDB (Development):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=prd_extrude_hose;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

#### For SQL Server Express:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=prd_extrude_hose;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=True"
  }
}
```

#### For Production Server:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_IP;Database=prd_extrude_hose;User Id=velasto_user;Password=YOUR_STRONG_PASSWORD;TrustServerCertificate=True"
  }
}
```

### 4. Build & Run

1. **Restore dependencies:**
   ```powershell
   dotnet restore
   ```

2. **Build project:**
   ```powershell
   dotnet build
   ```

3. **Run application:**
   ```powershell
   dotnet run
   ```

4. **Open browser:**
   - Navigate to: `https://localhost:7000`
   - Atau: `http://localhost:5000`

### 5. Initial Data Setup

Setelah aplikasi berjalan:

1. **Create Standard Parameter Settings**
   - Navigate: Master Data → Standard Parameter Setting
   - Add setting untuk setiap jenis hose yang diproduksi
   - Reference dari form fisik yang ada di plant

2. **Create Packing Standards**
   - Navigate: Master Data → Packing Standard
   - Import data dari daftar NA Code yang ada
   - Reference dari form "STANDARD LENGTH & QUANTITY PACKING"

3. **Test Production Flow**
   - Start new production (Now Producing)
   - Create production report
   - Add parameter readings
   - Add dimension readings
   - Generate lot tag
   - Update QC

## Troubleshooting

### Error: "Unable to connect to database"

**Solution 1** - Check SQL Server running:
```powershell
# Check LocalDB
sqllocaldb info MSSQLLocalDB

# Start if stopped
sqllocaldb start MSSQLLocalDB
```

**Solution 2** - Test connection string:
```powershell
sqlcmd -S (localdb)\mssqllocaldb -Q "SELECT @@VERSION"
```

### Error: "dotnet ef not found"

**Solution**:
```powershell
dotnet tool install --global dotnet-ef
```

Verify:
```powershell
dotnet ef --version
```

### Error: "Build failed"

**Solution**:
```powershell
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Error: Migration fails

**Solution 1** - Remove and recreate:
```powershell
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

**Solution 2** - Manual database creation:
1. Open SQL Server Management Studio
2. Create database manually: `prd_extrude_hose`
3. Run migration again

### Port Already in Use

**Solution** - Change port in `Properties/launchSettings.json`:
```json
{
  "applicationUrl": "https://localhost:7001;http://localhost:5001"
}
```

## Production Deployment

### IIS Deployment (Windows Server)

1. **Install IIS with ASP.NET Core Module**
   ```powershell
   # Install IIS
   Enable-WindowsOptionalFeature -Online -FeatureName IIS-WebServer
   
   # Download ASP.NET Core Hosting Bundle
   # https://dotnet.microsoft.com/download/dotnet/8.0
   ```

2. **Publish Application**
   ```powershell
   dotnet publish -c Release -o C:\inetpub\VelastoProduction
   ```

3. **Create IIS Site**
   - Open IIS Manager
   - Add Website
   - Physical path: `C:\inetpub\VelastoProduction`
   - Binding: http/*:80 atau https/*:443

4. **Configure App Pool**
   - No Managed Code
   - Identity: ApplicationPoolIdentity

5. **Set Permissions**
   ```powershell
   icacls "C:\inetpub\VelastoProduction" /grant "IIS AppPool\VelastoProduction:(OI)(CI)F" /T
   ```

### Docker Deployment (Optional)

Create `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["VelastoProductionSystem.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VelastoProductionSystem.dll"]
```

Build & Run:
```powershell
docker build -t velasto-production .
docker run -d -p 8080:80 velasto-production
```

## Backup & Maintenance

### Database Backup

**Automated Backup Script** (`backup.sql`):
```sql
BACKUP DATABASE prd_extrude_hose
TO DISK = 'C:\\Backups\\prd_extrude_hose_' + 
    CONVERT(VARCHAR, GETDATE(), 112) + '.bak'
WITH COMPRESSION, INIT;
```

Run daily via SQL Server Agent or Task Scheduler.

**Manual Backup**:
```powershell
sqlcmd -S localhost -Q "BACKUP DATABASE prd_extrude_hose TO DISK='C:\Backups\prd_extrude_hose.bak'"
```

### Application Logs

Logs location: `logs/` folder (to be configured)

Monitor for errors and performance issues.

## Network Configuration

### For Production Environment

1. **Open Firewall Ports**
   ```powershell
   # HTTP
   netsh advfirewall firewall add rule name="Velasto HTTP" dir=in action=allow protocol=TCP localport=80
   
   # HTTPS
   netsh advfirewall firewall add rule name="Velasto HTTPS" dir=in action=allow protocol=TCP localport=443
   ```

2. **Configure SSL Certificate** (for HTTPS)
   - Purchase SSL certificate atau use Let's Encrypt
   - Install di IIS
   - Update binding ke HTTPS

3. **Static IP Configuration**
   - Set static IP untuk production server
   - Update DNS jika diperlukan
   - Update connection strings di application

## Performance Tuning

### SQL Server

```sql
-- Index optimization
CREATE NONCLUSTERED INDEX IX_ProductionReport_Date 
ON ProductionReports (ProductionDate DESC);

CREATE NONCLUSTERED INDEX IX_LotTag_Number 
ON LotTags (LotTagNumber);

-- Update statistics
UPDATE STATISTICS ProductionReports;
UPDATE STATISTICS LotTags;
```

### Application

1. Enable response caching
2. Use async operations
3. Implement connection pooling
4. Monitor memory usage

## Support Contacts

**IT Department - PT. Velasto Indonesia**
- Email: it@velasto.co.id
- Phone: +62-xxx-xxxx-xxxx
- Plant: 2504 - Tango

**System Administrator**
- On-site support available
- Remote assistance via TeamViewer/AnyDesk

---

Last Updated: March 2026
Version: 1.0
