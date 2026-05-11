$serverName = "10.14.149.34"
$databaseName = "prd_extrude_hose"
$username = "usrvelasto"
$password = "H1s@na2025!!"

$connectionString = "Server=$serverName;Database=$databaseName;User Id=$username;Password=$password;TrustServerCertificate=True;Connection Timeout=300;"

$sqlScript = Get-Content -Path "add_feed3_tolerance.sql" -Raw

try {
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    Write-Host "Connected to database." -ForegroundColor Green
    
    $command = $connection.CreateCommand()
    $command.CommandText = $sqlScript
    $command.CommandTimeout = 300
    
    Write-Host "Adding Feed3 tolerance columns..." -ForegroundColor Yellow
    $command.ExecuteNonQuery() | Out-Null
    
    Write-Host "[SUCCESS] Columns added!" -ForegroundColor Green
    Write-Host "Added Feed3_Min, Feed3_Asli, Feed3_Max" -ForegroundColor Cyan
    Write-Host "Total tolerance columns now: 187 (62 fields x 3 + Feed3 x 3)" -ForegroundColor Cyan
    
    $connection.Close()
}
catch {
    Write-Host "[ERROR] Failed: $_" -ForegroundColor Red
    exit 1
}
