# Perbaikan Pengelompokan Kolom Tabel - SPS Master Index

## 📊 Masalah yang Diperbaiki

### 1. **Colspan Salah** (Header Terpotong)
**SEBELUM:**
```
Grup 2: Spesifikasi Hose & Dies  → colspan="20" (SALAH! Aktual 32 kolom)
Grup 3: Mesh                     → colspan="3"  (SALAH! Aktual 12 kolom)
Grup 4: Suhu Pengerjaan          → colspan="17" (SALAH! Aktual 45 kolom)
Grup 5: Output Mesin             → colspan="21" (SALAH! Aktual 49 kolom)
Grup 6: Proses Akhir             → colspan="11" (SALAH! Aktual 17 kolom)
Grup 7: Final Quality Matrix     → colspan="31" (SALAH! Aktual 39 kolom)
```

**SESUDAH (DIPERBAIKI):**
```
Grup 1: Identifikasi Utama        → colspan="11"  ✅ (11 kolom)
Grup 2: Spesifikasi Hose & Dies   → colspan="32"  ✅ (32 kolom)
Grup 3: Mesh                      → colspan="12"  ✅ (12 kolom)
Grup 4: Suhu Pengerjaan           → colspan="45"  ✅ (45 kolom)
Grup 5: Output Mesin              → colspan="49"  ✅ (49 kolom)
Grup 6: Proses Akhir              → colspan="17"  ✅ (17 kolom)
Grup 7: Final Quality Matrix      → colspan="39"  ✅ (39 kolom)
Kolom Opsi                        → rowspan="2"   ✅ (1 kolom)

TOTAL: 206 KOLOM
```

---

## 🎨 Visual Improvements

### 2. **Border Visual Separator**
- ✅ Group header diberi border kiri-kanan putih untuk pembatas grup
- ✅ Kolom pertama tiap grup diberi **border kuning tebal** (5px) + background kuning transparan
- ✅ Marker-im (red) dan marker-re (cyan) diperlebar jadi 4px untuk lebih jelas
- ✅ Padding tambahan untuk kolom dengan marker

**CSS Baru:**
```css
.group-label {
    border-left: 4px solid rgba(255,255,255,0.3) !important;
    border-right: 4px solid rgba(255,255,255,0.3) !important;
}

.group-start {
    border-left: 5px solid #ffc107 !important;      /* Border kuning tebal */
    background-color: rgba(255, 193, 7, 0.1) !important;  /* Background kuning */
}

.marker-im { 
    border-left: 4px solid #dc3545 !important;      /* Red border lebih tebal */
    padding-left: 8px !important;
}

.marker-re { 
    border-left: 4px solid #17a2b8 !important;      /* Cyan border lebih tebal */
    padding-left: 8px !important;
}
```

---

## 📋 Detail Per Grup

### **Grup 1: Identifikasi Utama (11 kolom)**
1. Checkbox
2. AKSI
3. ID EXCEL
4. NO
5. Machine
6. ITEM List
7. NO. DOC
8. NO. REV
9. REV. DATE
10. FORMULASI
11. MC

---

### **Grup 2: Spesifikasi Hose & Dies (32 kolom)** 🔶 PERBAIKAN TERBESAR
**Kolom Biasa (14):**
- Customer*, Hose Type*, Dimensi, Material*, Inner Tube, Middle Tube, Outer Cover
- Use Limits In, Use Limits Mid, Use Limits Out
- Yarn, Pitch Yarn, Tension In, Tension Out

**Kolom Tolerance Split (18):**
- Nipple (MIN/ASLI/MAX)
- Tube Die (MIN/ASLI/MAX)
- Middle Die (MIN/ASLI/MAX)
- Cover Die (MIN/ASLI/MAX)
- Spacer (MIN/ASLI/MAX)
- A Distance (MIN/ASLI/MAX)

*Marker: IM (red border)*

---

### **Grup 3: Mesh (12 kolom)** 🔶 PERBAIKAN BESAR
- Mesh Screen 1
- Mesh Dim 1 (MIN/ASLI/MAX) → 3 kolom
- Mesh Screen 2
- Mesh Dim 2 (MIN/ASLI/MAX) → 3 kolom
- Mesh Screen 3
- Mesh Dim 3 (MIN/ASLI/MAX) → 3 kolom

---

### **Grup 4: Suhu Pengerjaan °C (45 kolom)** 🔶 PERBAIKAN SANGAT BESAR
**Extruder 1 (15 kolom):**
- Head 1 (MIN/ASLI/MAX)
- Cylinder 1-1 (MIN/ASLI/MAX)
- Cylinder 2-1 (MIN/ASLI/MAX)
- Cylinder 3-1 (MIN/ASLI/MAX)
- Screw Temp 1 (MIN/ASLI/MAX)

**Extruder 2 (15 kolom):**
- Head 2 (MIN/ASLI/MAX)
- Cylinder 1-2, 2-2, 3-2 (masing-masing MIN/ASLI/MAX)
- Screw Temp 2 (MIN/ASLI/MAX)

**Extruder 3 (15 kolom):**
- Head 3 (MIN/ASLI/MAX)
- Cylinder 1-3, 2-3, 3-3 (masing-masing MIN/ASLI/MAX)
- Screw Temp 3 (MIN/ASLI/MAX)

*Marker: RE (cyan border)*

---

### **Grup 5: Output Mesin (49 kolom)** 🔶 PERBAIKAN SANGAT BESAR
**Feed & Speed (33 kolom tolerance):**
- Feed 1/2 (masing-masing MIN/ASLI/MAX)
- Screw Speed 1/2/3 (masing-masing MIN/ASLI/MAX)
- Feed Roll Ratio 1/2 (masing-masing MIN/ASLI/MAX)
- Feed 3 (MIN/ASLI/MAX)

**Pressure (9 kolom tolerance):**
- Pressure 1/2/3 (masing-masing MIN/ASLI/MAX)

**Meter Readings (8 kolom string):**
- Current Value, Am Meter 1/2/3, Preset Value, Control Value, Sp-Setting, Sp-Display

**Speed (6 kolom tolerance):**
- Spiral Speed (MIN/ASLI/MAX)
- Hose Speed (MIN/ASLI/MAX)

**Sensor (2 kolom):**
- Unsmooth Surface, OD Sensor

---

### **Grup 6: Proses Akhir (17 kolom)** 🔶 PERBAIKAN BESAR
**Cooling & Gap (9 kolom tolerance):**
- Chiller Water Temp (MIN/ASLI/MAX)
- Caterpillar Gap (MIN/ASLI/MAX)
- Take-up Conveyor Speed (MIN/ASLI/MAX)

**Controls (5 kolom string):**
- Dancer Position, Cutting Speed, Cool Speed 1/2, Conveyor Ratio

**Marking (3 kolom):**
- Marking Sort, Text Marking Material, Marking Colour

---

### **Grup 7: Final Quality Matrix (39 kolom)** 🔶 PERBAIKAN BESAR
**Tolerance Fields (2):** ± Inner, ± Outer

**Thickness Measurements (12 kolom tolerance):**
- Tebal Inner (MIN/ASLI/MAX)
- Inner+Middle (MIN/ASLI/MAX)
- Tebal Outer (MIN/ASLI/MAX)
- Total Thickness (MIN/ASLI/MAX)

**Selisih (1):** Selisih Tebal

**Matrix Data (24 kolom - 4 sections × 6 metrics):**
- Inner: Target, Tol, LCL, Min, UCL, Max
- Inner Mid: Target, Tol, LCL, Min, UCL, Max (cyan text)
- Thickness: Target, Tol, LCL, Min, UCL, Max
- Total: Target, Tol, LCL, Min, UCL, Max

---

## ✅ Build Status
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

---

## 🚀 Test Sekarang

```powershell
dotnet run
```

Buka: **http://localhost:5000/SpsMaster**

**Yang akan terlihat:**
1. ✅ Header grup tidak terpotong lagi
2. ✅ Border kuning tebal menandai awal tiap grup
3. ✅ Marker merah (IM) dan cyan (RE) lebih jelas
4. ✅ Scroll horizontal smooth untuk 206 kolom
5. ✅ Group header dengan warna biru/orange bergantian

---

## 🎨 Visual Scheme

```
[BIRU]    Identifikasi Utama (11)
[ORANGE]  Spesifikasi Hose & Dies (32) ← BORDER KUNING DI KOLOM CUSTOMER
[BIRU]    Mesh (12) ← BORDER KUNING DI MESH SCREEN 1
[ORANGE]  Suhu Pengerjaan (45) ← BORDER KUNING DI HEAD 1
[BIRU]    Output Mesin (49) ← BORDER KUNING DI FEED 1
[ORANGE]  Proses Akhir (17) ← BORDER KUNING DI CHILLER
[BIRU]    Final Quality Matrix (39) ← BORDER KUNING DI ± INNER
[PUTIH]   Opsi (1)
```

Silakan test dan screenshot hasil barunya! 🎉
