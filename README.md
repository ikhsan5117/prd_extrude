# Velasto Production System

Sistem Manajemen Produksi Hose terintegrasi untuk PT. Velasto Indonesia - Plant 2504 Manufacturing Facility 4 - Tango

## 📋 Deskripsi

Sistem ini adalah aplikasi web berbasis ASP.NET Core MVC yang dirancang untuk mengelola semua aspek produksi hose di Velasto Indonesia, termasuk:

- **Standard Parameter Setting** - Master data parameter setting produksi
- **Now Producing** - Monitoring produksi realtime
- **Production Report** - Laporan lengkap parameter dan dimensi produksi
- **Lot Tag Management** - Pengelolaan lot tag dengan barcode dan QC tracking
- **Packing Standard** - Standard packing dan quantity per part number

## 🎯 Fitur Utama

### 1. Standard Parameter Setting (SPS)
- Input dan manajemen parameter standard untuk setiap jenis hose
- Mencakup:
  - Spesifikasi material (Inner, Outer, Yarn)
  - Setting die dimensions
  - Temperature settings (Head, Cylinder 1-3, Screw)
  - Speed & pressure settings
  - Spiral settings
  - Conveyor settings
  - Quality parameters

### 2. Now Producing
- Dokumentasi produksi yang sedang berjalan
- Tracking material yang digunakan (Inner, Middle, Outer)
- Monitoring dandori time dan production time
- SPV check integration
- Status tracking: Setup → Running → Completed

### 3. Production Report
- **Parameter Setting Report** - Record actual parameter vs standard
- **Dimension Report** - Quality control dimensions:
  - Inner diameter & thickness
  - Total thickness
  - Spiral pitch
  - Visual inspection
  - Qty tracking (OK/NG)
- Time-series reading (interval 30 minutes)
- Multi-reading per production run

### 4. Lot Tag Management
- Generate lot tag dengan barcode
- Tracking:
  - Lot number (VH + timestamp)
  - Part number & description
  - Target & actual quantity
  - Compound information
  - Component list (Daftar Komponen)
- Quality Check (QC):
  - Subcon & Mesin check
  - Qty OK/NG tracking
  - NG reason documentation
- Print-ready format sesuai standar Velasto
- Duplicate side tag untuk identifikasi

### 5. Packing Standard
- Master data packing standard per NA Code
- Dandori & DH* tracking
- Standard quantity per part
- VIN code mapping

## 🛠️ Teknologi

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server dengan Entity Framework Core
- **Frontend**: Bootstrap 5 + Bootstrap Icons
- **Authentication**: (Siap untuk ditambahkan)
- **Reporting**: Print-ready views dengan CSS print styling

## 📦 Struktur Database

### Models
1. **StandardParameterSetting** - Parameter standard produksi
2. **NowProducing** - Data produksi aktif
3. **ProductionReport** - Laporan produksi (parent)
4. **ProductionReading** - Reading parameter per interval (child)
5. **DimensionReading** - Reading dimensi per interval (child)
6. **LotTag** - Tag lot produksi
7. **PackingStandard** - Standard packing

### Relasi Database
```
StandardParameterSetting (1) ─→ (*) ProductionReport
ProductionReport (1) ─→ (*) ProductionReading
ProductionReport (1) ─→ (*) DimensionReading
ProductionReport (1) ─→ (*) LotTag
```

## 🚀 Instalasi & Setup

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2019 atau lebih baru
- Visual Studio 2022 atau VS Code

### Langkah Instalasi

1. **Clone Repository**
```bash
git clone [repository-url]
cd VelastoProductionSystem
```

2. **Update Connection String**
Edit `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=prd_extrude_hose;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

Untuk production server, update dengan:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=prd_extrude_hose;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
  }
}
```

3. **Install Dependencies**
```bash
dotnet restore
```

4. **Create Database Migration**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. **Run Application**
```bash
dotnet run
```

Application akan berjalan di: `https://localhost:7000` atau `http://localhost:5000`

## 📱 Penggunaan

### 1. Setup Standard Parameter
1. Navigate ke **Master Data → Standard Parameter Setting**
2. Klik **Create New**
3. Isi semua parameter sesuai dengan SOP produksi
4. Save dan Approve

### 2. Start Production
1. Navigate ke **Production → Now Producing**
2. Klik **Start New Production**
3. Isi informasi:
   - Hose type, dimension, yarn
   - Material (Inner, Middle, Outer) dengan lot number dan SG
   - Dandori & production time
4. Submit untuk SPV check

### 3. Create Production Report
1. Navigate ke **Production → Production Report**
2. Klik **Create Production Report**
3. Pilih standard parameter setting yang sesuai
4. Isi informasi material dan die check
5. Save report

### 4. Add Parameter Readings
Dalam production report:
1. Klik **Add Reading**
2. Isi actual parameters (temperature, speed, pressure, dll)
3. Save (waktu reading otomatis tercatat)
4. Ulangi setiap 30 menit atau sesuai interval

### 5. Add Dimension Readings
Dalam production report:
1. Klik **Add Dimension**
2. Isi measurements:
   - Inner diameter (5 readings)
   - Inner thickness (5 readings)
   - Total thickness (5 readings)
   - Spiral pitch
   - Visual check
3. Isi Qty (OK/NG)
4. Save

### 6. Generate Lot Tag
1. Navigate ke **Production → Lot Tag**
2. Klik **Create New Lot Tag**
3. Isi:
   - Part number & description
   - Target qty & lot packaging
   - Compound information
   - Component list
4. Save → sistem generate lot tag number otomatis (VHyyMMddHHmmss)
5. Print lot tag

### 7. Quality Check Update
1. Dari Lot Tag list, klik **QC Update**
2. Isi:
   - Subcon & mesin check
   - Tanggal check
   - Qty OK & NG
   - NG reason (jika ada)
3. Update → status berubah ke "Completed"

## 🖨️ Printing

Semua form memiliki print-ready format:
- **Standard Parameter Setting** - Sesuai form VI-SOP-PROD-131
- **Now Producing** - Form checksheet untuk operator
- **Production Report** - Complete report dengan readings
- **Lot Tag** - Format label dengan barcode (include duplicate side tag)
- **Packing Standard** - Reference table

Tips printing:
- Use browser print (Ctrl+P)
- Set margins ke "None" atau "Minimum"
- Enable "Background graphics" untuk melihat borders
- Paper size: A4 untuk reports, Custom untuk lot tags

## 📊 Dashboard

Dashboard menampilkan:
- Total production reports
- Active productions
- Today's productions
- Total lot tags
- Quick actions menu
- System information

## 🔐 Security (To Be Implemented)

Saat ini sistem belum memiliki authentication. Untuk production deployment, tambahkan:
- ASP.NET Core Identity untuk user management
- Role-based access control:
  - **Operator** - Input production data
  - **Supervisor** - Approve & check
  - **QC** - Quality control updates
  - **Admin** - Master data management
  - **Manager** - View reports only

## 🎨 Customization

### Branding
Warna Velasto sudah terdefined di `_Layout.cshtml`:
```css
:root {
    --velasto-blue: #003D82;
    --velasto-red: #E30613;
}
```

### Logo
Tambahkan logo file di `wwwroot/images/velasto-logo.png` dan update di layout.

## 🐛 Troubleshooting

### Database Connection Error
```
Error: Cannot open database "prd_extrude_hose"
```
**Solution**: Pastikan SQL Server berjalan dan connection string benar.

### Migration Error
```
Error: Build failed
```
**Solution**: 
```bash
dotnet clean
dotnet build
dotnet ef migrations add InitialCreate
```

### Print Layout Issues
**Solution**: 
- Clear browser cache
- Check print preview
- Ensure "Background graphics" enabled

## 📞 Support

Untuk pertanyaan atau issue, hubungi:
- IT Department - PT. Velasto Indonesia
- Plant 2504 - Manufacturing Facility 4 - Tango

## 📝 Changelog

### Version 1.0.0 (March 2026)
- Initial release
- Complete CRUD untuk semua modules
- Print-ready forms
- Dashboard dengan statistics
- Lot tag dengan barcode generation
- Quality control tracking

## 🚧 Roadmap

Planned features:
- [ ] User authentication & authorization
- [ ] Real barcode scanner integration
- [ ] Excel export untuk reports
- [ ] Advanced search & filtering
- [ ] Production analytics & charts
- [ ] Email notifications untuk QC approval
- [ ] Mobile-responsive improvements
- [ ] API untuk integration dengan sistem lain
- [ ] Audit trail untuk semua perubahan data
- [ ] Batch lot tag printing

## 📄 License

© 2026 PT. Velasto Indonesia. Internal use only.

---

**Developed for PT. Velasto Indonesia**  
Plant 2504 - Manufacturing Facility 4 - Tango  
Velasto Production System v1.0
