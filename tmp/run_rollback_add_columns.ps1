$serverName = "10.14.149.34"
$databaseName = "prd_extrude_hose"
$username = "usrvelasto"
$password = "H1s@na2025!!"

$connectionString = "Server=$serverName;Database=$databaseName;User Id=$username;Password=$password;TrustServerCertificate=True;Connection Timeout=300;"

$sqlScript = Get-Content -Path "rollback_add_columns.sql" -Raw

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    Write-Host "Connected to database." -ForegroundColor Green
    
    $command = $connection.CreateCommand()
    $command.CommandText = $sqlScript
    $command.CommandTimeout = 300
    
    Write-Host "Executing rollback migration..." -ForegroundColor Yellow
    $command.ExecuteNonQuery() | Out-Null
    
    Write-Host "[SUCCESS] Rollback executed!" -ForegroundColor Green
    Write-Host "Added back 24 tolerance columns to SpsMasters table." -ForegroundColor Cyan
    Write-Host "Total tolerance columns now: 184 (61 fields x 3, minus Feed3 x 3)" -ForegroundColor Cyan
    
    $connection.Close()
}
catch {
    Write-Host "[ERROR] Rollback failed: $_" -ForegroundColor Red
    exit 1
}
