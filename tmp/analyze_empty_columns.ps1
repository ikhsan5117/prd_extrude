$serverName = "10.14.149.34"
$databaseName = "prd_extrude_hose"
$username = "usrvelasto"
$password = "H1s@na2025!!"

$connectionString = "Server=$serverName;Database=$databaseName;User Id=$username;Password=$password;TrustServerCertificate=True;Connection Timeout=300;"

$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$connection.Open()

Write-Host "Analyzing SpsMasters table for empty columns..." -ForegroundColor Cyan
Write-Host ""

# Get all column names
$columnsQuery = @"
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SpsMasters' 
AND COLUMN_NAME NOT IN ('Id', 'CreatedAt', 'UpdatedAt')
ORDER BY ORDINAL_POSITION
"@

$columnsCmd = $connection.CreateCommand()
$columnsCmd.CommandText = $columnsQuery
$reader = $columnsCmd.ExecuteReader()

$columns = @()
while ($reader.Read()) {
    $columns += $reader["COLUMN_NAME"]
}
$reader.Close()

Write-Host "Total columns to analyze: $($columns.Count)" -ForegroundColor Yellow
Write-Host ""

$emptyColumns = @()
$nonEmptyColumns = @()

foreach ($col in $columns) {
    $countQuery = "SELECT COUNT(*) as cnt FROM SpsMasters WHERE [$col] IS NOT NULL"
    $countCmd = $connection.CreateCommand()
    $countCmd.CommandText = $countQuery
    $count = [int]$countCmd.ExecuteScalar()
    
    if ($count -eq 0) {
        $emptyColumns += $col
        Write-Host "[EMPTY] $col" -ForegroundColor Red
    } else {
        $nonEmptyColumns += $col
        Write-Host "[HAS DATA] $col (count: $count)" -ForegroundColor Green
    }
}

$connection.Close()

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "SUMMARY" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Total columns: $($columns.Count)" -ForegroundColor White
Write-Host "Columns with data: $($nonEmptyColumns.Count)" -ForegroundColor Green
Write-Host "Empty columns: $($emptyColumns.Count)" -ForegroundColor Red
Write-Host ""

if ($emptyColumns.Count -gt 0) {
    Write-Host "Empty columns list:" -ForegroundColor Yellow
    $emptyColumns | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
}

# Save to file
$emptyColumns | Out-File -FilePath "empty_columns.txt" -Encoding UTF8
Write-Host ""
Write-Host "Empty columns saved to: empty_columns.txt" -ForegroundColor Cyan
