$connString = 'Server=10.14.149.34,1433;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True;MultipleActiveResultSets=True;Encrypt=False;Connect Timeout=30;'
$conn = New-Object System.Data.SqlClient.SqlConnection($connString)
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT DISTINCT UPPER(LTRIM(RTRIM(ItemList))) as ItemList FROM SpsItemLists WHERE ItemList IS NOT NULL AND ItemList <> ''"
$r = $cmd.ExecuteReader()
$dbItems = @{}
while($r.Read()) { $dbItems[$r['ItemList'].ToString()] = 1 }
$r.Close()
$conn.Close()
Write-Host "Total item unik di DB: $($dbItems.Count)"

# Baca Excel files menggunakan COM
$excel = New-Object -ComObject Excel.Application
$excel.Visible = $false
$excel.DisplayAlerts = $false

$excelFiles = @(
    "D:\extrude\Masterlist SPS Double Layer.xlsx",
    "D:\extrude\Masterlist SPS CHS 2 Layer DIG.xlsx",
    "D:\extrude\Masterlist SPS CHS 3 Layer DIG.xlsx",
    "D:\extrude\Masterlist SPS Double Layer_Digitalisasi.xlsx"
)

$allExcelItems = @{}
$missingItems = [System.Collections.ArrayList]::new()

foreach ($file in $excelFiles) {
    if (-not (Test-Path $file)) { Write-Host "File tidak ada: $file"; continue }

    $wb = $excel.Workbooks.Open($file)
    $ws = $wb.Worksheets.Item(1)
    $usedRange = $ws.UsedRange
    $rowCount = $usedRange.Rows.Count
    $colCount = $usedRange.Columns.Count
    $fname = [System.IO.Path]::GetFileName($file)

    # Cari kolom item code dari header row 1
    $itemCol = -1
    for ($c = 1; $c -le [Math]::Min($colCount, 20); $c++) {
        $cellVal = $ws.Cells.Item(1, $c).Value2
        if ($cellVal) {
            $cv = $cellVal.ToString().ToUpper()
            if ($cv -match "ITEM|KODE|HOSE|PART|TYPE") {
                $itemCol = $c
                Write-Host "[$fname] Kolom item: '$cellVal' (col $c)"
                break
            }
        }
    }

    if ($itemCol -eq -1) {
        Write-Host "[$fname] Header tidak ditemukan. Menampilkan 10 header pertama:"
        for ($c = 1; $c -le [Math]::Min($colCount, 10); $c++) {
            $v = $ws.Cells.Item(1, $c).Value2
            if ($v) { Write-Host "  Col ${c}: $v" }
        }
    } else {
        $fileCount = 0
        for ($row = 2; $row -le $rowCount; $row++) {
            $val = $ws.Cells.Item($row, $itemCol).Value2
            if ($val -and $val.ToString().Trim() -ne "") {
                $code = $val.ToString().Trim().ToUpper()
                $allExcelItems[$code] = $fname
                if (-not $dbItems.ContainsKey($code)) {
                    [void]$missingItems.Add([PSCustomObject]@{ ItemCode = $code; SourceFile = $fname })
                }
                $fileCount++
            }
        }
        Write-Host "[$fname] Total item: $fileCount"
    }

    $wb.Close($false)
}

$excel.Quit()
[System.Runtime.InteropServices.Marshal]::ReleaseComObject($excel) | Out-Null

Write-Host ""
Write-Host "===== HASIL PERBANDINGAN ====="
Write-Host "Total item di semua Excel: $($allExcelItems.Count)"
Write-Host "Item BELUM ADA di DB    : $($missingItems.Count)"
Write-Host ""
if ($missingItems.Count -gt 0) {
    $missingItems | Sort-Object ItemCode | Format-Table -AutoSize
    # Simpan ke file
    $missingItems | Sort-Object ItemCode | Export-Csv -Path "D:\extrude\scratch\missing_from_db.csv" -NoTypeInformation
    Write-Host "Hasil disimpan ke: D:\extrude\scratch\missing_from_db.csv"
}
