$connectionString = "Server=10.14.149.34;Database=prd_extrude_hose;User Id=usrvelasto;Password=H1s@na2025!!;TrustServerCertificate=True"
$sqlFile = "D:\extrude\tmp\add_tolerance_columns_batch2.sql"

Write-Host "Executing SQL migration: Batch 2 (39 tolerance columns)..." -ForegroundColor Yellow

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    Write-Host "Connected to database." -ForegroundColor Green
    
    $sql = Get-Content $sqlFile -Raw
    $command = $connection.CreateCommand()
    $command.CommandText = $sql
    $command.CommandTimeout = 300
    
    Write-Host "Executing migration..." -ForegroundColor Cyan
    $result = $command.ExecuteNonQuery()
    
    Write-Host "`n[SUCCESS] Migration executed!" -ForegroundColor Green
    Write-Host "Added 39 new tolerance columns to SpsMasters table." -ForegroundColor Cyan
    Write-Host "Total tolerance columns now: 174 (58 fields x 3)" -ForegroundColor Cyan
    
    $connection.Close()
}
catch {
    Write-Host "`n[ERROR] Migration failed:" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    exit 1
}
